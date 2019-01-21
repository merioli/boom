using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using scarfproject.ViewModel;

namespace scarfproject.ViewModel
{
    public class cartmodelview : BaseViewModel
    {
        public List<ProductDetail> listofproduct { get; set; }
    }
}