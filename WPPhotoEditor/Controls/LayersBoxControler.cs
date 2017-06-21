using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPPhotoEditor.UControls
{
    /// <summary>
    /// Canvas控件横行排序控制器
    /// 固定坐标：(Left,Top)
    /// </summary>
    public class LayersBoxControler
    {
        public LayersBoxControler(Canvas container)
        {
            this.cavLayersContainer = container;

            this.ucLayerList = new List<UCLayerItem>();
        }

        public const int StartLeft = 10;
        public const int StartTop = 200;
        public const int IntervalH = 5;
        public const int IntervalV = 10;
        private double layerWidth = 124;

        private Canvas cavLayersContainer;

        private UCLayerItem selectedLayerItem;

        /// <summary>
        /// 逻辑管理的图层列表
        /// -->cavLayersContainer.Children(实际显示的有序图层列表)
        /// </summary>
        private List<UCLayerItem> ucLayerList;

        public event Action<int, int> LayerExchaged;

        public event Action<UCLayerItem> LayerSelected;

        public int LayersCount
        {
            get
            {
                if (this.ucLayerList == null)
                {
                    return 0;
                }
                else
                {
                    return this.ucLayerList.Count;
                }
                
            }
        }

        /// <summary>
        /// 添加图层项
        /// </summary>
        /// <param name="layer"></param>
        /// <returns>返回新增图层的显示索引</returns>
        public int AddLayer(UCLayerItem layer)
        {
            this.layerWidth = layer.Width;

            layer.Index = this.ucLayerList.Count;
            this.ucLayerList.Add(layer);
            this.cavLayersContainer.Children.Add(layer);

            this.SequenceLayers();
            this.SelectLayer(layer);

            return layer.Index;
        }

        public bool DeleteLayer(UCLayerItem layer)
        {
            if (this.ucLayerList.Count == 1)
            {
                MessageBox.Show(WPPhotoEditor.Resources.AppResources.UCOneLayerShouldBeKept);
                return false;
            }

            if (layer == null)
            {
                return false;
            }

            //更新图层序列索引          
            for (int i = layer.Index + 1; i < ucLayerList.Count; i++)
            {
                ucLayerList[i].Index = i - 1;
                Canvas.SetZIndex(ucLayerList[i], ucLayerList[i].Index);
            }

            ucLayerList.Remove(layer);
            this.cavLayersContainer.Children.Remove(layer);

            SequenceLayers();

            return true;
        }

        /// <summary>
        /// 交换两图层位置
        /// </summary>
        /// <param name="index1">要交换的图层索引1</param>
        /// <param name="index2">要交换的图层索引2</param>
        private void ExchageLayer(int index1, int index2)
        {
            if (ucLayerList == null)
            {
                return;
            }

            if (index1 < 0 || index2 < 0)
            {
                return;
            }

            if (index1 >= ucLayerList.Count || index2 >= ucLayerList.Count)
            {
                return;
            }

            UCLayerItem layer1 = ucLayerList[index1] as UCLayerItem;
            UCLayerItem layer2 = ucLayerList[index2] as UCLayerItem;

            layer1.Index = index2;
            layer2.Index = index1;

            Canvas.SetZIndex(layer2, layer2.Index);
            Canvas.SetZIndex(layer1, layer1.Index);

            ucLayerList[index1] = layer2;
            ucLayerList[index2] = layer1;

            // ExchageLayer(index1, index2);
            //触发图层层次更换事件
            // LayerSequenceChanged(layer1, new LayerSequenceEventArges() { SourceIndex = index1, TargetIndex = index2 }); ;
        }

        public void SelectLayer(UCLayerItem layer)
        {
            foreach (UCLayerItem layerItem in ucLayerList)
            {
                if (layerItem != layer && layerItem != null)
                {
                    layerItem.HadChosen = false;
                }
            }

            if (layer != null)
            {
                layer.HadChosen = true;
            }

            selectedLayerItem = layer;

            if (this.LayerSelected != null)
            {
                this.LayerSelected(layer);
            }

           
        }

        /// <summary>
        /// 选择当前图层的下面一个非隐藏图层，如果没有则从上面找
        /// 注意：隐藏问题，如果所有图层都隐藏掉，则会找不到，故以后需要考虑是否允许隐藏所图层
        /// </summary>
        /// <param name="AaccordingItem"></param>
        public void SelectUnderLayer(UCLayerItem AaccordingItem)
        {
            UCLayerItem selectItem = null;

            #region 向下选中
            for (int i = AaccordingItem.Index - 1; i >= 0; i--)
            {
                UCLayerItem item = ucLayerList[i];
                if (item.IsVisibleStatus)
                {
                    selectItem = item;
                    break;
                }
            }
            #endregion

            #region 向下没有，则向上选
            if (selectedLayerItem == null)
            {
                for (int i = AaccordingItem.Index + 1; i < this.ucLayerList.Count; i++)
                {
                    UCLayerItem item = ucLayerList[i];
                    if (item.IsVisibleStatus)
                    {
                        selectItem = item;
                        break;
                    }
                }
            }
            #endregion

            this.SelectLayer(selectItem);
        }

        public void CheckLayerExchageNeedWhenMoving(UCLayerItem layerItem, PoxEventArgs e)
        {
            Point? pt = GetAbsolutePosition(layerItem);
            if (pt != null)
            {
                double totalPox = e.X - pt.Value.X;
                int? exchangeIndex = GetExchangeLayerIndex(layerItem, totalPox);
                if (exchangeIndex != null && exchangeIndex.Value != layerItem.Index)
                {
                    //动画移动目标图层项
                    UCLayerItem layer = this.ucLayerList[exchangeIndex.Value] as UCLayerItem;
                    if (layer != null)
                    {
                        Canvas.SetLeft(layer, pt.Value.X);
                        Canvas.SetTop(layer, pt.Value.Y);

                        if (LayerExchaged != null)
                        {
                            this.LayerExchaged(exchangeIndex.Value, layerItem.Index);
                        }

                        ExchageLayer(exchangeIndex.Value, layerItem.Index);
                    }

                }
            }
        }

        /// <summary>
        /// 设置图层控件到序列位置
        /// </summary>
        /// <param name="layer"></param>
        public void ReNewLayerPosition(UCLayerItem layer)
        {
            if (layer == null)
            {
                return;
            }

            Point? pt = GetAbsolutePosition(layer);
            Canvas.SetLeft(layer, pt.Value.X);
            Canvas.SetTop(layer, pt.Value.Y);
        }

        /// <summary>
        /// 更新指定图层项的缩略图
        /// </summary>
        /// <param name="bmpSource"></param>
        public void UpdateLayerItemPreview(BitmapSource bmpSource, int index)
        {
            if (index < 0 || index > this.ucLayerList.Count - 1)
            {
                return;
            }

            this.ucLayerList[index].LayerImage = bmpSource;
        }


        /// <summary>
        /// 查测图层交换的索引
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="totalPox">X轴上移动的距离</param>
        /// <returns></returns>        
        private int? GetExchangeLayerIndex(UCLayerItem layer, double totalPox)
        {
            int? exchangeIndex = null;
            int curIndex = layer.Index;


            if (totalPox > (layer.Width / 2 * 2 / 3))
            {
                if (curIndex < this.ucLayerList.Count - 1)
                {
                    exchangeIndex = curIndex + 1;
                }
            }
            else if (totalPox < (-1 * layer.Width / 2 * 2 / 3))
            {
                if (curIndex > 0)
                {
                    exchangeIndex = curIndex - 1;
                }
            }


            if (exchangeIndex != null)
            {
                System.Diagnostics.Debug.WriteLine("原索引：" + curIndex.ToString() + "  目索引：" + exchangeIndex.ToString());
            }
            return exchangeIndex;
        }

        /// <summary>
        /// 获取指定图层索引对应的固定坐标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Point GetAbsolutePosition(int index)
        {
            double left = LayersBoxControler.StartLeft;
            double top = LayersBoxControler.StartTop;


            for (int i = 0; i < index; i++)
            {
                left += (this.layerWidth / 2 + LayersBoxControler.IntervalH);
                top -= LayersBoxControler.IntervalV;
            }
            return new Point(left, top);
        }

        /// <summary>
        ///  获取指定图层对应的固定坐标
        ///  控件左上角
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public Point? GetAbsolutePosition(UCLayerItem layer)
        {
            if (layer == null)
            {
                return null;
            }

            return GetAbsolutePosition(layer.Index);
        }

        /// <summary>
        /// 使图层按顺序排列
        /// </summary>
        private void SequenceLayers()
        {
            double left = LayersBoxControler.StartLeft;
            double top = LayersBoxControler.StartTop;

            //ucLayerList.Clear();


            // IEnumerable<UCLayerItem> query=this.cavLayersContainer.Children.OrderBy((item)=>item.Index);

            for (int i = 0; i < this.ucLayerList.Count; i++)
            {
                UCLayerItem layer = this.ucLayerList[i];
                if (layer != null)
                {
                    Canvas.SetLeft(layer, left);
                    Canvas.SetTop(layer, top);
                    Canvas.SetZIndex(layer, i);

                    left += (layer.Width / 2 + LayersBoxControler.IntervalH);
                    top -= LayersBoxControler.IntervalV;

                    //调试：
                    //layer.Index = i;
                    //if (layer != this._curLayer)
                    //{
                    //    layer.HadChoosen = false;
                    //}

                    // bucLayerList.Add(layer);
                }


            }
        }
    }

}
