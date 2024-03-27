using System.Net;

namespace Dfe.PrepareTransfers.Data;

public class ApiResponse<TBody>
{
   public ApiResponse(HttpStatusCode statusCode, TBody body)
   {
      Success = (int)statusCode >= 200 && (int)statusCode < 300;
      Body = body;
      StatusCode = statusCode;
   }

   public bool Success { get; }
   public HttpStatusCode StatusCode { get; }
   public TBody Body { get; }
}
