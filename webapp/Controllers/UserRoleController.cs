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
    public class UserRoleController : Controller
    {
        private IUserRoleRepository _userRoleRepository;
        public UserRoleController(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }
        public JsonResult GetUserRoles(int userId,UserRoleViewModel model, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            IEnumerable<UserRole> viewModel = _userRoleRepository.GetUserRoles(userId);
            return Json(viewModel.ToDataSourceResult(dataSourceRequest, Mapper.Map<UserRoleViewModel>), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateUserRoles([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<UserRoleViewModel> userRoles)
        {
            IEnumerable<UserRole> usrRoles = Mapper.Map<IEnumerable<UserRoleViewModel>, IEnumerable<UserRole>>(userRoles);
            _userRoleRepository.CreateOrUpdateUserRoles(usrRoles);
            return Json(userRoles.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Dashboard()
        {
            DashboardViewModel view = new DashboardViewModel();
            view.DepositStatistic = "1300, 1877, 2500, 2577, 2000, 2100, 3000, 2700, 3631, 2471, 2700, 3631, 2471";
            return View(view);
        }
	}
}