﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RefundCgas
{
    public class httpHelper
    {
        public static string MakeRpcUrlPost(string url, string method, out byte[] data, params JValue[] _params)
        {
            var json = new JObject();
            json["id"] = 1;
            json["jsonrpc"] = "2.0";
            json["method"] = method;
            StringBuilder sb = new StringBuilder();
            var array = new JArray();
            for (var i = 0; i < _params.Length; i++)
            {

                array.Add(_params[i]);
            }
            json["params"] = array;
            data = System.Text.Encoding.UTF8.GetBytes(json.ToString());
            return url;
        }

        public static string HttpGet(string url)
        {
            WebClient wc = new WebClient();
            return wc.DownloadString(url);
        }
        public static string HttpPost(string url, byte[] data)
        {
            WebClient wc = new WebClient();
            wc.Headers["content-type"] = "text/plain;charset=UTF-8";
            byte[] retdata = wc.UploadData(url, "POST", data);
            return System.Text.Encoding.UTF8.GetString(retdata);
        }


        //流模式post
        public static string Post(string url, string data, Encoding encoding, int type = 3)
        {
            HttpWebRequest req = null;
            HttpWebResponse rsp = null;
            Stream reqStream = null;
            //Stream resStream = null;

            try
            {
                req = WebRequest.CreateHttp(new Uri(url));
                if (type == 1)
                {
                    req.ContentType = "application/json;charset=utf-8";
                }
                else if (type == 2)
                {
                    req.ContentType = "application/xml;charset=utf-8";
                }
                else
                {
                    req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                }

                req.Method = "POST";
                //req.Accept = "text/xml,text/javascript";
                req.ContinueTimeout = 60000;

                byte[] postData = encoding.GetBytes(data);
                reqStream = req.GetRequestStreamAsync().Result;
                reqStream.Write(postData, 0, postData.Length);
                //reqStream.Dispose();

                rsp = (HttpWebResponse)req.GetResponseAsync().Result;
                string result = GetResponseAsString(rsp, encoding);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // 释放资源
                if (reqStream != null)
                {
                    reqStream.Close();
                    reqStream = null;
                }
                if (rsp != null)
                {
                    rsp.Close();
                    rsp = null;
                }
                if (req != null)
                {
                    req.Abort();

                    req = null;
                }
            }
        }

        private static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);
                return reader.ReadToEnd();
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Dispose();
                if (stream != null) stream.Dispose();
                if (rsp != null) rsp.Dispose();
            }
        }
    }
}
