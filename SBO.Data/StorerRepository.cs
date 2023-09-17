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
    public class StorerRepository : DataRepository<Storer>, IStorerRepository
    {
        public StorerRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<Storer> sqlGenerator)
            : base( cache,connection, sqlGenerator)
        {

        }
        public IEnumerable<Storer> GetStorerByName(string storerName)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
             
                string sql = string.Empty;
                sql = "SELECT " +
                      "[StorerKey]=StorerKey, " +
                      "[Company]=RTRIM(LTRIM(s.COMPANY)), " +
                      "Contact1, " +
                      "Email1, " +
                      "Address1, " +
                      "Address2, " +
                      "Address3, " +
                      "Address4 " +
                      "FROM " +
                      "STORER s " +
                      "WHERE s.COMPANY LIKE @StorerName";
                if (storerName != string.Empty)
                    return cn.Query<Storer>(sql, new { StorerName = storerName + "%" });
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
        public IEnumerable<Storer> GetStorerByNameByUserId(string storerName,string userId)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {

                string sql = string.Empty;
                sql = "SELECT " +
                      "[StorerKey]=StorerKey, " +
                      "[Company]=RTRIM(LTRIM(s.COMPANY)), " +
                      "Contact1, " +
                      "Email1, " +
                      "Address1, " +
                      "Address2, " +
                      "Address3, " +
                      "Address4 " +
                      "FROM " +
                      "STORER s " +
                      "WHERE s.COMPANY LIKE @StorerName AND STORERKEY IN (SELECT STORERKEY FROM USERSTORERGROUP WHERE UserId=@UserId)";
                if (storerName != string.Empty)
                    return cn.Query<Storer>(sql, new { StorerName = storerName + "%", UserId = userId });
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
