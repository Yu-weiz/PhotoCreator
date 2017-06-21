using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.ComponentModel;

namespace Yuweiz.Phone.Media
{
    public enum VoiceState { Stop, Recording, Playing }

    public class Voice
    {
        public static DynamicSoundEffectInstance VoicePlayer = new DynamicSoundEffectInstance(Microphone.Default.SampleRate, AudioChannels.Mono);
        
        public Voice()
        {
            microphone = Microphone.Default;
            microphone.BufferReady += new EventHandler<EventArgs>(OnMicrophoneBufferReady);
            playback = new DynamicSoundEffectInstance(microphone.SampleRate, AudioChannels.Mono);      //创建一个new DynamicSoundEffectInstace

            proPlayTimer.Interval = TimeSpan.FromMilliseconds(1000);
            proPlayTimer.Tick += new EventHandler(proPlayTimer_Tick);
        }


        void proPlayTimer_Tick(object sender, EventArgs e)
        {
            _playingTimeSpan = _playingTimeSpan.Add(new TimeSpan(0, 0, 0, 1));

            if (PlayingTimeSpanceChange != null)
            {
                PlayingTimeSpanceChange(_playingTimeSpan);
            }

            if (_playingTimeSpan.CompareTo(_spaceTime) >= 0)
            {
                proPlayTimer.Stop();
            }
        }

        Microphone microphone;                                           //麦克风
        VoiceInfo memoInfo;                                            //录音信息文件
        List<byte[]> memoBufferCollection = new List<byte[]>();
        DynamicSoundEffectInstance playback;
        TimeSpan _spaceTime;                                             //记录录音长度

        public TimeSpan SpaceTime
        {
            get { return _spaceTime; }
            set { _spaceTime = value; }
        }

        TimeSpan _playingTimeSpan;
        TimeSpan spaceTime
        {
            get { return _spaceTime; }
            set
            {
                _spaceTime = value;
                if (SpaceTimeChange != null)
                {
                    SpaceTimeChange(value);
                }
            }
        }

        public event Action<TimeSpan> PlayingTimeSpanceChange;
        public event Action<TimeSpan> SpaceTimeChange;


        System.Windows.Threading.DispatcherTimer proPlayTimer = new System.Windows.Threading.DispatcherTimer();



        //录音准备，将缓存地址添加到列表byt[]
        void OnMicrophoneBufferReady(object sender, EventArgs args)
        {

            // Get buffer from microphone and add to collection
            byte[] buffer = new byte[microphone.GetSampleSizeInBytes(microphone.BufferDuration)];
            int bytesReturned = microphone.GetData(buffer);
            memoBufferCollection.Add(buffer);

            long lg = memoBufferCollection.Count * microphone.GetSampleSizeInBytes(microphone.BufferDuration);
            spaceTime = microphone.GetSampleDuration((int)Math.Min(lg, Int32.MaxValue));
        }

        VoiceState _curVoiceState;

        public VoiceState CurVoiceState
        {
            get { return _curVoiceState; }
        }

        public void StartRecording()
        {
            if (microphone == null)
            {
                microphone = Microphone.Default;
                microphone.BufferReady += new EventHandler<EventArgs>(OnMicrophoneBufferReady);
            }

            if (microphone.State == MicrophoneState.Stopped)
            {
                // 清空存储的录音缓存
                memoBufferCollection.Clear();
                // Stop any playback in progress (not really necessary, but polite I guess)
                //  playback.Stop();
                VoicePlayer.Stop();
                //  GC.Collect();

                // 开始录音
                microphone.Start();
                //  recordTimer.Start();

                _curVoiceState = VoiceState.Recording;
            }
            else
            {
                StopRecording();
            }
        }

        public void StopRecording()
        {

            // Get the last partial buffer
            int sampleSize = microphone.GetSampleSizeInBytes(microphone.BufferDuration);
            byte[] extraBuffer = new byte[sampleSize];
            int extraBytes = microphone.GetData(extraBuffer);

            // Stop recording
            microphone.Stop();
            //  recordTimer.Stop();

            // Create MemoInfo object and add at top of collection
            int totalSize = memoBufferCollection.Count * sampleSize + extraBytes;
            TimeSpan duration = microphone.GetSampleDuration(totalSize);
            memoInfo = new VoiceInfo(DateTime.UtcNow, totalSize, duration);
            // memoFiles.Insert(0, memoInfo);

            memoInfo.VoiceBuffer = memoBufferCollection;
            memoInfo.ExtraBuffer = extraBuffer;
            memoInfo.ExtraBytes = extraBytes;

            // Save data in isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists("TempVoice"))
                {
                    storage.DeleteFile("TempVoice");
                }
                using (IsolatedStorageFileStream stream = storage.CreateFile("TempVoice"))
                {
                    // Write buffers from collection
                    foreach (byte[] buffer in memoBufferCollection)
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }

                    // Write partial buffer
                    stream.Write(extraBuffer, 0, extraBytes);
                }
            }

            _curVoiceState = VoiceState.Stop;
        }

        public void PlayTheVoice()
        {
            // isPlayingVoice != isPlayingVoice;
            string tempVoiceFile = "TempVoice";

            if (playback.State == SoundState.Playing)
            {
                playback.Stop();
            }

            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (storage.FileExists(tempVoiceFile))
                    {
                        using (IsolatedStorageFileStream stream = storage.OpenFile(tempVoiceFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        {
                            byte[] buffer = new byte[stream.Length];
                            stream.Read(buffer, 0, buffer.Length);
                            playback = new DynamicSoundEffectInstance(microphone.SampleRate, AudioChannels.Mono);
                            playback.SubmitBuffer(buffer);     //在原有基础上指交要播放的音频（即添加到播放队列）

                            _playingTimeSpan = new TimeSpan(0, 0, 0, 0);
                            playback.Play();


                            _curVoiceState = VoiceState.Playing;
                            proPlayTimer.Start();
                        }
                    }
                }
            }
            catch { }


        }

        public void StopPlayVoice()
        {
            playback.Stop();
            proPlayTimer.Stop();
            _curVoiceState = VoiceState.Stop;

        }

        public string SaveVoice()
        {
            if (memoInfo == null)
            {
                return "";
            }
            // Save data in isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {


                if (!storage.DirectoryExists("Voice"))
                {
                    storage.CreateDirectory("Voice");
                }

                if (storage.FileExists("TempVoice"))
                {
                    storage.MoveFile("TempVoice", "/Voice/" + memoInfo.FileName);
                }
                else
                {
                    return "";
                }

            }
            return "/Voice/" + memoInfo.FileName;
        }

        public string SaveVoice(string voicePath)
        {

            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {


                if (!storage.DirectoryExists("Voice"))
                {
                    storage.CreateDirectory("Voice");
                }

                if (storage.FileExists("TempVoice"))
                {
                    storage.MoveFile("TempVoice", voicePath);
                }
                else
                {
                    return "";
                }

            }
            return voicePath;
        }

        public void DisposeVoice()
        {
            if (microphone != null)
            {
                microphone.BufferReady -= new EventHandler<EventArgs>(OnMicrophoneBufferReady);
                microphone = null;
            }
        }
    }

    public class VoiceInfo : INotifyPropertyChanged
    {
        bool isPlaying;

        // Event is only fired for IsPlaying property
        public event PropertyChangedEventHandler PropertyChanged;

        public VoiceInfo(string filename, long filesize, TimeSpan duration)
        {
            this.FileName = filename;

            this.SpaceTime = new SpaceTime
            {
                Space = filesize,
                Time = duration
            };
        }

        public VoiceInfo(DateTime datetime, long filesize, TimeSpan duration)
            : this(null, filesize, duration)
        {
            // Convert DateTime to packed string
            FileName = String.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}{6:D3}",
                                     datetime.Year, datetime.Month, datetime.Day,
                                     datetime.Hour, datetime.Minute, datetime.Second,
                                     datetime.Millisecond);
        }

        public bool IsPlaying
        {
            set
            {
                if (value != isPlaying)
                {
                    isPlaying = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsPlaying"));
                    }
                }
            }
            get
            {
                return isPlaying;
            }
        }

        public bool IsPaused { set; get; }

        public string FileName { protected set; get; }

        public SpaceTime SpaceTime { protected set; get; }

        public DateTime DateTime
        {
            get
            {
                // Convert packed string to DateTime
                int year = int.Parse(FileName.Substring(0, 4));
                int mon = int.Parse(FileName.Substring(4, 2));
                int day = int.Parse(FileName.Substring(6, 2));
                int hour = int.Parse(FileName.Substring(8, 2));
                int min = int.Parse(FileName.Substring(10, 2));
                int sec = int.Parse(FileName.Substring(12, 2));
                int msec = int.Parse(FileName.Substring(14, 3));

                DateTime dt = new DateTime(year, mon, day, hour, min, sec, msec, DateTimeKind.Utc);
                return dt.ToLocalTime();
            }
        }

        public List<byte[]> VoiceBuffer
        {
            get;
            set;
        }

        public byte[] ExtraBuffer
        {
            get;
            set;
        }

        public int ExtraBytes
        {
            get;
            set;
        }
    }

    public class SpaceTime : INotifyPropertyChanged
    {
        static readonly string[] sizeUnits = { "bytes", "K", "M", "G", "T" };

        long space;
        TimeSpan time;

        public event PropertyChangedEventHandler PropertyChanged;

        public long Space
        {
            set
            {
                if (space != value)
                {
                    space = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Space"));
                    OnPropertyChanged(new PropertyChangedEventArgs("FormattedSpace"));
                }
            }
            get
            {
                return space;
            }
        }

        public string FormattedSpace
        {
            get { return GetFormattedFileSize(this.Space); }
        }

        public TimeSpan Time
        {
            set
            {
                if (time != value)
                {
                    time = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Time"));
                    OnPropertyChanged(new PropertyChangedEventArgs("FormattedTime"));
                }
            }
            get
            {
                return time;
            }
        }

        public string FormattedTime
        {
            get { return GetFormattedDuration(this.Time); }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, args);
        }

        public static string GetFormattedFileSize(long filesize)
        {
            int magnitude = 0;
            double display = 0;
            string format = "G";

            if (filesize > 0)
            {
                int base2log = (int)(Math.Log(filesize) / Math.Log(2));
                magnitude = base2log / 10;
                display = filesize / Math.Pow(2, 10 * magnitude);
                int sigDigits = 1 + (int)Math.Log10(display);
                format = "N" + Math.Max(0, 3 - sigDigits);
            }
            return display.ToString(format) + ' ' + sizeUnits[magnitude];
        }

        public static string GetFormattedDuration(TimeSpan Duration)
        {
            if (Duration.Days > 0)
            {
                int hours = Duration.Hours + (Duration.Minutes + Duration.Seconds / 30) / 30;

                return String.Format("{0} day{1} {2}hr{3}",
                                     Duration.Days, Duration.Days == 1 ? "" : "s",
                                     hours, hours == 1 ? "" : "s");
            }

            else if (Duration.Hours > 0)
            {
                int minutes = Duration.Minutes + Duration.Seconds / 30;

                return String.Format("{0} hr{1} {2} min{3}",
                                     Duration.Hours, Duration.Hours == 1 ? "" : "s",
                                     minutes, minutes == 1 ? "" : "s");
            }

            else if (Duration.Minutes > 0)
            {
                int seconds = (int)Math.Round(Duration.Seconds + Duration.Milliseconds / 1000.0);

                return String.Format("{0} min{1} {2} sec{3}",
                                     Duration.Minutes, Duration.Minutes == 1 ? "" : "s",
                                     seconds, seconds == 1 ? "" : "s");
            }

            else if (Duration.Seconds > 0)
            {
                double seconds = Math.Round(Duration.Seconds + Duration.Milliseconds / 1000.0,
                                            Duration.Seconds < 10 ? 2 : 1);

                return String.Format("{0} second{1}",
                                     seconds, seconds == 1 ? "" : "s");
            }

            return String.Format("{0} msecs", Duration.Milliseconds);
        }
    }
}
