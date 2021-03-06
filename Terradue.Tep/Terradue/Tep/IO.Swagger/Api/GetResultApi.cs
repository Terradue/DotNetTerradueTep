/* 
 * The ZOO-Project WPS / OGC API - Processing Development Server
 *
 * Development version of ZOO-Project OGC WPS. See http://www.zoo-project.org
 *
 * OpenAPI spec version: 1.6.0-933M
 * Contact: gerald.fenoy@geolabs.fr
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RestSharp.Portable;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace IO.Swagger.Api
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
        public interface IGetResultApi : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// The result of a job execution.
        /// </summary>
        /// <remarks>
        /// The result of a job execution.
        /// </remarks>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">The id of a process</param>
        /// <param name="jobID">The id of a job</param>
        /// <returns>Result</returns>
        Result ProcessesIdJobsJobIDResultGet (string id, string jobID);

        /// <summary>
        /// The result of a job execution.
        /// </summary>
        /// <remarks>
        /// The result of a job execution.
        /// </remarks>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">The id of a process</param>
        /// <param name="jobID">The id of a job</param>
        /// <returns>ApiResponse of Result</returns>
        ApiResponse<Result> ProcessesIdJobsJobIDResultGetWithHttpInfo (string id, string jobID);
        #endregion Synchronous Operations
        #region Asynchronous Operations
        /// <summary>
        /// The result of a job execution.
        /// </summary>
        /// <remarks>
        /// The result of a job execution.
        /// </remarks>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">The id of a process</param>
        /// <param name="jobID">The id of a job</param>
        /// <returns>Task of Result</returns>
        System.Threading.Tasks.Task<Result> ProcessesIdJobsJobIDResultGetAsync (string id, string jobID);

        /// <summary>
        /// The result of a job execution.
        /// </summary>
        /// <remarks>
        /// The result of a job execution.
        /// </remarks>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">The id of a process</param>
        /// <param name="jobID">The id of a job</param>
        /// <returns>Task of ApiResponse (Result)</returns>
        System.Threading.Tasks.Task<ApiResponse<Result>> ProcessesIdJobsJobIDResultGetAsyncWithHttpInfo (string id, string jobID);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
        public partial class GetResultApi : IGetResultApi
    {
        private IO.Swagger.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetResultApi"/> class.
        /// </summary>
        /// <returns></returns>
        public GetResultApi(String basePath)
        {
            this.Configuration = new IO.Swagger.Client.Configuration { BasePath = basePath };

            ExceptionFactory = IO.Swagger.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetResultApi"/> class
        /// </summary>
        /// <returns></returns>
        public GetResultApi()
        {
            this.Configuration = IO.Swagger.Client.Configuration.Default;

            ExceptionFactory = IO.Swagger.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetResultApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public GetResultApi(IO.Swagger.Client.Configuration configuration = null)
        {
            if (configuration == null) // use the default one in Configuration
                this.Configuration = IO.Swagger.Client.Configuration.Default;
            else
                this.Configuration = configuration;

            ExceptionFactory = IO.Swagger.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public String GetBasePath()
        {
            return this.Configuration.ApiClient.RestClient.BaseUrl.ToString();
        }

        /// <summary>
        /// Sets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        [Obsolete("SetBasePath is deprecated, please do 'Configuration.ApiClient = new ApiClient(\"http://new-path\")' instead.")]
        public void SetBasePath(String basePath)
        {
            // do nothing
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public IO.Swagger.Client.Configuration Configuration {get; set;}

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public IO.Swagger.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        /// Gets the default header.
        /// </summary>
        /// <returns>Dictionary of HTTP header</returns>
        [Obsolete("DefaultHeader is deprecated, please use Configuration.DefaultHeader instead.")]
        public IDictionary<String, String> DefaultHeader()
        {
            return new ReadOnlyDictionary<string, string>(this.Configuration.DefaultHeader);
        }

        /// <summary>
        /// Add default header.
        /// </summary>
        /// <param name="key">Header field name.</param>
        /// <param name="value">Header field value.</param>
        /// <returns></returns>
        [Obsolete("AddDefaultHeader is deprecated, please use Configuration.AddDefaultHeader instead.")]
        public void AddDefaultHeader(string key, string value)
        {
            this.Configuration.AddDefaultHeader(key, value);
        }

        /// <summary>
        /// The result of a job execution. The result of a job execution.
        /// </summary>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">The id of a process</param>
        /// <param name="jobID">The id of a job</param>
        /// <returns>Result</returns>
        public Result ProcessesIdJobsJobIDResultGet (string id, string jobID)
        {
             ApiResponse<Result> localVarResponse = ProcessesIdJobsJobIDResultGetWithHttpInfo(id, jobID);
             return localVarResponse.Data;
        }

        /// <summary>
        /// The result of a job execution. The result of a job execution.
        /// </summary>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">The id of a process</param>
        /// <param name="jobID">The id of a job</param>
        /// <returns>ApiResponse of Result</returns>
        public ApiResponse< Result > ProcessesIdJobsJobIDResultGetWithHttpInfo (string id, string jobID)
        {
            // verify the required parameter 'id' is set
            if (id == null)
                throw new Client.ApiException(400, "Missing required parameter 'id' when calling GetResultApi->ProcessesIdJobsJobIDResultGet");
            // verify the required parameter 'jobID' is set
            if (jobID == null)
                throw new Client.ApiException(400, "Missing required parameter 'jobID' when calling GetResultApi->ProcessesIdJobsJobIDResultGet");

            var localVarPath = "./processes/{id}/jobs/{jobID}/result";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (id != null) localVarPathParams.Add("id", this.Configuration.ApiClient.ParameterToString(id)); // path parameter
            if (jobID != null) localVarPathParams.Add("jobID", this.Configuration.ApiClient.ParameterToString(jobID)); // path parameter

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                ApiException exception = ExceptionFactory("ProcessesIdJobsJobIDResultGet", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<Result>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (Result) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(Result)));
        }

        /// <summary>
        /// The result of a job execution. The result of a job execution.
        /// </summary>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">The id of a process</param>
        /// <param name="jobID">The id of a job</param>
        /// <returns>Task of Result</returns>
        public async System.Threading.Tasks.Task<Result> ProcessesIdJobsJobIDResultGetAsync (string id, string jobID)
        {
             ApiResponse<Result> localVarResponse = await ProcessesIdJobsJobIDResultGetAsyncWithHttpInfo(id, jobID);
             return localVarResponse.Data;

        }

        /// <summary>
        /// The result of a job execution. The result of a job execution.
        /// </summary>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="id">The id of a process</param>
        /// <param name="jobID">The id of a job</param>
        /// <returns>Task of ApiResponse (Result)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<Result>> ProcessesIdJobsJobIDResultGetAsyncWithHttpInfo (string id, string jobID)
        {
            // verify the required parameter 'id' is set
            if (id == null)
                throw new Client.ApiException(400, "Missing required parameter 'id' when calling GetResultApi->ProcessesIdJobsJobIDResultGet");
            // verify the required parameter 'jobID' is set
            if (jobID == null)
                throw new Client.ApiException(400, "Missing required parameter 'jobID' when calling GetResultApi->ProcessesIdJobsJobIDResultGet");

            var localVarPath = "./processes/{id}/jobs/{jobID}/result";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (id != null) localVarPathParams.Add("id", this.Configuration.ApiClient.ParameterToString(id)); // path parameter
            if (jobID != null) localVarPathParams.Add("jobID", this.Configuration.ApiClient.ParameterToString(jobID)); // path parameter

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                ApiException exception = ExceptionFactory("ProcessesIdJobsJobIDResultGet", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<Result>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (Result) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(Result)));
        }

    }
}
