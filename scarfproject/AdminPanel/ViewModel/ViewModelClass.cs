using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace AdminPanel.ViewModel
{
    public class ViewModelClass
    {
    }



    public class Datumm
    {
        public IEnumerable<SelectListItem> Colores { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }
        public IEnumerable<SelectListItem> Brands { get; set; }
        public FilterList filters { get; set; }
        public ProductFilterList productfilterlist { get; set; }
        public string ID { get; set; }
        public string count { get; set; }
        public string SetId { get; set; }

        public string discount { get; set; }
        public string title { get; set; }
        public string color { get; set; }
        public string type { get; set; }
        public string brand { get; set; }
        public string description { get; set; }
        public string productprice { get; set; }
        public List<Imagelist> imagelist { get; set; }
        public string IsNew { get; set; }
        public string IsOffer { get; set; }
        public string PriceNow { get; set; }
        public string isActive { get; set; }
    }
    public class Datum
    {
        public string ID { get; set; }
        public string count { get; set; }
        public string SetId { get; set; }

        public string discount { get; set; }
        public string title { get; set; }
        public string color { get; set; }
        public string type { get; set; }
        public string brand { get; set; }
        public string description { get; set; }
        public string productprice { get; set; }
        public List<Imagelist> imagelist { get; set; }
        public string IsNew { get; set; }
        public string IsOffer { get; set; }
        public string PriceNow { get; set; }
        public string isActive { get; set; }
    }
    public class earlyRoot
    {
        public List<Datum> data { get; set; }
    }

    public class catsdetail
    {
        public string ID { get; set; }
        public string title { get; set; }
        public string filtername { get; set; }

    }
    public class colordetail
    {
        public string ID { get; set; }
        public string title { get; set; }
        public string code { get; set; }

    }
    public class filterdetail
    {
        public string ID { get; set; }
        public string filtername { get; set; }

    }

    public class filtereslist
    {
        public List<filterdetail> data { get; set; }
    }
    public class FilterfordelViewModel
    {
        public string SelectedFilterfordel { get; set; }
        public IEnumerable<SelectListItem> Filtersfordel { get; set; }
    }
    public class coloreslist
    {
        public List<colordetail> data { get; set; }
    }
    public class catslist
    {
        public List<catsdetail> data { get; set; }
    }


    public class orderdetail
    {
        public string ID { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }

    public class oderdetaillist
    {
        public List<orderdetail> data { get; set; }
    }



    public class Imagelist
    {
        public string title { get; set; }
        public string ID { get; set; }

    }

    public class orderDT
    {
        public string ID { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public List<Imagelist> imagelist { get; set; }
    }

    public class orderdetaillst
    {
        public List<orderDT> data { get; set; }
    }

    public class sliderDT
    {
        public string ID { get; set; }
        public string title { get; set; }

    }

    public class sliderlst
    {
        public List<sliderDT> data { get; set; }
    }



    public class userdata
    {
        public string ID { get; set; }
        public string code { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
    }
    public class userinfo
    {
        public string ID { get; set; }
        public string fullname { get; set; }
        public string instagram { get; set; }
        public string telegram { get; set; }
        public string aboutus { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
        public string address { get; set; }
    }
    public class userinfolist
    {
        public List<userinfo> data { get; set; }
    }
    public class BaseViewModel
    {
        public String username { get; set; }
        public String pass { get; set; }
        public String name { get; set; }
    }

    public class imagelistfordel
    {
        public string title { get; set; }
    }


    public class Filterdetaile
    {
        public string ID { get; set; }
        public string detailname { get; set; }
    }

    public class Filter
    {
        public string ID { get; set; }
        public string filtername { get; set; }
        public string filterkinde { get; set; }
        public List<Filterdetaile> filterdetailes { get; set; }
    }

    public class FilterList
    {
        public List<Filter> filters { get; set; }
    }

    public class ProductFilter
    {
        public string detailname { get; set; }
        public string filterID { get; set; }
    }

    public class ProductFilterList
    {
        public List<ProductFilter> filters { get; set; }
    }
}