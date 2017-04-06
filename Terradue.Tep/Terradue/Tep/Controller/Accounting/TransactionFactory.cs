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
            var accoutings = GetAccountingListFromRemote(user.Username, lastTransaction != null ? lastTransaction.LogTime : DateTime.MinValue);

            foreach (var accounting in accoutings) {
                if (accounting.account.username != null && accounting.account.username.Equals(user.Username)) {
                    var r = new System.Text.RegularExpressions.Regex(@"^(?<platform>[a-zA-Z0-9_\-]+)-(?<type>[a-zA-Z0-9_\-]+)-(?<reference>[a-zA-Z0-9_\-]+)");
                    var m = r.Match(accounting.account.reference);
                    if (!m.Success) continue;

                    //get referenced entity
                    var type = m.Result("${type}");
                    var identifier = m.Result("${reference}");
                    Entity entityservice = null;
                    Entity entityself = null;
                    switch (type) {
                    case "wpsjob":
                        var job = WpsJob.FromIdentifier(context, identifier);
                        entityself = job;
                        entityservice = job.Process;
                        break;
                    case "datapackage":
                        var dp = DataPackage.FromIdentifier(context, identifier);
                        entityself = dp;
                        entityservice = dp;
                        break;
                    default:
                        break;
                    }

                    if (entityservice == null) continue;//TODO: remove when handling cases with no refs

                    //get balance
                    double balance = Rates.GetBalanceFromRates(context, entityservice, accounting.quantity);
                       
                    var transaction = new Transaction(context);
                    transaction.Entity = entityself;
                    transaction.OwnerId = user.Id;
                    transaction.Identifier = identifier;
                    transaction.LogTime = accounting.timestamp;
                    transaction.ProviderId = entityservice.OwnerId;
                    transaction.Balance = balance;
                    transaction.Kind = TransactionKind.Debit;
                    transaction.Store();
                }
            }
        }

        private List<T2Accounting> GetAccountingListFromRemote(string username, DateTime timestamp) {
            List<T2Accounting> accoutings = new List<T2Accounting>();
            return accoutings;//TODO: remove
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/{1}/{2}{3}",
                                                                                     context.GetConfigValue("t2-accounting-baseurl"),
                                                                                     context.GetConfigValue("SiteName").Replace(" ",""),
                                                                                     username,
                                                                                     timestamp != DateTime.MinValue ? "?startDate=" + timestamp.ToString("s") : ""));
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Proxy = null;

            try {
                using (var httpResponse = (HttpWebResponse)request.GetResponse()) {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                        string result = streamReader.ReadToEnd();
                        accoutings = JsonSerializer.DeserializeFromString<List<T2Accounting>>(result);
                    }
                }
            } catch (Exception e) {
                throw e;
            }
            return accoutings;
        }
    }
}
