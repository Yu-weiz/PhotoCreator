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
using System.Collections.Generic;
using WPPhotoEditor.OLD.Entity;

namespace WPPhotoEditor.Utility
{
    public class UnReDoManage
    {
        public List<LayerState> undoLayerStateCollection = new List<LayerState>();
        public List<LayerState> redoLayerStateCollection = new List<LayerState>();

        public void AddUndoToCollection(LayerState addLState)
        {
            if (undoLayerStateCollection.Count > 5)
            {
                undoLayerStateCollection.RemoveAt(0);
            }
            undoLayerStateCollection.Add(addLState);
        }

        public void AddRedoToCollection(LayerState addLState)
        {
            if (redoLayerStateCollection.Count > 5)
            {
                redoLayerStateCollection.RemoveAt(0);
            }
            redoLayerStateCollection.Add(addLState);
        }


        public LayerState GetUndoLayerState()
        {
            LayerState getLStae = null;
            if (undoLayerStateCollection.Count > 0)
            {
                getLStae = undoLayerStateCollection[undoLayerStateCollection.Count - 1];
                undoLayerStateCollection.RemoveAt(undoLayerStateCollection.Count - 1);
                AddRedoToCollection(getLStae);
            }

            return getLStae;
        }

        public LayerState GetRedoLayerState()
        {
            LayerState getLStae = null;
            if (redoLayerStateCollection.Count > 0)
            {
                getLStae = redoLayerStateCollection[redoLayerStateCollection.Count - 1];
                redoLayerStateCollection.RemoveAt(redoLayerStateCollection.Count - 1);
                AddUndoToCollection(getLStae);
            }

            return getLStae;
        }

        public void DelUnDoOnLayerIndex(int selIndex)
        {
            List<LayerState> delUnRedo = new List<LayerState>();

            #region 更新撤消
            foreach (LayerState lState in undoLayerStateCollection)
            {
                if (lState.LayerIndex == selIndex)
                {
                    delUnRedo.Add(lState);
                }
            }

            foreach (LayerState lState in delUnRedo)
            {
                undoLayerStateCollection.Remove(lState);
            }
            #endregion

            delUnRedo = new List<LayerState>();


            #region 更新重做
            foreach (LayerState lState in redoLayerStateCollection)
            {
                if (lState.LayerIndex == selIndex)
                {
                    delUnRedo.Add(lState);
                }
            }

            foreach (LayerState lState in delUnRedo)
            {
                redoLayerStateCollection.Remove(lState);
            }
            #endregion
        }
    }
}
