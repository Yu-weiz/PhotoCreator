using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Ad.Model
{
    public class AdOrderModel
    {
        public AdOrderModel()
        {   
            this.ValidDays = 0;
            this.Message = "Please check you order number,It's invalid.";
        }

        public AdOrderModel(AdOrderDataModel model)
        {
            if (model == null)
            {
                this.ValidDays = 0;
                this.Message = "Please check you order number,It's invalid.";
                return;
            }

            this.IsAuthorization = model.Authorization == "true" ? true : false;

            int days = 0;
            int.TryParse(model.ValidDays, out days);
            this.ValidDays = days;

            this.Message = model.Message;
        }

        public bool IsAuthorization { get; set; }

        public int ValidDays { get; set; }

        public string Message { get; set; }
    }
}
