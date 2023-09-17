using System.Collections.Generic;
using System.Threading.Tasks;
using DapperExtensions;

namespace WMSPortal.Data.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDataRepository<T> where T : new()
    {
        #region Sync

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        IEnumerable<T> GetWhere(object filters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        T GetFirst(object filters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool Insert(T instance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Delete(object key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool Update(T instance);

        string WMSConnectionString();

        #endregion

        #region Async

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetWhereAsync(object filters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<T> GetFirstAsync(object filters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T instance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        Task<bool> InsertAsync(T instance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(object instance);

        #endregion
    }
}
