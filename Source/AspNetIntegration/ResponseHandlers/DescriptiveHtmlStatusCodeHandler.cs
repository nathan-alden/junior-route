using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

using Junior.Common;
using Junior.Route.Http.RequestHeaders;
using Junior.Route.Routing.Caching;
using Junior.Route.Routing.Responses;

namespace Junior.Route.AspNetIntegration.ResponseHandlers
{
	public class DescriptiveHtmlStatusCodeHandler : IResponseHandler
	{
		private readonly StatusAndSubStatusCode[] _statusCodes;

		public DescriptiveHtmlStatusCodeHandler(IEnumerable<StatusAndSubStatusCode> handlesStatusCodes)
		{
			handlesStatusCodes.ThrowIfNull("handlesStatusCodes");

			_statusCodes = handlesStatusCodes.ToArray();
		}

		public DescriptiveHtmlStatusCodeHandler(params StatusAndSubStatusCode[] handlesStatusCodes)
			: this((IEnumerable<StatusAndSubStatusCode>)handlesStatusCodes)
		{
		}

		public DescriptiveHtmlStatusCodeHandler(IEnumerable<int> handlesStatusCodes)
			: this(handlesStatusCodes.Select(arg => new StatusAndSubStatusCode(arg)))
		{
		}

		public DescriptiveHtmlStatusCodeHandler(params int[] handlesStatusCodes)
			: this((IEnumerable<int>)handlesStatusCodes)
		{
		}

		public DescriptiveHtmlStatusCodeHandler(IEnumerable<HttpStatusCode> handlesStatusCodes)
			: this(handlesStatusCodes.Select(arg => new StatusAndSubStatusCode(arg)))
		{
		}

		public DescriptiveHtmlStatusCodeHandler(params HttpStatusCode[] handlesStatusCodes)
			: this((IEnumerable<HttpStatusCode>)handlesStatusCodes)
		{
		}

		public async Task<ResponseHandlerResult> HandleResponseAsync(HttpContextBase context, IResponse suggestedResponse, ICache cache, string cacheKey)
		{
			context.ThrowIfNull("context");
			suggestedResponse.ThrowIfNull("suggestedResponse");

			StatusAndSubStatusCode statusCode = suggestedResponse.StatusCode;

			if (!_statusCodes.Contains(statusCode))
			{
				return ResponseHandlerResult.ResponseNotHandled();
			}

			AcceptHeader[] acceptHeaders = AcceptHeader.ParseMany(context.Request.Headers["Accept"]).ToArray();

			if (acceptHeaders.Any() && !acceptHeaders.Any(arg => arg.MediaTypeMatches("text/html")))
			{
				return ResponseHandlerResult.ResponseNotHandled();
			}

			const string format = @"<!DOCTYPE html>
<html>
	<head>
		<title>{0}</title>
		<style>h1 {{ margin: 0; padding: 0; }}</style>
	</head>
	<body>
		<h1>{0}</h1>
		<hr/>
		HTTP {1}{2}
	</body>
</html>";
			Response response = new Response(statusCode)
				.TextHtml()
				.Content(String.Format(format, statusCode.StatusDescription, statusCode.StatusCode, statusCode.SubStatusCode == 0 ? "" : "." + statusCode.SubStatusCode));

			response.CachePolicy.NoClientCaching();

			await new CacheResponse(response).WriteResponseAsync(context.Response);

			context.Response.TrySkipIisCustomErrors = true;

			return ResponseHandlerResult.ResponseWritten();
		}
	}
}