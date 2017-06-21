using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Yuweiz.Phone.IO
{
    /// <summary>
    /// 独立存储器文件管理
    /// </summary>
    public class IsolatedStorageDAL
    {
        private IsolatedStorageDAL()
        { }

        private static IsolatedStorageDAL instance;

        public static IsolatedStorageDAL Instance
        {
            get
            {
                if (instance == null)
                {
                    IsolatedStorageDAL.instance = new IsolatedStorageDAL();
                }

                return instance;
            }
        }

        /// <summary>
        /// 打开指定路径的文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns>返回流</returns>
        public Stream OpenFile(string path = "Temp.file")
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(path))
                {
                    IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(path, FileMode.Open, FileAccess.Read);
                    return fileStream;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 打开指定路径的文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns>返回流</returns>
        public ImageSource OpenPicFile(string path = "Temp.file")
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(path))
                {
                    IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(path, FileMode.Open, FileAccess.Read);
                    WriteableBitmap wbmp = new WriteableBitmap(1,1);
                    wbmp.SetSource(fileStream);
                    return wbmp;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 保存图片到指定路径
        /// </summary>
        /// <param name="wbmp"></param>
        /// <param name="path"></param>
        /// <returns>返回保存的路径</returns>
        public string SavePicture(WriteableBitmap wbmp, string path = "Temp.file")
        {
            if (wbmp == null)
            {
                return string.Empty; ;
            }

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                #region 检查文件夹
                string[] directories = path.Split('/');
                if (directories != null && directories.Length > 1)
                {
                    string curDirectory = "";
                    for (int i = 0; i < directories.Length - 1; i++)
                    {
                        curDirectory += "/" + directories[i];
                        if (!myIsolatedStorage.DirectoryExists(curDirectory))
                        {
                            myIsolatedStorage.CreateDirectory(curDirectory);
                        }
                    }
                }
                #endregion

                if (myIsolatedStorage.FileExists(path))
                {
                    myIsolatedStorage.DeleteFile(path);
                }
                IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(path);
                System.Windows.Media.Imaging.Extensions.SaveJpeg(wbmp, fileStream, wbmp.PixelWidth, wbmp.PixelHeight, 0, 100);
                fileStream.Close();
            }

            return path;
        }

        public bool RenameFile(string path, string target)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(path))
                {
                    try
                    {
                        myIsolatedStorage.MoveFile(path, target);
                    }
                    catch { }     
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public bool CheckFileIsExit(string path)
        {
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(path))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 保存图片到指定路径
        /// </summary>
        /// <param name="wbmp"></param>
        /// <param name="path"></param>
        /// <returns>返回保存的路径</returns>
        public string SavePngPicture(WriteableBitmap wbmp, string path = "Temp.file")
        {
            if (wbmp == null)
            {
                return string.Empty; ;
            }

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                #region 检查文件夹
                string[] directories = path.Split('/');
                if (directories != null && directories.Length > 2)
                {
                    string curDirectory = "";
                    for (int i = 0; i < directories.Length - 1; i++)
                    {
                        curDirectory += "/" + directories[i];
                        if (!myIsolatedStorage.DirectoryExists(curDirectory))
                        {
                            myIsolatedStorage.CreateDirectory(curDirectory);
                        }
                    }
                }
                #endregion

                if (myIsolatedStorage.FileExists(path))
                {
                    myIsolatedStorage.DeleteFile(path);
                }
                IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(path);
                wbmp.SavePng(fileStream);
                fileStream.Close();
            }

            return path;
        }

        /// <summary>
        /// 删除指定文夹及其中的文件
        /// </summary>
        /// <param name="directoryName"></param>
       // [Obsolete("未用过尝实践修正的方法")]
        public void DeleteDirectory(string directoryName)
        {
            try
            {
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    string[] directories = myIsolatedStorage.GetDirectoryNames("*" + directoryName);
                    if (directories != null && directories.Length > 0)
                    {
                        string[] filesNames = myIsolatedStorage.GetFileNames(directoryName + "/*");
                        if (filesNames != null && filesNames.Length > 0)
                        {
                            foreach (string name in filesNames)
                            {
                                string path = directoryName + "/" + name;
                                if (myIsolatedStorage.FileExists(path))
                                {
                                    myIsolatedStorage.DeleteFile(path);
                                }
                            }

                        }
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 删除指定路径文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool DeleteFile(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    return true;
                }

                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (myIsolatedStorage.FileExists(path))
                    {
                        myIsolatedStorage.DeleteFile(path);
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 将实体序例化Json字符串
        /// 实体中的类型应为基础类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public void SaveObjce<T>(T model, string path = "objJsonString")
        {
            string jsonString;
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(model.GetType()).WriteObject(ms, model);
                jsonString = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);

                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    #region 检查文件夹
                    string[] directories = path.Split('/');
                    if (directories != null && directories.Length > 1)
                    {
                        string curDirectory = "";
                        for (int i = 0; i < directories.Length - 1; i++)
                        {
                            curDirectory += "/" + directories[i];
                            if (!myIsolatedStorage.DirectoryExists(curDirectory))
                            {
                                myIsolatedStorage.CreateDirectory(curDirectory);
                            }
                        }
                    }
                    #endregion

                    if (myIsolatedStorage.FileExists(path))
                    {
                        myIsolatedStorage.DeleteFile(path);
                    }
                    using (IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(path))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(fileStream))
                        {
                            streamWriter.WriteLine(jsonString);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将Json字符串反序例化实体
        /// 实体中的类型应为基础类型
        /// </summary>
        /// <param name="path"></param>
        /// <returns>返实体</returns>
        public T OpenObject<T>(string path = "objJsonString") where T : new()
        {
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(path))
                {
                    using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(path, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader stramReader = new StreamReader(fileStream))
                        {
                            string jsonString = stramReader.ReadToEnd();
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
                            {
                                T tobj = (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
                                return tobj;
                            }
                        }
                    }
                }

                return default(T);
            }

        }

        public void SaveTextFile(string fileData, string path = "file.str")
        {
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                #region 检查文件夹
                string[] directories = path.Split('/');
                if (directories != null && directories.Length > 1)
                {
                    string curDirectory = "";
                    for (int i = 0; i < directories.Length - 1; i++)
                    {
                        curDirectory += "/" + directories[i];
                        if (!myIsolatedStorage.DirectoryExists(curDirectory))
                        {
                            myIsolatedStorage.CreateDirectory(curDirectory);
                        }
                    }
                }
                #endregion

                if (myIsolatedStorage.FileExists(path))
                {
                    myIsolatedStorage.DeleteFile(path);
                }
                using (IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(path))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.WriteLine(fileData);
                    }
                }
            }
        }

        public void SaveFile(Stream stream, string path = "file.str")
        {
            if (stream == null)
            {
                return;
            }
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                #region 检查文件夹
                string[] directories = path.Split('/');
                if (directories != null && directories.Length > 1)
                {
                    string curDirectory = "";
                    for (int i = 0; i < directories.Length - 1; i++)
                    {
                        curDirectory += "/" + directories[i];
                        if (!myIsolatedStorage.DirectoryExists(curDirectory))
                        {
                            myIsolatedStorage.CreateDirectory(curDirectory);
                        }
                    }
                }
                #endregion

                if (myIsolatedStorage.FileExists(path))
                {
                    myIsolatedStorage.DeleteFile(path);
                }

                byte[] bys = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(bys, 0, bys.Length);
                using (IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(path))
                {
                    fileStream.Write(bys, 0, bys.Length);
                }
            }
        }

        public string OpenTextFile(string path = "file.str")
        {
            string fileData = string.Empty;
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(path))
                {
                    using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(path, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader stramReader = new StreamReader(fileStream))
                        {
                            fileData = stramReader.ReadToEnd();
                        }
                    }
                }
            }

            return fileData;
        }

        public async Task<string> ReadFileFromInstalledLocation(string path)
        {
            try
            {
                string text;

                IStorageFolder applicationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var dfd = applicationFolder.GetFolderAsync("Resources");
                IStorageFile storageFile = await applicationFolder.GetFileAsync(path);

                IRandomAccessStream accessStream = await storageFile.OpenReadAsync();

                using (Stream stream = accessStream.AsStreamForRead((int)accessStream.Size))
                {

                    byte[] content = new byte[stream.Length];

                    await stream.ReadAsync(content, 0, (int)stream.Length);

                    text = Encoding.UTF8.GetString(content, 0, content.Length);

                }

                return text;
            }
            catch
            {
                return null;
            }

        }

        #region Windows 8专用

        public async Task WriteFile(string text, string path = "file.str")
        {

            IStorageFolder applicationFolder = ApplicationData.Current.LocalFolder;

            #region 检查文件夹
            string[] directories = path.Split('/');
            if (directories != null && directories.Length > 1)
            {
                string curDirectory = "";
                for (int i = 0; i < directories.Length - 1; i++)
                {
                    curDirectory = directories[i];
                    IStorageFolder curFolder = await applicationFolder.GetFolderAsync(curDirectory);
                    if (curFolder != null)
                    {
                        applicationFolder = await curFolder.CreateFolderAsync(curDirectory);
                    }
                }
            }
            #endregion


            IStorageFile storageFile = await applicationFolder.CreateFileAsync(path, CreationCollisionOption.ReplaceExisting);



            using (Stream stream = await storageFile.OpenStreamForWriteAsync())
            {

                byte[] content = Encoding.UTF8.GetBytes(text);

                await stream.WriteAsync(content, 0, content.Length);

            }

        }

        public async Task<string> ReadFile(string path = "file.str")
        {

            try
            {
                string text;

                IStorageFolder applicationFolder = ApplicationData.Current.LocalFolder;

                IStorageFile storageFile = await applicationFolder.GetFileAsync(path);

                IRandomAccessStream accessStream = await storageFile.OpenReadAsync();

                using (Stream stream = accessStream.AsStreamForRead((int)accessStream.Size))
                {

                    byte[] content = new byte[stream.Length];

                    await stream.ReadAsync(content, 0, (int)stream.Length);

                    text = Encoding.UTF8.GetString(content, 0, content.Length);

                }

                return text;
            }
            catch
            {
                return null;
            }

        }

        #endregion
    }
}
