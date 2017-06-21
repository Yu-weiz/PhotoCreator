using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Yuweiz.Phone.Common;

namespace Yuweiz.Phone.Controls
{
    public enum MessageBoxExtResult { None, DO, OK, Cancel };

    public class MessageBoxExt : XControl<MessageBoxExt>
    {
        public MessageBoxExt()
        {
            // this.DefaultStyleKey = typeof(MessageBoxExt);
        }

        private static MessageBoxExt instance;

        public static MessageBoxExt Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MessageBoxExt();
                }
                return instance;
            }
        }

        public string Message
        {
            get { return this.tMessage.Text; }
            set { this.tMessage.Text = value ?? value; }
        }

        public string SaveString
        {
            get { return this.btnSave.Content.ToString(); }
            set { this.btnSave.Content = value; }
        }

        public string LeaveString
        {
            get { return this.btnLeave.Content.ToString(); }
            set { this.btnLeave.Content = value; }
        }

        public string CancelString
        {
            get { return this.btnCancel.Content.ToString(); }
            set { this.btnCancel.Content = value; }
        }

        #region 控件集
        private Popup popupMain;
        private Grid grLeaveMessageBox;
        private Button btnSave;
        private Button btnLeave;
        private Button btnCancel;

        private TextBlock tMessage;
        #endregion

        private Panel CurPanel
        {
            get
            {
                Panel curPanel = null;
                Frame frame = Application.Current.RootVisual as Frame;
                var presenters = frame.GetLogicalChildrenByType<ContentPresenter>(false);
                for (var i = 0; i < presenters.Count(); i++)
                {
                    var panels = presenters.ElementAt(i).GetLogicalChildrenByType<Panel>(false);

                    if (!panels.Any())
                        continue;

                    curPanel = panels.First();
                    break;
                }
                return curPanel;
            }
        }

        private PhoneApplicationPage CurPage
        {
            get
            {
                Frame frame = Application.Current.RootVisual as Frame;
                return frame.GetFirstLogicalChildByType<PhoneApplicationPage>(false);
            }
        }

        protected override void OnApplyXTemplate()
        {
            base.OnApplyXTemplate();

            this.popupMain = this.GetTemplateChild("popupMain") as Popup;
            this.grLeaveMessageBox = this.GetTemplateChild("grLeaveMessageBox") as Grid;
            this.btnSave = this.GetTemplateChild("btnSave") as Button;
            this.btnLeave = this.GetTemplateChild("btnLeave") as Button;
            this.btnCancel = this.GetTemplateChild("btnCancel") as Button;
            this.tMessage = this.GetTemplateChild("tMessage") as TextBlock;

            //初始化
            this.Initialize();
        }

        public MessageBoxExtResult MessageBoxExtResult { get; set; }

        public async Task<MessageBoxExtResult> Show()
        {
            this.MessageBoxExtResult = MessageBoxExtResult.None;

            if (this.CurPage != null && this.CurPage.ApplicationBar != null)
            {
                this.CurPage.ApplicationBar.IsVisible = false;
            }

            this.CurPanel.Children.Add(this);
            this.popupMain.IsOpen = true;

            Task<MessageBoxExtResult> task = new Task<MessageBoxExtResult>(CheckClick);
            task.Start();
            this.MessageBoxExtResult = await task;

            this.popupMain.IsOpen = false;
            if (this.CurPage != null && this.CurPage.ApplicationBar != null)
            {
                this.CurPage.ApplicationBar.IsVisible = true;
            }

            this.CurPanel.Children.Remove(this);

            return this.MessageBoxExtResult;
        }


        private void Initialize()
        {
            this.btnSave.Click += btnSave_Click;
            this.btnLeave.Click += btnLeave_Click;
            this.btnCancel.Click += btnCancel_Click;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxExtResult = MessageBoxExtResult.DO;
        }

        private void btnLeave_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxExtResult = MessageBoxExtResult.OK;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxExtResult = MessageBoxExtResult.Cancel;
        }

        private MessageBoxExtResult CheckClick()
        {
            while (this.MessageBoxExtResult == MessageBoxExtResult.None)
            {
                Thread.Sleep(100);
            }

            return this.MessageBoxExtResult;

        }
    }



}
