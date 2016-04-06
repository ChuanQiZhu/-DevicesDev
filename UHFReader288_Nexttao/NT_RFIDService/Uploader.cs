using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace NT_RFIDService
{
    public class Uploader
    {
        private static ILog log = log4net.LogManager.GetLogger(typeof(Uploader));

        // 发送读写器数据
        public static void uploadReaderData(object param)
        {
            if (param == null) return;
            Array paramArray = new Object[3];
            paramArray = (Array)param;
            string url = (string)paramArray.GetValue(0);
            List<EPC> data = (List<EPC>)paramArray.GetValue(1);
            string DeviceNo = (string)paramArray.GetValue(2);
            // 序列化
            var jsonString = JSON.stringify(data);
            //使用FormUrlEncodedContent做HttpContent
            string jsonData = "{\"DeviceNo\":\"" + DeviceNo + "\", \"EpcList\":" + jsonString + " }";

            upload(url, DeviceNo, jsonData);
        }


        // 发送读写器连接日志数据
        public static void uploadLogData(object param)
        {
            if (param == null) return;
            Array paramArray = new Object[3];
            paramArray = (Array)param;
            string url = (string)paramArray.GetValue(0);
            string jsonString = (string)paramArray.GetValue(1);
            string DeviceNo = (string)paramArray.GetValue(2);
            string jsonData = "{\"DeviceNo\":\"" + DeviceNo + "\", \"logInfo\":\"" + jsonString + "\" }";

            log.Info("url in uploadLogData(): " + url);

            upload(url, DeviceNo, jsonData);
        }


        public static void upload(string url, string DeviceNo, string jsonData)
        {
            try
            {
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
                    log.Info("Success to upload --- data:" + jsonData + " to " + url + ", Result: " + sr.ReadToEnd());
                }


            }
            catch (Exception e)
            {
                log.Warn("Fail to upload data " + jsonData + " to " + url, e);
            }
        }

    }
}
