using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Yuweiz.Phone.IO
{
    public class ShellTitleDataManager
    {
        /// <summary>
        /// 保存Title磁贴图片
        /// </summary>
        /// <param name="bmp">图片源</param>
        /// <param name="shellTitleId">TitleID</param>
        /// <param name="tag">图片标签</param>
        /// <param name="widthHeight">新的图片宽与高的值，如果为null则使用图片源的值</param>
        /// <returns>返回生的图片路径</returns>
        public static string SaveShellTitleImage(BitmapSource bmp, string shellTitleId, string tag, Size? widthHeight)
        {
            if (bmp == null || string.IsNullOrEmpty(shellTitleId) || string.IsNullOrEmpty(tag) || bmp.PixelWidth < 1 || bmp.PixelHeight < 1)
            {
                return null;
            }

            WriteableBitmap wbmp = bmp as WriteableBitmap;
            if (wbmp == null)
            {
                wbmp = new WriteableBitmap(bmp);
            }

            string savedPath = "Shared/ShellContent/" + shellTitleId + "/" + tag;
            if (widthHeight != null)
            {
                wbmp = wbmp.Resize((int)widthHeight.Value.Width, (int)widthHeight.Value.Height, WriteableBitmapExtensions.Interpolation.NearestNeighbor);
            }
            IsolatedStorageDAL.Instance.SavePngPicture(wbmp, savedPath);

            return savedPath;
        }

        public static void DeleteShellTitleImage(string shellTitleId, string tag)
        {
            string path = "Shared/ShellContent/" + shellTitleId + "/" + tag;
            IsolatedStorageDAL.Instance.DeleteFile(path);
        }

        public static void DeleteShellTitleImage(string shellTitleId)
        {
            string[] deleFilesTag = 
            {
                "BackgroundImage",
                "BackBackgroundImage",
                "WideBackgroundImage",
                "WideBackBackgroundImage",
                "CycleImages1",
                "CycleImages2",
                "CycleImages3",
                "CycleImages4",
                "CycleImages5",
                "CycleImages6",
                "CycleImages7",
                "CycleImages8",
                "CycleImages9",
            };

            foreach (string tag in deleFilesTag)
            {
                string path = "Shared/ShellContent/" + shellTitleId + "/" + tag;
                Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.DeleteFile(path);
            }
        }


    }
}
