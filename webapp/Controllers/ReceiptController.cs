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
using System.Collections;
using System.IO;

namespace WMSPortal.Controllers
{
    [SessionExpire]
    public class ReceiptController : Controller
    {
        private IReceiptRepository _receiptRepository;
        private ICodelkupRepository _codeLookupRepository;
        public ReceiptController(IReceiptRepository receiptRepository, ICodelkupRepository codeLookupRepository)
        {
            _receiptRepository = receiptRepository;
            _codeLookupRepository = codeLookupRepository;
        }
        [HttpPost]
        public JsonResult GetInboundShipment(string column, string value1, string value2, [DataSourceRequest] DataSourceRequest dataSourceRequest, string userId)
        {
            IEnumerable<ReceiptDetail> transections = _receiptRepository.GetInboundShipment(column, value1, value2, 1, userId);
            var results = transections.ToDataSourceResult(dataSourceRequest);
            var jsonResult = Json(results, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpPost]
        public JsonResult GetBaggageList(string column, string value1, string value2, [DataSourceRequest] DataSourceRequest dataSourceRequest, string userId, string status)
        {
            IEnumerable<TEMP_ID> transections = _receiptRepository.GetBaggageList(column, value1, value2, 1, userId, status);
            var results = transections.ToDataSourceResult(dataSourceRequest);
            var jsonResult = Json(results, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpPost]
        public JsonResult GetMawbBaggages(string mawb, string containerno,[DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            IEnumerable<TEMP_ID> transections = _receiptRepository.GetMawbBaggages(mawb, containerno);
            var results = transections.ToDataSourceResult(dataSourceRequest);
            var jsonResult = Json(results, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpPost]
        public JsonResult AddBaggage(string tagNo, string uldNo,string mawb,string type,[DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            try
            {
                if (tagNo.Length != 29)
                {
                    return Json(new
                    {
                        isValid = false,
                        exceptionErrorMessage = "BAGGAGE NO wrong format.it must be 29 chars.",
                    });
                }
                TEMP_ID item = new TEMP_ID()
                {
                    ID = tagNo,
                    PCNO = uldNo,
                    SKU = string.Empty,
                    TYPE = type,
                    ID_OPTION = string.Empty,
                    COLOR = string.Empty,
                    PACKTYPE = string.Empty,
                    PACKUNIT = "1",
                    VANPLANT = string.Empty,
                    VANSIZE = string.Empty,
                    SITE = string.Empty,
                    MASTER_ID = string.Empty,
                    MAWB = mawb,
                    LOT = string.Empty,
                    EXPECTED_DATE = null,
                    ORIGIN_COUNTRY = tagNo.Substring(0, 2),
                    ORIGIN_PORT = tagNo.Substring(2, 3),
                    DESTINATION_COUNTRY = tagNo.Substring(6, 2),
                    DESTINATION_PORT = tagNo.Substring(8, 3),
                    ORIGIN_POST_CODE = tagNo.Substring(0, 6),
                    DESTINATION_POST_CODE = tagNo.Substring(6, 6),
                    DESPATCH_NO = tagNo.Substring(16, 4),
                    RECEPTACLE_NO = tagNo.Substring(20, 3),
                    SERVICE_TYPE = string.Format("00{0}", tagNo.Substring(13, 2)),
                    GROSS_WEIGHT = Convert.ToDecimal(string.Format("{0}.{1}", tagNo.Substring(25, 3), tagNo.Substring(28, 1))),
                    EDIT_DATE = DateTime.Now,
                    SHIPMENT_STATUS = (uldNo == string.Empty ? "2" : (type== "MANIFESTNOVALIDATION" ? "1":"3"))
                };
                var results = _receiptRepository.AddBaggage(item);
                var jsonResult = Json(results, JsonRequestBehavior.AllowGet);
                return jsonResult;
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.ErrorCode == -2146232060)
                {
                    return Json(new
                    {
                        isValid = false,
                        exceptionErrorMessage = string.Format("[ErrorCode-{0}]-Id '{1}' already exist.", sqlEx.ErrorCode, tagNo),
                    });
                }
                else
                {
                    return Json(new
                    {
                        isValid = false,
                        exceptionErrorMessage = string.Format("[ErrorCode-{0}]-{1}", sqlEx.ErrorCode, sqlEx.Message),
                    });
                }
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
        [HttpPost]
        public JsonResult DeleteBaggage(string baggageNo, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            try
            {
                var results = _receiptRepository.DeleteBaggage(baggageNo);
                var jsonResult = Json(results, JsonRequestBehavior.AllowGet);
                return jsonResult;
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
        [HttpPost]
        public JsonResult UnManifestBaggage(string[] bagId, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            try
            {
                var results = _receiptRepository.UnManifestItems(bagId);
                var jsonResult = Json(results, JsonRequestBehavior.AllowGet);
                return jsonResult;
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
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteBaggageItems([DataSourceRequest] DataSourceRequest dataSourceRequest, string[] bagId,string type)
        {
            try
            {
                var model = new ReceiptViewModel();
                List<string> lst = new List<string>();
                lst.AddRange(bagId);
                bool ret = _receiptRepository.DeleteBaggageItems(lst, type);
                var resultData = new[] { Mapper.Map<ReceiptViewModel, Receipt>(model) };
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
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddBaggageItems([DataSourceRequest] DataSourceRequest dataSourceRequest, string[] bagId,string uldNo,string mawb)
        {
            try
            {
                var model = new ReceiptViewModel();
                List<string> lst = new List<string>();
                lst.AddRange(bagId);
                bool ret = _receiptRepository.AddBaggageItems(bagId, mawb, uldNo);
                var resultData = new[] { Mapper.Map<ReceiptViewModel, Receipt>(model) };
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
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteCN38Items([DataSourceRequest] DataSourceRequest dataSourceRequest, string mawb,string[] cn38)
        {
            try
            {
                var model = new ReceiptViewModel();
                List<string> lst = new List<string>();
                lst.AddRange(cn38);
                bool ret = _receiptRepository.DeleteCN38Items(mawb, lst);
                var resultData = new[] { Mapper.Map<ReceiptViewModel, Receipt>(model) };
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
        public ActionResult ReceiptInfo(string receiptKey, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            Receipt item = _receiptRepository.GetReceipt(receiptKey);
            ReceiptViewModel model = Mapper.Map<Receipt, ReceiptViewModel>(item);
            return View(model);
        }
        public JsonResult GetReceiptDetail(string receiptKey, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            IEnumerable<ReceiptDetail> items = _receiptRepository.GetReceiptDetail(receiptKey);
            var results = items.ToDataSourceResult(dataSourceRequest);
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SaveBaggage([DataSourceRequest] DataSourceRequest dataSourceRequest, BaggageListViewModel baggageViewModel)
        {
            try
            {
                TEMP_ID model = Mapper.Map<BaggageListViewModel, TEMP_ID>(baggageViewModel);
                //model.ID = string.Format("{0}{1}{2}{3}", "MAN",model.DESTINATION_PORT, model.DESPATCH_NO, model.RECEPTACLE_NO);
                string tagNo = model.ID;
                model.ORIGIN_COUNTRY = tagNo.Substring(0, 2);
                model.ORIGIN_PORT = tagNo.Substring(2, 3);
                model.DESTINATION_COUNTRY = tagNo.Substring(6, 2);
                model.DESTINATION_PORT = tagNo.Substring(8, 3);
                model.ORIGIN_POST_CODE = tagNo.Substring(0, 6);
                model.DESTINATION_POST_CODE = tagNo.Substring(6, 6);
                model.DESPATCH_NO = tagNo.Substring(16, 4);
                model.RECEPTACLE_NO = tagNo.Substring(20, 3);
                model.SERVICE_TYPE = string.Format("00{0}", tagNo.Substring(13, 2));
                model.GROSS_WEIGHT = Convert.ToDecimal(string.Format("{0}.{1}", tagNo.Substring(25, 3), tagNo.Substring(28, 1)));
                model.EDIT_DATE = DateTime.Now;
                model.SHIPMENT_STATUS = "2";
                model.LOT = model.CN38;
                model.EDIT_DATE = DateTime.Now;
                model.RECEPTACLE_NO = Convert.ToInt16(model.RECEPTACLE_NO).ToString("000");
                TEMP_ID tempId =_receiptRepository.AddBaggage(model);
                var resultData = new[] { Mapper.Map<TEMP_ID, BaggageListViewModel>(model) };
                if (tempId.ID == string.Empty)
                {
                    return Json(new
                    {
                        isValid = false,
                        exceptionErrorMessage = string.Format("Error: Unable to save. Check to make sure you have scan baggage Id '{0}' ", model.DESPATCH_NO),
                    });
                }
                else
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void UploadPreAlert()
        {

        }
        //
        // GET: /Receipt/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ReceiptList()
        {
            return View();
        }
        public ActionResult BaggageList()
        {
            return View();
        }
        public ActionResult BaggageScreening()
        {
            return View();
        }
        public ActionResult AddNewBaggage(string mawb,string containerno)
        {
            ViewBag.Mawb = mawb;
            ViewBag.ContainerNo = containerno;
            return View();
        }
        public ActionResult AddCN38(string mawb)
        {
            ViewBag.Mawb = mawb;
            return View();
        }
        public ActionResult UploadUpliftReport()
        {
            return View();
        }
        public ActionResult BaggageManagment()
        {
            return View();
        }

        public ActionResult SaveUpliftReport(IEnumerable<HttpPostedFileBase> files)
        {
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(file.FileName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    // The files are not actually saved in this demo
                    // file.SaveAs(physicalPath);
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult RemoveUpliftReport(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        // System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        [HttpPost]
        public JsonResult AddCN38Items(string mawb, string[] bagId, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            try
            {
                string invalidBagId =string.Empty;
                string  originPort = _codeLookupRepository.GetShortDescripion (LookupType.MAWB, mawb);
                if (string.IsNullOrEmpty(originPort))
                {
                    return Json(new
                    {
                        isValid = false,
                        exceptionErrorMessage = string.Format("[ErrorCode-{0}]-{1}", "ERR01", "There is no origin port.Please provide them before pre-booking"),
                    });
                }
                else
                {
                    IEnumerable<Codelkup> codes = _codeLookupRepository.GetLookupListByIMPC();
                    if (codes.Count() > 0)
                    {
                        foreach (var bag in bagId)
                        {
                            int ret = codes.Where(x => x.Code == bag.Substring(0, 6) && x.Description.UnNull().Trim()== originPort.UnNull().Trim()).Count();
                            if (ret == 0)
                            {
                                invalidBagId += Environment.NewLine + bag;
                            }
                        }
                    }
                    if (invalidBagId != string.Empty)
                    {
                        return Json(new
                        {
                            isValid = false,
                            exceptionErrorMessage = string.Format("[ErrorCode-{0}]-{1}", "ERR02", "There are invalid CN35." + Environment.NewLine + invalidBagId),
                        });
                    }
                    else
                    {
                        var results = _receiptRepository.AddCN38Items(mawb, bagId.ToList());
                        var jsonResult = Json(results, JsonRequestBehavior.AllowGet);
                        return jsonResult;
                    }
                }
               
            }
            catch (SqlException sqlEx)
            {
                {
                    return Json(new
                    {
                        isValid = false,
                        exceptionErrorMessage = string.Format("[ErrorCode-{0}]-{1}", sqlEx.ErrorCode, sqlEx.Message),
                    });
                }
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

        [HttpPost]
        public JsonResult GetMawbItems(string mawb, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            IEnumerable<TEMP_ID> transections = _receiptRepository.GetMawbItems(mawb);
            var results = transections.ToDataSourceResult(dataSourceRequest);
            var jsonResult = Json(results, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpPost]
        public JsonResult GetCN38Items(string cn38, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            IEnumerable<TEMP_ID> transections = _receiptRepository.GetCN38Items(cn38);
            var results = transections.ToDataSourceResult(dataSourceRequest);
            var jsonResult = Json(results, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        [HttpPost]
        public JsonResult GetMissingCN35(string mawb, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            IEnumerable<TEMP_ID> transections = _receiptRepository.GetMissingCN35(mawb);
            var results = transections.ToDataSourceResult(dataSourceRequest);
            var jsonResult = Json(results, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        
    }
}