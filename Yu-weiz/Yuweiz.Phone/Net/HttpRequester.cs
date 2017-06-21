using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Yuweiz.Phone.Net
{
    public class HttpRequester
    {
        public HttpRequester()
        {
            BaseAddress = string.Empty;
        }

        public HttpRequester(string baseUrl)
        {
            if (baseUrl == null)
            {
                BaseAddress = string.Empty;
            }
            BaseAddress = baseUrl;
        }

        /// <summary>
        /// 基本地址：加在url前
        /// </summary>
        public string BaseAddress { get; set; }

        #region 异步回调

        /// <summary>
        /// 向指定的地址，请求数据
        /// 完成后，执行回调函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestCallBack"></param>
        /// <author>庾伟荣</author>
        public void Post(string url, Action<string> requestCallBack)
        {
            url = BaseAddress + url;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST"; //httpRequest.Method = "GET"时禁用缓存策略失效
            httpRequest.Headers["Pragma"] = "no-cache";
            httpRequest.Headers["Cache-Control"] = "no-cache";
            httpRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";  //GET方式此项需注释掉，否则引发异常       

            httpRequest.BeginGetRequestStream(hr =>
            {
                HttpWebRequest httpRequest1 = (HttpWebRequest)hr.AsyncState;
                System.IO.Stream postStream = httpRequest1.EndGetRequestStream(hr);
                string postData = string.Empty;
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
                postStream.Write(byteArray, 0, postData.Length);
                postStream.Close();  //必须要关闭数据流
                httpRequest1.BeginGetResponse(hr1 =>
                {
                    WebResponse httpResponse1 = null;
                    try
                    {
                        httpResponse1 = httpRequest1.EndGetResponse(hr1);
                        using (var reader = new System.IO.StreamReader(httpResponse1.GetResponseStream(), System.Text.UTF8Encoding.UTF8))
                        {
                            string xmStream = reader.ReadToEnd();
                            reader.Close();

                            Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                // MessageBox.Show("下载成功");
                                requestCallBack(xmStream);
                            }));

                        }
                    }
                    catch (Exception ex)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {                            //MessageBox.Show(ex.Message);
                            requestCallBack(ex.Message);
                        }));
                    }
                    finally
                    {
                        if (httpRequest != null) httpRequest.Abort();
                        if (httpRequest1 != null) httpRequest1.Abort();
                        if (httpResponse1 != null) httpResponse1.Close();
                    }
                }, httpRequest1);
            }, httpRequest);
        }

        /// <summary>
        /// 向指定的地址，请求数据
        /// 完成后，执行回调函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestCallBack"></param>
        /// <author>庾伟荣</author>
        public async void Get(string url, Action<string> requestCallBack)
        {
            url = BaseAddress + url;

            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Accept = "application/json;odata=verbose";
            // Use the Task Parallel Library pattern
            var factory = new TaskFactory();
            var task = factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

            try
            {
                var response = await task;
                System.IO.Stream responseStream = response.GetResponseStream();
                string data;
                using (var reader = new System.IO.StreamReader(responseStream))
                {
                    data = reader.ReadToEnd();
                }
                responseStream.Close();

                if (requestCallBack != null)
                {
                    requestCallBack(data);
                }
            }
            catch (Exception ex)
            {
                requestCallBack(ex.Message);
            }
        }

        /// <summary>
        /// 向指定的地址，请求数据
        /// 完成后，执行回调函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestCallBack"></param>
        /// <author>庾伟荣</author>
        public void PostForStream(string url, Action<Stream> requestCallBack)
        {
            url = BaseAddress + url;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST"; //httpRequest.Method = "GET"时禁用缓存策略失效
            httpRequest.Headers["Pragma"] = "no-cache";
            httpRequest.Headers["Cache-Control"] = "no-cache";
            httpRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";  //GET方式此项需注释掉，否则引发异常       

            httpRequest.BeginGetRequestStream(hr =>
            {
                HttpWebRequest httpRequest1 = (HttpWebRequest)hr.AsyncState;
                System.IO.Stream postStream = httpRequest1.EndGetRequestStream(hr);
                string postData = string.Empty;
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
                postStream.Write(byteArray, 0, postData.Length);
                postStream.Close();  //必须要关闭数据流
                httpRequest1.BeginGetResponse(hr1 =>
                {
                    WebResponse httpResponse1 = null;
                    try
                    {
                        httpResponse1 = httpRequest1.EndGetResponse(hr1);
                        Stream stream = httpResponse1.GetResponseStream();
                        if (requestCallBack != null)
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {                            //MessageBox.Show(ex.Message);
                                requestCallBack(stream);
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {                            //MessageBox.Show(ex.Message);
                            requestCallBack(null);
                        }));
                    }
                    finally
                    {
                        if (httpRequest != null) httpRequest.Abort();
                        if (httpRequest1 != null) httpRequest1.Abort();
                        if (httpResponse1 != null) httpResponse1.Close();
                    }
                }, httpRequest1);
            }, httpRequest);
        }

        /// <summary>
        /// 向指定的地址，请求数据
        /// 完成后，执行回调函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestCallBack"></param>
        /// <author>庾伟荣</author>
        public async void GetForStream(string url, Action<Stream> requestCallBack)
        {
            url = BaseAddress + url;

            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Accept = "application/json;odata=verbose";
            // Use the Task Parallel Library pattern
            var factory = new TaskFactory();
            var task = factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

            try
            {
                var response = await task;
                System.IO.Stream responseStream = response.GetResponseStream();
                requestCallBack(responseStream);

                if (requestCallBack != null)
                {
                    requestCallBack(null);
                }
            }
            catch (Exception ex)
            {
                requestCallBack(null);
            }
        }

        /// <summary>
        /// 向指定的地址，请求数据
        /// 完成后，执行回调函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestCallBack"></param>
        /// <author>庾伟荣</author>
        public void Post(string url, byte[] dataByteArray, Action<string> requestCallBack)
        {
            url = BaseAddress + url;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST"; //httpRequest.Method = "GET"时禁用缓存策略失效
            httpRequest.Headers["Pragma"] = "no-cache";
            httpRequest.Headers["Cache-Control"] = "no-cache";
            httpRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";  //GET方式此项需注释掉，否则引发异常       

            httpRequest.BeginGetRequestStream(hr =>
            {
                HttpWebRequest httpRequest1 = (HttpWebRequest)hr.AsyncState;
                System.IO.Stream postStream = httpRequest1.EndGetRequestStream(hr);
                postStream.Write(dataByteArray, 0, dataByteArray.Length);
                postStream.Close();  //必须要关闭数据流
                httpRequest1.BeginGetResponse(hr1 =>
                {
                    WebResponse httpResponse1 = null;
                    try
                    {
                        httpResponse1 = httpRequest1.EndGetResponse(hr1);
                        using (var reader = new System.IO.StreamReader(httpResponse1.GetResponseStream(), System.Text.UTF8Encoding.UTF8))
                        {
                            string xmStream = reader.ReadToEnd();
                            reader.Close();

                            Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                // MessageBox.Show("下载成功");
                                requestCallBack(xmStream);
                            }));

                        }
                    }
                    catch (Exception ex)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {                            //MessageBox.Show(ex.Message);
                            requestCallBack(ex.Message);
                        }));
                    }
                    finally
                    {
                        if (httpRequest != null) httpRequest.Abort();
                        if (httpRequest1 != null) httpRequest1.Abort();
                        if (httpResponse1 != null) httpResponse1.Close();
                    }
                }, httpRequest1);
            }, httpRequest);
        }

        /// <summary>
        /// 分段上传
        /// </summary>
        /// <param name="url">要上传的http目标地址</param>
        /// <param name="data">要上传的数据</param>
        /// <param name="requestCallBack"></param>
        public async void PostPiecewise(string url, byte[] data, Action<string> requestCallBack)
        {
            url = BaseAddress + url;

            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            var factory = new TaskFactory();
            var task = factory.FromAsync<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream, null);
            try
            {
                var response = await task;
                System.IO.Stream requestStream = response;

                // 把 data 对象保存到 Stream 对象中
                MemoryStream memoryStream = new MemoryStream(data);

                // 如果数据小于 8KB，则一次上次；如果大于 8KB，则分段上传，每次 8KB
                byte[] buffer = new Byte[checked((uint)Math.Min(4096 * 2, data.Length))];
                memoryStream.Seek(0, SeekOrigin.Begin);
                int bytesRead = 0;

                while ((bytesRead = memoryStream.Read(buffer, 0, buffer.Length)) != 0)
                    requestStream.Write(buffer, 0, bytesRead);
                requestStream.Close();
                request.BeginGetResponse((result =>
                {
                    #region 请求完成
                    HttpWebRequest requestback = result.AsyncState as HttpWebRequest;
                    WebResponse responseback = null;
                    try
                    {
                        responseback = requestback.EndGetResponse(result);
                    }
                    catch (Exception ex)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            requestCallBack(ex.Message);
                        }));
                        return;
                    }

                    HttpWebResponse httpResponse = responseback as HttpWebResponse;
                    if (httpResponse != null && httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Stream responseStream = responseback.GetResponseStream();
                        using (StreamReader sr = new StreamReader(responseStream))
                        {
                            string Text = sr.ReadToEnd();
                            Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {                            //MessageBox.Show(ex.Message);
                                requestCallBack(Text);
                            }));
                        }
                    }
                    #endregion

                }), request);
            }

            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                {                            //MessageBox.Show(ex.Message);
                    requestCallBack(ex.Message);
                }));
            }
        }

        public void PostMultipart(string url, Dictionary<string, Stream> dict,
                   System.Collections.Generic.Dictionary<string, string> stringDict, Action<string> requestCallBack)
        {
            url = BaseAddress + url;

            StringBuilder sb = new StringBuilder();
            var memStream = new MemoryStream();
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers["Accept-Encoding"] = "utf-8";
            // 边界符  
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            // 边界符  
            var beginBoundary = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            var dicBeginBoundary = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            //var fileStream = Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.OpenFile(filePath);
            //var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            // 最后的结束符  
            var endBoundary = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            // 设置属性  
            webRequest.Method = "POST";
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

            if (!(dict != null && dict.Count > 0))
            {
                memStream.Write(beginBoundary, 0, beginBoundary.Length);
                sb.Append("--" + boundary + "\r\n");
            }

            foreach (var item in dict)
            {
                string fileKeyName = item.Key;
                Stream fileStream = item.Value;
                fileStream.Seek(0, SeekOrigin.Begin);

                // 写入文件  
                const string filePartHeader =
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                     "Content-Type:application/octet-stream\r\n\r\n";
                var header = string.Format(filePartHeader, fileKeyName, fileKeyName);
                var headerbytes = Encoding.UTF8.GetBytes(header);

                sb.Append("\r\n--" + boundary + "\r\n");
                sb.Append(header);
                memStream.Write(dicBeginBoundary, 0, dicBeginBoundary.Length);
                memStream.Write(headerbytes, 0, headerbytes.Length);

                var buffer = new byte[fileStream.Length];
                int bytesRead; // =0  
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }
                //--------------------------
                fileStream.Close();
                //sb.Append("-0afsafdfContent");
            }
            // 写入字符串的Key  
            //var stringKeyHeader = "\r\n--" + boundary +
            //                       "\r\nContent-Disposition: form-data; name=\"{0}\"" +
            //                       "\r\n\r\n{1}\r\n";

            //foreach (byte[] formitembytes in from string key in stringDict.Keys
            //                                 select string.Format(stringKeyHeader, key, stringDict[key])
            //                                     into formitem
            //                                     select Encoding.UTF8.GetBytes(formitem))
            //{
            //    memStream.Write(formitembytes, 0, formitembytes.Length);
            //}
            foreach (var item in stringDict)
            {
                var stringKeyHeader = "\r\n--" + boundary +
                                       "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                       "\r\n\r\n{1}";
                string s = string.Format(stringKeyHeader, item.Key, item.Value);
                byte[] formitembytes = Encoding.UTF8.GetBytes(s);
                memStream.Write(formitembytes, 0, formitembytes.Length);
                sb.Append(s);
            }


            // 写入最后的结束边界符  
            sb.Append("\r\n--" + boundary + "--\r\n");
            memStream.Write(endBoundary, 0, endBoundary.Length);
            webRequest.ContentLength = memStream.Length;
            var dataByteArray = Yuweiz.Phone.IO.StreamConvert.StreamToBytes(memStream);

            //int i = 0;
            //foreach (byte b in dataByteArray)
            //{
            //    i++;
            //    if (b != 0)
            //    {

            //        requestCallBack(i.ToString());
            //        break;
            //    }
            //}

            #region
            webRequest.BeginGetRequestStream(hr =>
            {
                HttpWebRequest httpRequest1 = (HttpWebRequest)hr.AsyncState;
                System.IO.Stream postStream = httpRequest1.EndGetRequestStream(hr);
                postStream.Write(dataByteArray, 0, dataByteArray.Length);
                postStream.Close();  //必须要关闭数据流
                httpRequest1.BeginGetResponse(hr1 =>
                {
                    WebResponse httpResponse1 = null;
                    try
                    {
                        httpResponse1 = httpRequest1.EndGetResponse(hr1);
                        using (var reader = new System.IO.StreamReader(httpResponse1.GetResponseStream(), System.Text.UTF8Encoding.UTF8))
                        {
                            string xmStream = reader.ReadToEnd();
                            reader.Close();

                            Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                // MessageBox.Show("下载成功");
                                requestCallBack(xmStream);
                            }));

                        }
                    }
                    catch (Exception ex)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {                            //MessageBox.Show(ex.Message);
                            requestCallBack(ex.Message);
                        }));
                    }
                    finally
                    {
                        if (webRequest != null) webRequest.Abort();
                        if (httpRequest1 != null) httpRequest1.Abort();
                        if (httpResponse1 != null) httpResponse1.Close();
                    }
                }, httpRequest1);
            }, webRequest);
            #endregion
        }

        public void PostMultipart(string url, string keyName, List<Stream> listStream,
              System.Collections.Generic.Dictionary<string, string> stringDict, Action<string> requestCallBack)
        {
            url = BaseAddress + url;

            StringBuilder sb = new StringBuilder();
            var memStream = new MemoryStream();
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers["Accept-Encoding"] = "utf-8";
            // 边界符  
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            // 边界符  
            var beginBoundary = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            var dicBeginBoundary = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            //var fileStream = Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.OpenFile(filePath);
            //var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            // 最后的结束符  
            var endBoundary = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            // 设置属性  
            webRequest.Method = "POST";
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

            if (!(listStream != null && listStream.Count > 0))
            {
                memStream.Write(beginBoundary, 0, beginBoundary.Length);
                sb.Append("--" + boundary + "\r\n");
            }

            foreach (var item in listStream)
            {
                string fileKeyName = keyName;
                Stream fileStream = item;
                fileStream.Seek(0, SeekOrigin.Begin);

                // 写入文件  
                const string filePartHeader =
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                     "Content-Type:application/octet-stream\r\n\r\n";
                var header = string.Format(filePartHeader, fileKeyName, fileKeyName);
                var headerbytes = Encoding.UTF8.GetBytes(header);

                sb.Append("\r\n--" + boundary + "\r\n");
                sb.Append(header);
                memStream.Write(dicBeginBoundary, 0, dicBeginBoundary.Length);
                memStream.Write(headerbytes, 0, headerbytes.Length);

                var buffer = new byte[fileStream.Length];
                int bytesRead; // =0  
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }
                //--------------------------
                fileStream.Close();
                //sb.Append("-0afsafdfContent");
            }
            // 写入字符串的Key  
            //var stringKeyHeader = "\r\n--" + boundary +
            //                       "\r\nContent-Disposition: form-data; name=\"{0}\"" +
            //                       "\r\n\r\n{1}\r\n";

            //foreach (byte[] formitembytes in from string key in stringDict.Keys
            //                                 select string.Format(stringKeyHeader, key, stringDict[key])
            //                                     into formitem
            //                                     select Encoding.UTF8.GetBytes(formitem))
            //{
            //    memStream.Write(formitembytes, 0, formitembytes.Length);
            //}
            foreach (var item in stringDict)
            {
                var stringKeyHeader = "\r\n--" + boundary +
                                       "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                       "\r\n\r\n{1}";
                string s = string.Format(stringKeyHeader, item.Key, item.Value);
                byte[] formitembytes = Encoding.UTF8.GetBytes(s);
                memStream.Write(formitembytes, 0, formitembytes.Length);
                sb.Append(s);
            }


            // 写入最后的结束边界符  
            sb.Append("\r\n--" + boundary + "--\r\n");
            memStream.Write(endBoundary, 0, endBoundary.Length);
            webRequest.ContentLength = memStream.Length;
            var dataByteArray = Yuweiz.Phone.IO.StreamConvert.StreamToBytes(memStream);

            //int i = 0;
            //foreach (byte b in dataByteArray)
            //{
            //    i++;
            //    if (b != 0)
            //    {

            //        requestCallBack(i.ToString());
            //        break;
            //    }
            //}

            #region
            webRequest.BeginGetRequestStream(hr =>
            {
                HttpWebRequest httpRequest1 = (HttpWebRequest)hr.AsyncState;
                System.IO.Stream postStream = httpRequest1.EndGetRequestStream(hr);
                postStream.Write(dataByteArray, 0, dataByteArray.Length);
                postStream.Close();  //必须要关闭数据流
                httpRequest1.BeginGetResponse(hr1 =>
                {
                    WebResponse httpResponse1 = null;
                    try
                    {
                        httpResponse1 = httpRequest1.EndGetResponse(hr1);
                        using (var reader = new System.IO.StreamReader(httpResponse1.GetResponseStream(), System.Text.UTF8Encoding.UTF8))
                        {
                            string xmStream = reader.ReadToEnd();
                            reader.Close();

                            Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                // MessageBox.Show("下载成功");
                                requestCallBack(xmStream);
                            }));

                        }
                    }
                    catch (Exception ex)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {                            //MessageBox.Show(ex.Message);
                            requestCallBack(ex.Message);
                        }));
                    }
                    finally
                    {
                        if (webRequest != null) webRequest.Abort();
                        if (httpRequest1 != null) httpRequest1.Abort();
                        if (httpResponse1 != null) httpResponse1.Close();
                    }
                }, httpRequest1);
            }, webRequest);
            #endregion
        }

        public void PostMultipart(string url, string fileKeyName,
                              Stream fileStream, System.Collections.Generic.Dictionary<string, string> stringDict, Action<string> requestCallBack)
        {
            url = BaseAddress + url;

            StringBuilder sb = new StringBuilder();
            var memStream = new MemoryStream();
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers["Accept-Encoding"] = "utf-8";
            // 边界符  
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            // 边界符  
            var beginBoundary = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            //var fileStream = Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.OpenFile(filePath);
            //var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            // 最后的结束符  
            var endBoundary = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            // 设置属性  
            webRequest.Method = "POST";
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            memStream.Write(beginBoundary, 0, beginBoundary.Length);
            sb.Append("--" + boundary + "\r\n");

            // 写入文件  
            const string filePartHeader =
                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                 "Content-Type:application/octet-stream\r\n\r\n";
            var header = string.Format(filePartHeader, fileKeyName, fileKeyName);
            var headerbytes = Encoding.UTF8.GetBytes(header);

            sb.Append("--" + boundary + "\r\n");
            sb.Append(header);
            memStream.Write(beginBoundary, 0, beginBoundary.Length);
            memStream.Write(headerbytes, 0, headerbytes.Length);

            var buffer = new byte[1024];
            int bytesRead; // =0  
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                memStream.Write(buffer, 0, bytesRead);
            }
            //--------------------------
            fileStream.Close();
            sb.Append("-0afsafdfContent");

            // 写入字符串的Key  
            //var stringKeyHeader = "\r\n--" + boundary +
            //                       "\r\nContent-Disposition: form-data; name=\"{0}\"" +
            //                       "\r\n\r\n{1}\r\n";

            //foreach (byte[] formitembytes in from string key in stringDict.Keys
            //                                 select string.Format(stringKeyHeader, key, stringDict[key])
            //                                     into formitem
            //                                     select Encoding.UTF8.GetBytes(formitem))
            //{
            //    memStream.Write(formitembytes, 0, formitembytes.Length);
            //}
            foreach (var item in stringDict)
            {
                var stringKeyHeader = "\r\n--" + boundary +
                                       "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                       "\r\n\r\n{1}";
                string s = string.Format(stringKeyHeader, item.Key, item.Value);
                byte[] formitembytes = Encoding.UTF8.GetBytes(s);
                memStream.Write(formitembytes, 0, formitembytes.Length);
                sb.Append(s);
            }


            // 写入最后的结束边界符  
            sb.Append("\r\n--" + boundary + "--\r\n");
            memStream.Write(endBoundary, 0, endBoundary.Length);
            webRequest.ContentLength = memStream.Length;
            var dataByteArray = Yuweiz.Phone.IO.StreamConvert.StreamToBytes(memStream);

            //int i = 0;
            //foreach (byte b in dataByteArray)
            //{
            //    i++;
            //    if (b != 0)
            //    {

            //        requestCallBack(i.ToString());
            //        break;
            //    }
            //}

            #region
            webRequest.BeginGetRequestStream(hr =>
            {
                HttpWebRequest httpRequest1 = (HttpWebRequest)hr.AsyncState;
                System.IO.Stream postStream = httpRequest1.EndGetRequestStream(hr);
                postStream.Write(dataByteArray, 0, dataByteArray.Length);
                postStream.Close();  //必须要关闭数据流
                httpRequest1.BeginGetResponse(hr1 =>
                {
                    WebResponse httpResponse1 = null;
                    try
                    {
                        httpResponse1 = httpRequest1.EndGetResponse(hr1);
                        using (var reader = new System.IO.StreamReader(httpResponse1.GetResponseStream(), System.Text.UTF8Encoding.UTF8))
                        {
                            string xmStream = reader.ReadToEnd();
                            reader.Close();

                            Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                // MessageBox.Show("下载成功");
                                requestCallBack(xmStream);
                            }));

                        }
                    }
                    catch (Exception ex)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {                            //MessageBox.Show(ex.Message);
                            requestCallBack(ex.Message);
                        }));
                    }
                    finally
                    {
                        if (webRequest != null) webRequest.Abort();
                        if (httpRequest1 != null) httpRequest1.Abort();
                        if (httpResponse1 != null) httpResponse1.Close();
                    }
                }, httpRequest1);
            }, webRequest);
            #endregion
        }

        public void PostMultipart(string url, string fileKeyName,
                                 string filePath, System.Collections.Generic.Dictionary<string, string> stringDict, Action<string> requestCallBack)
        {
            url = BaseAddress + url;

            var fileStream = Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.OpenFile(filePath);
            this.PostMultipart(url, fileKeyName, fileStream, stringDict, requestCallBack);
        }

        #endregion

        #region 异步方法

        /// <summary>
        /// 向指定的地址，请求数据
        /// 完成后，执行回调函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestCallBack"></param>
        /// <returns>异常返回空</returns>
        /// <author>庾伟荣</author>
        public async Task<string> GetString(string url)
        {
            url = BaseAddress + url;

            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Accept = "application/json;odata=verbose";
            // Use the Task Parallel Library pattern
            var factory = new TaskFactory();
            var task = factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

            WebResponse response = null;
            try
            {
                response = await task;
            }
            catch (Exception ex)
            {
                return null;
            }

            System.IO.Stream responseStream = response.GetResponseStream();
            string data;
            using (var reader = new System.IO.StreamReader(responseStream))
            {
                data = reader.ReadToEnd();
            }
            responseStream.Close();

            return data;
        }

        /// <summary>
        /// 向指定的地址，请求数据
        /// 完成后，执行回调函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestCallBack"></param>
        /// <author>庾伟荣</author>
        public async Task<Stream> GetStream(string url)
        {
            url = BaseAddress + url;

            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Accept = "application/json;odata=verbose";
            // Use the Task Parallel Library pattern
            var factory = new TaskFactory();
            var task = factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

            WebResponse response = null;
            try
            {
                response = await task;
            }
            catch (Exception ex)
            {
                return null;
            }
            System.IO.Stream responseStream = response.GetResponseStream();
            return responseStream;
        }

        /// <summary>
        /// 向指定的地址，请求数据
        /// 完成后，执行回调函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestCallBack"></param>
        /// <author>庾伟荣</author>
        public async Task<string> PostForString(string url, string body)
        {
            url = BaseAddress + url;

            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Accept = "application/json;odata=verbose";
            request.Method = "POST"; //httpRequest.Method = "GET"时禁用缓存策略失效
            request.Headers["Pragma"] = "no-cache";
            request.Headers["Cache-Control"] = "no-cache";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";  //GET方式此项需注释掉，否则引发异常       

            #region Post的 Stream 内容

            byte[] btBodys = Encoding.UTF8.GetBytes(body);
            request.ContentLength = btBodys.Length;
            var postStream = await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, null);
            postStream.Write(btBodys, 0, btBodys.Length);
            postStream.Close();  //必须要关闭数据流

            #endregion

            // Use the Task Parallel Library pattern
            var factory = new TaskFactory();
            var task = factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

            WebResponse response = null;
            try
            {
                response = await task;
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif

                return null;
            }
            System.IO.Stream responseStream = response.GetResponseStream();
            string data;
            using (var reader = new System.IO.StreamReader(responseStream))
            {
                data = reader.ReadToEnd();
            }
            responseStream.Close();

            return data;
        }

        /// <summary>
        /// 向指定的地址，请求数据
        /// 完成后，执行回调函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestCallBack"></param>
        /// <author>庾伟荣</author>
        public async Task<string> PostForString2(string url, string body)
        {
            url = BaseAddress + url;

            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Accept = "application/json;odata=verbose";
            request.Method = "POST"; //httpRequest.Method = "GET"时禁用缓存策略失效
            request.Headers["Pragma"] = "no-cache";
            request.Headers["Cache-Control"] = "no-cache";
            request.ContentType = "application/json;charset=UTF-8";  //GET方式此项需注释掉，否则引发异常       

            #region Post的 Stream 内容

            byte[] btBodys = Encoding.UTF8.GetBytes(body);
            request.ContentLength = btBodys.Length;
            var postStream = await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, null);
            postStream.Write(btBodys, 0, btBodys.Length);
            postStream.Close();  //必须要关闭数据流

            #endregion

            // Use the Task Parallel Library pattern
            var factory = new TaskFactory();
            var task = factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

            WebResponse response = null;
            try
            {
                response = await task;
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif

                return null;
            }
            System.IO.Stream responseStream = response.GetResponseStream();
            string data;
            using (var reader = new System.IO.StreamReader(responseStream))
            {
                data = reader.ReadToEnd();
            }
            responseStream.Close();

            return data;
        }

        /// <summary>
        /// 向指定的地址，请求数据
        /// 完成后，执行回调函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestCallBack"></param>
        /// <author>庾伟荣</author>
        public async Task<Stream> ForStream(string url)
        {
            url = BaseAddress + url;

            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Accept = "application/json;odata=verbose";
            request.Method = "POST"; //httpRequest.Method = "GET"时禁用缓存策略失效
            request.Headers["Pragma"] = "no-cache";
            request.Headers["Cache-Control"] = "no-cache";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";  //GET方式此项需注释掉，否则引发异常       

            // Use the Task Parallel Library pattern
            var factory = new TaskFactory();
            var task = factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

            WebResponse response = null;
            try
            {
                response = await task;
            }
            catch (Exception ex)
            {

                return null;
            }
            System.IO.Stream responseStream = response.GetResponseStream();
            return responseStream;
        }

        #endregion

        #region

        /// <summary>
        /// 此函数无效，仅供学习
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileKeyName"></param>
        /// <param name="fileStream"></param>
        /// <param name="stringDict"></param>
        /// <param name="requestCallBack"></param>
        [Obsolete("无效")]
        private void HttpPostData2(string url, string fileKeyName,
                                    Stream fileStream, System.Collections.Generic.Dictionary<string, string> stringDict, Action<string> requestCallBack)
        {
            var memStream = new MemoryStream();
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            // 边界符  
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            // 边界符  
            var beginBoundary = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            // 最后的结束符  
            var endBoundary = Encoding.UTF8.GetBytes("--" + boundary + "--\r\n");

            // 设置属性  
            webRequest.Method = "POST";
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

            // 写入文件  
            const string filePartHeader =
                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                 "Content-Type:image/jpeg\r\n\r\n";
            var header = string.Format(filePartHeader, fileKeyName, "c:\\" + fileKeyName + ".jpg");
            var headerbytes = Encoding.UTF8.GetBytes(header);

            memStream.Write(beginBoundary, 0, beginBoundary.Length);
            memStream.Write(headerbytes, 0, headerbytes.Length);

            var buffer = new byte[1024];
            int bytesRead; // =0  

            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                memStream.Write(buffer, 0, bytesRead);
            }

            // 写入字符串的Key  
            var stringKeyHeader = "\r\n--" + boundary +
                                   "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                   "\r\n\r\n{1}\r\n";

            foreach (byte[] formitembytes in from string key in stringDict.Keys
                                             select string.Format(stringKeyHeader, key, stringDict[key])
                                                 into formitem
                                             select Encoding.UTF8.GetBytes(formitem))
            {
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }

            // 写入最后的结束边界符  
            memStream.Write(endBoundary, 0, endBoundary.Length);
            webRequest.ContentLength = memStream.Length;
            var dataByteArray = Yuweiz.Phone.IO.StreamConvert.StreamToBytes(memStream);

            #region
            webRequest.BeginGetRequestStream(hr =>
            {
                HttpWebRequest httpRequest1 = (HttpWebRequest)hr.AsyncState;
                System.IO.Stream postStream = httpRequest1.EndGetRequestStream(hr);
                postStream.Write(dataByteArray, 0, dataByteArray.Length);
                postStream.Close();  //必须要关闭数据流
                httpRequest1.BeginGetResponse(hr1 =>
                {
                    WebResponse httpResponse1 = null;
                    try
                    {
                        httpResponse1 = httpRequest1.EndGetResponse(hr1);
                        using (var reader = new System.IO.StreamReader(httpResponse1.GetResponseStream(), System.Text.UTF8Encoding.UTF8))
                        {
                            string xmStream = reader.ReadToEnd();
                            reader.Close();

                            Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                // MessageBox.Show("下载成功");
                                requestCallBack(xmStream);
                            }));

                        }
                    }
                    catch (Exception ex)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {                            //MessageBox.Show(ex.Message);
                            requestCallBack(ex.Message);
                        }));
                    }
                    finally
                    {
                        if (webRequest != null) webRequest.Abort();
                        if (httpRequest1 != null) httpRequest1.Abort();
                        if (httpResponse1 != null) httpResponse1.Close();
                    }
                }, httpRequest1);
            }, webRequest);
            #endregion
        }
        #endregion
    }
}
