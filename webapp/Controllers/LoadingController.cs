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

namespace WMSPortal.Controllers
{
    [SessionExpire]
    public class LoadingController : Controller
    {


        private readonly ILoadingViewModelListBuilder _loadingViewModelListBuilder;
        private readonly ILoadingRepository _loadingRepository;
        private readonly ILoadingDetailRepository _loadingDetailRepository;
        public LoadingController(IOrdersRepository ordersRepository, 
            ILoadingViewModelListBuilder loadingViewModelListBuilder, 
            ILoadingRepository loadingRepository,
            ILoadingDetailRepository loadingDetailRepository)
        {
            _loadingViewModelListBuilder = loadingViewModelListBuilder;
            _loadingRepository = loadingRepository;
            _loadingDetailRepository = loadingDetailRepository;
        }

        public ActionResult LoadingList()
        {
            return View();
        }
        public ActionResult LoadingManagment(string loadingNo)
        {
            var viewModel = new LoadingViewModel();
            if (string.IsNullOrEmpty(loadingNo))
            {
                viewModel.IsNew = true;
            }
            else
            {
                Loading item = _loadingRepository.GetLoading(loadingNo);
                viewModel = Mapper.Map<Loading, LoadingViewModel>(item);
                viewModel.IsNew = false;
            }
            SetViewModelFields(viewModel);
            return View(viewModel);
        }
        private void SetViewModelFields(LoadingViewModel viewModel)
        {
            _loadingViewModelListBuilder.BuildListsLoadingViewModel(viewModel);
        }
        public ActionResult Signature()
        {
            return View();
        }
        public void saveData(string signature)
        {
            byte[] bytIn = null;
            signature = signature.Replace("data:image/png;base64,","");
            bytIn = Convert.FromBase64String(signature);
        }
        public JsonResult SaveLoading([DataSourceRequest] DataSourceRequest dataSourceRequest, LoadingViewModel loadingViewModel)
        {
            try
            {
                Loading model = Mapper.Map<LoadingViewModel, Loading>(loadingViewModel);
                _loadingRepository.SaveLoading(model);
                var resultData = new[] { Mapper.Map<Loading, LoadingViewModel>(model) };
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

        public JsonResult DeleteLoading([DataSourceRequest] DataSourceRequest dataSourceRequest, string loadingNo)
        {
            try
            {
                var model = new OrdersViewModel();
                _loadingRepository.DeleteLoading(loadingNo.Trim());
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
        public JsonResult GetLoadingDetail(string loadingNo, [DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            try
            {

                IEnumerable<LoadingDetail> items = _loadingDetailRepository.GetLoadingDetail(loadingNo);
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
        public ActionResult CreateLoadingDetail([DataSourceRequest] DataSourceRequest request, LoadingDetail loadingItem)
        {
            try
            {
                if (loadingItem != null)
                {
                    ModelState.Clear();
                    _loadingDetailRepository.CreateLoadingDetail(loadingItem);
                    return Json(new[] { loadingItem }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    return Json(new[] { loadingItem }.ToDataSourceResult(request, ModelState));
                }
            }
            catch (SqlException sqlEx)
            {
                ModelState.AddModelError("ex", sqlEx.Message);
                return Json(new
                {
                    isValid = false,
                    exceptionErrorMessage = sqlEx.Message,
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult UpdateLoadingDetail([DataSourceRequest] DataSourceRequest request, LoadingDetail loadingItem)
        {
            try
            {
                if (loadingItem != null && ModelState.IsValid)
                {
                    _loadingDetailRepository.UpdateLoadingDetail(loadingItem);
                    return Json(new[] { loadingItem }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (SqlException sqlEx)
            {
                ModelState.AddModelError("ex", sqlEx.Message);
                return Json(new
                {
                    isValid = false,
                    exceptionErrorMessage = sqlEx.Message,
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex", ex.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DeleteLoadingDetail([DataSourceRequest] DataSourceRequest request, LoadingDetail loadingItem)
        {
            try
            {
                _loadingDetailRepository.DeleteLoadingDetail(loadingItem);
                return Json(new[] { loadingItem }.ToDataSourceResult(request, ModelState));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ex_delete", ex.Message);
                return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetLoadingList(string column, string value1, string value2, string sectionView, [DataSourceRequest] DataSourceRequest dataSourceRequest, string userId)
        {
            try
            {
                IEnumerable<Loading> loading = _loadingRepository.GetLoadingList(column, value1, value2, sectionView, userId);
                var results = loading.ToDataSourceResult(dataSourceRequest);
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
        public ActionResult ProofOfDelivery()
        {
            return View();
        }
        public JsonResult GetAllLoading([DataSourceRequest] DataSourceRequest dataSourceRequest)
        {
            try
            {
                IEnumerable<TestLoading> loading = _loadingRepository.GetTestLoadingAll();
                var results = loading.ToDataSourceResult(dataSourceRequest);
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

    }
}