using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scarfproject.ViewModelPost
{
    
    public class Slide
    {
        public string image { get; set; }
        public string title { get; set; }
    }

    public class SimilarProduct
    {
        public int ID { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public string oldPrice { get; set; }
        public string price { get; set; }
    }

    public class OtherColor
    {
        public int ID { get; set; }
        public string colorTitle { get; set; }
        public string colorCode { get; set; }
        public string imageTitle { get; set; }
    }
    public class Filter
    {
        public string detailname { get; set; }
        public string filtername { get; set; }
    }

    public class viewProductViewModel
    {
        public string ID { get; set; }
        public List<Slide> slides { get; set; }
        public List<SimilarProduct> similarProduct { get; set; }
        public List<OtherColor> otherColors { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
        public string price { get; set; }
        public string oldPrice { get; set; }
        public string type { get; set; }
        public string brand { get; set; }
        public string color { get; set; }
        public string colorTitle { get; set; }
        public List<Filter> filter { get; set; }
    }
}