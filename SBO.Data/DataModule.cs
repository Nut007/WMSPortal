using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Autofac;
using WMSPortal.Data.Repositories;
using System.Data;
using System.Data.SqlClient;
using MicroOrm.Pocos.SqlGenerator;
using WMSPortal.Core.Model;

namespace WMSPortal.Data
{
    public class DataModule : Module
    {
        private string _ConnectionString;
        private readonly int _iteration = 3;
        public DataModule(string connectionString)
        {
            this._ConnectionString = connectionString;
        }
      
        protected override void Load(ContainerBuilder builder)
        {
            
            var connectionString = ConfigurationManager.ConnectionStrings[this._ConnectionString].ConnectionString;
            
            System.Data.SqlClient.SqlConnectionStringBuilder cnnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
            
            cnnBuilder.Password= EncryptionHelper.Decrypt(cnnBuilder.Password);
            connectionString= cnnBuilder.ConnectionString;

            //Database connection
            builder.Register(c => new SqlConnection(connectionString)).As<IDbConnection>().InstancePerLifetimeScope();
            //Sql generators
            builder.RegisterType<SqlGenerator<Role>>().As<ISqlGenerator<Role>>().SingleInstance();
            builder.RegisterType<SqlGenerator<ApplicationRole>>().As<ISqlGenerator<ApplicationRole>>().SingleInstance();
            builder.RegisterType<SqlGenerator<User>>().As<ISqlGenerator<User>>().SingleInstance();
            builder.RegisterType<SqlGenerator<UserRole>>().As<ISqlGenerator<UserRole>>().SingleInstance();
            builder.RegisterType<SqlGenerator<Receipt>>().As<ISqlGenerator<Receipt>>().SingleInstance();
            builder.RegisterType<SqlGenerator<Orders>>().As<ISqlGenerator<Orders>>().SingleInstance();
            builder.RegisterType<SqlGenerator<OrderDetail>>().As<ISqlGenerator<OrderDetail>>().SingleInstance();
            builder.RegisterType<SqlGenerator<LotxLocxId>>().As<ISqlGenerator<LotxLocxId>>().SingleInstance();
            builder.RegisterType<SqlGenerator<Storer>>().As<ISqlGenerator<Storer>>().SingleInstance();
            builder.RegisterType<SqlGenerator<ImportDeclarationReport>>().As<ISqlGenerator<ImportDeclarationReport>>().SingleInstance();
            builder.RegisterType<SqlGenerator<Codelkup>>().As<ISqlGenerator<Codelkup>>().SingleInstance();
            builder.RegisterType<SqlGenerator<Product>>().As<ISqlGenerator<Product>>().SingleInstance();
            builder.RegisterType<SqlGenerator<Loading>>().As<ISqlGenerator<Loading>>().SingleInstance();
            builder.RegisterType<SqlGenerator<LoadingDetail>>().As<ISqlGenerator<LoadingDetail>>().SingleInstance();
            builder.RegisterType<SqlGenerator<TEMP_ID>>().As<ISqlGenerator<TEMP_ID>>().SingleInstance();
            builder.RegisterType<SqlGenerator<QatarECommerce>>().As<ISqlGenerator<QatarECommerce>>().SingleInstance();
            builder.RegisterType<SqlGenerator<UserLog>>().As<ISqlGenerator<UserLog>>().SingleInstance();

            // register types
            builder.RegisterType<RoleRepository>().As<IRoleRepository>();
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<ApplicationRoleRepository>().As<IApplicationRoleRepository>();
            builder.RegisterType<UserRoleRepository>().As<IUserRoleRepository>();
            builder.RegisterType<ReceiptRepository>().As<IReceiptRepository>();
            builder.RegisterType<OrdersRepository>().As<IOrdersRepository>();
            builder.RegisterType<OrderDetailRepository>().As<IOrderDetailRepository>();
            builder.RegisterType<StockBalanceRepository>().As<IStockBalanceRepository>();
            builder.RegisterType<DefaultCacheProvider>().As<ICacheProvider>();

            builder.RegisterType<ReportRepository>().As<IReportRepository>();
            builder.RegisterType<HelperRepository>().As<IHelperRepository>();
            builder.RegisterType<CodelkupRepository>().As<ICodelkupRepository>();
            builder.RegisterType<StorerRepository>().As<IStorerRepository>();
            builder.RegisterType<ProductRepository>().As<IProductRepository>();

            builder.RegisterType<LoadingRepository>().As<ILoadingRepository>();
            builder.RegisterType<LoadingDetailRepository>().As<ILoadingDetailRepository>();
            builder.RegisterType<QatarECommerceRepository>().As<IQatarECommerceRepository>();

            builder.RegisterType<UserLogRepository>().As<IUserLogRepository>();
            base.Load(builder);
        }
    }
}
