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
    public class UserRepository : DataRepository<User>, IUserRepository
    {
        public UserRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<User> sqlGenerator)
            : base(cache,connection, sqlGenerator)
        {

        }

        public IEnumerable<User> GetUsers(string column, string value)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                string sql = string.Empty;
                if (column == null)
                    sql = string.Format("SELECT * FROM [USER] WHERE UserName LIKE '%' + @SearchValue +'%'", column);
                else
                    sql = string.Format("SELECT * FROM [USER] WHERE {0} LIKE '%' + @SearchValue +'%'", column);
                return cn.Query<User>(sql, new { SearchValue = value });
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

        public User GetUser(int userID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetUserList()
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
                return cn.GetList<User>();

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

        public int CreateUser(User user)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
                return cn.Insert<User>(user);

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

        public bool UpdateUser(User user)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
                return cn.Update<User>(user);

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

        public bool DeleteUsers(List<int> userIDs)
        {

            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
                var query = "DELETE FROM [User] WHERE UserID IN @Id";
                var result = cn.Query<Role>(query, new { Id = userIDs.ToArray() });
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

        public bool CreateOrUpdateUser(User user)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();

                using (var transaction = cn.BeginTransaction())
                {
                    if (user._IsNew)
                        cn.Insert<User>(user, transaction);
                    else
                        cn.Update<User>(user, transaction);

                    foreach (var userRole in user.UserRoles)
                    {
                        if (userRole._IsNew)
                        {
                            userRole.UserId = user.UserID;
                            cn.Insert<UserRole>(userRole, transaction);
                        }
                        else
                            if (!userRole.IsSelected)
                                cn.Delete<UserRole>(userRole, transaction);
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
        public User FindByUserAndPassword(string userName, string password)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
                var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                pg.Predicates.Add(Predicates.Field<User>(f => f.UserName, Operator.Eq, userName));
                pg.Predicates.Add(Predicates.Field<User>(f => f.Password, Operator.Eq, password));
                User user = cn.GetList<User>(pg).SingleOrDefault();

                if (user != null)
                {
                    var pgRole = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    pgRole.Predicates.Add(Predicates.Field<UserRole>(f => f.UserId, Operator.Eq, user.UserID));
                    user.UserRoles = cn.GetList<UserRole>(pgRole);
                    user.Roles = this.GetRolesByUserId(user.UserID);
                    user.ApplicationRoles = this.GetApplicationRolesByUserId(user.UserID);
                    
                }
                return user;

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
        public IEnumerable<Role> GetRolesByUser(string userName)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                string query;
                query = "SELECT " +
                "* " +
                "FROM " +
                "[ROLE] WHERE Id IN " +
                "(" +
                "SELECT " +
                "RoleId " +
                "FROM " +
                "UserRole ur LEFT JOIN [User] u ON ur.UserID = u.UserID " +
                "WHERE u.UserName =@userName " +
                ") ";

                cn.Open();
                var result = cn.Query<Role>(query, new { userName = userName });
                return result;
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


        public IEnumerable<Role> GetRolesByUserId(int userId)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                string query;
                query = "SELECT " +
                "* " +
                "FROM " +
                "[ROLE] WHERE Id IN " +
                "(" +
                "SELECT " +
                "RoleId " +
                "FROM " +
                "UserRole ur LEFT JOIN [User] u ON ur.UserID = u.UserID " +
                "WHERE u.UserID =@userID " +
                ") ";

                cn.Open();
                var result = cn.Query<Role>(query, new { userID = userId });
                return result;
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

        public IEnumerable<ApplicationRole> GetApplicationRolesByUserId(int userId)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                string query;
                query = "SELECT " +
                "* " +
                "FROM " +
                "[ApplicationRole] WHERE RoleId IN " +
                "(" +
                "SELECT " +
                "DISTINCT " +
                "RoleId " +
                "FROM " +
                "UserRole ur LEFT JOIN [User] u ON ur.UserID = u.UserID " +
                "WHERE u.UserID =@userID " +
                ") ";

                cn.Open();
                var result = cn.Query<ApplicationRole>(query, new { userID = userId });
                return result;
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
