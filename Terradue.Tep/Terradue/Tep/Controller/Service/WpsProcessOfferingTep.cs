﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using IO.Swagger.Model;
using OpenGis.Wps;
using Terradue.OpenSearch;
using Terradue.Portal;
using Terradue.ServiceModel.Ogc.Owc.AtomEncoding;

namespace Terradue.Tep {
    [EntityTable(null, EntityTableConfiguration.Custom, Storage = EntityTableStorage.Above, AllowsKeywordSearch = true)]
    public class WpsProcessOfferingTep : WpsProcessOffering {
        public WpsProcessOfferingTep(IfyContext context) : base(context) {
        }

        public static new WpsProcessOfferingTep FromIdentifier(IfyContext context, string identifier) {
            var p = new WpsProcessOfferingTep(context);
            p.Identifier = identifier;
            p.Load();
            return p;
        }

        /***********/
        /* WPS 3.0 */
        /***********/

        /// <summary>
        /// Is this Service WPS 3.0
        /// </summary>
        /// <returns></returns>
        public bool IsWPS3() {
            return IsWPS3(this.Url);
        }

        public static bool IsWPS3(string url) {
            if (url == null) return false;
            return url.Contains("/wps3/");
        }

        /// <summary>
        /// Get list of Processing Services in DB for domain and tags
        /// </summary>
        /// <param name="context"></param>
        /// <param name="domain"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static List<WpsProcessOffering> GetWpsProcessingOfferingsForApp(IfyContext context, Domain domain, string[] tags) {
            //get services already in DB
            EntityList<WpsProcessOffering> dbProcesses = new EntityList<WpsProcessOffering>(context);
            dbProcesses.SetFilter("DomainId", domain.Id);
            if (tags != null && tags.Count() > 0) {
                IEnumerable<IEnumerable<string>> permutations = GetPermutations(tags, tags.Count());
                var r1 = permutations.Select(subset => string.Join("*", subset.Select(t => t).ToArray())).ToArray();
                var tagsresult = string.Join(",", r1.Select(t => "*" + t + "*"));
                dbProcesses.SetFilter("Tags", tagsresult);
            }
            dbProcesses.Load();
            return dbProcesses.GetItemsAsList();
        }

        /// <summary>
        /// Get list of Processing Services from Atom Feed
        /// Each Item of the feed contains a Describe Process url
        /// </summary>
        /// <param name="context"></param>
        /// <param name="feed"></param>
        /// <param name="createProviderIfNotFound"></param>
        /// <returns></returns>
        public static List<WpsProcessOffering> GetWpsProcessingOfferingsFromAtomFeed(IfyContext context, OwsContextAtomFeed feed, bool createProviderIfNotFound) {
            var remoteProcesses = new List<WpsProcessOffering>();

            if (feed.Items != null) {
                foreach (OwsContextAtomEntry item in feed.Items) {
                    var wpsOffering = item.Offerings.FirstOrDefault(of => of.Code == "http://www.opengis.net/spec/owc/1.0/req/atom/wps");
                    if (wpsOffering == null) continue;

                    var appIconLink = item.Links.FirstOrDefault(l => l.RelationshipType == "icon");
                    string icon = null;
                    if (appIconLink != null) icon = appIconLink.Uri.AbsoluteUri;

                    var operation = wpsOffering.Operations.FirstOrDefault(o => o.Code == "ProcessDescription");
                    var href = operation.Href;
                    switch (operation.Code) {
                        case "ProcessDescription":
                            var dpUri = new Uri(href);
                            var describeProcessUrl = dpUri.GetLeftPart(UriPartial.Path);
                            var providerBaseUrl = describeProcessUrl.Substring(0, describeProcessUrl.LastIndexOf("/"));
                            var processIdentifier = describeProcessUrl.Substring(describeProcessUrl.LastIndexOf("/") + 1);
                            WpsProvider wpsprovider = null;
                            try {
                                wpsprovider = WpsProvider.FromBaseUrl(context, providerBaseUrl);
                            } catch (System.Exception e) {
                                if (createProviderIfNotFound) {
                                    wpsprovider = new WpsProvider(context);
                                    wpsprovider.Identifier = new Uri(describeProcessUrl).Host;
                                    wpsprovider.BaseUrl = providerBaseUrl;
                                    wpsprovider.StageResults = true;
                                    wpsprovider.Proxy = true;
                                    wpsprovider.Store();

                                    wpsprovider.GrantPermissionsToAll();
                                }
                            }

                            //case WPS 3.0
                            if (IsWPS3(describeProcessUrl)) {
                                WpsProcessOffering process = GetProcessingFromDescribeProcessWps3(context, describeProcessUrl);
                                if (wpsprovider != null) process.Provider = wpsprovider;
                                process.IconUrl = icon;
                                remoteProcesses.Add(process);
                            }
                            break;
                        case "DescribeProcess":
                            //case WPS 1.0 -> TODO
                            break;
                        default:
                            break;
                    }

                }
            }
            return remoteProcesses;
        }

        /******************************/
        /* WPS 3.0 - DESCRIBE PROCESS */
        /******************************/

        /// <summary>
        /// Describe Process
        /// </summary>GetWpsProcessingFromDescribeProcess
        /// <returns></returns>
        public new object DescribeProcess() {

            if (this.IsWPS3()) {
                var wps3 = GetWps3ProcessingFromDescribeProcess(this.Url);
                return GetDescribeProcessFromWps3(wps3.Process);
            } else {
                return base.DescribeProcess();
            }
        }

        /// <summary>
        /// Get Wps Processing Offering from DescribeProcess url
        /// </summary>
        /// <param name="describeProcessUrl"></param>
        /// <returns></returns>
        public static WpsProcessOffering GetProcessingFromDescribeProcessWps3(IfyContext context, string describeProcessUrl) {
            var wps3 = GetWps3ProcessingFromDescribeProcess(describeProcessUrl);

            WpsProcessOffering process = new WpsProcessOffering(context);
            process.Identifier = Guid.NewGuid().ToString();
            process.RemoteIdentifier = wps3.Process.Id;
            process.Name = wps3.Process.Title;
            process.Description = wps3.Process.Abstract ?? wps3.Process.Title;
            process.Version = wps3.Process.Version;
            process.Url = describeProcessUrl;

            return process;
        }

        /// <summary>
        /// Get WPS3 processing From Describe Process url
        /// </summary>
        /// <param name="describeProcessUrl"></param>
        /// <returns></returns>
        public static Wps3 GetWps3ProcessingFromDescribeProcess(string describeProcessUrl) {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(describeProcessUrl);
            webRequest.Method = "GET";
            webRequest.Accept = "application/json";

            using (var httpResponse = (HttpWebResponse)webRequest.GetResponse()) {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    var result = streamReader.ReadToEnd();
                    var response = ServiceStack.Text.JsonSerializer.DeserializeFromString<Wps3>(result);
                    return response;
                }
            }
        }

        /// <summary>
        /// Get Describe process from wps3
        /// </summary>
        /// <param name="wps3"></param>
        /// <returns></returns>
        private ProcessDescriptions GetDescribeProcessFromWps3(Process wps3) {
            ProcessDescriptions processDescriptions = new ProcessDescriptions();

            var description = new ProcessDescriptionType();
            description.Identifier = new CodeType { Value = this.Identifier };
            description.Title = new LanguageStringType { Value = this.Name };
            description.Abstract = new LanguageStringType { Value = this.Description };
            description.DataInputs = new List<InputDescriptionType>();
            description.ProcessOutputs = new List<OutputDescriptionType>();

            //inputs
            foreach (var inputWps3 in wps3.Inputs) {
                var input = new InputDescriptionType();
                input.Identifier = new CodeType { Value = inputWps3.Id };
                input.Title = new LanguageStringType { Value = inputWps3.Title };
                input.Abstract = new LanguageStringType { Value = inputWps3.Abstract };
                input.minOccurs = inputWps3.MinOccurs.ToString();
                input.maxOccurs = inputWps3.MaxOccurs.ToString();

                if (inputWps3.Input.LiteralDataDomains != null) {
                    input.LiteralData = new LiteralInputType();
                    var literaldomain = inputWps3.Input.LiteralDataDomains[0];
                    if (literaldomain.DataType != null) input.LiteralData.DataType = new DomainMetadataType { Value = literaldomain.DataType.Name, reference = literaldomain.DataType.Reference };
                    if (literaldomain.Uom != null) input.LiteralData.UOMs = new SupportedUOMsType { Default = new SupportedUOMsTypeDefault { UOM = new DomainMetadataType { Value = literaldomain.Uom.Name, reference = literaldomain.Uom.Reference } } };
                    if (literaldomain.DefaultValue != null) input.LiteralData.DefaultValue = literaldomain.DefaultValue;
                    //if (literaldomain.ValueDefinition != null) input.LiteralData.AnyValue = new ServiceModel.Ogc.Ows11.AnyValue { AnyValue =  }
                }
                description.DataInputs.Add(input);
            }

            processDescriptions.ProcessDescription = new List<ProcessDescriptionType> { description };
            return processDescriptions;
        }

        /**********************/
        /* WPS 3.0 GET STATUS */
        /**********************/

        private ProcessBriefType ProcessBrief {
            get {
                ProcessBriefType processbrief = new ProcessBriefType();
                processbrief.Identifier = new CodeType { Value = this.RemoteIdentifier };
                processbrief.Abstract = new LanguageStringType { Value = this.Description };
                processbrief.Title = new LanguageStringType { Value = this.Name };
                processbrief.processVersion = this.Version;
                return processbrief;
            }
        }

        private StatusInfo GetJobStatus(string location) {

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(location);
            webRequest.Method = "GET";
            webRequest.Accept = "application/json";
            webRequest.ContentType = "application/json";


            using (var httpResponse = (HttpWebResponse)webRequest.GetResponse()) {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    var result = streamReader.ReadToEnd();
                    var response = ServiceStack.Text.JsonSerializer.DeserializeFromString<StatusInfo>(result);
                    return response;
                }
            }
        }

        public object GetStatusLocationContent(string statuslocation) {

            var statusInfo = GetJobStatus(statuslocation);

            //create response
            ExecuteResponse response = new ExecuteResponse();
            response.statusLocation = statuslocation;

            var uri = new Uri(statuslocation);
            response.serviceInstance = string.Format("{0}://{1}/", uri.Scheme, uri.Host);
            response.Process = ProcessBrief;
            response.service = "WPS";
            response.version = "3.0.0";

            switch (statusInfo.Status) {
                case StatusInfo.StatusEnum.Accepted:
                    response.Status = new StatusType { ItemElementName = ItemChoiceType.ProcessAccepted, Item = new ProcessAcceptedType() { Value = statusInfo.Message } };
                    break;
                case StatusInfo.StatusEnum.Running:
                    response.Status = new StatusType { ItemElementName = ItemChoiceType.ProcessStarted, Item = new ProcessStartedType() { Value = statusInfo.Message, percentCompleted = statusInfo.Progress.ToString() } };
                    break;
                case StatusInfo.StatusEnum.Dismissed:
                case StatusInfo.StatusEnum.Failed:
                    response.Status = new StatusType { ItemElementName = ItemChoiceType.ProcessFailed, Item = new ProcessFailedType() { ExceptionReport = new ExceptionReport { Exception = new List<ExceptionType>() { new ExceptionType { ExceptionText = new List<string>() { statusInfo.Message } } } } } };
                    break;
                case StatusInfo.StatusEnum.Successful:
                    response.Status = new StatusType { ItemElementName = ItemChoiceType.ProcessSucceeded, Item = new ProcessSucceededType() { Value = statusInfo.Message } };
                    var outputs = GetOutputs(statuslocation);
                    var urib = new UriBuilder(this.Provider.BaseUrl);
                    var wfoutput = outputs.outputs.First(o => o.id == "wf_outputs");
                    urib.Path = urib.Path.Substring(0, urib.Path.IndexOf("/", 1)) + wfoutput.value.href;
                    var resultlink = urib.Uri.AbsoluteUri;
                    string s3link = null;
                    if (resultlink.StartsWith("s3:"))
                        s3link = resultlink;
                    else {
                        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(resultlink);
                        webRequest.Method = "GET";
                        webRequest.Accept = "application/json";
                        webRequest.ContentType = "application/json";

                        using (var httpResponse = (HttpWebResponse)webRequest.GetResponse()) {
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                                var result = streamReader.ReadToEnd();
                                var res = ServiceStack.Text.JsonSerializer.DeserializeFromString<StacItemResult>(result);
                                s3link = res.StacCatalogUri;
                            }
                        }

                    }
                    var resultdescription = s3link;

                    if (System.Configuration.ConfigurationManager.AppSettings["SUPERVISOR_WPS_STAGE_URL"] != null && !string.IsNullOrEmpty(s3link)) {
                        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["SUPERVISOR_WPS_STAGE_URL"]);
                        webRequest.Method = "POST";
                        webRequest.Accept = "application/json";
                        webRequest.ContentType = "application/json";
                        var access_token = DBCookie.LoadDBCookie(context, System.Configuration.ConfigurationManager.AppSettings["SUPERVISOR_COOKIE_TOKEN_ACCESS"]).Value;
                        webRequest.Headers.Set(HttpRequestHeader.Authorization, "Bearer " + access_token);
                        webRequest.Timeout = 10000;

                        var jsonurl = new JsonUrl { url = s3link };
                        var json = ServiceStack.Text.JsonSerializer.SerializeToString(jsonurl);

                        byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
                        webRequest.ContentLength = data.Length;

                        using (var requestStream = webRequest.GetRequestStream()) {
                            requestStream.Write(data, 0, data.Length);
                            requestStream.Close();
                            using (var httpResponse = (HttpWebResponse)webRequest.GetResponse()) {
                                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                                    var location = httpResponse.Headers["Location"];
                                    if (!string.IsNullOrEmpty(location)) {
                                        resultdescription = new Uri(httpResponse.Headers["Location"], UriKind.RelativeOrAbsolute).AbsoluteUri;
                                    }
                                }
                            }
                        }
                    }

                    if (outputs != null && wfoutput != null) {
                        response.ProcessOutputs = new List<OutputDataType> { };
                        response.ProcessOutputs.Add(new OutputDataType {
                            Identifier = new CodeType { Value = "result_osd" },
                            Item = new OpenGis.Wps.DataType {
                                Item = new OpenGis.Wps.ComplexDataType {
                                    mimeType = "application/xml",
                                    Reference = new OutputReferenceType {
                                        href = resultdescription,
                                        mimeType = "application/opensearchdescription+xml"
                                    }
                                }
                            }
                        });
                    }
                    break;
            }

            return response;
        }


        /*******************/
        /* WPS 3.0 EXECUTE */
        /*******************/

        public new object Execute(OpenGis.Wps.Execute executeInput, string jobreference = null) {

            if (this.IsWPS3()) {

                var location = SubmitExecute(executeInput);

                ExecuteResponse response = new ExecuteResponse();
                response.statusLocation = location;

                var uri = new Uri(location);
                response.serviceInstance = string.Format("{0}://{1}/", uri.Scheme, uri.Host);
                response.Process = ProcessBrief;
                response.service = "WPS";
                response.version = "3.0.0";

                response.Status = new StatusType { Item = new ProcessAcceptedType() { Value = string.Format("Preparing job") } };//TODO

                return response;

                //TODO: handle case of errors
            } else {
                return base.Execute(executeInput, jobreference);
            }
        }

        public string SubmitExecute(OpenGis.Wps.Execute executeInput) {

            IO.Swagger.Model.Execute execute = BuildExecute(executeInput);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(this.Url + "/jobs");
            webRequest.Method = "POST";
            webRequest.Accept = "application/json";
            webRequest.ContentType = "application/json";

            var json = ServiceStack.Text.JsonSerializer.SerializeToString<IO.Swagger.Model.Execute>(execute);

            byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
            webRequest.ContentLength = data.Length;

            using (var requestStream = webRequest.GetRequestStream()) {
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                using (var httpResponse = (HttpWebResponse)webRequest.GetResponse()) {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                        var result = streamReader.ReadToEnd();
                        var response = ServiceStack.Text.JsonSerializer.DeserializeFromString<Wps3>(result);
                        var location = new Uri(httpResponse.Headers["Location"], UriKind.RelativeOrAbsolute);
                        if (!location.AbsoluteUri.StartsWith("http"))
                            location = new Uri(new Uri(this.Url), location);
                        return location.AbsoluteUri;
                    }
                }
            }
        }

        protected IO.Swagger.Model.Execute BuildExecute(OpenGis.Wps.Execute executeInput) {

            var wps3 = GetWps3ProcessingFromDescribeProcess(this.Url);

            List<IO.Swagger.Model.Input> inputs = new List<IO.Swagger.Model.Input>();
            foreach (var dataInput in executeInput.DataInputs) {
                var inp = new Inputs();
                inp.Id = dataInput.Identifier.Value;

                if (dataInput.Data != null && dataInput.Data.Item != null) {
                    if (dataInput.Data.Item is OpenGis.Wps.LiteralDataType) {

                        var datatype = "string";
                        foreach (var i in wps3.Process.Inputs) {
                            if (inp.Id == i.Id) {
                                if (i.Input.LiteralDataDomains != null) {
                                    var literaldomain = i.Input.LiteralDataDomains[0];
                                    if (!string.IsNullOrEmpty(literaldomain.DataType.Reference))
                                        datatype = literaldomain.DataType.Reference;
                                }
                            }
                        }

                        var ld = dataInput.Data.Item as OpenGis.Wps.LiteralDataType;
                        StupidData literalData = new StupidData(ld.Value, new StupidDataType(datatype));
                        inputs.Add(new IO.Swagger.Model.Input(dataInput.Identifier.Value, literalData));
                    }
                }
            }

            List<IO.Swagger.Model.Output> outputs = new List<IO.Swagger.Model.Output>();
            var output = new IO.Swagger.Model.Output();
            output.Id = "wf_outputs";
            output.TransmissionMode = TransmissionMode.Reference;
            output.Format = new IO.Swagger.Model.Format("application/json");
            outputs.Add(output);

            return new IO.Swagger.Model.Execute(inputs, outputs, IO.Swagger.Model.Execute.ModeEnum.Async, IO.Swagger.Model.Execute.ResponseEnum.Raw, null);
        }

        /*******************/
        /* WPS 3.0 RESULTS */
        /*******************/

        public ResultOutputs GetOutputs(string jobStatusLocation) {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(jobStatusLocation + "/result");
            webRequest.Method = "GET";
            webRequest.Accept = "application/json";
            webRequest.ContentType = "application/json";


            using (var httpResponse = (HttpWebResponse)webRequest.GetResponse()) {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    var result = streamReader.ReadToEnd();
                    var response = ServiceStack.Text.JsonSerializer.DeserializeFromString<ResultOutputs>(result);
                    return response;
                }
            }
        }

        public override object GetFilterForParameter(string parameter, string value) {
            switch (parameter) {
                case "correlatedTo":
                    var settings = MasterCatalogue.OpenSearchFactorySettings;
                    var urlBOS = new UrlBasedOpenSearchable(context, new OpenSearchUrl(value), settings);
                    var entity = urlBOS.Entity;
                    if (entity is EntityList<ThematicApplicationCached>) {
                        var entitylist = entity as EntityList<ThematicApplicationCached>;
                        var items = entitylist.GetItemsAsList();
                        if (items.Count > 0) {
                            var feed = ThematicAppCachedFactory.GetOwsContextAtomFeed(items[0].TextFeed);
                            if (feed != null) {
                                var entry = feed.Items.First();
                                foreach (var offering in entry.Offerings) {
                                    switch (offering.Code) {
                                        case "http://www.opengis.net/spec/owc/1.0/req/atom/wps":
                                            if (offering.Operations != null && offering.Operations.Length > 0) {
                                                foreach (var operation in offering.Operations) {
                                                    var href = operation.Href;
                                                    switch (operation.Code) {
                                                        case "ListProcess":
                                                            var result = new List<KeyValuePair<string, string>>();
                                                            var uri = new Uri(href);
                                                            var nvc = HttpUtility.ParseQueryString(uri.Query);
                                                            foreach (var key in nvc.AllKeys) {
                                                                switch (key) {
                                                                    case "domain":
                                                                        if (nvc[key] != null) {
                                                                            string domainIdentifier = null;
                                                                            if (nvc[key].Contains("${USERNAME}")) {
                                                                                var user = UserTep.FromId(context, context.UserId);
                                                                                user.LoadCloudUsername();
                                                                                domainIdentifier = nvc[key].Replace("${USERNAME}", user.TerradueCloudUsername);
                                                                            } else domainIdentifier = nvc[key];
                                                                            if (!string.IsNullOrEmpty(domainIdentifier)) {
                                                                                var domain = Domain.FromIdentifier(context, domainIdentifier);
                                                                                result.Add(new KeyValuePair<string, string>("DomainId", domain.Id + ""));
                                                                            }
                                                                        }
                                                                        break;
                                                                    case "tag":
                                                                        if (!string.IsNullOrEmpty(nvc[key])) {
                                                                            var tags = nvc[key].Split(",".ToArray());
                                                                            IEnumerable<IEnumerable<string>> permutations = GetPermutations(tags, tags.Count());
                                                                            var r1 = permutations.Select(subset => string.Join("*", subset.Select(t => t).ToArray())).ToArray();
                                                                            var tagsresult = string.Join(",", r1.Select(t => "*" + t + "*"));
                                                                            result.Add(new KeyValuePair<string, string>("Tags", tagsresult));
                                                                        }
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                            }
                                                            return result;
                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    return new KeyValuePair<string, string>("DomainId", "-1");//we don't want any result to be returned, as no service is returned to the app (no wps search link)
                default:
                    return base.GetFilterForParameter(parameter, value);
            }
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length) {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}
