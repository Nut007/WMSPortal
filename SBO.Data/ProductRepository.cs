using MicroOrm.Pocos.SqlGenerator;
using WMSPortal.Core.Model;
using WMSPortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;

namespace WMSPortal.Data
{
    public class ProductRepository : DataRepository<Product>, IProductRepository
    {
        public ProductRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<Product> sqlGenerator)
            : base( cache,connection, sqlGenerator)
        {

        }

        public IEnumerable<Product> GetSkuByCode(string sku, string storerKey)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {

                string sql = string.Empty;
                sql = "SELECT " +
                      "Sku, " +
                      "DESCR " +
                      "FROM " +
                      "Sku s " +
                      "WHERE s.Sku LIKE @Sku AND s.StorerKey =@StorerKey";
                if (sku != string.Empty)
                    return cn.Query<Product>(sql, new { Sku = sku + "%", StorerKey = storerKey });
                else
                    return null;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

      
    }
}
