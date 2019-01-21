using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Newtonsoft.Json;
using scarfproject.ViewModel;
using System.Web.Script.Serialization;
using scarfproject.Classes;
using System.IO;
using System.Text;
//using scarfproject.ServiceReference1;




namespace scarfproject.Controllers
{
    public class ErrorController : Controller
    {

        public ActionResult Error404()
        {

            return View();
        }
    }
}
