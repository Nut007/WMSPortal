using MicroOrm.Pocos.SqlGenerator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using WMSPortal.Core.Model;
using WMSPortal.Data.Repositories;

namespace WMSPortal.Data
{
    public class RoleRepository : DataRepository<Role>, IRoleRepository
    {
        public RoleRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<Role> sqlGenerator)
            : base( cache,connection, sqlGenerator)
        {

        }
       
        public IEnumerable<Role> GetRoleList()
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
                return cn.GetList<Role>();

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

        public int CreateRole(Role role)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
                return cn.Insert<Role>(role);

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

        public bool UpdateRole(Role role)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
                return cn.Update<Role>(role);

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

        public bool DeleteRole(Role lookupMaster)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Role> GetRoles(string column, string value)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
                string sql = string.Empty;
                sql = string.Format("SELECT * FROM Role WHERE Name LIKE '%' + @SearchValue +'%'", column);

                return cn.Query<Role>(sql, new { SearchValue = value });
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

        public bool DeleteRoles(List<int> roldIDs)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
                var query = "DELETE FROM Role WHERE Id IN @Id";
                var result = cn.Query<Role>(query, new { Id = roldIDs.ToArray() });
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
        public Role GetRole(int Id)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
                var predicate = Predicates.Field<Role>(f => f.Id, Operator.Eq, Id);
                return cn.GetList<Role>(predicate).SingleOrDefault();
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
        public bool CreateOrUpdateRole(Role role)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
               
                using (var transaction = cn.BeginTransaction())
                {
                    if (role._IsNew)
                        cn.Insert<Role>(role, transaction);
                    else
                        cn.Update<Role>(role, transaction);

                    foreach (var applicationRole in role.ApplicationRoles)
                    {
                        if (applicationRole._IsNew)
                        {
                            applicationRole.RoleId = role.Id;
                            cn.Insert<ApplicationRole>(applicationRole, transaction);
                        }
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



        public IEnumerable<Role> GetRolesByUser(string column, string value)
        {
            throw new NotImplementedException();
        }
    }
}
