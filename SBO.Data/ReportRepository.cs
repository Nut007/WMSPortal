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
using System.Globalization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.IO;

namespace WMSPortal.Data
{
    public class ReportRepository : DataRepository<ImportDeclarationReport>, IReportRepository
    {
        private IHelperRepository _helperRepository;
        public ReportRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<ImportDeclarationReport> sqlGenerator, IHelperRepository helperRepository)
            : base(cache, connection, sqlGenerator)
        {
            _helperRepository = helperRepository;
        }
        public IEnumerable<ImportDeclarationReport> GetImportDeclaraion(string dateType, string startDate, string stopDate, string year, string month, object[] declarationType, object[] importers,string sku)
        {
            string connectionString = this.WMSConnectionString();
            var cn = new SqlConnection(connectionString);
            try
            {
                string sql = string.Empty;
                sql = "SELECT " +
                      //"r.ReceiptKey," +
                      //"[Id]=rd.ToId, " +
                      "[ImportDeclarationNo]=r.Reference1," +
                      "[ImporterName]=(SELECT COMPANY FROM STORER WHERE STORERKEY =r.STORERKEY)," +
                      "[Invoice]=rd.Reference8," +
                      "[ImportDeclarationItemNo]=rd.POLineNumber," +
                      "[ImportDeclarationDate]=r.ReceiptDate," +
                      "[WarehouseReceivedDate]=CAST(rd.DateReceived as DateTime)," +
                      "[Sku]=rd.Sku, " +
                      "[SkuDescription]=(CASE WHEN rd.Sku='NOVC' THEN rd.Reference5 ELSE (SELECT DESCR FROM SKU WHERE STORERKEY=rd.StorerKey AND SKU=rd.Sku) END)," +
                      "[Quantity]=SUM(rd.QtyReceived),  " +
                      "[UnitPrice]=rd.UnitPrice," +
                      "[NetWgt]=rd.NetWgt," +
                      "[UOM]='ชิ้น'," +
                      "[GrossWgt]=rd.GrossWgt, " +
                      "[TotalAmount]=rd.ExtendedPrice, " +
                      "[TotalDuty]=rd.OtherUnit1, " +
                      "[DutyUnit]=(rd.OtherUnit1/rd.QtyReceived), " +
                      "[ReportType]=COALESCE((SELECT DESCRIPTION FROM CODELKUP WHERE LISTNAME ='DECTYPE' AND CODE=SUBSTRING(r.REFERENCE1,5,1)),'ไม่ระบุประเภทใบขน') " +
                      "FROM " +
                      "Receipt r LEFT JOIN ReceiptDetail rd ON r.Receiptkey=rd.Receiptkey " +
                      "WHERE r.Reference1 NOT LIKE 'CAN%' AND rd.QtyReceived > 0 AND rd.ConditionCode <> 'NOTOK' ";
                      //"AND  rd.ReceiptKey='0000039197' ";
                if (dateType == "0")
                {
                    sql += string.Format(" AND CONVERT(VARCHAR(6),r.ReceiptDate,112) ='{0}{1}' ", year, month);
                }
                else if (dateType == "1")
                {
                    sql += string.Format(" AND r.ReceiptDate BETWEEN '{0}' AND '{1}' ", startDate, stopDate);
                }
                if (declarationType != null)
                {
                    sql += string.Format(" AND SUBSTRING(r.Reference1,5,1) IN ({0})", _helperRepository.GetSQLInCondition(declarationType));
                }
                if (importers != null)
                {
                    sql += string.Format(" AND r.StorerKey IN ({0})", _helperRepository.GetSQLInCondition(importers));
                }
                sql +=  " GROUP BY " +
                        "r.Reference1," +
                        "rd.Reference8," +
                        "r.ReceiptDate," +
                        "r.STORERKEY," +
                        "rd.POLineNumber," +
                        "rd.STORERKEY," +
                        "CAST(rd.DateReceived as DateTime)," +
                        //"rd.ToId, " +
                        "rd.Sku, " +
                        "rd.UnitPrice," +
                        "rd.NetWgt," +
                        "rd.GrossWgt, " +
                        "rd.ExtendedPrice, " +
                        "rd.OtherUnit1, " +
                        "rd.Reference5," +
                        "rd.OtherUnit1/rd.QtyReceived " ;
                sql += " ORDER BY r.Reference1,rd.PoLineNumber ";
                return cn.Query<ImportDeclarationReport>(sql);

            }
            catch (SqlException sqlEx)
            {
                throw sqlEx;
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
        public IEnumerable<ExportDeclarationReport> GetExportDeclaraion(string dateType, string startDate, string stopDate, string year, string month, object[] declarationType, object[] exporters, string sku)
        {
            string connectionString = this.WMSConnectionString();
            var cn = new SqlConnection(connectionString);
            try
            {
                string groupBy = string.Empty;
                string orderBy = string.Empty;
                string sql = string.Empty;
                sql = "SELECT " +
                      //"o.OrderKey," +
                      "[ExportDeclarationNo]=o.ShippingInstructions1," +
                      "[ExportDeclarationItemNo]=od.OrderLineNumber," +
                      "[ExportDeclarationDate]=o.OrderDate," +
                      "[WarehouseOrderdDate]=od.AddDate," +
                      "[ExportName]=(SELECT COMPANY FROM STORER WHERE STORERKEY =o.STORERKEY)," +
                      "[Sku]=od.Sku, " +
                      "[SkuDescription]=(CASE WHEN od.Sku ='NOVC' THEN rd.Reference5 ELSE (SELECT DESCR FROM SKU WHERE STORERKEY=od.StorerKey AND SKU=od.Sku) END)," +
                      "[Quantity]=SUM(pd.Qty),  " +
                      "[UOM]='C62',  " +
                      "[UnitPrice]=rd.UnitPrice," +
                      "[NetWgt]=SUM(pd.Qty)*(rd.NetWgt/SUM(rd.QtyReceived))," +
                      "[GrossWgt]= SUM(pd.Qty)*(rd.GrossWgt/SUM(rd.QtyReceived)), " +
                      "[TotalAmount]=rd.UnitPrice*SUM(pd.Qty), " +
                      "[TotalDuty]= SUM(pd.Qty)*(rd.OtherUnit1/SUM(rd.QtyReceived)), " +
                      "[ImportDeclarationNo]=r.Reference1, " +
                      "[ImportDeclarationItemNo]=rd.POLineNumber, " +
                      "[InvoiceItemNo]=ROW_NUMBER() OVER (PARTITION BY o.ShippingInstructions1 ORDER By od.AddDate), " +
                      "[ReportType]=COALESCE((SELECT DESCRIPTION FROM CODELKUP WHERE LISTNAME ='DECTYPE' AND CODE=SUBSTRING(o.ShippingInstructions1,5,1)),'ไม่ระบุประเภทใบขน') " +
                      "FROM " +
                      "Orders o LEFT JOIN OrderDetail od ON o.OrderKey=od.OrderKey " +
                      "LEFT JOIN PickDetail pd ON od.Orderkey=pd.Orderkey AND od.OrderLineNumber=pd.OrderLineNumber " +
                      "LEFT JOIN ReceiptDetail rd ON pd.ID=rd.ToId " +
                      "LEFT JOIN Receipt r ON rd.ReceiptKey=r.ReceiptKey " +
                      "WHERE o.ShippingInstructions1 NOT LIKE 'CAN%' AND rd.ConditionCode <> 'NOTOK' ";

                if (dateType == "0")
                {
                    sql += string.Format(" AND CONVERT(VARCHAR(6),o.OrderDate,112) ='{0}{1}' ", year, month);
                }
                else if (dateType == "1")
                {
                    sql += string.Format(" AND (CONVERT(VARCHAR,o.OrderDate,112) BETWEEN '{0}' AND '{1}') ", startDate, stopDate);
                }
                if (declarationType != null)
                {
                    sql += string.Format(" AND SUBSTRING(o.ShippingInstructions1,5,1) IN ({0})", _helperRepository.GetSQLInCondition(declarationType));
                }
                if (exporters != null)
                {
                    sql += string.Format(" AND o.StorerKey IN ({0})", _helperRepository.GetSQLInCondition(exporters));
                }

                groupBy = " GROUP BY " +
                    "o.StorerKey," +
                    //"o.OrderKey," +
                    //"pd.PickDetailKey, " +
                    "o.ShippingInstructions1," +
                    "od.OrderLineNumber," +
                    "o.OrderDate," +
                    //"od.LineItemID," +
                    "od.AddDate," +
                    "od.Sku, " +
                    "od.StorerKey," +
                    "rd.UnitPrice," +
                    "rd.NetWgt," +
                    "rd.GrossWgt, " +
                    "rd.OtherUnit1, " +
                    "rd.Reference5," +
                    //"rd.QtyReceived, " +
                    "r.Reference1, " +
                    "rd.POLineNumber ";

                //orderBy = " ORDER BY o.OrderDate ";
                return cn.Query<ExportDeclarationReport>(string.Format("{0} {1} {2}", sql, groupBy, orderBy));

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
        public IEnumerable<GLDeclarationReport> GetStockDeclaration(string dateType, string inventoryDate, string year, string month, object[] declarationType, object[] importers,string sku)
        {
            string connectionString = this.WMSConnectionString();
            var cn = new SqlConnection(connectionString);
            string cutOffDate = string.Empty;
            if (dateType == "0")
                cutOffDate = string.Format("{0}{1}{2}", year, month, DateTime.DaysInMonth(Convert.ToInt16(year), Convert.ToInt16(month)));
            else if (dateType == "1")
                cutOffDate = inventoryDate;
            try
            {
                string groupBy = string.Empty;
                string sql = string.Empty;

                sql = "SELECT " +
                "[ImporterName]=s.Company," +
                "[ImportDeclarationNo]=r.Reference1," +
                "[ImportDeclarationItemNo]=rd.POLineNumber," +
                "[ImportDeclarationDate]=CAST(CONVERT(VARCHAR,r.ReceiptDate,101) AS DATETIME)," +
                "[WarehouseReceivedDate]=CAST(CONVERT(VARCHAR,MIN(rd.DateReceived),101) AS DATETIME)," +
                "[Sku]=rd.Sku, " +
                //"[SkuDescription]=(CASE WHEN rd.Sku='NOVC' THEN rd.Reference5 ELSE (SELECT DESCR FROM SKU WHERE STORERKEY=rd.StorerKey AND SKU=rd.Sku) END)," +
                "[SkuDescription]=rd.Reference5," +
                "[Quantity]=SUM(rd.QtyReceived),  " +
                "[UnitPrice]=rd.UnitPrice,  " +
                "[NetWgt]=SUM((rd.QtyReceived-COALESCE(pd.Qty,0))*(rd.NetWgt/rd.QtyReceived))," +
                "[UOM]=(CASE WHEN rd.ContainerKey='' THEN 'C62' ELSE rd.ContainerKey END), " +
                "[TotalAmount]=SUM(rd.UnitPrice*(rd.QtyReceived-COALESCE(pd.Qty,0))), " +
                "[TotalDuty]=SUM((rd.QtyReceived-COALESCE(pd.Qty,0))*(rd.OtherUnit1/rd.QtyReceived))," +
                "[QtyBalance]=SUM((rd.QtyReceived-COALESCE(pd.Qty,0))) " +
                "FROM " +
                "Receipt r LEFT JOIN ReceiptDetail rd ON r.Receiptkey=rd.Receiptkey " +
                "LEFT JOIN Storer s ON r.StorerKey=s.StorerKey " +
                "LEFT JOIN " +
                "(" +
                string.Format("SELECT pd.Id,[Qty]=SUM(pd.QTY) FROM ORDERS o LEFT JOIN PICKDETAIL pd ON o.ORDERKEY=pd.ORDERKEY WHERE CONVERT(VARCHAR,o.ORDERDATE,112) <='{0}' GROUP BY pd.Id ", cutOffDate) +
                ") pd ON rd.ToId=pd.Id " +
                "WHERE rd.QtyReceived >0 AND r.Reference1 NOT LIKE 'CAN%' AND rd.ConditionCode <> 'NOTOK' ";

                sql += string.Format(" AND CONVERT(VARCHAR,r.ReceiptDate,112) <='{0}' ", cutOffDate);

                if (declarationType != null)
                {
                    sql += string.Format(" AND SUBSTRING(r.Reference1,5,1) IN ({0})", _helperRepository.GetSQLInCondition(declarationType));
                }
                if (importers != null)
                {
                    sql += string.Format(" AND r.StorerKey IN ({0})", _helperRepository.GetSQLInCondition(importers));
                }
                if (sku != null)
                {
                    sql += string.Format(" AND rd.Sku ='{0}' ", sku);
                }
                groupBy = "GROUP BY " +
                "r.Reference1," +
                //"rd.POLineNumber," +
                "rd.StorerKey," +
                "CAST(CONVERT(VARCHAR,r.ReceiptDate,101) AS DATETIME)," +
                "rd.Sku, " +
                "rd.ContainerKey,"+
                "rd.Reference5," +
                "rd.UnitPrice," +
                "rd.POLineNumber," +
                "s.Company  " +
                "HAVING SUM((rd.QtyReceived-COALESCE(pd.Qty,0))) >0 " +
                "ORDER BY CAST(CONVERT(VARCHAR,r.ReceiptDate,101) AS DATETIME),r.Reference1," +
                "rd.POLineNumber ";

                return cn.Query<GLDeclarationReport>(string.Format("{0} {1}", sql, groupBy));

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
        public IEnumerable<GLDeclarationReport> GetLedgerDeclaration(string dateType, string inventoryDate, string year, string month, object[] declarationType, object[] importers)
        {
            string connectionString = this.WMSConnectionString();
            var cn = new SqlConnection(connectionString);
            string cutOffDate = string.Empty;
            if (dateType == "0")
                cutOffDate = string.Format("{0}{1}{2}", year, month, DateTime.DaysInMonth(Convert.ToInt16(year), Convert.ToInt16(month)));
            else if (dateType == "1")
                cutOffDate = inventoryDate;
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
                "[SkuDescription]=(CASE WHEN rd.Sku='NOVC' THEN rd.Reference5 ELSE (SELECT DESCR FROM SKU WHERE STORERKEY=rd.StorerKey AND SKU=rd.Sku) END)," +
                "[Quantity]=rd.QtyReceived,  " +
                "[NetWgt]=(rd.NetWgt/rd.QtyReceived)," +
                "[UOM]=rd.ContainerKey, " +
                "[ExportDeclarationNo]=pd.ShippingInstructions1," +
                "[ExportDeclarationDate]=pd.OrderDate," +
                "[QtyPicked]=pd.Qty, " +
                "[QtyBalance]=(rd.QtyReceived-COALESCE(pd.Qty,0)) " +
                "FROM " +
                "Receipt r LEFT JOIN ReceiptDetail rd ON r.Receiptkey=rd.Receiptkey " +
                "LEFT JOIN Storer s ON r.StorerKey=s.StorerKey " +
                "LEFT JOIN " +
                "(" +
                string.Format("SELECT o.OrderKey,o.OrderDate,o.ShippingInstructions1,pd.Id,[Qty]=SUM(pd.QTY) FROM ORDERS o LEFT JOIN PICKDETAIL pd ON o.ORDERKEY=pd.ORDERKEY WHERE CONVERT(VARCHAR,o.ORDERDATE,112) <='{0}' " +
                    "GROUP BY o.OrderKey,o.OrderDate,o.ShippingInstructions1,pd.Id ", cutOffDate) +
                ") pd ON rd.ToId=pd.Id " +
                "WHERE rd.QtyReceived >0 AND rd.ConditionCode <> 'NOTOK'  ";

                sql += string.Format(" AND CONVERT(VARCHAR,r.ReceiptDate,112) <='{0}' ", cutOffDate);

                if (declarationType != null)
                {
                    sql += string.Format(" AND SUBSTRING(r.Reference1,5,1) IN ({0})", _helperRepository.GetSQLInCondition(declarationType));
                }
                if (importers != null)
                {
                    sql += string.Format(" AND r.StorerKey IN ({0})", _helperRepository.GetSQLInCondition(importers));
                }

                groupBy = "GROUP BY " +
                "r.ReceiptKey," +
                "r.Reference1," +
                "rd.POLineNumber," +
                "rd.ToId," +
                "rd.StorerKey," +
                "rd.ContainerKey," +
                "r.ReceiptDate," +
                "rd.Sku, " +
                "rd.QtyReceived,  " +
                "rd.Reference5," +
                "rd.DateReceived,  " +
                "rd.NetWgt," +
                "s.Company, " +
                "pd.ShippingInstructions1, " +
                "pd.OrderDate, " +
                "pd.Qty " +
                "ORDER BY r.ReceiptDate,r.ReceiptKey,rd.POLineNumber ";

                return cn.Query<GLDeclarationReport>(string.Format("{0} {1}", sql, groupBy));

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
            string connectionString = this.WMSConnectionString();
            var cn = new SqlConnection(connectionString);
            try
            {
                string groupBy = string.Empty;
                string sql = string.Empty;

                sql = "SELECT " +
                    //"r.ReceiptKey, " +
                    "r.ReceiptType, " +
                    "[ImportDeclarationNo]=r.Reference1, " +
                    "[ImportDeclarationItemNo]=rd.POLineNumber, " +
                    "[ImportDeclarationDate]=r.ReceiptDate, " +
                    "[ImporterName]=s.Company," +
                    "[WarehouseReceivedDate]=MIN(rd.DateReceived), " +
                    "[Sku]=MIN(l.Sku),  " +
                    "[SkuDescription]=(CASE WHEN rd.Sku='NOVC' THEN rd.Reference5 ELSE (SELECT DESCR FROM SKU WHERE STORERKEY=l.StorerKey AND SKU=l.Sku) END), " +
                    "[Quantity]=SUM(l.Qty-(l.QtyPicked+l.QtyAllocated))," +
                    "[NetWgt]=SUM((l.Qty-(l.QtyPicked+l.QtyAllocated))*(rd.NetWgt/rd.QtyReceived)) ," +
                    "[UnitPrice]=rd.UnitPrice ," +
                    "[CustomsTax]=rd.OtherUnit1 " +
                    "FROM " +
                    "LotxlocxId l LEFT JOIN ReceiptDetail rd ON l.id=rd.ToId " +
                    "LEFT JOIN Storer s ON l.StorerKey=s.StorerKey " +
                    "LEFT JOIN Receipt r ON rd.Receiptkey=r.Receiptkey " +
                    "WHERE " +
                    "(l.Qty-(l.QtyPicked+l.QtyAllocated)) > 0 ";

                groupBy = "GROUP BY " +
                //"r.ReceiptKey, " +
                "r.ReceiptType, " +
                "r.Reference1, " +
                "rd.POLineNumber, " +
                "r.ReceiptDate," +
                //"rd.QtyReceived," +
                //"rd.DateReceived," +
                //"rd.NetWgt," +
                "rd.UnitPrice, " +
                "rd.Reference5," +
                "l.StorerKey," +
                //"l.Sku," +
                //"rd.Sku," +
                "rd.OtherUnit1," +
                "s.Company " +
                "ORDER BY r.ReceiptDate,r.Reference1,rd.POLineNumber ";


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
            string connectionString = this.WMSConnectionString();
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
                "[SkuDescription]=(CASE WHEN rd.Sku='NOVC' THEN rd.Reference5 ELSE (SELECT DESCR FROM SKU WHERE STORERKEY=rd.StorerKey AND SKU=rd.Sku) END), " +
                "[Quantity]=rd.QtyReceived, " +
                "[UnitPrice]=rd.UnitPrice, " +
                "[DutyAmount]=rd.OtherUnit1, " +
                //"[QtyPicked]=COALESCE((SELECT SUM(Qty) FROM PickDetail pd LEFT JOIN ORDERS o ON pd.OrderKey=o.OrderKey WHERE pd.Id=rd.ToId AND CONVERT(VARCHAR,o.OrderDate,112) <= @PreviousDate),0), " +
                "[BroughtForward]=rd.QtyReceived-(COALESCE((SELECT SUM(Qty) FROM PickDetail pd LEFT JOIN ORDERS o ON pd.OrderKey=o.OrderKey WHERE pd.Id=rd.ToId AND CONVERT(VARCHAR,o.OrderDate,112) <= @PreviousDate),0)) " +
                "FROM " +
                "Receipt r LEFT JOIN ReceiptDetail rd ON r.Receiptkey=rd.Receiptkey  " +
                "LEFT JOIN Storer s ON r.StorerKey=s.StorerKey  " +
                "WHERE CONVERT(VARCHAR,r.ReceiptDate,112) <= @PreviousDate " +
                "AND r.Reference1 NOT LIKE 'CAN%' AND rd.QtyReceived > 0 AND rd.ConditionCode <> 'NOTOK'  ";

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
                "rd.Reference5," +
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

                //movementReport.GLDeclarationItems = GetGLDeclaration(startDate, stopDate, customerCode);

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
        public InventoryMovementReport GetMovementDeclaration(string dateType, string startDate, string stopDate, string year, string month, object[] declarationType, object[] importers,string sku)
        {
            string previousDate =string.Empty;
            string sqlWhere = string.Empty;

            if (dateType == "0")
            {
                previousDate = Convert.ToDateTime(string.Format("{0}-{1}-01", year, month)).AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
                startDate = string.Format("{0}{1}{2}", year, month, "01");
                stopDate =string.Format("{0}{1}{2}", year, month, DateTime.DaysInMonth(Convert.ToInt16(year), Convert.ToInt16(month)));
            }
            else if (dateType == "1")
            { 
                previousDate = Convert.ToDateTime(startDate).AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
            }
            

            InventoryMovementReport movementReport = new InventoryMovementReport();
            string connectionString = this.WMSConnectionString();
            var cn = new SqlConnection(connectionString);

            try
            {
                string groupBy = string.Empty;
                string sql = string.Empty;

                sql = "SELECT " +
                "pd.Id," +
                "[ExportDeclarationNo]=o.ShippingInstructions1," +
                "[ExportDeclarationItemNo]=od.OrderLineNumber," +
                "[ReportType]=COALESCE((SELECT DESCRIPTION FROM CODELKUP WHERE LISTNAME ='DECTYPE' AND CODE=SUBSTRING(o.ShippingInstructions1,5,1)),'ไม่ระบุประเภทใบขน'), " +
                "[Quantity]=SUM(pd.Qty)," +
                "[UnitPrice]=rd.UnitPrice, " +
                "[ImportDeclarationNo]=r.Reference1, " +
                "[ImportDeclarationItemNo]=rd.POLineNumber, " +
                //"[ExportDutyUnit]=(SELECT OtherUnit1/QtyReceived FROM ReceiptDetail WHERE toId=pd.Id)," +
                "[InvoiceItemNo]=0 " + //ROW_NUMBER() OVER (PARTITION BY o.OrderKey ORDER By o.OrderKey, pd.PickDetailKey) " +
                "FROM " +
                "Orders o LEFT JOIN OrderDetail od ON o.OrderKey=od.OrderKey " +
                "LEFT JOIN PickDetail pd ON od.OrderKey=pd.OrderKey AND od.OrderLineNumber=pd.OrderLineNumber " +
                "LEFT JOIN ReceiptDetail rd ON pd.ID=rd.ToId " +
                "LEFT JOIN Receipt r ON rd.ReceiptKey=r.ReceiptKey " +
                "WHERE rd.ConditionCode <> 'NOTOK' AND o.ShippingInstructions1 NOT LIKE 'CAN%' AND pd.Qty > 0  ";
                //"AND rd.ReceiptKey IN ('0000000735','0000000736') ";
                if (dateType == "0")
                {
                    sqlWhere += string.Format(" AND CONVERT(VARCHAR(6),o.OrderDate,112) ='{0}{1}' ", year, month);
                }
                else if (dateType == "1")
                {
                    sqlWhere += string.Format(" AND (CONVERT(VARCHAR,o.OrderDate,112) BETWEEN '{0}' AND '{1}') ", startDate, stopDate);
                }
                if (importers != null)
                {
                    sqlWhere += string.Format(" AND o.StorerKey IN ({0})", _helperRepository.GetSQLInCondition(importers));
                }
                if (sku != null)
                {
                    sqlWhere += string.Format(" AND rd.SKU = '{0}' ", sku);
                }
                sql += sqlWhere;
                groupBy = "GROUP BY " +
                "rd.UnitPrice," +
                "r.Reference1," +
                "rd.POLineNumber," +
                "o.OrderKey," +
                "pd.Id," +
                "pd.PickDetailKey," +
                "o.ShippingInstructions1," +
                "od.OrderLineNumber " +
                "ORDER BY o.OrderKey,od.OrderLineNumber";

                movementReport.BroughtForwardItems = GetStockDeclaration ("1", previousDate, year, month, declarationType, importers,sku);
                movementReport.ImportDeclarationItems = GetImportDeclaraionFreeZone("1", startDate, stopDate, year, month, declarationType, importers,true,sku);
                movementReport.ExportDeclarationItems = cn.Query<ExportDeclarationReport>(string.Format("{0} {1}", sql, groupBy));
                

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
        public IEnumerable<ExportDeclarationReport> GetExportFreeZoneDeclaraion(string dateType, string startDate, string stopDate, string year, string month, object[] declarationType, object[] exporters,string sku)
        {
            string connectionString = this.WMSConnectionString();
            var cn = new SqlConnection(connectionString);
            try
            {
                string groupBy = string.Empty;
                string orderBy = string.Empty;
                string sql = string.Empty;
                sql = "SELECT " +
                      "[OrderKey]=o.OrderKey," +
                      "[ExportDeclarationNo]=RTRIM(LTRIM(o.ShippingInstructions1))," +
                      "[ExportInvoiceNo]=RTRIM(LTRIM(o.ExternOrderKey))," +
                      "[ExportDeclarationItemNo]=od.OrderLineNumber," +
                      "[ExportDeclarationDate]=o.OrderDate," +
                      "[WarehouseOrderdDate]=od.AddDate," +
                      "[ExportName]=(SELECT COMPANY FROM STORER WHERE STORERKEY =o.STORERKEY)," +
                      "[Sku]=od.Sku, " +
                      "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=od.StorerKey AND SKU=od.Sku)," +
                      "[AltSkuDescription]=od.Notes2," +
                      "[Quantity]=SUM(pd.Qty),  " +
                      "[UOM]=(CASE WHEN rd.ContainerKey='' THEN 'PCS' ELSE rd.ContainerKey END),  " +
                      "[UnitPrice]=od.UnitPrice," +
                      "[NetWgt]=SUM(CASE WHEN od.NetWeight =0 THEN (pd.Qty)*(rd.NetWgt/(rd.QtyReceived)) ELSE (pd.Qty)*(od.NetWeight/(od.OpenQty+od.ShippedQty)) END)," +
                      "[GrossWgt]= SUM(CASE WHEN od.GrossWeight=0 THEN 0 ELSE (pd.Qty)*(od.GrossWeight/(od.OpenQty+od.ShippedQty)) END), " +
                      "[TotalAmount]=(CASE WHEN od.ExtendedPrice=0 THEN 0 ELSE SUM(pd.Qty)*(od.ExtendedPrice/(od.OpenQty+od.ShippedQty)) END), " +
                      "[TotalDuty]= 0, " +
                      "[ImportDeclarationNo]=RTRIM(LTRIM(r.Reference1)), " +
                      "[ImportDeclarationItemNo]=rd.POLineNumber, " +
                      "[InvoiceItemNo]=0, " +
                      "[ReportType]=COALESCE((SELECT DESCRIPTION FROM CODELKUP WHERE LISTNAME ='DECTYPE' AND CODE=SUBSTRING(o.ShippingInstructions1,5,1)),'ไม่ระบุประเภทใบขน') " +
                      "FROM " +
                      "Orders o LEFT JOIN OrderDetail od ON o.OrderKey=od.OrderKey " +
                      "LEFT JOIN PickDetail pd ON od.Orderkey=pd.Orderkey AND od.OrderLineNumber=pd.OrderLineNumber " +
                      "LEFT JOIN ReceiptDetail rd ON pd.ID=rd.ToId " +
                      "LEFT JOIN Receipt r ON rd.ReceiptKey=r.ReceiptKey " +
                      "WHERE rd.ConditionCode <> 'NOTOK' AND o.ShippingInstructions1 NOT LIKE 'CAN%' ";
                      //"AND o.ShippingInstructions1='A0151591102838' ";

                if (dateType == "0")
                {
                    sql += string.Format(" AND CONVERT(VARCHAR(6),o.OrderDate,112) ='{0}{1}' ", year, month);
                }
                else if (dateType == "1")
                {
                    sql += string.Format(" AND o.OrderDate BETWEEN '{0}' AND '{1}' ", startDate, stopDate);
                }
                if (declarationType != null)
                {
                    sql += string.Format(" AND SUBSTRING(o.ShippingInstructions1,5,1) IN ({0})", _helperRepository.GetSQLInCondition(declarationType));
                }
                if (exporters != null)
                {
                    sql += string.Format(" AND o.StorerKey IN ({0})", _helperRepository.GetSQLInCondition(exporters));
                }
                if (sku != null)
                {
                    sql += string.Format(" AND od.Sku = '{0}' ", sku);
                }
                groupBy = " GROUP BY " +
                    "o.StorerKey," +
                    "o.ShippingInstructions1," +
                    "o.ExternOrderKey," +
                    "o.OrderKey," +
                    "od.OrderLineNumber," +
                    "o.OrderDate," +
                    "od.AddDate," +
                    "od.Sku, " +
                    "od.StorerKey," +
                    "od.OpenQty," +
                    "od.UnitPrice," +
                    "od.Notes2," +
                    "od.NetWeight," +
                    "od.GrossWeight," +
                    "od.ShippedQty, " +
                    "od.ExtendedPrice," +
                    "rd.UnitPrice," +
                    "rd.ContainerKey,"+
                    "rd.NetWgt," +
                    "rd.GrossWgt, " +
                    "rd.OtherUnit1, " +
                    //"rd.QtyReceived," +
                    "r.Reference1, " +
                    "rd.POLineNumber ";

                orderBy = " ORDER BY  o.OrderDate,o.ShippingInstructions1,od.OrderLineNumber,r.Reference1 ";
                return cn.Query<ExportDeclarationReport>(string.Format("{0} {1} {2}", sql, groupBy, orderBy));

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
        public IEnumerable<ExportDeclarationReport> GetExportFreeZoneDeclaraionNew(string dateType, string startDate, string stopDate, string year, string month, object[] declarationType, object[] exporters, string sku)
        {
            string connectionString = this.WMSConnectionString();
            var cn = new SqlConnection(connectionString);
            try
            {
                string groupBy = string.Empty;
                string orderBy = string.Empty;
                string sql = string.Empty;
                sql = "SELECT " +
                      "[OrderKey]=''," +
                      "[GoodsLoadedDate]=o.Date2," +
                      "[ExportDeclarationNo]=RTRIM(LTRIM(o.ShippingInstructions1))," +
                      "[ExportInvoiceNo]=RTRIM(LTRIM(od.Notes2))," +
                      "[ExportDeclarationItemNo]=''," + 
                      "[ExportDeclarationDate]=o.OrderDate," +
                      "[WarehouseOrderdDate]=MIN(od.AddDate)," +
                      "[ExportName]=(SELECT COMPANY FROM STORER WHERE STORERKEY =o.STORERKEY)," +
                      "[Sku]=od.Sku, " +
                      "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=od.StorerKey AND SKU=od.Sku)," +
                      "[AltSkuDescription]=MAX(od.Notes2)," +
                      "[Quantity]=SUM(pd.Qty),  " +
                      "[UOM]=(CASE WHEN MIN(rd.ContainerKey)='' THEN 'ชิ้น' ELSE MIN(rd.ContainerKey) END),  " +
                      "[UnitPrice]=MIN(od.UnitPrice)," +
                      "[NetWgt]=SUM(CASE WHEN od.NetWeight =0 THEN (pd.Qty)*(rd.NetWgt/(rd.QtyReceived)) ELSE (pd.Qty)*(od.NetWeight/(od.OpenQty+od.ShippedQty)) END)," +
                      "[GrossWgt]= SUM(CASE WHEN od.GrossWeight=0 THEN (pd.Qty)*(rd.GrossWgt/(rd.QtyReceived))  ELSE (pd.Qty)*(od.GrossWeight/(od.OpenQty+od.ShippedQty)) END), " +
                      //"[TotalAmount]=SUM(pd.Qty)*SUM(CASE WHEN od.ExtendedPrice=0 THEN (rd.ExtendedPrice/rd.QtyReceived) ELSE (od.ExtendedPrice/(od.OpenQty+od.ShippedQty)) END), " +
                      "[TotalAmount] = (SUM(pd.Qty) * MIN(od.UnitPrice))," +
                      "[TotalDuty]= 0, " +
                      "[ReportType]=COALESCE((SELECT DESCRIPTION FROM CODELKUP WHERE LISTNAME ='DECTYPE' AND CODE=SUBSTRING(o.ShippingInstructions1,5,1)),'ไม่ระบุประเภทใบขน') " +
                      "FROM " +
                      "Orders o LEFT JOIN OrderDetail od ON o.OrderKey=od.OrderKey " +
                      "LEFT JOIN PickDetail pd ON od.Orderkey=pd.Orderkey AND od.OrderLineNumber=pd.OrderLineNumber " +
                      "LEFT JOIN " +
                      "(" +
                         "SELECT " +
                         "SKU," +
                         "STORERKEY," +
                         "TOID," +
                         "NetWgt," +
                         "GrossWgt," +
                         "ContainerKey," +
                         "QtyReceived " +
                         "FROM RECEIPTDETAIL " +
                         string.Format("WHERE ConditionCode <> 'NOTOK' AND StorerKey IN ({0}) ", _helperRepository.GetSQLInCondition(exporters)) +
                         "GROUP BY " +
                         "SKU," +
                         "STORERKEY," +
                         "TOID," +
                         "NetWgt," +
                         "GrossWgt," +
                         "ContainerKey," +
                         "QtyReceived" +
                      ") rd ON pd.ID=rd.ToId AND (pd.Storerkey=rd.StorerKey AND pd.Sku=rd.Sku) " +
                      "WHERE o.ShippingInstructions1 NOT LIKE 'CAN%' ";
              
                if (dateType == "0")
                {
                    sql += string.Format(" AND CONVERT(VARCHAR(6),o.OrderDate,112) ='{0}{1}' ", year, month);
                }
                else if (dateType == "1")
                {
                    sql += string.Format(" AND o.OrderDate BETWEEN '{0}' AND '{1}' ", startDate, stopDate);
                }
                if (declarationType != null)
                {
                    sql += string.Format(" AND SUBSTRING(o.ShippingInstructions1,5,1) IN ({0})", _helperRepository.GetSQLInCondition(declarationType));
                }
                if (exporters != null)
                {
                    sql += string.Format(" AND o.StorerKey IN ({0})", _helperRepository.GetSQLInCondition(exporters));
                }
                if (sku != null)
                {
                    sql += string.Format(" AND od.Sku = '{0}' ", sku);
                }
                groupBy = " GROUP BY " +
                    "o.StorerKey," +
                    "o.ShippingInstructions1," +
                    "od.Notes2," +
                    "o.OrderDate," +
                    "o.Date2," +
                    "od.Sku, " +
                    "od.StorerKey"; 

                orderBy = " ORDER BY  o.ShippingInstructions1,RTRIM(LTRIM(od.Notes2)) ";
                return cn.Query<ExportDeclarationReport>(string.Format("{0} {1} {2}", sql, groupBy, orderBy));

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
        public IEnumerable<ImportDeclarationReport> GetImportDeclaraionFreeZone(string dateType, string startDate, string stopDate, string year, string month, object[] declarationType, object[] importers ,bool isSkipRemark,string sku)
        {
            string connectionString = this.WMSConnectionString();
            var cn = new SqlConnection(connectionString);
            try
            {
                string sql = string.Empty;
                sql = "SELECT " +
                    //"r.ReceiptKey," +
                    //"[Id]=rd.ToId, " +
                      "[ImportDeclarationNo]=LTRIM(RTRIM(r.Reference1))," +
                      "[ImporterName]=(SELECT COMPANY FROM STORER WHERE STORERKEY =r.STORERKEY)," +
                      "[ImportDeclarationItemNo]=rd.POLineNumber," +
                      "[CustomsPermitDate]=r.Date1," +
                      "[ImportDeclarationDate]=CAST(CONVERT(VARCHAR,r.ReceiptDate,101) AS DATETIME)," +
                    //"[WarehouseReceivedDate]=r.ReceiptDate," +
                      "[Sku]=rd.Sku, " +
                      "[SkuDescription]=(CASE WHEN rd.Reference5='' THEN (SELECT DESCR FROM SKU WHERE STORERKEY=rd.StorerKey AND SKU=rd.Sku) ELSE rd.Reference5 END)," +
                      "[Quantity]=SUM(rd.QtyReceived),  " +
                      "[UnitPrice]=MAX(rd.UnitPrice)," +
                      "[NetWgt]=MAX(rd.NetWgt)," +
                      "[UOM]=(CASE WHEN rd.ContainerKey='' THEN 'C62' ELSE rd.ContainerKey END)," +
                      "[GrossWgt]=SUM(rd.GrossWgt), " +
                      "[Invoice]=rd.Reference8," +
                      "[Taxincentive]=rd.Reference9," +
                      "[Remark]=" + (isSkipRemark == true ? "'', " : "rd.Reference14, ") +
                      "[TotalAmount]=MAX(rd.ExtendedPrice), " +
                      "[TotalDuty]=rd.OtherUnit1, " +
                      "[DutyUnit]=(rd.OtherUnit1/rd.QtyReceived), " +
                      "[ReportType]=COALESCE((SELECT DESCRIPTION FROM CODELKUP WHERE LISTNAME ='DECTYPE' AND CODE=SUBSTRING(r.REFERENCE1,5,1)),'ไม่ระบุประเภทใบขน') " +
                      "FROM " +
                      "Receipt r LEFT JOIN ReceiptDetail rd ON r.Receiptkey=rd.Receiptkey " +
                      "LEFT JOIN Storer s ON r.StorerKey=s.StorerKey " +
                      "WHERE rd.QtyReceived > 0 AND r.Reference1 NOT LIKE 'CAN%' AND rd.ConditionCode <> 'NOTOK'  ";
                      //"AND rd.ReceiptKey IN ('0000000735','0000000736') ";
                      //"AND  rd.ReceiptKey='0000039197' ";
                if (dateType == "0")
                {
                    sql += string.Format(" AND CONVERT(VARCHAR(6),r.ReceiptDate,112) ='{0}{1}' ", year, month);
                }
                else if (dateType == "1")
                {
                    sql += string.Format(" AND r.ReceiptDate BETWEEN '{0}' AND '{1}' ", startDate, stopDate);
                }
                if (declarationType != null)
                {
                    sql += string.Format(" AND SUBSTRING(r.Reference1,5,1) IN ({0})", _helperRepository.GetSQLInCondition(declarationType));
                }
                if (importers != null)
                {
                    sql += string.Format(" AND r.StorerKey IN ({0})", _helperRepository.GetSQLInCondition(importers));
                }
                if (sku != null)
                {
                    sql += string.Format(" AND rd.Sku = '{0}' ", sku);
                }
                sql += " GROUP BY " +
                        "CAST(CONVERT(VARCHAR,r.ReceiptDate,101) AS DATETIME)," +
                        "r.Reference1," +
                        "r.STORERKEY," +
                        "r.Date1," +
                        "s.LocationCategory," +
                        "rd.POLineNumber," +
                        "rd.STORERKEY," +
                        "rd.Sku, " +
                        //"rd.UnitPrice," +
                        "rd.Reference5," +
                        "rd.Reference8," +
                        "rd.Reference9,";
                        if (!isSkipRemark)
                            sql += "rd.Reference14,";
                        sql += "rd.ContainerKey," +
                        //"rd.NetWgt," +
                        //"rd.GrossWgt, " +
                        //"rd.ExtendedPrice, " +
                        "rd.OtherUnit1, " +
                        "rd.OtherUnit1/rd.QtyReceived ";
                        sql += " ORDER BY CAST(CONVERT(VARCHAR,r.ReceiptDate,101) AS DATETIME),r.Reference1,rd.POLineNumber ";
                return cn.Query<ImportDeclarationReport>(sql);

            }
            catch (SqlException sqlEx)
            {
                throw sqlEx;
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
        public IEnumerable<ImportDeclarationReport> GetImportDeclaraionIncludeId(string dateType, string startDate, string stopDate, string year, string month, object[] declarationType, object[] importers)
        {
            string connectionString = this.WMSConnectionString();
            var cn = new SqlConnection(connectionString);
            try
            {
                string sql = string.Empty;
                sql = "SELECT " +
                    //"r.ReceiptKey," +
                      "[Id]=rd.ToId, " +
                      "[ImportDeclarationNo]=r.Reference1," +
                      "[ImporterName]=(SELECT COMPANY FROM STORER WHERE STORERKEY =r.STORERKEY)," +
                      "[ImportDeclarationItemNo]=rd.POLineNumber," +
                      "[ImportDeclarationDate]=CAST(CONVERT(VARCHAR,r.ReceiptDate,101) AS DATETIME)," +
                      //"[WarehouseReceivedDate]=CAST(rd.DateReceived as DateTime)," +
                      "[Sku]=rd.Sku, " +
                      "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=rd.StorerKey AND SKU=rd.Sku)," +
                      "[Quantity]=SUM(rd.QtyReceived),  " +
                      "[UnitPrice]=rd.UnitPrice," +
                      "[NetWgt]=rd.NetWgt," +
                      "[UOM]='C62'," +
                      "[GrossWgt]=rd.GrossWgt, " +
                      "[TotalAmount]=rd.ExtendedPrice, " +
                      "[TotalDuty]=rd.OtherUnit1, " +
                      "[DutyUnit]=(rd.OtherUnit1/rd.QtyReceived), " +
                      "[ReportType]=COALESCE((SELECT DESCRIPTION FROM CODELKUP WHERE LISTNAME ='DECTYPE' AND CODE=SUBSTRING(r.REFERENCE1,5,1)),'ไม่ระบุประเภทใบขน') " +
                      "FROM " +
                      "Receipt r LEFT JOIN ReceiptDetail rd ON r.Receiptkey=rd.Receiptkey " +
                      "WHERE r.Reference1 NOT LIKE 'CAN%' AND rd.QtyReceived > 0 AND rd.ConditionCode <> 'NOTOK' ";
                      //"AND  rd.ReceiptKey='0000039197' ";
                if (dateType == "0")
                {
                    sql += string.Format(" AND CONVERT(VARCHAR(6),r.ReceiptDate,112) ='{0}{1}' ", year, month);
                }
                else if (dateType == "1")
                {
                    sql += string.Format(" AND r.ReceiptDate BETWEEN '{0}' AND '{1}' ", startDate, stopDate);
                }
                if (declarationType != null)
                {
                    sql += string.Format(" AND SUBSTRING(r.Reference1,5,1) IN ({0})", _helperRepository.GetSQLInCondition(declarationType));
                }
                if (importers != null)
                {
                    sql += string.Format(" AND r.StorerKey IN ({0})", _helperRepository.GetSQLInCondition(importers));
                }
                sql += " GROUP BY " +
                        "r.Reference1," +
                        "CAST(CONVERT(VARCHAR,r.ReceiptDate,101) AS DATETIME)," +
                        "r.STORERKEY," +
                        "rd.POLineNumber," +
                        "rd.STORERKEY," +
                        "CAST(rd.DateReceived as DateTime)," +
                        "rd.ToId, " +
                        "rd.Sku, " +
                        "rd.UnitPrice," +
                        "rd.NetWgt," +
                        "rd.GrossWgt, " +
                        "rd.ExtendedPrice, " +
                        "rd.OtherUnit1, " +
                        "rd.OtherUnit1/rd.QtyReceived ";
                sql += " ORDER BY r.Reference1,rd.PoLineNumber ";
                return cn.Query<ImportDeclarationReport>(sql);

            }
            catch (SqlException sqlEx)
            {
                throw sqlEx;
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
        public InventoryMovementReport GetLedgerDeclarationFreezone(string dateType, string inventoryDate, string year, string month, object[] declarationType, object[] importers)
        {
            string orderBy = string.Empty;
            string groupBy = string.Empty;
            string cutOffDate = string.Empty;
            if (dateType == "0")
                cutOffDate = string.Format("{0}{1}{2}", year, month, DateTime.DaysInMonth(Convert.ToInt16(year), Convert.ToInt16(month)));
            else if (dateType == "1")
                cutOffDate = inventoryDate;

            InventoryMovementReport movementReport = new InventoryMovementReport();
            string connectionString = this.WMSConnectionString();
            var cn = new SqlConnection(connectionString);

            try
            {
                string sql = string.Empty;
                sql = "SELECT " +
                      "[ImportDeclarationNo]=r.Reference1," +
                      "[ImporterName]=(SELECT COMPANY FROM STORER WHERE STORERKEY =r.STORERKEY)," +
                      "[ImportDeclarationItemNo]=rd.POLineNumber," +
                      "[ImportDeclarationDate]=CAST(CONVERT(VARCHAR,r.ReceiptDate,101) AS DATETIME)," +
                      "[Sku]=rd.Sku, " +
                      "[SkuDescription]=(CASE WHEN rd.Reference5='' THEN (SELECT DESCR FROM SKU WHERE STORERKEY=rd.StorerKey AND SKU=rd.Sku) ELSE rd.Reference5 END)," +
                      "[Quantity]=SUM(rd.QtyReceived),  " +
                      "[UnitPrice]=MAX(rd.UnitPrice)," +
                      "[NetWgt]=SUM(rd.NetWgt)," +
                      "[UOM]=(CASE WHEN rd.ContainerKey='' THEN 'C62' ELSE rd.ContainerKey END)," +
                      "[GrossWgt]=SUM(rd.GrossWgt), " +
                      "[Invoice]=rd.Reference8," +
                      "[Taxincentive]=rd.Reference9," +
                      "[Remark]=''," + //rd.Reference14," +
                      "[TotalAmount]=rd.ExtendedPrice, " +
                      "[TotalDuty]=rd.OtherUnit1, " +
                      "[DutyUnit]=(rd.OtherUnit1/rd.QtyReceived), " +
                      "[ReportType]=COALESCE((SELECT DESCRIPTION FROM CODELKUP WHERE LISTNAME ='DECTYPE' AND CODE=SUBSTRING(r.REFERENCE1,5,1)),'ไม่ระบุประเภทใบขน') " +
                      "FROM " +
                      "Receipt r LEFT JOIN ReceiptDetail rd ON r.Receiptkey=rd.Receiptkey " +
                      "LEFT JOIN Storer s ON r.StorerKey=s.StorerKey " +
                      "WHERE rd.QtyReceived > 0 AND r.Reference1 NOT LIKE 'CAN%' AND rd.ConditionCode <> 'NOTOK' ";
                       sql += string.Format(" AND CONVERT(VARCHAR(6),r.ReceiptDate,112) <='{0}' ", cutOffDate);
              
                if (declarationType != null)
                {
                    sql += string.Format(" AND SUBSTRING(r.Reference1,5,1) IN ({0})", _helperRepository.GetSQLInCondition(declarationType));
                }
                if (importers != null)
                {
                    sql += string.Format(" AND r.StorerKey IN ({0})", _helperRepository.GetSQLInCondition(importers));
                }
                sql += " GROUP BY " +
                        "CAST(CONVERT(VARCHAR,r.ReceiptDate,101) AS DATETIME)," +
                        "r.Reference1," +
                        "r.STORERKEY," +
                        "s.LocationCategory," +
                        "rd.POLineNumber," +
                        "rd.STORERKEY," +
                        "rd.Sku, " +
                        //"rd.UnitPrice," +
                        "rd.Reference5," +
                        "rd.Reference8," +
                        "rd.Reference9," +
                        //"rd.Reference14," +
                        "rd.ContainerKey," +
                        //"rd.NetWgt," +
                        //"rd.GrossWgt, " +
                        "rd.ExtendedPrice, " +
                        "rd.OtherUnit1, " +
                        "rd.OtherUnit1/rd.QtyReceived ";
                sql += " ORDER BY CAST(CONVERT(VARCHAR,r.ReceiptDate,101) AS DATETIME),r.Reference1,rd.POLineNumber ";

                movementReport.ImportDeclarationItems = cn.Query<ImportDeclarationReport>(sql);


                sql = "SELECT " +
                      "[ExportDeclarationNo]=o.ShippingInstructions1," +
                      "[ExportDeclarationDate]=o.OrderDate," +
                      "[Sku]=od.Sku, " +
                      "[Quantity]=SUM(pd.Qty),  " +
                      "[ImportDeclarationNo]=r.Reference1, " +
                      "[ImportDeclarationItemNo]=rd.POLineNumber " +
                      "FROM " +
                      "Orders o LEFT JOIN OrderDetail od ON o.OrderKey=od.OrderKey " +
                      "LEFT JOIN PickDetail pd ON od.Orderkey=pd.Orderkey AND od.OrderLineNumber=pd.OrderLineNumber " +
                      "LEFT JOIN ReceiptDetail rd ON pd.ID=rd.ToId " +
                      "LEFT JOIN Receipt r ON rd.ReceiptKey=r.ReceiptKey " +
                      "WHERE rd.ConditionCode <> 'NOTOK' AND o.ShippingInstructions1 NOT LIKE 'CAN%' ";
                sql += string.Format(" AND CONVERT(VARCHAR(6),o.OrderDate,112) <='{0}' ", cutOffDate);
               
                if (declarationType != null)
                {
                    sql += string.Format(" AND SUBSTRING(o.ShippingInstructions1,5,1) IN ({0})", _helperRepository.GetSQLInCondition(declarationType));
                }
                if (importers != null)
                {
                    sql += string.Format(" AND o.StorerKey IN ({0})", _helperRepository.GetSQLInCondition(importers));
                }

                groupBy = " GROUP BY " +
                    "o.ShippingInstructions1," +
                    "o.OrderDate," +
                    "od.Sku, " +
                    "r.Reference1,"+
                    "rd.POLineNumber";
                orderBy = " ORDER BY  o.OrderDate,o.ShippingInstructions1 ";
                movementReport.ExportDeclarationItems = cn.Query<ExportDeclarationReport>(string.Format("{0} {1} {2}", sql, groupBy, orderBy));

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
        public IEnumerable<GLDeclarationReport> GetStockDeclarationFreeZone(string dateType, string inventoryDate, string year, string month, object[] declarationType, object[] importers, string sku)
        {
            string connectionString = this.WMSConnectionString();
            var cn = new SqlConnection(connectionString);
            string cutOffDate = string.Empty;
            if (dateType == "0")
                cutOffDate = string.Format("{0}{1}{2}", year, month, DateTime.DaysInMonth(Convert.ToInt16(year), Convert.ToInt16(month)));
            else if (dateType == "1")
                cutOffDate = inventoryDate;
            try
            {
                string groupBy = string.Empty;
                string sql = string.Empty;

                sql = "SELECT " +
                "[ImporterName]=s.Company," +
                "[ImportDeclarationNo]=r.Reference1," +
                "[ImportDeclarationItemNo]=rd.POLineNumber," +
                "[ImportDeclarationDate]=CAST(CONVERT(VARCHAR,r.ReceiptDate,101) AS DATETIME)," +
                "[WarehouseReceivedDate]=CAST(CONVERT(VARCHAR,MIN(rd.DateReceived),101) AS DATETIME)," +
                "[Sku]=MIN(rd.Sku), " +
                 //"[SkuDescription]=(CASE WHEN rd.Sku='NOVC' THEN rd.Reference5 ELSE (SELECT DESCR FROM SKU WHERE STORERKEY=rd.StorerKey AND SKU=rd.Sku) END)," +
                "[SkuDescription]=rd.Reference5," +
                "[Quantity]=SUM(rd.QtyReceived),  " +
                "[UnitPrice]=rd.UnitPrice,  " +
                "[NetWgt]=SUM((rd.QtyReceived-COALESCE(pd.Qty,0))*(rd.NetWgt/rd.QtyReceived))," +
                "[UOM]=(CASE WHEN rd.ContainerKey='' THEN 'C62' ELSE rd.ContainerKey END), " +
                "[TotalAmount]=SUM(rd.UnitPrice*(rd.QtyReceived-COALESCE(pd.Qty,0))), " +
                "[TotalDuty]=SUM((rd.QtyReceived-COALESCE(pd.Qty,0))*(rd.OtherUnit1/rd.QtyReceived))," +
                "[QtyBalance]=SUM((rd.QtyReceived-COALESCE(pd.Qty,0))) " +
                "FROM " +
                "Receipt r LEFT JOIN ReceiptDetail rd ON r.Receiptkey=rd.Receiptkey " +
                "LEFT JOIN Storer s ON r.StorerKey=s.StorerKey " +
                "LEFT JOIN " +
                "(" +
                string.Format("SELECT pd.Id,[Qty]=SUM(pd.QTY) FROM ORDERS o LEFT JOIN PICKDETAIL pd ON o.ORDERKEY=pd.ORDERKEY WHERE CONVERT(VARCHAR,o.ORDERDATE,112) <='{0}' GROUP BY pd.Id ", cutOffDate) +
                ") pd ON rd.ToId=pd.Id " +
                "WHERE rd.QtyReceived >0 AND r.Reference1 NOT LIKE 'CAN%' AND rd.ConditionCode <> 'NOTOK'  ";

                sql += string.Format(" AND CONVERT(VARCHAR,r.ReceiptDate,112) <='{0}' ", cutOffDate);

                if (declarationType != null)
                {
                    sql += string.Format(" AND SUBSTRING(r.Reference1,5,1) IN ({0})", _helperRepository.GetSQLInCondition(declarationType));
                }
                if (importers != null)
                {
                    sql += string.Format(" AND r.StorerKey IN ({0})", _helperRepository.GetSQLInCondition(importers));
                }
                if (sku != null)
                {
                    sql += string.Format(" AND rd.Sku ='{0}' ", sku);
                }
                groupBy = "GROUP BY " +
                "r.Reference1," +
                "rd.POLineNumber," +
                "rd.StorerKey," +
                "CAST(CONVERT(VARCHAR,r.ReceiptDate,101) AS DATETIME)," +
                    //"rd.Sku, " +
                "rd.ContainerKey," +
                "rd.Reference5," +
                "rd.UnitPrice," +
                "s.Company  " +
                "HAVING SUM((rd.QtyReceived-COALESCE(pd.Qty,0))) >0 " +
                "ORDER BY CAST(CONVERT(VARCHAR,r.ReceiptDate,101) AS DATETIME),r.Reference1,rd.POLineNumber ";

                return cn.Query<GLDeclarationReport>(string.Format("{0} {1}", sql, groupBy));

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

        public IEnumerable<GLDeclarationReport> GetStockDeclarationByLocation(string dateType, string inventoryDate, string year, string month, object[] declarationType, object[] importers, string sku)
        {
            string connectionString = this.WMSConnectionString();
            var cn = new SqlConnection(connectionString);
            string cutOffDate = string.Empty;
            if (dateType == "0")
                cutOffDate = string.Format("{0}{1}{2}", year, month, DateTime.DaysInMonth(Convert.ToInt16(year), Convert.ToInt16(month)));
            else if (dateType == "1")
                cutOffDate = inventoryDate;
            try
            {
                string groupBy = string.Empty;
                string sql = string.Empty;

                sql = "SELECT " +
                "[ImporterName]=r.ImporterName," +
                "[ImportDeclarationNo]=r.ImportDeclarationNo," +
                "[ImportDeclarationItemNo]=r.ImportDeclarationItemNo," +
                "[ImportDeclarationDate]=r.ImportDeclarationDate," +
                "[WarehouseReceivedDate]=r.WarehouseReceivedDate," +
                "[BinLocation]=r.BinLocation," +
                "[NetWgt]=r.NetWgt," +
                "[Sku]=r.Sku, " +
                "[StorerKey]=r.StorerKey, " +
                "[ApprovedDate]=r.ApprovedDate, " +
                "[TariffCode]=r.TariffCode, " +
                "[SkuDescription]=r.SkuDescription," +
                "[UnitPrice]=r.UnitPrice,  " +
                "[NetWgt]=r.NetWgt," +
                "[UOM]='C62', " +
                "[TotalAmount]=r.TotalAmount, " +
                "[QtyBalance]=r.QtyBalance, " +
                "[CameraNo]=c.Short, " +
                "[CameraLink]=c.CAMERA_LINK " +
                "FROM " +
                "CUSTOMS_INVENTORY r " +
                "LEFT JOIN " +
                "(" +
                "SELECT " +
                "A.CODE," +
                "A.SHORT," +
                "B.CAMERA_LINK " +
                "FROM CODELKUP A LEFT JOIN(SELECT CODE, [CAMERA_LINK]= DESCRIPTION FROM CODELKUP WHERE LISTNAME = 'CAMERALINK')  B ON A.LONG = B.CODE " +
                "WHERE A.LISTNAME = 'CAMERA' " +
                ") c ON r.RowNo=c.Code " +
                "WHERE COALESCE(r.ImportDeclarationNo,'') <> '' AND r.QtyBalance >0  ";

                if (declarationType != null)
                {
                    sql += string.Format(" AND r.DeclarationType IN ({0})", _helperRepository.GetSQLInCondition(declarationType));
                }
                if (importers != null)
                {
                    sql += string.Format(" AND r.StorerKey IN ({0})", _helperRepository.GetSQLInCondition(importers));
                }
                if (sku != null)
                {
                    sql += string.Format(" AND r.Sku ='{0}' ", sku);
                }
                groupBy = "" +
                "ORDER BY r.ImportDeclarationDate,r.ImportDeclarationNo,r.ImportDeclarationItemNo ";
              

                return cn.Query<GLDeclarationReport>(string.Format("{0} {1}", sql, groupBy));

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
