using DataServer;
using gRpc.Library.Server;
using Grpc.Core;
using System;
using System.Configuration;
namespace gRpc.Client
{
    class Program
    {
        private readonly static string clientHost = ConfigurationManager.AppSettings["LocalHost"];
        private readonly static string serverHost = ConfigurationManager.AppSettings["ServerHost"];
        private readonly static string clientKey = ConfigurationManager.AppSettings["ClientKey"];
        static void Main(string[] args)
        {
            RunServer();
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
                var reply = client.Send(new Input { Client = clientKey, Transaction = input });
                Console.WriteLine("the response =>" + reply.Message);

                goto retryLable;
            }
            catch (Exception ex)
            {
                Console.WriteLine("something wrong..." + ex);
                channel.ShutdownAsync().Wait();
            }

            Console.WriteLine("press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// run this to build a server for send datas from client to server when get the request from server
        /// </summary>
        private static void RunServer()
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
    }
}
