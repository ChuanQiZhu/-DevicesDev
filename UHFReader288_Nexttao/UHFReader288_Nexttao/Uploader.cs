using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace UHFReader288_Nexttao
{
    public class Uploader
    {
        private static ILog log = log4net.LogManager.GetLogger(typeof(Uploader));

        public static void upload1(object param)
        {
            if (param == null) return;
            Array paramArray = new Object[3];
            paramArray = (Array)param;
            string url = (string)paramArray.GetValue(0);
            List<EPC> data = (List<EPC>)paramArray.GetValue(1);
            string DeviceNo = (string)paramArray.GetValue(2);
            // 序列化
            var jsonString = JSON.stringify(data);
            try
            {
                //使用FormUrlEncodedContent做HttpContent
                string jsonData = "{\"DeviceNo\":\"" + DeviceNo + "\", \"EpcList\":" + jsonString + " }";
                
                Console.WriteLine(jsonData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "POST";
                byte[] bytePost = Encoding.UTF8.GetBytes(jsonData);
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytePost, 0, bytePost.Length);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    log.Info("Success to upload --- data:" + jsonString + " to " + url + ", Result: " + sr.ReadToEnd());
                }

            }
            catch (Exception e)
            {
                log.Warn("Fail to upload data " + jsonString + " to " + url, e);
            }
        }

    }
}
