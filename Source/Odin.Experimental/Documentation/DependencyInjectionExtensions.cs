using Microsoft.OpenApi.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Dependency injection methods to add member documentation
    /// </summary>
    public static class DocumentationExtensions
    {
        /// <summary>
        /// Sets up FluentEmail to from configuration...
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="appName"></param>
        /// <param name="appVersion"></param>
        /// <param name="xmlDocsPath"></param>
        /// <returns></returns>
        public static void AddSwaggerDocumentation(
            this IServiceCollection serviceCollection, string appName, string appVersion, string xmlDocsPath ="Documentation" )
        {
            DirectoryInfo docFolder =
                new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, xmlDocsPath));
            FileInfo[]? xmlDocs = null;

            if (docFolder.Exists)
            {
                xmlDocs = docFolder.GetFiles("*.xml");
            }

            if (xmlDocs != null && xmlDocs.GetLength(0) > 0)
            {
                serviceCollection.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(appVersion, new OpenApiInfo
                    {
                        Title = appName,
                        Version = appVersion
                    });
                    foreach (FileInfo xmlDoc in xmlDocs)
                    {
                        c.IncludeXmlComments(xmlDoc.FullName);
                    }
                });
            }
        }
    }
}