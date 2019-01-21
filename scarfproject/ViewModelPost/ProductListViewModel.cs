using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scarfproject.ViewModelPost
{
    
    public class Product
    {
        public int ID { get; set; }
        public string title { get; set; }
        public int price { get; set; }
        public string image { get; set; }
        public int oldPrice { get; set; }
        public string desc { get; set; }
        public string type { get; set; }
        public string brand { get; set; }
        public string color { get; set; }
        public int colorCount { get; set; }
        public int isSpecial { get; set; }
        public List<string> colors { get; set; }
    }

    public class ProductListViewModel
    {
        public List<Product> products { get; set; }
        public string productsCounts { get; set; }
        public string currentPage { get; set; }
    }
}