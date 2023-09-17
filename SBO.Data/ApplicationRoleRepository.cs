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
    public class ApplicationRoleRepository : DataRepository<ApplicationRole>, IApplicationRoleRepository
    {
        public ApplicationRoleRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<ApplicationRole> sqlGenerator)
            : base( cache,connection, sqlGenerator)
        {
           
        }

        public IEnumerable<ApplicationRole> GetApplicationRoles(int roleId)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
                string sql = "SELECT [_IsNew]=(CASE WHEN COALESCE(ApplicationRole.RoleId,'')='' THEN 1 ELSE 0 END),[RoleId]=@RoleId,Applications.ApplicationId,Applications.ApplicationName,IsAllowAccess,IsRead,IsReadWrite,IsAllowApprove,[IsShowApproval]=(CASE WHEN Applications.ApplicationId IN (1,2) THEN 1 ELSE 0 END) FROM Applications LEFT JOIN " +
                            "(" +
                            "SELECT * FROM ApplicationRole WHERE ApplicationRole.RoleId =@RoleId " +
                            ") ApplicationRole " +
                            "ON Applications.ApplicationId=ApplicationRole.ApplicationId  ";
                sql = string.Format(sql, roleId);

                return cn.Query<ApplicationRole>(sql, new { RoleId = roleId });
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
        public bool CreateOrUpdateApplicationRoles(IEnumerable<ApplicationRole> applicationRoles)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
                using (var transaction = cn.BeginTransaction())
                {
                    foreach (var applicationRole in applicationRoles)
                    {
                        if (applicationRole._IsNew)
                            cn.Insert<ApplicationRole>(applicationRole, transaction);
                        else
                            cn.Update<ApplicationRole>(applicationRole, transaction);
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
