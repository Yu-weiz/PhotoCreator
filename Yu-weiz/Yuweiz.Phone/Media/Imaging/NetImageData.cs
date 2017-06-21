using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Yuweiz.Phone.Media.Imaging
{

    // Data类从INotifyPropertyChanged派生，要实现绑定属性改变的事件，用于图片异步请求完成之后可以更新到UI上
    public class NetImageData : INotifyPropertyChanged
    {
        // 图片名字属性
        public string Name { get; set; }
        // 当前的页面对象，用于触发UI线程
        public Page Page { get; set; }
        // 图片的网络地址
        private Uri imageUri;
        public Uri ImageUri
        {
            get
            {
                return imageUri;
            }
            set
            {
                if (imageUri == value)
                {
                    return;
                }
                imageUri = value;
                bitmapImage = null;
            }
        }

        #region 异步下载图片
        // 若引用对象，用于存储下载好的图片对象
        WeakReference bitmapImage;
        // ImageSource属性用于绑定到列表的Image控件上
        public ImageSource ImageSource
        {
            get
            {
                if (bitmapImage != null)
                {
                    // 如果弱引用没有没回收，则取弱引用的值
                    if (bitmapImage.IsAlive)
                        return (ImageSource)bitmapImage.Target;
                    else
                        Debug.WriteLine("数据已经被回收");
                }
                // 弱引用已经被回收那么则通过图片网络地址进行异步下载
                if (imageUri != null)
                {
                    Task.Factory.StartNew(() => { DownloadImage(imageUri); });
                }
                return null;
            }
        }
        // 下载图片的方法
        void DownloadImage(object state)
        {
            HttpWebRequest request = WebRequest.CreateHttp(state as Uri);
            request.BeginGetResponse(DownloadImageComplete, request);
        }
        // 完成图片下载的回调方法
        void DownloadImageComplete(IAsyncResult result)
        {
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
            // 读取网络的数据
            Stream stream = response.GetResponseStream();
            int length = int.Parse(response.Headers["Content-Length"]);
            // 注意需要把数据流重新复制一份，否则会出现跨线程错误
            // 网络下载到的图片数据流，属于后台线程的对象，不能在UI上使用
            Stream streamForUI = new MemoryStream(length);
            byte[] buffer = new byte[length];
            int read = 0;
            do
            {
                read = stream.Read(buffer, 0, length);
                streamForUI.Write(buffer, 0, read);
            } while (read == length);
            streamForUI.Seek(0, SeekOrigin.Begin);

            // 触发UI线程处理位图和UI更新
            Page.Dispatcher.BeginInvoke(() =>
            {
                BitmapImage bm = new BitmapImage();
                bm.SetSource(streamForUI);
                // 把图片位图对象存放到若引用对象里面
                if (bitmapImage == null)
                    bitmapImage = new WeakReference(bm);
                else
                    bitmapImage.Target = bm;

                //触发UI绑定属性的改变
                OnPropertyChanged("ImageSource");
            }
            );
        }

        // 属性改变事件
        void OnPropertyChanged(string property)
        {
            var hander = PropertyChanged;
            if (hander != null)
                Page.Dispatcher.BeginInvoke(() =>
                {
                    hander(this, new PropertyChangedEventArgs(property));
                });
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
