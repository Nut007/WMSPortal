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
    public class StockBalanceRepository : DataRepository<LotxLocxId>, IStockBalanceRepository
    {
        public StockBalanceRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<LotxLocxId> sqlGenerator)
            : base( cache,connection, sqlGenerator)
        {

        }
        public IEnumerable<LotxLocxId> GetInventory(List<string> columnvalues, string viewBy, int pageSize, int page,string userId)
        {
            string condition = string.Empty;
            string conditionTemplate = "{0} LIKE '%{1}%'";
            string paging = " OFFSET (@RowsPerPage * (@DesiredPageNumber - 1)) ROWS FETCH NEXT @RowsPerPage ROWS ONLY;";

            foreach (string colvalue in columnvalues)
            {
                string[] values = colvalue.ToString().Split('|');
                if (condition == string.Empty)
                    condition = string.Format(conditionTemplate, values[0] ,values[1] );
                else
                    condition += " AND " + string.Format(conditionTemplate, values[0], values[1]);
            }
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                cn.Open();
                string sql = string.Empty;
                
                string groupBy = string.Empty;
                string orderBy = string.Empty;


                condition = "WHERE l.Qty-(l.QtyAllocated+l.QtyPicked)>0 And " + condition + string.Format(" And l.StorerKey IN (SELECT STORERKEY FROM USERSTORERGROUP WHERE USERID='{0}')", userId);
                if (viewBy == "0")
                {
                    sql = "SELECT " +
                    "COUNT(*) OVER() TotalRows," +
                    //"l.WarehouseKey," +
                    "l.StorerKey," +
                    "l.Lot," +
                    "l.Loc, " +
                    "l.Id," +
                    "rd.UnitPrice," +
                    "rd.NetWgt," +
                    "rd.GrossWgt," +
                    "[Sku]=(CASE WHEN l.Sku='DUMMY SKU' THEN rd.Reference5 ELSE l.Sku END)," +
                    "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=l.Storerkey AND SKU=l.Sku),"+
                    "[ImportEntry]=(SELECT Reference1 FROM RECEIPT WHERE RECEIPTKEY=rd.Receiptkey)," +
                    //"l.Sku," +
                    "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=l.StorerKey AND SKU=l.Sku)," +
                    "l.Qty," +
                    "l.QtyAllocated," +
                    "l.QtyPicked," +
                    "[QtyAvaliable]=(l.Qty-(l.QtyAllocated+l.QtyPicked)), " +
                    "lt.Lottable01," +
                    "lt.Lottable02," +
                    "lt.Lottable03," +
                    "lt.Lottable04," +
                    "lt.Lottable05 " +
                    "FROM " +
                    "LotxLocxId l LEFT JOIN LotAttribute lt ON l.Lot=lt.Lot "+
                    "LEFT JOIN ReceiptDetail rd ON l.id=rd.TOID ";
                    orderBy = "ORDER BY l.StorerKey,l.Sku";
                }
                else if (viewBy == "1")
                {
                    sql = "SELECT " +
                    "COUNT(*) OVER() TotalRows," +
                   //"l.WarehouseKey," +
                   "l.StorerKey," +
                   //"l.Sku," +
                   "[Sku]=(CASE WHEN l.Sku='DUMMY SKU' THEN rd.Reference5 ELSE l.Sku END)," +
                   "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=l.StorerKey AND SKU=l.Sku)," +
                   "[Qty]=SUM(l.Qty)," +
                   "[QtyAllocated]=SUM(l.QtyAllocated)," +
                   "[QtyPicked]=SUM(l.QtyPicked)," +
                   "[QtyAvaliable]=SUM(l.Qty-(l.QtyAllocated+l.QtyPicked)) " +
                   "FROM " +
                   "LotxLocxId l LEFT JOIN LotAttribute lt ON l.Lot=lt.Lot "+
                   "LEFT JOIN ReceiptDetail rd ON l.id=rd.TOID ";
                    groupBy = "GROUP BY l.StorerKey,l.Sku,(CASE WHEN l.Sku='DUMMY SKU' THEN rd.Reference5 ELSE l.Sku END) ";
                    orderBy = "ORDER BY l.StorerKey,l.Sku";
                }
                else if (viewBy == "2")
                {
                    sql = "SELECT " +
                       "COUNT(*) OVER() TotalRows," +
                      //"l.WarehouseKey," +
                      "l.StorerKey," +
                      "[Sku]=(CASE WHEN l.Sku='DUMMY SKU' THEN rd.Reference5 ELSE l.Sku END)," +
                      "l.Loc," +
                      "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=l.StorerKey AND SKU=l.Sku)," +
                      "[Qty]=SUM(l.Qty)," +
                      "[QtyAllocated]=SUM(l.QtyAllocated)," +
                      "[QtyPicked]=SUM(l.QtyPicked)," +
                      "[QtyAvaliable]=SUM(l.Qty-(l.QtyAllocated+l.QtyPicked)) " +
                      "FROM " +
                      "LotxLocxId l LEFT JOIN LotAttribute lt ON l.Lot=lt.Lot " +
                      "LEFT JOIN ReceiptDetail rd ON l.id=rd.TOID ";
                    groupBy = "GROUP BY l.StorerKey,l.Sku,l.Loc,(CASE WHEN l.Sku='DUMMY SKU' THEN rd.Reference5 ELSE l.Sku END) ";
                    orderBy = "ORDER BY l.StorerKey,l.Sku,l.Loc ";
                }

                sql = sql + condition + groupBy + orderBy +paging;

                return cn.Query<LotxLocxId>(string.Format(sql, columnvalues[0]), new { RowsPerPage = pageSize, DesiredPageNumber = page });
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
        public IEnumerable<LotxLocxId> GetAllInventory()
        {
            string condition = string.Empty;          
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                cn.Open();
                string sql = string.Empty;

                string groupBy = string.Empty;
                string orderBy = string.Empty;
                sql = "SELECT [PcNo]=PCNO " +
                      "FROM " +
                      "TEMP_ID ";

                return cn.Query<LotxLocxId>(sql);
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

        public IEnumerable<LotxLocxId> GetCommodityInfo(string pcNo)
        {
            string condition = string.Empty;
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                cn.Open();
                string sql = string.Empty;

                string groupBy = string.Empty;
                string orderBy = string.Empty;

                sql = "SELECT " +
                      "[PcNo]=PCNO, " +
                      "[Sku]=Sku, " +
                      "[Qty]=PACKUNIT, " +
                      "[Loc]=SITE " +
                      "FROM " +
                      "TEMP_ID ";
                if (pcNo != string.Empty)
                    sql += string.Format("WHERE PCNO='{0}' AND (SKU LIKE '%toy%' OR SKU LIKE '%watch%' OR SKU LIKE '%bluetooth%' OR SKU LIKE '%nail%')", pcNo);
                else
                    sql += string.Format("WHERE PCNO=null", pcNo);
                return cn.Query<LotxLocxId>(sql);
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
        public IEnumerable<LotxLocxId> GetCommodityInfo()
        {
            throw new NotImplementedException();
        }
        public IEnumerable<LotxLocxId> GetInventoryBySku(string storerKey, string sku)
        {
            string condition = string.Empty;
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                cn.Open();
                string sql = string.Empty;

                sql = "SELECT " +
                    "l.StorerKey," +
                    "l.Lot," +
                    "l.Loc, " +
                    "l.Id," +
                    "rd.UnitPrice," +
                    "rd.NetWgt," +
                    "rd.GrossWgt," +
                    "[Sku]=(CASE WHEN l.Sku='DUMMY SKU' THEN rd.Reference5 ELSE l.Sku END)," +
                    "[ImportEntry]=(SELECT Reference1 FROM RECEIPT WHERE RECEIPTKEY=rd.Receiptkey)," +
                    "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=l.StorerKey AND SKU=l.Sku)," +
                    "l.Qty," +
                    "l.QtyAllocated," +
                    "l.QtyPicked," +
                    "[QtyAvaliable]=(l.Qty-(l.QtyAllocated+l.QtyPicked)), " +
                    "lt.Lottable01," +
                    "lt.Lottable02," +
                    "lt.Lottable03," +
                    "lt.Lottable04," +
                    "lt.Lottable05 " +
                    "FROM " +
                    "LotxLocxId l LEFT JOIN LotAttribute lt ON l.Lot=lt.Lot " +
                    "LEFT JOIN ReceiptDetail rd ON l.id=rd.TOID  " +
                    "WHERE l.StorerKey=@StorerKey AND l.Sku=@Sku AND (l.Qty-(l.QtyAllocated+l.QtyPicked)) > 0 ORDER BY l.Id";

                return cn.Query<LotxLocxId>(sql, new { StorerKey = storerKey,Sku=sku });
            }
            catch (Exception ex)
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
        public IEnumerable<LotxLocxId> GetInventoryGroupBySku(string column, string value, string userId)
        {
            string condition = string.Empty;
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                cn.Open();
                string sql = string.Empty;

                sql = "SELECT " +
                    "l.StorerKey," +
                    "l.Sku," +
                    "[SkuDescription]=s.DESCR," +
                    "[QtyAvaliable]=SUM((l.Qty-(l.QtyAllocated+l.QtyPicked))) " +
                    "FROM " +
                    "LotxLocxId l LEFT JOIN LotAttribute lt ON l.Lot=lt.Lot " +
                    "LEFT JOIN ReceiptDetail rd ON l.id=rd.TOID  " +
                    "LEFT JOIN SKU s ON s.STORERKEY=l.STORERKEY AND s.SKU=l.SKU  " +
                    string.Format("WHERE {0} LIKE '{1}%' AND (l.Qty-(l.QtyAllocated+l.QtyPicked)) > 0 AND l.StorerKey IN (SELECT STORERKEY FROM USERSTORERGROUP WHERE USERID='{2}') GROUP BY l.StorerKey,l.Sku,s.DESCR", column, value,userId);
                if (value != string.Empty)
                    return cn.Query<LotxLocxId>(sql);
                else
                    return new List<LotxLocxId>();
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
