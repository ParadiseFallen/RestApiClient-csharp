using System;
using System.Net;
using System.Net.Http;

namespace ApiClient
{
    /// <summary>
    /// <see cref="HttpClient"/> but added extra properties like
    /// <see cref="HttpClientHandler"/> and <see cref="CookieContainer"/>
    /// </summary>
    public class RestClient : HttpClient
    {
        #region Extra props
        public HttpClientHandler Handler { get; init; }
        /// <summary>
        /// Fast acces to <see cref="HttpClientHandler.CookieContainer"/>
        /// </summary>
        public CookieContainer Cookies { get => Handler.CookieContainer; set => Handler.CookieContainer = value; }
        #endregion

        #region Ctors
        /// <summary>
        /// Constructor. <see cref="RestClient"/> need <see cref="HttpClientHandler"/> instance
        /// </summary>
        /// <param name="handler"></param>
        public RestClient(HttpClientHandler handler) : base(handler,true)
        {
            //recive and store handler
            Handler = handler ?? throw new NullReferenceException("HttpClientHandler is null.");
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Dispose <see cref="HttpClientHandler"/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            Handler.Dispose();
            base.Dispose(disposing);
        }
        #endregion
    }
}
