using Grpc.Core;
using DataServer;
using System.Threading.Tasks;
using static DataServer.DataServer;
using System.Collections.Generic;
using System.Linq;
using System;

namespace gRpc.Library.Server
{
    public class ServiceImpl : DataServerBase
    {
        /// <summary>
        /// this is a list of mock data
        /// the mapping of client key and client server uri
        /// in real project , it better saved in database
        /// </summary>
        private readonly List<KeyValuePair<string, string>> configs = new List<KeyValuePair<string, string>>() {
          new KeyValuePair<string,string>("org00001","localhost:50052"),
          new KeyValuePair<string,string>("org00002","localhost:50053"),
        };

        /// <summary>
        /// This is what client send to center
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<Output> CallBack(Input request, ServerCallContext context)
        {
            return Task.FromResult(new Output { Message = "this is my datas => " + Guid.NewGuid().ToString("N") });
        }

        /// <summary>
        /// this is what server do
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<Output> Send(Input request, ServerCallContext context)
        {
            var config = configs.FirstOrDefault(t => t.Key == request.Client);

            if (config.GetHashCode() > 0)
            {
                var channel = new Channel(config.Value, ChannelCredentials.Insecure);
                var client = new DataServerClient(channel);
                var reply = client.CallBack(new Input { Client = request.Client, Transaction = request.Transaction });

                // this place : do things with the reply from client
                // TODO reply

                return Task.FromResult(new Output { Message = "Success,transaction compelete :" + request.Transaction });
            }
            else
            {
                return Task.FromResult(new Output { Message = "Failed,no such client" });
            }
        }
    }
}
