using System;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Backend.Exceptions
{
    public class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ContentType { get; set; } = @"text/plain";
        
        #region Constructor
        public AppException(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;
        }

        public AppException(HttpStatusCode statusCode, string message) 
            : base(message)
        {
            this.StatusCode = statusCode;
        }

        public AppException(HttpStatusCode statusCode, Exception inner) 
            : this(statusCode, inner.ToString()) { }

        public AppException(HttpStatusCode statusCode, JObject errorObject) 
            : this(statusCode, errorObject.ToString())
        {
            this.ContentType = @"application/json";
        }
        #endregion
    }
}