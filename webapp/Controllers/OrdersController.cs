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
using WMSPortal.Models;
using System.Data.SqlClient;
using WMSPortal.Extensions;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Newtonsoft.Json;

namespace WMSPortal.Controllers
{
    [SessionExpire]
    public class OrdersController : Controller
    {

        private readonly IOrdersRepository _ordersRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IOrdersViewModelListBuilder _ordersViewModelListBuilder;
        private readonly IStorerRepository _storerRepository;
        private readonly IProductRepository _skuRepository;
        public OrdersController(IOrdersRepository ordersRepository, IOrderDetailRepository orderDetailRepository, IOrdersViewModelListBuilder ordersViewModelListBuilder, IStorerRepository storerRepository, IProductRepository skuRepository)
        {
            _ordersViewModelListBuilder = ordersViewModelListBuilder;
            _ordersRepository = ordersRepository;
            _storerRepository = storerRepository;
            _orderDetailRepository = orderDetailRepository;
            _skuRepository = skuRepository;
        }
        public JsonResult GetOutboundShipment(string column, string value1, string value2, string sectionView, [DataSourceRequest] DataSourceRequest dataSourceRequest, string userId)
        {

            try
            {
                IEnumerable<OrderDetail> orders = _ordersRepository.GetOutboundShipment(column, value1, value2, sectionView, userId);
                var results = orders.ToDataSourceResult(dataSourceRequest);
                var jsonResult = Json(results, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (SqlException sqlEx)
            {
                ModelState.AddModelError("ex", sqlEx.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPickingDashboard([DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            try
            {
                IEnumerable<OrderDetail> orders = _ordersRepository.PickingDashboard();
                var results = orders.ToDataSourceResult(dataSourceRequest);
                var jsonResult = Json(results, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (SqlException sqlEx)
            {
                ModelState.AddModelError("ex", sqlEx.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult OrdersInfo(string orderKey, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            Orders item = _ordersRepository.GetOrders(orderKey);
            OrdersViewModel model = Mapper.Map<Orders, OrdersViewModel>(item);
            return View(model);
        }
        public JsonResult GetPickDetail(string orderKey, string orderLineNumber, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            IEnumerable<PickDetail> items = _ordersRepository.GetPickDetail(orderKey, orderLineNumber);
            var results = items.ToDataSourceResult(dataSourceRequest);
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetConsigneeInfo(string companyName)
        {
            IEnumerable<Storer> consigneeInfo = _storerRepository.GetStorerByName(companyName);
            return this.Json(consigneeInfo, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStorerInfo(string companyName, string userId)
        {
            IEnumerable<Storer> consigneeInfo = _storerRepository.GetStorerByNameByUserId(companyName, userId);
            return this.Json(consigneeInfo, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSkuInfo(string sku, string storerKey)
        {
            IEnumerable<Product> skuInfo = _skuRepository.GetSkuByCode(sku, storerKey);
            return this.Json(skuInfo, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOrdersDashboard([DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            try
            {
                OrdersDashboard orderDashboard = _ordersRepository.GetOrdersDashboard();
                //OrdersDashboardViewModel model = Mapper.Map<OrdersDashboard, OrdersDashboardViewModel>(orderDashboard);
                var jsonResult = Json(orderDashboard, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (SqlException sqlEx)
            {
                ModelState.AddModelError("ex", sqlEx.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
        }
        //
        // GET: /Orders/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult OrdersList()
        {
            return View();
        }
        public ActionResult PickingDashboard()
        {
            return View();
        }
        public ActionResult PODOrders()
        {
            return View();
        }
        public ActionResult GmapTesting()
        {
            return View();
        }
        public ActionResult ShipmentOrders(string orderKey)
        {
            var orderViewModel = new OrdersViewModel();
            if (string.IsNullOrEmpty(orderKey))
            {
                orderViewModel.IsNew = true;
            }
            else
            {
                Orders item = _ordersRepository.GetOrders(orderKey);
                orderViewModel = Mapper.Map<Orders, OrdersViewModel>(item);
                orderViewModel.IsNew = false;
            }

            SetViewModelFields(orderViewModel);
            return View(orderViewModel);

        }
        private void SetViewModelFields(OrdersViewModel viewModel)
        {
            _ordersViewModelListBuilder.BuildListsOrdersViewModel(viewModel);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SaveShipmentOrder([DataSourceRequest] DataSourceRequest dataSourceRequest, OrdersViewModel orderViewModel)
        {
            try
            {
                Orders model = Mapper.Map<OrdersViewModel, Orders>(orderViewModel);
                _ordersRepository.SaveShipmentOrder(model);
                var resultData = new[] { Mapper.Map<Orders, OrdersViewModel>(model) };
               
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
        public JsonResult DeleteShipmentOrder([DataSourceRequest] DataSourceRequest dataSourceRequest, string orderKey)
        {
            try
            {
                var model = new OrdersViewModel();
                _ordersRepository.DeleteShipmentOrder(orderKey.Trim());
                var resultData = new[] { Mapper.Map<OrdersViewModel, Orders>(model) };
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
        [OutputCache(NoStore = true, Duration = 0)]
        public JsonResult GetOrderDetail(string orderKey, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            try
            {
                IEnumerable<OrderDetail> items = _orderDetailRepository.GetOrderDetail(orderKey);
                var results = items.ToDataSourceResult(dataSourceRequest);
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            catch (SqlException sqlEx)
            {
                ModelState.AddModelError("ex", sqlEx.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult AddOrderItem([DataSourceRequest] DataSourceRequest request, OrderDetail ordersItem)
        {
            try
            {
                if (ordersItem != null)
                {
                    ModelState.Clear();
                    //ordersItem.OrderKey = orderKey;
                    //ordersItem.OrderLineNumber = "00002";
                    ordersItem.ExternOrderKey = string.Empty;
                    //ordersItem.StorerKey = "HGST";
                    //ordersItem.Sku = "0F27454";
                    ordersItem.UOM = "EA";
                    ordersItem.PackKey = "STD";
                    //ordersItem.Notes1 = "TEST SKU SKU00001";
                    //ordersItem.OpenQty = 1000;
                    //ordersItem.OrderDate = DateTime.Now;
                    ordersItem.AddWho = "sa";
                    ordersItem.AddDate = DateTime.Now;
                    ordersItem.EditWho = "sa";
                    ordersItem.EditDate = DateTime.Now;
                    ordersItem.EffectiveDate = DateTime.Now;
                    _orderDetailRepository.InsertOrderItem(ordersItem);
                    return Json(new[] { ordersItem }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    //string errorMessage = HtmlExtensions.ValidationMessage("", ModelState);
                    return Json(new[] { ordersItem }.ToDataSourceResult(request, ModelState));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult UpdateOrderItem([DataSourceRequest] DataSourceRequest request, OrderDetail ordersItem)
        {
            try
            {
                if (ordersItem != null && ModelState.IsValid)
                {

                    ordersItem.AddWho = "sa";
                    ordersItem.AddDate = DateTime.Now;
                    ordersItem.EditWho = "sa";
                    ordersItem.ExternOrderKey = string.Empty;
                    ordersItem.EditDate = DateTime.Now;
                    ordersItem.EffectiveDate = DateTime.Now;
                    _orderDetailRepository.UpdateOrderItem(ordersItem);
                    return Json(new[] { ordersItem }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DeleteOrderItem([DataSourceRequest] DataSourceRequest request, OrderDetail ordersItem)
        {
            try
            {
                _orderDetailRepository.DeleteOrderItem(ordersItem);
                return Json(new[] { ordersItem }.ToDataSourceResult(request, ModelState));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex_delete", ex.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AllocateOrderItems([DataSourceRequest] DataSourceRequest request, OrderDetail ordersItem, string userId)
        {
            try
            {
                if (ordersItem != null && ModelState.IsValid)
                {
                    _orderDetailRepository.AllocateOrderItems(ordersItem.OrderKey, string.Empty);
                    return Json(new[] { ordersItem }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
                }
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
        public ActionResult BatchAllocateItems([DataSourceRequest] DataSourceRequest request, string orderKey, [Bind(Prefix = "models")]IEnumerable<StockBalanceViewModel> orderItems, string userId)
        {
            try
            {
                if (orderItems != null && ModelState.IsValid)
                {
                    IEnumerable<LotxLocxId> models = Mapper.Map<IEnumerable<StockBalanceViewModel>, IEnumerable<LotxLocxId>>(orderItems);
                    _orderDetailRepository.BatchAllocateItems(orderKey, models, userId);
                    return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (SqlException sqlEx)
            {
                ModelState.AddModelError("ex", sqlEx.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PostOrder([DataSourceRequest] DataSourceRequest request, string orderKey)
        {
            try
            {
                var model = new OrdersViewModel();
                _ordersRepository.PostOrder(orderKey);
                var resultData = new[] { Mapper.Map<OrdersViewModel, Orders>(model) };
                return Json(resultData.AsQueryable().ToDataSourceResult(request, ModelState));
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
        public ActionResult ExportOrdersTransectionReport(OrdersTransectionReportParameters ordersReportParameters)
        {
            string msg = string.Empty;
            bool result = true;
            string handle = string.Empty;
            int recordCount = 0;
            IEnumerable<PickDetail> items = null;
            try
            {
                handle = Guid.NewGuid().ToString();

                items = _ordersRepository.GetOrdersTransection(ordersReportParameters.orderKey);

                if (items.Count() == 0)
                {
                    result = false;
                    msg = "There is no record found.";
                }
                else
                {
                    MemoryStream stream;
                    stream = GetStreamOrdersTransection(items);

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
                Data = new { Result = result, FileGuid = handle, FileName = "OrdersTransReport.xlsx", ErrorMessage = msg, RecordCount = recordCount }
            };
        }
        private MemoryStream GetStreamOrdersTransection(IEnumerable<PickDetail> items)
        {
            int rowIndex = 1;
            ExcelPackage pck = new ExcelPackage();
            string companyNameHeader = string.Empty;
            companyNameHeader = "Orders Transection Report";


            string orderKey = items.ToList()[0].OrderKey;
            var wsEnum = pck.Workbook.Worksheets.Add(string.Format("OrdersTrans-{0}", orderKey));

            wsEnum.Cells[string.Format("A{0}:H{1}", 1, 1)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[rowIndex, 1].Value = string.Format("Orders Transection for Order No-{0}", orderKey);
            wsEnum.Cells[string.Format("A{0}:H{1}", 1, 1)].Merge = true;

            rowIndex++;
            wsEnum.Cells[rowIndex, 1].Value = "Order No";
            wsEnum.Cells[rowIndex, 2].Value = "Order Date";
            wsEnum.Cells[rowIndex, 3].Value = "Sale Order No";
            wsEnum.Cells[rowIndex, 4].Value = "Delivery Date";
            wsEnum.Cells[rowIndex, 5].Value = "Customer Name";
            wsEnum.Cells[rowIndex, 6].Value = "Part No";
            wsEnum.Cells[rowIndex, 7].Value = "Qty";
            wsEnum.Cells[rowIndex, 8].Value = "Serial No";


            wsEnum.Cells[string.Format("A{0}:H{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
            wsEnum.Cells[string.Format("A{0}:H{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsEnum.Cells[string.Format("A{0}:H{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

            rowIndex++;

            foreach (var item in items)
            {
                wsEnum.Cells[rowIndex, 1].Value = item.OrderKey;
                wsEnum.Cells[rowIndex, 2].Value = item.OrderDate;
                wsEnum.Cells[rowIndex, 3].Value = item.ExternOrderKey;
                wsEnum.Cells[rowIndex, 4].Value = item.DeliveryDate;
                wsEnum.Cells[rowIndex, 5].Value = item.CustomerName;
                wsEnum.Cells[rowIndex, 6].Value = item.Sku;
                wsEnum.Cells[rowIndex, 7].Value = item.Qty;
                wsEnum.Cells[rowIndex, 8].Value = item.SerialNo;
                rowIndex++;
            }
            // Footer //
            wsEnum.Cells[rowIndex, 1].Value = "Total Qty";
            wsEnum.Cells[string.Format("A{0}:F{1}", rowIndex, rowIndex)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            wsEnum.Cells[rowIndex, 7].Value = items.Sum(pk => pk.Qty);

            wsEnum.Cells["A1:H" + rowIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            wsEnum.Cells["A1:H" + rowIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            // End Footer //

            // Formatting //
            wsEnum.Cells["B2:B" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            wsEnum.Cells["D2:D" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            wsEnum.Cells["G2:G" + rowIndex].Style.Numberformat.Format = "#,##0";

            wsEnum.Cells[wsEnum.Dimension.Address].AutoFitColumns();


            MemoryStream memoryStream = new MemoryStream();

            pck.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        [HttpGet]
        public JsonResult GetPivotOrders([DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            try
            {
                IEnumerable<OrderDetail> orders = _ordersRepository.GetPivotOrders(DateTime.Now.AddDays(-7).ToString("yyyyMMdd"), DateTime.Now.ToString("yyyyMMdd"));
                IEnumerable<PivotPendingOrdersViewModel> models = Mapper.Map<IEnumerable<OrderDetail>, IEnumerable<PivotPendingOrdersViewModel>>(orders);
                //var results = models.ToDataSourceResult(dataSourceRequest);
                var result = new JsonNetResult
                {
                    Data = models,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Settings = { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
                };
                return result;
            }
            catch (SqlException sqlEx)
            {
                ModelState.AddModelError("ex", sqlEx.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
        }

    }
}