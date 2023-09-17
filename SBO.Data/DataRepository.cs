using WMSPortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using System.Data.SqlClient;
using MicroOrm.Pocos.SqlGenerator;
using WMSPortal.Core.Model;

namespace WMSPortal.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DataRepository<T> : IDataRepository<T> where T : new()
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        #region Properties
        protected ICacheProvider Cache { get; set; }
        protected IDbConnection Connection { get; private set; }
        protected ISqlGenerator<T> SqlGenerator { get; private set; }

        #endregion

        protected DataRepository(ICacheProvider cache,IDbConnection connection, ISqlGenerator<T> sqlGenerator)
        {
            Connection = connection;
            SqlGenerator = sqlGenerator;
            Cache = cache;
        }
        #endregion

        #region Repository sync base actions

        public virtual IEnumerable<T> GetAll()
        {
            var sql = SqlGenerator.GetSelectAll();
            return Connection.Query<T>(sql);
        }

        public virtual IEnumerable<T> GetWhere(object filters)
        {
            var sql = SqlGenerator.GetSelect(filters);
            return Connection.Query<T>(sql, filters);
        }

        public virtual T GetFirst(object filters)
        {
            return this.GetWhere(filters).FirstOrDefault();
        }

        public virtual bool Insert(T instance)
        {
            bool added = false;
           
            var sql = SqlGenerator.GetInsert();

            if (SqlGenerator.IsIdentity)
            {
                var newId = Connection.Query<decimal>(sql, instance).Single();
                added = newId > 0;

                if (added)
                {
                    var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentityProperty.PropertyInfo.PropertyType);
                    SqlGenerator.IdentityProperty.PropertyInfo.SetValue(instance, newParsedId);
                }
            }
            else
            {
                added = Connection.Execute(sql, instance) > 0;
            }

            return added;
        }

        public virtual bool Delete(object key)
        {
            var sql = SqlGenerator.GetDelete();
            return Connection.Execute(sql, key) > 0;
        }

        public virtual bool Update(T instance)
        {
            var sql = SqlGenerator.GetUpdate();
            return Connection.Execute(sql, instance) > 0;
        }

        #endregion

        #region Repository async base action

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var sql = SqlGenerator.GetSelectAll();
            return await Connection.QueryAsync<T>(sql);
        }

        public virtual async Task<IEnumerable<T>> GetWhereAsync(object filters)
        {
            var sql = SqlGenerator.GetSelect(filters);
            return await Connection.QueryAsync<T>(sql, filters);
        }

        public virtual async Task<T> GetFirstAsync(object filters)
        {
            var sql = SqlGenerator.GetSelect(filters);
            Task<IEnumerable<T>> queryTask = Connection.QueryAsync<T>(sql, filters);
            IEnumerable<T> data = await queryTask;
            return data.FirstOrDefault();
        }

        public virtual async Task<bool> UpdateAsync(T instance)
        {
            bool added = false;
            var sql = SqlGenerator.GetInsert();

            if (SqlGenerator.IsIdentity)
            {
                Task<IEnumerable<decimal>> queryTask = Connection.QueryAsync<decimal>(sql, instance);
                IEnumerable<decimal> result = await queryTask;
                var newId = result.Single();
                added = newId > 0;

                if (added)
                {
                    var newParsedId = Convert.ChangeType(newId, SqlGenerator.IdentityProperty.PropertyInfo.PropertyType);
                    SqlGenerator.IdentityProperty.PropertyInfo.SetValue(instance, newParsedId);
                }
            }
            else
            {
                Task<IEnumerable<int>> queryTask = Connection.QueryAsync<int>(sql, instance);
                IEnumerable<int> result = await queryTask;
                added = result.Single() > 0;
            }

            return added;
        }

        public virtual async Task<bool> InsertAsync(T instance)
        {
            var sql = SqlGenerator.GetDelete();
            Task<IEnumerable<int>> queryTask = Connection.QueryAsync<int>(sql, instance);
            IEnumerable<int> result = await queryTask;
            return result.SingleOrDefault() > 0;
        }

        public virtual async Task<bool> DeleteAsync(object instance)
        {
            var sql = SqlGenerator.GetUpdate();
            Task<IEnumerable<int>> queryTask = Connection.QueryAsync<int>(sql, instance);
            IEnumerable<int> result = await queryTask;
            return result.SingleOrDefault() > 0;
        }

        #endregion
        public virtual string WMSConnectionString()
        {
            Role activeConnection = Cache.Get("RoleConnection") as Role;
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder["Data Source"] = activeConnection.ServerName;
            builder["Initial Catalog"] = activeConnection.DatabaseName;
            builder["uid"] = activeConnection.UserName;
            builder["password"] = activeConnection.Password;
            return builder.ConnectionString;
    }
    }
}
