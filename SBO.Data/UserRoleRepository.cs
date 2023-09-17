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
    public class UserRoleRepository : DataRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(ICacheProvider cache,IDbConnection connection, ISqlGenerator<UserRole> sqlGenerator)
            : base(cache,connection, sqlGenerator)
        {

        }

        public IEnumerable<UserRole> GetUserRoles(int userId)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();

                string sql = "SELECT " +
                            "[_IsNew]=(CASE WHEN COALESCE(UserRole.RoleId,'')='' THEN 1 ELSE 0 END)," +
                            "[UserId]=@UserId," +
                            "[RoleId]=[Role].Id," +
                            "[Role].Name," +
                            "[IsSelected]=(CASE WHEN COALESCE(UserRole.RoleId,'')='' THEN 0 ELSE 1 END) " +
                            "FROM [Role] LEFT JOIN " +
                            "(" +
                            "SELECT * FROM UserRole WHERE UserRole.UserId =@UserId " +
                            ") UserRole " +
                            "ON [Role].Id=UserRole.RoleId  ";
               return cn.Query<UserRole>(sql, new { UserId = userId });
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
        public bool CreateOrUpdateUserRoles(IEnumerable<UserRole> userRoles)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
                using (var transaction = cn.BeginTransaction())
                {
                    foreach (var role in userRoles)
                    {
                        if (role._IsNew)
                            cn.Insert<UserRole>(role, transaction);
                        else
                            cn.Update<UserRole>(role, transaction);
                    }
                    transaction.Commit();
                }
                return true;
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
