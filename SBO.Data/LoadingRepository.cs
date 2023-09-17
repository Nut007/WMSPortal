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
    public class LoadingRepository : DataRepository<Loading>, ILoadingRepository
    {
        IHelperRepository _helper;
        public LoadingRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<Loading> sqlGenerator, IHelperRepository helper)
            : base(cache,connection, sqlGenerator)
        {
            _helper = helper;
        }


        public IEnumerable<Loading> GetLoadingList(string column, string value1, string value2, string sectionView, string userId)
        {
            string sql = string.Empty;
            string groupBy = string.Empty;
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                IEnumerable<Loading> results;
                if (sectionView == Convert.ToInt16(SectionView.Header).ToString())
                {
                    sql = string.Empty;
                    sql = "SELECT " +
                           "l.LOADINGNO, " +
                           "l.LOADING_DATE, " +
                           "l.PLATE_NO," +
                           "l.STATUS " +
                           "FROM " +
                           "LOADING_HEADER l  " +
                           "WHERE ";
                    if (column == "l.LOADING_DATE")
                    {
                        sql += "{0} Between @StartDate AND @StopDate ";
                        sql += groupBy + " ORDER BY l.LOADINGNO ";

                        results = cn.Query<Loading>(string.Format(sql, column), new { StartDate = value1, StopDate = value2, UserId = userId });
                    }
                    else
                    {
                        sql += "{0} LIKE '%' + @SearchValue +'%' ";
                        sql += groupBy + " ORDER BY l.LOADINGNO ";
                        results = cn.Query<Loading>(string.Format(sql, column), new { SearchValue = value1, UserId = userId });
                    }
                }
                else
                {
                    sql = "SELECT " +
                           "l.LOADINGNO, " +
                           "l.LOADING_DATE, " +
                           "l.PLATE_NO," +
                           "l.STATUS," +
                           "ld.PACKINGNO " +
                           "FROM " +
                           "LOADING_HEADER l LEFT JOIN LOADING_DETAIL ld ON l.LOADINGNO=ld.LOADINGNO " +
                           "WHERE ";
                    if (column == "l.LOADING_DATE")
                    {
                        sql += "{0} Between @StartDate AND @StopDate ";
                        sql += groupBy + " ORDER BY l.LOADINGNO  ";

                        results = cn.Query<Loading>(string.Format(sql, column), new { StartDate = value1, StopDate = value2, UserId = userId });
                    }
                    else
                    {
                        sql += "{0} LIKE '%' + @SearchValue +'%' ";
                        sql += groupBy + " ORDER BY l.LOADINGNO ";
                        results = cn.Query<Loading>(string.Format(sql, column), new { SearchValue = value1, UserId = userId });
                    }

                }
                return results;
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

        public Loading GetLoading(string loadingNo)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                Loading item = new Loading();
                
                string sql = string.Empty;
                sql = "SELECT " +
                      "* " +
                      "FROM " +
                      "LOADING_HEADER l " +
                      "WHERE l.LOADINGNO = @LoadingNo";
                item = cn.Query<Loading>(sql, new { LoadingNo = loadingNo }).SingleOrDefault();
                return item;

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

        public IEnumerable<Loading> GetLoadingAll()
        {
            throw new NotImplementedException();
        }

        public void SaveLoading(Loading loading)
        {
            try
            {
                this.Connection.ConnectionString = this.WMSConnectionString();
                if (string.IsNullOrEmpty(loading.LOADINGNO))
                {
                    loading.ADDDATE = DateTime.Now;
                    loading.LOADINGNO = _helper.GetDocumentNo(DocumentType.Loading);
                    this.Insert(loading);
                }
                else
                {
                    loading.EDITDATE = DateTime.Now;
                    this.Update(loading);
                }

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

        public bool UpdateLoading(Loading loading)
        {
            throw new NotImplementedException();
        }

        public bool DeleteLoading(string loading)
        {
            try
            {
                Loading o = new Loading() { LOADINGNO = loading };
                this.Connection.ConnectionString = this.WMSConnectionString();
                return this.Delete(o);
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

        public bool CreateOrUpdateLoading(Loading loading)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<TestLoading> GetTestLoadingAll()
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                
                string sql = string.Empty;
                sql = "SELECT [name]='nut', " +
                "[est]='2017-01-01'," +
                "[contacts]='2017-01-01'," +
                "[status]='1'," +
                "[target-actual]='2017-01-01'," +
                "[starts]='2017-01-01'," +
                "[ends]='2017-01-01'," +
                "[tracker]='2017-01-01', " +
                "[comments]='2017-01-01', ";
                sql += "[action]='<button class=' + char(39) + 'button-info' + char(39) + '>Save Changes</button>' ";
                return cn.Query<TestLoading>(sql);
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
