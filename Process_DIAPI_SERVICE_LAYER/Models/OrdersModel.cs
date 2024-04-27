using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Process_DIAPI_SERVICE_LAYER.Models
{
    public class OrdersModel
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public DateTime DocDueDate { get; set; }
        public int BPL_IDAssignedToInvoice { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Comments { get; set; }
        public List<Orderline> DocumentLines { get; set; }
    }

    public class Orderline
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }

    }


}
