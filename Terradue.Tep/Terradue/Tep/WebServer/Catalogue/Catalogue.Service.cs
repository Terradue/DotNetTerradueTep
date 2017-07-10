using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using Terradue.OpenSearch;
using Terradue.OpenSearch.Engine;
using Terradue.OpenSearch.Result;
using Terradue.OpenSearch.Schema;
using Terradue.Portal;
using Terradue.ServiceModel.Syndication;
using Terradue.Tep.WebServer;
using Terradue.WebService.Model;

namespace Terradue.Tep.WebServer.Services
{
    [Api("Tep Terradue webserver")]
	[Restrict(EndpointAttributes.InSecure | EndpointAttributes.InternalNetworkAccess | EndpointAttributes.Json,
		EndpointAttributes.Secure   | EndpointAttributes.External | EndpointAttributes.Json)]
	public class CatalogueServiceTep : ServiceStack.ServiceInterface.Service{		

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
		/*				! 
		 * \fn Get(Getseries request)
		 * \brief Response to the Get request with a Getseries object (get the complete list of series)
		 * \param request request content
		 * \return the series list
		 */
		public object Get(GetOpensearchDescription request)
		{
			OpenSearchDescription OSDD;
            var context = TepWebContext.GetWebContext(PagePrivileges.EverybodyView);
			try{
				context.Open();
                context.LogInfo(this,string.Format("/data/collection/{{serieId}}/description GET serieId='{0}'", request.serieId));

				UriBuilder baseUrl = new UriBuilder ( context.BaseUrl );

				if ( request.serieId == null )
                    throw new ArgumentNullException(Terradue.Tep.WebServer.CustomErrorMessages.WRONG_IDENTIFIER);
					
                Terradue.Tep.Collection serie = Terradue.Tep.Collection.FromIdentifier(context,request.serieId);

				// The new URL template list 
				Hashtable newUrls = new Hashtable();
				UriBuilder urib;
				NameValueCollection query = new NameValueCollection();
				string[] queryString;

				urib = new UriBuilder( baseUrl.ToString() );

                OSDD = serie.GetOpenSearchDescription();
				urib.Path = baseUrl.Path + "/data/collection/" + serie.Identifier + "/search";
				query.Set("format","atom");
                query.Add(serie.GetOpenSearchParameters("application/atom+xml"));

				queryString = Array.ConvertAll(query.AllKeys, key => string.Format("{0}={1}", key, query[key]));
				urib.Query = string.Join("&", queryString);
				newUrls.Add("application/atom+xml",new OpenSearchDescriptionUrl("application/atom+xml", urib.ToString(), "search"));

				query.Set("format","json");
				queryString = Array.ConvertAll(query.AllKeys, key => string.Format("{0}={1}", key, query[key]));
				urib.Query = string.Join("&", queryString);
				newUrls.Add("application/json",new OpenSearchDescriptionUrl("application/json", urib.ToString(), "search"));

				query.Set("format","html");
				queryString = Array.ConvertAll(query.AllKeys, key => string.Format("{0}={1}", key, query[key]));
				urib.Query = string.Join("&", queryString);
				newUrls.Add("text/html",new OpenSearchDescriptionUrl("application/html", urib.ToString(), "search"));
				OSDD.Url = new OpenSearchDescriptionUrl[newUrls.Count];

				newUrls.Values.CopyTo(OSDD.Url,0);
				context.Close ();
			}catch(Exception e) {
                context.LogError(this, e.Message);
				context.Close ();
                throw e;
			}
			HttpResult hr = new HttpResult ( OSDD, "application/opensearchdescription+xml" );
			return hr;
		}

		/// <summary>
		/// Get the specified request.
		/// </summary>
		/// <param name="request">Request.</param>
		public object Get(GetOpensearchDescriptions request){
			OpenSearchDescription OSDD;
            var context = TepWebContext.GetWebContext(PagePrivileges.EverybodyView);
			try{
				context.Open();
                context.LogInfo(this,string.Format("/data/collection/description GET"));

                MasterCatalogue cat = new MasterCatalogue(context);
                OSDD = cat.GetOpenSearchDescription();
				
				context.Close ();
			}catch(Exception e) {
                context.LogError(this, e.Message);
				context.Close ();
                throw e;
			}
			HttpResult hr = new HttpResult ( OSDD, "application/opensearchdescription+xml" );
			return hr;
		}

		/// <summary>
		/// Get the specified request.
		/// </summary>
		/// <param name="request">Request.</param>
		public object Get(GetOpensearchSearch request){
            var context = TepWebContext.GetWebContext(PagePrivileges.EverybodyView);
            IOpenSearchResultCollection result = null;
			try{
				context.Open();
                context.LogInfo(this,string.Format("/data/collection/{{serieId}}/search GET serieId='{0}'", request.serieId));

				// Load the complete request
                HttpRequest httpRequest = HttpContext.Current.Request;

              	if ( request.serieId == null )
                    throw new ArgumentNullException(Terradue.Tep.WebServer.CustomErrorMessages.WRONG_IDENTIFIER);

                Terradue.Tep.Collection serie = Terradue.Tep.Collection.FromIdentifier(context,request.serieId);

                OpenSearchEngine ose = MasterCatalogue.OpenSearchEngine;
                ose.DefaultTimeOut = 60000;

                Type type = OpenSearchFactory.ResolveTypeFromRequest(httpRequest, ose);

                result = ose.Query(serie, httpRequest.QueryString, type);

                var descriptionUrl = serie.GetDescriptionBaseUrl("application/opensearchdescription+xml");
                if (descriptionUrl != null) {
                    result.Links.Add (new SyndicationLink (new Uri(descriptionUrl.Template), descriptionUrl.Relation, "OpenSearch Description link", descriptionUrl.Type, 0));
                }

				context.Close ();

			}catch(Exception e) {
                context.LogError(this, e.Message);
				context.Close ();
                throw e;
			}

            return new HttpResult(result.SerializeToString(), result.ContentType);
		}

		/// <summary>
		/// Get the specified request.
		/// </summary>
		/// <param name="request">Request.</param>
		public object Get(GetOpensearchSearchs request){
			// This page is public

			// But its content will be adapted accrding to context (user id, ...)

			// Load the complete request
			HttpRequest httpRequest = HttpContext.Current.Request;
            var context = TepWebContext.GetWebContext(PagePrivileges.EverybodyView);
            IOpenSearchResultCollection result = null;
			try{
				context.Open();
                context.LogInfo(this,string.Format("/data/collection/search GET"));

				//MasterCatalogue cat = new MasterCatalogue(context);
				//OpenSearchEngine ose = MasterCatalogue.OpenSearchEngine;
				//ose.DefaultTimeOut = 60000;

				//Type type = OpenSearchFactory.ResolveTypeFromRequest(httpRequest, ose);
				//result = ose.Query(cat, httpRequest.QueryString, type);		

                EntityList<Collection> collections = new EntityList<Collection>(context);
				OpenSearchEngine ose = MasterCatalogue.OpenSearchEngine;

				Type responseType = OpenSearchFactory.ResolveTypeFromRequest(httpRequest, ose);
				result = ose.Query(collections, httpRequest.QueryString, responseType);

				context.Close ();
			}catch(Exception e) {
                context.LogError(this, e.Message);
				context.Close ();
                throw e;
			}

            return new HttpResult(result.SerializeToString(), result.ContentType);
		}

        /// <summary>
        /// Get the specified request.
        /// </summary>
        /// <param name="request">Request.</param>
        public object Get(DataPackageSearchRequest request) {
            var context = TepWebContext.GetWebContext(PagePrivileges.EverybodyView);
            IOpenSearchResultCollection result = null;
            try {
                context.Open();
                context.LogInfo(this,string.Format("/data/package/{{DataPackageId}}/search GET DataPackageId='{0}'", request.DataPackageId));

                Terradue.Tep.DataPackage datapackage;

                try{
                    datapackage = DataPackage.FromIdentifier(context, request.DataPackageId);
                }catch(Exception e){
                    if(request.Key != null) {//or if public
                        context.AccessLevel = EntityAccessLevel.Administrator;
                        datapackage = DataPackage.FromIdentifier(context, request.DataPackageId);
                        if(request.Key != null && !request.Key.Equals(datapackage.AccessKey))
                            throw new UnauthorizedAccessException(CustomErrorMessages.WRONG_ACCESSKEY);
                    } else 
                        datapackage = DataPackage.FromIdentifier(context, request.DataPackageId);
                }

                datapackage.SetOpenSearchEngine(MasterCatalogue.OpenSearchEngine);

                OpenSearchEngine ose = MasterCatalogue.OpenSearchEngine;

                Type responseType = OpenSearchFactory.ResolveTypeFromRequest(HttpContext.Current.Request,ose);

                if(!String.IsNullOrEmpty(Request.QueryString["grouped"]) && Request.QueryString["grouped"] == "true"){
                    result = ose.Query(datapackage, Request.QueryString, responseType);
                }else{
                    result = ose.Query(datapackage, Request.QueryString, responseType);
                }

                var openSearchDescription = datapackage.GetLocalOpenSearchDescription();
                var uri_s = datapackage.GetSearchBaseUrl();
                OpenSearchDescriptionUrl openSearchUrlByRel = OpenSearchFactory.GetOpenSearchUrlByRel(openSearchDescription, "self");
                Uri uri_d;
                if (openSearchUrlByRel != null) {
                    uri_d = new Uri(openSearchUrlByRel.Template);
                }
                else {
                    uri_d = openSearchDescription.Originator;
                }
                if (uri_d != null) {
                    result.Links.Add(new SyndicationLink(uri_d, "search", "OpenSearch Description link", "application/opensearchdescription+xml", 0));
                }
                if (uri_s != null) {
                    result.Links.Add(new SyndicationLink(uri_s, "self", "OpenSearch Search link", "application/atom+xml", 0));
                }

                ActivityTep activity = new ActivityTep(context, datapackage, EntityOperationType.Search);
                activity.SetParam("items", result.TotalResults + "");
                activity.Store();

                context.Close();

            } catch (Exception e) {
                context.LogError(this, e.Message);
                context.Close();
                throw e;
            }

            return new HttpResult(result.SerializeToString(), result.ContentType);
        }

        /// <summary>
        /// Get the specified request.
        /// </summary>
        /// <param name="request">Request.</param>
        public object Get(DataPackagesSearchRequest request) {
            var context = TepWebContext.GetWebContext(PagePrivileges.EverybodyView);
            IOpenSearchResultCollection result = null;
            try {
                context.Open();
                context.LogInfo(this,string.Format("/data/package/search GET"));

                EntityList<Terradue.Tep.DataPackage> datapackages = new EntityList<DataPackage>(context);
                if (!string.IsNullOrEmpty (request.Key)) {
                    UserTep user = UserTep.FromApiKey (context, request.Key);
                    datapackages.UserId = user.Id;
                    context.AccessLevel = EntityAccessLevel.Privilege;
                }
                datapackages.SetFilter("Kind", RemoteResourceSet.KINDRESOURCESETNORMAL.ToString());
               
                // Load the complete request
                HttpRequest httpRequest = HttpContext.Current.Request;
                OpenSearchEngine ose = MasterCatalogue.OpenSearchEngine;

                // the opensearch cache system uses the query parameters
                // we add to the parameters the filters added to the load in order to avoir wrong cache
                // we use 't2-' in order to not interfer with possibly used query parameters
                var qs = new NameValueCollection(Request.QueryString);
                foreach (var filter in datapackages.FilterValues) qs.Add("t2-" + filter.Key.FieldName, filter.Value.ToString());

                Type responseType = OpenSearchFactory.ResolveTypeFromRequest(httpRequest, ose);
                result = ose.Query(datapackages, qs, responseType);

                context.Close();

            } catch (Exception e) {
                context.LogError(this, e.Message);
                context.Close();
                throw e;
            }

            return new HttpResult(result.SerializeToString(), result.ContentType);
        }


        /// <summary>
        /// Get the specified request.
        /// </summary>
        /// <param name="request">Request.</param>
        public object Get(DataPackageDescriptionRequest request) {
            var context = TepWebContext.GetWebContext(PagePrivileges.EverybodyView);
            try {
                context.Open();
                context.LogInfo(this,string.Format("/data/package/{{DataPackageId}}/description GET DataPackageId='{0}'", request.DataPackageId));

                Terradue.Tep.DataPackage datapackage;
                if(request.Key != null) {
                    context.AccessLevel = EntityAccessLevel.Administrator;
                    datapackage = DataPackage.FromIdentifier(context, request.DataPackageId);
                    if(request.Key != null && !request.Key.Equals(datapackage.AccessKey))
                        throw new UnauthorizedAccessException(CustomErrorMessages.WRONG_ACCESSKEY);
                } else {
                    datapackage = DataPackage.FromIdentifier(context, request.DataPackageId);
                }

                datapackage.SetOpenSearchEngine(MasterCatalogue.OpenSearchEngine);
                OpenSearchDescription osd = datapackage.GetLocalOpenSearchDescription();

                context.Close();

                return new HttpResult(osd,"application/opensearchdescription+xml");
            } catch (Exception e) {
                context.LogError(this, e.Message);
                context.Close();
                throw e;
            }
        }

        public object Get (DataPackagesDescriptionRequest request)
        {
            var context = TepWebContext.GetWebContext (PagePrivileges.EverybodyView);
            try {
                context.Open ();
                context.LogInfo (this, string.Format ("/data/package/description GET"));

                EntityList<Terradue.Tep.DataPackage> tmp_datapackages = new EntityList<DataPackage> (context);

                OpenSearchDescription osd = tmp_datapackages.GetOpenSearchDescription ();

                context.Close ();

                return new HttpResult (osd, "application/opensearchdescription+xml");
            } catch (Exception e) {
                context.LogError (this, e.Message);
                context.Close ();
                throw e;
            }
        }

	}

}