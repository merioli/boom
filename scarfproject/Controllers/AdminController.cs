using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using AdminPanel.ViewModel;
using AdminPanel.Classes;
using PagedList;
using System.Web.Helpers;
using System.Drawing;
using System.Xml.Serialization;
using System.Web.Script.Serialization;


namespace scarfproject.Controllers
{
    public class AdminController : Controller
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string serializeproductmodel(productinfoviewdetail model)
        {
            try
            {
                productinfoviewdetail aPerson = model;      // The Person object which we will serialize
                string serializedData = string.Empty;                   // The string variable that will hold the serialized data

                XmlSerializer serializer = new XmlSerializer(aPerson.GetType());
                serializer.Serialize(Console.Out, aPerson);
                using (StringWriter sw = new StringWriter())
                {
                    serializer.Serialize(sw, aPerson);
                    serializedData = sw.ToString();
                }
                return serializedData;
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public productinfoviewdetail deserializestringtomodel(string srt)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(productinfoviewdetail));
            productinfoviewdetail deserializedPerson = new productinfoviewdetail();
            using (TextReader tr = new StringReader(srt))
            {
                deserializedPerson = (productinfoviewdetail)deserializer.Deserialize(tr);
            }
            return deserializedPerson;
        }
        public ActionResult CustomerLogin(string pass, string ischecked, string phone)
        {

            string json;
            using (var client2 = new WebClient())
            {
                json = client2.DownloadString("http://supectco.com/webs/textile/Admin/getuserid.php?pass=" + pass + "&mobile=" + phone);
            }
            var log = JsonConvert.DeserializeObject<List<userdata>>(json);
            if (log != null)
            {
                userdata user = log[0];
                if (user.ID != "2" && user.ID != "3")
                {
                    Session["LogedInUser2"] = user;
                    if (Request.Cookies["productcookiie"] != null)
                    {
                        HttpCookie currentUserCookie = Request.Cookies["productcookiie"];
                        Response.Cookies.Remove("productcookiie");
                        currentUserCookie.Expires = DateTime.Now.AddDays(-10);
                        currentUserCookie.Value = null;
                        Response.SetCookie(currentUserCookie);
                    }




                    if (ischecked == "checked")
                    {
                        HttpCookie Username = new HttpCookie("Username");
                        HttpCookie Password = new HttpCookie("Password");
                        DateTime now = DateTime.Now;
                        Username.Value = phone;
                        Username.Expires = now.AddMonths(1);
                        Password.Value = pass;
                        Password.Expires = now.AddMonths(1);
                        Response.Cookies.Add(Username);
                        Response.Cookies.Add(Password);
                    }

                }
                return Content("1/Admin/product");
            }
            else
            {
                return Content("2/Admin/product");
            }



        }
        public ActionResult Index()
        {
            BaseViewModel basemodel = new BaseViewModel();
            if (Session["LogedInUser2"] != null)
            {

                userdata user = Session["LogedInUser2"] as userdata;

            }

            else
            {
                HttpCookie Username = new HttpCookie("Username");

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

            }
            return View(basemodel);
        }
        public static IEnumerable<SelectListItem> GetProvincesList()
        {
            IList<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Text = "California", Value = "B"},
                new SelectListItem{Text = "Alaska", Value = "B"},
                new SelectListItem{Text = "Illinois", Value = "B"},
                new SelectListItem{Text = "Texas", Value = "B"},
                new SelectListItem{Text = "Washington", Value = "B"}

            };
            return items;
        }
        public ActionResult gotoindex()
        {
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult productdetail()
        {


            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            userdata user = Session["LogedInUser2"] as userdata;
            string json = "";
            using (var client = new WebClient())
            {

                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getcatslist.php?ID=" + user.ID);

            }
            var log = JsonConvert.DeserializeObject<catslist>(json);
            List<catsdetail> mylist = new List<catsdetail>();
            catsdetail newearlydatum = new catsdetail();
            if (log.data != null)
            {
                foreach (var myvar in log.data)
                {
                    mylist.Add(myvar);
                }
            }
            choosecat mymodel = new choosecat()
            {
                Cats = new SelectList(mylist, "ID", "title"),
               

            };


            return View(mymodel);



        }
        public ActionResult Menu()
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            // CatPageViewModel model = new CatPageViewModel();
            userdata user = Session["LogedInUser2"] as userdata;
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getcatslist.php?ID=" + user.ID);

            }

            var log = JsonConvert.DeserializeObject<catslist>(json);
            List<catsdetail> mylist = new List<catsdetail>();
            //getmemyearlydataViewModel model = new getmemyearlydataViewModel();
            catsdetail newearlydatum = new catsdetail();
            if (log.data != null)
            {
                foreach (var myvar in log.data)
                {
                    mylist.Add(myvar);
                }
            }

            CatPageViewModel model = new CatPageViewModel()
            {
                Cats = new SelectList(mylist, "ID", "title")
                // SelectedModel = ? if you want to preselect an item
            };
            return View(model);
        }
        public ActionResult getfilters(string catID)
        {
            if (catID != null)
            {
                GlobalVariables.catidforfilter = catID;

                string json1 = "";
                string json3 = "";


                using (var client = new WebClient())
                {

                    //filters = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getfilterlistfordel.php?catID=" + catID);
                    json1 = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getfilterslist.php?catID=" + catID);
                    //json2 = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getbrandslist.php?catID=" + catID);
                    json3 = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getcoloreslist.php?catID=" + catID);
                }

                //var log = JsonConvert.DeserializeObject<catslist>(filters);
                FilterList log1 = JsonConvert.DeserializeObject<FilterList>(json1);
                var log3 = JsonConvert.DeserializeObject<coloreslist>(json3);
                List<colordetail> colorList = new List<colordetail>();
                colordetail newearlydatum3 = new colordetail();
                if (log3.data != null)
                {
                    foreach (var myvar3 in log3.data)
                    {
                        colorList.Add(myvar3);
                    }
                }


                productDetailPageViewModel model = new productDetailPageViewModel()
                {
                    //Filtersfordel = new SelectList(mylist, "ID", "title"),
                    Colores = new SelectList(colorList, "code", "title"),
                    filterlist = log1
                    // SelectedModel = ? if you want to preselect an item
                };
                return PartialView("/Views/Shared/AdminShared/_filterHolder.cshtml", model);
            }
            else
            {
                productDetailPageViewModel model = new productDetailPageViewModel();
                return PartialView("/Views/Shared/AdminShared/_filterHolder.cshtml", model);
            }


        }
        public ActionResult getfiltersfordel(string catID)
        {
            if (catID != null)
            {
                GlobalVariables.catidforfilter = catID;

                string json1 = "";
               


                using (var client = new WebClient())
                {

                    json1 = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getfilterlistfordel.php?catID=" + catID);
                }

                var log = JsonConvert.DeserializeObject<filtereslist>(json1);
                List<filterdetail> filterlist = new List<filterdetail>();
                filterdetail newearlydatum3 = new filterdetail();
                if (log.data != null)
                {
                    foreach (var myvar3 in log.data)
                    {
                        filterlist.Add(myvar3);
                    }
                }


                FilterfordelViewModel model = new FilterfordelViewModel()
                {
                    Filtersfordel = new SelectList(filterlist, "ID", "filtername"),
                 
                    // SelectedModel = ? if you want to preselect an item
                };
                return PartialView("/Views/Shared/AdminShared/_filterListForDelHolder.cshtml", model);
            }
            else
            {
                FilterfordelViewModel model = new FilterfordelViewModel();
                return PartialView("/Views/Shared/AdminShared/_filterListForDelHolder.cshtml", model);
            }


        }
        public ActionResult addNewFilter(string name)
        {
            if (name != null)
            {
                string catID = GlobalVariables.catidforfilter;

                string json = "";
               


                using (var client = new WebClient())
                {

                    json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/addNewFilter.php?catID=" + catID + "&filterName=" + name);
                  
                }


                if (json.Contains("success"))
                {
                    return Content("success");
                }
                else if (json.Contains("exist"))
	            {
                    return Content("exist");
	            }
                else
                {
                    return Content("error");
                }
               
            }
            else
            {
                return Content("error");
            }


        }

        public ActionResult delFilter(string name)
        {
            if (Session["LogedInUser2"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            string catID = GlobalVariables.catidforfilter;
            string json = "";

            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/deletefilter.php?id=" + name);
            }

            if (json.Contains("success"))
            {
                return Content("success");
            }

            else
            {
                return Content("error");
            }
        }

        //public ActionResult getsubcatlistandmoreforchange(string catid)
        //{
        //    if (Session["LogedInUser2"] == null)
        //    {

        //        return RedirectToAction("Index", "Admin");

        //    }
        //    if (catid == "")
        //    {
        //        return PartialView("/Views/Shared/AdminShared/_SubCatVoid.cshtml");
        //    }
        //    else
        //    {
        //        GlobalVariables.catidfororderlist = catid;

        //        string json = "";
        //        using (var client = new WebClient())
        //        {
        //            json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getsubcatslist.php?id=" + catid);
        //        }

        //        var log = JsonConvert.DeserializeObject<catslist>(json);
        //        List<catsdetail> mylist = new List<catsdetail>();
        //        //getmemyearlydataViewModel model = new getmemyearlydataViewModel();
        //        catsdetail newearlydatum = new catsdetail();
        //        if (log.data != null)
        //        {
        //            foreach (var myvar in log.data)
        //            {
        //                mylist.Add(myvar);
        //            }
        //        }

        //        CatPageViewModel model = new CatPageViewModel()
        //        {
        //            Cats = new SelectList(mylist, "ID", "title")
        //            // SelectedModel = ? if you want to preselect an item
        //        };
        //        return PartialView("/Views/Shared/AdminShared/_SubCatListAndMoresubcat2.cshtml", model);
        //    }

        //}
        //public ActionResult getsubcatlistandmore(string catid)
        //{
        //    if (Session["LogedInUser2"] == null)
        //    {

        //        return RedirectToAction("Index", "Admin");

        //    }
        //    if (catid == "")
        //    {
        //        return PartialView("/Views/Shared/AdminShared/_SubCatVoid.cshtml");
        //    }
        //    else
        //    {
        //        GlobalVariables.catidfororderlist = catid;
        //        GlobalVariables.catidfordef = catid;

        //        string json = "";
        //        using (var client = new WebClient())
        //        {
        //            json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getsubcatslist.php?id=" + catid);
        //        }

        //        var log = JsonConvert.DeserializeObject<catslist>(json);
        //        List<catsdetail> mylist = new List<catsdetail>();
        //        //getmemyearlydataViewModel model = new getmemyearlydataViewModel();
        //        catsdetail newearlydatum = new catsdetail();
        //        if (log.data != null)
        //        {
        //            foreach (var myvar in log.data)
        //            {
        //                mylist.Add(myvar);
        //            }
        //        }

        //        CatPageViewModel model = new CatPageViewModel()
        //        {
        //            Cats = new SelectList(mylist, "ID", "title")
        //            // SelectedModel = ? if you want to preselect an item
        //        };
        //        return PartialView("/Views/Shared/AdminShared/_SubCatListAndMore.cshtml", model);
        //    }

        //}
        public ActionResult getsubcatlist(string catid, string levelfinder)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            if (catid == "")
            {
                return PartialView("/Views/Shared/AdminShared/_SubCatVoid.cshtml");
            }
            else
            {
                switch (levelfinder) {
                    case "list":
                        GlobalVariables.catid = catid;
                        break;
                    case "del" :
                        GlobalVariables.catidfordel = catid;
                        break;
                        case "def" :
                        GlobalVariables.catidfordef = catid;
                        break;
                        case "chg" :
                        GlobalVariables.catidforchg = catid;
                        break;
                }
             

                string json = "";
                using (var client = new WebClient())
                {
                    json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getsubcatslist.php?id=" + catid);
                }

                var log = JsonConvert.DeserializeObject<catslist>(json);
                List<catsdetail> mylist = new List<catsdetail>();
                //getmemyearlydataViewModel model = new getmemyearlydataViewModel();
                catsdetail newearlydatum = new catsdetail();
                if (log.data != null)
                {
                    foreach (var myvar in log.data)
                    {
                        mylist.Add(myvar);
                    }
                }

                CatPageViewModel model = new CatPageViewModel()
                {
                    Cats = new SelectList(mylist, "ID", "title")
                    // SelectedModel = ? if you want to preselect an item
                };
                model.levelfinder = levelfinder;
                return PartialView("/Views/Shared/AdminShared/_SubCatList.cshtml", model);
            }

        }
        public ActionResult getsubcatlist2(string catid, string levelfinder)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            if (levelfinder == "list")
            {
                GlobalVariables.subcatid = catid;
            }
            else if (levelfinder == "chg")
            {
                GlobalVariables.subcatidforchg = catid;
            }
            else if (levelfinder == "def")
            {
                GlobalVariables.subcatidfordef = catid;
            }
            else if (levelfinder == "del")
            {
                GlobalVariables.subcatidfordel = catid;
            }
            
           
            if (catid == "")
            {
                return PartialView("/Views/Shared/AdminShared/_SubCatVoid.cshtml");
            }
            else
            {
                // GlobalVariables.sub = catid;

                string json = "";
                using (var client = new WebClient())
                {
                    json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getsubcats2list.php?id=" + catid);
                }

                var log = JsonConvert.DeserializeObject<catslist>(json);
                List<catsdetail> mylist = new List<catsdetail>();
                //getmemyearlydataViewModel model = new getmemyearlydataViewModel();
                catsdetail newearlydatum = new catsdetail();
                if (log.data != null)
                {
                    foreach (var myvar in log.data)
                    {
                        mylist.Add(myvar);
                    }
                }

                CatPageViewModel model = new CatPageViewModel()
                {
                    Cats = new SelectList(mylist, "ID", "title")
                    // SelectedModel = ? if you want to preselect an item
                };
                model.levelfinder = levelfinder;
                return PartialView("/Views/Shared/AdminShared/_SubCatList2.cshtml", model);
            }

        }
        public ActionResult getsubcatlistlevel3(string subcatid, string levelfinder)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            if (subcatid == "")
            {
                return PartialView("/Views/Shared/AdminShared/_SubCatVoid.cshtml");
            }
            else
            {
                GlobalVariables.catid = subcatid;
                switch (levelfinder)
                {
                    case "list":
                        GlobalVariables.subcatid2 = subcatid;
                        break;
                    case "del":
                        GlobalVariables.subcatidfordel2 = subcatid;
                        break;
                    case "def":
                        GlobalVariables.subcatidfordef2 = subcatid;
                        break;
                    case "chg":
                        GlobalVariables.subcatidforchg2 = subcatid;
                        break;
                }

                string json = "";
                using (var client = new WebClient())
                {
                    json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getsubcatslist.php?id=" + subcatid);
                }

                var log = JsonConvert.DeserializeObject<catslist>(json);
                List<catsdetail> mylist = new List<catsdetail>();
                //getmemyearlydataViewModel model = new getmemyearlydataViewModel();
                catsdetail newearlydatum = new catsdetail();
                if (log.data != null)
                {
                    foreach (var myvar in log.data)
                    {
                        mylist.Add(myvar);
                    }
                }

                CatPageViewModel model = new CatPageViewModel()
                {
                    Cats = new SelectList(mylist, "ID", "title")
                    // SelectedModel = ? if you want to preselect an item
                };
                model.levelfinder = levelfinder;
                return PartialView("/Views/Shared/AdminShared/_SubCatListlevel3.cshtml", model);
            }

        }

        public ActionResult setglobalsubcatid(string subcatid)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            GlobalVariables.subcatid = subcatid;
            GlobalVariables.subcatidfordef = subcatid;
            return Content("dd");

        }
        public ActionResult setglobalsubcatid2(string subcatid2 , string levelfinder)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            GlobalVariables.subcatid2 = subcatid2;
            GlobalVariables.subcatidfordef2 = subcatid2;
            return Content("sss");
        }
        public ActionResult GetTheListOfItems(int? page)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            string catid = "";
            string subcatid = GlobalVariables.subcatid;

            string subcatid2 = GlobalVariables.subcatid2;
            if (GlobalVariables.catid != null)
            {
                catid = GlobalVariables.catid;
            }

            string json = "";
            using (var client = new WebClient())
                if (subcatid2 != null)
                {
                    json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getorderlist.php?subcatid2=" + subcatid2);

                }
                else if (subcatid != null)
                {
                    json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getorderlist.php?subcatid=" + subcatid);
                }
                else
                {
                    json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getorderlist.php?catid=" + catid);
                }

            var log = JsonConvert.DeserializeObject<oderdetaillist>(json);
            List<orderdetail> mylist = new List<orderdetail>();
            //getmemyearlydataViewModel model = new getmemyearlydataViewModel();
            orderdetail newearlydatum = new orderdetail();
            if (log.data != null)
            {
                foreach (var myvar in log.data)
                {
                    mylist.Add(myvar);
                }
            }

            var queryable = mylist.AsQueryable();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            foreach (var item in queryable)
            {
                item.description = item.description.Trim();
            }

            return PartialView("/Views/Shared/AdminShared/_OrderList.cshtml", queryable.ToPagedList(pageNumber, pageSize));

        }



        public ActionResult setnewfilter(string filterid, string detailtitle)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/setnewfilterdetail.php?title=" + detailtitle + "&filterID=" + filterid);
            }
            if (json.Contains("1"))
            {
                return Content("1");
            }

            else if (json.Contains("0"))
            {
                return Content("0");
            }
            else
            {
                return Content("3");
            }
            return Content("1");
        }
        public ActionResult delfilterdetail(string detailid)
        {
            if (Session["LogedInUser2"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/deletefromfilterdetail.php?id=" + detailid);
            }

            if (json.Contains("1"))
            {
                return Content("1");
            }

            else
            {
                return Content("3");
            }

        }
       
        public ActionResult delallcolor()
        {
            if (Session["LogedInUser2"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            string catID = GlobalVariables.catidforfilter;
            string json = "";

            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/deletefromcolor.php?catID=" + catID);
            }

            if (json.Contains("1"))
            {
                return Content("1");
            }

            else
            {
                return Content("3");
            }
        }

        public ActionResult setnewcat(string cattitle)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            userdata user = Session["LogedInUser2"] as userdata;
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/setnewcat.php?title=" + cattitle + "&ID=" + user.ID);
            }
            if (json.Contains("1"))
            {
                return Content("1");
            }

            else if (json.Contains("0"))
            {
                return Content("0");
            }
            else
            {
                return Content("3");
            }
        }

        public ActionResult setnewtype(string typetitle)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            string catID = GlobalVariables.catidforfilter;
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/setnewtype.php?title=" + typetitle + "&catID=" + catID);
            }
            if (json.Contains("1"))
            {
                return Content("1");
            }

            else if (json.Contains("0"))
            {
                return Content("0");
            }
            else
            {
                return Content("3");
            }
        }
        public ActionResult setnewcolor(string colortitle, string colorcode)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            string catID = GlobalVariables.catidforfilter;
            colorcode = colorcode.Substring(1, colorcode.Count() - 1);
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/setnewcolor.php?title=" + colortitle + "&code=" + colorcode + "&catID=" + catID);
            }
            if (json.Contains("1"))
            {
                return Content("1");
            }

            else if (json.Contains("0"))
            {
                return Content("0");
            }
            else
            {
                return Content("3");
            }
        }
        public ActionResult setnewbrand(string brandtitle)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            string catID = GlobalVariables.catidforfilter;
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/setnewbrand.php?title=" + brandtitle + "&catID=" + catID);
            }
            if (json.Contains("1"))
            {
                return Content("1");
            }

            else if (json.Contains("0"))
            {
                return Content("0");
            }
            else
            {
                return Content("3");
            }
        }
        public ActionResult delnewcat(string catid)
        {
            if (Session["LogedInUser2"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/deletefromcat.php?id=" + catid);
            }
            if (json != "\n\"0\"")
            {
                IEnumerable<imagelistfordel> log = JsonConvert.DeserializeObject<IEnumerable<imagelistfordel>>(json);

                if (log != null)
                {
                    foreach (var item in log)
                    {
                        string pathString = "~/images/panelimages";
                        string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(item.title));
                        System.IO.File.Delete(savedFileName);
                    } return Content("1");
                }


                else
                {
                    return Content("3");
                }
            }
            else
            {
                return Content("1");
            }

        }

        public ActionResult delnewcolor(string colorcode)
        {
            if (Session["LogedInUser2"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            string json = "";
            colorcode = colorcode.Substring(1, colorcode.Count() - 1);
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/deletefromcolor.php?id=" + colorcode);
            }

            if (json.Contains("1"))
            {
                return Content("1");
            }

            else
            {
                return Content("3");
            }
        }
        public ActionResult delnewtype(string typeid)
        {
            if (Session["LogedInUser2"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/deletefromtype.php?id=" + typeid);
            }

            if (json.Contains("1"))
            {
                return Content("1");
            }

            else
            {
                return Content("3");
            }
        }
        public ActionResult delnewbrand(string brandid)
        {
            if (Session["LogedInUser2"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/deletefrombrand.php?id=" + brandid);
            }

            if (json.Contains("1"))
            {
                return Content("1");
            }

            else
            {
                return Content("3");
            }
        }

        public ActionResult setnewsubcat(string catid, string subcattitle)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/setnewsubcat.php?title=" + subcattitle + "&id=" + catid);
            }
            if (json.Contains("1"))
            {
                return Content("1");
            }

            else if (json.Contains("0"))
            {
                return Content("0");
            }
            else
            {
                return Content("3");
            }
        }
        public ActionResult setnewsubcat2(string subcatid, string subcat2, string catid)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/setnewsubcat2.php?title=" + subcat2 + "&subcatid=" + subcatid + "&catid=" + catid);
            }
            if (json.Contains("1"))
            {
                return Content("1");
            }

            else if (json.Contains("0"))
            {
                return Content("0");
            }
            else
            {
                return Content("3");
            }
        }
        public ActionResult deletsubcat2(string ID)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/deletefromsubcat2.php?id=" + ID);
            }
            if (json != "\n\"0\"")
            {
                IEnumerable<imagelistfordel> log = JsonConvert.DeserializeObject<IEnumerable<imagelistfordel>>(json);
                if (log != null)
                {
                    foreach (var item in log)
                    {
                        string pathString = "~/images/panelimages";
                        string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(item.title));
                        System.IO.File.Delete(savedFileName);
                    } return Content("1");
                }
                else
                {
                    return Content("3");
                }
            }
            else
            {
                return Content("1");
            }

        }
        public ActionResult deletsubcat(string ID)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/deletefromsubcat.php?id=" + ID);
            }

            if (json != "\n\"0\"")
            {
                IEnumerable<imagelistfordel> log = JsonConvert.DeserializeObject<IEnumerable<imagelistfordel>>(json);
                if (log != null)
                {
                    foreach (var item in log)
                    {
                        string pathString = "~/images/panelimages";
                        string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(item.title));
                        System.IO.File.Delete(savedFileName);
                    } return Content("1");
                }

                else
                {
                    return Content("3");
                }
            }
            else
            {
                return Content("1");
            }



        }
        public ActionResult changecatname(string ID, string newname, string level)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            string json = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/changecatname.php?id=" + ID + "&level=" + level + "&newname=" + newname);
            }

            if (json == "\n\"1\"")
            {
                return Content("1");
            }
            else
            {
                return Content("2");
            }



        }

        public ActionResult getprofiledata()
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            userdata user = Session["LogedInUser2"] as userdata;
            return PartialView("/Views/Shared/AdminShared/_UserProfile.cshtml", user);
        }


        public ActionResult bringFilterForProductSet(string catid)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }

            if (catid != null)
            {
                GlobalVariables.catidforfilter = catid;

                string json1 = "";
                string json3 = "";


                using (var client = new WebClient())
                {

                    //filters = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getfilterlistfordel.php?catID=" + catID);
                    json1 = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getfilterslist.php?catID=" + catid);
                    //json2 = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getbrandslist.php?catID=" + catID);
                    json3 = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getcoloreslist.php?catID=" + catid);
                }

                //var log = JsonConvert.DeserializeObject<catslist>(filters);
                FilterList log1 = JsonConvert.DeserializeObject<FilterList>(json1);
                var log3 = JsonConvert.DeserializeObject<coloreslist>(json3);
                List<colordetail> colorList = new List<colordetail>();
                colordetail newearlydatum3 = new colordetail();
                if (log3.data != null)
                {
                    foreach (var myvar3 in log3.data)
                    {
                        colorList.Add(myvar3);
                    }
                }
               

                productDetailPageViewModel model = new productDetailPageViewModel()
                {
                    //Filtersfordel = new SelectList(mylist, "ID", "title"),
                    Colores = new SelectList(colorList, "code", "title"),
                    filterlist = log1
                    // SelectedModel = ? if you want to preselect an item
                };
                return PartialView("/Views/Shared/AdminShared/_filterForProductSet.cshtml", model);
            }
            else
            {
                productDetailPageViewModel model = new productDetailPageViewModel();
                return PartialView("/Views/Shared/AdminShared/_filterHolder.cshtml", model);
            }

          
        }

        public ActionResult product(int? page, int? MSG)
        {
            if (Session["LogedInUser2"] != null)
            {
                userdata user = Session["LogedInUser2"] as userdata;
                string suerid = user.ID;

                productinfoviewdetail productmodel = new productinfoviewdetail();
                //if (Request.Cookies["productcookiie"] != null)
                //{
                //    HttpCookie productdetail = new HttpCookie("productcookiie");
                //    productdetail = Request.Cookies["productcookiie"];
                //    productmodel = JsonConvert.DeserializeObject<productinfoviewdetail>(productdetail.Value);

                //}


                // CatPageViewModel model = new CatPageViewModel();
                ViewBag.msg = MSG;
                ViewBag.page = page;
                string json = "";
                string json2 = "";
                string json3 = "";
                string json4 = "";
                using (var client = new WebClient())
                {
                    json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getcatslist.php?ID= " + user.ID);
                    json2 = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getcoloreslist.php ");
                    json3 = client.DownloadString("http://www.supectco.com/webs/textile/Admin/gettypeslist.php? ");
                    json4 = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getbrandslist.php?");
                }

                var log = JsonConvert.DeserializeObject<catslist>(json);
                var log3 = JsonConvert.DeserializeObject<catslist>(json3);
                var log4 = JsonConvert.DeserializeObject<catslist>(json4);
                var log2 = JsonConvert.DeserializeObject<coloreslist>(json2);
                List<catsdetail> mylist = new List<catsdetail>();
                List<catsdetail> mylist3 = new List<catsdetail>();
                List<catsdetail> mylist4 = new List<catsdetail>();
                List<colordetail> mylist2 = new List<colordetail>();
                //getmemyearlydataViewModel model = new getmemyearlydataViewModel();
                catsdetail newearlydatum = new catsdetail();
                catsdetail newearlydatum3 = new catsdetail();
                catsdetail newearlydatum4 = new catsdetail();
                colordetail newearlydatum2 = new colordetail();
                if (log.data != null)
                {
                    foreach (var myvar in log.data)
                    {
                        mylist.Add(myvar);
                    }
                }

                if (log3.data != null)
                {
                    foreach (var myvar3 in log3.data)
                    {
                        mylist3.Add(myvar3);
                    }
                }

                if (log4.data != null)
                {
                    foreach (var myvar4 in log4.data)
                    {
                        mylist4.Add(myvar4);
                    }
                }

                if (log2.data != null)
                {
                    foreach (var myvar2 in log2.data)
                    {
                        mylist2.Add(myvar2);
                    }
                }

                CatPageViewModel model = new CatPageViewModel()
                {
                    Cats = new SelectList(mylist, "ID", "title"),
                    Colores = new SelectList(mylist2, "code", "title"),
                    types = new SelectList(mylist3, "ID", "title"),
                    brands = new SelectList(mylist4, "ID", "title"),
                    selectedfilters = string.IsNullOrEmpty(productmodel.Selectedfilters) ? "" : productmodel.Selectedfilters,
                    //Selectedtype = string.IsNullOrEmpty(productmodel.Selectedtype) ? "" : productmodel.Selectedtype,
                    //Selectedbrand = string.IsNullOrEmpty(productmodel.Selectedbrand) ? "" : productmodel.Selectedbrand,
                     SelectedCat = string.IsNullOrEmpty(productmodel.SelectedCat) ? "" : productmodel.SelectedCat,
                     productmodel = productmodel
                    // SelectedModel = ? if you want to preselect an item
                };
                return View(model);


            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }

        }



        [HttpPost]
        public ActionResult setproduct(productinfoviewdetail detail)
        {

            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            detail.inputallfilterid = detail.inputallfilterid.Substring(0, detail.inputallfilterid.Length - 1);
            List<string> myfilter = detail.inputallfilterid.Split(';').ToList();
            List<string> query = (from p in myfilter
                         let index = p.IndexOf("-")
                         let first = p.Substring(0, index)
                         group p by first into g
                         select g.Last()).ToList();
            string finalfilter = "";
            foreach (var filter in query)
            {
                finalfilter += filter + ";"; 
            }
            finalfilter = finalfilter.Substring(0, finalfilter.Length - 1);
            productinfoviewdetail firsmodel = detail;

            firsmodel.file = null;
            var js = new JavaScriptSerializer().Serialize(firsmodel);
            HttpCookie productcookie = new HttpCookie("productcookiie");
            productcookie.Value = js;
            productcookie.Expires = DateTime.Now.AddHours(1);
            Response.Cookies.Add(productcookie);
            string catid = GlobalVariables.catidfordef;
            string subcatid;
            string subcatid2;
            if (GlobalVariables.subcatidfordef == null)
            {
                subcatid = "0";
            }
            else
            {
                subcatid = GlobalVariables.subcatidfordef;
            }
            if (GlobalVariables.subcatidfordef2 == null)
            {
                subcatid2 = "0";
            }
            else
            {
                subcatid2 = GlobalVariables.subcatidfordef2;
            }


            //if (Session["LogedInUser2"] == null)
            //{
            //    return RedirectToAction("Index", "Admin");
            //}

            // productviewmodel model = new productviewmodel();

            string pathString = "~/images/panelimages";
            if (!Directory.Exists(Server.MapPath(pathString)))
            {
                DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(pathString));
            }


            int message = 1;

            try
            {

                List<string> imagelist = new List<string>();

                for (int i = 0; i < Request.Files.Count; i++)
                {

                    HttpPostedFileBase hpf = Request.Files[i];
                    string tobeaddedtoimagename = RandomString(7);
                    imagelist.Add(tobeaddedtoimagename + ".jpg");
                    if (hpf.ContentLength == 0)
                        continue;



                    //using (WebClient client = new WebClient())
                    //{
                    //    string ftpUsername = @"meri@supectco.com";
                    //    string ftpPassword = @"!)lAx3_-h43s";
                    //    client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                    //    client.UploadFile("ftp://www.supectco.com/public_html/webs/textile/api/portal/uploads/" + hpf.FileName, "STOR", savedFileName);
                    //}
                }
                string imglst = "";
                foreach (var item in imagelist)
                {
                    imglst += "&imaglist[]=" + item;
                }

                string json;
                string title = detail.productname;
                string desc = detail.productdesc;
                string discount = detail.productdiscount;
                string color = "";
                if (detail.SelectedColor != null)
                {
                    color = detail.SelectedColor;
                    color = color.Substring(1, color.Count() - 1);
                }
                string filter = "";
                if (finalfilter != "")
                {
                    filter = finalfilter;
                }

                string price = detail.productprice;
                string setid = detail.SetID;
                

              
                userdata USER = Session["LogedInUser2"] as userdata;
                string MezonId = USER.ID;
                using (var client = new WebClient())
                {

                    json = client.DownloadString("http://supectco.com/webs/textile/Admin/addProduct.php?title=" + title + "&catid=" + catid + "&subcatid=" + subcatid + "&subcatid2=" + subcatid2 + "&desc=" + desc + "&SetID=" + setid + "&discount=" + discount + "&color=" + color + "&filter=" + filter + imglst);
                    // json = client.DownloadString("http://supectco.com/webs/textile/addProduct.php?title=" + title + "&parentid=" + parentid + "&setid=" + setid + "&price=" + price + "&imagethum=" + imagethum + "&image1=" + image1 + "&image2=" + image2 + "&image3=" + image3 + "&image4=" + image4 + "&image5=" + image5 + "&desc=" + desc + "&mezonid=" + MezonId + "&discount=" + discount + "&color=" + color);
                }

                if (json.Contains("error"))
                {

                    message = 2;
                }
                else if(json.Contains("exist")){
                     message = 3;
                }
                else
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {

                        HttpPostedFileBase hpf = Request.Files[i];
                        string imagename = imagelist[i];
                        if (hpf.ContentLength == 0)
                            continue;

                        string savedFileName = Path.Combine(Server.MapPath(pathString), imagename);
                        int k = 1;
                        hpf.SaveAs(savedFileName); // Save the file
                        //int width = 500;
                        //int height = 500;
                        //Image image = Image.FromFile(savedFileName);
                        //var destRect = new Rectangle(0, 0, width, height);
                        //var destImage = new Bitmap(width, height);

                        //destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                        //using (var graphics = Graphics.FromImage(destImage))
                        //{
                        //    graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                        //    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        //    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        //    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        //    graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                        //    using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                        //    {
                        //        wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                        //        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                        //    }
                        //}
                        //string thumsavedFileName = Path.Combine(Server.MapPath(pathString), json + Path.GetFileName(hpf.FileName));
                        //destImage.Save(thumsavedFileName, System.Drawing.Imaging.ImageFormat.Jpeg);



                        //using (WebClient client = new WebClient())
                        //{
                        //    string ftpUsername = @"meri@supectco.com";
                        //    string ftpPassword = @"!)lAx3_-h43s";
                        //    client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                        //    client.UploadFile("ftp://www.supectco.com/public_html/webs/textile/api/portal/uploads/thum" + hpf.FileName, "STOR", thumsavedFileName);
                        //}
                        //destImage.Dispose();
                        //image.Dispose();


                        //  System.IO.File.Delete(savedFileName);

                        //using (WebClient client = new WebClient())
                        //{
                        //    string ftpUsername = @"meri@supectco.com";
                        //    string ftpPassword = @"!)lAx3_-h43s";
                        //    client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                        //    client.UploadFile("ftp://www.supectco.com/public_html/webs/textile/api/portal/uploads/" + hpf.FileName, "STOR", savedFileName);
                        //}

                    }
                }






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
                message = 2;
            }

            return RedirectToAction("product", "Admin", new { MSG = message });
        }
        public ActionResult Delete(int id)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            ViewBag.Message = "Your application description page.";


            productinfoviewdetail model = new productinfoviewdetail();
            string json = "";
            using (var client = new WebClient())
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/deleteproduct.php?id=" + id);

            scarfproject.ViewModel.imagelistViwModel log = JsonConvert.DeserializeObject<scarfproject.ViewModel.imagelistViwModel>(json);
            if (log.List.Count > 0)
            {
                foreach (var item in log.List)
                {
                    string pathString = "~/images/panelimages";
                    string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(item.title));
                    System.IO.File.Delete(savedFileName);
                }
                return Content("1");
            }
            else
            {
                return Content("2");
            }


        }
        public ActionResult Edit(int id)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }

            string json = "";
            using (var client = new WebClient())
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getorderdetail.php?id=" + id);

            var log = JsonConvert.DeserializeObject<earlyRoot>(json);
            List<Datum> mylist = new List<Datum>();
            //getmemyearlydataViewModel model = new getmemyearlydataViewModel();
            Datum newearlydatum = new Datum();
            newearlydatum = log.data[0];
            string catID = GlobalVariables.catidfororderlist;
            string json1 = "";
            string json2 = "";
            string json3 = "";
          
            using (var client = new WebClient())
            {
                json1 = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getfilterslist.php?catID=" + catID);
                json2 = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getcoloreslist.php?catID=" + catID);
                json3 = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getproductfilters.php?ID=" + id);
            }

            FilterList log1 = JsonConvert.DeserializeObject<FilterList>(json1);
            ProductFilterList log3 = JsonConvert.DeserializeObject<ProductFilterList>(json3);
          
            var log2 = JsonConvert.DeserializeObject<coloreslist>(json2);

    
            List<colordetail> mylist2 = new List<colordetail>();
            //getmemyearlydataViewModel model = new getmemyearlydataViewModel();

            colordetail newearlydatum2 = new colordetail();


       

            if (log2.data != null)
            {
                foreach (var myvar2 in log2.data)
                {
                    mylist2.Add(myvar2);
                }
            }

            ViewBag.Message = "Your application description page.";




            //if (log.data != null)
            //{
            //    foreach (var myvar in log.data)
            //    {
            //        mylist.Add(myvar);
            //    }
            //}
            newearlydatum.ID = id.ToString();


            Datumm model = new Datumm()
            {
                color = newearlydatum.color,
                count = newearlydatum.count,
                description = newearlydatum.description,
                discount = newearlydatum.discount,
                ID = newearlydatum.ID,
                imagelist = newearlydatum.imagelist,
                isActive = newearlydatum.isActive,
                IsNew = newearlydatum.IsNew,
                IsOffer = newearlydatum.IsOffer,
                PriceNow = newearlydatum.PriceNow,
                productprice = newearlydatum.productprice,
                SetId = newearlydatum.SetId,
                title = newearlydatum.title,
                brand = newearlydatum.brand,
                type = newearlydatum.type,
                Colores = new SelectList(mylist2, "code", "title"),
                filters = log1,
                productfilterlist = log3

            };
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(productinfoforedit detail)
        {

            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            detail.inputallfilterid = detail.inputallfilterid.Substring(0, detail.inputallfilterid.Length - 1);
            List<string> myfilter = detail.inputallfilterid.Split(';').ToList();
            List<string> query = (from p in myfilter
                                  let index = p.IndexOf("-")
                                  let first = p.Substring(0, index)
                                  group p by first into g
                                  select g.Last()).ToList();
            string finalfilter = "";
            foreach (var filter in query)
            {
                finalfilter += filter + ";";
            }
            finalfilter = finalfilter.Substring(0, finalfilter.Length - 1);

            //if (Session["LogedInUser2"] == null)
            //{
            //    return RedirectToAction("Index", "Admin");
            //}

            // productviewmodel model = new productviewmodel();

            string pathString = "~/images/panelimages";
            if (!Directory.Exists(Server.MapPath(pathString)))
            {
                DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(pathString));
            }




            try
            {
                List<string> imagelist = new List<string>();


                if (detail.file.Count() > 0 && detail.file.First() != null)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase hpf = Request.Files[i];
                        imagelist.Add(hpf.FileName);
                        if (hpf.ContentLength == 0)
                            continue;
                        string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(hpf.FileName));
                        hpf.SaveAs(savedFileName); // Save the file

                        //using (WebClient client = new WebClient())
                        //{
                        //    string ftpUsername = @"meri@supectco.com";
                        //    string ftpPassword = @"!)lAx3_-h43s";
                        //    client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                        //    client.UploadFile("ftp://www.supectco.com/public_html/webs/textile/api/portal/uploads/" + hpf.FileName, "STOR", savedFileName);
                        //}
                    }
                }

                string imglst = "";
                foreach (var item in imagelist)
                {
                    imglst += "&imaglist[]=" + item;
                }

                string json;
                string title = detail.title;
                string desc = detail.productdesc;
                string id = detail.ID;
                string type = detail.type;
                string brand = detail.brand;
                string color = "";

                
                if (detail.color != null)
                {
                    color = detail.color;
                    color = color.Substring(1, color.Count() - 1);
                }
                string filter = "";
                if (finalfilter != "")
                {
                    filter = finalfilter;
                }
                // userdata USER = Session["LogedInUser2"] as userdata;
                //string MezonId = USER.ID;
                using (var client = new WebClient())
                {

                    json = client.DownloadString("http://supectco.com/webs/textile/Admin/editproduct.php?title=" + title + "&description=" + desc + "&id=" + id + "&price=" + detail.productprice + "&setid=" + detail.SetID + "&discount=" + detail.discount + "&filter=" + filter + " &brandx=" + brand + "&color=" + color + imglst);
                    int i = 0;
                    // json = client.DownloadString("http://supectco.com/webs/textile/addProduct.php?title=" + title + "&parentid=" + parentid + "&setid=" + setid + "&price=" + price + "&imagethum=" + imagethum + "&image1=" + image1 + "&image2=" + image2 + "&image3=" + image3 + "&image4=" + image4 + "&image5=" + image5 + "&desc=" + desc + "&mezonid=" + MezonId + "&discount=" + discount + "&color=" + color);
                }

                ViewBag.message = "محصول مورد نظر اضافه شد";

                return RedirectToAction("Edit", "Admin", detail.ID);
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
        public ActionResult Slider()
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            ViewBag.Message = "Your application description page.";


            productinfoviewdetail model = new productinfoviewdetail();
            string json = "";
            using (var client = new WebClient())
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getsliderlist.php");

            var log = JsonConvert.DeserializeObject<sliderlst>(json);
            List<sliderDT> mylist = new List<sliderDT>();
            //getmemyearlydataViewModel model = new getmemyearlydataViewModel();
            sliderDT newearlydatum = new sliderDT();
            // newearlydatum = log.data[0];
            if (log.data != null)
            {
                foreach (var myvar in log.data)
                {
                    mylist.Add(myvar);
                }
            }

            return View(mylist);
        }
        [HttpPost]
        public ActionResult Slider(sliderforedit detail)
        {

            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }

            string tobeaddedtosliderimage = RandomString(5);
            //if (Session["LogedInUser2"] == null)
            //{
            //    return RedirectToAction("Index", "Admin");
            //}

            // productviewmodel model = new productviewmodel();

            string pathString = "~/images/panelimages";
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
                    imagelist.Add(tobeaddedtosliderimage + hpf.FileName);
                    if (hpf.ContentLength == 0)
                        continue;
                    string savedFileName = Path.Combine(Server.MapPath(pathString), tobeaddedtosliderimage + Path.GetFileName(hpf.FileName));
                    hpf.SaveAs(savedFileName); // Save the file

                    //using (WebClient client = new WebClient())
                    //{
                    //    string ftpUsername = @"meri@supectco.com";
                    //    string ftpPassword = @"!)lAx3_-h43s";
                    //    client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                    //    client.UploadFile("ftp://www.supectco.com/public_html/webs/textile/api/portal/uploads/" + hpf.FileName, "STOR", savedFileName);
                    //}
                }
                string imglst = "";
                foreach (var item in imagelist)
                {
                    imglst += "&imaglist[]=" + item;
                }

                string json;



                // userdata USER = Session["LogedInUser2"] as userdata;
                //string MezonId = USER.ID;
                using (var client = new WebClient())
                {
                    json = client.DownloadString("http://supectco.com/webs/textile/Admin/addslider.php?" + imglst);
                    // json = client.DownloadString("http://supectco.com/webs/textile/addProduct.php?title=" + title + "&parentid=" + parentid + "&setid=" + setid + "&price=" + price + "&imagethum=" + imagethum + "&image1=" + image1 + "&image2=" + image2 + "&image3=" + image3 + "&image4=" + image4 + "&image5=" + image5 + "&desc=" + desc + "&mezonid=" + MezonId + "&discount=" + discount + "&color=" + color);
                }

                ViewBag.message = "محصول مورد نظر اضافه شد";

                return RedirectToAction("Slider", "Admin");
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
        public ActionResult Profile(int? num)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            if (num == 1)
            {
                ViewBag.num = 1;
            }
            else if (num == 2)
            {
                ViewBag.num = 2;
            }
            userdata user = Session["LogedInUser2"] as userdata;



            productinfoviewdetail model = new productinfoviewdetail();
            string json = "";
            using (var client = new WebClient())
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/getuserinfo.php?id=" + user.ID);

            var log = JsonConvert.DeserializeObject<userinfolist>(json);
            List<sliderDT> mylist = new List<sliderDT>();
            //getmemyearlydataViewModel model = new getmemyearlydataViewModel();
            userinfo newearlydatum = new userinfo();
            newearlydatum = log.data[0];
            //if (log.data != null)
            //{
            //    foreach (var myvar in log.data)
            //    {
            //        mylist.Add(myvar);
            //    }
            //}

            return View(newearlydatum);
        }
        [HttpPost]
        public ActionResult Profile(userinfo detail)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            userdata user = Session["LogedInUser2"] as userdata;
            try
            {

                string json;

                using (var client = new WebClient())
                {
                    json = client.DownloadString("http://supectco.com/webs/textile/Admin/editprofile.php?id=" + user.ID + "&fullname=" + detail.fullname + "&aboutus=" + detail.aboutus + "&phobe=" + detail.phone + "&mobile=" + detail.mobile + "&instagram=" + detail.instagram + "&telegram=" + detail.telegram + "&address=" + detail.address);

                }

                if (json.Contains("1"))
                {
                    return RedirectToAction("Profile", "Admin", new { num = 1 });
                }

                return RedirectToAction("Profile", "Admin", new { num = 2 });
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
        public ActionResult deleteimage(string id, string title)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            string str = id;
            str = str.Substring(9, str.Length - 9);
            ViewBag.Message = "Your application description page.";


            string pathString = "~/images/panelimages";
            string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(title));
            System.IO.File.Delete(savedFileName);

            productinfoviewdetail model = new productinfoviewdetail();
            string json = "";
            using (var client = new WebClient())
                json = client.DownloadString("http://www.supectco.com/webs/textile/Admin/deleteimage.php?id=" + str);


            return Content("1");
        }


        public ActionResult Thumbnail(string filename)
        {
            if (Session["LogedInUser2"] == null)
            {

                return RedirectToAction("Index", "Admin");

            }
            string pathString = "~/images/panelimages";
            string savedFileName = Path.Combine(Server.MapPath(pathString), Path.GetFileName(filename));

            var img = new WebImage(savedFileName)
                .Resize(320, 285, false, true);
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
    }
}