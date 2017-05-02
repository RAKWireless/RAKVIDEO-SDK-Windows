using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
//using System.DirectoryServices.Protocols;
//using System.ServiceModel.Security;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Wisview
{
    class HTTP
    {
        /// <summary>
        ///通用HttpWebRequest
        /// </summary>
        /// <param name="method">请求方式（POST/GET）</param>
        /// <param name="url">请求地址</param>
        /// <param name="param">请求参数</param>
        /// <param name="onComplete">完成后执行的操作(可选参数，通过此方法可以获取到HTTP状态码)</param>
        /// <returns>请求返回的结果</returns>
        public static string Request(string method, string url, string admin, string param, Action<HttpStatusCode, string> onComplete = null)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("URL");

            switch (method.ToUpper())
            {
                case "POST":
                    return Post(url, param, onComplete);
                case "GET":
                    return Get(url, admin, param, onComplete);
                default:
                    throw new EntryPointNotFoundException("method:" + method);
            }
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="param">参数</param>
        /// <param name="onComplete">完成后执行的操作(可选参数，通过此方法可以获取到HTTP状态码)</param>
        /// <returns>请求返回的结果</returns>
        public static string Post(string url, string param, Action<HttpStatusCode, string> onComplete = null )
        {
            UrlCheck(ref url);

            byte[] bufferBytes = Encoding.UTF8.GetBytes(param);

            var request = WebRequest.Create(url) as HttpWebRequest;//HttpWebRequest方法继承自WebRequest, Create方法在WebRequest中定义，因此此处要显示的转换
            request.Method = "POST";
            request.ContentLength = bufferBytes.Length;
            request.ContentType = "application/x-www-form-urlencoded";

            try
            {
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bufferBytes, 0, bufferBytes.Length);
                }
            }
            catch (WebException ex)
            {

                return ex.Message;
            }

            return HttpRequest(request, onComplete);
        }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="param">参数</param>
        /// <param name="onComplete">完成后执行的操作(可选参数，通过此方法可以获取到HTTP状态码)</param>
        /// <returns>请求返回结果</returns>
        public static string Get(string url, string admin, string param, Action<HttpStatusCode, string> onComplete = null)
        {
            UrlCheck(ref url);

            if (!string.IsNullOrEmpty(param))
                if (!param.StartsWith("?"))
                    param += "?" + param;
                else
                    param += param;

            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add(admin);

            return HttpRequest(request, onComplete);
        }

        /// <summary>
        /// 请求的主体部分（由此完成请求并返回请求结果）
        /// </summary>
        /// <param name="request">请求的对象</param>
        /// <param name="onComplete">完成后执行的操作(可选参数，通过此方法可以获取到HTTP状态码)</param>
        /// <returns>请求返回结果</returns>
        private static string HttpRequest(HttpWebRequest request, Action<HttpStatusCode, string> onComplete = null)
        {
            HttpWebResponse response = null;

            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }

            if (response == null)
            {
                if (onComplete != null)
                    onComplete(HttpStatusCode.NotFound, "请求远程返回为空");
                return null;
            }

            string result = string.Empty;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }

            if (onComplete != null)
                onComplete(response.StatusCode, result);

            return result;

        }

        /// <summary>
        /// URL拼写完整性检查
        /// </summary>
        /// <param name="url">待检查的URL</param>
        private static void UrlCheck(ref string url)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "http://" + url;
        }
    }
}
