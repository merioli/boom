using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminPanel.ViewModel
{
    public class productinfoviewdetail
    {
        public string productname { get; set; }
        public string  producttag { get; set; }
        public string productunit { get; set; }
        public string SelectedColor { get; set; }
        public string SelectedCat { get; set; }
        public string Selectedfilters { get; set; }
        public string Selectedbrand { get; set; }
        public string productprice { get; set; }
        public string productdiscount { get; set; }
        public string productdesc { get; set; }
        public string SetID { get; set; }
        public List<HttpPostedFileBase>   file { get; set; }
        public string  inputallfilterid { get; set; }
       
    }
    public class productinfoforedit
    {
        public string ID { get; set; }
        public string title { get; set; }
        public string color { get; set; }
        public string type { get; set; }
        public string brand { get; set; }
        public string discount { get; set; }
        public string productprice { get; set; }
        public string SetID { get; set; }
        public string productdesc { get; set; }
        public List<HttpPostedFileBase> file { get; set; }
        public string inputallfilterid { get; set; }

    }
    public class sliderforedit
    {
     
        public List<HttpPostedFileBase> file { get; set; }

    }

}