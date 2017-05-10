﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ServiceStack.Text;
using Terradue.Portal;

namespace Terradue.Tep {
    public class TransactionFactory {
        private IfyContext context;

        public TransactionFactory(IfyContext context) {
            this.context = context;
        }

        /// <summary>
        /// Gets the user transactions.
        /// </summary>
        /// <returns>The user transactions.</returns>
        /// <param name="usrid">Usrid.</param>
        /// <param name="withref">If set to <c>true</c> with referenced items.</param>
        /// <param name="withnoref">If set to <c>true</c> with unreferenced items.</param>
        public List<Transaction> GetUserTransactions(int usrid, bool withref = true, bool withnoref = true) {
            EntityList<Transaction> transactions = new EntityList<Transaction>(context);
            if (!withref && !withnoref) return new List<Transaction>();

            transactions.SetFilter("OwnerId", usrid + "");
            if (!withref || !withnoref) {
                if (!withref) transactions.SetFilter("Identifier", SpecialSearchValue.Null);
                else if (!withnoref) transactions.SetFilter("Identifier", SpecialSearchValue.NotNull);
            }
            transactions.Load();
            return transactions.GetItemsAsList();
        }

        /// <summary>
        /// Gets the user last transaction.
        /// </summary>
        /// <returns>The user last transaction.</returns>
        /// <param name="usrid">Usrid.</param>
        public Transaction GetUserLastTransaction(int usrid) {
            EntityList<Transaction> transactions = new EntityList<Transaction>(context);
            transactions.SetFilter("OwnerId", usrid + "");
            transactions.AddSort("LogTime", SortDirection.Descending);
            transactions.ItemsPerPage = 1;
            transactions.Load();
            var items = transactions.GetItemsAsList();
            return items.Count > 0 ? items[0] : null;
        }

        /// <summary>
        /// Gets the transactions associated to a reference.
        /// </summary>
        /// <returns>The transactions by reference.</returns>
        /// <param name="reference">Reference.</param>
        public List<Transaction> GetTransactionsByReference(string reference) {
            EntityList<Transaction> transactions = new EntityList<Transaction>(context);
            transactions.SetFilter("Identifier", reference);
            transactions.Load();
            return transactions.GetItemsAsList();
        }

        /// <summary>
        /// Gets the deposit transaction.
        /// </summary>
        /// <returns>The deposit transaction.</returns>
        /// <param name="reference">Reference.</param>
        public Transaction GetDepositTransaction(string reference) {
            EntityList<Transaction> transactions = new EntityList<Transaction>(context);
            transactions.SetFilter("Identifier", reference);
            transactions.SetFilter("Kind", (int)TransactionKind.ActiveDeposit + "," + (int)TransactionKind.ResolvedDeposit);
            transactions.Load();
            var items = transactions.GetItemsAsList();
            return items.Count > 0 ? items[0] : null;
        }

        public List<AggregatedTransaction> GetUserAggregatedTransaction(int usrId) {
            var result = new List<AggregatedTransaction>();
            //get all transactions without reference
            List<Transaction> transactions = GetUserTransactions(usrId, false, true);
            foreach (var transaction in transactions) result.Add(new AggregatedTransaction(context, transaction));

            //get all references
            var references = GetTransactionsReferences(usrId);

            //foreach reference, calculate the balance (using deposit)
            foreach (var reference in references) {
                var deposit = GetDepositTransaction(reference);
                double cost = 0;
                var reftransactions = GetTransactionsByReference(reference);
                foreach (var reftransaction in reftransactions) {
                    if (!reftransaction.IsDeposit()) cost += reftransaction.Balance;
                }
                var agg = new AggregatedTransaction(context, deposit);
                agg.Identifier = reference;
                agg.Deposit = deposit.Balance;
                if(cost != deposit.Balance) agg.RealCost = cost;
                agg.Balance = deposit.Balance;
                agg.Kind = deposit.Kind;
                agg.LogTime = deposit.LogTime;
                result.Add(agg);
            }
            result.Sort();
            result.Reverse();
            return result;
        }

        /// <summary>
        /// Gets the user balance.
        /// </summary>
        /// <returns>The user balance.</returns>
        /// <param name="user">User.</param>
        public double GetUserBalance(User user) {
            double balance = 0;
            try {

                //first we sync transactions
                SyncTransactions(user);

                //get all transactions without reference
                List<Transaction> transactions = GetUserTransactions(user.Id, false, true);
                foreach (var transaction in transactions) balance += transaction.GetTransactionBalance();

                //get all references
                var references = GetTransactionsReferences(user.Id);

                //foreach reference, calculate the balance (using deposit)
                foreach (var reference in references) {
                    var deposit = GetDepositTransaction(reference);
                    double refBalance = 0;
                    var reftransactions = GetTransactionsByReference(reference);
                    foreach (var reftransaction in reftransactions) {
                        if (!reftransaction.IsDeposit()) refBalance += reftransaction.Balance;
                    }
                    balance += GetBalanceWithDeposit(refBalance, deposit);
                }
            } catch (Exception e) {
                context.LogError(this, e.Message + "-" + e.StackTrace);
                balance = 0;
            }

            return balance;
        }

        private double GetBalanceWithDeposit(double balance, Transaction deposit) {
            //TODO: get policy and do calculation according to policy
            //default is we pay what we used, and never more than the deposit

            //if transaction is resolved, we return the minimum between the sum of transactions and the deposit
            if (deposit.Kind == TransactionKind.ResolvedDeposit) return -Math.Min(balance, deposit.Balance);
            //otherwise we return the value of the deposit and the sum of transactions is not taken into account
            else return deposit.GetTransactionBalance();
        }

        /// <summary>
        /// Gets the transactions references.
        /// </summary>
        /// <returns>The transactions references.</returns>
        /// <param name="userid">Userid.</param>
        private List<string> GetTransactionsReferences(int userid) {
            List<string> references = new List<string>();

            string sql = string.Format("SELECT DISTINCT reference FROM transaction WHERE id_usr={0} AND reference IS NOT NULL;", userid);
            var dbConnection = context.GetDbConnection();
            var reader = context.GetQueryResult(sql, dbConnection);
            while (reader.Read()) {
                references.Add(reader.GetString(0));
            }
            context.CloseQueryResult(reader, dbConnection);

            return references;
        }

        /// <summary>
        /// Syncs the transactions.
        /// </summary>
        /// <returns>The new transactions.</returns>
        /// <param name="context">Context.</param>
        /// <param name="user">User.</param>
        public void SyncTransactions(User user) {

            var lastTransaction = GetUserLastTransaction(user.Id);

            //get transactions from remote accounting server
            var transactions = GetRemoteTransactions(user.Username, lastTransaction != null ? lastTransaction.LogTime : DateTime.MinValue);

            foreach (var transaction in transactions) {
                //if (accounting.account.username != null && accounting.account.username.Equals(user.Username)) {
                //    var r = new System.Text.RegularExpressions.Regex(@"^(?<platform>[a-zA-Z0-9_\-]+)-(?<type>[a-zA-Z0-9_\-]+)-(?<reference>[a-zA-Z0-9_\-]+)");
                //    var m = r.Match(accounting.account.reference);
                //    if (!m.Success) continue;

                //    //get referenced entity
                //    var type = m.Result("${type}");
                //    var identifier = m.Result("${reference}");
                //    Entity entityservice = null;
                //    Entity entityself = null;
                //    switch (type) {
                //    case "wpsjob":
                //        var job = WpsJob.FromIdentifier(context, identifier);
                //        entityself = job;
                //        entityservice = job.Process;
                //        break;
                //    case "datapackage":
                //        var dp = DataPackage.FromIdentifier(context, identifier);
                //        entityself = dp;
                //        entityservice = dp;
                //        break;
                //    default:
                //        break;
                //    }

                //    if (entityservice == null) continue;//TODO: remove when handling cases with no refs

                //    //get balance
                //    double balance = Rates.GetBalanceFromRates(context, entityservice, accounting.quantity);

                //    var transaction = new Transaction(context);
                //    transaction.Entity = entityself;
                //    transaction.OwnerId = user.Id;
                //    transaction.Identifier = identifier;
                //    transaction.LogTime = accounting.timestamp;
                //    transaction.ProviderId = entityservice.OwnerId;
                //    transaction.Balance = balance;
                //    transaction.Kind = TransactionKind.Debit;
                //    transaction.Store();
                //}
                transaction.Store();
            }
        }

        private ElasticTransactionSearchRequest GetTransactionJsonToPost(string username, DateTime timestamp) {
            ElasticTransactionSearchRequest json = new ElasticTransactionSearchRequest {
                size = 0,
                query = new ETQuery {
                    constant_score = new ETConstantScore {
                        filter = new ETFilter {
                            etbool = new ETBool {
                                must = new List<ETMust> {
                                    new ETMust{
                                        range = new ETRange{
                                            //timestamp = new ETTimestamp{ from = timestamp.ToString("s"), to = "now" }
                                            timestamp = new ETTimestamp { from = "now/1d-2M", to = "now/1d" }
                                        }
                                    },
                                    new ETMust{
                                        term = new ETTerm{ accountUserName = "tep_" +username }
                                    }
                                }
                            }
                        }
                    }
                },
                aggs = new ETAggs {
                    user = new ETUser {
                        terms = new ETTerms { field = "account.userName" },
                        aggs = new ETAggs2 {
                            account_ref = new ETAccountRef {
                                terms = new ETTerms { field = "account.ref" },
                                aggs = new ETAggs3 {
                                    quantities = new ETQuantities {
                                        terms = new ETTerms { field = "quantity.id" },
                                        aggs = new ETAggs4 {
                                            total = new ETTotal {
                                                sum = new ETTerms { field = "quantity.value" }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return json;
        }

        private List<Transaction> GetRemoteTransactions(string username, DateTime timestamp) {
            List<Transaction> transactions = new List<Transaction>();

            var request = (HttpWebRequest)WebRequest.Create(context.GetConfigValue("t2-accounting-baseurl"));
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Proxy = null;

            var estr = GetTransactionJsonToPost(username, timestamp);
            string json = JsonSerializer.SerializeToString<ElasticTransactionSearchRequest>(estr);

            try {
                using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    using (var httpResponse = (HttpWebResponse)request.GetResponse()) {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                            string result = streamReader.ReadToEnd();
                            var etResponse = JsonSerializer.DeserializeFromString<ElasticTransactionSearchResponse>(result);
                            var userBuckets = etResponse.aggregations.user.buckets[0].account_ref.buckets;
                            foreach (var bucket in userBuckets) {
                                var identifier = bucket.key;

                                //get job and service
                                var job = WpsJob.FromIdentifier(context, identifier);
                                var entityservice = job.Process;
                                double balance = 0;
                                foreach (var qBucket in bucket.quantities.buckets) {
                                    //calculate balance
                                    if (!string.IsNullOrEmpty(qBucket.key) && qBucket.total != null) {
                                        balance += Rates.GetBalanceFromRate(context, entityservice, qBucket.key, qBucket.total.value);
                                    }
                                }
                                var transaction = new Transaction(context);
                                transaction.Entity = job;
                                transaction.OwnerId = job.OwnerId;
                                transaction.Identifier = identifier;
                                transaction.LogTime = DateTime.UtcNow;
                                transaction.ProviderId = entityservice.OwnerId;
                                transaction.Balance = balance;
                                transaction.Kind = TransactionKind.Debit;
                                transactions.Add(transaction);
                            }
                        }
                    }
                }
            } catch (Exception e) {
                context.LogError(this, e.Message);
            }
            return transactions;
        }

    }
}
