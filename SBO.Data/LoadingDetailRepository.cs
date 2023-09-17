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
    public class LoadingDetailRepository : DataRepository<LoadingDetail>, ILoadingDetailRepository
    {
        IHelperRepository _helper;
        public LoadingDetailRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<LoadingDetail> sqlGenerator, IHelperRepository helper)
            : base(cache,connection, sqlGenerator)
        {
            _helper = helper;
        }

        public IEnumerable<LoadingDetail> GetLoadingDetailList(string column, string value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LoadingDetail> GetLoadingDetail(string loadingNo)
        {
            try
            {
                this.Connection.ConnectionString = this.WMSConnectionString();
                if (!string.IsNullOrEmpty(loadingNo))
                    return this.GetWhere(new { LoadingNo = loadingNo });
                else
                    return Enumerable.Empty<LoadingDetail>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (this.Connection.State == ConnectionState.Open)
                    this.Connection.Close();
            }
        }

        public IEnumerable<LoadingDetail> GetLoadingAll()
        {
            throw new NotImplementedException();
        }

        public bool CreateLoadingDetail(LoadingDetail loadingDetail)
        {
            try
            {
                this.Connection.ConnectionString = this.WMSConnectionString();
                loadingDetail.ITEMNO = GetNextLoadingItemNumber(loadingDetail.LOADINGNO);
                return this.Insert(loadingDetail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateLoadingDetail(LoadingDetail item)
        {
            try
            {
                this.Connection.ConnectionString = this.WMSConnectionString();
                return this.Update(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteLoadingDetail(LoadingDetail item)
        {
            try
            {
                this.Connection.ConnectionString = this.WMSConnectionString();
                return this.Delete(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetNextLoadingItemNumber(string loadingNo)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = "SELECT " +
                      "[ITEMNO]=COALESCE(MAX(ITEMNO),0) +1 " +
                      "FROM " +
                      "LOADING_DETAIL " +
                      "WHERE LOADINGNO = @LoadingNo";


                int line = cn.Query<int>(sql, new { LoadingNo = loadingNo.Trim() }).SingleOrDefault();
                return string.Format("{0:00000}", line);

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
