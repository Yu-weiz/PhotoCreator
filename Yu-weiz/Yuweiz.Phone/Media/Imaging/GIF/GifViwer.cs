using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Yuweiz.Phone.Media.Imaging.GifLib2;

namespace Yuweiz.Phone.Media.Imaging.GIF
{
    public class GifViwer
    {

        public GifViwer(Image img)
        {
            IsUninform = true;
            image = img;
        }

        private Image image { get; set; }

        public bool IsUninform { get; set; }


        public void LoadGif(string name)
        {
            GifDecoder decoder = new GifDecoder();
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!store.FileExists(name))
                {
                    MessageBox.Show("gif image 不存在");
                    return;
                }
                using (var stream = store.OpenFile(name, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    decoder.Read(stream);

                    //get frame size to set image size
                    Size size = decoder.GetFrameSize();
                    image.Width = size.Width;
                    image.Height = size.Height;

                    int delay = decoder.GetDelay(1);

                    //0 stand for loop forever, otherwise is the real count
                    int loopCount = decoder.GetLoopCount();
                    //decoder.GetLoopCount
                    int imagecount = decoder.GetFrameCount();
                    for (int i = 0; i < imagecount; i++)
                    {
                        imageList.Add(decoder.GetFrame(i));
                    }
                    DisplayGif(delay, loopCount);
                }
            }
        }

        public void LoadGif(Stream stream)
        {
            GifDecoder decoder = new GifDecoder();
            decoder.Read(stream);

            if (IsUninform)
            {
                //get frame size to set image size
                Size size = decoder.GetFrameSize();
                image.Width = size.Width;
                image.Height = size.Height;
            }

            int delay = decoder.GetDelay(1);

            //0 stand for loop forever, otherwise is the real count
            int loopCount = decoder.GetLoopCount();
            //decoder.GetLoopCount
            int imagecount = decoder.GetFrameCount();
            for (int i = 0; i < imagecount; i++)
            {
                imageList.Add(decoder.GetFrame(i));
            }

            DisplayGif(delay, loopCount);
        }

        public WriteableBitmap GetFrame(int index)
        {
            if (index < imageList.Count && index > -1)
            {
                return imageList[index];
            }

            return null;
        }

        private List<WriteableBitmap> imageList = new List<WriteableBitmap>();

        private void DisplayGif(int delay, int loopCount)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(delay);
            int index = 0;
            int loopedCount = 0;//已经循环的次数
            timer.Tick += (sender, e) =>
            {
                //如果是永远循环
                if (loopCount == 0)
                {
                    if (index == imageList.Count - 1)
                    {
                        index = 0;
                    }
                }
                else
                {
                    if (loopCount == loopedCount)
                    {
                        timer.Stop();
                        return;
                    }                    
                }
                if (imageList.Count <= index)
                {
                    loopedCount++;
                    index = 0;
                }

                image.Source = imageList[index];
                index++;
            };
            timer.Start();
        }
    }
}
