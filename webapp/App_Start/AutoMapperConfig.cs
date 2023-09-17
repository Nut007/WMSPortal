using System;
using System.Linq;
using AutoMapper;
using WMSPortal.Core.Model;
using WMSPortal.ViewModels;
using System.Collections.Generic;

namespace WMSPortal
{
    public class AutoMapperConfig
    {
        public static void SetupMappings()
        {

            Mapper.CreateMap<Role, RoleListViewModel>();
            Mapper.CreateMap<RoleListViewModel, Role>();
            Mapper.CreateMap<UserListViewModel, User>();
            Mapper.CreateMap<User, UserListViewModel>();

            Mapper.CreateMap<ApplicationRoleViewModel, ApplicationRole>();
            Mapper.CreateMap<ApplicationRole, ApplicationRoleViewModel>();

            Mapper.CreateMap<UserRoleViewModel, UserRole>();
            Mapper.CreateMap<UserRole, UserRoleViewModel>();

            Mapper.CreateMap<Receipt, ReceiptViewModel>();
            Mapper.CreateMap<ReceiptViewModel, Receipt>();

            Mapper.CreateMap<Orders, OrdersViewModel>();
            Mapper.CreateMap<OrdersViewModel, Orders>();

            Mapper.CreateMap<LotxLocxId, StockBalanceViewModel>();
            Mapper.CreateMap<StockBalanceViewModel, LotxLocxId>();

            Mapper.CreateMap<GLDeclarationReport, InboundTransReportViewModels>();
            Mapper.CreateMap<InboundTransReportViewModels, GLDeclarationReport>();

            Mapper.CreateMap<Loading, LoadingViewModel>();
            Mapper.CreateMap<LoadingViewModel, Loading>();

            Mapper.CreateMap<TEMP_ID, BaggageListViewModel>();
            Mapper.CreateMap<BaggageListViewModel, TEMP_ID>();

            Mapper.CreateMap<TEMP_ID, MailReportViewModels>();
            Mapper.CreateMap<MailReportViewModels, TEMP_ID>();


            Mapper.CreateMap<CodeLookupViewModel, Codelkup>();
            Mapper.CreateMap<Codelkup, CodeLookupViewModel>();

            Mapper.CreateMap<QatarECommerce, ConsignmentListViewModel>();
            Mapper.CreateMap<ConsignmentListViewModel, QatarECommerce>();

            Mapper.CreateMap<OrdersDashboard, OrdersDashboardViewModel>();
            Mapper.CreateMap<OrdersDashboardViewModel, OrdersDashboard>();

            Mapper.CreateMap<PivotPendingOrdersViewModel, OrderDetail>();
            Mapper.CreateMap<OrderDetail, PivotPendingOrdersViewModel>();

        }
    }
}