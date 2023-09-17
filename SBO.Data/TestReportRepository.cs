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
using System.Globalization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.IO;
using WMSPortal.Core.Model;

namespace WMSPortal.Data
{
    public class TestReportRepository
    {
        public IEnumerable<ImportDeclarationReport> GetImportDeclaraion(string startDate, string stopDate, string customerCode)
        {
            string connectionString = "server=10.137.15.2;database=CUSTOMS_FZ;uid=sa;password=sql";
            var cn = new SqlConnection(connectionString);
            try
            {
                string sql = string.Empty;
                sql = "SELECT " +
                      "r.ReceiptKey," +
                      "[ImportDeclarationNo]=r.Reference1," +
                      "[ImportDeclarationItemNo]=rd.POLineNumber," +
                      "[ImportDeclarationDate]=r.ReceiptDate," +
                      "[WarehouseReceivedDate]=rd.DateReceived," +
                      "[Sku]=rd.Sku, " +
                      "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=rd.StorerKey AND SKU=rd.Sku)," +
                      "[Quantity]=rd.QtyReceived,  " +
                      "[UnitPrice]=rd.UnitPrice," +
                      "[NetWgt]=rd.NetWgt," +
                      "[GrossWgt]=rd.GrossWgt, " +
                      "[TotalAmount]=rd.UnitPrice*rd.QtyReceived, " +
                      "[TotalCustomsTax]=rd.ExtendedPrice " +
                      "FROM " +
                      "Receipt r LEFT JOIN ReceiptDetail rd ON r.Receiptkey=rd.Receiptkey " +
                      "WHERE CONVERT(VARCHAR,r.ReceiptDate,112) BETWEEN @StartDate AND @StopDate AND rd.ConditionCode <> 'NOTOK' ";

                if (customerCode == string.Empty)
                    return cn.Query<ImportDeclarationReport>(sql, new { StartDate = startDate, StopDate = stopDate });
                else
                {
                    sql += "AND rd.StorerKey=@CustomerCode";
                    return cn.Query<ImportDeclarationReport>(sql, new { StartDate = startDate, StopDate = stopDate, CustomerCode = customerCode });
                }
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
        public IEnumerable<ExportDeclarationReport> GetExportDeclaraion(string startDate, string stopDate, string customerCode)
        {
            string connectionString = "server=10.137.15.2;database=CUSTOMS_FZ;uid=sa;password=sql";
            var cn = new SqlConnection(connectionString);
            try
            {
                string groupBy = string.Empty;
                string sql = string.Empty;
                sql = "SELECT " +
                      "o.OrderKey," +
                      "[ExportDeclarationNo]=o.ShippingInstructions1," +
                      "[ExportDeclarationItemNo]=od.OrderLineNumber," +
                      "[ExportDeclarationDate]=o.OrderDate," +
                      "[WarehouseOrderdDate]=od.AddDate," +
                      "[Sku]=od.Sku, " +
                      "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=od.StorerKey AND SKU=od.Sku)," +
                      "[Quantity]=SUM(pd.Qty),  " +
                      "[UnitPrice]=od.UnitPrice," +
                      "[NetWgt]=od.NetWeight," +
                      "[GrossWgt]=od.GrossWeight, " +
                      "[TotalAmount]=od.UnitPrice*SUM(pd.Qty), " +
                      "[DutyAmount]=od.ExtendedPrice, " +
                      "[ImportDeclarationNo]=r.Reference1, " +
                      "[ImportDeclarationItemNo]=rd.POLineNumber " +
                      "FROM " +
                      "Orders o LEFT JOIN OrderDetail od ON o.OrderKey=od.OrderKey " +
                      "LEFT JOIN PickDetail pd ON od.Orderkey=pd.Orderkey AND od.OrderLineNumber=pd.OrderLineNumber " +
                      "LEFT JOIN ReceiptDetail rd ON pd.ID=rd.ToId " +
                      "LEFT JOIN Receipt r ON rd.ReceiptKey=r.ReceiptKey " +
                      "WHERE CONVERT(VARCHAR,o.OrderDate,112) BETWEEN @StartDate AND @StopDate ";

                groupBy = "GROUP BY " +
                    "o.OrderKey," +
                    "o.ShippingInstructions1," +
                    "od.OrderLineNumber," +
                    "o.OrderDate," +
                    "od.AddDate," +
                    "od.Sku, " +
                    "od.StorerKey," +
                    "od.UnitPrice," +
                    "od.NetWeight," +
                    "od.GrossWeight, " +
                    "od.ExtendedPrice, " +
                    "r.Reference1, " +
                    "rd.POLineNumber ";

                if (customerCode == string.Empty)
                {
                    sql += groupBy;
                    return cn.Query<ExportDeclarationReport>(sql, new { StartDate = startDate, StopDate = stopDate });
                }
                else
                {
                    sql += "AND r.StorerKey=@CustomerCode ";
                    sql += groupBy;
                    return cn.Query<ExportDeclarationReport>(sql, new { StartDate = startDate, StopDate = stopDate, CustomerCode = customerCode });
                }
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
        public IEnumerable<GLDeclarationReport> GetGLDeclaration(string startDate, string stopDate, string customerCode)
        {
            string connectionString = "server=10.137.15.2;database=CUSTOMS_FZ;uid=sa;password=sql";
            var cn = new SqlConnection(connectionString);
            try
            {
                string groupBy = string.Empty;
                string sql = string.Empty;

                sql = "SELECT " +
                "[Id]=rd.ToId," +
                "r.ReceiptKey," +
                "[ImporterName]=s.Company," +
                "[ImportDeclarationNo]=r.Reference1," +
                "[ImportDeclarationItemNo]=rd.POLineNumber," +
                "[ImportDeclarationDate]=r.ReceiptDate," +
                "[WarehouseReceivedDate]=rd.DateReceived," +
                "[Sku]=rd.Sku, " +
                "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=rd.StorerKey AND SKU=rd.Sku)," +
                "[Quantity]=rd.QtyReceived,  " +
                "[NetWgt]=rd.NetWgt," +
                "pd.OrderKey," +
                "[ExportDeclarationNo]=(SELECT o.ShippingInstructions1 FROM ORDERS o WHERE OrderKey=pd.OrderKey)," +
                "[ExportDeclarationDate]=(SELECT o.OrderDate FROM ORDERS o WHERE OrderKey=pd.OrderKey)," +
                "[QtyPicked]=SUM(pd.Qty) " +
                "FROM " +
                "Receipt r LEFT JOIN ReceiptDetail rd ON r.Receiptkey=rd.Receiptkey " +
                "LEFT JOIN Storer s ON r.StorerKey=s.StorerKey " +
                "LEFT JOIN PickDetail pd ON rd.ToId=pd.Id " +
                "WHERE CONVERT(VARCHAR,r.ReceiptDate,112) BETWEEN @StartDate AND @StopDate  AND rd.ConditionCode <> 'NOTOK'  ";

                groupBy = "GROUP BY " +
                "r.ReceiptKey," +
                "r.Reference1," +
                "rd.POLineNumber," +
                "rd.ToId," +
                "rd.StorerKey," +
                "r.ReceiptDate," +
                "rd.Sku, " +
                "rd.QtyReceived,  " +
                "rd.DateReceived,  " +
                "rd.NetWgt," +
                "s.Company," +
                "pd.OrderKey " +
                "ORDER BY r.ReceiptKey,rd.POLineNumber ";

                if (customerCode == string.Empty)
                {
                    sql += groupBy;
                    return cn.Query<GLDeclarationReport>(sql, new { StartDate = startDate, StopDate = stopDate });
                }
                else
                {
                    sql += "AND r.StorerKey=@CustomerCode ";
                    sql += groupBy;
                    return cn.Query<GLDeclarationReport>(sql, new { StartDate = startDate, StopDate = stopDate, CustomerCode = customerCode });
                }
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
        public IEnumerable<GLDeclarationReport> GetInventoryDeclaration(string customerCode)
        {
            string connectionString = "server=10.137.15.2;database=CUSTOMS_FZ;uid=sa;password=sql";
            var cn = new SqlConnection(connectionString);
            try
            {
                string groupBy = string.Empty;
                string sql = string.Empty;

                sql = "SELECT " +
                    "r.ReceiptKey, " +
                    "[ImportDeclarationNo]=r.Reference1, " +
                    "[ImportDeclarationItemNo]=rd.POLineNumber, " +
                    "[ImportDeclarationDate]=r.ReceiptDate, " +
                    "[ImporterName]=s.Company," +
                    "[WarehouseReceivedDate]=rd.DateReceived, " +
                    "[Sku]=l.Sku,  " +
                    "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=l.StorerKey AND SKU=l.Sku), " +
                    "[Quantity]=SUM(l.Qty-(l.QtyPicked+l.QtyAllocated))," +
                    "[NetWgt]=rd.NetWgt ," +
                    "[UnitPrice]=rd.UnitPrice ," +
                    "[CustomsTax]=rd.OtherUnit1 " +
                    "FROM " +
                    "LotxlocxId l LEFT JOIN ReceiptDetail rd ON l.id=rd.ToId " +
                    "LEFT JOIN Storer s ON l.StorerKey=s.StorerKey " +
                    "LEFT JOIN Receipt r ON rd.Receiptkey=r.Receiptkey " +
                    "WHERE " +
                    "(l.Qty-(l.QtyPicked+l.QtyAllocated)) > 0 ";

                groupBy = "GROUP BY " +
                "r.ReceiptKey, " +
                "r.Reference1, " +
                "rd.POLineNumber, " +
                "r.ReceiptDate," +
                "rd.QtyReceived," +
                "rd.DateReceived," +
                "rd.NetWgt," +
                "rd.UnitPrice, " +
                "l.StorerKey," +
                "l.Sku," +
                "rd.OtherUnit1," +
                "s.Company " +
                "ORDER BY r.ReceiptKey,rd.POLineNumber ";


                if (customerCode == string.Empty)
                {
                    sql += groupBy;
                    return cn.Query<GLDeclarationReport>(sql);
                }
                else
                {
                    sql += "AND r.StorerKey=@CustomerCode ";
                    sql += groupBy;
                    return cn.Query<GLDeclarationReport>(sql, new { CustomerCode = customerCode });
                }
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
        public InventoryMovementReport GetMovementTransection(string startDate, string stopDate, string customerCode)
        {
            string previousDate = DateTime.ParseExact(startDate,
                                        "yyyyMMdd",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None).AddDays(-1).ToString("yyyyMMdd");
            InventoryMovementReport movementReport = new InventoryMovementReport();
            string connectionString = "server=10.137.15.2;database=CUSTOMS_FZ;uid=sa;password=sql";
            var cn = new SqlConnection(connectionString);
            try
            {
                string groupBy = string.Empty;
                string sql = string.Empty;

                sql = "SELECT  " +
                "[Id]=rd.ToId, " +
                "r.ReceiptKey, " +
                "[ImportDeclarationNo]=r.Reference1, " +
                "[ImportDeclarationItemNo]=rd.POLineNumber, " +
                "[ImportDeclarationDate]=r.ReceiptDate, " +
                "[WarehouseReceivedDate]=rd.DateReceived, " +
                "[ImporterName]=s.Company," +
                "[Sku]=rd.Sku, " +
                "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=rd.StorerKey AND SKU=rd.Sku), " +
                "[Quantity]=rd.QtyReceived, " +
                "[UnitPrice]=rd.UnitPrice, " +
                "[DutyAmount]=rd.OtherUnit1, " +
                //"[QtyPicked]=COALESCE((SELECT SUM(Qty) FROM PickDetail pd LEFT JOIN ORDERS o ON pd.OrderKey=o.OrderKey WHERE pd.Id=rd.ToId AND CONVERT(VARCHAR,o.OrderDate,112) <= @PreviousDate),0), " +
                "[BroughtForward]=rd.QtyReceived-(COALESCE((SELECT SUM(Qty) FROM PickDetail pd LEFT JOIN ORDERS o ON pd.OrderKey=o.OrderKey WHERE pd.Id=rd.ToId AND CONVERT(VARCHAR,o.OrderDate,112) <= @PreviousDate),0)) " +
                "FROM " +
                "Receipt r LEFT JOIN ReceiptDetail rd ON r.Receiptkey=rd.Receiptkey  " +
                "LEFT JOIN Storer s ON r.StorerKey=s.StorerKey  " +
                "WHERE CONVERT(VARCHAR,r.ReceiptDate,112) <= @PreviousDate " +
                "AND rd.ConditionCode <> 'NOTOK'  ";

                groupBy = "GROUP BY  " +
                "r.ReceiptKey, " +
                "rd.ToId, " +
                "r.Reference1, " +
                "rd.POLineNumber, " +
                "rd.StorerKey, " +
                "r.ReceiptDate, " +
                "rd.Sku,  " +
                "rd.QtyReceived,  " +
                "rd.DateReceived, " +
                "rd.UnitPrice, " +
                "rd.OtherUnit1, " +
                "s.Company " +
                "ORDER BY r.ReceiptKey,rd.POLineNumber  ";


                if (customerCode == string.Empty)
                {
                    sql += groupBy;
                    movementReport.BroughtForwardItems = cn.Query<GLDeclarationReport>(sql, new { PreviousDate = previousDate });
                }
                else
                {
                    sql += "AND r.StorerKey=@CustomerCode ";
                    sql += groupBy;
                    movementReport.BroughtForwardItems = cn.Query<GLDeclarationReport>(sql, new { PreviousDate = previousDate, CustomerCode = customerCode });
                }

                sql = "SELECT " +
                "pd.Id," +
                "[ExportDeclarationNo]=o.ShippingInstructions1," +
                "[ExportDeclarationItemNo]=od.OrderLineNumber," +
                "[Quantity]=SUM(pd.Qty)" +
                "FROM " +
                "Orders o LEFT JOIN OrderDetail od ON o.OrderKey=od.OrderKey " +
                "LEFT JOIN PickDetail pd ON od.OrderKey=pd.OrderKey AND od.OrderLineNumber=pd.OrderLineNumber " +
                "WHERE " +
                "CONVERT(VARCHAR,o.OrderDate,112) BETWEEN @StartDate AND @StopDate ";

                groupBy = "GROUP BY " +
                "pd.Id," +
                "o.ShippingInstructions1," +
                "od.OrderLineNumber";


                if (customerCode == string.Empty)
                {
                    sql += groupBy;
                    movementReport.ExportDeclarationItems = cn.Query<ExportDeclarationReport>(sql, new { StartDate = startDate, StopDate = stopDate });
                }
                else
                {
                    sql += "AND o.StorerKey=@CustomerCode ";
                    sql += groupBy;
                    movementReport.ExportDeclarationItems = cn.Query<ExportDeclarationReport>(sql, new { StartDate = startDate, StopDate = stopDate, CustomerCode = customerCode });
                }

                movementReport.GLDeclarationItems = GetGLDeclaration(startDate, stopDate, customerCode);

                return movementReport;
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
