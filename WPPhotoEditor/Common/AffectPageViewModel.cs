using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPPhotoEditor.Models;

namespace WPPhotoEditor.ViewModels
{
    public class AffectPageViewModel
    {
        public AffectPageViewModel()
        {
            this.FilterItemModelCollection = new ObservableCollection<FilterItemModel>();

            int i = 0;
            foreach (FilterItem item in FilterItem.FilterSource)
            {
                int hadNum = usedFilters.Where(num => num == item.Index).FirstOrDefault();
                if ((hadNum == 0 && i == 0) || hadNum > 0)
                {
                    FilterItemModel model = new FilterItemModel() { FilterItem = item, Index = i++ };
                    this.FilterItemModelCollection.Add(model);
                }
            }

            this.CurFilterItemModel = this.FilterItemModelCollection[0];
        }

        public FilterItemModel CurFilterItemModel { get; set; }

        private static int[] usedFilters = { 0, 2, 6, 7, 8, 9, 13, 22, 23, 24, 26, 27, 28, 29, 32, 43, 47, 52, 53 };

        public static int FiltersCount
        {
            get { return usedFilters.Length; }
        }

        public ObservableCollection<FilterItemModel> FilterItemModelCollection { get; set; }

        public async Task<int> UpdateFilterSampleList()
        {
            int i = 0;
            foreach (FilterItemModel item in this.FilterItemModelCollection)
            {
                bool updated = await item.UpdateSample(AppSession.TempAffectBitmapPath);
                i = updated ? i + 1 : i;
            }

            return i;
        }


    }
}
