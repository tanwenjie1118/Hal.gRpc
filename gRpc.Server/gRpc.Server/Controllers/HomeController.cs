using DataServer;
using gRpc.Servers.Helpers;
using Grpc.Core;
using System.Web.Mvc;
using static DataServer.DataServer;

namespace gRpc.Servers.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var info = WebHelper.GetClientInfo();
            if (!string.IsNullOrWhiteSpace(info.Client))
            {
                var channel = new Channel(info.ClientAddress, ChannelCredentials.Insecure);
                var client = new DataServerClient(channel);

                // the real datas we need in from client
                var reply = client.CallBack(new Input { Client = info.Client, Transaction = "9201" });
                ViewBag.Reply = reply;
            }
            else
            {
                ViewBag.Reply = "no client connect";
            }

            return View();
        }
    }
}