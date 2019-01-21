using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scarfproject.ViewModel
{
    public class userdata
    {
        public string ID { get; set; }
        public string code { get; set; }
        public string token { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
    }
    public class userdatalist
    {
        public List<userdata> data { get; set; } 
    }
   
}