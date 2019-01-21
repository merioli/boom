using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scarfproject.ViewModel
{
    public class ProductDetail : BaseViewModel
    {
        public int productid { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public decimal baseprice { get; set; }
        public decimal discount { get; set; }
        public string agreedprice { get; set; }
        public string description { get; set; }
        public string imageaddress1 { get; set; }
        public string imageaddress2 { get; set; }
        public string imageaddress3 { get; set; }
        public string imageaddress4 { get; set; }
        public string imageaddress5 { get; set; }
        public string imagethum { get; set; }




    }
}