using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using System.Web.Script.Serialization;
using WMSPortal.Core;
using WMSPortal.Core.Model;
using WMSPortal.Data;
using WMSPortal.Data.Repositories;
using WMSPortal.Helpers;

namespace WMSPortal.Controllers
{
    public class UploaderController : Controller
    {
        private IReceiptRepository _receiptRepository;
        private ICodelkupRepository _codelkupRepository;
        private AttachmentRepository attachmentManager = new AttachmentRepository();
        public UploaderController(IReceiptRepository receiptRepository, ICodelkupRepository codelkupRepository)
        {
            _receiptRepository = receiptRepository;
            _codelkupRepository = codelkupRepository;
        }
        // GET: Uploader
        public ActionResult Index()
        {
            return View();
        }
        // GET: /Uploader/Upload
        public ActionResult Upload()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView("UploadSingle");
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }
        [HttpPost]
        public JsonResult Upload(HttpPostedFileBase uploadedFile,string type)
        {
            int rowIterator = 0;
            try
            {
                List<TEMP_ID> items = new List<TEMP_ID>();
                if (uploadedFile != null && uploadedFile.ContentLength > 0)
                {
                    using (var package = new ExcelPackage(uploadedFile.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfColumns = workSheet.Dimension.End.Column;
                        var noOfRows = workSheet.Dimension.End.Row+1;

                        for (rowIterator = 2; rowIterator < noOfRows; rowIterator++)
                        {
                            //if (workSheet.Cells[rowIterator, 3].Value.ToString() == "6215")
                            //    Console.Write(workSheet.Cells[rowIterator, 3].Value.ToString());

                            TEMP_ID item = new TEMP_ID();
                            if (workSheet.Cells[rowIterator, 1].Value!=null)
                            {
                                if (type == "MANUALUPLOAD")
                                {
                                    if (workSheet.Cells[rowIterator, 4].Value == null)
                                        item.RECEPTACLE_NO = "001";
                                    else
                                        item.RECEPTACLE_NO = Convert.ToDecimal(workSheet.Cells[rowIterator, 6].Value).ToString("000");

                                    item.ID = workSheet.Cells[rowIterator, 3].Value.ToString();
                                    item.ISSUED_DATE = DateTime.FromOADate(Convert.ToDouble(workSheet.Cells[rowIterator, 10].Value)).ToString("yyyyMMdd");
                                    item.LOT = workSheet.Cells[rowIterator,8].Value.ToString();
                                    item.DESPATCH_NO = Convert.ToDecimal(workSheet.Cells[rowIterator, 5].Value.ToString()).ToString("0000");
                                    item.SHIPMENT_STATUS = "1";
                                    item.SERVICE_TYPE = workSheet.Cells[rowIterator, 3].Value.ToString().Substring(13,2);
                                    item.DESTINATION_PORT = workSheet.Cells[rowIterator, 4].Value.ToString();
                                    item.GROSS_WEIGHT = Convert.ToDecimal(workSheet.Cells[rowIterator, 7].Value.ToString());
                                    item.EXPECTED_DATE = null;
                                    item.ID_OPTION = string.Empty;
                                    item.MAWB = workSheet.Cells[rowIterator, 9].Value.ToString();
                                    item.PACKUNIT = "1";
                                    item.PCNO = workSheet.Cells[rowIterator, 2].Value.ToString();
                                    items.Add(item);
                                }
                                else
                                {

                                    string receiptLine = string.Empty;
                                    if (workSheet.Cells[1, 2].Value.ToString() == "CN38")
                                    {
                                        if (workSheet.Cells[rowIterator, 8].Value == null)
                                            item.RECEPTACLE_NO = "001";
                                        else
                                            item.RECEPTACLE_NO = Convert.ToDecimal(workSheet.Cells[rowIterator, 8].Value).ToString("000");

                                        item.ID = workSheet.Cells[rowIterator, 4].Value.ToString();
                                        item.ISSUED_DATE = Convert.ToDateTime(workSheet.Cells[rowIterator, 3].Value).ToString("yyyyMMdd");
                                        item.LOT = workSheet.Cells[rowIterator, 2].Value.ToString();
                                        item.DESPATCH_NO = Convert.ToDecimal(workSheet.Cells[rowIterator, 7].Value.ToString()).ToString("0000");
                                        item.SHIPMENT_STATUS = "1";
                                        item.SERVICE_TYPE = workSheet.Cells[rowIterator, 9].Value.ToString();
                                        item.DESTINATION_PORT = workSheet.Cells[rowIterator, 11].Value.ToString();
                                        item.GROSS_WEIGHT = Convert.ToDecimal(workSheet.Cells[rowIterator, 14].Value.ToString());
                                        item.EXPECTED_DATE = null;
                                        item.ID_OPTION = workSheet.Cells[rowIterator, 12].Value.UnNull();
                                        item.MASTER_ID = string.Empty;
                                        item.PACKUNIT = "1";
                                        items.Add(item);
                                    }
                                    else
                                    {
                                        string[] issueDate = workSheet.Cells[rowIterator, 2].Value.ToString().Split('/');
                                        if (workSheet.Cells[rowIterator, 4].Value == null)
                                            item.RECEPTACLE_NO = "001";
                                        else
                                            item.RECEPTACLE_NO = Convert.ToDecimal(workSheet.Cells[rowIterator, 4].Value).ToString("000");

                                        item.ID = workSheet.Cells[rowIterator, 12].Value.ToString();
                                        item.ISSUED_DATE = Convert.ToDateTime(workSheet.Cells[rowIterator, 2].Value).ToString("yyyyMMdd");
                                        item.LOT = workSheet.Cells[rowIterator, 1].Value.ToString();
                                        item.DESPATCH_NO = Convert.ToDecimal(workSheet.Cells[rowIterator, 3].Value.ToString()).ToString("0000");
                                        item.SHIPMENT_STATUS = "1";
                                        item.SERVICE_TYPE = string.Format("00{0}", workSheet.Cells[rowIterator, 5].Value.ToString());
                                        item.DESTINATION_PORT = workSheet.Cells[rowIterator, 6].Value.ToString();
                                        item.GROSS_WEIGHT = Convert.ToDecimal(workSheet.Cells[rowIterator, 8].Value.ToString());
                                        item.EXPECTED_DATE = null;
                                        item.ID_OPTION = workSheet.Cells[rowIterator, 10].Value.UnNull();
                                        item.MASTER_ID = workSheet.Cells[rowIterator, 11].Value.UnNull();
                                        item.PACKUNIT = "1";
                                        items.Add(item);
                                    }
                                    
                                }
                                if (type == "UPDATESCANNING")
                                    _receiptRepository.UpdateScanningStatus(item);
                                else
                                    _receiptRepository.ImportBaggage(item);
                            }
                        }
                    }
                }
                //data = items
                return Json(new
                {
                    statusCode = 200,
                    status = "The data has been uploaded."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    statusCode = 400,
                    status = string.Format("[ERROR]:Line {0}-{1}",rowIterator,ex.Message),
                    file = string.Empty
                }, JsonRequestBehavior.AllowGet);
            }
            
           
        }

    }
}