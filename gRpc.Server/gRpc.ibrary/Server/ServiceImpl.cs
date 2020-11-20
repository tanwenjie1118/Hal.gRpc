using Grpc.Core;
using DataServer;
using System.Threading.Tasks;
using static DataServer.DataServer;
using System.Linq;
using System;
using System.Collections.Concurrent;

namespace gRpc.Library.Server
{
    public class ServiceImpl : DataServerBase
    {
        /// <summary>
        /// this is a list of mock data
        /// the mapping of client key and client server uri
        /// in real project , it's better saved in nosql database
        /// </summary>
        private readonly ConcurrentDictionary<string, string> configs = new ConcurrentDictionary<string, string>() { };

        /// <summary>
        /// This is what client callback to center
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<Output> CallBack(Input request, ServerCallContext context)
        {
            return Task.FromResult(new Output { Message = Guid.NewGuid().ToString("N") });
        }

        /// <summary>
        /// this is what server do when received client requests
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<Output> Send(Input request, ServerCallContext context)
        {
            if (!string.IsNullOrWhiteSpace(request.Client) && !string.IsNullOrWhiteSpace(request.ClientAddress))
            {
                configs.AddOrUpdate(request.Client, request.ClientAddress, (x, y) => y);
            }
            else
            {
                return Task.FromResult(new Output { Message = "Failed, no such client" });
            }

            var config = configs.FirstOrDefault(t => t.Key == request.Client);
            var channel = new Channel(config.Value, ChannelCredentials.Insecure);
            var client = new DataServerClient(channel);

            // the real datas we need in from client
            var reply = client.CallBack(new Input { Client = request.Client, Transaction = request.Transaction });

            if (string.IsNullOrWhiteSpace(reply.Message))
            {
                return Task.FromResult(new Output { Message = "Failed, client has no datas to pull" });
            }
            // this place : start a transaction with the datas

            return Task.FromResult(new Output { Message = "Success,transaction compeletes :" + request.Transaction });
        }
    }
}
