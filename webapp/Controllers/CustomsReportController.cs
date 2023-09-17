using AutoMapper;
using CrystalDecisions.CrystalReports.Engine;
using Kendo.Mvc.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Style.XmlAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WMSPortal.Core.Model;
using WMSPortal.Data.Repositories;
using WMSPortal.ViewModels;

namespace WMSPortal.Controllers
{
    public class CustomsReportController : Controller
    {
        private IReportRepository _reportRepository;
        private IUserLogRepository _userLogRepository;
        private UserLog userLog;
        public CustomsReportController(IReportRepository reportRepository, IUserLogRepository userLogRepository)
        {
            _userLogRepository = userLogRepository;
            _reportRepository = reportRepository;

            userLog = new UserLog();
            if (System.Web.HttpContext.Current.Session["userRoles"] == null)
                userLog.LoginName = string.Empty;
            else
                userLog.LoginName = ((User)System.Web.HttpContext.Current.Session["userRoles"]).UserName;
        }
        // GET: CustomsReport
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DynamicReports()
        {
            userLog.EventDate = DateTime.Now;
            userLog.EventType = EventLog.Access.ToString();
            userLog.EventDescription = "DynamicReports";
            _userLogRepository.CreateUserLog(userLog);
            return View();
        }

        public ActionResult CustomsInboundReport()
        {
            userLog.EventDate = DateTime.Now;
            userLog.EventType = EventLog.Access.ToString();
            userLog.EventDescription = "CustomsInboundReport";
            _userLogRepository.CreateUserLog(userLog);
           
            var dataSource = new DataTable();
            //dataSource.Columns.Add("Field1");
            //dataSource.Columns.Add("Field2", typeof(int));
            //for (int i = 0; i < 10; i++)
            //{
            //    dataSource.Rows.Add("value" + i, i);
            //}

            return View(dataSource);
        }
        public ActionResult CustomsOutboundReport()
        {
            userLog.EventDate = DateTime.Now;
            userLog.EventType = EventLog.Access.ToString();
            userLog.EventDescription = "CustomsOutboundReport";
            _userLogRepository.CreateUserLog(userLog);

            return View();
        }
        public ActionResult StockBalanceReport()
        {
            userLog.EventDate = DateTime.Now;
            userLog.EventType = EventLog.Access.ToString();
            userLog.EventDescription = "StockBalanceReport";
            _userLogRepository.CreateUserLog(userLog);
            return View();
        }
        public ActionResult LedgerReport()
        {
            userLog.EventDate = DateTime.Now;
            userLog.EventType = EventLog.Access.ToString();
            userLog.EventDescription = "LedgerReport";
            _userLogRepository.CreateUserLog(userLog);

            return View();
        }
        public ActionResult MovementReport()
        {
            userLog.EventDate = DateTime.Now;
            userLog.EventType = EventLog.Access.ToString();
            userLog.EventDescription = "MovementReport";
            _userLogRepository.CreateUserLog(userLog);

            return View();
        }
        public ActionResult CustomsInboundTransReport()
        {
            userLog.EventDate = DateTime.Now;
            userLog.EventType = EventLog.Access.ToString();
            userLog.EventDescription = "CustomsInboundTransReport";
            _userLogRepository.CreateUserLog(userLog);

            return View();
        }
        
        [HttpPost]
        public ActionResult ExportCustomsInboundTransReport()
        {
            string handle = string.Empty;
            Random rnd = new Random();
            List<GLDeclarationReport> items = new List<GLDeclarationReport>();
            for (int i = 0; i < 10; i++)
            {
                GLDeclarationReport item = new GLDeclarationReport();
                item.ReceiptKey = i.ToString();
                item.Sku = "Sku" + i.ToString();
                item.Quantity = rnd.Next(1, 13);
                items.Add(item);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "InboundTransections.rpt"));
            rd.SetDataSource(items);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                handle = Guid.NewGuid().ToString();
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                Session[handle] = stream;
                return new JsonResult()
                {
                    Data = new { Result = items, FileGuid = handle, FileName = "CustomsInboundTransRepor.pdf", ErrorMessage = string.Empty, RecordCount = 0 }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public ActionResult ExportCustomsInboundReport(InboundReportParameters inboundReportParameters)
        {
            string msg = string.Empty;
            bool result = true;
            string handle = string.Empty;
            int recordCount = 0;
            IEnumerable<ImportDeclarationReport> items = null;
            try
            {
                userLog.EventDate = DateTime.Now;
                userLog.EventType = EventLog.Execute.ToString();
                userLog.EventDescription = "Generate inbound report [ExportCustomsInboundReport]";

                var jsonUserLog = Newtonsoft.Json.JsonConvert.SerializeObject(inboundReportParameters);
                userLog.SqlCommand = jsonUserLog;
                _userLogRepository.CreateUserLog(userLog);

                handle = Guid.NewGuid().ToString();
                string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;
                string companyNameHeader = string.Empty;
                if (serverType == "0")
                {
                    items = _reportRepository.GetImportDeclaraion
                        (
                            inboundReportParameters.DateType,
                            inboundReportParameters.StartDate,
                            inboundReportParameters.StopDate,
                            inboundReportParameters.YearSelected,
                            inboundReportParameters.MonthSelected,
                            inboundReportParameters.DeclarationType,
                            inboundReportParameters.Importers,
                            inboundReportParameters.Sku
                            );
                }
                else
                {
                    items = _reportRepository.GetImportDeclaraionFreeZone
                            (
                            inboundReportParameters.DateType,
                            inboundReportParameters.StartDate,
                            inboundReportParameters.StopDate,
                            inboundReportParameters.YearSelected,
                            inboundReportParameters.MonthSelected,
                            inboundReportParameters.DeclarationType,
                            inboundReportParameters.Importers,
                            false,
                            inboundReportParameters.Sku
                            );
                }

                if (items.Count() == 0)
                {
                    result = false;
                    msg = "ไม่พบข้อมูลที่กำลังค้นหา";
                }
                else
                {
                    DateTime startDate = DateTime.ParseExact(inboundReportParameters.StartDate.Substring(0, 10).Replace("-", ""),
                                        "yyyyMMdd",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None);
                    DateTime stopDate = DateTime.ParseExact(inboundReportParameters.StopDate.Substring(0, 10).Replace("-", ""),
                                        "yyyyMMdd",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None);
                    string dateTitle = (inboundReportParameters.DateType == "0" ? string.Format("ประจำปี {0} เดือน {1}", inboundReportParameters.YearSelected, inboundReportParameters.MonthNameSelected) : string.Format("ระหว่างวันที่ {0} ถึง {1}", startDate.ToString("dd/MM/yyyy"), stopDate.ToString("dd/MM/yyyy")));
                    string importerTitle = string.Empty;
                    result = true;
                    MemoryStream stream;
                    if (serverType == "0")
                        stream = GetStreamImportDeclaration(items, dateTitle, importerTitle);
                    else
                        stream = GetStreamImportDeclarationFreezoneNew(items, dateTitle, importerTitle);

                    Session[handle] = stream.ToArray();
                }

            }
            catch (SqlException sqlEx)
            {
                result = false;
                msg = sqlEx.Message;
            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }
            finally
            {
                if (items == null)
                    recordCount = 0;
                else
                    recordCount = items.Count();
            }
            return new JsonResult()
            {
                Data = new { Result = result, FileGuid = handle, FileName = "CustomsInboundReport.xlsx", ErrorMessage = msg, RecordCount = recordCount }
            };

        }
        [HttpPost]
        public ActionResult ExportCustomsOutboundReport(OutboundReportParameters outboundReportParameters)
        {
            string msg = string.Empty;
            bool result = true;
            string handle = string.Empty;
            int recordCount = 0;
            IEnumerable<ExportDeclarationReport> items = null;
            try
            {
                userLog.EventDate = DateTime.Now;
                userLog.EventType = EventLog.Execute.ToString();
                userLog.EventDescription = "Generate outbound report [ExportCustomsOutboundReport]";

                var jsonUserLog = Newtonsoft.Json.JsonConvert.SerializeObject(outboundReportParameters);
                userLog.SqlCommand = jsonUserLog;
                _userLogRepository.CreateUserLog(userLog);

                handle = Guid.NewGuid().ToString();
                string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;
                string companyNameHeader = string.Empty;
                if (serverType == "0")
                {
                    items = _reportRepository.GetExportDeclaraion
                   (
                        outboundReportParameters.DateType,
                        outboundReportParameters.StartDate,
                        outboundReportParameters.StopDate,
                        outboundReportParameters.YearSelected,
                        outboundReportParameters.MonthSelected,
                        outboundReportParameters.DeclarationType,
                        outboundReportParameters.Importers,
                        outboundReportParameters.Sku
                   );
                }
                else
                {
                    items = _reportRepository.GetExportFreeZoneDeclaraionNew
                    (
                        outboundReportParameters.DateType,
                        outboundReportParameters.StartDate,
                        outboundReportParameters.StopDate,
                        outboundReportParameters.YearSelected,
                        outboundReportParameters.MonthSelected,
                        outboundReportParameters.DeclarationType,
                        outboundReportParameters.Importers,
                        outboundReportParameters.Sku
                    );
                }


                if (items.Count() == 0)
                {
                    result = false;
                    msg = "ไม่พบข้อมูลที่กำลังค้นหา";
                }
                else
                {
                    DateTime startDate = DateTime.ParseExact(outboundReportParameters.StartDate.Substring(0, 10).Replace("-", ""),
                                        "yyyyMMdd",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None);
                    DateTime stopDate = DateTime.ParseExact(outboundReportParameters.StopDate.Substring(0, 10).Replace("-", ""),
                                        "yyyyMMdd",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None);
                    string dateTitle = (outboundReportParameters.DateType == "0" ? string.Format("ประจำปี {0} เดือน {1}", outboundReportParameters.YearSelected, outboundReportParameters.MonthNameSelected) : string.Format("ระหว่างวันที่ {0} ถึง {1}", startDate.ToString("dd/MM/yyyy"), stopDate.ToString("dd/MM/yyyy")));
                    string exporterTitle = string.Empty;
                    result = true;
                    MemoryStream stream;
                    if (serverType == "0")
                        stream = GetStreamExportDeclaration(items, dateTitle, exporterTitle);
                    else
                        stream = GetStreamExportDeclarationFreezoneNew(items, dateTitle, exporterTitle);

                    Session[handle] = stream.ToArray();
                }

            }
            catch (SqlException sqlEx)
            {
                result = false;
                msg = sqlEx.Message;
            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }
            finally
            {
                if (items == null)
                    recordCount = 0;
                else
                    recordCount = items.Count();
            }
            return new JsonResult()
            {
                Data = new { Result = result, FileGuid = handle, FileName = "CustomsOutboundReport.xlsx", ErrorMessage = msg, RecordCount = recordCount }
            };

        }
        [HttpPost]
        public ActionResult ExportStockReport(StockBalanceReportParameters stockReportParameters)
        {
            string msg = string.Empty;
            bool result = true;
            string handle = string.Empty;
            int recordCount = 0;
            IEnumerable<GLDeclarationReport> items = null;
            string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;

            try
            {
                
                userLog.EventDate = DateTime.Now;
                userLog.EventType = EventLog.Execute.ToString();
                userLog.EventDescription = "Generate stock report [ExportStockReport]";
                
                var jsonUserLog = Newtonsoft.Json.JsonConvert.SerializeObject(stockReportParameters);
                userLog.SqlCommand = jsonUserLog;
                _userLogRepository.CreateUserLog(userLog);

                handle = Guid.NewGuid().ToString();

                if (stockReportParameters.isShowLocation)
                {
                  items = _reportRepository.GetStockDeclarationByLocation
                  (
                      stockReportParameters.DateType,
                      stockReportParameters.InventoryDate,
                      stockReportParameters.YearSelected,
                      stockReportParameters.MonthSelected,
                      stockReportParameters.DeclarationType,
                      stockReportParameters.Importers,
                      stockReportParameters.Sku
                   );
                }
                else
                {
                   items = _reportRepository.GetStockDeclaration
                   (
                       stockReportParameters.DateType,
                       stockReportParameters.InventoryDate,
                       stockReportParameters.YearSelected,
                       stockReportParameters.MonthSelected,
                       stockReportParameters.DeclarationType,
                       stockReportParameters.Importers,
                       stockReportParameters.Sku
                    );
                }

                if (items.Count() == 0)
                {
                    result = false;
                    msg = "ไม่พบข้อมูลที่กำลังค้นหา";
                }
                else
                {
                    DateTime inventoryDate;
                    if (stockReportParameters.DateType == "0")
                    {
                        inventoryDate = DateTime.ParseExact(string.Format("{0}{1}{2}", stockReportParameters.YearSelected, stockReportParameters.MonthSelected, DateTime.DaysInMonth(Convert.ToInt16(stockReportParameters.YearSelected), Convert.ToInt16(stockReportParameters.MonthSelected))),
                                        "yyyyMMdd",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None);
                    }
                    else
                    {
                        inventoryDate = DateTime.ParseExact(stockReportParameters.InventoryDate.Substring(0, 10).Replace("-", ""),
                                       "yyyyMMdd",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None);
                    }
                    string dateTitle = inventoryDate.ToString("dd/MM/yyyy");
                    string exporterTitle = string.Empty;
                    result = true;
                    MemoryStream stream;
                    if (serverType == "0")
                        stream = GetStreamStockDeclaration(items, dateTitle, exporterTitle);
                    else
                        stream = GetStreamStockDeclarationFreezone(items, dateTitle, exporterTitle);

                    Session[handle] = stream.ToArray();
                }

            }
            catch (SqlException sqlEx)
            {
                result = false;
                msg = sqlEx.Message;
            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }
            finally
            {
                if (items == null)
                    recordCount = 0;
                else
                    recordCount = items.Count();
            }
            return new JsonResult()
            {
                Data = new { Result = result, FileGuid = handle, FileName = "StockBalanceReport.xlsx", ErrorMessage = msg, RecordCount = recordCount }
            };

        }
        [HttpPost]
        public ActionResult ExportLedgerReport(StockBalanceReportParameters stockReportParameters)
        {
            string msg = string.Empty;
            bool result = true;
            string handle = string.Empty;
            int recordCount = 0;
            IEnumerable<GLDeclarationReport> items = null;
            InventoryMovementReport glItems = null;
            string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;

            try
            {
                userLog.EventDate = DateTime.Now;
                userLog.EventType = EventLog.Execute.ToString();
                userLog.EventDescription = "Generate ledger report [ExportLedgerReport]";

                var jsonUserLog = Newtonsoft.Json.JsonConvert.SerializeObject(stockReportParameters);
                userLog.SqlCommand = jsonUserLog;
                _userLogRepository.CreateUserLog(userLog);

                handle = Guid.NewGuid().ToString();
                if (serverType == "0")
                {
                    items = _reportRepository.GetLedgerDeclaration
                    (
                        stockReportParameters.DateType,
                        stockReportParameters.InventoryDate,
                        stockReportParameters.YearSelected,
                        stockReportParameters.MonthSelected,
                        stockReportParameters.DeclarationType,
                        stockReportParameters.Importers
                        );
                }
                else
                {
                    glItems = _reportRepository.GetLedgerDeclarationFreezone
                    (
                        stockReportParameters.DateType,
                        stockReportParameters.InventoryDate,
                        stockReportParameters.YearSelected,
                        stockReportParameters.MonthSelected,
                        stockReportParameters.DeclarationType,
                        stockReportParameters.Importers
                        );
                }

                if (serverType == "0")
                {
                    if (items.Count() == 0)
                    {
                        result = false;
                        msg = "ไม่พบข้อมูลที่กำลังค้นหา";
                    }
                }
                else
                {
                    if (glItems.ImportDeclarationItems.Count() == 0)
                    {
                        result = false;
                        msg = "ไม่พบข้อมูลที่กำลังค้นหา";
                    }
                }


                if (msg == string.Empty)
                {
                    DateTime inventoryDate;
                    if (stockReportParameters.DateType == "0")
                    {
                        inventoryDate = DateTime.ParseExact(string.Format("{0}{1}{2}", stockReportParameters.YearSelected, stockReportParameters.MonthSelected, DateTime.DaysInMonth(Convert.ToInt16(stockReportParameters.YearSelected), Convert.ToInt16(stockReportParameters.MonthSelected))),
                                        "yyyyMMdd",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None);
                    }
                    else
                    {
                        inventoryDate = DateTime.ParseExact(stockReportParameters.InventoryDate.Substring(0, 10).Replace("-", ""),
                                       "yyyyMMdd",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None);
                    }
                    string dateTitle = inventoryDate.ToString("dd/MM/yyyy");
                    string exporterTitle = string.Empty;
                    result = true;
                    MemoryStream stream;
                    if (serverType == "0")
                        stream = GetStreamLedgerDeclaration(items, dateTitle, exporterTitle);
                    else
                        stream = GetStreamLedgerDeclarationFreezone(glItems, dateTitle, exporterTitle);

                    Session[handle] = stream.ToArray();
                }

            }
            catch (SqlException sqlEx)
            {
                result = false;
                msg = sqlEx.Message;
            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }
            finally
            {
                if (serverType == "0")
                {
                    if (items == null)
                        recordCount = 0;
                    else
                        recordCount = items.Count();
                }
                else
                {
                    if (glItems.ImportDeclarationItems == null)
                        recordCount = 0;
                    else
                        recordCount = glItems.ImportDeclarationItems.Count();
                }
            }
            return new JsonResult()
            {
                Data = new { Result = result, FileGuid = handle, FileName = "LedgerReport.xlsx", ErrorMessage = msg, RecordCount = recordCount }
            };

        }
        [HttpPost]
        public ActionResult ExportMovementReport(InboundReportParameters inboundReportParameters)
        {
            string msg = string.Empty;
            bool result = true;
            string handle = string.Empty;
            int totalRecordCount = 0;
            InventoryMovementReport invMovement = null;
            string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;

            try
            {

                userLog.EventDate = DateTime.Now;
                userLog.EventType = EventLog.Execute.ToString();
                userLog.EventDescription = "Generate movement report [ExportMovementReport]";

                var jsonUserLog = Newtonsoft.Json.JsonConvert.SerializeObject(inboundReportParameters);
                userLog.SqlCommand = jsonUserLog;
                _userLogRepository.CreateUserLog(userLog);

                handle = Guid.NewGuid().ToString();

                invMovement = _reportRepository.GetMovementDeclaration(
                        inboundReportParameters.DateType,
                        inboundReportParameters.StartDate,
                        inboundReportParameters.StopDate,
                        inboundReportParameters.YearSelected,
                        inboundReportParameters.MonthSelected,
                        inboundReportParameters.DeclarationType,
                        inboundReportParameters.Importers,
                        inboundReportParameters.Sku
                        );
                totalRecordCount = invMovement.ImportDeclarationItems.Count() +
                    invMovement.BroughtForwardItems.Count() +
                    invMovement.ExportDeclarationItems.Count();
                if (totalRecordCount == 0)
                {
                    result = false;
                    msg = "ไม่พบข้อมูลที่กำลังค้นหา";
                }
                else
                {
                    Nullable<DateTime> startDate = null;
                    Nullable<DateTime> stopDate = null;
                    string dateTitle = string.Empty;

                    if (inboundReportParameters.DateType == "0")
                    {
                        startDate = DateTime.ParseExact(string.Format("{0}{1}{2}", inboundReportParameters.YearSelected, inboundReportParameters.MonthSelected, "01"),
                                       "yyyyMMdd",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None);
                        stopDate = DateTime.ParseExact(string.Format("{0}{1}{2}",
                                            inboundReportParameters.YearSelected,
                                            inboundReportParameters.MonthSelected,
                                            string.Format("{0:00}", DateTime.DaysInMonth(Convert.ToInt16(inboundReportParameters.YearSelected), Convert.ToInt16(inboundReportParameters.MonthSelected)))
                                            ),
                                            "yyyyMMdd",
                                            CultureInfo.InvariantCulture,
                                            DateTimeStyles.None);
                    }
                    else
                    {

                        startDate = DateTime.ParseExact(inboundReportParameters.StartDate.Substring(0, 10).Replace("-", ""),
                                       "yyyyMMdd",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None);
                        stopDate = DateTime.ParseExact(inboundReportParameters.StopDate.Substring(0, 10).Replace("-", ""),
                                            "yyyyMMdd",
                                            CultureInfo.InvariantCulture,
                                            DateTimeStyles.None);
                    }
                    dateTitle = string.Format("ระหว่างวันที่ {0} ถึงวันที่ {1}", String.Format("{0:dd/MM/yyyy}", startDate), String.Format("{0:dd/MM/yyyy}", stopDate));
                    string importerTitle = string.Empty;
                    result = true;
                    MemoryStream stream;
                    stream = GetStreamMovementDeclaration(invMovement, dateTitle, importerTitle);
                  
                    Session[handle] = stream.ToArray();
                }

            }
            catch (SqlException sqlEx)
            {
                result = false;
                msg = sqlEx.Message;
            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
            }
            finally
            {
                //
            }
            return new JsonResult()
            {
                Data = new { Result = result, FileGuid = handle, FileName = "MovementReport.xlsx", ErrorMessage = msg, RecordCount = totalRecordCount }
            };

        }
        [HttpGet]
        public virtual ActionResult Download(string fileGuid, string fileName)
        {
            if (Session[fileGuid] != null)
            {

                byte[] data = Session[fileGuid] as byte[];
                return File(data, "application/vnd.ms-excel", fileName);

            }
            else
            {
                return new EmptyResult();
            }
        }
        [HttpGet]
        public virtual ActionResult DownloadPdf(string fileGuid, string fileName)
        {
            if (Session[fileGuid] != null)
            {
                Stream data = Session[fileGuid] as Stream;
                return File(data, "application/pdf", fileName);
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                // redirect to another controller action - whatever fits with application
                return new EmptyResult();
            }
        }
        private MemoryStream GetStreamImportDeclaration(IEnumerable<ImportDeclarationReport> items, string dateTitle, string importerTitle)
        {
            decimal grandTotalQty = 0;
            decimal grandTotalNetWgt = 0;
            decimal grandTotalAmount = 0;
            decimal grandTotalDuty = 0;

            string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;
            string companyNameHeader = string.Empty;
            if (serverType == "1" || serverType == "2")
                companyNameHeader = "เขตปลอดอากร (Free Zone : FZ)";
            else
                companyNameHeader = "คลังสินค้าทัณฑ์บนทั่วไป";

            int rowIndex = 1;
            int rowDecl = 1;
            string importDeclarationNo = string.Empty;

            ExcelPackage pck = new ExcelPackage();

            var reportTypes = items.GroupBy(
                                i => i.ReportType,
                                (key, group) => group.First()
                            ).ToArray();

            foreach (var reportType in reportTypes)
            {
                grandTotalQty = 0;
                grandTotalNetWgt = 0;
                grandTotalAmount = 0;
                grandTotalDuty = 0;
                var imps = items.GroupBy(
                                              i => i.ImportDeclarationNo,
                                              (key, group) => group.First()
                                          )
                                          .Where(i => i.ReportType == reportType.ReportType.ToString())
                                          .ToArray();


                var wsEnum = pck.Workbook.Worksheets.Add(reportType.ReportType.ToString());

                //Add the headers
                //Solid black border around the board.
                wsEnum.Cells[string.Format("A{0}:L{1}", 1, 1)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[1, 1].Value = "แบบแนบท้ายประมวลฯ ข้อ ๕ ๐๒ ๐๒ ๒๗";
                wsEnum.Cells[string.Format("A{0}:L{1}", 1, 1)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 1, 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                wsEnum.Cells[string.Format("A{0}:L{1}", 2, 2)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[2, 1].Value = "คทบ ๑๔";
                wsEnum.Cells[string.Format("A{0}:L{1}", 2, 2)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 2, 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                wsEnum.Cells[string.Format("A{0}:L{1}", 3, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[3, 1].Value = string.Format("{0} บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396", companyNameHeader);
                wsEnum.Cells[string.Format("A{0}:L{1}", 3, 3)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:L{1}", 4, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[4, 1].Value = string.Format("รายงานของที่นำเข้าเก็บใน{0}", companyNameHeader);
                wsEnum.Cells[string.Format("A{0}:L{1}", 4, 4)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:L{1}", 5, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[5, 1].Value = string.Format("ประเภท : {0}", reportType.ReportType.ToString()); ;
                wsEnum.Cells[string.Format("A{0}:L{1}", 5, 5)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:L{1}", 6, 6)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[6, 1].Value = dateTitle;
                wsEnum.Cells[string.Format("A{0}:L{1}", 6, 6)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 6, 6)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells[string.Format("A{0}:L{1}", 7, 7)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);

                rowIndex = 8;

                wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
                wsEnum.Cells[rowIndex, 2].Value = "เลขที่ใบขนสินค้านำเข้าคลัง";
                wsEnum.Cells[rowIndex, 3].Value = "รายการที่";
                wsEnum.Cells[rowIndex, 4].Value = "ชื่อผู้นำเข้า";
                wsEnum.Cells[rowIndex, 5].Value = "วันที่นำเข้า";
                wsEnum.Cells[rowIndex, 6].Value = "วันที่นำเข้าคลัง";
                wsEnum.Cells[rowIndex, 7].Value = "ชื่อของ";
                wsEnum.Cells[rowIndex, 8].Value = "ปริมาณ";
                wsEnum.Cells[rowIndex, 9].Value = "หน่วยนับ";
                wsEnum.Cells[rowIndex, 10].Value = "น้ำหนัก";
                wsEnum.Cells[rowIndex, 11].Value = "มูลค่า (บาท)";
                wsEnum.Cells[rowIndex, 12].Value = "ภาษีอากรรวม (บาท)";
                wsEnum.Cells[rowIndex, 13].Value = "อินวอยซ์";

                wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                rowIndex++;

                rowDecl = 1;

                foreach (var imp in imps)
                {
                    importDeclarationNo = imp.ImportDeclarationNo;
                    wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                    wsEnum.Cells[rowIndex, 2].Value = imp.ImportDeclarationNo;
                    IEnumerable<ImportDeclarationReport> impds = items.Where(x => x.ImportDeclarationNo == imp.ImportDeclarationNo);
                    int rowCount = 1;
                    foreach (ImportDeclarationReport impd in impds)
                    {
                        if (impd.ImportDeclarationItemNo.Trim() == string.Empty)
                            wsEnum.Cells[rowIndex, 3].Value = string.Format("{0:00000}", rowCount);
                        else
                            wsEnum.Cells[rowIndex, 3].Value = string.Format("{0:00000}", impd.ImportDeclarationItemNo);

                        wsEnum.Cells[rowIndex, 4].Value = impd.ImporterName;
                        wsEnum.Cells[rowIndex, 5].Value = impd.ImportDeclarationDate;
                        wsEnum.Cells[rowIndex, 6].Value = impd.WarehouseReceivedDate;
                        wsEnum.Cells[rowIndex, 7].Value = impd.SkuDescription;
                        wsEnum.Cells[rowIndex, 8].Value = impd.Quantity;
                        wsEnum.Cells[rowIndex, 9].Value = impd.UOM;
                        wsEnum.Cells[rowIndex, 10].Value = impd.NetWgt;
                        wsEnum.Cells[rowIndex, 11].Value = impd.TotalAmount;
                        wsEnum.Cells[rowIndex, 12].Value = impd.TotalDuty;
                        wsEnum.Cells[rowIndex, 13].Value = impd.Invoice;
                        rowIndex++;
                        rowCount++;
                    }

                    wsEnum.Cells[rowIndex, 1].Value = "รวม";
                    wsEnum.Cells[string.Format("A{0}:G{1}", rowIndex, rowIndex)].Merge = true;
                    wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsEnum.Cells[rowIndex, 8].Value = items.Where(x => x.ImportDeclarationNo == importDeclarationNo).Sum(pk => pk.Quantity);
                    wsEnum.Cells[rowIndex, 10].Value = items.Where(x => x.ImportDeclarationNo == importDeclarationNo).Sum(pk => pk.NetWgt);
                    wsEnum.Cells[rowIndex, 11].Value = items.Where(x => x.ImportDeclarationNo == importDeclarationNo).Sum(pk => pk.TotalAmount);
                    wsEnum.Cells[rowIndex, 12].Value = items.Where(x => x.ImportDeclarationNo == importDeclarationNo).Sum(pk => pk.TotalDuty);
                    wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                    wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    grandTotalQty += Convert.ToDecimal(wsEnum.Cells[rowIndex, 8].Value);
                    grandTotalNetWgt += Convert.ToDecimal(wsEnum.Cells[rowIndex, 10].Value);
                    grandTotalAmount += Convert.ToDecimal(wsEnum.Cells[rowIndex, 11].Value);
                    grandTotalDuty += Convert.ToDecimal(wsEnum.Cells[rowIndex, 12].Value);
                    rowIndex++;
                    rowDecl++;
                }


                // Footer //
                wsEnum.Cells[rowIndex, 1].Value = "รวมทั้งสิ้น";
                wsEnum.Cells[string.Format("A{0}:G{1}", rowIndex, rowIndex)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsEnum.Cells[rowIndex, 8].Value = grandTotalQty;// items.Sum(pk => pk.Quantity);
                wsEnum.Cells[rowIndex, 10].Value = grandTotalNetWgt;// items.Sum(pk => pk.NetWgt);
                wsEnum.Cells[rowIndex, 11].Value = grandTotalAmount;// items.Sum(pk => pk.TotalAmount);
                wsEnum.Cells[rowIndex, 12].Value = grandTotalDuty;// items.Sum(pk => pk.TotalDuty);
                wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                wsEnum.Cells["A1:L" + rowIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                wsEnum.Cells["A1:L" + rowIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                // End Footer //

                // Formatting //
                wsEnum.Cells["E2:E" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
                wsEnum.Cells["F2:F" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
                wsEnum.Cells["H2:L" + rowIndex].Style.Numberformat.Format = "#,##0";
                wsEnum.Cells["J2:L" + rowIndex].Style.Numberformat.Format = "#,##0.00";

                wsEnum.Cells[wsEnum.Dimension.Address].AutoFitColumns();


            }


            MemoryStream memoryStream = new MemoryStream();

            pck.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        private MemoryStream GetStreamImportDeclarationFreezone(IEnumerable<ImportDeclarationReport> items, string dateTitle, string importerTitle)
        {
            decimal grandTotalQty = 0;
            decimal grandTotalNetWgt = 0;
            decimal grandTotalAmount = 0;

            string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;
            string companyNameHeader = string.Empty;

            companyNameHeader = "เขตปลอดอากร (Free Zone : FZ)";

            int rowIndex = 1;
            int rowDecl = 1;
            string importDeclarationNo = string.Empty;

            ExcelPackage pck = new ExcelPackage();

            var reportTypes = items.GroupBy(
                                i => i.ReportType,
                                (key, group) => group.First()
                            ).ToArray();

            foreach (var reportType in reportTypes)
            {
                grandTotalQty = 0;
                grandTotalNetWgt = 0;
                grandTotalAmount = 0;

                var imps = items.GroupBy(
                                i => i.ImportDeclarationNo,
                                (key, group) => group.First()
                            )
                            .Where(i => i.ReportType == reportType.ReportType.ToString())
                            .ToArray();


                var wsEnum = pck.Workbook.Worksheets.Add(reportType.ReportType.ToString());


                wsEnum.Cells[string.Format("A{0}:N{1}", 3, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[3, 1].Value = string.Format("{0} บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396", companyNameHeader);
                wsEnum.Cells[string.Format("A{0}:N{1}", 3, 3)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:N{1}", 4, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[4, 1].Value = string.Format("รายงานของที่นำเข้าเก็บใน{0}", companyNameHeader);
                wsEnum.Cells[string.Format("A{0}:N{1}", 4, 4)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:N{1}", 5, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[5, 1].Value = string.Format("ประเภท : {0}", reportType.ReportType.ToString()); ;
                wsEnum.Cells[string.Format("A{0}:N{1}", 5, 5)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:N{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:N{1}", 6, 6)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[6, 1].Value = dateTitle;
                wsEnum.Cells[string.Format("A{0}:N{1}", 6, 6)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 6, 6)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells[string.Format("A{0}:N{1}", 7, 7)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);

                rowIndex = 8;

                wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
                wsEnum.Cells[rowIndex, 2].Value = "เลขที่ใบขนสินค้านำเข้าคลัง";
                wsEnum.Cells[rowIndex, 3].Value = "รายการที่";
                wsEnum.Cells[rowIndex, 4].Value = "ชื่อผู้นำเข้า";
                wsEnum.Cells[rowIndex, 5].Value = "วันที่นำเข้าคลัง";
                wsEnum.Cells[rowIndex, 6].Value = "อินวอยซ์";
                wsEnum.Cells[rowIndex, 7].Value = "รหัสคลัง";
                wsEnum.Cells[rowIndex, 8].Value = "รหัสสินค้า";
                wsEnum.Cells[rowIndex, 9].Value = "ชื่อของ";
                wsEnum.Cells[rowIndex, 10].Value = "ปริมาณ";
                wsEnum.Cells[rowIndex, 11].Value = "หน่วยนับ";
                wsEnum.Cells[rowIndex, 12].Value = "น้ำหนัก";
                wsEnum.Cells[rowIndex, 13].Value = "มูลค่า (บาท)";
                wsEnum.Cells[rowIndex, 14].Value = "หมายเหตุ";

                wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                rowIndex++;

                rowDecl = 1;

                foreach (var imp in imps)
                {
                    importDeclarationNo = imp.ImportDeclarationNo;
                    wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                    wsEnum.Cells[rowIndex, 2].Value = imp.ImportDeclarationNo;
                    IEnumerable<ImportDeclarationReport> impds = items.Where(x => x.ImportDeclarationNo == imp.ImportDeclarationNo);
                    int rowCount = 1;
                    foreach (ImportDeclarationReport impd in impds)
                    {
                        if (impd.ImportDeclarationItemNo.Trim() == string.Empty)
                            wsEnum.Cells[rowIndex, 3].Value = string.Format("{0:00000}", rowCount);
                        else
                            wsEnum.Cells[rowIndex, 3].Value = string.Format("{0:00000}", impd.ImportDeclarationItemNo);

                        wsEnum.Cells[rowIndex, 4].Value = impd.ImporterName;
                        wsEnum.Cells[rowIndex, 5].Value = impd.ImportDeclarationDate;
                        wsEnum.Cells[rowIndex, 6].Value = impd.Invoice;
                        wsEnum.Cells[rowIndex, 7].Value = impd.Taxincentive;
                        wsEnum.Cells[rowIndex, 8].Value = impd.Sku;
                        wsEnum.Cells[rowIndex, 9].Value = impd.SkuDescription;
                        wsEnum.Cells[rowIndex, 10].Value = impd.Quantity;
                        wsEnum.Cells[rowIndex, 11].Value = impd.UOM;
                        wsEnum.Cells[rowIndex, 12].Value = impd.NetWgt;
                        wsEnum.Cells[rowIndex, 13].Value = impd.TotalAmount;
                        wsEnum.Cells[rowIndex, 14].Value = impd.Remark;
                        rowIndex++;
                        rowCount++;
                    }

                    wsEnum.Cells[rowIndex, 1].Value = "รวม";
                    wsEnum.Cells[string.Format("A{0}:I{1}", rowIndex, rowIndex)].Merge = true;
                    wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsEnum.Cells[rowIndex, 10].Value = items.Where(x => x.ImportDeclarationNo == importDeclarationNo).Sum(pk => pk.Quantity);
                    wsEnum.Cells[rowIndex, 12].Value = items.Where(x => x.ImportDeclarationNo == importDeclarationNo).Sum(pk => pk.NetWgt);
                    wsEnum.Cells[rowIndex, 13].Value = items.Where(x => x.ImportDeclarationNo == importDeclarationNo).Sum(pk => pk.TotalAmount);
                    wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                    wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    grandTotalQty += Convert.ToDecimal(wsEnum.Cells[rowIndex, 10].Value);
                    grandTotalNetWgt += Convert.ToDecimal(wsEnum.Cells[rowIndex, 12].Value);
                    grandTotalAmount += Convert.ToDecimal(wsEnum.Cells[rowIndex, 13].Value);
                    rowIndex++;
                    rowDecl++;
                }


                // Footer //
                wsEnum.Cells[rowIndex, 1].Value = "รวมทั้งสิ้น";
                wsEnum.Cells[string.Format("A{0}:I{1}", rowIndex, rowIndex)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsEnum.Cells[rowIndex, 10].Value = grandTotalQty;// items.Sum(pk => pk.Quantity);
                wsEnum.Cells[rowIndex, 12].Value = grandTotalNetWgt;// items.Sum(pk => pk.NetWgt);
                wsEnum.Cells[rowIndex, 13].Value = grandTotalAmount;// items.Sum(pk => pk.TotalAmount);
                wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                wsEnum.Cells["A1:N" + rowIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                wsEnum.Cells["A1:N" + rowIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                // End Footer //

                // Formatting //
                wsEnum.Cells["E2:E" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";

                wsEnum.Cells["J2:J" + rowIndex].Style.Numberformat.Format = "#,##0";
                wsEnum.Cells["L2:L" + rowIndex].Style.Numberformat.Format = "#,##0.000";
                wsEnum.Cells["M2:M" + rowIndex].Style.Numberformat.Format = "#,##0.00";

                wsEnum.Cells[wsEnum.Dimension.Address].AutoFitColumns();


            }


            MemoryStream memoryStream = new MemoryStream();

            pck.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        private MemoryStream GetStreamImportDeclarationFreezoneNew(IEnumerable<ImportDeclarationReport> items, string dateTitle, string importerTitle)
        {
            decimal grandTotalQty = 0;
            decimal grandTotalNetWgt = 0;
            decimal grandTotalAmount = 0;

            string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;
            string companyNameHeader = string.Empty;

            companyNameHeader = "เขตปลอดอากร (Free Zone : FZ)";

            int rowIndex = 1;
            int rowDecl = 1;
            string importDeclarationNo = string.Empty;

            ExcelPackage pck = new ExcelPackage();

            var reportTypes = items.GroupBy(
                                i => i.ReportType,
                                (key, group) => group.First()
                            ).ToArray();

            foreach (var reportType in reportTypes)
            {
                grandTotalQty = 0;
                grandTotalNetWgt = 0;
                grandTotalAmount = 0;

                var imps = items.GroupBy(
                                i => i.ImportDeclarationNo,
                                (key, group) => group.First()
                            )
                            .Where(i => i.ReportType == reportType.ReportType.ToString())
                            .ToArray();


                var wsEnum = pck.Workbook.Worksheets.Add(reportType.ReportType.ToString());


                wsEnum.Cells[string.Format("A{0}:K{1}", 3, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[3, 1].Value = string.Format("{0} บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396", companyNameHeader);
                wsEnum.Cells[string.Format("A{0}:K{1}", 3, 3)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:K{1}", 4, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[4, 1].Value = "รายงานของที่นำของเข้า";
                wsEnum.Cells[string.Format("A{0}:K{1}", 4, 4)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:K{1}", 5, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[5, 1].Value = string.Format("ประเภท : {0}", reportType.ReportType.ToString()); ;
                wsEnum.Cells[string.Format("A{0}:K{1}", 5, 5)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:K{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:K{1}", 6, 6)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[6, 1].Value = dateTitle;
                wsEnum.Cells[string.Format("A{0}:K{1}", 6, 6)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 6, 6)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells[string.Format("A{0}:K{1}", 7, 7)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);

                rowIndex = 8;

                wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
                wsEnum.Cells[rowIndex, 2].Value = "เลขที่ใบขนสินค้า/คำร้อง";
                wsEnum.Cells[rowIndex, 3].Value = "เลขที่ Invoice";
                wsEnum.Cells[rowIndex, 4].Value = "วันที่สถานะใบขนฯ 0๔0๙";
                wsEnum.Cells[rowIndex, 5].Value = "รหัสสินค้า/วัตถุดิบ";
                wsEnum.Cells[rowIndex, 6].Value = "รายละเอียดสินค้า";
                wsEnum.Cells[rowIndex, 7].Value = "ปริมาณ";
                wsEnum.Cells[rowIndex, 8].Value = "หน่วยนับ";
                wsEnum.Cells[rowIndex, 9].Value = "น้ำหนัก (KG)";
                wsEnum.Cells[rowIndex, 10].Value = "ราคาต่อหน่วย";
                wsEnum.Cells[rowIndex, 11].Value = "มูลค่า (บาท)";
            

                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                rowIndex++;

                rowDecl = 1;

                foreach (var imp in imps)
                {
                    importDeclarationNo = imp.ImportDeclarationNo;
                    wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                    wsEnum.Cells[rowIndex, 2].Value = imp.ImportDeclarationNo;
                    IEnumerable<ImportDeclarationReport> impds = items.Where(x => x.ImportDeclarationNo == imp.ImportDeclarationNo);
                    int rowCount = 1;
                    foreach (ImportDeclarationReport impd in impds)
                    {
                        wsEnum.Cells[rowIndex, 3].Value = impd.Invoice;
                        wsEnum.Cells[rowIndex, 4].Value = impd.CustomsPermitDate;
                        wsEnum.Cells[rowIndex, 5].Value = impd.Sku;
                        wsEnum.Cells[rowIndex, 6].Value = impd.SkuDescription;
                        wsEnum.Cells[rowIndex, 7].Value = impd.Quantity;
                        wsEnum.Cells[rowIndex, 8].Value = impd.UOM;
                        wsEnum.Cells[rowIndex, 9].Value = impd.NetWgt;
                        wsEnum.Cells[rowIndex, 10].Value = (impd.TotalAmount/ impd.Quantity);
                        wsEnum.Cells[rowIndex, 11].Value = impd.TotalAmount;
                        
                        rowIndex++;
                        rowCount++;
                    }

                    wsEnum.Cells[rowIndex, 1].Value = "รวม";
                    wsEnum.Cells[string.Format("A{0}:F{1}", rowIndex, rowIndex)].Merge = true;
                    wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsEnum.Cells[rowIndex, 7].Value = items.Where(x => x.ImportDeclarationNo == importDeclarationNo).Sum(pk => pk.Quantity);
                    wsEnum.Cells[rowIndex, 9].Value = items.Where(x => x.ImportDeclarationNo == importDeclarationNo).Sum(pk => pk.NetWgt);
                    wsEnum.Cells[rowIndex, 11].Value = items.Where(x => x.ImportDeclarationNo == importDeclarationNo).Sum(pk => pk.TotalAmount);
                    wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                    wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    grandTotalQty += Convert.ToDecimal(wsEnum.Cells[rowIndex, 7].Value);
                    grandTotalNetWgt += Convert.ToDecimal(wsEnum.Cells[rowIndex, 9].Value);
                    grandTotalAmount += Convert.ToDecimal(wsEnum.Cells[rowIndex, 11].Value);
                    rowIndex++;
                    rowDecl++;
                }


                // Footer //
                wsEnum.Cells[rowIndex, 1].Value = "รวมทั้งสิ้น";
                wsEnum.Cells[string.Format("A{0}:F{1}", rowIndex, rowIndex)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsEnum.Cells[rowIndex, 7].Value = grandTotalQty;
                wsEnum.Cells[rowIndex, 9].Value = grandTotalNetWgt;
                wsEnum.Cells[rowIndex, 11].Value = grandTotalAmount;
                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                wsEnum.Cells["A1:K" + rowIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                wsEnum.Cells["A1:K" + rowIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                // End Footer //

                // Formatting //
                wsEnum.Cells["D2:D" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";

                wsEnum.Cells["G2:G" + rowIndex].Style.Numberformat.Format = "#,##0";
                wsEnum.Cells["I2:I" + rowIndex].Style.Numberformat.Format = "#,##0.000";
                wsEnum.Cells["J2:K" + rowIndex].Style.Numberformat.Format = "#,##0.00";

                wsEnum.Cells[wsEnum.Dimension.Address].AutoFitColumns();


            }


            MemoryStream memoryStream = new MemoryStream();

            pck.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        private MemoryStream GetStreamExportDeclaration(IEnumerable<ExportDeclarationReport> items, string dateTitle, string exporterTitle)
        {
            decimal grandTotalQty = 0;
            decimal grandTotalNetWgt = 0;
            decimal grandTotalAmount = 0;

            string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;
            string companyNameHeader = string.Empty;
            if (serverType == "1" || serverType == "2")
                companyNameHeader = "เขตปลอดอากร (Free Zone : FZ)";
            else
                companyNameHeader = "คลังสินค้าทัณฑ์บนทั่วไป";

            int rowIndex = 1;
            int rowDecl = 1;
            string currentExportDeclaration = string.Empty;

            ExcelPackage pck = new ExcelPackage();

            var reportTypes = items.GroupBy(
                                i => i.ReportType,
                                (key, group) => group.First()
                            ).ToArray();

            foreach (var reportType in reportTypes)
            {
                grandTotalQty = 0;
                grandTotalNetWgt = 0;
                grandTotalAmount = 0;

                var exps = items.GroupBy(
                                              i => i.ExportDeclarationNo,
                                              (key, group) => group.First()
                                          )
                                          .Where(i => i.ReportType == reportType.ReportType.ToString())
                                          .ToArray();


                var wsEnum = pck.Workbook.Worksheets.Add(reportType.ReportType.ToString());

                //Add the headers
                //Solid black border around the board.
                wsEnum.Cells[string.Format("A{0}:M{1}", 1, 1)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[1, 1].Value = "แบบแนบท้ายประมวลฯ ข้อ ๕ ๐๒ ๐๒ ๒๗";
                wsEnum.Cells[string.Format("A{0}:M{1}", 1, 1)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 1, 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                wsEnum.Cells[string.Format("A{0}:M{1}", 2, 2)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[2, 1].Value = "คทบ ๑๕";
                wsEnum.Cells[string.Format("A{0}:M{1}", 2, 2)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 2, 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                wsEnum.Cells[string.Format("A{0}:M{1}", 3, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[3, 1].Value = string.Format("{0} บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396", companyNameHeader);
                wsEnum.Cells[string.Format("A{0}:M{1}", 3, 3)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:M{1}", 4, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[4, 1].Value = string.Format("รายงานของที่นำออกจาก{0}", companyNameHeader);
                wsEnum.Cells[string.Format("A{0}:M{1}", 4, 4)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:M{1}", 5, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[5, 1].Value = string.Format("ประเภท : {0}", reportType.ReportType.ToString());
                wsEnum.Cells[string.Format("A{0}:M{1}", 5, 5)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:M{1}", 6, 6)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[6, 1].Value = dateTitle;
                wsEnum.Cells[string.Format("A{0}:M{1}", 6, 6)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 6, 6)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells[string.Format("A{0}:M{1}", 7, 7)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);

                rowIndex = 8;

                wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
                wsEnum.Cells[rowIndex, 2].Value = "เลขที่ใบขนสินค้านำออกจากคลังฯ";
                wsEnum.Cells[rowIndex, 3].Value = "รายการที่";
                wsEnum.Cells[rowIndex, 4].Value = "ชื่อผู้นำออก";
                wsEnum.Cells[rowIndex, 5].Value = "วันที่ส่งออก/วันที่นำออก";
                wsEnum.Cells[rowIndex, 6].Value = "ชื่อของ";
                wsEnum.Cells[rowIndex, 7].Value = "ปริมาณ";
                wsEnum.Cells[rowIndex, 8].Value = "หน่วยนับ";
                wsEnum.Cells[rowIndex, 9].Value = "น้ำหนัก";
                wsEnum.Cells[rowIndex, 10].Value = "มูลค่า (บาท)";
                wsEnum.Cells[rowIndex, 11].Value = "ตัดจากใบขนสินค้านำเข้าคลังฯเลขที่";
                wsEnum.Cells[rowIndex, 12].Value = "รายการที่อ้างอิงถึง";

                wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                rowIndex++;

                rowDecl = 1;

                foreach (var exp in exps)
                {
                    currentExportDeclaration = exp.ExportDeclarationNo;
                    wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                    wsEnum.Cells[rowIndex, 2].Value = exp.ExportDeclarationNo;
                    IEnumerable<ExportDeclarationReport> expds = items.Where(x => x.ExportDeclarationNo == exp.ExportDeclarationNo);
                    int rowCount = 1;
                    foreach (ExportDeclarationReport expd in expds)
                    {
                        if (expd.InvoiceItemNo == 0)
                            wsEnum.Cells[rowIndex, 3].Value = string.Format("{0:00000}", rowCount);
                        else
                            wsEnum.Cells[rowIndex, 3].Value = string.Format("{0:00000}", expd.InvoiceItemNo);

                        wsEnum.Cells[rowIndex, 4].Value = expd.ExportName;
                        wsEnum.Cells[rowIndex, 5].Value = expd.ExportDeclarationDate;

                        if (serverType == "2")
                            wsEnum.Cells[rowIndex, 6].Value = expd.AltSkuDescription;
                        else
                            wsEnum.Cells[rowIndex, 6].Value = expd.SkuDescription;

                        wsEnum.Cells[rowIndex, 7].Value = expd.Quantity;
                        wsEnum.Cells[rowIndex, 8].Value = expd.UOM;
                        wsEnum.Cells[rowIndex, 9].Value = expd.NetWgt;
                        wsEnum.Cells[rowIndex, 10].Value = expd.TotalAmount;
                        wsEnum.Cells[rowIndex, 11].Value = expd.ImportDeclarationNo;
                        wsEnum.Cells[rowIndex, 12].Value = expd.ImportDeclarationItemNo;


                        rowIndex++;
                        rowCount++;
                    }

                    wsEnum.Cells[rowIndex, 1].Value = "รวม";
                    wsEnum.Cells[string.Format("A{0}:F{1}", rowIndex, rowIndex)].Merge = true;
                    wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsEnum.Cells[rowIndex, 7].Value = items.Where(x => x.ExportDeclarationNo == currentExportDeclaration).Sum(pk => pk.Quantity);
                    wsEnum.Cells[rowIndex, 9].Value = items.Where(x => x.ExportDeclarationNo == currentExportDeclaration).Sum(pk => pk.NetWgt);
                    wsEnum.Cells[rowIndex, 10].Value = items.Where(x => x.ExportDeclarationNo == currentExportDeclaration).Sum(pk => pk.TotalAmount);

                    wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                    wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                    grandTotalQty += Convert.ToDecimal(wsEnum.Cells[rowIndex, 7].Value);
                    grandTotalNetWgt += Convert.ToDecimal(wsEnum.Cells[rowIndex, 9].Value);
                    grandTotalAmount += Convert.ToDecimal(wsEnum.Cells[rowIndex, 10].Value);

                    rowIndex++;
                    rowDecl++;
                }


                // Footer //
                wsEnum.Cells[rowIndex, 1].Value = "รวมทั้งสิ้น";
                wsEnum.Cells[string.Format("A{0}:F{1}", rowIndex, rowIndex)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsEnum.Cells[rowIndex, 7].Value = grandTotalQty;// items.Sum(pk => pk.Quantity);
                wsEnum.Cells[rowIndex, 9].Value = grandTotalNetWgt;// items.Sum(pk => pk.NetWgt);
                wsEnum.Cells[rowIndex, 10].Value = grandTotalAmount;// items.Sum(pk => pk.TotalAmount);

                wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                wsEnum.Cells["A1:M" + rowIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                wsEnum.Cells["A1:M" + rowIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                // End Footer //

                // Formatting //
                wsEnum.Cells["E2:E" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
                wsEnum.Cells["G2:G" + rowIndex].Style.Numberformat.Format = "#,##0";
                wsEnum.Cells["I2:I" + rowIndex].Style.Numberformat.Format = "#,##0.0000";
                wsEnum.Cells["J2:J" + rowIndex].Style.Numberformat.Format = "#,##0.00";
                wsEnum.Cells[wsEnum.Dimension.Address].AutoFitColumns();


            }


            MemoryStream memoryStream = new MemoryStream();

            pck.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        private MemoryStream GetStreamExportDeclarationFreezone(IEnumerable<ExportDeclarationReport> items, string dateTitle, string exporterTitle)
        {
            decimal grandTotalQty = 0;
            decimal grandTotalNetWgt = 0;
            decimal grandTotalAmount = 0;

            string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;
            string companyNameHeader = string.Empty;

            companyNameHeader = "เขตปลอดอากร (Free Zone : FZ)";

            int rowIndex = 1;
            int rowDecl = 1;
            string currentExportDeclaration = string.Empty;

            ExcelPackage pck = new ExcelPackage();

            var reportTypes = items.GroupBy(
                                i => i.ReportType,
                                (key, group) => group.First()
                            ).ToArray();

            foreach (var reportType in reportTypes)
            {
                grandTotalQty = 0;
                grandTotalNetWgt = 0;
                grandTotalAmount = 0;

                var exps = items.GroupBy(
                                              i => i.ExportDeclarationNo,
                                              (key, group) => group.First()
                                          )
                                          .Where(i => i.ReportType == reportType.ReportType.ToString())
                                          .ToArray();


                var wsEnum = pck.Workbook.Worksheets.Add(reportType.ReportType.ToString());

                wsEnum.Cells[string.Format("A{0}:L{1}", 3, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[3, 1].Value = string.Format("{0} บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396", companyNameHeader);
                wsEnum.Cells[string.Format("A{0}:L{1}", 3, 3)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:L{1}", 4, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[4, 1].Value = string.Format("รายงานของที่นำออกจาก{0}", companyNameHeader);
                wsEnum.Cells[string.Format("A{0}:L{1}", 4, 4)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:L{1}", 5, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[5, 1].Value = string.Format("ประเภท : {0}", reportType.ReportType.ToString());
                wsEnum.Cells[string.Format("A{0}:L{1}", 5, 5)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:L{1}", 6, 6)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[6, 1].Value = dateTitle;
                wsEnum.Cells[string.Format("A{0}:L{1}", 6, 6)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 6, 6)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells[string.Format("A{0}:L{1}", 7, 7)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);

                rowIndex = 8;

                wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
                wsEnum.Cells[rowIndex, 2].Value = "เลขที่ใบขนสินค้านำออกจากคลังฯ";
                wsEnum.Cells[rowIndex, 3].Value = "รายการที่";
                wsEnum.Cells[rowIndex, 4].Value = "ชื่อผู้นำออก";
                wsEnum.Cells[rowIndex, 5].Value = "วันที่ส่งออกคลัง";
                wsEnum.Cells[rowIndex, 6].Value = "รหัสสินค้า";
                wsEnum.Cells[rowIndex, 7].Value = "ชื่อของ";
                wsEnum.Cells[rowIndex, 8].Value = "ปริมาณ";
                wsEnum.Cells[rowIndex, 9].Value = "หน่วยนับ";
                wsEnum.Cells[rowIndex, 10].Value = "น้ำหนัก (KG.)";
                wsEnum.Cells[rowIndex, 11].Value = "มูลค่า (บาท)";

                wsEnum.Cells[rowIndex, 12].Value = "ตัดจากใบขนสินค้านำเข้าคลังฯเลขที่";
                wsEnum.Cells[rowIndex, 13].Value = "รายการที่อ้างอิงถึง";

                wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                rowIndex++;

                rowDecl = 1;

                foreach (var exp in exps)
                {
                    currentExportDeclaration = exp.ExportDeclarationNo;
                    wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                    wsEnum.Cells[rowIndex, 2].Value = exp.ExportDeclarationNo;
                    IEnumerable<ExportDeclarationReport> expds = items.Where(x => x.ExportDeclarationNo == exp.ExportDeclarationNo);
                    int rowCount = 1;
                    foreach (ExportDeclarationReport expd in expds)
                    {
                        if (expd.InvoiceItemNo == 0)
                            wsEnum.Cells[rowIndex, 3].Value = string.Format("{0:00000}", rowCount);
                        else
                            wsEnum.Cells[rowIndex, 3].Value = string.Format("{0:00000}", expd.InvoiceItemNo);

                        wsEnum.Cells[rowIndex, 4].Value = expd.ExportName;
                        wsEnum.Cells[rowIndex, 5].Value = expd.ExportDeclarationDate;
                        wsEnum.Cells[rowIndex, 6].Value = expd.Sku;
                        if (serverType == "2")
                            wsEnum.Cells[rowIndex, 7].Value = expd.AltSkuDescription;
                        else
                            wsEnum.Cells[rowIndex, 7].Value = expd.SkuDescription;


                        wsEnum.Cells[rowIndex, 8].Value = expd.Quantity;
                        wsEnum.Cells[rowIndex, 9].Value = expd.UOM;
                        wsEnum.Cells[rowIndex, 10].Value = expd.NetWgt;
                        wsEnum.Cells[rowIndex, 11].Value = expd.TotalAmount;

                        wsEnum.Cells[rowIndex, 12].Value = expd.ImportDeclarationNo;
                        wsEnum.Cells[rowIndex, 13].Value = expd.ImportDeclarationItemNo;


                        rowIndex++;
                        rowCount++;
                    }

                    wsEnum.Cells[rowIndex, 1].Value = "รวม";
                    wsEnum.Cells[string.Format("A{0}:G{1}", rowIndex, rowIndex)].Merge = true;
                    wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsEnum.Cells[rowIndex, 8].Value = items.Where(x => x.ExportDeclarationNo == currentExportDeclaration).Sum(pk => pk.Quantity);
                    wsEnum.Cells[rowIndex, 10].Value = items.Where(x => x.ExportDeclarationNo == currentExportDeclaration).Sum(pk => pk.NetWgt);
                    wsEnum.Cells[rowIndex, 11].Value = items.Where(x => x.ExportDeclarationNo == currentExportDeclaration).Sum(pk => pk.TotalAmount);

                    wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                    wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                    grandTotalQty += Convert.ToDecimal(wsEnum.Cells[rowIndex, 8].Value);
                    grandTotalNetWgt += Convert.ToDecimal(wsEnum.Cells[rowIndex, 10].Value);
                    grandTotalAmount += Convert.ToDecimal(wsEnum.Cells[rowIndex, 11].Value);

                    rowIndex++;
                    rowDecl++;
                }


                // Footer //
                wsEnum.Cells[rowIndex, 1].Value = "รวมทั้งสิ้น";
                wsEnum.Cells[string.Format("A{0}:G{1}", rowIndex, rowIndex)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsEnum.Cells[rowIndex, 8].Value = grandTotalQty;// items.Sum(pk => pk.Quantity);
                wsEnum.Cells[rowIndex, 10].Value = grandTotalNetWgt;// items.Sum(pk => pk.NetWgt);
                wsEnum.Cells[rowIndex, 11].Value = grandTotalAmount;// items.Sum(pk => pk.TotalAmount);

                wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                wsEnum.Cells["A1:M" + rowIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                wsEnum.Cells["A1:M" + rowIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                // End Footer //

                // Formatting //
                wsEnum.Cells["E2:E" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
                wsEnum.Cells["H2:H" + rowIndex].Style.Numberformat.Format = "#,##0";
                wsEnum.Cells["J2:J" + rowIndex].Style.Numberformat.Format = "#,##0.000";
                wsEnum.Cells["K2:K" + rowIndex].Style.Numberformat.Format = "#,##0.00";

                wsEnum.Cells[wsEnum.Dimension.Address].AutoFitColumns();


            }


            MemoryStream memoryStream = new MemoryStream();

            pck.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        private MemoryStream GetStreamExportDeclarationFreezoneNew(IEnumerable<ExportDeclarationReport> items, string dateTitle, string exporterTitle)
        {
            decimal grandTotalQty = 0;
            decimal grandTotalNetWgt = 0;
            decimal grandTotalAmount = 0;

            string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;
            string companyNameHeader = string.Empty;

            companyNameHeader = "เขตปลอดอากร (Free Zone : FZ)";

            int rowIndex = 1;
            int rowDecl = 1;
            string currentExportDeclaration = string.Empty;

            ExcelPackage pck = new ExcelPackage();

            var reportTypes = items.GroupBy(
                                i => i.ReportType,
                                (key, group) => group.First()
                            ).ToArray();

            foreach (var reportType in reportTypes)
            {
                grandTotalQty = 0;
                grandTotalNetWgt = 0;
                grandTotalAmount = 0;

                var exps = items.GroupBy(
                                              i => i.ExportDeclarationNo,
                                              (key, group) => group.First()
                                          )
                                          .Where(i => i.ReportType == reportType.ReportType.ToString())
                                          .ToArray();


                var wsEnum = pck.Workbook.Worksheets.Add(reportType.ReportType.ToString());

                wsEnum.Cells[string.Format("A{0}:K{1}", 3, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[3, 1].Value = string.Format("{0} บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396", companyNameHeader);
                wsEnum.Cells[string.Format("A{0}:K{1}", 3, 3)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:K{1}", 4, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[4, 1].Value = string.Format("รายงานของที่นำออกจาก{0}", companyNameHeader);
                wsEnum.Cells[string.Format("A{0}:K{1}", 4, 4)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:K{1}", 5, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[5, 1].Value = string.Format("ประเภท : {0}", reportType.ReportType.ToString());
                wsEnum.Cells[string.Format("A{0}:K{1}", 5, 5)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:K{1}", 6, 6)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[6, 1].Value = dateTitle;
                wsEnum.Cells[string.Format("A{0}:K{1}", 6, 6)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 6, 6)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells[string.Format("A{0}:K{1}", 7, 7)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);

                rowIndex = 8;

                wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
                wsEnum.Cells[rowIndex, 2].Value = "เลขที่ใบขนสินค้า/คำร้อง";
                wsEnum.Cells[rowIndex, 3].Value = "เลขที่ Invoice";
                wsEnum.Cells[rowIndex, 4].Value = "วันที่สถานะใบขนฯ 0๔0๙";
                wsEnum.Cells[rowIndex, 5].Value = "รหัสสินค้า/วัตถุดิบ";
                wsEnum.Cells[rowIndex, 6].Value = "รายละเอียดสินค้า";
                wsEnum.Cells[rowIndex, 7].Value = "ปริมาณ";
                wsEnum.Cells[rowIndex, 8].Value = "หน่วยนับ";
                wsEnum.Cells[rowIndex, 9].Value = "น้ำหนัก (KG)";
                wsEnum.Cells[rowIndex, 10].Value = "ราคาต่อหน่วย";
                wsEnum.Cells[rowIndex, 11].Value = "มูลค่า (บาท)";

                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                rowIndex++;

                rowDecl = 1;

                foreach (var exp in exps)
                {
                    currentExportDeclaration = exp.ExportDeclarationNo;
                    wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                    wsEnum.Cells[rowIndex, 2].Value = exp.ExportDeclarationNo;
                    
                    IEnumerable<ExportDeclarationReport> expds = items.Where(x => x.ExportDeclarationNo == exp.ExportDeclarationNo);
                    int rowCount = 1;
                    foreach (ExportDeclarationReport expd in expds)
                    {
                        wsEnum.Cells[rowIndex, 3].Value = expd.ExportInvoiceNo;
                        wsEnum.Cells[rowIndex, 4].Value = expd.GoodsLoadedDate;
                        wsEnum.Cells[rowIndex, 5].Value = expd.Sku;
                        wsEnum.Cells[rowIndex, 6].Value = expd.SkuDescription;
                        wsEnum.Cells[rowIndex, 7].Value = expd.Quantity;
                        wsEnum.Cells[rowIndex, 8].Value = expd.UOM;
                        wsEnum.Cells[rowIndex, 9].Value = expd.NetWgt;
                        if (expd.Quantity == 0)
                        {
                            wsEnum.Cells[rowIndex, 10].Value = expd.UnitPrice;
                        }
                        else
                        {
                            wsEnum.Cells[rowIndex, 10].Value = (expd.TotalAmount / expd.Quantity);
                        }
                        wsEnum.Cells[rowIndex, 11].Value = expd.TotalAmount;


                        rowIndex++;
                        rowCount++;
                    }

                    wsEnum.Cells[rowIndex, 1].Value = "รวม";
                    wsEnum.Cells[string.Format("A{0}:F{1}", rowIndex, rowIndex)].Merge = true;
                    wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsEnum.Cells[rowIndex, 7].Value = items.Where(x => x.ExportDeclarationNo == currentExportDeclaration).Sum(pk => pk.Quantity);
                    wsEnum.Cells[rowIndex, 9].Value = items.Where(x => x.ExportDeclarationNo == currentExportDeclaration).Sum(pk => pk.NetWgt);
                    wsEnum.Cells[rowIndex, 11].Value = items.Where(x => x.ExportDeclarationNo == currentExportDeclaration).Sum(pk => pk.TotalAmount);

                    wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                    wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                    grandTotalQty += Convert.ToDecimal(wsEnum.Cells[rowIndex, 7].Value);
                    grandTotalNetWgt += Convert.ToDecimal(wsEnum.Cells[rowIndex, 9].Value);
                    grandTotalAmount += Convert.ToDecimal(wsEnum.Cells[rowIndex, 11].Value);

                    rowIndex++;
                    rowDecl++;
                }


                // Footer //
                wsEnum.Cells[rowIndex, 1].Value = "รวมทั้งสิ้น";
                wsEnum.Cells[string.Format("A{0}:F{1}", rowIndex, rowIndex)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsEnum.Cells[rowIndex, 7].Value = grandTotalQty;
                wsEnum.Cells[rowIndex, 9].Value = grandTotalNetWgt;
                wsEnum.Cells[rowIndex, 11].Value = grandTotalAmount;

                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                wsEnum.Cells["A1:K" + rowIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                wsEnum.Cells["A1:K" + rowIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                // End Footer //

                // Formatting //
                wsEnum.Cells["D2:D" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
                wsEnum.Cells["G2:G" + rowIndex].Style.Numberformat.Format = "#,##0";
                wsEnum.Cells["I2:I" + rowIndex].Style.Numberformat.Format = "#,##0.000";
                wsEnum.Cells["J2:K" + rowIndex].Style.Numberformat.Format = "#,##0.00";

                wsEnum.Cells[wsEnum.Dimension.Address].AutoFitColumns();


            }


            MemoryStream memoryStream = new MemoryStream();

            pck.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        private MemoryStream GetStreamStockDeclaration(IEnumerable<GLDeclarationReport> items, string dateTitle, string importerTitle)
        {
            string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;
            string companyNameHeader = string.Empty;
            if (serverType == "1")
                companyNameHeader = "เขตปลอดอากร (Free Zone : FZ)";
            else
                companyNameHeader = "คลังสินค้าทัณฑ์บนทั่วไป";

            int rowIndex = 1;
            int rowDecl = 1;
            string currentReceiptKey = string.Empty;

            ExcelPackage pck = new ExcelPackage();

            var wsEnum = pck.Workbook.Worksheets.Add("รายงานคงเหลือ");

            //Add the headers
            //Solid black border around the board.
            wsEnum.Cells[string.Format("A{0}:L{1}", 1, 1)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[1, 1].Value = "แบบแนบท้ายประมวลฯ ข้อ ๕ ๐๒ ๐๒ ๒๗";
            wsEnum.Cells[string.Format("A{0}:L{1}", 1, 1)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 1, 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            wsEnum.Cells[string.Format("A{0}:L{1}", 2, 2)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[2, 1].Value = "คทบ ๑๗";
            wsEnum.Cells[string.Format("A{0}:L{1}", 2, 2)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 2, 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            wsEnum.Cells[string.Format("A{0}:L{1}", 3, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[3, 1].Value = string.Format("{0} บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396", companyNameHeader);
            wsEnum.Cells[string.Format("A{0}:L{1}", 3, 3)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:L{1}", 4, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[4, 1].Value = "รายงานคงเหลือ";
            wsEnum.Cells[string.Format("A{0}:L{1}", 4, 4)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:L{1}", 5, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[5, 1].Value = string.Format("ณ วันสิ้นงวด วันที่ {0}", dateTitle);
            wsEnum.Cells[string.Format("A{0}:L{1}", 5, 5)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells[string.Format("A{0}:L{1}", 6, 6)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);

            rowIndex = 7;

            wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
            wsEnum.Cells[rowIndex, 2].Value = "เลขที่ใบขนสินค้านำเข้าคลังฯ";
            wsEnum.Cells[rowIndex, 3].Value = "รายการที่";
            wsEnum.Cells[rowIndex, 4].Value = "ชื่อผู้นำเข้า";
            wsEnum.Cells[rowIndex, 5].Value = "วันที่นำเข้า";
            wsEnum.Cells[rowIndex, 6].Value = "วันที่นำเข้าคลังฯ";
            wsEnum.Cells[rowIndex, 7].Value = "ชื่อของ";
            wsEnum.Cells[rowIndex, 8].Value = "ปริมาณ";
            wsEnum.Cells[rowIndex, 9].Value = "หน่วยนับ";
            wsEnum.Cells[rowIndex, 10].Value = "น้ำหนัก";
            wsEnum.Cells[rowIndex, 11].Value = "มูลค่า (บาท)";
            wsEnum.Cells[rowIndex, 12].Value = "ภาษีอากรรวม (บาท)";

            wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
            wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

            rowIndex++;

            rowDecl = 1;

            var imps = items.GroupBy(
                                             i => i.ImportDeclarationNo,
                                             (key, group) => group.First()
                                         )
                                         .ToArray();

            foreach (var item in imps)
            {
                currentReceiptKey = item.ImportDeclarationNo;
                wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                wsEnum.Cells[rowIndex, 2].Value = item.ImportDeclarationNo;
                IEnumerable<GLDeclarationReport> itemds = items.Where(x => x.ImportDeclarationNo == currentReceiptKey);
                foreach (GLDeclarationReport itemd in itemds)
                {
                    wsEnum.Cells[rowIndex, 3].Value = itemd.ImportDeclarationItemNo;
                    wsEnum.Cells[rowIndex, 4].Value = itemd.ImporterName;
                    wsEnum.Cells[rowIndex, 5].Value = itemd.ImportDeclarationDate;
                    wsEnum.Cells[rowIndex, 6].Value = itemd.WarehouseReceivedDate;
                    wsEnum.Cells[rowIndex, 7].Value = itemd.SkuDescription;
                    wsEnum.Cells[rowIndex, 8].Value = itemd.QtyBalance;
                    wsEnum.Cells[rowIndex, 9].Value = itemd.UOM;
                    wsEnum.Cells[rowIndex, 10].Value = itemd.NetWgt;
                    wsEnum.Cells[rowIndex, 11].Value = itemd.TotalAmount;
                    wsEnum.Cells[rowIndex, 12].Value = itemd.TotalDuty;
                    rowIndex++;
                }

                wsEnum.Cells[rowIndex, 1].Value = "รวม";
                wsEnum.Cells[string.Format("A{0}:G{1}", rowIndex, rowIndex)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsEnum.Cells[rowIndex, 8].Value = items.Where(x => x.ImportDeclarationNo == currentReceiptKey).Sum(pk => pk.QtyBalance);
                wsEnum.Cells[rowIndex, 10].Value = items.Where(x => x.ImportDeclarationNo == currentReceiptKey).Sum(pk => pk.NetWgt);
                wsEnum.Cells[rowIndex, 11].Value = items.Where(x => x.ImportDeclarationNo == currentReceiptKey).Sum(pk => pk.TotalAmount);
                wsEnum.Cells[rowIndex, 12].Value = items.Where(x => x.ImportDeclarationNo == currentReceiptKey).Sum(pk => pk.TotalDuty);
                wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                rowIndex++;
                rowDecl++;
            }


            // Footer //
            wsEnum.Cells[rowIndex, 1].Value = "รวมทั้งสิ้น";
            wsEnum.Cells[string.Format("A{0}:G{1}", rowIndex, rowIndex)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            wsEnum.Cells[rowIndex, 8].Value = items.Sum(pk => pk.QtyBalance);
            wsEnum.Cells[rowIndex, 10].Value = items.Sum(pk => pk.NetWgt);
            wsEnum.Cells[rowIndex, 11].Value = items.Sum(pk => pk.TotalAmount);
            wsEnum.Cells[rowIndex, 12].Value = items.Sum(pk => pk.TotalDuty);
            wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
            wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsEnum.Cells[string.Format("A{0}:L{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

            wsEnum.Cells["A1:L" + rowIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            wsEnum.Cells["A1:L" + rowIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            // End Footer //

            // Formatting //
            wsEnum.Cells["E2:E" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            wsEnum.Cells["F2:F" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            wsEnum.Cells["H2:H" + rowIndex].Style.Numberformat.Format = "#,##0";
            wsEnum.Cells["I2:L" + rowIndex].Style.Numberformat.Format = "#,##0.00";

            wsEnum.Cells[wsEnum.Dimension.Address].AutoFitColumns();

            MemoryStream memoryStream = new MemoryStream();

            pck.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        private MemoryStream GetStreamStockDeclarationFreezone(IEnumerable<GLDeclarationReport> items, string dateTitle, string importerTitle)
        {
            string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;
            string companyNameHeader = string.Empty;

            companyNameHeader = "เขตปลอดอากร (Free Zone : FZ)";

            int rowIndex = 1;
            int rowDecl = 1;
            string currentReceiptKey = string.Empty;

            ExcelPackage pck = new ExcelPackage();

            var wsEnum = pck.Workbook.Worksheets.Add("รายงานคงเหลือ");
            //SET HYPERLINK STYLE-----------------------  
            string StyleName = "HyperStyle";
            ExcelNamedStyleXml HyperStyle = wsEnum.Workbook.Styles.CreateNamedStyle(StyleName);
            HyperStyle.Style.Font.UnderLine = true;
            HyperStyle.Style.Font.Size = 12;
            HyperStyle.Style.Font.Color.SetColor(Color.Blue);

            wsEnum.Cells[string.Format("A{0}:N{1}", 3, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[3, 1].Value = string.Format("{0} บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396", companyNameHeader);
            wsEnum.Cells[string.Format("A{0}:N{1}", 3, 3)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:N{1}", 4, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[4, 1].Value = "รายงานคงเหลือ";
            wsEnum.Cells[string.Format("A{0}:N{1}", 4, 4)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:N{1}", 5, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[5, 1].Value = string.Format("ณ วันสิ้นงวด วันที่ {0}", dateTitle);
            wsEnum.Cells[string.Format("A{0}:N{1}", 5, 5)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells[string.Format("A{0}:N{1}", 6, 6)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);

            rowIndex = 7;

            wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
            wsEnum.Cells[rowIndex, 2].Value = "เลขที่ใบขนสินค้านำเข้าคลังฯ";
            wsEnum.Cells[rowIndex, 3].Value = "รายการที่";
            wsEnum.Cells[rowIndex, 4].Value = "ชื่อผู้นำเข้า";
            wsEnum.Cells[rowIndex, 5].Value = "วันที่นำเข้าคลังฯ";
            wsEnum.Cells[rowIndex, 6].Value = "รหัสสินค้า";
            wsEnum.Cells[rowIndex, 7].Value = "ชื่อของ";
            wsEnum.Cells[rowIndex, 8].Value = "ปริมาณ";
            wsEnum.Cells[rowIndex, 9].Value = "หน่วยนับ";
            wsEnum.Cells[rowIndex, 10].Value = "น้ำหนัก (KG.)";
            wsEnum.Cells[rowIndex, 11].Value = "ราคาต่อหน่วย";
            wsEnum.Cells[rowIndex, 12].Value = "มูลค่า (บาท)";

            wsEnum.Cells[rowIndex, 13].Value = "สถานที่จัดเก็บ";
            wsEnum.Cells[rowIndex, 14].Value = "หมายเลขกล้อง";


            wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
            wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

            rowIndex++;

            rowDecl = 1;

            var imps = items.GroupBy(
                i => i.ImportDeclarationNo,
                (key, group) => group.First()
            )
            .ToArray();

            foreach (var item in imps)
            {
                currentReceiptKey = item.ImportDeclarationNo;
                wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                wsEnum.Cells[rowIndex, 2].Value = item.ImportDeclarationNo;
                IEnumerable<GLDeclarationReport> itemds = items.Where(x => x.ImportDeclarationNo == currentReceiptKey);
                foreach (GLDeclarationReport itemd in itemds)
                {
                    wsEnum.Cells[rowIndex, 3].Value = itemd.ImportDeclarationItemNo;
                    wsEnum.Cells[rowIndex, 4].Value = itemd.ImporterName;
                    wsEnum.Cells[rowIndex, 5].Value = itemd.ImportDeclarationDate;
                    wsEnum.Cells[rowIndex, 6].Value = itemd.Sku;
                    wsEnum.Cells[rowIndex, 7].Value = itemd.SkuDescription;
                    wsEnum.Cells[rowIndex, 8].Value = itemd.QtyBalance;
                    wsEnum.Cells[rowIndex, 9].Value = itemd.UOM;
                    wsEnum.Cells[rowIndex, 10].Value = itemd.NetWgt;
                    wsEnum.Cells[rowIndex, 11].Value = (itemd.TotalAmount/ itemd.QtyBalance) ;
                    wsEnum.Cells[rowIndex, 12].Value = itemd.TotalAmount;
                    wsEnum.Cells[rowIndex, 13].Value = itemd.BinLocation;
                    //wsEnum.Cells[rowIndex, 14].Value = itemd.CameraNo;

                   
                    var cell = wsEnum.Cells[rowIndex, 14];
                    cell.Value = itemd.CameraNo.UnNull().Trim();
                    if (itemd.CameraLink.UnNull().Trim() != String.Empty)
                    {
                        cell.Hyperlink = new Uri(itemd.CameraLink.UnNull().Trim());
                        cell.StyleName = StyleName;
                    }


                    rowIndex++;
                }

                wsEnum.Cells[rowIndex, 1].Value = "รวม";
                wsEnum.Cells[string.Format("A{0}:F{1}", rowIndex, rowIndex)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsEnum.Cells[rowIndex, 8].Value = items.Where(x => x.ImportDeclarationNo == currentReceiptKey).Sum(pk => pk.QtyBalance);
                wsEnum.Cells[rowIndex, 10].Value = items.Where(x => x.ImportDeclarationNo == currentReceiptKey).Sum(pk => pk.NetWgt);
                wsEnum.Cells[rowIndex, 12].Value = items.Where(x => x.ImportDeclarationNo == currentReceiptKey).Sum(pk => pk.TotalAmount);
                wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                rowIndex++;
                rowDecl++;
            }


            // Footer //
            wsEnum.Cells[rowIndex, 1].Value = "รวมทั้งสิ้น";
            wsEnum.Cells[string.Format("A{0}:F{1}", rowIndex, rowIndex)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            wsEnum.Cells[rowIndex, 8].Value = items.Sum(pk => pk.QtyBalance);
            wsEnum.Cells[rowIndex, 10].Value = items.Sum(pk => pk.NetWgt);
            wsEnum.Cells[rowIndex, 12].Value = items.Sum(pk => pk.TotalAmount);
            wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
            wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

            wsEnum.Cells["A1:N" + rowIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            wsEnum.Cells["A1:N" + rowIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            // End Footer //

            // Formatting //
            wsEnum.Cells["E2:E" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            wsEnum.Cells["H2:H" + rowIndex].Style.Numberformat.Format = "#,##0";
            wsEnum.Cells["J2:J" + rowIndex].Style.Numberformat.Format = "#,##0.000";
            wsEnum.Cells["K2:L" + rowIndex].Style.Numberformat.Format = "#,##0.00";

            wsEnum.Cells[wsEnum.Dimension.Address].AutoFitColumns();

            MemoryStream memoryStream = new MemoryStream();

            pck.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        private MemoryStream GetStreamLedgerDeclaration(IEnumerable<GLDeclarationReport> items, string dateTitle, string importerTitle)
        {
            Int64 decCount = 0;
            string decTextProgress;
            string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;
            string companyNameHeader = string.Empty;
            if (serverType == "1")
                companyNameHeader = "เขตปลอดอากร (Free Zone : FZ)";
            else
                companyNameHeader = "คลังสินค้าทัณฑ์บนทั่วไป";

            int rowIndex = 1;
            int rowDecl = 1;
            decimal currentQty = 0;
            decimal currentWeight = 0;
            string endColumnName = "N";
            string currentReceipItem = string.Empty;
            string dummyId = string.Empty;
            try
            {
                ExcelPackage pck = new ExcelPackage();
                var wsEnum = pck.Workbook.Worksheets.Add("GLDeclaration16");

                //Add the headers
                //Solid black border around the board.
                wsEnum.Cells[string.Format("A{0}:{1}{2}", 1, endColumnName, 1)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[1, 1].Value = "แบบแนบท้ายประมวลฯ ข้อ ๕ ๐๒ ๐๒ ๒๗";
                wsEnum.Cells[string.Format("A{0}:{1}{2}", 1, endColumnName, 1)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 1, 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                wsEnum.Cells[string.Format("A{0}:{1}{2}", 2, endColumnName, 2)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[2, 1].Value = "คทบ ๑๖";
                wsEnum.Cells[string.Format("A{0}:{1}{2}", 2, endColumnName, 2)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 2, 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                wsEnum.Cells[string.Format("A{0}:{1}{2}", 3, endColumnName, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[3, 1].Value = string.Format("{0} บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396", companyNameHeader);
                wsEnum.Cells[string.Format("A{0}:{1}{2}", 3, endColumnName, 3)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:{1}{2}", 4, endColumnName, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[4, 1].Value = "รายงานแยกประเภท";
                wsEnum.Cells[string.Format("A{0}:{1}{2}", 4, endColumnName, 4)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:{1}{2}", 5, endColumnName, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[5, 1].Value = string.Format("สำหรับงวด วันที่ {0}", dateTitle);
                wsEnum.Cells[string.Format("A{0}:{1}{2}", 5, endColumnName, 5)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                rowIndex = 7;

                wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
                wsEnum.Cells["A7:A8"].Merge = true;
                wsEnum.Cells["A7:A8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["A7:A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 2].Value = "เลขที่ใบขนสินค้านำเข้าคลังฯ";
                wsEnum.Cells["B7:B8"].Merge = true;
                wsEnum.Cells["B7:B8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["B7:B8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 3].Value = "รายการที่";
                wsEnum.Cells["C7:C8"].Merge = true;
                wsEnum.Cells["C7:C8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["C7:C8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 4].Value = "วันที่นำเข้า";
                wsEnum.Cells["D7:D8"].Merge = true;
                wsEnum.Cells["D7:D8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["D7:D8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 5].Value = "วันที่นำเข้าคลังฯ";
                wsEnum.Cells["E7:E8"].Merge = true;
                wsEnum.Cells["E7:E8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["E7:E8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 6].Value = "ชื่อของ";
                wsEnum.Cells["F7:F8"].Merge = true;
                wsEnum.Cells["F7:F8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["F7:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 7].Value = "ปริมาณ";
                wsEnum.Cells["G7:G8"].Merge = true;
                wsEnum.Cells["G7:G8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["G7:G8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 8].Value = "น้ำหนัก";
                wsEnum.Cells["H7:H8"].Merge = true;
                wsEnum.Cells["H7:H8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["H7:H8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 9].Value = "เลขที่ใบขนสินค้านำออกจากคลังฯ";
                wsEnum.Cells["I7:I8"].Merge = true;
                wsEnum.Cells["I7:I8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["I7:I8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 10].Value = "วันที่นำออก";
                wsEnum.Cells["J7:J8"].Merge = true;
                wsEnum.Cells["J7:J8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["J7:J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 11].Value = "ปริมาณ";
                wsEnum.Cells["K7:K8"].Merge = true;
                wsEnum.Cells["K7:K8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["K7:K8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 12].Value = "น้ำหนัก";
                wsEnum.Cells["L7:L8"].Merge = true;
                wsEnum.Cells["L7:L8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["L7:L8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 13].Value = "คงเหลือ";
                wsEnum.Cells["M7:N7"].Merge = true;
                wsEnum.Cells["M7:N7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells["M7:N8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 13].Value = "คงเหลือ";
                wsEnum.Cells[rowIndex + 1, 13].Value = "ปริมาณ";
                wsEnum.Cells[rowIndex + 1, 14].Value = "น้ำหนัก";

                wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex + 1)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex + 1)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex + 1)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                rowIndex = 9;

                var receipts = items.GroupBy(x => new { x.ReceiptKey })
                        .Select(y => new GLDeclarationReport()
                        {
                            ReceiptKey = y.Key.ReceiptKey
                        }
                        );
                decCount = receipts.Count();

                foreach (var sn in receipts)
                {
                    var exps = items
                           .Where(x => x.ReceiptKey == sn.ReceiptKey)
                           .GroupBy(x => new { x.ReceiptKey, x.ImportDeclarationItemNo, x.ImportDeclarationNo })
                           .Select(y => new GLDeclarationReport()
                           {
                               ReceiptKey = y.Key.ReceiptKey,
                               ImportDeclarationItemNo = y.Key.ImportDeclarationItemNo,
                               ImportDeclarationNo = y.Key.ImportDeclarationNo
                           }
                           );

                    wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                    wsEnum.Cells[rowIndex, 2].Value = exps.ToList()[0].ImportDeclarationNo.UnNull();
                    foreach (var exp in exps)
                    {
                        IEnumerable<GLDeclarationReport> expds = items.Where(x => x.ReceiptKey == exp.ReceiptKey && x.ImportDeclarationItemNo == exp.ImportDeclarationItemNo);

                        wsEnum.Cells[rowIndex, 3].Value = expds.ToList()[0].ImportDeclarationItemNo;
                        wsEnum.Cells[rowIndex, 4].Value = expds.ToList()[0].ImportDeclarationDate;
                        wsEnum.Cells[rowIndex, 5].Value = expds.ToList()[0].WarehouseReceivedDate;
                        wsEnum.Cells[rowIndex, 6].Value = expds.ToList()[0].SkuDescription;
                        wsEnum.Cells[rowIndex, 7].Value = expds.ToList()[0].Quantity;
                        wsEnum.Cells[rowIndex, 8].Value = expds.ToList()[0].NetWgt * expds.ToList()[0].Quantity;

                        currentQty = expds.ToList()[0].Quantity;
                        currentWeight = expds.ToList()[0].NetWgt * expds.ToList()[0].Quantity;

                        foreach (GLDeclarationReport expd in expds)
                        {
                            dummyId = expd.ReceiptKey;
                            decimal unitWeight = expd.NetWgt;
                            //currentWeight -= unitWeight;
                            currentQty -= expd.QtyPicked;

                            wsEnum.Cells[rowIndex, 9].Value = expd.ExportDeclarationNo.UnNull();
                            wsEnum.Cells[rowIndex, 10].Value = (expd.ExportDeclarationDate == DateTime.MinValue ? null : expd.ExportDeclarationDate);
                            wsEnum.Cells[rowIndex, 11].Value = expd.QtyPicked;
                            wsEnum.Cells[rowIndex, 12].Value = unitWeight * expd.QtyPicked;

                            wsEnum.Cells[rowIndex, 13].Value = currentQty;
                            wsEnum.Cells[rowIndex, 14].Value = unitWeight * currentQty;

                            rowIndex++;
                        }
                    }


                    //wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Merge = true;
                    //wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    //wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                    //wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //wsEnum.Cells[string.Format("A{0}:N{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                    rowIndex++;
                    rowDecl++;
                    decTextProgress = string.Format("{0}/{1}", rowDecl, decCount);
                }

                // Formatting //
                //wsEnum.Cells["D2:D" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
                //wsEnum.Cells["E2:E" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
                //wsEnum.Cells["J2:J" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
                //wsEnum.Cells["G2:G" + rowIndex].Style.Numberformat.Format = "#,##0";
                //wsEnum.Cells["H2:H" + rowIndex].Style.Numberformat.Format = "#,##0.00";
                //wsEnum.Cells["K2:K" + rowIndex].Style.Numberformat.Format = "#,##0";
                //wsEnum.Cells["L2:L" + rowIndex].Style.Numberformat.Format = "#,##0.00";
                //wsEnum.Cells["M2:M" + rowIndex].Style.Numberformat.Format = "#,##0";
                //var modelCells = wsEnum.Cells[string.Format("A7:{0}{1}", endColumnName, rowIndex - 1)];
                //var border = modelCells.Style.Border.Top.Style = modelCells.Style.Border.Left.Style = modelCells.Style.Border.Right.Style = modelCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                //wsEnum.Cells[wsEnum.Dimension.Address].AutoFitColumns();

                MemoryStream memoryStream = new MemoryStream();

                pck.SaveAs(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private MemoryStream GetStreamLedgerDeclarationFreezone(InventoryMovementReport items, string dateTitle, string importerTitle)
        {
            IEnumerable<ImportDeclarationReport> imps = items.ImportDeclarationItems;
            IEnumerable<ExportDeclarationReport> exps = items.ExportDeclarationItems;
            string dummyImportDec = string.Empty;
            string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;
            string companyNameHeader = string.Empty;
            companyNameHeader = "เขตปลอดอากร (Free Zone : FZ)";

            int rowIndex = 1;
            int rowDecl = 1;

            decimal currentQty = 0;
            decimal currentWeight = 0;
            string endColumnName = "M";
            string currentReceipItem = string.Empty;
            string dummyId = string.Empty;
            try
            {
                ExcelPackage pck = new ExcelPackage();
                var wsEnum = pck.Workbook.Worksheets.Add("GLDeclaration16");

                wsEnum.Cells[string.Format("A{0}:{1}{2}", 3, endColumnName, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[3, 1].Value = string.Format("{0} บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396", companyNameHeader);
                wsEnum.Cells[string.Format("A{0}:{1}{2}", 3, endColumnName, 3)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:{1}{2}", 4, endColumnName, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[4, 1].Value = "รายงานแยกประเภท";
                wsEnum.Cells[string.Format("A{0}:{1}{2}", 4, endColumnName, 4)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:{1}{2}", 5, endColumnName, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[5, 1].Value = string.Format("สำหรับงวด วันที่ {0}", dateTitle);
                wsEnum.Cells[string.Format("A{0}:{1}{2}", 5, endColumnName, 5)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                rowIndex = 7;

                wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
                wsEnum.Cells["A7:A8"].Merge = true;
                wsEnum.Cells["A7:A8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["A7:A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 2].Value = "เลขที่ใบขนสินค้านำเข้าคลังฯ";
                wsEnum.Cells["B7:B8"].Merge = true;
                wsEnum.Cells["B7:B8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["B7:B8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 3].Value = "รายการที่";
                wsEnum.Cells["C7:C8"].Merge = true;
                wsEnum.Cells["C7:C8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["C7:C8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 4].Value = "วันที่นำเข้าคลังฯ";
                wsEnum.Cells["D7:D8"].Merge = true;
                wsEnum.Cells["D7:D8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["D7:D8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 5].Value = "ชื่อของ";
                wsEnum.Cells["E7:E8"].Merge = true;
                wsEnum.Cells["E7:E8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["E7:E8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 6].Value = "ปริมาณ";
                wsEnum.Cells["F7:F8"].Merge = true;
                wsEnum.Cells["F7:F8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["F7:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 7].Value = "น้ำหนัก";
                wsEnum.Cells["G7:G8"].Merge = true;
                wsEnum.Cells["G7:G8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["G7:G8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 8].Value = "เลขที่ใบขนสินค้านำออกจากคลังฯ";
                wsEnum.Cells["H7:H8"].Merge = true;
                wsEnum.Cells["H7:H8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["H7:H8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 9].Value = "วันที่นำออก";
                wsEnum.Cells["I7:I8"].Merge = true;
                wsEnum.Cells["I7:I8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["I7:I8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 10].Value = "ปริมาณ";
                wsEnum.Cells["J7:J8"].Merge = true;
                wsEnum.Cells["J7:J8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["J7:J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 11].Value = "น้ำหนัก";
                wsEnum.Cells["K7:K8"].Merge = true;
                wsEnum.Cells["K7:K8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["K7:K8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 12].Value = "คงเหลือ";
                wsEnum.Cells["L7:M7"].Merge = true;
                wsEnum.Cells["L7:L7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells["L7:L8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 12].Value = "คงเหลือ";
                wsEnum.Cells[rowIndex + 1, 12].Value = "ปริมาณ";
                wsEnum.Cells[rowIndex + 1, 13].Value = "น้ำหนัก";

                wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex + 1)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex + 1)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex + 1)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                rowIndex = 9;

                var grpImps = imps.GroupBy(x => new { x.ImportDeclarationNo })
                       .Select(y => new GLDeclarationReport()
                       {
                           ImportDeclarationNo = y.Key.ImportDeclarationNo
                       }
                       );

                foreach (var grpImp in grpImps)
                {
                    wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                    wsEnum.Cells[rowIndex, 2].Value = grpImp.ImportDeclarationNo;
                    var impls = imps
                                .Where(x => x.ImportDeclarationNo == grpImp.ImportDeclarationNo)
                                .GroupBy(x => new { x.ReceiptKey, x.ImportDeclarationItemNo, x.ImportDeclarationNo })
                                .Select(y => new GLDeclarationReport()
                                {
                                    ReceiptKey = y.Key.ReceiptKey,
                                    ImportDeclarationItemNo = y.Key.ImportDeclarationItemNo,
                                    ImportDeclarationNo = y.Key.ImportDeclarationNo
                                }
                                );

                    foreach (var impl in impls)
                    {
                        IEnumerable<ImportDeclarationReport> impds = imps.Where(x => x.ImportDeclarationNo == impl.ImportDeclarationNo && x.ImportDeclarationItemNo == impl.ImportDeclarationItemNo);

                        foreach (var impd in impds)
                        {
                            wsEnum.Cells[rowIndex, 3].Value = impds.ToList()[0].ImportDeclarationItemNo;
                            wsEnum.Cells[rowIndex, 4].Value = impds.ToList()[0].ImportDeclarationDate;
                            wsEnum.Cells[rowIndex, 5].Value = impds.ToList()[0].SkuDescription;
                            wsEnum.Cells[rowIndex, 6].Value = impds.ToList()[0].Quantity;
                            wsEnum.Cells[rowIndex, 7].Value = impds.ToList()[0].NetWgt;

                            currentQty = impds.ToList()[0].Quantity;
                            currentWeight = impds.ToList()[0].NetWgt;
                            decimal unitWeight = currentWeight / currentQty;
                            IEnumerable<ExportDeclarationReport> expds = exps.Where(x => x.ImportDeclarationNo == impd.ImportDeclarationNo && x.ImportDeclarationItemNo == impd.ImportDeclarationItemNo);
                            if (expds.Count() == 0)
                            {
                                wsEnum.Cells[rowIndex, 8].Value = string.Empty;
                                wsEnum.Cells[rowIndex, 9].Value = null;
                                wsEnum.Cells[rowIndex, 10].Value = 0;
                                wsEnum.Cells[rowIndex, 11].Value = 0;
                                wsEnum.Cells[rowIndex, 12].Value = currentQty;
                                wsEnum.Cells[rowIndex, 13].Value = currentWeight;
                                rowIndex++;
                            }
                            else
                            {
                                foreach (var expd in expds)
                                {
                                    currentWeight -= unitWeight * expd.Quantity;
                                    currentQty -= expd.Quantity;

                                    wsEnum.Cells[rowIndex, 8].Value = expd.ExportDeclarationNo.UnNull();
                                    if (expd.ExportDeclarationDate == DateTime.MinValue)
                                        wsEnum.Cells[rowIndex, 9].Value = null;
                                    else
                                        wsEnum.Cells[rowIndex, 9].Value = expd.ExportDeclarationDate;

                                    wsEnum.Cells[rowIndex, 10].Value = expd.Quantity;
                                    wsEnum.Cells[rowIndex, 11].Value = unitWeight * expd.Quantity;

                                    wsEnum.Cells[rowIndex, 12].Value = currentQty;
                                    wsEnum.Cells[rowIndex, 13].Value = currentWeight;

                                    rowIndex++;
                                }
                            }

                        }

                    }

                    //wsEnum.Cells[rowIndex, 1].Value = "รวม";
                    wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Merge = true;
                    wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    //wsEnum.Cells[rowIndex, 12].Value = accumulateBoh;
                    //wsEnum.Cells[rowIndex, 13].Value = currentWeight;
                    wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                    wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsEnum.Cells[string.Format("A{0}:M{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

                    rowIndex++;
                    rowDecl++;
                }

                // Formatting //
                wsEnum.Cells["D2:D" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
                wsEnum.Cells["I2:I" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";

                wsEnum.Cells["F2:F" + rowIndex].Style.Numberformat.Format = "#,##0";
                wsEnum.Cells["G2:G" + rowIndex].Style.Numberformat.Format = "#,##0.0000";
                wsEnum.Cells["J2:J" + rowIndex].Style.Numberformat.Format = "#,##0";
                wsEnum.Cells["K2:K" + rowIndex].Style.Numberformat.Format = "#,##0.0000";
                wsEnum.Cells["L2:L" + rowIndex].Style.Numberformat.Format = "#,##0";
                wsEnum.Cells["M2:M" + rowIndex].Style.Numberformat.Format = "#,##0.0000";

                var modelCells = wsEnum.Cells[string.Format("A7:{0}{1}", endColumnName, rowIndex - 1)];
                var border = modelCells.Style.Border.Top.Style = modelCells.Style.Border.Left.Style = modelCells.Style.Border.Right.Style = modelCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                wsEnum.Cells[string.Format("A1:{0}{1}", endColumnName, rowIndex - 1)].AutoFitColumns();
                wsEnum.Cells[wsEnum.Dimension.Address].AutoFitColumns();

                MemoryStream memoryStream = new MemoryStream();

                pck.SaveAs(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private MemoryStream GetStreamMovementDeclaration(InventoryMovementReport items, string dateTitle, string importerTitle)
        {
            string msg;
            bool result = true;
            int rowIndex = 1;
            int rowDecl = 1;
            decimal bohByDeclaration = 0;
            decimal totalShippedByDeclaration = 0;
            decimal totalImportQtyByDeclaration = 0;
            decimal totalBfQtyByDeclaration = 0;
            try
            {
                string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;
                string companyNameHeader = string.Empty;
                if (serverType == "1" || serverType == "2")
                    companyNameHeader = "เขตปลอดอากร (Free Zone : FZ)";
                else
                    companyNameHeader = "คลังสินค้าทัณฑ์บนทั่วไป";


                int firstRow = 0;
                decimal currentQty;
                decimal currentDutyUnit;
                string endColumnName = "S";
                string currentReceipItem = string.Empty;
                string currentReceiptKey = string.Empty;

                ExcelPackage pck = new ExcelPackage();

                var wsEnum = pck.Workbook.Worksheets.Add("MovementTrans18");

                wsEnum.Cells[string.Format("A{0}:{1}{2}", 1, endColumnName, 1)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[1, 1].Value = "แบบแนบท้ายประมวลฯ ข้อ ๕ ๐๒ ๐๒ ๒๗";
                wsEnum.Cells[string.Format("A{0}:{1}{2}", 1, endColumnName, 1)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 1, 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                wsEnum.Cells[string.Format("A{0}:{1}{2}", 2, endColumnName, 2)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[2, 1].Value = "คทบ ๑๘";
                wsEnum.Cells[string.Format("A{0}:{1}{2}", 2, endColumnName, 2)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 2, 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                wsEnum.Cells[string.Format("A{0}:{1}{2}", 3, endColumnName, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[3, 1].Value = string.Format("{0} บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396", companyNameHeader);
                wsEnum.Cells[string.Format("A{0}:{1}{2}", 3, endColumnName, 3)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:{1}{2}", 4, endColumnName, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[4, 1].Value = "รายงานการเคลื่อนไหวของของที่นำเข้า";
                wsEnum.Cells[string.Format("A{0}:{1}{2}", 4, endColumnName, 4)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[string.Format("A{0}:{1}{2}", 5, endColumnName, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
                wsEnum.Cells[5, 1].Value = dateTitle;
                wsEnum.Cells[string.Format("A{0}:{1}{2}", 5, endColumnName, 5)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                rowIndex = 7;

                wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
                wsEnum.Cells["A7:A8"].Merge = true;
                wsEnum.Cells["A7:A8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["A7:A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells["A7:A8"].Style.WrapText = true;

                wsEnum.Cells[rowIndex, 2].Value = "ชื่อผู้นำเข้า";
                wsEnum.Cells["B7:B8"].Merge = true;
                wsEnum.Cells["B7:B8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["B7:B8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells["B7:B8"].Style.WrapText = true;

                wsEnum.Cells[rowIndex, 3].Value = "วันที่นำเข้า";
                wsEnum.Cells["C7:C8"].Merge = true;
                wsEnum.Cells["C7:C8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["C7:C8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells["C7:C8"].Style.WrapText = true;

                wsEnum.Cells[rowIndex, 4].Value = "เลขที่ใบขนสินค้านำเข้าคลังฯ";
                wsEnum.Cells["D7:D8"].Merge = true;
                wsEnum.Cells["D7:D8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["D7:D8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells["D7:D8"].Style.WrapText = true;

                wsEnum.Cells[rowIndex, 5].Value = "รายการที่";
                wsEnum.Cells["E7:E8"].Merge = true;
                wsEnum.Cells["E7:E8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["E7:E8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells["E7:E8"].Style.WrapText = true;

                wsEnum.Cells[rowIndex, 6].Value = "ชนิดของเป็นภาษาอังกฤษ";
                wsEnum.Cells["F7:F8"].Merge = true;
                wsEnum.Cells["F7:F8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["F7:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells["F7:F8"].Style.WrapText = true;

                wsEnum.Cells[rowIndex, 7].Value = "หน่วยของปริมาณ";
                wsEnum.Cells["G7:G8"].Merge = true;
                wsEnum.Cells["G7:G8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsEnum.Cells["G7:G8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells["G7:G8"].Style.WrapText = true;

                wsEnum.Cells[rowIndex, 8].Value = "ยกมา";
                wsEnum.Cells[rowIndex + 1, 8].Value = "ปริมาณยกมา";
                wsEnum.Cells[rowIndex + 1, 9].Value = "มูลค่า (บาท)";
                wsEnum.Cells[rowIndex + 1, 10].Value = "ภาษีอากรรวม (บาท)";
                wsEnum.Cells[rowIndex + 1, 10].Style.WrapText = true;
                wsEnum.Cells["H7:J7"].Merge = true;
                wsEnum.Cells["H7:J7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsEnum.Cells[rowIndex, 11].Value = "นำเข้า";
                wsEnum.Cells[rowIndex + 1, 11].Value = "ปริมาณนำเข้า";
                wsEnum.Cells[rowIndex + 1, 12].Value = "มูลค่า (บาท)";
                wsEnum.Cells[rowIndex + 1, 13].Value = "ภาษีอากรรวม (บาท)";
                wsEnum.Cells[rowIndex + 1, 13].Style.WrapText = true;
                wsEnum.Cells["K7:M7"].Merge = true;
                wsEnum.Cells["K7:M7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                wsEnum.Cells[rowIndex, 14].Value = "นำออก";
                wsEnum.Cells[rowIndex + 1, 14].Value = "เลขที่ใบขนสินค้านำออกคลังฯ";
                wsEnum.Cells[rowIndex + 1, 14].Style.WrapText = true;
                wsEnum.Cells[rowIndex + 1, 15].Value = "รายการที่";
                wsEnum.Cells[rowIndex + 1, 16].Value = "ปริมาณนำออก";
                wsEnum.Cells["N7:P7"].Merge = true;
                wsEnum.Cells["N7:P7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                wsEnum.Cells[rowIndex, 17].Value = "คงเหลือ";
                wsEnum.Cells[rowIndex + 1, 17].Value = "ปริมาณคงเหลือ";
                wsEnum.Cells[rowIndex + 1, 18].Value = "มูลค่า (บาท)";
                wsEnum.Cells[rowIndex + 1, 18].Style.WrapText = true;
                wsEnum.Cells[rowIndex + 1, 19].Value = "ภาษีอากรรวม (บาท)";
                wsEnum.Cells[rowIndex + 1, 19].Style.WrapText = true;
                wsEnum.Cells["Q7:S7"].Merge = true;
                wsEnum.Cells["Q7:S7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells["A7:K7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells["H8:S8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsEnum.Cells["H8:S8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                rowIndex = 9;

                IEnumerable<GLDeclarationReport> bfItems = items.BroughtForwardItems;
                IEnumerable<ExportDeclarationReport> expItems = items.ExportDeclarationItems;
                IEnumerable<ImportDeclarationReport> impItems = items.ImportDeclarationItems;

                var bfs = bfItems.GroupBy(x => new { x.ImportDeclarationNo })
                        .Select(y => new GLDeclarationReport()
                        {
                            ImportDeclarationNo = y.Key.ImportDeclarationNo
                        }
                        );
                foreach (var bfh in bfs)
                {
                    bohByDeclaration = 0;
                    totalShippedByDeclaration = 0;
                    totalImportQtyByDeclaration = 0;
                    totalBfQtyByDeclaration = 0;
                    if (rowIndex == 11894)
                    {
                        Console.Write("test");
                    }
                    IEnumerable<GLDeclarationReport> bfImps = bfItems.Where(x => x.ImportDeclarationNo == bfh.ImportDeclarationNo);

                    wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                    wsEnum.Cells[rowIndex, 2].Value = bfImps.ToList()[0].ImporterName.Trim();
                    wsEnum.Cells[rowIndex, 3].Value = bfImps.ToList()[0].ImportDeclarationDate;
                    wsEnum.Cells[rowIndex, 4].Value = bfImps.ToList()[0].ImportDeclarationNo.Trim();

                    foreach (var bf in bfImps)
                    {
                        if (rowIndex == 11894)
                        {
                            Console.Write("test");
                        }
                        currentQty = bf.Quantity;
                        currentDutyUnit = bf.TotalDuty / bf.Quantity;

                        wsEnum.Cells[rowIndex, 5].Value = bf.ImportDeclarationItemNo.Trim();
                        if (bf.SkuDescription == null)
                            wsEnum.Cells[rowIndex, 6].Value = string.Empty;
                        else
                            wsEnum.Cells[rowIndex, 6].Value = bf.SkuDescription.Trim();
                        wsEnum.Cells[rowIndex, 7].Value = bf.UOM;


                        wsEnum.Cells[rowIndex, 8].Value = bf.Quantity;
                        wsEnum.Cells[rowIndex, 9].Value = bf.UnitPrice * bf.Quantity;
                        wsEnum.Cells[rowIndex, 10].Value = bf.TotalDuty;

                        wsEnum.Cells[rowIndex, 11].Value = 0;
                        wsEnum.Cells[rowIndex, 12].Value = 0;
                        wsEnum.Cells[rowIndex, 13].Value = 0;

                        IEnumerable<ExportDeclarationReport> expfItems = expItems.Where(x => x.ImportDeclarationNo == bf.ImportDeclarationNo && x.ImportDeclarationItemNo == bf.ImportDeclarationItemNo);
                        firstRow = 0;
                        if (rowIndex == 11894)
                        {
                            Console.Write("test");
                        }
                        if (expfItems.Count() == 0)
                        {
                            wsEnum.Cells[rowIndex, 16].Value = 0;
                            wsEnum.Cells[rowIndex, 17].Value = bf.Quantity;
                            wsEnum.Cells[rowIndex, 18].Value = bf.UnitPrice * bf.Quantity;
                            wsEnum.Cells[rowIndex, 19].Value = bf.TotalDuty;
                            bohByDeclaration += bf.Quantity;
                            totalShippedByDeclaration = 0;


                        }
                        else
                        {
                            foreach (var exp in expfItems)
                            {
                                if (rowIndex == 11894)
                                {
                                    Console.Write("test");
                                }
                                if (firstRow > 0) rowIndex++;
                                wsEnum.Cells[rowIndex, 14].Value = exp.ExportDeclarationNo.Trim();
                                wsEnum.Cells[rowIndex, 15].Value = string.Format("{0:00000}", exp.ExportDeclarationItemNo);
                                wsEnum.Cells[rowIndex, 16].Value = exp.Quantity;

                                currentQty -= exp.Quantity;

                                wsEnum.Cells[rowIndex, 17].Value = currentQty;
                                wsEnum.Cells[rowIndex, 18].Value = bf.UnitPrice * currentQty;
                                wsEnum.Cells[rowIndex, 19].Value = currentDutyUnit * currentQty;

                                totalShippedByDeclaration += exp.Quantity;
                                firstRow++;
                            }
                            bohByDeclaration += currentQty;
                        }

                        rowIndex++;

                    }

                    currentQty = 0;
                    wsEnum.Cells[rowIndex, 8].Value = bfImps.Sum(x => x.Quantity);
                    wsEnum.Cells[rowIndex, 11].Value = 0;
                    wsEnum.Cells[rowIndex, 16].Value = totalShippedByDeclaration;
                    wsEnum.Cells[rowIndex, 17].Value = bohByDeclaration;

                    //wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Merge = true;
                    //wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Style.Font.Bold = true;
                    wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    // Summary by declaration

                    rowIndex++;
                    rowDecl++;
                }


                var receipts = impItems.GroupBy(x => new { x.ImportDeclarationNo })
                           .Select(y => new GLDeclarationReport()
                           {
                               ImportDeclarationNo = y.Key.ImportDeclarationNo
                           }
                           );

                foreach (var sn in receipts)
                {
                    bohByDeclaration = 0;
                    totalShippedByDeclaration = 0;
                    if (rowIndex == 11894)
                    {
                        Console.Write("test");
                    }
                    IEnumerable<ImportDeclarationReport> imps = impItems.Where(x => x.ImportDeclarationNo == sn.ImportDeclarationNo);
                    wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                    wsEnum.Cells[rowIndex, 2].Value = imps.ToList()[0].ImporterName.Trim();
                    wsEnum.Cells[rowIndex, 3].Value = imps.ToList()[0].ImportDeclarationDate;
                    wsEnum.Cells[rowIndex, 4].Value = imps.ToList()[0].ImportDeclarationNo.Trim();

                    foreach (var imp in imps)
                    {
                        if (rowIndex == 11894)
                        {
                            Console.Write("test");
                        }
                        currentQty = imp.Quantity;
                        currentDutyUnit = imp.TotalDuty / imp.Quantity;

                        wsEnum.Cells[rowIndex, 5].Value = imp.ImportDeclarationItemNo.Trim();
                        wsEnum.Cells[rowIndex, 6].Value = imp.SkuDescription.Trim();
                        wsEnum.Cells[rowIndex, 7].Value = imp.UOM;

                        wsEnum.Cells[rowIndex, 8].Value = 0;
                        wsEnum.Cells[rowIndex, 9].Value = 0;
                        wsEnum.Cells[rowIndex, 10].Value = 0;

                        wsEnum.Cells[rowIndex, 11].Value = imp.Quantity;
                        wsEnum.Cells[rowIndex, 12].Value = imp.UnitPrice * imp.Quantity;
                        wsEnum.Cells[rowIndex, 13].Value = imp.TotalDuty;

                        IEnumerable<ExportDeclarationReport> expfItems = expItems.Where(x => x.ImportDeclarationNo == imp.ImportDeclarationNo && x.ImportDeclarationItemNo == imp.ImportDeclarationItemNo);
                        firstRow = 0;

                        if (expfItems.Count() == 0)
                        {
                            wsEnum.Cells[rowIndex, 16].Value = 0;
                            wsEnum.Cells[rowIndex, 17].Value = imp.Quantity;
                            wsEnum.Cells[rowIndex, 18].Value = imp.UnitPrice * imp.Quantity;
                            wsEnum.Cells[rowIndex, 19].Value = imp.TotalDuty;
                            bohByDeclaration += imp.Quantity;
                            totalShippedByDeclaration = 0;
                        }
                        else
                        {
                            foreach (var exp in expfItems)
                            {
                                if (firstRow > 0) rowIndex++;
                                wsEnum.Cells[rowIndex, 14].Value = exp.ExportDeclarationNo.Trim();
                                wsEnum.Cells[rowIndex, 15].Value = string.Format("{0:00000}", exp.ExportDeclarationItemNo);
                                wsEnum.Cells[rowIndex, 16].Value = exp.Quantity;

                                currentQty -= exp.Quantity;

                                wsEnum.Cells[rowIndex, 17].Value = currentQty;
                                wsEnum.Cells[rowIndex, 18].Value = imp.UnitPrice * currentQty;
                                wsEnum.Cells[rowIndex, 19].Value = currentDutyUnit * currentQty;

                                totalShippedByDeclaration += exp.Quantity;
                                firstRow++;
                            }
                            bohByDeclaration += currentQty;

                        }

                        rowIndex++;

                    }

                    currentQty = 0;
                    wsEnum.Cells[rowIndex, 8].Value = 0;
                    wsEnum.Cells[rowIndex, 11].Value = imps.Sum(x => x.Quantity);
                    wsEnum.Cells[rowIndex, 16].Value = totalShippedByDeclaration;
                    wsEnum.Cells[rowIndex, 17].Value = bohByDeclaration;

                    //wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Merge = true;
                    //wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Style.Font.Bold = true;
                    wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    // Summary by declaration

                    rowIndex++;
                    rowDecl++;
                }


                //0=นำเข้าจากต่างประเทศ
                //1=ส่งออกต่างประเทศ
                //A=โอนย้ายภายในประเทศ
                //B=โอนย้ายภายในประเทศ
                //C=โอนย้ายจากเขตปลอดอากร/เขตประกอบการเสรี
                //D=โอนย้ายเข้าเขตปลอดอากร/เขตประกอบการเสรี
                //P=ชำระภาษี

                var documentsType0 = impItems.Where(x => x.ReportType == "นำเข้าจากต่างประเทศ").Select(l => l.ReceiptKey).Distinct().Count();
                var documentsType0Amount = impItems.Where(x => x.ReportType == "นำเข้าจากต่างประเทศ").Sum(item => (item.Quantity * item.UnitPrice));
                var documentsType0DutyAmount = impItems.Where(x => x.ReportType == "นำเข้าจากต่างประเทศ").Sum(item => (item.Quantity * item.DutyUnit));

                var documentsTypeC = impItems.Where(x => x.ReportType == "โอนย้ายจากเขตปลอดอากร").Select(l => l.ReceiptKey).Distinct().Count();
                var documentsTypeCAmount = impItems.Where(x => x.ReportType == "โอนย้ายจากเขตปลอดอากร").Sum(item => (item.Quantity * item.UnitPrice));
                var documentsTypeCDutyAmount = impItems.Where(x => x.ReportType == "โอนย้ายจากเขตปลอดอากร").Sum(item => (item.Quantity * item.DutyUnit));

                var documentsType1 = expItems.Where(x => x.ReportType == "ส่งออกต่างประเทศ").Select(l => l.OrderKey).Distinct().Count();
                var documentsType1Amount = expItems.Where(x => x.ReportType == "ส่งออกต่างประเทศ").Sum(item => (item.Quantity * item.UnitPrice));
                var documentsType1DutyAmount = expItems.Where(x => x.ReportType == "ส่งออกต่างประเทศ").Sum(item => (item.Quantity * item.ExportDutyUnit));

                var documentsTypeD = expItems.Where(x => x.ReportType == "โอนย้ายเข้าเขตปลอดอากร").Select(l => l.ExportDeclarationNo).Distinct().Count();
                var documentsTypeDAmount = expItems.Where(x => x.ReportType == "โอนย้ายเข้าเขตปลอดอากร").Sum(item => (item.Quantity * item.UnitPrice));
                var documentsTypeDDutyAmount = expItems.Where(x => x.ReportType == "โอนย้ายเข้าเขตปลอดอากร").Sum(item => (item.Quantity * item.ExportDutyUnit));

                var documentsTypeP = expItems.Where(x => x.ReportType == "ชำระภาษี").Select(l => l.ExportDeclarationNo).Distinct().Count();
                var documentsTypePAmount = expItems.Where(x => x.ReportType == "ชำระภาษี").Sum(item => (item.Quantity * item.UnitPrice));
                var documentsTypePDutyAmount = expItems.Where(x => x.ReportType == "ชำระภาษี").Sum(item => (item.Quantity * item.ExportDutyUnit));

                //wsEnum.Cells["C2:C" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
                //wsEnum.Cells["G2:G" + rowIndex].Style.Numberformat.Format = "#,##0";
                //wsEnum.Cells["H2:H" + rowIndex].Style.Numberformat.Format = "#,##0";
                //wsEnum.Cells["I2:J" + rowIndex].Style.Numberformat.Format = "#,##0.00";

                //wsEnum.Cells["K2:K" + rowIndex].Style.Numberformat.Format = "#,##0";
                //wsEnum.Cells["L2:M" + rowIndex].Style.Numberformat.Format = "#,##0.00";

                //wsEnum.Cells["P2:Q" + rowIndex].Style.Numberformat.Format = "#,##0";
                //wsEnum.Cells["R2:S" + rowIndex].Style.Numberformat.Format = "#,##0.00";

                var modelCells = wsEnum.Cells[string.Format("A7:{0}{1}", endColumnName, rowIndex - 1)];
                //var border = modelCells.Style.Border.Top.Style = modelCells.Style.Border.Left.Style = modelCells.Style.Border.Right.Style = modelCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                //wsEnum.Cells[string.Format("A1:{0}{1}", endColumnName, rowIndex - 1)].AutoFitColumns();
                //wsEnum.Cells["E7:E8"].Merge = true;

                rowIndex += 3;
                int lastRow = rowIndex;
                wsEnum.Cells[rowIndex, 2].Value = "สรุปการนำเข้า/ออก";
                //wsEnum.Cells[string.Format("B{0}:{1}{0}", lastRow, "E")].Merge = true;
                //wsEnum.Cells[string.Format("B{0}:{1}{0}", lastRow, "E")].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rowIndex++;
                wsEnum.Cells[rowIndex, 2].Value = "รายการ";
                wsEnum.Cells[rowIndex, 3].Value = "จำนวนใบขน(ฉบับ)";
                //wsEnum.Cells[rowIndex, 3].Style.WrapText = true;
                wsEnum.Cells[rowIndex, 4].Value = "มูลค่า(บาท)";
                wsEnum.Cells[rowIndex, 5].Value = "ภาษีอากรรวม(บาท)";
                wsEnum.Cells[rowIndex, 5].Style.WrapText = true;
                rowIndex++;
                wsEnum.Cells[rowIndex, 2].Value = "ยอดยกมา";
                wsEnum.Cells[rowIndex, 3].Value = 0;
                wsEnum.Cells[rowIndex, 4].Value = 0;
                wsEnum.Cells[rowIndex, 5].Value = 0;
                //wsEnum.Cells[rowIndex, 2].Style.Numberformat.Format = "#,##0";
                //wsEnum.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0.00";
                //wsEnum.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
                rowIndex++;
                wsEnum.Cells[rowIndex, 2].Value = "นำเข้าจากต่างประเทศ";
                wsEnum.Cells[rowIndex, 2].Style.WrapText = true;
                wsEnum.Cells[rowIndex, 3].Value = documentsType0;
                wsEnum.Cells[rowIndex, 4].Value = documentsType0Amount;
                wsEnum.Cells[rowIndex, 5].Value = documentsType0DutyAmount;
                //wsEnum.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0";
                //wsEnum.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
                //wsEnum.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
                rowIndex++;
                wsEnum.Cells[rowIndex, 2].Value = "รับโอน";
                wsEnum.Cells[rowIndex, 3].Value = documentsTypeC;
                wsEnum.Cells[rowIndex, 4].Value = documentsTypeCAmount;
                wsEnum.Cells[rowIndex, 5].Value = documentsTypeCDutyAmount;
                //wsEnum.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0";
                //wsEnum.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
                //wsEnum.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
                rowIndex++;
                wsEnum.Cells[rowIndex, 2].Value = "ส่งออกต่างประเทศ";
                wsEnum.Cells[rowIndex, 2].Style.WrapText = true;
                wsEnum.Cells[rowIndex, 3].Value = documentsType1;
                wsEnum.Cells[rowIndex, 4].Value = documentsType1Amount;
                wsEnum.Cells[rowIndex, 5].Value = documentsType1DutyAmount;
                //wsEnum.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0";
                //wsEnum.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
                //wsEnum.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
                rowIndex++;
                wsEnum.Cells[rowIndex, 2].Value = "โอนย้าย";
                wsEnum.Cells[rowIndex, 3].Value = documentsTypeD;
                wsEnum.Cells[rowIndex, 4].Value = documentsTypeDAmount;
                wsEnum.Cells[rowIndex, 5].Value = documentsTypeDDutyAmount;
                //wsEnum.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0";
                //wsEnum.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
                //wsEnum.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
                rowIndex++;
                wsEnum.Cells[rowIndex, 2].Value = "ชำระภาษี";
                wsEnum.Cells[rowIndex, 3].Value = documentsTypeP;
                wsEnum.Cells[rowIndex, 4].Value = documentsTypePAmount;
                wsEnum.Cells[rowIndex, 5].Value = documentsTypePDutyAmount;
                //wsEnum.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0";
                //wsEnum.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
                //wsEnum.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
                rowIndex++;
                wsEnum.Cells[rowIndex, 2].Value = "อื่นๆ";
                wsEnum.Cells[rowIndex, 3].Value = 0;
                wsEnum.Cells[rowIndex, 4].Value = 0;
                wsEnum.Cells[rowIndex, 5].Value = 0;
                //wsEnum.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0";
                //wsEnum.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
                //wsEnum.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";

                //wsEnum.Cells[string.Format("B{0}:E{0}", lastRow + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //wsEnum.Cells[string.Format("B{0}:E{0}", lastRow + 1)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                //modelCells = wsEnum.Cells[string.Format("B{0}:{1}{2}", lastRow, "E", rowIndex)];
                //border = modelCells.Style.Border.Top.Style = modelCells.Style.Border.Left.Style = modelCells.Style.Border.Right.Style = modelCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                MemoryStream memoryStream = new MemoryStream();

                pck.SaveAs(memoryStream);
                memoryStream.Position = 0;

                return memoryStream;
            }
            catch (Exception ex)
            {
                result = false;
                msg = ex.Message;
                return null;
            }

        }
        private MemoryStream GetStreamMovementDeclarationFreezone(InventoryMovementReport items, string dateTitle, string importerTitle, string sku)
        {
            string serverType = ((User)System.Web.HttpContext.Current.Session["userRoles"]).CurrentRoleConnection.ServerType;
            string companyNameHeader = string.Empty;

            companyNameHeader = "เขตปลอดอากร (Free Zone : FZ)";

            int rowIndex = 1;
            int rowDecl = 1;
            int firstRow = 0;
            decimal currentQty;
            decimal currentDutyUnit;
            string endColumnName = "Q";
            string currentReceipItem = string.Empty;
            string currentReceiptKey = string.Empty;

            ExcelPackage pck = new ExcelPackage();

            var wsEnum = pck.Workbook.Worksheets.Add("MovementTrans18");

            wsEnum.Cells[string.Format("A{0}:{1}{2}", 3, endColumnName, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[3, 1].Value = string.Format("{0} บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396", companyNameHeader);
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 3, endColumnName, 3)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:{1}{2}", 4, endColumnName, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[4, 1].Value = "รายงานการเคลื่อนไหวของของที่นำเข้า";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 4, endColumnName, 4)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:{1}{2}", 5, endColumnName, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[5, 1].Value = dateTitle;
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 5, endColumnName, 5)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            rowIndex = 7;

            wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
            wsEnum.Cells["A7:A8"].Merge = true;
            wsEnum.Cells["A7:A8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["A7:A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["A7:A8"].Style.WrapText = true;

            wsEnum.Cells[rowIndex, 2].Value = "ชื่อผู้นำเข้า";
            wsEnum.Cells["B7:B8"].Merge = true;
            wsEnum.Cells["B7:B8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["B7:B8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["B7:B8"].Style.WrapText = true;

            wsEnum.Cells[rowIndex, 3].Value = "วันที่นำเข้าคลังฯ";
            wsEnum.Cells["C7:C8"].Merge = true;
            wsEnum.Cells["C7:C8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["C7:C8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["C7:C8"].Style.WrapText = true;

            wsEnum.Cells[rowIndex, 4].Value = "เลขที่ใบขนสินค้านำเข้าคลังฯ";
            wsEnum.Cells["D7:D8"].Merge = true;
            wsEnum.Cells["D7:D8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["D7:D8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["D7:D8"].Style.WrapText = true;

            wsEnum.Cells[rowIndex, 5].Value = "รายการที่";
            wsEnum.Cells["E7:E8"].Merge = true;
            wsEnum.Cells["E7:E8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["E7:E8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["E7:E8"].Style.WrapText = true;

            wsEnum.Cells[rowIndex, 6].Value = "รหัสสินค้า";
            wsEnum.Cells["F7:F8"].Merge = true;
            wsEnum.Cells["F7:F8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["F7:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["F7:F8"].Style.WrapText = true;

            wsEnum.Cells[rowIndex, 7].Value = "ชนิดของเป็นภาษาอังกฤษ";
            wsEnum.Cells["G7:G8"].Merge = true;
            wsEnum.Cells["G7:G8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["G7:G8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["G7:G8"].Style.WrapText = true;

            wsEnum.Cells[rowIndex, 8].Value = "หน่วยของปริมาณ";
            wsEnum.Cells["H7:H8"].Merge = true;
            wsEnum.Cells["H7:H8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["H7:H8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["H7:H8"].Style.WrapText = true;

            wsEnum.Cells[rowIndex, 9].Value = "ยกมา";
            wsEnum.Cells[rowIndex + 1, 9].Value = "ปริมาณยกมา";
            wsEnum.Cells[rowIndex + 1, 10].Value = "มูลค่า (บาท)";
            wsEnum.Cells[rowIndex + 1, 10].Style.WrapText = true;
            wsEnum.Cells["I7:J7"].Merge = true;
            wsEnum.Cells["I7:J7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[rowIndex, 11].Value = "นำเข้า";
            wsEnum.Cells[rowIndex + 1, 11].Value = "ปริมาณนำเข้า";
            wsEnum.Cells[rowIndex + 1, 12].Value = "มูลค่า (บาท)";
            wsEnum.Cells[rowIndex + 1, 12].Style.WrapText = true;
            wsEnum.Cells["K7:L7"].Merge = true;
            wsEnum.Cells["K7:L7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            wsEnum.Cells[rowIndex, 13].Value = "นำออก";
            wsEnum.Cells[rowIndex + 1, 13].Value = "เลขที่ใบขนสินค้านำออกคลังฯ";
            wsEnum.Cells[rowIndex + 1, 13].Style.WrapText = true;
            wsEnum.Cells[rowIndex + 1, 14].Value = "รายการที่";
            wsEnum.Cells[rowIndex + 1, 15].Value = "ปริมาณนำออก";
            wsEnum.Cells["M7:O7"].Merge = true;
            wsEnum.Cells["M7:O7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            wsEnum.Cells[rowIndex, 16].Value = "คงเหลือ";
            wsEnum.Cells[rowIndex + 1, 16].Value = "ปริมาณคงเหลือ";
            wsEnum.Cells[rowIndex + 1, 17].Value = "มูลค่า (บาท)";
            wsEnum.Cells[rowIndex + 1, 17].Style.WrapText = true;
            wsEnum.Cells["P7:Q7"].Merge = true;

            wsEnum.Cells["G7:Q7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //wsEnum.Cells["A7:K7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //wsEnum.Cells["H8:S8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //wsEnum.Cells["H8:S8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            rowIndex = 9;

            IEnumerable<GLDeclarationReport> bfItems = items.BroughtForwardItems;
            IEnumerable<ExportDeclarationReport> expItems = items.ExportDeclarationItems;
            IEnumerable<ImportDeclarationReport> impItems = items.ImportDeclarationItems;

            var bfs = bfItems.GroupBy(x => new { x.ImportDeclarationNo })
                    .Select(y => new GLDeclarationReport()
                    {
                        ImportDeclarationNo = y.Key.ImportDeclarationNo
                    }
                    );
            foreach (var bfh in bfs)
            {
                IEnumerable<GLDeclarationReport> bfImps = bfItems.Where(x => x.ImportDeclarationNo == bfh.ImportDeclarationNo);
                wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                wsEnum.Cells[rowIndex, 2].Value = bfImps.ToList()[0].ImporterName.Trim();
                wsEnum.Cells[rowIndex, 3].Value = bfImps.ToList()[0].ImportDeclarationDate;
                wsEnum.Cells[rowIndex, 4].Value = bfImps.ToList()[0].ImportDeclarationNo.Trim();

                foreach (var bf in bfImps)
                {
                    currentQty = bf.Quantity;
                    currentDutyUnit = bf.TotalDuty / bf.Quantity;

                    wsEnum.Cells[rowIndex, 5].Value = bf.ImportDeclarationItemNo.Trim();
                    wsEnum.Cells[rowIndex, 6].Value = bf.Sku.Trim();
                    wsEnum.Cells[rowIndex, 7].Value = bf.SkuDescription.Trim();
                    wsEnum.Cells[rowIndex, 8].Value = bf.UOM;


                    wsEnum.Cells[rowIndex, 9].Value = bf.Quantity;
                    wsEnum.Cells[rowIndex, 10].Value = bf.UnitPrice * bf.Quantity;


                    wsEnum.Cells[rowIndex, 11].Value = 0;
                    wsEnum.Cells[rowIndex, 12].Value = 0;

                    IEnumerable<ExportDeclarationReport> expfItems = expItems.Where(x => x.ImportDeclarationNo == bf.ImportDeclarationNo && x.ImportDeclarationItemNo == bf.ImportDeclarationItemNo);
                    firstRow = 0;

                    if (expfItems.Count() == 0)
                    {
                        wsEnum.Cells[rowIndex, 15].Value = 0;
                        wsEnum.Cells[rowIndex, 16].Value = bf.Quantity;
                        wsEnum.Cells[rowIndex, 17].Value = bf.UnitPrice * bf.Quantity;

                    }
                    else
                    {
                        foreach (var exp in expfItems)
                        {
                            if (firstRow > 0) rowIndex++;
                            if (sku != string.Empty)
                            {
                                wsEnum.Cells[rowIndex, 4].Value = bfImps.ToList()[0].ImportDeclarationNo.Trim();
                                wsEnum.Cells[rowIndex, 5].Value = bf.ImportDeclarationItemNo.Trim();
                                wsEnum.Cells[rowIndex, 6].Value = bf.Sku.Trim();
                            }
                            wsEnum.Cells[rowIndex, 13].Value = exp.ExportDeclarationNo.Trim();
                            wsEnum.Cells[rowIndex, 14].Value = string.Format("{0:00000}", exp.ExportDeclarationItemNo);
                            wsEnum.Cells[rowIndex, 15].Value = exp.Quantity;

                            currentQty -= exp.Quantity;

                            wsEnum.Cells[rowIndex, 16].Value = currentQty;
                            wsEnum.Cells[rowIndex, 17].Value = bf.UnitPrice * currentQty;


                            firstRow++;
                        }
                    }

                    rowIndex++;

                }

                currentQty = 0;

                if (sku == string.Empty)
                {
                    wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Merge = true;
                    wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Style.Font.Bold = true;
                    wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    rowIndex++;
                }
                // Summary by declaration


                rowDecl++;
            }


            var receipts = impItems.GroupBy(x => new { x.ImportDeclarationNo })
                       .Select(y => new GLDeclarationReport()
                       {
                           ImportDeclarationNo = y.Key.ImportDeclarationNo
                       }
                       );

            foreach (var sn in receipts)
            {
                IEnumerable<ImportDeclarationReport> imps = impItems.Where(x => x.ImportDeclarationNo == sn.ImportDeclarationNo);
                wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                wsEnum.Cells[rowIndex, 2].Value = imps.ToList()[0].ImporterName.Trim();
                wsEnum.Cells[rowIndex, 3].Value = imps.ToList()[0].ImportDeclarationDate;
                wsEnum.Cells[rowIndex, 4].Value = imps.ToList()[0].ImportDeclarationNo.Trim();

                foreach (var imp in imps)
                {
                    currentQty = imp.Quantity;
                    currentDutyUnit = imp.TotalDuty / imp.Quantity;

                    wsEnum.Cells[rowIndex, 5].Value = imp.ImportDeclarationItemNo.Trim();
                    wsEnum.Cells[rowIndex, 6].Value = imp.Sku.Trim();
                    wsEnum.Cells[rowIndex, 7].Value = imp.SkuDescription.Trim();
                    wsEnum.Cells[rowIndex, 8].Value = imp.UOM;

                    wsEnum.Cells[rowIndex, 9].Value = 0;
                    wsEnum.Cells[rowIndex, 10].Value = 0;
                    wsEnum.Cells[rowIndex, 11].Value = imp.Quantity;

                    wsEnum.Cells[rowIndex, 12].Value = imp.UnitPrice * imp.Quantity;


                    IEnumerable<ExportDeclarationReport> expfItems = expItems.Where(x => x.ImportDeclarationNo == imp.ImportDeclarationNo && x.ImportDeclarationItemNo == imp.ImportDeclarationItemNo);
                    firstRow = 0;

                    if (expfItems.Count() == 0)
                    {
                        wsEnum.Cells[rowIndex, 15].Value = 0;
                        wsEnum.Cells[rowIndex, 16].Value = imp.Quantity;
                        wsEnum.Cells[rowIndex, 17].Value = imp.UnitPrice * imp.Quantity;

                    }
                    else
                    {
                        foreach (var exp in expfItems)
                        {
                            if (firstRow > 0) rowIndex++;
                            if (sku != string.Empty)
                            {
                                wsEnum.Cells[rowIndex, 4].Value = imps.ToList()[0].ImportDeclarationNo.Trim();
                                wsEnum.Cells[rowIndex, 5].Value = imp.ImportDeclarationItemNo.Trim();
                                wsEnum.Cells[rowIndex, 6].Value = imp.Sku.Trim();
                            }
                            wsEnum.Cells[rowIndex, 13].Value = exp.ExportDeclarationNo.Trim();
                            wsEnum.Cells[rowIndex, 14].Value = string.Format("{0:00000}", exp.ExportDeclarationItemNo);
                            wsEnum.Cells[rowIndex, 15].Value = exp.Quantity;

                            currentQty -= exp.Quantity;

                            wsEnum.Cells[rowIndex, 16].Value = currentQty;
                            wsEnum.Cells[rowIndex, 17].Value = imp.UnitPrice * currentQty;



                            firstRow++;
                        }
                    }

                    rowIndex++;

                }

                currentQty = 0;

                if (sku == string.Empty)
                {
                    wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Merge = true;
                    wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Style.Font.Bold = true;
                    wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex, endColumnName, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    rowIndex++;
                }

                // Summary by declaration


                rowDecl++;
            }


            //0=นำเข้าจากต่างประเทศ
            //1=ส่งออกต่างประเทศ
            //A=โอนย้ายภายในประเทศ
            //B=โอนย้ายภายในประเทศ
            //C=โอนย้ายจากเขตปลอดอากร/เขตประกอบการเสรี
            //D=โอนย้ายเข้าเขตปลอดอากร/เขตประกอบการเสรี
            //P=ชำระภาษี

            var documentsTypeBf = bfItems.Select(l => l.ImportDeclarationNo).Distinct().Count();
            var documentsTypeBfAmount = bfItems.Sum(item => (item.Quantity * item.UnitPrice));
            var documentsTypeBfDutyAmount = bfItems.Sum(item => (item.Quantity * item.TotalDuty));

            var documentsType0 = impItems.Where(x => x.ReportType == "นำเข้าจากต่างประเทศ").Select(l => l.ImportDeclarationNo).Distinct().Count();
            var documentsType0Amount = impItems.Where(x => x.ReportType == "นำเข้าจากต่างประเทศ").Sum(item => (item.Quantity * item.UnitPrice));
            var documentsType0DutyAmount = impItems.Where(x => x.ReportType == "นำเข้าจากต่างประเทศ").Sum(item => (item.Quantity * item.DutyUnit));

            var documentsTypeC = impItems.Where(x => x.ReportType == "โอนย้ายจากเขตปลอดอากร").Select(l => l.ImportDeclarationNo).Distinct().Count();
            var documentsTypeCAmount = impItems.Where(x => x.ReportType == "โอนย้ายจากเขตปลอดอากร").Sum(item => (item.Quantity * item.UnitPrice));
            var documentsTypeCDutyAmount = impItems.Where(x => x.ReportType == "โอนย้ายจากเขตปลอดอากร").Sum(item => (item.Quantity * item.DutyUnit));

            var documentsType1 = expItems.Where(x => x.ReportType == "ส่งออกต่างประเทศ").Select(l => l.ExportDeclarationNo).Distinct().Count();
            var documentsType1Amount = expItems.Where(x => x.ReportType == "ส่งออกต่างประเทศ").Sum(item => (item.Quantity * item.UnitPrice));
            var documentsType1DutyAmount = expItems.Where(x => x.ReportType == "ส่งออกต่างประเทศ").Sum(item => (item.Quantity * item.ExportDutyUnit));

            var documentsTypeD = expItems.Where(x => x.ReportType == "โอนย้ายเข้าเขตปลอดอากร").Select(l => l.ExportDeclarationNo).Distinct().Count();
            var documentsTypeDAmount = expItems.Where(x => x.ReportType == "โอนย้ายเข้าเขตปลอดอากร").Sum(item => (item.Quantity * item.UnitPrice));
            var documentsTypeDDutyAmount = expItems.Where(x => x.ReportType == "โอนย้ายเข้าเขตปลอดอากร").Sum(item => (item.Quantity * item.ExportDutyUnit));

            var documentsTypeP = expItems.Where(x => x.ReportType == "ชำระภาษี").Select(l => l.ExportDeclarationNo).Distinct().Count();
            var documentsTypePAmount = expItems.Where(x => x.ReportType == "ชำระภาษี").Sum(item => (item.Quantity * item.UnitPrice));
            var documentsTypePDutyAmount = expItems.Where(x => x.ReportType == "ชำระภาษี").Sum(item => (item.Quantity * item.ExportDutyUnit));

            wsEnum.Cells["C2:C" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            wsEnum.Cells["I2:I" + rowIndex].Style.Numberformat.Format = "#,##0";

            wsEnum.Cells["J2:J" + rowIndex].Style.Numberformat.Format = "#,##0.00";
            wsEnum.Cells["K2:K" + rowIndex].Style.Numberformat.Format = "#,##0";

            wsEnum.Cells["L2:L" + rowIndex].Style.Numberformat.Format = "#,##0.00";
            wsEnum.Cells["M2:M" + rowIndex].Style.Numberformat.Format = "#,##0";

            wsEnum.Cells["P2:P" + rowIndex].Style.Numberformat.Format = "#,##0";
            wsEnum.Cells["Q2:Q" + rowIndex].Style.Numberformat.Format = "#,##0.00";

            var modelCells = wsEnum.Cells[string.Format("A7:{0}{1}", endColumnName, rowIndex - 1)];
            var border = modelCells.Style.Border.Top.Style = modelCells.Style.Border.Left.Style = modelCells.Style.Border.Right.Style = modelCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            wsEnum.Cells[string.Format("A1:{0}{1}", endColumnName, rowIndex - 1)].AutoFitColumns();
            wsEnum.Cells["E7:E8"].Merge = true;

            //rowIndex += 3;
            //int lastRow = rowIndex;
            //wsEnum.Cells[rowIndex, 2].Value = "สรุปการนำเข้า/ออก";
            //wsEnum.Cells[string.Format("B{0}:{1}{0}", lastRow, "E")].Merge = true;
            //wsEnum.Cells[string.Format("B{0}:{1}{0}", lastRow, "E")].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //rowIndex++;
            //wsEnum.Cells[rowIndex, 2].Value = "รายการ";
            //wsEnum.Cells[rowIndex, 3].Value = "จำนวนใบขน(ฉบับ)";
            //wsEnum.Cells[rowIndex, 3].Style.WrapText = true;
            //wsEnum.Cells[rowIndex, 4].Value = "มูลค่า(บาท)";
            //wsEnum.Cells[rowIndex, 5].Value = "ภาษีอากรรวม(บาท)";
            //wsEnum.Cells[rowIndex, 5].Style.WrapText = true;
            //rowIndex++;
            //wsEnum.Cells[rowIndex, 2].Value = "ยอดยกมา";
            //wsEnum.Cells[rowIndex, 3].Value = documentsTypeBf;
            //wsEnum.Cells[rowIndex, 4].Value = documentsTypeBfAmount;
            //wsEnum.Cells[rowIndex, 5].Value = documentsTypeBfDutyAmount;
            //wsEnum.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0";
            //wsEnum.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
            //wsEnum.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
            //rowIndex++;
            //wsEnum.Cells[rowIndex, 2].Value = "นำเข้าจากต่างประเทศ";
            //wsEnum.Cells[rowIndex, 2].Style.WrapText = true;
            //wsEnum.Cells[rowIndex, 3].Value = documentsType0;
            //wsEnum.Cells[rowIndex, 4].Value = documentsType0Amount;
            //wsEnum.Cells[rowIndex, 5].Value = documentsType0DutyAmount;
            //wsEnum.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0";
            //wsEnum.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
            //wsEnum.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
            //rowIndex++;
            //wsEnum.Cells[rowIndex, 2].Value = "รับโอน";
            //wsEnum.Cells[rowIndex, 3].Value = documentsTypeC;
            //wsEnum.Cells[rowIndex, 4].Value = documentsTypeCAmount;
            //wsEnum.Cells[rowIndex, 5].Value = documentsTypeCDutyAmount;
            //wsEnum.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0";
            //wsEnum.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
            //wsEnum.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
            //rowIndex++;
            //wsEnum.Cells[rowIndex, 2].Value = "ส่งออกต่างประเทศ";
            //wsEnum.Cells[rowIndex, 2].Style.WrapText = true;
            //wsEnum.Cells[rowIndex, 3].Value = documentsType1;
            //wsEnum.Cells[rowIndex, 4].Value = documentsType1Amount;
            //wsEnum.Cells[rowIndex, 5].Value = documentsType1DutyAmount;
            //wsEnum.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0";
            //wsEnum.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
            //wsEnum.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
            //rowIndex++;
            //wsEnum.Cells[rowIndex, 2].Value = "โอนย้าย";
            //wsEnum.Cells[rowIndex, 3].Value = documentsTypeD;
            //wsEnum.Cells[rowIndex, 4].Value = documentsTypeDAmount;
            //wsEnum.Cells[rowIndex, 5].Value = documentsTypeDDutyAmount;
            //wsEnum.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0";
            //wsEnum.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
            //wsEnum.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
            //rowIndex++;
            //wsEnum.Cells[rowIndex, 2].Value = "ชำระภาษี";
            //wsEnum.Cells[rowIndex, 3].Value = documentsTypeP;
            //wsEnum.Cells[rowIndex, 4].Value = documentsTypePAmount;
            //wsEnum.Cells[rowIndex, 5].Value = documentsTypePDutyAmount;
            //wsEnum.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0";
            //wsEnum.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
            //wsEnum.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
            //rowIndex++;
            //wsEnum.Cells[rowIndex, 2].Value = "อื่นๆ";
            //wsEnum.Cells[rowIndex, 3].Value = 0;
            //wsEnum.Cells[rowIndex, 4].Value = 0;
            //wsEnum.Cells[rowIndex, 5].Value = 0;
            //wsEnum.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0";
            //wsEnum.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
            //wsEnum.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";

            //wsEnum.Cells[string.Format("B{0}:E{0}", lastRow + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //wsEnum.Cells[string.Format("B{0}:E{0}", lastRow + 1)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //modelCells = wsEnum.Cells[string.Format("B{0}:{1}{2}", lastRow, "E", rowIndex)];
            //border = modelCells.Style.Border.Top.Style = modelCells.Style.Border.Left.Style = modelCells.Style.Border.Right.Style = modelCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


            MemoryStream memoryStream = new MemoryStream();

            pck.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

    }
}