using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Collections.Specialized;

namespace scarfproject.Controllers
{
    public class DemoController : Controller
    {
        //
        // GET: /Demo/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult info855888()
        {
            return View();
        }
        [HttpPost]
        public ActionResult info855888 (mydemo.question.questionmodel  mymodel)
        {
            string result = "";
            using (WebClient client = new WebClient())
            {

                var collection = new NameValueCollection();
                collection.Add("age", mymodel.age);
                collection.Add("education", mymodel.education);
                collection.Add("height", mymodel.height);
                collection.Add("wieght", mymodel.height);
                collection.Add("job", mymodel.job);
                collection.Add("phone", mymodel.phone);
                collection.Add("jobsite", mymodel.jobsite);
                collection.Add("question1", mymodel.question1);
                collection.Add("question2", mymodel.question2);
                collection.Add("question3", mymodel.question3);
                collection.Add("question4", mymodel.question4);
                collection.Add("question5", mymodel.question5);
                collection.Add("question6", mymodel.question6);
                collection.Add("question7", mymodel.question7);
                collection.Add("question8", mymodel.question8);
                collection.Add("question9", mymodel.question9);
                collection.Add("question10", mymodel.question10);
                collection.Add("question11", mymodel.question11);
                collection.Add("question12", mymodel.question12);
                collection.Add("question13", mymodel.question13);
                collection.Add("question14", mymodel.question14);
                collection.Add("question15", mymodel.question15);
                collection.Add("question15", mymodel.question16);
                collection.Add("question15", mymodel.question17);
                collection.Add("question15", mymodel.question18);
                collection.Add("question15", mymodel.question19);
              


                //foreach (var myvalucollection in imaglist) {
                //    collection.Add("imaglist[]", myvalucollection);
                //}
                byte[] response =
                client.UploadValues("http://supectco.com/webs/demo/question/setanswer.php", collection);

                result = System.Text.Encoding.UTF8.GetString(response);
            }
            ViewBag.message = result;
            return View();
        }


    }
}
