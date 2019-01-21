using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scarfproject.ViewModel
{
    public class slider
    {
    }
    public class sliderlst
    {
        public List<sliderDT> data { get; set; }
    }
    public class sliderDT
    {
        public string ID { get; set; }
        public string title { get; set; }

    }
}