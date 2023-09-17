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
    public class UserController : Controller
    {
        private IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public JsonResult GetAllUsers([DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            IEnumerable<User> users = _userRepository.GetUserList();

            var results = users.ToDataSourceResult(dataSourceRequest);
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateUser([DataSourceRequest] DataSourceRequest dataSourceRequest, UserListViewModel userListViewModel)
        {
            User user = Mapper.Map<UserListViewModel, User>(userListViewModel);
            _userRepository.UpdateUser(user);
            var resultData = new[] { Mapper.Map<User, UserListViewModel>(user) };
            return Json(resultData.AsQueryable().ToDataSourceResult(dataSourceRequest, ModelState));
        }
        public JsonResult CreateUser([DataSourceRequest] DataSourceRequest dataSourceRequest, UserListViewModel userListViewModel)
        {
            User model = Mapper.Map<UserListViewModel, User>(userListViewModel);
            _userRepository.CreateUser(model);
            var resultData = new[] { Mapper.Map<User, UserListViewModel>(model) };
            return Json(resultData.AsQueryable().ToDataSourceResult(dataSourceRequest, ModelState));
        }
        public JsonResult DeleteUsers([DataSourceRequest] DataSourceRequest dataSourceRequest, UserListViewModel userListViewModel, List<int> userIDs)
        {
            _userRepository.DeleteUsers(userIDs);
            var resultData = new[] { userListViewModel };
            return Json(resultData.AsQueryable().ToDataSourceResult(dataSourceRequest, ModelState));
        }
       
        public JsonResult RefreshUser([DataSourceRequest] DataSourceRequest dataSourceRequest, UserListViewModel userListViewModel)
        {
            User model = Mapper.Map<UserListViewModel, User>(userListViewModel);
            var resultData = new[] { Mapper.Map<User, UserListViewModel>(model) };
            return Json(resultData.AsQueryable().ToDataSourceResult(dataSourceRequest, ModelState));
        }
        public JsonResult GetUsers(string column, string value, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            IEnumerable<User> users = _userRepository.GetUsers(column, value);
            var results = users.ToDataSourceResult(dataSourceRequest);
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRolesByUser(string userName, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            IEnumerable<Role> roles = _userRepository.GetRolesByUser(userName);
            var results = roles.ToDataSourceResult(dataSourceRequest);
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateOrUpdateUser([DataSourceRequest] DataSourceRequest dataSourceRequest, UserListViewModel user)
        {
            User userRoles = Mapper.Map<UserListViewModel, User>(user);
            _userRepository.CreateOrUpdateUser(userRoles);
            var resultData = new[] { Mapper.Map<User, UserListViewModel>(userRoles) };
            return Json(resultData.AsQueryable().ToDataSourceResult(dataSourceRequest, ModelState));
        }

        public ActionResult UserList()
        {
            return View();
        }
	}
}