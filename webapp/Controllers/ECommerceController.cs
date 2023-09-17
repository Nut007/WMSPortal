using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using WMSPortal.Core.Model;
using WMSPortal.Data.Repositories;
using WMSPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;

namespace WMSPortal.Controllers
{
    public class ECommerceController : Controller
    {
        private IQatarECommerceRepository _eCommerceRepository;
        private IApplicationRoleRepository _applicationRoleRepository;
        public ECommerceController(IQatarECommerceRepository eCommerceRepository, IApplicationRoleRepository applicationRoleRepository)
        {
            _eCommerceRepository = eCommerceRepository;
            _applicationRoleRepository = applicationRoleRepository;
        }
        public JsonResult GetConsignmentList(string issueStartDate, string issueStopDate, string column, string value,  string userId, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            IEnumerable<QatarECommerce> roles = _eCommerceRepository.GetConsignmentInfo(issueStartDate,issueStopDate,column,value,1,userId);

            var results = roles.ToDataSourceResult(dataSourceRequest);
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ConsignmentList()
        {
            return View();
        }
     
	}
}