using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Yuweiz.Phone.Controls
{
    ///　<summary>
    ///　XControl
    ///　解决 OnApplyTemplate 延迟初始化的问题（显示时才初始化，GetTemplateChild才可用。但是控制在显示前已需要调用始绑定事件。
    ///  故重写OnApplyTemplate方法，OnApplyXTemplate
    ///　</summary>
    public class XControl<T> : Control
    {
        public XControl()
        {
            XControl<T>.GenericFileUri = new Uri("/Yuweiz.Phone;component/themes/generic.xaml", UriKind.Relative);

            this.DefaultStyleKey = typeof(T);
            InitializeComponent();
        }

        public XControl(Uri genericFileUri)
        {
            XControl<T>.GenericFileUri = genericFileUri;

            this.DefaultStyleKey = typeof(T);
            InitializeComponent();
        }

        /// <summary>
        /// 模版文件Uri
        /// </summary>
        protected static Uri GenericFileUri { get; set; }

        ///　<summary>
        ///　True表示已经应用过模板
        ///　</summary>
        private bool _TemplateApplied;

        ///　<summary>
        ///　初始化组件
        ///　</summary>
        private void InitializeComponent()
        {
            var objStyle = GetStyle(this.DefaultStyleKey as Type);
            if (objStyle != null)
            {
                this.Style = objStyle;
                this.ApplyTemplate();
            }
        }

        ///　<summary>
        ///　请重写此方法代替OnApplyTemplate
        ///　</summary>
        protected virtual void OnApplyXTemplate()
        {

        }

        ///　<summary>
        ///　请重写OnApplyXTemplate方法代替
        ///　</summary>
        [Obsolete("使用OnApplyXTemplate代替")]
        public override sealed void OnApplyTemplate()
        {
            if (this._TemplateApplied)
                return;
            this._TemplateApplied = true;
            this.OnApplyXTemplate();
        }

        ///　<summary>
        ///　样式字典集合
        ///　</summary>
        private static System.Collections.Generic.Dictionary<Type, ResourceDictionary> _ResourceDictionarys =
            new System.Collections.Generic.Dictionary<Type, ResourceDictionary>();

        ///　<summary>
        ///　获取样式
        ///　</summary>
        ///　<param　name="styleKey"></param>
        ///　<returns></returns>
        ///　<modify>直接取元素First()</modify>
        static public Style GetStyle(Type styleKey)
        {
            var rd = GetResourceDictionary(styleKey);
            if (rd != null)
            {
                string strKey = styleKey.ToString();
                //if (rd.Contains(strKey))
                //    return rd[strKey] as Style;
                if (rd.Count > 0)
                    return rd.First().Value as Style;
            }
            return null;
        }

        ///　<summary>
        ///　获取样式字典
        ///　</summary>
        ///　<param　name="styleKey"></param>
        ///　<returns></returns>
        static public ResourceDictionary GetResourceDictionary(Type styleKey)
        {
            if (_ResourceDictionarys.ContainsKey(styleKey))
                return _ResourceDictionarys[styleKey];

            string fullName = styleKey.Assembly.FullName;
            string baseName = fullName.Substring(0, fullName.IndexOf(",")) + ".g";

            System.Resources.ResourceManager manager = new System.Resources.ResourceManager(baseName, styleKey.Assembly);
            System.IO.UnmanagedMemoryStream stream = null;
            try
            {
                ResourceDictionary resourceDictionary = new ResourceDictionary();
                Application.LoadComponent(resourceDictionary, GenericFileUri);

                _ResourceDictionarys.Add(styleKey, resourceDictionary);
                return resourceDictionary;
            }
            catch { }
            finally
            {
                // stream.Dispose();
            }
            return null;
        }

    }
}
