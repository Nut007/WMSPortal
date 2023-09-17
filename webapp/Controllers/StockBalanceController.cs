using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using WMSPortal.Data.Repositories;
using WMSPortal.Core.Model;
using WMSPortal.ViewModels;
using System.Data.SqlClient;

namespace WMSPortal.Controllers
{
    public class StockBalanceController : Controller
    {
        private IStockBalanceRepository _stockBalanceRepository;
        public StockBalanceController(IStockBalanceRepository stockBalanceRepository)
        {
            _stockBalanceRepository = stockBalanceRepository;
        }
        
        public JsonResult GetInventory(List<string> columnvalues,string viewBy, [DataSourceRequest] DataSourceRequest dataSourceRequest,string userId)
        {
            try
            {
                int total = 0;
                
                IEnumerable<LotxLocxId> model = _stockBalanceRepository.GetInventory(columnvalues, viewBy, dataSourceRequest.PageSize, dataSourceRequest.Page, userId);
                if (model.Count() != 0)
                    total = model.First().TotalRows;
                var result = new DataSourceResult()
                {
                    Data = model, // Process data (paging and sorting applied)
                    Total = total// Total number of records
                };

                //var results = model.ToDataSourceResult(dataSourceRequest);
                var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            finally {
                Session["currentSkuFilter"] = string.Empty;
            }
        }
        public JsonResult GetInventoryGroupBySku(string column, string value, [DataSourceRequest] DataSourceRequest dataSourceRequest,string userId)
        {
            IEnumerable<LotxLocxId> model = _stockBalanceRepository.GetInventoryGroupBySku(column, value, userId);
            var results = model.ToDataSourceResult(dataSourceRequest);
            var jsonResult = Json(results, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetCommodityInfo(string packageNo, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            try
            {
                IEnumerable<LotxLocxId> model = _stockBalanceRepository.GetCommodityInfo(packageNo);
                var results = model.ToDataSourceResult(dataSourceRequest);
                var jsonResult = Json(results, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            finally
            {
                //
            }
        }
        public ActionResult StockBalanceList(string skuFilter)
        {
            ViewBag.SkuFilter = skuFilter;
            //if (!string.IsNullOrEmpty(skuFilter)) Session["currentSkuFilter"] = skuFilter;
            return View();
        }
        public ActionResult SelectStockBalance(string orderKey,string skuFilter)
        {
            ViewBag.SkuFilter = skuFilter;
            //if (!string.IsNullOrEmpty(skuFilter)) Session["currentSkuFilter"] = skuFilter;
            return View();
        }
        // GET: /StockBalance/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CommodityInfo()
        {
            return View();
        }
	}
}