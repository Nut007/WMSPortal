#region Using

using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WMSPortal.ViewModels;
using WMSPortal.Data.Repositories;
using Autofac;
using WMSPortal.Data;
using WMSPortal.Core.Model;
using System.Collections.Generic;
using System.Data;
using AutoMapper;


#endregion

namespace WMSPortal.Controllers
{
  
    public class HomeController : Controller
    {
        private readonly IOrdersRepository _ordersRepository;
        private IUserRepository _userRepository;
        public class Det
        {
            public int Id;
            public String Name;
            public decimal yVal;
        }
        public class OrdersWeekly
        {
            public DateTime OrderDate;
            public Int64 TotalOrders;
        }
        public ICacheProvider Cache { get; set; }
       
        // GET: home/index
        public HomeController(ICacheProvider cacheProvider, IOrdersRepository ordersRepository, IUserRepository userRepository)
        {
            this.Cache = cacheProvider;
            _ordersRepository = ordersRepository;
            _userRepository = userRepository;
        }
        public ActionResult Keepalive()
        {
            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChangeConnection(string connectionId)
        {
            //var currentUrl = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
            var currentUrl = this.Request.Url;
            var user  = ((User)System.Web.HttpContext.Current.Session["userRoles"]);
          
            Role roleConnection = user.Roles.Where(s => s.Id == Convert.ToInt16(connectionId)).SingleOrDefault();

            Cache.Invalidate("RoleConnection");
            Cache.Set("RoleConnection", roleConnection);

            return Json(new { Url = currentUrl, status = "OK" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return View();
        }
        public ActionResult ActivitiesDashboard()
        {
            OrdersDashboard item = _ordersRepository.GetOrdersPerfomance();
            OrdersDashboardViewModel model = new OrdersDashboardViewModel();

            model.WorkerPerfomanceWeekly = item.WorkerPerfomanceWeekly;
            model.LocationUtilization = item.LocationUtilization;
            model.ReceiptPerfomance = item.ReceiptPerfomance;
            model.TotalBin = item.TotalBin;
            model.BinUsage = item.BinUsage;
            model.AvaliableBin = item.TotalBin - item.BinUsage;
            model.AvaliableBinPercent = Math.Ceiling(Convert.ToDecimal(Convert.ToDecimal(item.BinUsage) / Convert.ToDecimal(item.TotalBin)) * 100);
            var lastReceived = model.ReceiptPerfomance.LastOrDefault();
            if (lastReceived != null)
            {
                model.TodayExpected = lastReceived.QtyExpected;
                model.TodayReceived = lastReceived.QtyReceived;
                model.TodayReceivedPercent = Convert.ToString(Math.Ceiling(Convert.ToDecimal(Convert.ToDecimal(lastReceived.QtyReceived) / Convert.ToDecimal(lastReceived.QtyExpected)) * 100)) + "%";
            }
            else
            {
                model.TodayExpected = 0;
                model.TodayReceived = 0;
                model.TodayReceivedPercent = "0%";
            }
          
            return View(model);
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AnalyticsReport()
        {
            return View();
        }
        [HttpGet]
        public JsonResult NewChart()
        {
            List<object> iData = new List<object>();
            //Creating sample data  
            DataTable dt = new DataTable();
            dt.Columns.Add("Employee", System.Type.GetType("System.String"));
            dt.Columns.Add("Credit", System.Type.GetType("System.Int32"));

            DataRow dr = dt.NewRow();
            dr["Employee"] = "Sam";
            dr["Credit"] = 123;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Employee"] = "Alex";
            dr["Credit"] = 456;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Employee"] = "Michael";
            dr["Credit"] = 587;
            dt.Rows.Add(dr);
            //Looping and extracting each DataColumn to List<Object>  
            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                iData.Add(x);
            }
            //Source data returned as JSON  
            return Json(iData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult NewFlotChart()
        {
            List<object> iData = new List<object>();
            //Creating sample data  
            DataTable dt = new DataTable();
            dt.Columns.Add("Employee", System.Type.GetType("System.String"));
            dt.Columns.Add("Credit", System.Type.GetType("System.Int32"));

            DataRow dr = dt.NewRow();
            dr["Employee"] = "Sam";
            dr["Credit"] = 123;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Employee"] = "Alex";
            dr["Credit"] = 456;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Employee"] = "Michael";
            dr["Credit"] = 587;
            dt.Rows.Add(dr);
            //Looping and extracting each DataColumn to List<Object>  
            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                iData.Add(x);
            }
            //Source data returned as JSON  
            return Json(iData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult OrderWeeklyChart()
        {
            List<OrdersWeekly> items = new List<OrdersWeekly>();
           
            for (int i = 1; i < 30; i++)
            {
                OrdersWeekly item = new OrdersWeekly();
                item.OrderDate = DateTime.Now.AddDays(i);
                item.TotalOrders = 10 + 10 + i;
                items.Add(item);
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetBrandDonutChart()
        {
            List<Det> obj = new List<Det>();
            Det def = new Det();

            def.Id = 1;
            def.Name = "Lenovo";
            def.yVal = Convert.ToDecimal(78.45);
            obj.Add(def);

            def.Id = 2;
            def.Name = "LG";
            def.yVal = Convert.ToDecimal(47);
            obj.Add(def);

            def.Id = 3;
            def.Name = "Dell";
            def.yVal = Convert.ToDecimal(55);
            obj.Add(def);

            def.Id = 4;
            def.Name = "Samsung";
            def.yVal = Convert.ToDecimal(45);
            obj.Add(def);

            return Json(obj, JsonRequestBehavior.AllowGet);
        }  
        // GET: home/inbox
        public ActionResult Inbox()
        {
            return View();
        }

        // GET: home/calendar
        public ActionResult Calendar()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            ViewBag.PressureData = new double[] {
                936, 968, 1025, 999, 998, 1014, 1017, 1010, 1010, 1007,
                1004, 988, 990, 988, 987, 995, 946, 954, 991, 984,
                974, 956, 986, 936, 955, 1021, 1013, 1005, 958, 953,
                952, 940, 937, 980, 966, 965, 928, 916, 910, 980
            };

            ViewBag.TemperatureData = new double[] {
                16, 17, 18, 19, 20, 21, 21, 22, 23, 22,
                20, 18, 17, 17, 16, 16, 17, 18, 19, 20,
                21, 22, 23, 25, 24, 24, 22, 22, 23, 22,
                22, 21, 16, 15, 15, 16, 19, 20, 20, 21
            };

            ViewBag.HumidityData = new double[] {
                71, 70, 69, 68, 65, 60, 55, 55, 50, 52,
                73, 72, 72, 71, 68, 63, 57, 58, 53, 55,
                63, 59, 61, 64, 58, 53, 48, 48, 45, 45,
                63, 64, 63, 67, 58, 56, 53, 59, 51, 54
            };

            ViewBag.TemperatureRange = new double[] { 21, 23 };

            ViewBag.ACStats = new Dictionary<string, int[]>
            {
                {"mon", new int[] {14, 10}},
                {"tue", new int[] {8, 16}},
                {"wed", new int[] {8, 16}},
                {"thu", new int[] {12, 12}},
                {"fri", new int[] {6, 18}},
                {"sat", new int[] {1, 23}},
                {"sun", new int[] {5, 19}}
            };

            return View();
        }
        // GET: home/google-map
        public ActionResult GoogleMap()
        {
            return View();
        }
        // GET: home/widgets
        public ActionResult Widgets()
        {
            //[TEST] to initialize the theme setter
            //could be called via jQuery or somewhere...
            Settings.SetValue<string>("config:CurrentTheme", "smart-style-5");

            return View();
        }
        // GET: home/chat
        public ActionResult Chat()
        {
            return View();
        }
    }
}