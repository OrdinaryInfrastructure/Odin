using Microsoft.AspNetCore.Http;

namespace Odin.Web
{
    /// <summary>
    /// Cookie utilities
    /// </summary>
    public static class CookieUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="options"></param>
        public static void HandleUserAgentSameSiteAnomalies(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                string userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                if (string.IsNullOrEmpty(userAgent))
                {
                    options.SameSite = SameSiteMode.None;
                    options.Secure = true;
                    return;
                }

                // Cover all iOS based browsers here. This includes:
                // - Safari on iOS 12 for iPhone, iPod Touch, iPad
                // - WkWebview on iOS 12 for iPhone, iPod Touch, iPad
                // - Chrome on iOS 12 for iPhone, iPod Touch, iPad
                // All of which are broken by SameSite=None, because they use the iOS networking stack
                if (userAgent.Contains("CPU iPhone OS 12") || userAgent.Contains("iPad; CPU OS 12"))
                {
                    options.SameSite = SameSiteMode.Unspecified;
                    return;
                }

                // Cover Mac OS X based browsers that use the Mac OS networking stack. This includes:
                // - Safari on Mac OS X.
                // This does not include:
                // - Chrome on Mac OS X
                // Because they do not use the Mac OS networking stack.
                if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
                    userAgent.Contains("Version/") && userAgent.Contains("Safari"))
                {
                    options.SameSite = SameSiteMode.Unspecified;
                    return;
                }

                // Cover Chrome 50-69, because some versions are broken by SameSite=None, 
                // and none in this range require it.
                // Note: this covers some pre-Chromium Edge versions, 
                // but pre-Chromium Edge does not require SameSite=None.
                if (userAgent.Contains("Chrome"))
                {
                    options.SameSite = SameSiteMode.Unspecified;
                }
            }
        }
    }
}