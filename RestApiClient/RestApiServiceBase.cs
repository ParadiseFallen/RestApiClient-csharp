using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiClient
{
    public abstract class RestApiServiceBase
    {
        /// <summary>
        /// DO NOT CALL <see cref="RestClient.GetAsync()"/>, <see cref="RestClient.PostAsync()"/>, <see cref="RestClient.PutAsync()"/> etc
        /// DIRECTLY. Use protected methods
        /// </summary>
        public RestClient RestClient { get; }
        public JsonSerializerOptions SerializerOptions { get; set; }

        public RestApiServiceBase(RestClient restClient)
        {
            RestClient = restClient ?? throw new NullReferenceException($"APIClient is null");
        }

        protected async Task<HttpResponseMessage> GetAsync(string uri) => 
            await RestClient.GetAsync(uri);

        protected virtual Request<RESPONSE_DATA_TYPE> CreateRequest<RESPONSE_DATA_TYPE>(Task<HttpResponseMessage> requestMethod) => 
            new Request<RESPONSE_DATA_TYPE>(requestMethod,SerializerOptions);

        protected async Task<HttpResponseMessage> PostAsync<T>(string uri, T data) => 
            await RestClient.PostAsJsonAsync(uri,data, SerializerOptions);

        protected async Task<HttpResponseMessage> PostAsync(string uri) =>
            await RestClient.PostAsync(uri,null);

        protected async Task<HttpResponseMessage> PutAsync<T>(string uri, T data)=>
            await RestClient.PutAsJsonAsync(uri, data, SerializerOptions);

        protected async Task<HttpResponseMessage> DeleteAsync<T>(string uri)=>
            await RestClient.DeleteAsync(uri);

        /// <summary>
        /// Safe execution of request. Wrapped by <c>Try Catch</c>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestMethod">Task with request</param>
        /// <param name="reciveHandler">Handler for recived HttpResponseMessage. Read <see cref="T"/> from <see cref="HttpResponseMessage.Content"/> <c>response.Content.ReadFromJsonAsync<T>(SerializerOptions)</c> </param>
        /// <param name="exceptionHandler">Handler for exception. Suppresses by default and return null</param>
        /// <returns><see cref="T"/></returns>
        [Obsolete("Obsolete method. Use CreateRequest<T>() and pipeline instead.")]
        protected virtual async Task<T> ExecuteRequestAsync<T>(
            Task<HttpResponseMessage> requestMethod,
            Func<HttpResponseMessage, Task<T>> reciveHandler = null,
            Func<Exception, Task<T>> exceptionHandler = null)
        {
            reciveHandler ??= new Func<HttpResponseMessage, Task<T>>(
                response => 
                    response.Content.ReadFromJsonAsync<T>(SerializerOptions));
            exceptionHandler ??= new Func<Exception, Task<T>>(async exception => default);

            try
            {
                return await reciveHandler(await requestMethod);
            }
            catch (Exception ex)
            {
                return await exceptionHandler(ex);
            }
        }
    }
}
