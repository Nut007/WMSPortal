using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMSPortal.Core.Model;
using WMSPortal.Data.Repositories;
using System.Web.UI;
using WMSPortal.ViewModels;
using System.Data;

namespace WMSPortal.Controllers
{
    public class HelperController : Controller
    {
        
        private IHelperRepository _helperRepository;

        public HelperController(IHelperRepository helperRepository)
        {
            _helperRepository = helperRepository;
        }
        public JsonResult GetStorers()
        {
            IEnumerable<Storer> storers = _helperRepository.GetStorers();
            return Json(storers, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetResultGrid()
        {
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            IEnumerable<Storer> storers = _helperRepository.GetStorers();
            DataTable dt = converter.ToDataTable(storers.ToList());
            //DataTable dt = new DataTable();
            return PartialView("_ResultGrid", dt);
        }
        public ActionResult GetStorers2([DataSourceRequest]DataSourceRequest request)   
        {
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            IEnumerable<Storer> storers = _helperRepository.GetStorers();
            DataTable dt = converter.ToDataTable(storers.ToList());
            return Json(dt.ToDataSourceResult(request));
        }
        public ActionResult ExecuteReport([DataSourceRequest]DataSourceRequest request, List<StoreProcedure> parameters,string storeprocedureName)
        {
            var results = _helperRepository.GetReportResult(parameters, storeprocedureName);
            var jsonResult = Json(results.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        //public ActionResult ExecuteReport([DataSourceRequest]DataSourceRequest request,List<StoreProcedure> parameters)
        //{
        //    var results =  _helperRepository.GetReportResult(parameters);
        //    return Json(results.ToDataSourceResult(request));
        //}
        
        public JsonResult GetStoreProcedureReport()
        {
            IEnumerable<StoreProcedure> storeProcedures = _helperRepository.GetStoreProcedureReport();
            storeProcedures = storeProcedures.Where(x => x.PROCEDURE_NAME.Contains("RPT_"));
            return Json(storeProcedures, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStoreProcedureColumns([DataSourceRequest] DataSourceRequest request, string storeProcedureName)
        {
            IEnumerable<StoreProcedure> storeColumns = _helperRepository.GetStoreProcedureColumns(storeProcedureName).Where(x=>x.COLUMN_NAME != "@RETURN_VALUE");
            return Json(storeColumns.ToDataSourceResult(request));
        }
        //public JsonResult GetStoreProcedureColumns (string storeProcedureName)
        //{GetStoreProcedureReport
        //    IEnumerable<StoreProcedure> storeColumns = _helperRepository.GetStoreProcedureColumns(storeProcedureName);
        //    return Json(storeColumns, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetDeclarationInboundType()
        {
            IEnumerable<Codelkup> decType = _helperRepository.GetDeclarationInboundType();
            return Json(decType, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDeclarationOutboundType()
        {
            IEnumerable<Codelkup> decType = _helperRepository.GetDeclarationOutboundType();
            return Json(decType, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDeclarationType()
        {
            IEnumerable<Codelkup> decType = _helperRepository.GetDeclarationType();
            return Json(decType, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMawbList()
        {
            IEnumerable<Codelkup> mawbList = _helperRepository.GetMawbList();
            return this.Json(mawbList, JsonRequestBehavior.AllowGet);
        }
    }
}