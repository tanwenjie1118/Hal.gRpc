using gRpc.Library.Server;
using Grpc.Core;
using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace gRpc.Servers
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly string serverHost = ConfigurationManager.AppSettings["ServerHost"];
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            try
            {
                // run gRpc
                var splitstr = serverHost.Split(':');
                var port = Convert.ToInt32(splitstr[1]);
                var serverImp = new ServiceImpl();
                Server server = new Server
                {
                    Services = { DataServer.DataServer.BindService(serverImp) },
                    Ports = { new ServerPort(splitstr[0], port, ServerCredentials.Insecure) }
                };

                server.Start();
            }
            catch (Exception ex)
            {
                throw new Exception("gRpc service start failed", ex);
            }
        }
    }
}
