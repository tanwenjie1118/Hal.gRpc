using DataServer;
using gRpc.Library.Server;
using Grpc.Core;
using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace gRpc.Client
{
    class Program
    {
        private static ProxyServer proxyServer;
        /// <summary>
        /// the host every client
        /// </summary>
        private readonly static string clientHost = ConfigurationManager.AppSettings["LocalHost"];
        /// <summary>
        /// the proxy web host every client
        /// </summary>
        private readonly static string proxyHost = ConfigurationManager.AppSettings["ProxyAddress"];
        /// <summary>
        /// the server host
        /// </summary>
        private readonly static string serverHost = ConfigurationManager.AppSettings["ServerHost"];
        /// <summary>
        /// the key can only tag client
        /// </summary>
        private readonly static string clientKey = ConfigurationManager.AppSettings["ClientKey"];
        /// <summary>
        /// the server web host
        /// </summary>
        private readonly static string webHost = ConfigurationManager.AppSettings["WebHost"];

        static void Main(string[] args)
        {
            RunGRpcServer();
            RunProxy();
            Console.WriteLine("press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// run this to build a server for send datas from client to server when get the request from server
        /// </summary>
        private static void RunGRpcServer()
        {
            var splitstr = clientHost.Split(':');
            var port = Convert.ToInt32(splitstr[1]);
            var serverImp = new ServiceImpl();
            Server server = new Server
            {
                Services = { DataServer.DataServer.BindService(serverImp) },
                Ports = { new ServerPort(splitstr[0], port, ServerCredentials.Insecure) }
            };

            server.Start();
        }

        /// <summary>
        /// open a http proxy to replace local client
        /// </summary>
        private static void RunProxy()
        {
            InitProxy(proxyHost);
        }

        private static void GRpcCall()
        {
            Channel channel = default;
            Console.WriteLine("Hello This is your gRpc !");
            Console.WriteLine("================================================================");

            try
            {
                channel = new Channel(serverHost, ChannelCredentials.Insecure);
                var client = new DataServer.DataServer.DataServerClient(channel);

            retryLable:

                Console.WriteLine();
                Console.WriteLine("please insert your transaction code :");
                var input = Console.ReadLine();
                var reply = client.Send(new Input { Client = clientKey, ClientAddress = clientHost, Transaction = input });

                Console.WriteLine("the response =>" + reply.Message);

                goto retryLable;
            }
            catch (Exception ex)
            {
                Console.WriteLine("something wrong..." + ex);
                channel.ShutdownAsync().Wait();
            }
        }

        /// <summary>
        /// create a http proxy
        /// </summary>
        /// <param name="proxyAddress"></param>
        private static void InitProxy(string proxyAddress)
        {
            proxyServer = new ProxyServer
            {
                ForwardToUpstreamGateway = true
            };

            proxyServer.CertificateManager.SaveFakeCertificates = true;

            var proxyAddArray = proxyAddress.Split(':');
            var proxyPort = int.Parse(proxyAddArray[1]);

            proxyServer.BeforeRequest += OnRequest;
            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Parse(proxyAddArray[0]), proxyPort);
            proxyServer.AddEndPoint(explicitEndPoint);
            proxyServer.Start();
        }

        /// <summary>
        /// add client keys to header
        /// </summary>
        /// <param name="sender">1</param>
        /// <param name="e">2</param>
        /// <returns></returns>
        private static Task OnRequest(object sender, SessionEventArgs e)
        {
            //var url = e.HttpClient.Request.Url.ToLower();
            e.HttpClient.Request.Headers.AddHeader("Client", clientKey);
            e.HttpClient.Request.Headers.AddHeader("ClientAddress", clientHost);
            e.HttpClient.Request.Host = webHost;

            return Task.CompletedTask;
        }
    }
}
