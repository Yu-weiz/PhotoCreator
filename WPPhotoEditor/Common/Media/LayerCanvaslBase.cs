using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using WPPhotoEditor.Media.Tools;
using System;
using WPPhotoEditor.ViewModels;
using WPPhotoEditor.Common;

namespace WPPhotoEditor.Media
{
    /// <summary>
    /// 图标容器
    /// </summary>
    public class LayerCanvaslBase : Canvas
    {
        public LayerCanvaslBase()
        {
            this.InitializeDefault();
        }

        private void InitializeDefault()
        {
            //this.AddLayer(null);
            //this.tool = DrawTool.Instance;
            //this.tool.BindLayer(this.selectedLayer);

            //AppSession.Instance.HistoryManager.Clear();
            //this.Width = 480;
            //this.Height = 640;
        }

        public Action<LayerBase> ShowLayerToolAction { get; set; }

        public Action<LayerBase> CheckToShowLayerToolAction { get; set; }

        public void BindDrawTool()
        {
            foreach (LayerBase layer in this.Children)
            {
                layer.Tool = new DrawTool();
            }

            //if (CurrentLayer != null)
            //{
            //    CurrentLayer.Tool = DrawTool.Instance;
            //}
        }

        public void BindDrawToolInstance()
        {
            if (CurrentLayer != null)
            {
                CurrentLayer.Tool = DrawTool.Instance;
            }
        }

        public LayerBase CurrentLayer { get; private set; }

        public void BindTransformTool()
        {
            foreach (LayerBase layer in this.Children)
            {
                layer.Tool = new TransformTool() { ShowLayerToolAction = this.ShowLayerToolAction, CheckShowLayerToolAction = this.CheckToShowLayerToolAction };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp">图层图像</param>
        /// <param name="index">图层ZIndex显示索引</param>
        public void AddLayer(BitmapSource bmp)
        {
            if (bmp == null)
            {
                AddLayer();
                return;
            }

            LayerBase newLayer = new LayerBase();
            newLayer.Index = this.Children.Count;
            newLayer.Width = System.Windows.Application.Current.Host.Content.ActualWidth;
            newLayer.Height = System.Windows.Application.Current.Host.Content.ActualHeight;
            newLayer.Tool = new TransformTool() { ShowLayerToolAction = this.ShowLayerToolAction, CheckShowLayerToolAction = this.CheckToShowLayerToolAction };
            newLayer.BackgroundWbmp = bmp is WriteableBitmap ? bmp as WriteableBitmap : new WriteableBitmap(bmp);

            this.Children.Add(newLayer);
            Canvas.SetZIndex(newLayer, newLayer.Index);

            CurrentLayer = newLayer;
        }

        public void AddLayer()
        {
            LayerBase newLayer = new LayerBase();
            newLayer.Index = this.Children.Count;
            newLayer.Width = System.Windows.Application.Current.Host.Content.ActualWidth;
            newLayer.Height = System.Windows.Application.Current.Host.Content.ActualHeight;
            newLayer.Background = new SolidColorBrush() { Color = Colors.White };  // Color.FromArgb(0, 255, 255, 255)

            this.Children.Add(newLayer);
            Canvas.SetZIndex(newLayer, newLayer.Index);

            CurrentLayer = newLayer;
        }

        /// <summary>
        /// 注意：cavLayersContainer，仅能包含图层控制
        /// </summary>
        /// <param name="index"></param>
        public void ChangeLayerVisibility(int index)
        {
            LayerBase layer = this.GetLayer(index);
            layer.Visibility = layer.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public void SelectLayer(int index)
        {
            LayerBase layer = GetLayer(index);
            layer.Tool = DrawTool.Instance;

            CurrentLayer = layer;
        }

        /// <summary>
        /// 注意：cavLayersContainer，仅能包含图层控制
        /// </summary>
        /// <param name="index"></param>
        public void DeleteLayer(int index)
        {
            //更新图层序列索引          
            for (int i = index + 1; i < this.Children.Count - 1; i++)
            {
                (this.Children[i] as LayerBase).Index = i - 1;
                Canvas.SetZIndex(this.Children[i], i - 1);
            }

            LayerBase layer = this.GetLayer(index);
            this.Children.Remove(layer);
        }

        //[Obsolete("索引未更新")]测试
        public void DeleteLayer(LayerBase layer)
        {
            int zIndex = Canvas.GetZIndex(layer);
            //更新图层序列索引          
            for (int i = 0; i < this.Children.Count; i++)
            {
                int itemIndex = Canvas.GetZIndex(this.Children[i]);
                if (itemIndex == zIndex + i)
                {
                    Canvas.SetZIndex(this.Children[i], itemIndex - 1);
                }
            }

            this.Children.Remove(layer);
        }

        public void ExchageLayer(int index1, int index2)
        {
            LayerBase LayerBase1 = GetLayer(index1);
            LayerBase LayerBase2 = GetLayer(index2);
            if (LayerBase1 == null || LayerBase2 == null)
            {
                return;
            }

            int tempIndex = index1;
            LayerBase1.Index = index2;
            LayerBase2.Index = index1;

            Canvas.SetZIndex(LayerBase1, LayerBase1.Index);
            Canvas.SetZIndex(LayerBase2, LayerBase2.Index);
        }

        public void SetTopLayer(int index)
        {
            int maxIndex = index;
            foreach (LayerBase LayerBase in this.Children)
            {
                if (LayerBase.Index > maxIndex)
                {
                    maxIndex = LayerBase.Index;
                }
            }

            LayerBase targetLayer = GetLayer(index);
            for (int i = index+1; i < maxIndex + 1; i++)
            {
                LayerBase layerBase = GetLayer(i);
                layerBase.Index--;
                Canvas.SetZIndex(layerBase, layerBase.Index);
            }
                      
            targetLayer.Index = maxIndex;
            Canvas.SetZIndex(targetLayer, maxIndex);
         
        }
             
        public void Undo()
        {
            PEHistoryItem peHistoryItem = AppSession.Instance.HistoryManager.Undo() as PEHistoryItem;
            if (peHistoryItem != null)
            {
                if ((peHistoryItem.OperationType & PEOperationType.Draw) == PEOperationType.Draw)
                {
                    peHistoryItem.LayerBase.BackgroundWbmp = peHistoryItem.Wbmp;
                }
                else
                {
                    peHistoryItem.LayerBase.OpacityMask = new ImageBrush() { ImageSource = peHistoryItem.Wbmp, Stretch = Stretch.Fill };
                }
            }
        }

        public void Redo()
        {
            PEHistoryItem peHistoryItem = AppSession.Instance.HistoryManager.Redo() as PEHistoryItem;
            if (peHistoryItem != null)
            {
               if ((peHistoryItem.OperationType & PEOperationType.Draw) == PEOperationType.Draw)
                {
                    peHistoryItem.LayerBase.BackgroundWbmp = peHistoryItem.Wbmp;
                }
                else
                {
                    peHistoryItem.LayerBase.OpacityMask = new ImageBrush() { ImageSource = peHistoryItem.Wbmp, Stretch = Stretch.Fill };
                }
            }
        }

        /// <summary>
        /// 将ZIndex设置更小值
        /// </summary>
        /// <param name="layer"></param>
        public void MoveDown(LayerBase layer)
        {
            int zIndex = Canvas.GetZIndex(layer);
            //更新图层序列索引          
            for (int i = this.Children.Count - 1; i > -1; i--)
            {
                if (Canvas.GetZIndex(this.Children[i]) == zIndex - 1)
                {
                    Canvas.SetZIndex(this.Children[i], zIndex);
                    Canvas.SetZIndex(layer, zIndex - 1);
                    break;
                }
            }
        }

        /// <summary>
        /// 将ZIndex设置更小值
        /// </summary>
        /// <param name="layer"></param>
        public void MoveUp(LayerBase layer)
        {
            int zIndex = Canvas.GetZIndex(layer);
            //更新图层序列索引          
            for (int i = this.Children.Count - 1; i > -1; i--)
            {
                if (Canvas.GetZIndex(this.Children[i]) == zIndex + 1)
                {
                    Canvas.SetZIndex(this.Children[i], zIndex);
                    Canvas.SetZIndex(layer, zIndex + 1);
                    break;
                }
            }
        }

        /// <summary>
        /// 更新图层管理的缩略图
        /// </summary>
        /// <param name="updateLayerItemPreview"></param>
        public void UpdateLayerItemsPreview(Action<BitmapSource, int> updateLayerItemPreview)
        {
            if (updateLayerItemPreview == null)
            {
                return;
            }

            foreach (LayerBase layer in this.Children)
            {

                WriteableBitmap wbmp = new WriteableBitmap(layer, null);
                updateLayerItemPreview(wbmp, layer.Index);

            }
        }

        /// <summary>
        /// 合并可见图层
        /// </summary>
        public WriteableBitmap MergeVisibleLayers()
        {
            WriteableBitmap mergedWbmp = new WriteableBitmap(this, new ScaleTransform() { ScaleX = 2, ScaleY = 2 });
            List<int> delIndexList = new List<int>();

            foreach (LayerBase layer in this.Children)
            {
               // if (layer.Visibility == Visibility.Visible)
                {
                    delIndexList.Add(layer.Index);
                }
            }

            foreach (int index in delIndexList)
            {
                this.DeleteLayer(index);
            }

            this.AddLayer(mergedWbmp);

            return mergedWbmp;
        }

        private LayerBase GetLayer(int index)
        {
            foreach (LayerBase LayerBase in this.Children)
            {
                if (LayerBase.Index == index)
                {
                    return LayerBase;
                }
            }

            return null;
        }

        public void ClearOpacityMask()
        {
            foreach (LayerBase layer in this.Children)
            {
                layer.OpacityMask = null;
            }
        }

        public void ClearDrawed()
        {
            foreach (LayerBase layer in this.Children)
            {
                layer.Background = new SolidColorBrush(Colors.Transparent);
            }
        }
    }
}
