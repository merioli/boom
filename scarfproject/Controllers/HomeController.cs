using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Newtonsoft.Json;
using scarfproject.ViewModel;
using scarfproject.ViewModePost;
using System.Web.Script.Serialization;
using scarfproject.Classes;
using System.IO;
using System.Text;
using scarfproject.ServiceReference1;
using System.Web.SessionState;
using System.Drawing;
using System.Web.Helpers;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Specialized;



namespace scarfproject.Controllers
{


    public class HomeController : Controller
    {

        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        public static string RandomString()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public ActionResult Index()
        {

            string device = RandomString();
            string code = MD5Hash(device + "ncase8934f49909");


           //Session["currentpage"] = "/Home/Index";
           //GlobalVariables.ordermode = "1";
           // GlobalVariables.specialpaginationid = "1";
           // GlobalVariables.offerpaginationid = "1";
           //  GlobalVariables.bestsellerpaginationid = "1";
           // GlobalVariables.pagenumberactive = "1";

            productinfoviewdetail model = new productinfoviewdetail();

            string result = "";
            using (WebClient client = new WebClient())
            {

                var collection = new NameValueCollection();
                collection.Add("device", device);
                collection.Add("code", code);

                //foreach (var myvalucollection in imaglist) {
                //    collection.Add("imaglist[]", myvalucollection);
                //}
                byte[] response =
                client.UploadValues("http://supectco.com/webs/textile/Main/handler/getMainData.php", collection);

                result = System.Text.Encoding.UTF8.GetString(response);
            }
            getMaindataViewModel log2 = JsonConvert.DeserializeObject<getMaindataViewModel>(result);


            //string json = "";
            //using (var client = new WebClient())
            //    json = client.DownloadString("http://supectco.com/webs/cosmetic/Admin/addProductPost.php?device=" + device + "&code=" + code);

            //var log = JsonConvert.DeserializeObject<sliderlst>(json);
            //List<sliderDT> mylist = new List<sliderDT>();
            ////getmemyearlydataViewModel model = new getmemyearlydataViewModel();
            //sliderDT newearlydatum = new sliderDT();
            //// newearlydatum = log.data[0];
            //if (log.data != null)
            //{
            //    foreach (var myvar in log.data)
            //    {
            //        mylist.Add(myvar);
            //    }
            //}



            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            Session["currentpage"] = "/Home/Index";
            Session["ordermodel"] = "1";
            Session["pagenumberactive"] = "1";

            return View(log2);






        } 
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ProductList(string catmode, string sortID)
        {
            Session["filterIds"] = "";
            Session["colorIds"] = "";
            Session["min"] = "";
            Session["max"] = "";

            Session ["sortID"] = sortID;
            string device = RandomString();
            string code = MD5Hash(device + "ncase8934f49909");
            string catLevelforuse = "";
             string catidforuse = "";
             HttpCookie catID = new HttpCookie("catID");
             HttpCookie catLevel = new HttpCookie("catLevel");
             HttpCookie query = new HttpCookie("query");
             query.Value = "";
            if (catmode == null)
            {
                catLevelforuse = "0";
                catidforuse = "0";
               
                catID.Value = "0";
                catLevel.Value = "0";
                
               
            }
            else
            {
                catLevelforuse = catmode.Substring(0, 1);
                catidforuse = catmode.Substring(1, catmode.Count() - 1);

                catID.Value = catidforuse;
                catLevel.Value = catLevelforuse;
            }

            catID.Expires = DateTime.Now.AddMinutes(360);
            catLevel.Expires = DateTime.Now.AddMinutes(360);
            query.Expires = DateTime.Now.AddMinutes(360);
            Response.Cookies.Add(catID);
            Response.Cookies.Add(catLevel);
            Response.Cookies.Add(query);

            string result = "";
            using (WebClient client = new WebClient())
            {

                var collection = new NameValueCollection();
                collection.Add("device", device);
                collection.Add("code", code);
                collection.Add("catLevel", catLevelforuse);
                collection.Add("catID", catidforuse);
                
                byte[] response =
                client.UploadValues("http://supectco.com/webs/textile/Main/handler/getTypeList.php", collection);

                result = System.Text.Encoding.UTF8.GetString(response);
            }
            scarfproject.ViewModelPost.ProductListFilterViewModel log2 = JsonConvert.DeserializeObject<scarfproject.ViewModelPost.ProductListFilterViewModel>(result);

                //GlobalVariables.catmode = catmode;
                //string  pagenumberactive = GlobalVariables.pagenumberactive;
         
                //GlobalVariables.currentpage = "/Home/shargh?catid=" + catmode + "&pagenumberactive=" + pagenumberactive;
                //Session["currentpage"] = "/Home/shargh?catid=" + catmode + "&pagenumberactive=" + pagenumberactive;
                productlistclass model = new productlistclass();
                List<SelectListItem> list = new List<SelectListItem>() {
                new SelectListItem(){ Value="1", Text="تازه ترین ها"},
                new SelectListItem(){ Value="2", Text="پربازدید ترین ها"},
                new SelectListItem(){ Value="3", Text="پرفروش ترین ها"},
                new SelectListItem(){ Value="4", Text="قیمت زیاد به کم"},
                new SelectListItem(){ Value="5", Text="قیمت کم به زیاد"}
                
            };
            listofordermode dropdown = new listofordermode();
            dropdown.modes = new SelectList(list, "Value", "Text");

            if (catmode == null) {
                model.filtergroup = log2;
            }
            else
            {
                model.dropdownlist = dropdown;
                model.filtergroup = log2;
            }
           



            return View(model);
        }
        public void changecolorides(string ID) 
        {

            Session["colorIds"] = ID;
           
        }
        
        public void changefilterides(string ID)
        {
            Session["filterIds"] = ID;
        }
        public void changeRangeIDes(string min, string max)
        {
            Session["min"] = min;
            Session["max"] = max;
        }
        
        
        public PartialViewResult gogetproductlist( )
        {
            string sortID = Session["sortID"] as string ;
            string priorityID = GlobalVariables.priorityID;
            string page = Session["pagenumberactive"] as string ;
            string colorIds = Session["colorIds"] as string;
            string filterIds = Session["filterIds"] as string;
            string min = Session["min"] as string;
            string max = Session["max"] as string;
            //string brandIds = GlobalVariables.brandIds;
            string query = Request.Cookies["query"].Value;
            string catID = Request.Cookies["catID"].Value;
            string catLevel = Request.Cookies["catLevel"].Value;




            string device = RandomString();
            string code = MD5Hash(device + "ncase8934f49909");
            if (catID == null)
            {
                catID = "0";
            }
            string result = "";
            using (WebClient client = new WebClient())
            {

                var collection = new NameValueCollection();
                collection.Add("device", device);
                collection.Add("code", code);
                collection.Add("sortID", sortID);
                collection.Add("priorityID", priorityID);
                collection.Add("page", page);
                collection.Add("colorIds", colorIds);
                collection.Add("filterIds", filterIds);
                collection.Add("min", min);
                collection.Add("max", max);
                collection.Add("query", query);
                collection.Add("catID", catID);
                collection.Add("catLevel", catLevel);

                //foreach (var myvalucollection in imaglist) {
                //    collection.Add("imaglist[]", myvalucollection);
                //}
                byte[] response =
                client.UploadValues("http://supectco.com/webs/textile/Main/handler/getDataProductList.php", collection);

                result = System.Text.Encoding.UTF8.GetString(response);
            }

            scarfproject.ViewModelPost.ProductListViewModel log2 = JsonConvert.DeserializeObject<scarfproject.ViewModelPost.ProductListViewModel>(result);
            if (page == null) {
                log2.currentPage = "1";
            }
            if (Convert.ToInt32(log2.productsCounts) % 10 > 0) {
                log2.productsCounts = ((Convert.ToInt32(log2.productsCounts) / 10) + 1).ToString();
            }
            else
            {
                log2.productsCounts = (Convert.ToInt32(log2.productsCounts) / 10).ToString();
            }
            if (log2.products != null)
            {
                foreach (var product in log2.products)
                {
                    string myString = product.color;
                    string[] splitString = myString.Split('$');
                    List<string> colors = new List<string>();
                    foreach (var color in splitString)
                    {
                        colors.Add(color);
                    }
                    product.colors = colors;
                }
            }
            
            
            return PartialView("/Views/Shared/_ProductListForBUTFULImage.cshtml", log2);
        }
        public PartialViewResult gogetspectialpagination(string id)
        {
            string specialpaginationid = GlobalVariables.specialpaginationid;


            ListAndPaginationViewModel model = new ListAndPaginationViewModel();
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://supectco.com/webs/textile/getnumrows.php?catid=" + id);
            }





            int totalrows = Convert.ToInt32(json.Substring(1));
            if (totalrows == 1)
            {
                return PartialView("/Views/Shared/_fakeaddress.cshtml", model);
            }
            if (totalrows % 12 == 0)
            {
                model.totalrows = totalrows / 12;

            }
            else
            {
                model.totalrows = (totalrows / 12) + 1;
            }
            model.activerow = Convert.ToInt32(specialpaginationid);



            return PartialView("/Views/Shared/_PaginationPartialForNewProduct.cshtml", model);
        }
        public PartialViewResult gogetofferpagination(string id)
        {
            string offerpaginationid = GlobalVariables.offerpaginationid;


            ListAndPaginationViewModel model = new ListAndPaginationViewModel();
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://supectco.com/webs/textile/getnumrows.php?catid=" + id);
            }





            int totalrows = Convert.ToInt32(json.Substring(1));
            if (totalrows == 1)
            {
                return PartialView("/Views/Shared/_fakeaddress.cshtml", model);
            }
            if (totalrows % 12 == 0)
            {
                model.totalrows = totalrows / 12;

            }
            else
            {
                model.totalrows = (totalrows / 12) + 1;
            }
            model.activerow = Convert.ToInt32(offerpaginationid);



            return PartialView("/Views/Shared/_PaginationPartialForOfferProduct.cshtml", model);
        }
        public string specialpaginationid(string subcatid)
        {
            GlobalVariables.specialpaginationid = subcatid;

            return "1";
        }
        public string bestsellerpaginationid(string subcatid)
        {
            GlobalVariables.bestsellerpaginationid = subcatid;

            return "1";
        }
        public string offerpaginationid(string subcatid)
        {
            GlobalVariables.offerpaginationid = subcatid;

            return "1";
        }
        public string changeordermodel(string id)
        {
            Session["ordermodel"] = id;
            Session["pagenumberactive"] = "1";

            return "1";
        }
        public string allpaginationid(string id)
        {
            Session["pagenumberactive"] = id;

            return "1";
        }

        public ActionResult ProductDetail(string id)
        {
            string device = RandomString();
            string code = MD5Hash(device + "ncase8934f49909");
            Session["currentpage"] = "/Home/ProductDetail?id=" + id;
            string ID = "";
            ID = id.Substring(1, id.Length - 1);
            string result = "";
            using (WebClient client = new WebClient())
            {

                var collection = new NameValueCollection();
                collection.Add("device", device);
                collection.Add("code", code);
                collection.Add("id", ID);
                

                //foreach (var myvalucollection in imaglist) {
                //    collection.Add("imaglist[]", myvalucollection);
                //}
                byte[] response =
                client.UploadValues("http://supectco.com/webs/textile/Main/handler/viewProduct.php", collection);

                result = System.Text.Encoding.UTF8.GetString(response);
            }

            scarfproject.ViewModelPost.viewProductViewModel log = JsonConvert.DeserializeObject<scarfproject.ViewModelPost.viewProductViewModel>(result);

            log.ID = ID;
            

            return View(log);
        }
        public string addtocart(string id, string price)
        {

            string str = id;
            str = str.Substring(10, str.Length - 10);



            string json2;
            using (var client = new WebClient())
            {
                json2 = client.DownloadString("http://supectco.com/webs/textile/getproductdetail.php?id=" + str);
            }
            var log = JsonConvert.DeserializeObject<earlyRoot>(json2);

            Datum productdetail = new Datum();
            if (log != null)
            {
                foreach (var myvar in log.data)
                {
                    productdetail = myvar;

                }
            }



            if (Request.Cookies["cartmodel"] != null)
            {

                ProductDetail newitem = new ViewModel.ProductDetail();
                HttpCookie productides = new HttpCookie("cartmodel");
                productides = Request.Cookies["cartmodel"];
                List<ProductDetail> data = JsonConvert.DeserializeObject<List<ProductDetail>>(productides.Value);
                int j = 0;
                foreach (var item in data)
                {
                    if (item.productid == Convert.ToInt32(str))
                    {
                        item.quantity += 1;
                        j = 1;

                    }

                }

                if (j == 0)
                {
                    newitem.productid = Convert.ToInt32(str);
                    newitem.quantity = 1;
                    newitem.name = Server.UrlEncode(productdetail.title);
                    newitem.price = Convert.ToInt32(productdetail.PriceNow);
                    newitem.baseprice = Convert.ToInt32(productdetail.productprice);
                    newitem.discount = Convert.ToInt32(productdetail.discount);
                    
                    newitem.imagethum =  productdetail.imagelist.First().title;
                    data.Add(newitem);
                    var json = new JavaScriptSerializer().Serialize(data);
                    productides.Value = json;
                    Response.Cookies.Add(productides);
                    return "1";
                }
                else
                {
                    var json = new JavaScriptSerializer().Serialize(data);
                    productides.Value = json;
                    Response.Cookies.Add(productides);
                    return "0";
                }

            }
            else
            {
                ProductDetail newitem = new ViewModel.ProductDetail();
                List<ProductDetail> data = new List<ViewModel.ProductDetail>();
                HttpCookie productides = new HttpCookie("cartmodel");

                newitem.productid = Convert.ToInt32(str);
                newitem.quantity = 1;
                newitem.name = Server.UrlEncode(productdetail.title);
                newitem.price = Convert.ToInt32(productdetail.PriceNow);
                newitem.baseprice = Convert.ToInt32(productdetail.productprice);
                newitem.discount = Convert.ToInt32(productdetail.discount);
                newitem.imagethum = productdetail.imagelist.First().title;
                data.Add(newitem);
                var json = new JavaScriptSerializer().Serialize(data);
                productides.Value = json;
                Response.Cookies.Add(productides);
                return "1";
            }


        }


        public void deletefromcart(string id)
        {

            string str = id;
            str = str.Substring(3, str.Length - 3);

            if (Request.Cookies["cartmodel"] != null)
            {

                ProductDetail newitem = new ViewModel.ProductDetail();
                HttpCookie productides = new HttpCookie("cartmodel");
                productides = Request.Cookies["cartmodel"];
                List<ProductDetail> data = JsonConvert.DeserializeObject<List<ProductDetail>>(productides.Value);
                List<ProductDetail> data2 = new List<ViewModel.ProductDetail>();

                foreach (var item in data)
                {
                    if (item.productid != Convert.ToInt32(str))
                    {
                        data2.Add(item);
                    }

                }
                var json = new JavaScriptSerializer().Serialize(data2);
                productides.Value = json;
                Response.Cookies.Add(productides);



            }
            else
            {


            }


        }

        public void emptycart()
        {
            Response.Cookies["cartmodel"].Expires = DateTime.Now.AddDays(-1);
        }

        public void minusfromcart(string id)
        {

            string str = id;
            str = str.Substring(5, str.Length - 5);

            if (Request.Cookies["cartmodel"] != null)
            {

                ProductDetail newitem = new ViewModel.ProductDetail();
                HttpCookie productides = new HttpCookie("cartmodel");
                productides = Request.Cookies["cartmodel"];
                List<ProductDetail> data = JsonConvert.DeserializeObject<List<ProductDetail>>(productides.Value);
                int j = 0;
                foreach (var item in data)
                {
                    if (item.productid == Convert.ToInt32(str))
                    {
                        if (item.quantity > 1)
                        {
                            item.quantity -= 1;
                        }


                    }

                }
                var json = new JavaScriptSerializer().Serialize(data);
                productides.Value = json;
                Response.Cookies.Add(productides);


            }
            else
            {


            }


        }
        public void addtocart2(string id)
        {

            string str = id;
            str = str.Substring(3, str.Length - 3);


            ProductDetail newitem = new ViewModel.ProductDetail();
            HttpCookie productides = new HttpCookie("cartmodel");
            productides = Request.Cookies["cartmodel"];
            List<ProductDetail> data = JsonConvert.DeserializeObject<List<ProductDetail>>(productides.Value);

            foreach (var item in data)
            {
                if (item.productid == Convert.ToInt32(str))
                {
                    item.quantity += 1;

                }

            }
            var json = new JavaScriptSerializer().Serialize(data);
            productides.Value = json;
            Response.Cookies.Add(productides);






        }

        public string gogetfinalprice()
        {

            if (Request.Cookies["cartmodel"] != null)
            {




                HttpCookie productides = new HttpCookie("cartmodel");
                productides = Request.Cookies["cartmodel"];
                List<ProductDetail> data = JsonConvert.DeserializeObject<List<ProductDetail>>(productides.Value);
                int j = 0;
                foreach (var item in data)
                {
                    j += (int)(item.price * item.quantity);


                }
                return j.ToString();
            }
            else
            {
                return "0";
            }


        }

        public string gogetfinalquantity()
        {

            if (Request.Cookies["cartmodel"] != null)
            {
                HttpCookie productides = new HttpCookie("cartmodel");
                productides = Request.Cookies["cartmodel"];
                List<ProductDetail> data = JsonConvert.DeserializeObject<List<ProductDetail>>(productides.Value);
                int j = 0;
                foreach (var item in data)
                {
                    j += item.quantity;


                }
                return j.ToString();
            }
            else
            {
                return "0";
            }


        }

     

        public ActionResult checkout()
        {
            Session["currentpage"] = "/Home/checkout";


            List<CheckoutViewModel> finalmodel = new List<CheckoutViewModel>();


            if (Request.Cookies["cartmodel"] != null)
            {




                HttpCookie productides = new HttpCookie("cartmodel");
                productides = Request.Cookies["cartmodel"];
                List<ProductDetail> data = JsonConvert.DeserializeObject<List<ProductDetail>>(productides.Value);

                foreach (var item in data)
                {
                    CheckoutViewModel j = new CheckoutViewModel();
                    j.price = item.price;
                    j.baseprice = item.baseprice;
                    j.discount = item.discount;
                    j.productid = item.productid;
                    j.productname = Server.UrlDecode(item.name);
                    j.quantity = item.quantity;
                    j.imageaddress = item.imagethum;
                    finalmodel.Add(j);
                }

            }





            //ViewBag.Message = "Your contact page.";

            return View(finalmodel);
        }
        public PartialViewResult checkoutsummery()
        {
            Session["currentpage"] = "/Home/Index";


            List<CheckoutViewModel> finalmodel = new List<CheckoutViewModel>();


            if (Request.Cookies["cartmodel"] != null)
            {




                HttpCookie productides = new HttpCookie("cartmodel");
                productides = Request.Cookies["cartmodel"];
                List<ProductDetail> data = JsonConvert.DeserializeObject<List<ProductDetail>>(productides.Value);

                foreach (var item in data)
                {
                    CheckoutViewModel j = new CheckoutViewModel();
                    j.price = item.price;
                    j.baseprice = item.baseprice;
                    j.discount = item.discount;
                    j.productid = item.productid;
                    j.productname = Server.UrlDecode(item.name);
                    j.quantity = item.quantity;
                    j.imageaddress = item.imagethum;
                    finalmodel.Add(j);
                }

            }





            //ViewBag.Message = "Your contact page.";

            return PartialView("/Views/Shared/_cartSummery.cshtml", finalmodel);
        }

        public PartialViewResult getothercoitem(string id)
        {
            string ID = id.Substring(1, id.Length - 1);
            string json;
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://supectco.com/webs/textile/getothercoitem.php?id=" + ID);
            }
         
            var log = JsonConvert.DeserializeObject<earlyRoot>(json);
            List<Datum> mylist = new List<Datum>();
            Datum newearlydatum = new Datum();
            if (log.data != null)
            {
                foreach (var myvar in log.data)
                {
                    newearlydatum = myvar;

                    mylist.Add(newearlydatum);
                }
            }

            return PartialView("/Views/Shared/_othercoitem.cshtml", mylist);
        }
        public string Khabaramkon(string id)
        {
            string json;
            if (Session["LogedInUser"] == null)
            {
                return "0";
            }
            userdata user = Session["LogedInUser"] as userdata;
            if (user.email == "")
            {
                using (var client = new WebClient())
                {
                    json = client.DownloadString("http://supectco.com/webs/textile/khabaramkon.php?id=" + id + "&phone=" + user.phone);
                }
            }

            else
            {
                using (var client = new WebClient())
                {
                    json = client.DownloadString("http://supectco.com/webs/textile/khabaramkon.php?id=" + id + "&email=" + user.email);
                }
            }

            var log = JsonConvert.DeserializeObject<string>(json);
            if (log == "1")
            {
                return "1";
            }
            else if (log == "2")
            {
                return "2";
            }
            else
            {
                return "3";
            }


        }

        public PartialViewResult gogetloginpart()
        {

            if (Session["LogedInUser"] != null)
            {

                userdata user = Session["LogedInUser"] as userdata;
                return PartialView("/Views/Shared/_loginpartin.cshtml", user);
            }

            else
            {
                HttpCookie Username = new HttpCookie("Username");
                BaseViewModel basemodel = new BaseViewModel();
                Username = Request.Cookies["Username"];
                HttpCookie Password = new HttpCookie("Password");
                Password = Request.Cookies["Password"];
                if (Username != null)
                {
                    basemodel.username = Username.Value;
                }
                if (Password != null)
                {
                    basemodel.pass = Password.Value;
                }
                return PartialView("/Views/Shared/_loginpart.cshtml", basemodel);
            }

        }
      
       
      
        public string IsLogedIn()
        {

            if (Session["LogedInUser"] != null)
            {
                return "1";

            }
            else
            {
                return "0";
            }

        }

        public ActionResult EndOrder()
        {
            ViewBag.Message = "Your contact page.";


            return View();
        }
        public PartialViewResult userdatapanel()
        {
            userdata user = Session["LogedInUser"] as userdata;
            return PartialView("/Views/Shared/_UserDataPanel.cshtml", user);

        }
        public PartialViewResult shopsummery()
        {
            List<string> finalmodel = new List<string>();
            HttpCookie productides = new HttpCookie("cartmodel");
            productides = Request.Cookies["cartmodel"];
            List<ProductDetail> data = JsonConvert.DeserializeObject<List<ProductDetail>>(productides.Value);

            foreach (var item in data)
            {
                string j = "";
                j = item.imagethum;
                finalmodel.Add(j);
            }
            return PartialView("/Views/Shared/_shopsummery.cshtml", finalmodel);

        }
        public PartialViewResult ReqestForPayment()
        {
            List<string> finalmodel = new List<string>();
            HttpCookie productides = new HttpCookie("cartmodel");
            productides = Request.Cookies["cartmodel"];
            List<ProductDetail> data = JsonConvert.DeserializeObject<List<ProductDetail>>(productides.Value);
            decimal finalprice = 0;
            foreach (var item in data)
            {
                finalprice += (item.price) * item.quantity;
            }
            userdata user = Session["LogedInUser"] as userdata;
            string id = user.ID;
            string json = "";
            string txtDescription = "پرداخت کاربر شماره" + id;
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://supectco.com/webs/textile/setpurchaserecord.php?totalprice=" + finalprice + "&userid=" + id);
            }

            List<timestamp> log = JsonConvert.DeserializeObject<List<timestamp>>(json);

            System.Net.ServicePointManager.Expect100Continue = false;
            scarfproject.ServiceReference1.PaymentGatewayImplementationServicePortTypeClient zp = new scarfproject.ServiceReference1.PaymentGatewayImplementationServicePortTypeClient();
            string Authority;

            int Status = zp.PaymentRequest("YOUR-ZARINPAL-MERCHANT-CODE", int.Parse(finalprice.ToString()), txtDescription, "info@charghadshop.com", "", "http://www.charghadshop.com/Home/Verify", out Authority);

            if (Status == 100)
            {
                using (var client = new WebClient())
                {
                    json = client.DownloadString("http://supectco.com/webs/textile/setAUTcode.php?auth=" + Authority + "&userid=" + id + "&timestamp=" + log[0].TimeStamp);
                }

                var log2 = JsonConvert.DeserializeObject<string>(json);
                Response.Redirect("https://www.zarinpal.com/pg/StartPay/" + Authority);
            }
            else
            {
                Response.Write("error: " + Status);
            }
            // setpurchaserecord
            return PartialView("/Views/Shared/_shopsummery.cshtml", finalmodel);

        }
        public ActionResult Profile()
        {
            if (Session["LogedInUser"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.name = GlobalVariables.productnameinadminpanelforeditdetail;
            ViewBag.message = "";
            return View();
        }
        public PartialViewResult setnewproduct()
        {
            productviewmodel MODEL = new productviewmodel();
            MODEL.name = GlobalVariables.productnameforproductadmin;
            MODEL.description = GlobalVariables.productdescforproductadmin;
            MODEL.price = GlobalVariables.productpriceforproductadmin;
            MODEL.setid = GlobalVariables.productsetidforproductadmin;
            MODEL.discount = GlobalVariables.productdiscountforproductadmin;
            MODEL.color = GlobalVariables.productcolorforproductadmin;

            return PartialView("/Views/Shared/_setnewproduct.cshtml", MODEL);

        }
        [HttpPost]
        public ActionResult Profile(productinfoviewdetail detail)
        {
            if (Session["LogedInUser"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            productviewmodel model = new productviewmodel();


            GlobalVariables.productnameforproductadmin = detail.productname;
            GlobalVariables.productcolorforproductadmin = detail.productcolor;
            GlobalVariables.productpriceforproductadmin = detail.productprice;
            GlobalVariables.productsetidforproductadmin = detail.productsetid;
            GlobalVariables.productdescforproductadmin = detail.productdesc;
            GlobalVariables.productdiscountforproductadmin = detail.productdiscount;



            string pathString = "~/images/sharghimages";
            if (!Directory.Exists(Server.MapPath(pathString)))
            {
                DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(pathString));
            }




            try
            {
                List<string> imagelist = new List<string>();

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase hpf = Request.Files[i];
                    imagelist.Add(hpf.FileName);
                    if (hpf.ContentLength == 0)
                        continue;
                    string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(hpf.FileName));
                    hpf.SaveAs(savedFileName); // Save the file

                    //   کدهای مربوط به  اف تی پی کردن فایل و همچنین تغییر عکس و ذخیره آن

                    //using (WebClient client = new WebClient())
                    //{
                    //    string ftpUsername = @"meri@supectco.com";
                    //    string ftpPassword = @"!)lAx3_-h43s";
                    //    client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                    //    client.UploadFile("ftp://www.supectco.com/public_html/webs/textile/api/portal/uploads/" + hpf.FileName, "STOR", savedFileName);
                    //}

                    //if (i == 0) {
                    int width = 250;
                    int height = 278;
                    Image image = Image.FromFile(savedFileName);
                    var destRect = new Rectangle(0, 0, width, height);
                    var destImage = new Bitmap(width, height);

                    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                    using (var graphics = Graphics.FromImage(destImage))
                    {
                        graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                        using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                        {
                            wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                        }
                    }
                    string thumsavedFileName = Path.Combine(Server.MapPath(pathString), "thum" + Path.GetFileName(hpf.FileName));
                    destImage.Save(thumsavedFileName, System.Drawing.Imaging.ImageFormat.Jpeg);

                    using (WebClient client = new WebClient())
                    {
                        string ftpUsername = @"meri@supectco.com";
                        string ftpPassword = @"!)lAx3_-h43s";
                        client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                        client.UploadFile("ftp://www.supectco.com/public_html/webs/textile/api/portal/uploads/thum" + hpf.FileName, "STOR", thumsavedFileName);
                    }
                    destImage.Dispose();
                    image.Dispose();
                    //}


                }

                string json;
                string title = detail.productname;
                string parentid = "1";
                string setid = detail.productsetid;
                string price = detail.productprice;
                string discount = detail.productdiscount;
                string color = detail.productcolor;
                string image1 = "";
                string image2 = "";
                string image3 = "";
                string image4 = "";
                string image5 = "";
                string imagethum = "";


                int count = imagelist.Count();
                int counter = 0;
                if (counter < count)
                {
                    image1 = imagelist[0];
                    imagethum = "thum" + imagelist[0];
                    counter += 1;
                }

                if (counter < count)
                {
                    image2 = imagelist[1];
                    counter += 1;
                }
                if (counter < count)
                {
                    image3 = imagelist[2];
                    counter += 1;
                }
                if (counter < count)
                {
                    image4 = imagelist[3];
                    counter += 1;
                }
                if (counter < count)
                {
                    image5 = imagelist[4];
                    counter += 1;
                }

                string desc = detail.productdesc;

                userdata USER = Session["LogedInUser"] as userdata;
                string MezonId = USER.ID;
                using (var client = new WebClient())
                {
                    //json = client.DownloadString("http://supectco.com/webs/textile/addProduct.php?title=" + title + "&parentid=" + parentid + "&setid=" + setid + "&price=" + price + "&image1=" + image1 + "&image2=" + image2 + "&image3=" + image3 + "&image4=" + image4 + "&image5=" + image5 + "&desc=" + desc + "&mezonid=" + MezonId);
                    json = client.DownloadString("http://supectco.com/webs/textile/addProduct.php?title=" + title + "&parentid=" + parentid + "&setid=" + setid + "&price=" + price + "&imagethum=" + imagethum + "&image1=" + image1 + "&image2=" + image2 + "&image3=" + image3 + "&image4=" + image4 + "&image5=" + image5 + "&desc=" + desc + "&mezonid=" + MezonId + "&discount=" + discount + "&color=" + color);
                }

                ViewBag.message = "محصول مورد نظر اضافه شد";
                return View();

            }
            catch (WebException exception)
            {

                string responseText;
                var responseStream = exception.Response.GetResponseStream();

                // var responseStream = exception.Response?"":.GetResponseStream();

                if (responseStream != null)
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseText = reader.ReadToEnd();
                    }
                }
                throw exception.InnerException;
            }


        }
        public ActionResult Autocomplete(string term)
        {
            string json2;
            using (var client = new WebClient())
            {
                json2 = client.DownloadString("http://supectco.com/webs/textile/getlistofproductname.php");
            }
            var log = JsonConvert.DeserializeObject<List<listofstring>>(json2);

            List<string> items = new List<string>();
            foreach (var item in log)
            {
                items.Add(item.title);
            }

            var filteredItems = items.Where(
         item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0
         );
            return Json(filteredItems, JsonRequestBehavior.AllowGet);



        }
        //public PartialViewResult getproductdetail(string name)
        //{

        //    //var finalname = Server.UrlEncode(name);
        //    string json;
        //    using (var client = new WebClient())
        //    {
        //        json = client.DownloadString("http://supectco.com/webs/textile/getproductdetail.php?name=" + name);
        //    }
        //    var log = JsonConvert.DeserializeObject<earlyRoot>(json);
        //    List<earlydatumfinal> finalmodel = new List<earlydatumfinal>();

        //    foreach (var item in log.data)
        //    {

        //        Datum productdetail = new Datum();
        //        earlydatumfinal model = new earlydatumfinal();
        //        productdetail = item;
        //        model.color = productdetail.color;
        //        model.description = productdetail.description;
        //        model.ID = productdetail.ID;
        //        model.productprice = productdetail.productprice;
        //        model.SetId = productdetail.SetId;
        //        model.title = productdetail.title;
        //        model.PriceNow = productdetail.PriceNow;
        //        model.discount = productdetail.discount;
        //        model.productprice = productdetail.productprice;
        //        List<string> imagelist = new List<string>();

        //        if (productdetail.image1 != "")
        //        {
        //            imagelist.Add("//supectco.com/webs/textile/api/portal/uploads/" + productdetail.image1);
        //            imagelist.Add("//supectco.com/webs/textile/api/portal/uploads/" + productdetail.imagethum);
        //        }
        //        if (productdetail.image2 != "")
        //        {
        //            imagelist.Add("//supectco.com/webs/textile/api/portal/uploads/" + productdetail.image2);
        //        }
        //        if (productdetail.image3 != "")
        //        {
        //            imagelist.Add("//supectco.com/webs/textile/api/portal/uploads/" + productdetail.image3);
        //        }
        //        if (productdetail.image4 != "")
        //        {
        //            imagelist.Add("//supectco.com/webs/textile/api/portal/uploads/" + productdetail.image4);
        //        }
        //        if (productdetail.image5 != "")
        //        {
        //            imagelist.Add("//supectco.com/webs/textile/api/portal/uploads/" + productdetail.image5);
        //        }

        //        model.imagelist = imagelist;
        //        finalmodel.Add(model);

        //    }



        //    return PartialView("/Views/Shared/_showimagsummery.cshtml", finalmodel);

        //}
        public PartialViewResult OrdersList()
        {

            //var finalname = Server.UrlEncode(name);
            string json;
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://supectco.com/webs/textile/getorderlist.php");
            }
            var log = JsonConvert.DeserializeObject<RootObjectoforderlist>(json);
            List<orderlistDatum> finalmodel = new List<orderlistDatum>();
            if (log.data != null)
            {
                foreach (var item in log.data)
                {
                    orderlistDatum orderdetail = new orderlistDatum();
                    orderdetail = item;
                    finalmodel.Add(orderdetail);
                }
            }




            return PartialView("/Views/Shared/_orderlist.cshtml", finalmodel);

        }
        public ActionResult updateproductdetail(productinfoviewdetail detail)
        {
            GlobalVariables.productnameinadminpanelforeditdetail = detail.productname;
            string pathString = "~/images/sharghimages";
            if (!Directory.Exists(Server.MapPath(pathString)))
            {
                DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(pathString));
            }

            List<string> imagelist = new List<string>();

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase hpf = Request.Files[i];
                imagelist.Add(hpf.FileName);
                if (hpf.ContentLength == 0)
                    continue;
                string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(hpf.FileName));
                hpf.SaveAs(savedFileName); // Save the file

                using (WebClient client = new WebClient())
                {
                    string ftpUsername = @"meri@supectco.com";
                    string ftpPassword = @"!)lAx3_-h43s";
                    client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                    client.UploadFile("ftp://www.supectco.com/public_html/webs/textile/api/portal/uploads/" + hpf.FileName, "STOR", savedFileName);
                }

                if (i == 0)
                {
                    int width = 250;
                    int height = 278;
                    Image image = Image.FromFile(savedFileName);
                    var destRect = new Rectangle(0, 0, width, height);
                    var destImage = new Bitmap(width, height);

                    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                    using (var graphics = Graphics.FromImage(destImage))
                    {
                        graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                        using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                        {
                            wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                        }
                    }
                    string thumsavedFileName = Path.Combine(Server.MapPath(pathString), "thum" + Path.GetFileName(hpf.FileName));
                    destImage.Save(thumsavedFileName, System.Drawing.Imaging.ImageFormat.Jpeg);

                    using (WebClient client = new WebClient())
                    {
                        string ftpUsername = @"meri@supectco.com";
                        string ftpPassword = @"!)lAx3_-h43s";
                        client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                        client.UploadFile("ftp://www.supectco.com/public_html/webs/textile/api/portal/uploads/thum" + hpf.FileName, "STOR", thumsavedFileName);
                    }
                    destImage.Dispose();
                    image.Dispose();
                }


            }



            string image1 = "";
            string image2 = "";
            string image3 = "";
            string image4 = "";
            string image5 = "";
            string imagethum = "";


            int count = imagelist.Count();
            int counter = 0;
            if (counter < count)
            {
                image1 = imagelist[0];
                imagethum = "thum" + imagelist[0];
                counter += 1;
            }

            if (counter < count)
            {
                image2 = imagelist[1];
                counter += 1;
            }
            if (counter < count)
            {
                image3 = imagelist[2];
                counter += 1;
            }
            if (counter < count)
            {
                image4 = imagelist[3];
                counter += 1;
            }
            if (counter < count)
            {
                image5 = imagelist[4];
                counter += 1;
            }



            {
                string email = Session["email"] as string;
                string json;
                using (var client = new WebClient())
                {
                    json = client.DownloadString("http://supectco.com/webs/textile/updateproductdetail.php?name=" + detail.productname + "&price=" + detail.productprice + "&setid=" + detail.productsetid + "&desc=" + detail.productdesc + "&ID=" + detail.productid + "&image1=" + image1 + "&image2=" + image2 + "&image3=" + image3 + "&image4=" + image4 + "&image5=" + image5 + "&imagethum=" + imagethum + "&discount=" + detail.productdiscount + "&color=" + detail.productcolor);
                    // json = client.DownloadString("http://supectco.com/webs/textile/updateproductdetail.php?name=" + detail.productname + "&price=" + detail.productprice + "&setid=" + detail.productsetid + "&desc=" + detail.productdesc + "&ID=" + detail.productid + "&image1=" + image1 + "&image2=" + image2 + "&image3=" + image3 + "&image4=" + image4 + "&image5=" + image5 );
                }
                var log = JsonConvert.DeserializeObject<string>(json);
                return RedirectToAction("Profile");

            }
        }
        public ViewResult ContactUs()
        {



            return View();

        }
        public ViewResult OrderListDetail(string id)
        {
            orderListDetailViewModel model = new orderListDetailViewModel();
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://supectco.com/webs/textile/getorderlistofproduct.php?id=" + id);
            }

            var log = JsonConvert.DeserializeObject<orderdetailforsendlist>(json);
            List<orderdetailforsend> mylist = new List<orderdetailforsend>();
            //getmemyearlydataViewModel model = new getmemyearlydataViewModel();
            orderdetailforsend newearlydatum = new orderdetailforsend();
            if (log.data != null)
            {
                foreach (var myvar in log.data)
                {
                    newearlydatum = myvar;
                    newearlydatum.imagethum = "//supectco.com/webs/textile/api/portal/uploads/" + newearlydatum.imagethum;
                    mylist.Add(newearlydatum);

                }
            }
            model.TotalPrice = log.data.First().TotalPrice;

            var json2 = "";
            userdata user = new userdata();
            using (var client2 = new WebClient())
            {
                json2 = client2.DownloadString("http://supectco.com/webs/textile/getuserinfo.php?id=" + id);
            }
            var log2 = JsonConvert.DeserializeObject<userdatalist>(json2);
            if (log2 != null)
            {
                user = log2.data[0];
            }

            model.listofproduct = mylist;
            model.userinfo = user;
            model.orderid = id;
            return View(model);
        }
        public void deliverydone(string id)
        {

            string json = "";

            using (var client = new WebClient())
            {
                json = client.DownloadString("http://supectco.com/webs/textile/setIsDeliverOfPurchase.php?id=" + id);
            }

            var log2 = JsonConvert.DeserializeObject<string>(json);

        }

        public PartialViewResult getmenue()
        {
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://supectco.com/webs/textile/getcatlist.php");
            }

            var log = JsonConvert.DeserializeObject<catdetaillist>(json);
            List<catdetail> catlist = new List<catdetail>();
            //getmemyearlydataViewModel model = new getmemyearlydataViewModel();
            catdetail newearlydatum = new catdetail();
            if (log.data != null)
            {
                foreach (var myvar in log.data)
                {
                    catlist.Add(myvar);
                }
            }

            using (var client = new WebClient())
            {
                json = client.DownloadString("http://supectco.com/webs/textile/getsubcatlistwhole.php");
            }

            var log2 = JsonConvert.DeserializeObject<subcatdetaillist>(json);
            List<subcatdetail> subcatlist = new List<subcatdetail>();
            //getmemyearlydataViewModel model = new getmemyearlydataViewModel();
            subcatdetail subcatdetail = new subcatdetail();
            if (log2.data != null)
            {
                foreach (var subcat in log2.data)
                {
                    subcatlist.Add(subcat);
                }
            }

            using (var client = new WebClient())
            {
                json = client.DownloadString("http://supectco.com/webs/textile/getsubcatlist2whole.php");
            }

            var log3 = JsonConvert.DeserializeObject<subcatdetail2list>(json);
            List<subcatdetail2> subcat2list = new List<subcatdetail2>();
            //getmemyearlydataViewModel model = new getmemyearlydataViewModel();
            subcatdetail2 subcatdetail2 = new subcatdetail2();
            if (log3.data != null)
            {
                foreach (var subcat2 in log3.data)
                {
                    subcat2list.Add(subcat2);
                }
            }


            MenuViewModel model = new MenuViewModel();
            model.catlist = catlist;
            model.subcatlist = subcatlist;
            model.subcatlist2 = subcat2list;




            return PartialView("/Views/Shared/_Menu.cshtml", model);
        }

        public ActionResult appslider(string filename)
        {
            string pathString = "~/images/panelimages";
            string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(filename));

            var img = new WebImage(savedFileName)
                .Resize(820, 716, false, true);
            return new ImageResult(new MemoryStream(img.GetBytes()), "binary/octet-stream");
        }
        public ActionResult slider(string filename)
        {
            string pathString = "~/images/panelimages";
            string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(filename));
            var img = new WebImage("~/images/panelimages/placeholder.jpg")
               .Resize(1000, 665, false, true);
            if (System.IO.File.Exists(savedFileName))
            {
                img = new WebImage(savedFileName)
               .Resize(1000, 665, false, true);
            }


            return new ImageResult(new MemoryStream(img.GetBytes()), "binary/octet-stream");
        }
        public ActionResult Thumbnail(string filename)
        {
            string pathString = "~/images/panelimages";
            string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(filename));

            var img = new WebImage("~/images/panelimages/placeholder.jpg")
               .Resize(220, 220, false, true);
            if (System.IO.File.Exists(savedFileName))
            {
                img = new WebImage(savedFileName)
               .Resize(291, 285, false, true);
            }
            return new ImageResult(new MemoryStream(img.GetBytes()), "binary/octet-stream");
        }
        public ActionResult colorShow(string filename)
        {
            string pathString = "~/images/panelimages";
            string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(filename));

            var img = new WebImage("~/images/panelimages/placeholder.jpg")
               .Resize(30, 30, false, true);
            if (System.IO.File.Exists(savedFileName))
            {
                img = new WebImage(savedFileName)
               .Resize(30, 30, false, true);
            }
            return new ImageResult(new MemoryStream(img.GetBytes()), "binary/octet-stream");
        }
        public ActionResult DetailImage(string filename)
        {
            string pathString = "~/images/panelimages";
            string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(filename));

            if (!System.IO.File.Exists(savedFileName))
            {
                savedFileName = "E:\\ALLPROJECT\\WEBSITES\\SCARF\\textileprojectFINAL\\scarfproject\\images\\placeholder.png";
            }
            var img = new WebImage(savedFileName);
            return new ImageResult(new MemoryStream(img.GetBytes()), "binary/octet-stream");
        }

        public class ImageResult : ActionResult
        {
            public ImageResult(Stream imageStream, string contentType)
            {
                if (imageStream == null)

                    throw new ArgumentNullException("imageStream");

                if (contentType == null)

                    throw new ArgumentNullException("contentType");
                this.ImageStream = imageStream;

                this.ContentType = contentType;

            }
            public Stream ImageStream { get; private set; }
            public string ContentType { get; private set; }
            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                HttpResponseBase response = context.HttpContext.Response;
                response.ContentType = this.ContentType;
                byte[] buffer = new byte[4096];
                while (true)
                {
                    int read = this.ImageStream.Read(buffer, 0, buffer.Length);
                    if (read == 0)
                        break;
                    response.OutputStream.Write(buffer, 0, read);
                }
                response.End();
            }

           
        }

        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
    }
}
