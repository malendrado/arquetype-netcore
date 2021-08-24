using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Backend.Api.Middlewares
{
    internal class LoggingMiddleware
	{
		private readonly RequestDelegate _nextRequestDelegate;
		private readonly ILogger<LoggingMiddleware> _logger;

		public LoggingMiddleware(
			RequestDelegate nextRequestDelegate,
			ILoggerFactory loggerFactory)
		{
			_nextRequestDelegate = nextRequestDelegate;
			_logger = loggerFactory.CreateLogger<LoggingMiddleware>();
		}

		public async Task Invoke(HttpContext httpContext)
		{
			try
			{
				//First, log the incoming request
				await LogRequest(httpContext.Request);
				//Copy a pointer to the original response body stream
				var originalBodyStream = httpContext.Response.Body;
				//Create a new memory stream...
				using var responseMemoryStream = new MemoryStream();
				//...and use that for the temporary response body
				httpContext.Response.Body = responseMemoryStream;
				//Continue down the Middleware pipeline, eventually returning to this class
				await _nextRequestDelegate(httpContext);
				//Log the response from the server
				await LogResponse(httpContext.Response);
				//Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
				await responseMemoryStream.CopyToAsync(originalBodyStream);
			}
			catch (Exception)
			{
				throw;
			}
		}

		private async Task LogRequest(HttpRequest httpRequest)
		{
			//This line allows us to set the reader for the request back at the beginning of its stream.
			httpRequest.EnableBuffering();
			var requestBody = httpRequest.Body;
			//We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
			var requestBuffer = new byte[Convert.ToInt32(httpRequest.ContentLength)];
			//...Then we copy the entire request stream into the new buffer.
			await httpRequest.Body.ReadAsync(
				buffer: requestBuffer,
				offset: 0,
				count: requestBuffer.Length);
			//We need to read the request stream from the beginning...
			httpRequest.Body.Seek(0, SeekOrigin.Begin);
			//We convert the byte[] into a string using UTF8 encoding...
			var requestBodyString = Encoding.UTF8.GetString(requestBuffer);
			//..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
			httpRequest.Body = requestBody;
			var logMessage = JsonConvert.SerializeObject(new
			{
				Scheme = httpRequest.Scheme,
				Host = httpRequest.Host.HasValue ? httpRequest.Host.ToString() : string.Empty,
				Path = httpRequest.Path.HasValue ? httpRequest.Path.ToString() : string.Empty,
				QueryString = httpRequest.QueryString.HasValue ? httpRequest.QueryString.ToString() : string.Empty,
				Payload = requestBodyString
			});
			_logger.LogInformation(logMessage);
		}

		private async Task LogResponse(HttpResponse httpResponse)
		{
			//We need to read the response stream from the beginning...
			httpResponse.Body.Seek(0, SeekOrigin.Begin);
			//...and copy it into a string
			var responseBodyString = await new StreamReader(httpResponse.Body).ReadToEndAsync();
			//We need to reset the reader for the response so that the client can read it.
			httpResponse.Body.Seek(0, SeekOrigin.Begin);
			//Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
			var logMessage = JsonConvert.SerializeObject(new
			{
				StatusCode = httpResponse.StatusCode,
				Payload = responseBodyString
			});
			_logger.LogInformation(logMessage);
		}
	}
}