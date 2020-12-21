using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiClient
{
    public class ServiceFactory
    {

        #region Singeltone default factory
        private static ServiceFactory factoryInstance = null;
        public static ServiceFactory Default 
        { 
            get 
            {
                if (factoryInstance==null)
                    factoryInstance = new ServiceFactory();
                return factoryInstance;
            } 
        }
        #endregion  

        public RestClient Client { get; set; }
        public JsonSerializerOptions SerializerOptions { get; set; }

        public Action<RestApiServiceBase> SetupServiceInstance { get; set; }
        public ServiceFactory()
        {
            SetupServiceInstance = service => service.SerializerOptions = SerializerOptions;
        }

        public T CreateService<T>() where T :  RestApiServiceBase
        {
            var type = typeof(T);
            var constructor = type.GetConstructor(new[] { typeof(RestClient) });
            T instance = (T)constructor.Invoke(new[] { Client });
            SetupServiceInstance(instance);
            return instance;
        }
    }
}
