using KetNoiDauSo.code.lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace KetNoiDauSo.Controllers
{
    public class SMSController : ApiController
    {
        // GET api/values
        public string Get()
        {
            string data = System.IO.File.ReadAllText(@"C:\Users\sms_ket_noi_dau_so.txt");
            if (data != null)
            {
                return data;
            }
            else
            {
                return "{\"status\":0, \"msg\": \"dịch vụ chưa sẵn sàng\"";
            }
        }

        public string Get(string id, string username)
        {
            if (username != null && username.Length > 0)
            {
                System.IO.File.WriteAllText(@"C:\Users\user_ket_noi_dau_so.txt", username);
                return "{\"status\": 1, \"msg\":\"dịch vụ đã được bật bởi " + username + "\"}";
            }
            else
            {
                System.IO.File.WriteAllText(@"C:\Users\user_ket_noi_dau_so.txt", "");
                return "{\"status\": 0, \"msg\":\"không bật được dịch vụ\"}";
            }
        }

        public string Get(string id, string phone, string content, string service_number, string mo_id, string card_id)
        {
            string username = System.IO.File.ReadAllText(@"C:\Users\user_ket_noi_dau_so.txt");
            if (username != null && username.Length > 0)
            {
                string log = "{\"phone\":\"" + phone + "\", \"content\":\"" + content +
                "\",\"service_number\":\"" + service_number + "\", \"mo_id\":\"" + mo_id + "\",\"card_id\":\"" + card_id + "\"}";
                System.IO.File.WriteAllText(@"C:\Users\ket_noi_dau_so.txt", log);

                SendSMS(phone, content, service_number, mo_id, card_id);

                return "{\"status\": 1, \"msg\":\"đã nhận tin nhắn thành công\"}";
            }
            else
            {
                System.IO.File.WriteAllText(@"C:\Users\sms_ket_noi_dau_so.txt", "{\"status\": 0, \"msg\":\"dịch vụ chưa được bật\"}");
                return "{\"status\": 0, \"msg\":\"dịch vụ chưa được bật\"}";
            }

            
        }

        static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        private  Random random = new Random();
        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        private void SendSMS(string phone, string content, string service_number, string mo_id, string card_id)
        {
            string username = System.IO.File.ReadAllText(@"C:\Users\user_ket_noi_dau_so.txt");
            if (username != null && username.Length > 0)
            {
                string keyUnique = RandomString(20);
                string sign = "";

                HttpResponse<String> response = Unirest.post("http://115.84.178.122:789/api_connect/sendSMSText")
                    .header("accept", "application/json")
                    .connectSMS(mo_id, content, service_number, card_id, phone, username, sign, keyUnique)
                    .asString();
                if (response.Body != null)
                {
                    string result = Regex.Unescape(response.Body);
                    string log =  "{\"request\":{\"phone\":\"" + phone + "\", \"content\":\"" + content + "\", \"service_number\"" + ",result:" +result + "}";
                    System.IO.File.WriteAllText(@"C:\Users\sms_ket_noi_dau_so.txt", log);
                }
            }
            else
            {
                System.IO.File.WriteAllText(@"C:\Users\sms_ket_noi_dau_so.txt", "{\"status\": 0, \"msg\":\"dịch vụ chưa được bật\"}");
            }
            
        }
    }
}
