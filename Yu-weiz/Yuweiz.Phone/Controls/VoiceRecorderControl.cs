using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Yuweiz.Phone.Controls
{
  public  class VoiceRecorderControl:Button
    {
      public VoiceRecorderControl()
        {
            this.InitializeStyle();
            this.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        private void InitializeStyle()
        {
            string styleString = @"  
                       <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'  xmlns:d='http://schemas.microsoft.com/expression/blend/2008'
    xmlns:mc='http://schemas.openxmlformats.org/markup-compatibility/2006' TargetType='Button'>
						<Grid x:Name='ButtonBackground'  RenderTransformOrigin='0.5,0.5'>
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
							<Ellipse  x:Name='ContentContainer'  Stroke='{TemplateBinding BorderBrush}' StrokeThickness='{StaticResource PhoneStrokeThickness}' Fill='{TemplateBinding Background}' RenderTransformOrigin='0.5,0.5' >
								<Ellipse.RenderTransform>
									<CompositeTransform/>
								</Ellipse.RenderTransform>
							</Ellipse>
							<Path Data='M36.5,32.5 L36.5,76.5 L72.5,52.5 z' Fill='{TemplateBinding Foreground}' Margin='35,26.5,25,23.5' Stretch='Fill' Stroke='{TemplateBinding BorderBrush}' UseLayoutRounding='False' StrokeThickness='0'/>												
						</Grid>
					</ControlTemplate>
";
            this.Template = XamlReader.Load(styleString) as ControlTemplate;

            //System.Windows.Style btnStyle = new System.Windows.Style();
            //btnStyle.TargetType = typeof(System.Windows.Controls.Control);
            //Setter setterRed = new Setter(System.Windows.Controls.Control.BackgroundProperty, new SolidColorBrush(Colors.Red));
            //btnStyle.Setters.Add(setterRed);
            //this.Style = btnStyle;
        }

        protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsPressedChanged(e);

          
        }
    }
}
