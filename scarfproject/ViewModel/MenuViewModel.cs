using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scarfproject.ViewModel
{
    public class MenuViewModel
    {
        public List<catdetail> catlist { get; set; }
        public List<subcatdetail> subcatlist { get; set; }
        public List<subcatdetail2> subcatlist2 { get; set; }
    }
    public class catdetail
    {
        public string ID { get; set; }
        public string title { get; set; }
        public string IsFinal { get; set; }
    }

    public class catdetaillist
    {
        public List<catdetail> data { get; set; }
    }

    public class subcatdetail
    {
        public string ID { get; set; }
        public string title { get; set; }
        public string CatID { get; set; }
        public string IsFinal { get; set; }
    }

    public class subcatdetaillist
    {
        public List<subcatdetail> data { get; set; }
    }
    public class subcatdetail2
    {
        public string ID { get; set; }
        public string title { get; set; }
        public string subcatid { get; set; }
        public string IsFinal { get; set; }
    }

    public class subcatdetail2list
    {
        public List<subcatdetail2> data { get; set; }
    }


}