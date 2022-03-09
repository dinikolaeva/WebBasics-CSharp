using SUS.HTTP;
using HttpMethod = SUS.HTTP.HttpMethod;

namespace SUS.MvcFramework
{
    public static class Host
    {
        public static async Task CreateHostAsync(IMvcApplication application, int port = 80)
        {
            List<Route> routeTable = new List<Route>();
            AutoRegisterStaticFiles(routeTable);
            AutoRegisterRoutes(routeTable, application);

            application.ConfigureServices();
            application.Configure(routeTable);

            IHttpServer server = new HttpServer(routeTable);

            await server.StartAsync(port);
        }

        private static void AutoRegisterRoutes(List<Route> routeTable, IMvcApplication application)
        {
            var controllerTypes = application.GetType()
                                             .Assembly
                                             .GetTypes()
                                             .Where(x => x.IsClass &&
                                                        !x.IsAbstract &&
                                                         x.IsSubclassOf(typeof(Controller)));

            foreach (var controllerType in controllerTypes)
            {
                var methods = controllerType.GetMethods()
                                            //use reflection to remove all methods that are not                      actions
                                            .Where(x => x.IsPublic &&
                                                       !x.IsStatic &&
                                                        x.DeclaringType == controllerType &&
                                                       !x.IsAbstract &&
                                                       !x.IsConstructor &&
                                                       //remove get and set properties methods!
                                                       !x.IsSpecialName);
                foreach (var method in methods)
                {
                    var url = "/" + controllerType.Name.Replace("Controller", string.Empty) + "/" + method.Name;

                    //get method attributes and use reflection to find http method attribute(get(deafult) or post)
                    var attribute = method.GetCustomAttributes(false)
                                          .Where(x => x.GetType()
                                          .IsSubclassOf(typeof(BaseHttpAttribute)))
                                          .FirstOrDefault() as BaseHttpAttribute;

                    var httpMethod = HttpMethod.Get;

                    if (attribute != null)
                    {
                        httpMethod = attribute.Method;
                    }

                    if (!string.IsNullOrEmpty(attribute?.Url))
                    {
                        url = attribute.Url;
                    }

                    routeTable.Add(new Route(url, httpMethod, (request) =>
                     {
                         var instance = Activator.CreateInstance(controllerType) as Controller;
                         instance.Request = request;
                         var response = method.Invoke(instance, new object[] { }) as HttpResponse;
                         return response;
                     }));
                }
            }
        }

        private static void AutoRegisterStaticFiles(List<Route> routeTable)
        {
            var staticFiles = Directory.GetFiles("wwwroot", "*", SearchOption.AllDirectories);

            foreach (var staticFile in staticFiles)
            {
                var url = staticFile.Replace("wwwroot", string.Empty)
                                    .Replace("\\", "/");

                routeTable.Add(new Route(url, HttpMethod.Get, (request) =>
                {
                    var fileContent = File.ReadAllBytes(staticFile);
                    var fileExt = new FileInfo(staticFile).Extension;
                    var contentType = fileExt switch
                    {
                        ".txt" => "text/plain",
                        ".js" => "text/javascript",
                        ".css" => "text/css",
                        ".jpg" => "image/jpg",
                        ".jpeg" => "image/jpg",
                        ".png" => "image/png",
                        ".gif" => "image/gif",
                        ".ico" => "image/vnd.microsoft.icon",
                        ".html" => "text/html",
                        _ => "text/plain"
                    };

                    return new HttpResponse(contentType, fileContent, HttpStatusCode.OK);
                }));
            }
        }
    }
}
