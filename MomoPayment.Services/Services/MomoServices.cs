using Microsoft.Extensions.Options;
using MomoPayment.Services.ViewModels;
using Services.Interfaces;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Services.Services
{
    public class MomoServices : IMomoServices
    {
        private readonly IOptions<MomoModel> _options;

        public MomoServices(IOptions<MomoModel> options)
        {
            _options = options;
        }

        private string SendMoMoRequest(string postJsonString)
        {
            try
            {
                //var endpoint = Config.Get().MOMO_TEST_ENV_ENDPOINT_API;
                var endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";

                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(endpoint);

                var postData = postJsonString;

                var data = Encoding.UTF8.GetBytes(postData);

                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/json";

                httpWReq.ContentLength = data.Length;
                httpWReq.ReadWriteTimeout = 30000;
                httpWReq.Timeout = 15000;
                Stream stream = httpWReq.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();

                string jsonresponse = "";

                using (var reader = new StreamReader(response.GetResponseStream()))
                {

                    string temp = null;
                    while ((temp = reader.ReadLine()) != null)
                    {
                        jsonresponse += temp;
                    }
                }

                //todo parse it
                return jsonresponse;
                //return new MomoResponse(mtid, jsonresponse);

            }
            catch (WebException e)
            {
                return e.Message;
            }
        }

        public bool VerifyMomoCallback(CallbackViaMomo callbackViaMomo)
        {
            var secretKey = _options.Value.SecretKey;
            var accessKey = _options.Value.AccessKey;

            string rawHash = "accessKey=" + accessKey +
              "&amount=" + callbackViaMomo.Amount +
              "&extraData=" + callbackViaMomo.ExtraData +
              "&message=" + callbackViaMomo.Message +
              "&orderId=" + callbackViaMomo.OrderId +
              "&orderInfo=" + callbackViaMomo.OrderInfo +
              "&orderType=" + callbackViaMomo.OrderType +
              "&partnerCode=" + callbackViaMomo.PartnerCode +
              "&payType=" + callbackViaMomo.PayType +
              "&requestId=" + callbackViaMomo.RequestId +
              "&responseTime=" + callbackViaMomo.ResponseTime +
              "&resultCode=" + callbackViaMomo.ResultCode +
              "&transId=" + callbackViaMomo.TransId
              ;

            string signature = ComputeHmacSha256(rawHash, secretKey);
            if (signature == callbackViaMomo.Signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // create signature for payment
        private string ComputeHmacSha256(string message, string secretKey)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                string hex = BitConverter.ToString(hashmessage);
                hex = hex.Replace("-", "").ToLower();
                return hex;

            }
        }

        
    }
}
