using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Yuweiz.Phone.Net
{
    /// <summary>
    /// Http请求类
    /// </summary>
    public class HttpRequest
    {
        /// <summary>
        /// 读取指定网址内容
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBack"></param>
        public void GetUrl(string url, Action<string> requestBack)
        {
            WebClient webClient = new WebClient();
            webClient.Encoding = System.Text.UTF8Encoding.UTF8;
            webClient.OpenReadAsync(new Uri(url, UriKind.RelativeOrAbsolute));              //在不阻止调用线程的情况下，从资源返回数据
            webClient.OpenReadCompleted += (s, o) =>
            {
                if (o.Error == null)
                {
                    //指定以UTF-8方式读取流
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(o.Result, System.Text.UTF8Encoding.UTF8))
                    {
                        string strStream = reader.ReadToEnd();   //这里是返回的数据
                        requestBack(strStream);
                    }
                }
                else
                {
                    //MessageBox.Show(o.Error.Message);
                }
            };

        }

        /// <summary>
        /// 读取指定网址内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public void StartRequestGet(string url, Action<string, bool> requestBack)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "GET"; // httpRequest.Method = "GET"时禁用缓存策略失效
            httpRequest.Headers["Pragma"] = "no-cache";
            httpRequest.Headers["Cache-Control"] = "no-cache";
            //httpRequest.ContentType = "Content-Type:application/x-www-form-urlencoded; charset=UTF-8"; //GET方式此项需注释掉，否则引发异常
            httpRequest.BeginGetResponse(r =>
            {
                HttpWebResponse httpResponse = null;
                try
                {
                    httpResponse = (HttpWebResponse)httpRequest.EndGetResponse(r);
                    using (var reader = new System.IO.StreamReader(httpResponse.GetResponseStream(), System.Text.UTF8Encoding.UTF8))
                    {
                        string xmStream = reader.ReadToEnd();
                        reader.Close();
                        Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            // MessageBox.Show("下载成功");
                            requestBack(xmStream, true);
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //MessageBox.Show(ex.Message);
                        requestBack(ex.Message, false);
                    }));
                }
                finally
                {
                    if (httpRequest != null) httpRequest.Abort();
                    if (httpResponse != null) httpResponse.Close();
                }
            }, httpRequest);
        }

        public void StartRequest(string url, Action<string, bool> requestBack)
        {
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
                                 requestBack(xmStream, true);
                             }));

                         }
                     }
                     catch (Exception ex)
                     {
                         Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                         {
                             //MessageBox.Show(ex.Message);
                             requestBack(ex.Message, false);
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

        public void StartRequestGbk(string url, Action<string, bool> requestBack)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST"; //httpRequest.Method = "GET"时禁用缓存策略失效
            httpRequest.Headers["Pragma"] = "no-cache";
            httpRequest.Headers["Cache-Control"] = "no-cache";
            httpRequest.ContentType = "application/x-www-form-urlencoded;charset=gbk";  //GET方式此项需注释掉，否则引发异常         
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
                        using (var reader = new System.IO.StreamReader(httpResponse1.GetResponseStream(), System.Text.Encoding.GetEncoding("bgk")))
                        {
                            string xmStream = reader.ReadToEnd();
                            reader.Close();

                            Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                // MessageBox.Show("下载成功");
                                requestBack(xmStream, true);
                            }));

                        }
                    }
                    catch (Exception ex)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            //MessageBox.Show(ex.Message);
                            requestBack(ex.Message, false);
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
    }
}
