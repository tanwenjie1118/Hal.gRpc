using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gRpc.Library.Model
{
   public class CertificationModel
    {
        public CertificationModel(string x,string y)
        {
            Client = x;
            ClientAddress = y;
        }

        public string Client { set; get; }
        public string ClientAddress { set; get; }
    }
}
