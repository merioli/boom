using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scarfproject.ViewModePost
{
    
    public class Slide
    {
        public int ID { get; set; }
        public string image { get; set; }
        public string title { get; set; }
    }

    public class Cat
    {
        public int ID { get; set; }
        public string title { get; set; }
        public int IsFinal { get; set; }
        public int catLevel { get; set; }
    }

    public class Newest
    {
        public int ID { get; set; }
        public string title { get; set; }
        public string price { get; set; }
        public string image { get; set; }
        public string oldPrice { get; set; }
    }

    public class Bestseller
    {
        public int ID { get; set; }
        public string title { get; set; }
        public string price { get; set; }
        public string image { get; set; }
        public string oldPrice { get; set; }
    }

    public class SpecialOffer
    {
        public string ID { get; set; }
        public string title { get; set; }
        public string price { get; set; }
        public string image { get; set; }
        public string oldPrice { get; set; }
    }

    public class getMaindataViewModel
    {
        public string banner1 { get; set; }
        public string banner2 { get; set; }
        public List<Slide> slides { get; set; }
        public List<Cat> cats { get; set; }
        public List<Newest> newest { get; set; }
        public List<Bestseller> bestseller { get; set; }
        public List<SpecialOffer> specialOffers { get; set; }
    }
}