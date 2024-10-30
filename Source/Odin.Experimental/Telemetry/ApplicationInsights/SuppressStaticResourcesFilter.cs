using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Odin.Logging.ApplicationInsights
{
    /// <summary>
    /// Filter out css, js, and image Request telemetry from AppInsights
    /// </summary>
    public sealed class SuppressStaticResourcesFilter : ITelemetryProcessor
    {
        private List<string> _suppressExtensions { get; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next"></param>
        public SuppressStaticResourcesFilter(ITelemetryProcessor next)
        {
            Next = next;
            _suppressExtensions = DefaultExtensions;
        }
        
        /// <summary>
        /// Constructor with extensions list
        /// </summary>
        /// <param name="next"></param>
        /// <param name="commaSeparatedListOfRequestNameEndings"></param>
        public SuppressStaticResourcesFilter(ITelemetryProcessor next, string commaSeparatedListOfRequestNameEndings)
        {
            Next = next;
            if (string.IsNullOrWhiteSpace(commaSeparatedListOfRequestNameEndings))
            {
                _suppressExtensions = DefaultExtensions;
            }
            else
            {
                _suppressExtensions = commaSeparatedListOfRequestNameEndings.Split(',').ToList();
            }
        }
        

        private ITelemetryProcessor Next { get; set; }

        /// <summary>
        /// Defaut extensions
        /// </summary>
        public static readonly List<string> DefaultExtensions = new List<string>
            {".css", ".js",".png",".jpg", ".gif",".jpeg", ".ico", ".woff", ".woff2", ".svg",".ttf"};


        /// <summary>
        /// ITelemetry.Process implementation
        /// </summary>
        /// <param name="item"></param>
        public void Process(ITelemetry item)
        {
            // To exclude static requests from our telemetry we should use RequestTelemetry
            RequestTelemetry req = item as RequestTelemetry;
            if (req != null)
            {
                for (int i = 0; i < _suppressExtensions.Count; i++)
                {
                    if (req.Name.EndsWith(_suppressExtensions[i], StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                }
            }

            // Send everything else
            Next.Process(item);
        }
    }
}