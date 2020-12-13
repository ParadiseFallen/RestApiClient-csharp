using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ApiClient
{
    public class Request<T>
    {
        public JsonSerializerOptions SerializerOptions { get; protected set; }
        public Task<HttpResponseMessage> RequestMethod { get; protected set; }
        public Func<Exception, Task<T>> OnException { get; protected set; }
        public Func<HttpResponseMessage, Task<T>> OnSuccses { get; protected set; }

        public Request(Task<HttpResponseMessage> request, JsonSerializerOptions serializerOptions)
        {
            SerializerOptions = serializerOptions;
            RequestMethod = request;

            OnSuccses = new Func<HttpResponseMessage, Task<T>>(
                async response => 
                    await response.Content.ReadFromJsonAsync<T>(SerializerOptions));
            OnException = new Func<Exception, Task<T>>(async exception => default);
        }
        #region Chain
        public Request<T> WithSerializerOptions(JsonSerializerOptions serializerOptions)
        {
            SerializerOptions = serializerOptions;
            return this;
        }
        public Request<T> ModifySerializerOptions(Func<JsonSerializerOptions, JsonSerializerOptions> modificationFunction)
        {
            SerializerOptions = modificationFunction(SerializerOptions);
            return this;
        }
        public Request<T> SetExceptionHandler(Func<Exception, Task<T>> exceptionHandler)
        {
            OnException = exceptionHandler;
            return this;
        }
        public Request<T> SetResponseHandler(Func<HttpResponseMessage, Task<T>> responseHandler)
        {
            OnSuccses = responseHandler;
            return this;
        }
        #endregion
        public async Task<T> ExecuteRequestAsync()
        {
            try
            {
                return await OnSuccses(await RequestMethod);
            }
            catch (Exception ex)
            {
                return await OnException(ex);
            }
        }
    }
}
