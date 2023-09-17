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
using System.Data.SqlClient;

namespace WMSPortal.Controllers
{
    public class CodeLookupController : Controller
    {
        private ICodelkupRepository _codeLookupRepository;
        public CodeLookupController(ICodelkupRepository codeLookupRepository)
        {
            _codeLookupRepository = codeLookupRepository;
        }
       
        public JsonResult GetAllCodeLookups([DataSourceRequest] DataSourceRequest dataSourceRequest, string lookupGroupCode)
        {
            LookupType m = (LookupType)Enum.Parse(typeof(LookupType), lookupGroupCode, true);
            IEnumerable<Codelkup> lookups = _codeLookupRepository.GetLookupListByType(m);
            var results = lookups.ToDataSourceResult(dataSourceRequest);
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateCodeLookup([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")]IEnumerable<CodeLookupViewModel> models, string lookupGroupCode)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    Codelkup lookup = Mapper.Map<CodeLookupViewModel, Codelkup>(model);
                    _codeLookupRepository.UpdateLookup(lookup);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateCodeLookup([DataSourceRequest] DataSourceRequest request,
          [Bind(Prefix = "models")]IEnumerable<CodeLookupViewModel> models, string lookupGroupCode)
        {
            var results = new List<CodeLookupViewModel>();
            User user = (User)System.Web.HttpContext.Current.Session["userRoles"];
                   
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    model.LISTNAME = lookupGroupCode;
                    model.Code = model.Description;
                    model.AddDate = (DateTime?)DateTime.Now;
                    model.EditDate = (DateTime?)DateTime.Now;
                    model.AddWho = user.UserName;
                    model.EditWho = user.UserName;
                    Codelkup lookup = Mapper.Map<CodeLookupViewModel, Codelkup>(model);
                    _codeLookupRepository.InsertLookup(lookup);
                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
        public JsonResult DeleteLookupItems([DataSourceRequest] DataSourceRequest dataSourceRequest, string[] codeLookupIdList, string codeLookupGroup)
        {
            try
            {
                var model = new CodeLookupViewModel();
                List<string> lst = new List<string>();
                lst.AddRange(codeLookupIdList);
                bool ret = _codeLookupRepository.DeleteLookupItems(lst.ToList(), codeLookupGroup);
                var resultData = new[] { Mapper.Map<CodeLookupViewModel, Codelkup>(model) };
                return Json(resultData.AsQueryable().ToDataSourceResult(dataSourceRequest, ModelState));
            }
            catch (SqlException sqlEx)
            {
                return Json(new
                {
                    isValid = false,
                    exceptionErrorMessage = sqlEx.Message,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isValid = false,
                    exceptionErrorMessage = ex.Message,
                });
            }

        }
        public JsonResult RefreshCodeLookup([DataSourceRequest] DataSourceRequest dataSourceRequest, CodeLookupViewModel codeLookupListViewModel)
        {
            Codelkup model = Mapper.Map<CodeLookupViewModel, Codelkup>(codeLookupListViewModel);
            var resultData = new[] { Mapper.Map<Codelkup, CodeLookupViewModel>(model) };
            return Json(resultData.AsQueryable().ToDataSourceResult(dataSourceRequest, ModelState));
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ManageCodelookup()
        {
            return View();
        }
        
    }
}