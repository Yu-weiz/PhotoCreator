using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Yuweiz.Phone.Media
{
    public class StoryBoards
    {
        /// <summary>
        /// 生成偏移界限的恢复动画
        /// </summary>
        /// <param name="element"></param>
        /// <param name="scaleMinLimit"></param>
        /// <returns></returns>
        public static Storyboard CreateTransLimitStoryBoard2(FrameworkElement element, FrameworkElement container, double transMinLimit)
        {
            Storyboard resumBoard = new Storyboard();

            CompositeTransform compositeTransform = element.RenderTransform as CompositeTransform;
            if (compositeTransform == null)
            {
                return resumBoard;
            }

            double transX = compositeTransform.CenterX + compositeTransform.TranslateX + transMinLimit;
            double transY = compositeTransform.CenterY + compositeTransform.TranslateY + transMinLimit;

            double transStanderX = container.ActualWidth - (compositeTransform.CenterX + transMinLimit);
            double transStanderY = container.ActualHeight - (compositeTransform.CenterY + transMinLimit);

            if (transX > container.ActualWidth || transY > container.ActualHeight)
            {
                DoubleAnimationUsingKeyFrames tsX = CreateDoubeFrame(compositeTransform.TranslateX, transStanderX);
                DoubleAnimationUsingKeyFrames tsY = CreateDoubeFrame(compositeTransform.CenterY, transStanderY);

                resumBoard.Children.Add(tsX);
                Storyboard.SetTarget(tsX, element);
                Storyboard.SetTargetProperty(tsX, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));

                resumBoard.Children.Add(tsY);
                Storyboard.SetTarget(tsY, element);
                Storyboard.SetTargetProperty(tsY, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            }

            return resumBoard;
        }

        /// <summary>
        /// 生成缩放界限的恢复动画
        /// </summary>
        /// <param name="element"></param>
        /// <param name="scaleMinLimit"></param>
        /// <returns></returns>
        public static Storyboard CreateScaleLimitStoryBoard(FrameworkElement element, double scaleMinLimit)
        {
            Storyboard resumBoard = new Storyboard();

            CompositeTransform compositeTransform = element.RenderTransform as CompositeTransform;
            if (compositeTransform == null)
            {
                return resumBoard;
            }

            double curScaleX = compositeTransform.ScaleX;
            double curScaleY = compositeTransform.ScaleY;

            DoubleAnimationUsingKeyFrames scX = CreateDoubeFrame(curScaleX, scaleMinLimit);
            DoubleAnimationUsingKeyFrames scY = CreateDoubeFrame(curScaleY, scaleMinLimit);

            ///小于最小值才生成功画
            if (curScaleX < scaleMinLimit)
            {
                resumBoard.Children.Add(scX);

                Storyboard.SetTarget(scX, element);
                Storyboard.SetTargetProperty(scX, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleX)"));

                resumBoard.Children.Add(scY);
                Storyboard.SetTarget(scY, element);
                Storyboard.SetTargetProperty(scY, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleY)"));

                resumBoard.AutoReverse = false;
                //storyTest.FillBehavior = FillBehavior.Stop;//加入这一行后，不管AutoReverse设置为何值，都会回复原状            
            }

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
            _frame2.KeyTime = new TimeSpan(0, 0, 0, 0, 500);

            newAnimation.KeyFrames.Add(_frame1);
            newAnimation.KeyFrames.Add(_frame2);

            return newAnimation;
        }


        /// <summary>
        /// 移动过界自动恢复
        /// </summary>
        /// <param name="curScaleX"></param>
        /// <param name="curScaleY"></param>
        /// <param name="limitScale">最小界限值</param>
        public static Storyboard CreateTransLimitStoryBoard(FrameworkElement element, Size containerSize, int transMinLimit)
        {
            Storyboard resumBoard = new Storyboard();

            CompositeTransform compositeTransform = element.RenderTransform as CompositeTransform;
            if (compositeTransform == null)
            {
                return resumBoard;
            }

            Point actualPox = GetActualPox(compositeTransform, new Size(element.ActualWidth, element.ActualHeight));
            Point? pt = GetActualLimitPox(actualPox, containerSize, element, transMinLimit);
            if (pt == null)
            {
                return resumBoard;
            }

            Point translatePt = GetTranslateLimitPox(compositeTransform, new Size(element.ActualWidth, element.ActualHeight), pt.Value);

            DoubleAnimationUsingKeyFrames tsX = CreateDoubeFrame(compositeTransform.TranslateX, translatePt.X);
            DoubleAnimationUsingKeyFrames tsY = CreateDoubeFrame(compositeTransform.TranslateY, translatePt.Y);

            resumBoard.Children.Add(tsX);
            Storyboard.SetTarget(tsX, element);
            Storyboard.SetTargetProperty(tsX, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));

            resumBoard.Children.Add(tsY);
            Storyboard.SetTarget(tsY, element);
            Storyboard.SetTargetProperty(tsY, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));


            resumBoard.AutoReverse = false;

            return resumBoard;
        }

        /// <summary>
        ///将含CompositeTransform变换可视元素的平移点，转换成实际（视觉中）的相对于屏幕左上方的平移距离
        /// </summary>
        /// <param name="frElement">移动的对象：且其变换类型为：CompositeTransform</param>       
        /// <returns></returns>
        private static Point GetActualPox(CompositeTransform compositeTransform, Size targetSize)
        {
            double actualPoxX = compositeTransform.TranslateX + (1 - compositeTransform.ScaleX) * targetSize.Width * (compositeTransform.CenterX / targetSize.Width);
            double actualPoxY = compositeTransform.TranslateY + (1 - compositeTransform.ScaleY) * targetSize.Height * (compositeTransform.CenterY / targetSize.Height);

            return new Point(actualPoxX, actualPoxY);
        }

        /// <summary>
        /// 获取边界量：即元素最多可平移的X，Y坐标
        /// </summary>
        /// <param name="curActualPox"></param>
        /// <param name="containerSize">容器大小</param>
        /// <returns></returns>
        private static Point? GetActualLimitPox(Point curActualPox, Size containerSize, FrameworkElement targetElement, int remainDistance)
        {
            if (!(targetElement.RenderTransform is CompositeTransform))
            {
                return null;
            }

            Point? limitPox = null;
            double limitPoxX = curActualPox.X;
            double limitPoxY = curActualPox.Y;

            CompositeTransform compositeTransform = targetElement.RenderTransform as CompositeTransform;
            double targetActualWidth = targetElement.ActualWidth * compositeTransform.ScaleX;
            double targetActualHeight = targetElement.ActualHeight * compositeTransform.ScaleY;

            if (curActualPox.X > containerSize.Width - remainDistance)
            {
                limitPoxX = containerSize.Width - remainDistance;
                limitPox = new Point(limitPoxX, limitPoxY);
            }
            else if (curActualPox.X < -1 * (targetActualWidth - remainDistance))
            {
                limitPoxX = -1 * (targetActualWidth - remainDistance);
                limitPox = new Point(limitPoxX, limitPoxY);
            }


            if (curActualPox.Y > containerSize.Height - remainDistance)
            {
                limitPoxY = containerSize.Height - remainDistance;
                limitPox = new Point(limitPoxX, limitPoxY);
            }
            else if (curActualPox.Y < -1 * (targetActualHeight - remainDistance))
            {
                limitPoxY = -1 * (targetActualHeight - remainDistance);
                limitPox = new Point(limitPoxX, limitPoxY);
            }

            return limitPox;
        }

        /// <summary>
        /// 获取实际要移动的偏移量
        /// </summary>
        /// <param name="compositeTransform"></param>
        /// <param name="targetSize"></param>
        /// <param name="actualLimitPox"></param>
        /// <returns></returns>
        private static Point GetTranslateLimitPox(CompositeTransform compositeTransform, Size targetSize, Point actualLimitPox)
        {
            double translateX = actualLimitPox.X - (1 - compositeTransform.ScaleX) * targetSize.Width * (compositeTransform.CenterX / targetSize.Width);
            double translateY = actualLimitPox.Y - (1 - compositeTransform.ScaleY) * targetSize.Height * (compositeTransform.CenterY / targetSize.Height);

            return new Point(translateX, translateY);
        }


    }
}
