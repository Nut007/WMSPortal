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
    public class RoleController : Controller
    {
        private IRoleRepository _roleRepository;
        private IApplicationRoleRepository _applicationRoleRepository;
        public RoleController(IRoleRepository roleRepository, IApplicationRoleRepository applicationRoleRepository)
        {
            _roleRepository = roleRepository;
            _applicationRoleRepository = applicationRoleRepository;
        }
        public JsonResult GetAllRoles([DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            IEnumerable<Role> roles = _roleRepository.GetRoleList();
            
            var results = roles.ToDataSourceResult(dataSourceRequest);
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRole([DataSourceRequest] DataSourceRequest dataSourceRequest, RoleListViewModel roleListViewModel)
        {
            Role role = Mapper.Map<RoleListViewModel, Role>(roleListViewModel);
            //_roleRepository.UpdateRole(role);
            var resultData = new[] { Mapper.Map<Role, RoleListViewModel>(role) };
            return Json(resultData.AsQueryable().ToDataSourceResult(dataSourceRequest, ModelState));
        }
        public JsonResult CreateRole([DataSourceRequest] DataSourceRequest dataSourceRequest, RoleListViewModel roleListViewModel)
        {
            Role model = Mapper.Map<RoleListViewModel, Role>(roleListViewModel);
            _roleRepository.CreateRole(model);
            var resultData = new[] { Mapper.Map<Role, RoleListViewModel>(model) };
            return Json(resultData.AsQueryable().ToDataSourceResult(dataSourceRequest, ModelState));
        }
        public JsonResult DeleteRoles([DataSourceRequest] DataSourceRequest dataSourceRequest, RoleListViewModel roleListViewModel, List<int> roleIDs)
        {
            _roleRepository.DeleteRoles(roleIDs);
            var resultData = new[] { roleListViewModel };
            return Json(resultData.AsQueryable().ToDataSourceResult(dataSourceRequest, ModelState));
        }
        public JsonResult RefreshRole([DataSourceRequest] DataSourceRequest dataSourceRequest, RoleListViewModel roleListViewModel)
        {
            Role model = Mapper.Map<RoleListViewModel, Role>(roleListViewModel);
            var resultData = new[] { Mapper.Map<Role, RoleListViewModel>(model) };
            return Json(resultData.AsQueryable().ToDataSourceResult(dataSourceRequest, ModelState));
        }
        public JsonResult GetRoles(string column, string value, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            IEnumerable<Role> roles=_roleRepository.GetRoles(column, value);
            var results = roles.ToDataSourceResult(dataSourceRequest);
            return Json(roles, JsonRequestBehavior.AllowGet);
        }
        public ActionResult _RoleListViewModel(int Id)
        {
            RoleListViewModel model = new RoleListViewModel();
            model.ApplicationRoles = _applicationRoleRepository.GetApplicationRoles(Id);
            return PartialView(model);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateOrUpdateRole([DataSourceRequest] DataSourceRequest dataSourceRequest, RoleListViewModel role)
        {
            Role appRoles = Mapper.Map<RoleListViewModel, Role>(role);
            _roleRepository.CreateOrUpdateRole(appRoles);
            var resultData = new[] { Mapper.Map<Role, RoleListViewModel>(appRoles) };
            return Json(resultData.AsQueryable().ToDataSourceResult(dataSourceRequest, ModelState));
        }
        public ActionResult RoleList()
        {
            return View();
        }
     
	}
}