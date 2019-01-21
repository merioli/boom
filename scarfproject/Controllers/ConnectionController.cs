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

//using scarfproject.ServiceReference1;




namespace scarfproject.Controllers
{

    public class ConnectionController : Controller
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
       
        public void ReqestForPayment()
        {

            string device = RandomString();
            string code = MD5Hash(device + "ncase8934f49909");
            List<string> finalmodel = new List<string>();
            HttpCookie productides = new HttpCookie("cartmodel");
            productides = Request.Cookies["cartmodel"];
            List<ProductDetail> data = JsonConvert.DeserializeObject<List<ProductDetail>>(productides.Value);

            userdata user = Session["LogedInUser"] as userdata;
            
            string email = "";
            if (user.email != null)
            {
                email = user.email;
            }
            string token = user.token;
            GlobalVariables.token = token;
            string address = user.address;
            string fullname = user.fullname;
            string phone = user.phone;
            string userid = user.ID;
            string state = "";
            string city = "";
            string discount = "";
            string postalCode = "";
            string result = "";
            string ids = ",";
            string nums = ",";

            foreach (var item in data)
            {
                ids += "," + (item.productid);
                nums += ","+ (item.quantity);
            }
            ids = ids.Substring(1, ids.Count() - 1);
            nums = ids.Substring(1, nums.Count() - 1);
            using (WebClient client = new WebClient())
            {

                var collection = new NameValueCollection();
                collection.Add("device", device);
                collection.Add("code", code);
                collection.Add("fullname", fullname);
                collection.Add("mobile", phone);
                collection.Add("email", email);
                collection.Add("state", state);
                collection.Add("city", city);
                collection.Add("address", address);
                collection.Add("ids", ids);
                collection.Add("nums", nums);
                collection.Add("token", token);
                collection.Add("discount", discount);
                collection.Add("postalCode", postalCode);

                //foreach (var myvalucollection in imaglist) {
                //    collection.Add("imaglist[]", myvalucollection);
                //}
                byte[] response =
                client.UploadValues("http://supectco.com/webs/textile/Main/handler/buyRequest.php", collection);

                result = System.Text.Encoding.UTF8.GetString(response);
            }

            scarfproject.ViewModelPost.buyRequest log2 = JsonConvert.DeserializeObject<scarfproject.ViewModelPost.buyRequest>(result);

            if (log2.status == 200)
            {
                string txtDescription = "شماره پیگیری:" + log2.peigiry;
                string timestamp = log2.orderID;
                System.Net.ServicePointManager.Expect100Continue = false;
                ServiceReference1.PaymentGatewayImplementationServicePortTypeClient zp = new ServiceReference1.PaymentGatewayImplementationServicePortTypeClient();
                string Authority;

                int Status = zp.PaymentRequest("9e9d57bc-07e1-11e8-ad17-000c295eb8fc", log2.amount, txtDescription, "info@charghadshop.com", "", "http://localhost:2618/Connection/Verify", out Authority);

               if (Status == 100)
                {
                    string result2 = "";
                    using (WebClient client = new WebClient())
                    {

                        var collection2 = new NameValueCollection();
                        collection2.Add("device", device);
                        collection2.Add("code", code);
                        collection2.Add("auth", Authority);
                        collection2.Add("timestamp", timestamp);
                        collection2.Add("token", token);
                        byte[] response =
                        client.UploadValues("http://supectco.com/webs/textile/Main/handler/setAUTcode.php", collection2);

                        result2 = System.Text.Encoding.UTF8.GetString(response);
                    }

                    //scarfproject.ViewModelPost.buyRequest log3 = JsonConvert.DeserializeObject<scarfproject.ViewModelPost.buyRequest>(result);

                    Response.Redirect("https://www.zarinpal.com/pg/StartPay/" + Authority);
                }
                else
                {
                    Response.Write("error: " + Status);
                }
            }
            //string json = "";
            


        }
        public ActionResult Verify()
        // public void Verify()
        {
            try
            {

                if (Request.QueryString["Status"] != "" && Request.QueryString["Status"] != null && Request.QueryString["Authority"] != "" && Request.QueryString["Authority"] != null)
                {
                    if (Request.QueryString["Status"].ToString().Equals("OK"))
                    {
                        string json = "";
                        string json2 = "";
                        using (var client = new WebClient())
                        {
                            json = client.DownloadString("http://supectco.com/webs/textile/getorderamount.php?auth=" + Request.QueryString["Authority"]);
                        }

                        var log2 = JsonConvert.DeserializeObject<List<AUTHModel>>(json);
                        int Amount = Convert.ToInt32(log2[0].TotalPrice);


                        long RefID;
                        System.Net.ServicePointManager.Expect100Continue = false;
                        ServiceReference1.PaymentGatewayImplementationServicePortTypeClient zp = new ServiceReference1.PaymentGatewayImplementationServicePortTypeClient();




                        int Status = zp.PaymentVerification("9e9d57bc-07e1-11e8-ad17-000c295eb8fc", Request.QueryString["Authority"].ToString(), Amount, out RefID);

                        if (Status == 100)
                        {

                            string device = RandomString();
                            string code = MD5Hash(device + "ncase8934f49909");

                            string result2 = "";
                            using (WebClient client = new WebClient())
                            {

                                var collection2 = new NameValueCollection();
                                collection2.Add("device", device);
                                collection2.Add("code", code);
                                collection2.Add("auth", Request.QueryString["Authority"]);
                                collection2.Add("amount", Amount.ToString());
                                collection2.Add("token", GlobalVariables.token);
                                collection2.Add("refID", RefID.ToString());
                                
                                byte[] response =
                                client.UploadValues("http://supectco.com/webs/textile/Main/handler/doFinalCheck.php", collection2);

                                result2 = System.Text.Encoding.UTF8.GetString(response);
                            }


                            //using (var client = new WebClient())
                            //{
                            //    json = client.DownloadString("http://supectco.com/webs/textile/getorderdetail.php?auth=" + Request.QueryString["Authority"]);
                            //}
                            //var log = JsonConvert.DeserializeObject<List<OrderIdModel>>(json);
                            //ViewBag.message = "موفق";
                            //ViewBag.message2 = RefID;
                            //ViewBag.message3 = log[0].ID;

                            //HttpCookie productides = new HttpCookie("cartmodel");
                            //productides = Request.Cookies["cartmodel"];
                            //List<ProductDetail> data = JsonConvert.DeserializeObject<List<ProductDetail>>(productides.Value);
                            //foreach (var item in data)
                            //{

                            //    using (var client = new WebClient())
                            //    {
                            //        json2 = client.DownloadString("http://supectco.com/webs/textile/addOrderProduct.php?orderid=" + log[0].ID + "&productid=" + item.productid + "&quantity=" + item.quantity);
                            //    }


                            //}

                            //Response.Cookies["cartmodel"].Expires = DateTime.Now.AddDays(-1);
                            HttpCookie myCookie = new HttpCookie("cartmodel");
                            myCookie.Expires = DateTime.Now.AddDays(-1d);
                            Response.Cookies.Add(myCookie);
                            ViewBag.message = "موفق";
                            ViewBag.message2 = RefID;



                        }
                        else
                        {
                            ViewBag.message = "عدم موفقیت خرید";
                            ViewBag.message2 = "شماره پیگیری:" + RefID;
                        }

                    }
                    else
                    {
                        Response.Write("some thing wrong with payment");
                    }
                }
                else
                {
                    Response.Write("Invalid Input");
                }
            }
            catch (Exception e)
            {
                Response.Write(e.Message);
            }

            return View();
        }
        public void test(string auth)
        {
            string json = "";
            string json2 = "";
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://supectco.com/webs/textile/getorderdetail.php?auth=" + auth);
            }
            var log = JsonConvert.DeserializeObject<List<OrderIdModel>>(json);
            ViewBag.message = "موفق";
            ViewBag.message2 = "225";
            ViewBag.message3 = log[0].ID;

            HttpCookie productides = new HttpCookie("cartmodel");
            productides = Request.Cookies["cartmodel"];
            List<ProductDetail> data = JsonConvert.DeserializeObject<List<ProductDetail>>(productides.Value);
            foreach (var item in data)
            {
                using (var client = new WebClient())
                {
                    json2 = client.DownloadString("http://supectco.com/webs/textile/addOrderProduct.php?orderid=" + log[0].ID + "&productid=" + item.productid + "&quantity=" + item.quantity);
                }
            }

            HttpCookie myCookie = new HttpCookie("cartmodel");
            myCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(myCookie);
        }
    }
}
