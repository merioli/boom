using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Newtonsoft.Json;
using System.Net.Mail;
using scarfproject.ViewModel;
using scarfproject.Classes;

namespace scarfproject.Controllers
{
    public class CustomerLoginController : Controller
    {
        //
        // GET: /CustomerLogin/

        public ActionResult Index()
        {
            return View();
        }
        public ContentResult CustomerLogin(string email, string pass, string ischecked, string phone, string kRP)
        {
            string currentpage = GlobalVariables.currentpage;
            var response = Request["g-recaptcha-response"];
            //secret that was generated in key value pair
            const string secret = "6LcrlkUUAAAAADrf4q1hRMplMXEvmJuogXztZY7c";

            var client = new WebClient();
            var reply =
                client.DownloadString(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}",
                secret, response));

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            //when response is false check for the error message
            if (captchaResponse.Success == "false")
            {
                return Content("3" + currentpage);
            }
            else
            {
               
                backpageviewmodel model = new backpageviewmodel();

                string json;
                using (var client2 = new WebClient())
                {
                    json = client2.DownloadString("http://supectco.com/webs/textile/getuserid.php?email=" + email + "&pass=" + pass + "&phone=" + phone);
                    //json = client.DownloadString("http://supectco.com/webs/polytrade/php/grid/getuserid.php?phone=" + phone + "&pass=" + pass);
                }
                var log = JsonConvert.DeserializeObject<List<userdata>>(json);
                if (log != null)
                {
                    userdata user = log[0];
                    if (user.ID != "2" && user.ID != "3")
                    {
                        Session["LogedInUser"] = user;
                       
                        if (ischecked == "checked")
                        {
                            HttpCookie Username = new HttpCookie("Username");
                            HttpCookie Password = new HttpCookie("Password");
                            DateTime now = DateTime.Now;
                            if (phone == "")
                            {
                                Username.Value = email;
                            }
                            else
                            {
                                Username.Value = phone;
                            }
                            Username.Expires = now.AddMonths(1);
                            Password.Value = pass;
                            Password.Expires = now.AddMonths(1);
                            Response.Cookies.Add(Username);
                            Response.Cookies.Add(Password);
                        }
                        if (user.code == "1")
                        {
                            return Content("1/Home/Profile" );

                        }


                        return Content("1" + currentpage);
                    }

                }

                return Content("2" + currentpage);
            }



           


        }

        //  public ContentResult CustomerRegister(string email, string pass, string phone)
        //public ContentResult CustomerRegister( string phone, string address, string name)
        public ContentResult CustomerVerification(string phone, string email)
        {
           
            string json;
            using (var client = new WebClient())
            {
                //json = client.DownloadString("http://supectco.com/webs/polytrade/php/grid/setuser.php?email=" + email + "&pass=" + pass + "&phone" + phone);
                // json = client.DownloadString("http://supectco.com/webs/textile/setuseraddressandphone.php?" + "phone=" + phone + "&address=" + address + "&name=" + name);
                json = client.DownloadString("http://supectco.com/webs/textile/IsUserExist.php?email=" + email + "&phone=" + phone);

            }
            var log = JsonConvert.DeserializeObject<List<userdata>>(json);
            userdata user = new userdata();
            if (log != null )
            {
                var random = new Random();
                int i = random.Next(1111, 9999);
                GlobalVariables.verificationval = i;
                user = log[0];
                if (user.ID == "3" && phone != "")
                {
                    
                    //send.SendSimpleSMS("mehrdadmansouri", "4444", new string[] { "09122014833" }, "500010003056548", "تست ارسال پیامک", false);

                    //پیام صوتی


                    scarfproject.com.payamakpanel.api.voice.Voice voice = new scarfproject.com.payamakpanel.api.voice.Voice();
                    string srt =   voice.SendSMSWithSpeechText(
                        "mehrdadmansouri",  // نام کاربری
                        "4444",             //کلمه عبور
                         "کُدِ فَعال سازی " + i,  //متن پیامک متنی جهت ارسال
                        "کُدِ فَعال سازی " + i + "تِکرار میکُنَم" + i,   // متن پیام صوتی جهت ارسال در صورت عدم دریافت پیامک
                        "500010003056548",  //شماره اختصاصی جهت ارسال
                        phone       //شماره مخاطب هدف
                        );
                    return Content("4");
                }

                else
                {

                    try
                    {
                        string strEmailFrom = "mail@charghadshop.com";
                        string strEmailPass = "Ick4a5*2";
                        string strEmailServer = "mail.charghadshop.com";
                        string strDisplayName = "چارقد";

                        MailMessage message = new MailMessage();
                        message.BodyEncoding = System.Text.Encoding.UTF8;
                        message.SubjectEncoding = System.Text.Encoding.UTF8;
                        message.Priority = MailPriority.Normal;
                        message.IsBodyHtml = true;
                        message.From = new MailAddress(strEmailFrom, strDisplayName);

                        message.To.Add(new MailAddress(email));
                        message.Subject = "کد تایید";
                        message.Body = "<p> کد تایید شما:" + "<br/>"+ i + "</br>" + "</div>";
                        message.IsBodyHtml = true;
                        SmtpClient client = new SmtpClient(strEmailServer, 25);
                        client.Credentials = new System.Net.NetworkCredential(strEmailFrom, strEmailPass);
                        client.Send(message);

                        return Content("5");

                    }
                    catch (Exception e)
                    {


                        return Content(e.ToString(), "text/plain");

                    }

                }

            }
            
            return Content("6");


            
        }

        [HttpPost]
        public void appsendsms(string phone, string code) {
            scarfproject.com.payamakpanel.api.voice.Voice voice = new scarfproject.com.payamakpanel.api.voice.Voice();
            string srt = voice.SendSMSWithSpeechText(
                "mehrdadmansouri",  // نام کاربری
                "4444",             //کلمه عبور
                 " کاربر گرامی کد فعال سازی شما " + code + " میباشد. باتشکر چارقد ",  //متن پیامک متنی جهت ارسال
                "کُدِ فَعال سازی شُما " + code + "تِکرار میشَوَد" + code,   // متن پیام صوتی جهت ارسال در صورت عدم دریافت پیامک
                "500010003056548",  //شماره اختصاصی جهت ارسال
                phone       //شماره مخاطب هدف
                );
        }
        
        public ContentResult CustomerRegister(string email, string pass, string phone, string val)
        {
            string currentpage = Session["currentpage"] as string;


            if( val == GlobalVariables.verificationval.ToString()){
               
                string json;
                using (var client = new WebClient())
                {
                    //json = client.DownloadString("http://supectco.com/webs/polytrade/php/grid/setuser.php?email=" + email + "&pass=" + pass + "&phone" + phone);
                    // json = client.DownloadString("http://supectco.com/webs/textile/setuseraddressandphone.php?" + "phone=" + phone + "&address=" + address + "&name=" + name);
                    json = client.DownloadString("http://supectco.com/webs/textile/setuser.php?pass=" + pass + "&email=" + email + "&phone=" + phone);

                }
                var log = JsonConvert.DeserializeObject<List<userdata>>(json);
                userdata user = new userdata();
                if (log != null)
                {
                    user = log[0];
                    if (user.ID != "2" && user.ID != "3")
                    {
                        Session["LogedInUser"] = user;
                        return Content(user.ID + "*" + currentpage);
                    }

                }

                return Content("2" + "*" + currentpage);
            }
            else
            {
                return Content("5" + "*" + currentpage);
            }
            
        }
        public ActionResult ForgetPass(string email)
        {
            string json;
            string srt = "http://supectco.com/webs/shargh/getuserpassword.php?email=" + email;
           // string srt = "http://localhost:2618/webs/shargh/getuserpassword.php?email=" + email;
            using (var client = new WebClient())
            {
                json = client.DownloadString(srt);
            }
            var log = JsonConvert.DeserializeObject<string>(json);
            if (log != "2" & log != "3")
            {
                try
                {
                    string strEmailFrom = "mail@charghadshop.com";
                    string strEmailPass = "Ick4a5*2";
                    string strEmailServer = "mail.charghadshop.com";
                    string strDisplayName = "چارقد";

                    MailMessage message = new MailMessage();
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.SubjectEncoding = System.Text.Encoding.UTF8;
                    message.Priority = MailPriority.Normal;
                    message.IsBodyHtml = true;
                    message.From = new MailAddress(strEmailFrom, strDisplayName);

                    message.To.Add(new MailAddress(email));
                    message.Subject = "رمز عبور";
                    // message.Body = "message body";
                http://localhost:2618/
                   // message.Body = "<p> لینک بازنشانی نام عبور:" + "<br/>" + "http://www.charghadshop.com/CustomerLogin/ChangePass?Token=" + log + "</br>" + "</div>";
                    message.Body = "<p> لینک بازنشانی نام عبور:" + "<br/>" + " http://localhost:2618/CustomerLogin/ChangePass?Token=" + log + "</br>" + "</div>";
                    // message.Body = "<p> لینک بازنشانی نام عبور:" + "<br/>"+"منتنمنتنمنتم"+ "</br>" + "</div>";

                    message.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient(strEmailServer, 25);
                    client.Credentials = new System.Net.NetworkCredential(strEmailFrom, strEmailPass);
                    client.Send(message);

                    return Content("1");

                }
                catch (Exception e)
                {


                    return Content(e.ToString(), "text/plain");

                }



            }
            else if (log == "3")
            {

                return Content("3");

            }
            else { return Content("2"); }

        }

        public ActionResult ChangePass(string Token)
        {
            string json;
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://supectco.com/webs/shargh/checktokenforpassword.php?token=" + Token);
            }
            var log = JsonConvert.DeserializeObject<string>(json);
            if (log == "3")
            {
                return RedirectToAction("Error");
            }

            Session["email"] = log;
            return View();
        }

        public ContentResult SetNewPass(string pass)
        {
            string email = Session["email"] as string;
            string json;
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://supectco.com/webs/shargh/setnewpass.php?email=" + email + "&pass=" + pass);
            }
            var log = JsonConvert.DeserializeObject<string>(json);
            return Content(log);
        }
        public ActionResult LogOff()
        {
            Session["LogedInUser"] = null;
            Session["shoppingcartlist"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ContentResult SetOrChangeAddress(string address)
        {
            userdata user = Session["LogedInUser"] as userdata;
            string id = user.ID;
            string json;
            using (var client = new WebClient())
            {
                json = client.DownloadString("http://supectco.com/webs/shargh/setorchangeaddress.php?ID=" + id + "&address=" + address);
            }
            var log = JsonConvert.DeserializeObject<string>(json);
            if (log == "1")
            {
                user.address = address;
                Session["LogedInUser"] = user;
            }

            return Content(log);
        }

        


    }
}
