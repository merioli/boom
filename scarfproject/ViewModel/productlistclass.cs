using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using scarfproject.ViewModelPost;

namespace scarfproject.ViewModel
{
    public class productlistclass
    {
        

        public List<earlydatum> list { get; set; }

        public listofordermode dropdownlist { get; set; }
        public string  catid { get; set; }
        public string subcatid { get; set; }
        public ProductListFilterViewModel filtergroup { get; set; }
        
    }
}