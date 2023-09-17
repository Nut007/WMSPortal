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
    public class CodelkupRepository : DataRepository<Codelkup>, ICodelkupRepository
    {
        
        public CodelkupRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<Codelkup> sqlGenerator)
            : base( cache,connection, sqlGenerator)
        {

        }

        public IEnumerable<Codelkup> GetLookupListByType(LookupType lookupType)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = "SELECT * FROM CODELKUP WHERE LISTNAME=@ListName";
                return cn.Query<Codelkup>(sql, new { ListName = lookupType.ToString() });
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

        public IEnumerable<Codelkup> GetLookupListByGroupType(LookupType lookupType, int? lookupGroupId)
        {
            throw new NotImplementedException();
        }

        public string GetLookupDescripion(LookupType lookupType, int lookupId)
        {
            throw new NotImplementedException();
        }


        public bool DeleteLookupItems(List<string> codeLookupId, string codeLookupGroup)
        {
            try
            {
                var conn = new SqlConnection(this.WMSConnectionString());
                conn.Open();
                try
                {
                    conn.Execute(@"DELETE FROM [Codelkup] WHERE LISTNAME=@CodeLookupGroup AND CODE = @CodeLookupId", codeLookupId.Select(x => new { codeLookupId = x, CodeLookupGroup = codeLookupGroup }).ToArray());
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
               
                return true;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                //
            }
        }

        public void InsertLookup(Codelkup lookup)
        {
            try
            {
                var conn = new SqlConnection(this.WMSConnectionString());
                conn.Open();
                try
                {
                    conn.Insert(lookup);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                //
            }
        }

        public void UpdateLookup(Codelkup lookup)
        {
            try
            {
                var conn = new SqlConnection(this.WMSConnectionString());
                conn.Open();
                try
                {
                    conn.Update(lookup);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                //
            }
        }

        public IEnumerable<Codelkup> GetLookupListByShort(string shortDescr)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = "SELECT * FROM CODELKUP WHERE Short=@Short";
                return cn.Query<Codelkup>(sql, new { Short = shortDescr });
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

        public string GetShortDescripion(LookupType lookupType, string lookupId)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = "SELECT Short FROM CODELKUP WHERE LISTNAME=@ListName AND CODE=@Code";
                return cn.Query<string>(sql, new { ListName = lookupType.ToString(),Code= lookupId }).FirstOrDefault();
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

        public IEnumerable<Codelkup> GetLookupListByIMPC()
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = "SELECT * FROM CODELKUP WHERE LISTNAME LIKE 'IMPC%'";
                return cn.Query<Codelkup>(sql);
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
