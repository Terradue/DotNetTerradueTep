/* 
 * The OGC API - Processes
 *
 * WARNING - THIS IS WORK IN PROGRESS
 *
 * OpenAPI spec version: 1.0-draft.5
 * Contact: b.pross@52north.org
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
        public interface IResultApi : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// retrieve the result(s) of a job
        /// </summary>
        /// <remarks>
        /// Lists available results of a job. In case of a failure, lists exceptions instead.
        /// </remarks>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="processId">local identifier of a process</param>
        /// <param name="jobId">local identifier of a job</param>
        /// <returns>Result</returns>
        Result GetResult (string processId, string jobId);

        /// <summary>
        /// retrieve the result(s) of a job
        /// </summary>
        /// <remarks>
        /// Lists available results of a job. In case of a failure, lists exceptions instead.
        /// </remarks>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="processId">local identifier of a process</param>
        /// <param name="jobId">local identifier of a job</param>
        /// <returns>ApiResponse of Result</returns>
        ApiResponse<Result> GetResultWithHttpInfo (string processId, string jobId);
        #endregion Synchronous Operations
        #region Asynchronous Operations
        /// <summary>
        /// retrieve the result(s) of a job
        /// </summary>
        /// <remarks>
        /// Lists available results of a job. In case of a failure, lists exceptions instead.
        /// </remarks>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="processId">local identifier of a process</param>
        /// <param name="jobId">local identifier of a job</param>
        /// <returns>Task of Result</returns>
        System.Threading.Tasks.Task<Result> GetResultAsync (string processId, string jobId);

        /// <summary>
        /// retrieve the result(s) of a job
        /// </summary>
        /// <remarks>
        /// Lists available results of a job. In case of a failure, lists exceptions instead.
        /// </remarks>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="processId">local identifier of a process</param>
        /// <param name="jobId">local identifier of a job</param>
        /// <returns>Task of ApiResponse (Result)</returns>
        System.Threading.Tasks.Task<ApiResponse<Result>> GetResultAsyncWithHttpInfo (string processId, string jobId);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
        public partial class ResultApi : IResultApi
    {
        private IO.Swagger.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultApi"/> class.
        /// </summary>
        /// <returns></returns>
        public ResultApi(String basePath)
        {
            this.Configuration = new IO.Swagger.Client.Configuration { BasePath = basePath };

            ExceptionFactory = IO.Swagger.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultApi"/> class
        /// </summary>
        /// <returns></returns>
        public ResultApi()
        {
            this.Configuration = IO.Swagger.Client.Configuration.Default;

            ExceptionFactory = IO.Swagger.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public ResultApi(IO.Swagger.Client.Configuration configuration = null)
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
        /// retrieve the result(s) of a job Lists available results of a job. In case of a failure, lists exceptions instead.
        /// </summary>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="processId">local identifier of a process</param>
        /// <param name="jobId">local identifier of a job</param>
        /// <returns>Result</returns>
        public Result GetResult (string processId, string jobId)
        {
             ApiResponse<Result> localVarResponse = GetResultWithHttpInfo(processId, jobId);
             return localVarResponse.Data;
        }

        /// <summary>
        /// retrieve the result(s) of a job Lists available results of a job. In case of a failure, lists exceptions instead.
        /// </summary>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="processId">local identifier of a process</param>
        /// <param name="jobId">local identifier of a job</param>
        /// <returns>ApiResponse of Result</returns>
        public ApiResponse< Result > GetResultWithHttpInfo (string processId, string jobId)
        {
            // verify the required parameter 'processId' is set
            if (processId == null)
                throw new Client.ApiException(400, "Missing required parameter 'processId' when calling ResultApi->GetResult");
            // verify the required parameter 'jobId' is set
            if (jobId == null)
                throw new Client.ApiException(400, "Missing required parameter 'jobId' when calling ResultApi->GetResult");

            var localVarPath = "./processes/{processId}/jobs/{jobId}/results";
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
                "application/json",
                "text/html"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (processId != null) localVarPathParams.Add("processId", this.Configuration.ApiClient.ParameterToString(processId)); // path parameter
            if (jobId != null) localVarPathParams.Add("jobId", this.Configuration.ApiClient.ParameterToString(jobId)); // path parameter

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                ApiException exception = ExceptionFactory("GetResult", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<Result>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (Result) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(Result)));
        }

        /// <summary>
        /// retrieve the result(s) of a job Lists available results of a job. In case of a failure, lists exceptions instead.
        /// </summary>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="processId">local identifier of a process</param>
        /// <param name="jobId">local identifier of a job</param>
        /// <returns>Task of Result</returns>
        public async System.Threading.Tasks.Task<Result> GetResultAsync (string processId, string jobId)
        {
             ApiResponse<Result> localVarResponse = await GetResultAsyncWithHttpInfo(processId, jobId);
             return localVarResponse.Data;

        }

        /// <summary>
        /// retrieve the result(s) of a job Lists available results of a job. In case of a failure, lists exceptions instead.
        /// </summary>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="processId">local identifier of a process</param>
        /// <param name="jobId">local identifier of a job</param>
        /// <returns>Task of ApiResponse (Result)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<Result>> GetResultAsyncWithHttpInfo (string processId, string jobId)
        {
            // verify the required parameter 'processId' is set
            if (processId == null)
                throw new Client.ApiException(400, "Missing required parameter 'processId' when calling ResultApi->GetResult");
            // verify the required parameter 'jobId' is set
            if (jobId == null)
                throw new Client.ApiException(400, "Missing required parameter 'jobId' when calling ResultApi->GetResult");

            var localVarPath = "./processes/{processId}/jobs/{jobId}/results";
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
                "application/json",
                "text/html"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (processId != null) localVarPathParams.Add("processId", this.Configuration.ApiClient.ParameterToString(processId)); // path parameter
            if (jobId != null) localVarPathParams.Add("jobId", this.Configuration.ApiClient.ParameterToString(jobId)); // path parameter

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                ApiException exception = ExceptionFactory("GetResult", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<Result>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (Result) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(Result)));
        }

    }
}
