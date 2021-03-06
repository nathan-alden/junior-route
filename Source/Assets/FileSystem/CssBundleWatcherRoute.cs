using System;
using System.Web;

using Junior.Common;
using Junior.Route.Common;
using Junior.Route.Routing;
using Junior.Route.Routing.Responses;
using Junior.Route.Routing.Responses.Text;

namespace Junior.Route.Assets.FileSystem
{
	public class CssBundleWatcherRoute : BundleWatcherRoute<CssResponse>
	{
		private readonly ISystemClock _systemClock;

		public CssBundleWatcherRoute(string name, Guid id, Scheme scheme, string relativePath, BundleWatcher watcher, IHttpRuntime httpRuntime, ISystemClock systemClock)
			: base(name, id, scheme, relativePath, watcher, httpRuntime)
		{
			systemClock.ThrowIfNull("systemClock");

			_systemClock = systemClock;
		}

		protected override CssResponse GetResponse(HttpContextBase context, string bundleContents)
		{
			context.ThrowIfNull("context");
			bundleContents.ThrowIfNull("bundleContents");

			return new CssResponse(bundleContents, ConfigureResponse);
		}

		private void ConfigureResponse(Response response)
		{
			DateTime expirationUtcTimestamp = _systemClock.UtcDateTime.AddYears(1);

			response.CachePolicy
				.ServerCaching(expirationUtcTimestamp)
				.PublicClientCaching(expirationUtcTimestamp)
				.ETag(Id.ToString("N"));
		}
	}
}