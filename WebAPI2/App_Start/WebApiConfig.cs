using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebAPI2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 設定和服務
            //config.EnableCors();

            //全站啟用CORS 設定
            var cors = new EnableCorsAttribute(
                            origins: "http://localhost:25536",
                            headers: "*",
                            methods: "*"
                        );
            config.EnableCors(cors);

            // Web API 路由(2.屬性路由)
            config.MapHttpAttributeRoutes();

            //1.傳統路由
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
