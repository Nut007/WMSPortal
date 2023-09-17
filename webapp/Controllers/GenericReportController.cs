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

#endregion

namespace WMSPortal.Controllers
{
    [Authorize]
    public class GenericReportController : Controller
    {
        private IUserLogRepository _userLogRepository;
        private UserLog userLog;
        private IUserRepository _userRepository;
        public ICacheProvider Cache { get; set; }
        public GenericReportController(ICacheProvider cacheProvider, IUserRepository userRepository, IUserLogRepository userLogRepository)
        {
            _userRepository = userRepository;
            _userLogRepository = userLogRepository;
            this.Cache = cacheProvider;

            userLog = new UserLog();
            if (System.Web.HttpContext.Current.Session["userRoles"] == null)
                userLog.LoginName = string.Empty;
            else
                userLog.LoginName = ((User)System.Web.HttpContext.Current.Session["userRoles"]).UserName;

        }
        public ActionResult GenericQueryReport()
        {

            GeneralQueryReportViewModel model = new GeneralQueryReportViewModel();
           
            for (int i = 0; i < 10; i++)
            {
                ObjectControl obj = new ObjectControl();
                obj.ControlType = "text";
                obj.Name = string.Format("text{0}",i.ToString());
                obj.Caption = string.Format("text{0}", i.ToString());
                model.FilterCollection.Add(obj);
            }
           

            ObjectControl obj1= new ObjectControl();
            obj1.ControlType = "select";
            obj1.Name = "select1";
            obj1.Caption = "select";
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "1111", Value = "1" });
            items.Add(new SelectListItem() { Text = "222", Value = "2" });
            items.Add(new SelectListItem() { Text = "333", Value = "3" });
            items.Add(new SelectListItem() { Text = "444", Value = "4" });
            obj1.Options.AddRange(items);

            ObjectControl obj2 = new ObjectControl();
            obj2.ControlType = "date";
            obj2.Name = "date1";
            obj2.Caption = "date1";

            ObjectControl obj3 = new ObjectControl();
            obj3.ControlType = "datetime";
            obj3.Name = "datetime1";
            obj3.Caption = "datetime1";

         

            model.FilterCollection.Add(obj2);
            model.FilterCollection.Add(obj1);
            model.FilterCollection.Add(obj3);
 
            return View(model);
        }
        
    }
}