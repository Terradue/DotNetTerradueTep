﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using Terradue.OpenSearch;
using Terradue.OpenSearch.Engine;
using Terradue.OpenSearch.Result;
using Terradue.Portal;
using Terradue.Tep.WebServer;
using Terradue.WebService.Model;

namespace Terradue.Tep.WebServer.Services {

    [Route("/share", "POST", Summary = "share an entity", Notes = "")]
    public class ShareCreateRequestTep : IReturn<WebResponseBool>{
        [ApiMember(Name="self", Description = "url representing the item shared", ParameterType = "query", DataType = "string", IsRequired = true)]
        public string self { get; set; }

        [ApiMember(Name = "to", Description = "url(s) representing the item to which the entity is shared", ParameterType = "query", DataType = "List<string>", IsRequired = true)]
        public List<string> to { get; set; }

        [ApiMember(Name="id", Description = "thematic application id", ParameterType = "query", DataType = "string", IsRequired = true)]
        public string id { get; set; }

        [ApiMember(Name = "publish", Description = "publish or not the shared item to community index (if applicable)", ParameterType = "query", DataType = "bool", IsRequired = false)]
        public bool publish { get; set; }
    }

    [Route("/share", "DELETE", Summary = "share an entity", Notes = "")]
    public class ShareDeleteRequestTep : IReturn<WebResponseBool> {
        [ApiMember(Name = "self", Description = "url representing the item shared", ParameterType = "query", DataType = "string", IsRequired = true)]
        public string self { get; set; }
    }

    [Route("/share", "GET", Summary = "share an entity", Notes = "")]
    public class ShareGetRequestTep {
        [ApiMember(Name="url", Description = "url representing the item shared", ParameterType = "query", DataType = "string", IsRequired = true)]
        public string url { get; set; }

        [ApiMember(Name="id", Description = "thematic applicaiton id", ParameterType = "query", DataType = "string", IsRequired = true)]
        public string id { get; set; }
    }

     [Api("Tep Terradue webserver")]
    [Restrict(EndpointAttributes.InSecure | EndpointAttributes.InternalNetworkAccess | EndpointAttributes.Json,
              EndpointAttributes.Secure   | EndpointAttributes.External | EndpointAttributes.Json)]
    public class ShareServiceTep : ServiceStack.ServiceInterface.Service {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public object Delete(ShareDeleteRequestTep request) {
            var context = TepWebContext.GetWebContext(PagePrivileges.UserView);
            context.Open();
            context.LogInfo(this, string.Format("/share DELETE self='{0}'", request.self));

            var settings = MasterCatalogue.OpenSearchFactorySettings;
            var entitySelf = new UrlBasedOpenSearchable(context, new OpenSearchUrl(request.self), settings).Entity;

            if (entitySelf is EntityList<WpsJob>) {
                var entitylist = entitySelf as EntityList<WpsJob>;
                var items = entitylist.GetItemsAsList();
                if (items.Count > 0) {
                    foreach (var item in items) {
                        item.RevokePermissionsFromAll(true, false);
                        if (item.Owner != null && item.DomainId != item.Owner.DomainId) {
                            item.DomainId = item.Owner.DomainId;
                            item.Store();
                        }

                        //share on store
                        try {
                            DataGatewayFactory.ShareOnStore(context.GetConfigValue("SiteName"),item.StatusLocation, "results", "private");
                        }catch(Exception e){
                            context.LogError(this, "Unable to share on STORE : " + e.Message);
                        }

                        //unpublish on community index
                        try {
                            if (string.IsNullOrEmpty(item.Identifier)) throw new Exception("Invalid job identifier");
                            var index = context.GetConfigValue("catalog-communityIndex");
                            //var username = context.GetConfigValue("catalog-admin-username");
                            //var apikey = context.GetConfigValue("catalog-admin-apikey");
                            var user = UserTep.FromId(context, context.UserId);
                            CatalogueFactory.DeleteEntryFromIndex(context, index, item.Identifier, user.Username, user.GetSessionApiKey());
                        } catch (Exception e) {
                            context.LogError(this, "Unable to publish on catalog community index : " + e.Message);
                        }
                    }
                }
            } else if (entitySelf is EntityList<DataPackage>) {
                var entitylist = entitySelf as EntityList<DataPackage>;
                var items = entitylist.GetItemsAsList();
                if (items.Count > 0) {
                    foreach (var item in items) {
                        item.RevokePermissionsFromAll(true, false);
                        if (item.Owner != null && item.DomainId != item.Owner.DomainId) {
                            item.DomainId = item.Owner.DomainId;
                            item.Store();
                        }
                    }
                }
            }

            context.Close();
            return new WebResponseBool(true);
        }

        public object Post(ShareCreateRequestTep request) {
            var context = TepWebContext.GetWebContext(PagePrivileges.UserView);
            context.Open();
            context.LogInfo(this,string.Format("/share POST self='{0}',to='{1}'", request.self, request.to != null ? string.Join("", request.to) : ""));
                            
            var settings = MasterCatalogue.OpenSearchFactorySettings;
            var entitySelf = new UrlBasedOpenSearchable(context, new OpenSearchUrl(request.self), settings).Entity;

            //case WpsJob
            if (entitySelf is EntityList<WpsJob>) {
                var entitylist = entitySelf as EntityList<WpsJob>;
                var wpsjobs = entitylist.GetItemsAsList();
                if (wpsjobs.Count == 0) return new WebResponseBool(false);

                //if to is null, we share publicly
                if (request.to == null) {
                    foreach (var job in wpsjobs) { //the entitySelf can return several entities
                        job.GrantPermissionsToAll();

						//share on store
						try {
							DataGatewayFactory.ShareOnStore(context.GetConfigValue("SiteName"), job.StatusLocation, "results", "public");
						} catch (Exception e) {
							context.LogError(this, "Unable to share on STORE : " + e.Message);
						}

                        //publish on community index
                        if (request.publish) {
                            try {
                                var feed = job.GetJobAtomFeedFromOwsUrl();
                                if (feed != null) {
                                    var index = context.GetConfigValue("catalog-communityIndex");
                                    //var username = context.GetConfigValue("catalog-admin-username");
                                    //var apikey = context.GetConfigValue("catalog-admin-apikey");
                                    var user = UserTep.FromId(context, context.UserId);
                                    CatalogueFactory.PostAtomFeedToIndex(context, feed, index, user.Username, user.GetSessionApiKey());
                                } else {
                                    context.LogError(this, "Unable to publish on catalog community index : feed is empty");
                                }
                            } catch (Exception e) {
                                context.LogError(this, "Unable to publish on catalog community index : " + e.Message);
                            }
                        }

                        Activity activity = new Activity(context, job, EntityOperationType.Share);
                        activity.Store();
                    }
                }

                //we share with restriction (community + users)
                else {
                    foreach (var job in wpsjobs) { //the entitySelf can return several entities

                        //remove previous visibility sharing
                        job.RevokePermissionsFromAll(true, false);
                        if (job.Owner != null && job.DomainId != job.Owner.DomainId) {
                            job.DomainId = job.Owner.DomainId;
                            job.Store();
                        }

                        var sharedUsers = new List<string>();
                        var sharedCommunities = new List<ThematicCommunity>();

                        foreach (var to in request.to) {
                            var entityTo = new UrlBasedOpenSearchable(context, new OpenSearchUrl(to), settings).Entity;

                            //case community
                            if (entityTo is EntityList<ThematicCommunity>) {
                                var entityTolist = entityTo as EntityList<ThematicCommunity>;
                                var communities = entityTolist.GetItemsAsList();
                                if (communities.Count == 0) return new WebResponseBool(false);

                                var community = communities[0];

                                //the entitySelflist can return several entities but we only take the first one (we can share with only one community)
                                community.ShareEntity(job);
                                job.DomainId = community.Id;
                                job.Store();

                                sharedCommunities.Add(community);

                                ActivityTep activity = new ActivityTep(context, job, EntityOperationType.Share);
                                activity.AppId = request.id;
                                activity.DomainId = communities[0].Id;
                                activity.Store();
                            }

                            //case user
                            else if (entityTo is EntityList<UserTep>) {
                                var entityTolist = entityTo as EntityList<UserTep>;
                                var users = entityTolist.GetItemsAsList();
                                if (users.Count == 0) return new WebResponseBool(false);
                                job.GrantPermissionsToUsers(users);

                                foreach (var usr in users) {
                                    if (string.IsNullOrEmpty(usr.TerradueCloudUsername)) usr.LoadCloudUsername();
                                    if (!string.IsNullOrEmpty(usr.TerradueCloudUsername)) sharedUsers.Add(usr.TerradueCloudUsername);
                                }

                                ActivityTep activity = new ActivityTep(context, job, EntityOperationType.Share);
                                activity.AppId = request.id;
                                activity.Store();
                            }
                        }

						//share on store
						try {
							DataGatewayFactory.ShareOnStore(context.GetConfigValue("SiteName"), job.StatusLocation, "results", "restricted", sharedUsers, sharedCommunities);
						} catch (Exception e) {
							context.LogError(this, "Unable to share on STORE : " + e.Message);
						}

                        //unpublish on community index
                        try {
                            if (string.IsNullOrEmpty(job.Identifier)) throw new Exception("Invalid job identifier");
                            var index = context.GetConfigValue("catalog-communityIndex");
                            var username = context.GetConfigValue("catalog-admin-username");
                            var apikey = context.GetConfigValue("catalog-admin-apikey");
                            CatalogueFactory.DeleteEntryFromIndex(context, index, job.Identifier, username, apikey);
                        } catch (Exception e) {
                            context.LogError(this, "Unable to publish on catalog community index : " + e.Message);
                        }
                    }
                }
            }

            //case DataPackage
            else if (entitySelf is EntityList<DataPackage>) {
                var entitylist = entitySelf as EntityList<DataPackage>;
                var datapackages = entitylist.GetItemsAsList();
                if (datapackages.Count == 0) return new WebResponseBool(false);

                //if to is null, we share publicly
                if (request.to == null) {
                    foreach (var dp in datapackages) { //the entitySelf can return several entities
                        dp.GrantPermissionsToAll();

                        Activity activity = new Activity(context, dp, EntityOperationType.Share);
                        activity.Store();
                    }
                }

                //we share with restriction (community + users)
                else {
                    foreach (var dp in datapackages) { //the entitySelf can return several entities
                        //remove previous visibility sharing
                        dp.RevokePermissionsFromAll(true, false);
                        if (dp.Owner != null && dp.DomainId != dp.Owner.DomainId) {
                            dp.DomainId = dp.Owner.DomainId;
                            dp.Store();
                        }

                        foreach (var to in request.to) {
                            var entityTo = new UrlBasedOpenSearchable(context, new OpenSearchUrl(to), settings).Entity;

                            //case community
                            if (entityTo is EntityList<ThematicCommunity>) {
                                var entityTolist = entityTo as EntityList<ThematicCommunity>;
                                var communities = entityTolist.GetItemsAsList();
                                if (communities.Count == 0) return new WebResponseBool(false);
                                //the entitySelflist can return several entities but we only take the first one (we can share with only one community)
                                dp.DomainId = communities[0].Id;
                                dp.Store();

                                ActivityTep activity = new ActivityTep(context, dp, EntityOperationType.Share);
                                activity.AppId = request.id;
                                activity.DomainId = communities[0].Id;
                                activity.Store();
                            }

                            //case user
                            else if (entityTo is EntityList<UserTep>) {
                                var entityTolist = entityTo as EntityList<UserTep>;
                                var users = entityTolist.GetItemsAsList();
                                if (users.Count == 0) return new WebResponseBool(false);
                                dp.GrantPermissionsToUsers(users);

                                ActivityTep activity = new ActivityTep(context, dp, EntityOperationType.Share);
                                activity.AppId = request.id;
                                activity.Store();
                            }
                        }
                    }
                }

            }

            context.Close ();
            return new WebResponseBool(true);
        }

        public object Get(ShareGetRequestTep request) {
            var context = TepWebContext.GetWebContext(PagePrivileges.EverybodyView);
            context.Open();
            context.LogInfo(this,string.Format("/share GET url='{0}'", request.url));

            var AppSettings = System.Configuration.ConfigurationManager.AppSettings;

            var redirect = new UriBuilder(context.BaseUrl);
            redirect.Path = "geobrowser";
            string redirectUrl = redirect.Uri.AbsoluteUri + (!string.IsNullOrEmpty (request.id) ? "/?id=" + request.id : "/") + "#!";

            var pathUrl = new Uri(request.url).LocalPath.Replace(new Uri(context.BaseUrl).LocalPath, "");
            Match match = Regex.Match(pathUrl, @"(\/?.*)search(\/?.*)");
            if (match.Success) {
                var resultType = match.Groups[1].Value.Trim('/');
                if (resultType.Equals(EntityType.GetEntityType(typeof(Series)).Keyword)) {
                    redirectUrl += "resultType=" + EntityType.GetEntityType(typeof(Series)).Keyword;
                } else if (resultType.Equals(EntityType.GetEntityType(typeof(DataPackage)).Keyword)) {
                    redirectUrl += "resultType=" + EntityType.GetEntityType(typeof(DataPackage)).Keyword;
                } else if (resultType.Contains(EntityType.GetEntityType(typeof(DataPackage)).Keyword)) {
                    redirectUrl += "resultType=" + "data";//in this case it is a search (over a data package) so we use data keyword
                } else if (resultType.Equals(EntityType.GetEntityType(typeof(WpsJob)).Keyword)) {
                    redirectUrl += "resultType=" + EntityType.GetEntityType(typeof(WpsJob)).Keyword;
                } else if (resultType.Equals(EntityType.GetEntityType(typeof(WpsProvider)).Keyword)) {
                    redirectUrl += "resultType=" + EntityType.GetEntityType(typeof(WpsProvider)).Keyword;
                } else if (resultType.Equals(EntityType.GetEntityType(typeof(WpsProcessOffering)).Keyword)) {
                    redirectUrl += "resultType=" + EntityType.GetEntityType(typeof(WpsProcessOffering)).Keyword;
                } else {
                    if (request.url.StartsWith(AppSettings["CatalogBaseUrl"]) || request.url.StartsWith(AppSettings["RecastBaseUrl"])) {
                        redirectUrl += "resultType=" + "data";
                    } else {
                        try {
                            var settings = MasterCatalogue.OpenSearchFactorySettings;
                            var os = new GenericOpenSearchable (new OpenSearchUrl (request.url), settings);
                            redirectUrl += "resultType=" + "data";
                        } catch (Exception e) { 
                            redirectUrl += "resultType=" + "na";
                        }
                    }
                }
                redirectUrl += "&url=" + HttpUtility.UrlEncode(request.url);
            } else {
                context.LogError(this, "Wrong format shared url");
                throw new Exception("Wrong format shared url");
            }

            var keyword = match.Groups[1].Value.StartsWith("/") ? match.Groups[1].Value.Substring(1) : match.Groups[1].Value;
            EntityType entityType = EntityType.GetEntityTypeFromKeyword(keyword);

            context.Close ();

            return new HttpResult(){ StatusCode = System.Net.HttpStatusCode.Redirect, Headers = {{HttpHeaders.Location, redirectUrl}}};
        }
    }

}