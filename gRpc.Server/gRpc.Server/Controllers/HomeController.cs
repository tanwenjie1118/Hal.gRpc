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
            if (!(info == null))
            {
                ViewBag.Reply = "The Proxy Website";
            }
            else
            {
                ViewBag.Reply = "The Real Website";
            }

            return View();
        }

        public ActionResult GetClientDatas()
        {
            var info = WebHelper.GetClientInfo();
            if (!(info == null))
            {
                var channel = new Channel(info.ClientAddress, ChannelCredentials.Insecure);
                var client = new DataServerClient(channel);

                // the real datas we need pull from client
                var reply = client.CallBack(new Input { Client = info.Client, Transaction = "Index" });
                return Content("This is what we get from client =>" + reply);
            }
            else
            {
                return Content("No connection to client");
            }
        }
    }
}