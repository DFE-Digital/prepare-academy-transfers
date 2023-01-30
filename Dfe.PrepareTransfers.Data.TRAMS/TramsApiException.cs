using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;

namespace Dfe.PrepareTransfers.Data.TRAMS
{
   [Serializable]
    public class TramsApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public TramsApiException(HttpResponseMessage httpResponseMessage, string message = null) 
            : base(message ?? $"API encountered an error-({httpResponseMessage.StatusCode})")
        {
            StatusCode = httpResponseMessage.StatusCode;
            base.Data.Add("Sentry:Tag:StatusCode", StatusCode);
            base.Data.Add("Content", httpResponseMessage.Content?.ReadAsStringAsync().Result);
        }

        protected TramsApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}