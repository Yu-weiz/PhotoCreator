using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Yuweiz.Phone.Controls
{
    public class EllipseButton : Button
    {
        private const int DEFAULT_SROKE_THICKNES = 1;

        public EllipseButton()
        {
            this.InitializeStyle();
            this.RenderTransformOrigin = new Point(0.5, 0.5);
            IsPressedStoryboard = true;
            // this.ApplyTemplate();
        }

        public bool IsPressedStoryboard
        {
            get;
            set;
        }

        private void InitializeStyle()
        {
            string styleString = @"  
                       <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'  xmlns:d='http://schemas.microsoft.com/expression/blend/2008'
    xmlns:mc='http://schemas.openxmlformats.org/markup-compatibility/2006' TargetType='Button'>
						<Grid x:Name='ButtonBackground' RenderTransformOrigin='0.5,0.5'>
							<Grid.RenderTransform>
								<CompositeTransform/>
							</Grid.RenderTransform>
							<VisualStateManager.VisualStateGroups>
								<VisualStateGroup x:Name='CommonStates'>
									<VisualState x:Name='Normal'/>
									<VisualState x:Name='MouseOver'/>									
									<VisualState x:Name='Pressed'>
										<Storyboard>
											<DoubleAnimation Duration='0' To='0.85' Storyboard.TargetProperty='(UIElement.RenderTransform).(CompositeTransform.ScaleX)' Storyboard.TargetName='ButtonBackground'>
												<DoubleAnimation.EasingFunction>
													<CircleEase EasingMode='EaseIn'/>
												</DoubleAnimation.EasingFunction>
											</DoubleAnimation>
											<DoubleAnimation Duration='0' To='0.85' Storyboard.TargetProperty='(UIElement.RenderTransform).(CompositeTransform.ScaleY)' Storyboard.TargetName='ButtonBackground'>
												<DoubleAnimation.EasingFunction>
													<CircleEase EasingMode='EaseIn'/>
												</DoubleAnimation.EasingFunction>
											</DoubleAnimation>	                                      
										</Storyboard>
									</VisualState>
									<VisualState x:Name='Disabled'/>									
								</VisualStateGroup>
							</VisualStateManager.VisualStateGroups>
							<Ellipse  x:Name='ContentContainer'  StrokeThickness='" + DEFAULT_SROKE_THICKNES.ToString() + @"'  RenderTransformOrigin='0.5,0.5' >
								<Ellipse.RenderTransform>
									<CompositeTransform/>
								</Ellipse.RenderTransform>
							</Ellipse>		
										
						</Grid>
					</ControlTemplate>
";
            this.Template = XamlReader.Load(styleString) as ControlTemplate;
            bool b = this.ApplyTemplate();
            //System.Windows.Style btnStyle = new System.Windows.Style();
            //btnStyle.TargetType = typeof(System.Windows.Controls.Control);
            //Setter setterRed = new Setter(System.Windows.Controls.Control.BackgroundProperty, new SolidColorBrush(Colors.Red));
            //btnStyle.Setters.Add(setterRed);
            //this.Style = btnStyle;
        }

        private Ellipse ellipse;

        #region ImageSourceProperty

        /// <summary>
        /// 包含完整的素描文件信息（含素描笔迹数据）
        /// Tag：类型亦为SketchModel，但不包含素描笔迹数据
        /// </summary>
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set
            {
                SetValue(ImageSourceProperty, value);

            }
        }

        public static readonly DependencyProperty ImageSourceProperty =
          DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(EllipseButton), new PropertyMetadata(null, OnImageSourceChanged));

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ImageSource img = e.NewValue as ImageSource;
                EllipseButton obj = d as EllipseButton;
                if (obj.ellipse != null)
                {
                    obj.ellipse.Fill = new ImageBrush() { ImageSource = img };
                }
            }
        }

        #endregion

        #region ImageSourceProperty

        /// <summary>
        /// 包含完整的素描文件信息（含素描笔迹数据）
        /// Tag：类型亦为SketchModel，但不包含素描笔迹数据
        /// </summary>
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set
            {
                SetValue(BorderBrushProperty, value);

            }
        }

        public static readonly DependencyProperty BorderBrushProperty =
          DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(EllipseButton), new PropertyMetadata(null, OnBorderBrushChanged));

        private static void OnBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                Brush value = e.NewValue as Brush;
                EllipseButton obj = d as EllipseButton;
                if (obj.ellipse != null)
                {
                    obj.ellipse.Stroke = value;
                }
            }
        }

        #endregion

        protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!IsPressedStoryboard)
            {
                return;
            }

            base.OnIsPressedChanged(e);

            if (this.IsPressed)
            {
                Storyboard storyboard = CreateStoryBoard(this, 0, 30);
                storyboard.Begin();
            }
            else
            {
                Storyboard storyboard = CreateStoryBoard(this, 30, 0);
                storyboard.Begin();
            }
        }

        public void FocuseMe()
        {
            Storyboard storyboard = CreateStoryBoard(this, 0, 360);
            storyboard.Begin();
        }

        /// <summary>
        /// 生成缩放界限的恢复动画
        /// </summary>
        /// <param name="element"></param>
        /// <param name="scaleMinLimit"></param>
        /// <returns></returns>
        public static Storyboard CreateStoryBoard(FrameworkElement element, int begin, int end)
        {
            Storyboard resumBoard = new Storyboard();
            CompositeTransform compositeTransform = element.RenderTransform as CompositeTransform;
            if (compositeTransform == null)
            {
                element.RenderTransform = new CompositeTransform();
            }

            DoubleAnimationUsingKeyFrames scX = CreateDoubeFrame(begin, end);
            resumBoard.Children.Add(scX);
            Storyboard.SetTarget(scX, element);
            Storyboard.SetTargetProperty(scX, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.Rotation)"));
            resumBoard.AutoReverse = false;

            return resumBoard;
        }

        /// <summary>
        /// 创建动画帧
        /// </summary>
        /// <param name="valueBegin"></param>
        /// <param name="valueEnd"></param>
        /// <returns></returns>
        private static DoubleAnimationUsingKeyFrames CreateDoubeFrame(double valueBegin, double valueEnd)
        {
            DoubleAnimationUsingKeyFrames newAnimation = new DoubleAnimationUsingKeyFrames();
            EasingDoubleKeyFrame _frame1 = new EasingDoubleKeyFrame();
            _frame1.Value = valueBegin;
            _frame1.KeyTime = new TimeSpan(0, 0, 0, 0, 0);

            EasingDoubleKeyFrame _frame2 = new EasingDoubleKeyFrame();
            _frame2.Value = valueEnd;
            _frame2.KeyTime = new TimeSpan(0, 0, 0, 0, 50);

            newAnimation.KeyFrames.Add(_frame1);
            newAnimation.KeyFrames.Add(_frame2);

            return newAnimation;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.ellipse = this.GetTemplateChild("ContentContainer") as Ellipse;
        }

        public double StrokeThickness
        {
            get
            {
                if (this.ellipse != null)
                {
                    return this.ellipse.StrokeThickness;
                }

                return DEFAULT_SROKE_THICKNES;
            }
            set
            {
                if (this.ellipse == null)
                {
                    this.ApplyTemplate();
                    this.ellipse = this.GetTemplateChild("ContentContainer") as Ellipse;
                }

                if (this.ellipse != null)
                {
                    this.ellipse.StrokeThickness = value;
                }
            }
        }
    }
}

