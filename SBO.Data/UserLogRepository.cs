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
    public class UserLogRepository : DataRepository<UserLog>, IUserLogRepository
    {
        public UserLogRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<UserLog> sqlGenerator)
            : base(cache,connection, sqlGenerator)
        {

        }
     
        public int CreateUserLog(UserLog userLog)
        {
            var cn = new SqlConnection(this.Connection.ConnectionString);
            try
            {
                cn.Open();
                return cn.Insert<UserLog>(userLog);

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
