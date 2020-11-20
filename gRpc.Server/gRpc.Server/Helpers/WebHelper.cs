using gRpc.Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gRpc.Servers.Helpers
{
    public static class WebHelper
    {
        /// <summary>
        /// 鉴权字符串
        /// </summary>
        private static string Client = nameof(Client);
        private static string ClientAddress = nameof(ClientAddress);

        /// <summary>
        /// 获取客户端信息
        /// </summary>
        /// <returns></returns>
        public static CertificationModel GetClientInfo()
        {
            var x = HttpContext.Current.Request.Headers[Client];
            var y = HttpContext.Current.Request.Headers[ClientAddress];

            if (string.IsNullOrWhiteSpace(x) || string.IsNullOrWhiteSpace(y))
            {
                return null;
            }

            return new CertificationModel(x, y);
        }
    }
}