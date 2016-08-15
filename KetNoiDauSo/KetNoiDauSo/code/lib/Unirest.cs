using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace KetNoiDauSo.code.lib
{
    public class Unirest
    {
        public static HttpRequest post(string url)
        {
            return new HttpRequest(HttpMethod.Post, url);
        }
    }
}