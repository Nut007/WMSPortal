using AutoMapper;
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

namespace WMSPortal.Controllers
{
    public class ApplicationRoleController : Controller
    {
        private IApplicationRoleRepository _applicationRoleRepository;
        public ApplicationRoleController(IApplicationRoleRepository applicationRoleRepository)
        {
            _applicationRoleRepository = applicationRoleRepository;
        }
        public JsonResult GetApplicationRoles(int roleId,ApplicationRoleViewModel model, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            IEnumerable<ApplicationRole> viewModel = _applicationRoleRepository.GetApplicationRoles(roleId);
            return Json(viewModel.ToDataSourceResult(dataSourceRequest, Mapper.Map<ApplicationRoleViewModel>), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateApplicationRoles([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ApplicationRoleViewModel> applicationRoles)
        {
            IEnumerable<ApplicationRole> appRoles = Mapper.Map<IEnumerable<ApplicationRoleViewModel>, IEnumerable<ApplicationRole>>(applicationRoles);
            _applicationRoleRepository.CreateOrUpdateApplicationRoles(appRoles);
            return Json(applicationRoles.ToDataSourceResult(request, ModelState));
        }
   

    }
}