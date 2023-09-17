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
    public class ReceiptRepository : DataRepository<Receipt>, IReceiptRepository
    {
        public ReceiptRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<Receipt> sqlGenerator)
            : base(cache, connection, sqlGenerator)
        {

        }

        public Receipt GetReceipt(string receiptKey)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                Receipt item = new Receipt();

                string sql = string.Empty;
                sql = "SELECT " +
                      "r.StorerKey," +
                      "[StorerName]=(SELECT COMPANY FROM STORER WHERE STORERKEY=r.StorerKey)," +
                    //"r.WarehouseKey," +
                      "r.ReceiptDate, " +
                      "r.Reference1," +
                      "r.ReceiptKey," +
                      "r.PoKey," +
                      "r.WarehouseReference," +
                      "r.CarrierReference," +
                      "r.VehicleNumber " +
                      "FROM " +
                      "Receipt r " +
                      "WHERE r.ReceiptKey = @ReceiptKey";
                item = cn.Query<Receipt>(sql, new { ReceiptKey = receiptKey }).SingleOrDefault();
                return item;

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
        public IEnumerable<ReceiptDetail> GetReceiptDetail(string receiptKey)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = "SELECT " +
                      "r.StorerKey," +
                      "[StorerName]=(SELECT COMPANY FROM STORER WHERE STORERKEY=r.StorerKey)," +
                    //"r.WarehouseKey," +
                      "r.ReceiptDate, " +
                      "r.ReceiptKey," +
                      "r.PoKey," +
                      "r.WarehouseReference," +
                      "r.CarrierReference," +
                      "r.VehicleNumber," +
                      "r.Reference1," +
                      "rd.Reference8," +
                      "rd.ReceiptLineNumber," +
                      "[Sku]=(CASE WHEN rd.Sku='DUMMY SKU' THEN rd.Reference5 ELSE rd.Sku END)," +
                      "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=rd.StorerKey AND SKU=rd.Sku)," +
                      "rd.PackKey," +
                      "rd.UOM," +
                      "rd.ToId," +
                      "rd.QtyExpected," +
                      "rd.QtyReceived, " +
                      "[UnitPrice]=rd.UnitPrice," +
                      "[NetWgt]=rd.NetWgt," +
                      "[GrossWgt]=rd.GrossWgt " +
                      "FROM " +
                      "Receipt r LEFT JOIN ReceiptDetail rd ON r.Receiptkey=rd.Receiptkey " +
                      "WHERE r.ReceiptKey = @ReceiptKey AND rd.ConditionCode <> 'NOTOK' ";

                return cn.Query<ReceiptDetail>(sql, new { ReceiptKey = receiptKey });
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
        public IEnumerable<ReceiptDetail> GetInboundShipment(string column, string value1, string value2, int connectionId, string userId)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            string groupBy = "GROUP BY r.ReceiptDate,r.ReceiptKey,r.PoKey," +
                "r.StorerKey,r.ReceiptType,r.ExternReceiptKey,r.WarehouseReference,rd.Sku," +
                "rd.StorerKey,r.Reference1, rd.Reference8,rd.Reference5," +
                "rd.UnitPrice," +
                "rd.NetWgt," +
                "rd.GrossWgt";
            try
            {
                IEnumerable<ReceiptDetail> results;
                string sql = string.Empty;
                sql = "SELECT " +
                    //"r.WarehouseKey," +
                      "r.ReceiptDate, " +
                      "r.ReceiptKey," +
                      "r.StorerKey," +
                      "r.ReceiptType," +
                      "r.PoKey," +
                      "r.Reference1," +
                      "r.ExternReceiptKey," +
                      "[WarehouseReference]=(CASE WHEN COALESCE(rd.Reference8,'') <> '' THEN rd.Reference8 ELSE r.WarehouseReference END)," +
                      "[Sku]=(CASE WHEN rd.Sku='DUMMY SKU' THEN rd.Reference5 ELSE rd.Sku END)," +
                    //"rd.Sku," +
                      "[UnitPrice]=rd.UnitPrice," +
                      "[NetWgt]=rd.NetWgt," +
                      "[GrossWgt]=rd.GrossWgt," +
                      "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=rd.StorerKey AND SKU=rd.Sku)," +
                      "[QtyExpected]=SUM(rd.QtyExpected)," +
                      "[QtyReceived]=SUM(rd.QtyReceived) " +
                      "FROM " +
                      "Receipt r LEFT JOIN ReceiptDetail rd ON r.Receiptkey=rd.Receiptkey " +
                      "WHERE rd.ConditionCode <> 'NOTOK' AND " +
                      "r.StorerKey IN (SELECT STORERKEY FROM USERSTORERGROUP WHERE USERID=@UserId) AND ";
                if (column == "ReceiptDate")
                {
                    sql += "{0} Between @StartDate AND @StopDate ";
                    sql += groupBy;
                    results = cn.Query<ReceiptDetail>(string.Format(sql, column), new { StartDate = value1, StopDate = value2, UserId = userId });
                }
                else
                {
                    sql += "{0} LIKE '%' + @SearchValue +'%' ";
                    sql += groupBy;
                    sql += " Order By r.ReceiptKey";
                    results = cn.Query<ReceiptDetail>(string.Format(sql, column), new { SearchValue = value1, UserId = userId });
                }
                return results;
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
        public TEMP_ID AddBaggage(TEMP_ID baggage)
        {
            List<TEMP_ID> items = new List<TEMP_ID>();
            int qtyBaggage = Convert.ToInt16(baggage.PACKUNIT);
            if (Convert.ToInt16(baggage.RECEPTACLE_NO) == 0)
            {
                for (int i = 1; i <= qtyBaggage; i++)
                {
                    TEMP_ID b = (TEMP_ID)baggage.Clone();
                    b.RECEPTACLE_NO = i.ToString("000");
                    items.Add(b);
                }
            }
            else
            {
                items.Add(baggage);
            }
            string sql = string.Empty;
            int ret = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(this.WMSConnectionString()))
                {
                    conn.Open();
                    if (!string.IsNullOrEmpty(baggage.CN38))
                    {
                        //conn.Insert(baggage);
                    }
                    else
                    {
                        foreach (var item in items)
                        {
                            if (item.TYPE == "MANIFESTNOVALIDATION")
                            {
                                try
                                {
                                    var retInserted =conn.Insert<TEMP_ID>(item);
                                }
                                catch(Exception ex)
                                {
                                    throw new ApplicationException(ex.Message);
                                }
                            }
                            else
                            {
                                if (item.PCNO == string.Empty)
                                {
                                    sql = "UPDATE TEMP_ID SET " +
                                   "SHIPMENT_STATUS=@SHIPMENT_STATUS,EDIT_DATE=getdate() " +
                                   "WHERE ID=@ID ";
                                    ret = conn.Execute(sql, new
                                    {
                                        ID = item.ID,
                                        SHIPMENT_STATUS = '2'
                                    });
                                }
                                else
                                {
                                    sql = "UPDATE TEMP_ID SET " +
                                        "SHIPMENT_STATUS=@SHIPMENT_STATUS,PCNO=@PCNO,EDIT_DATE=getdate() " +
                                        "WHERE ID=@ID AND MAWB=@MAWB";
                                    ret = conn.Execute(sql, new
                                    {
                                        ID = item.ID,
                                        SHIPMENT_STATUS = '3',
                                        PCNO = item.PCNO,
                                        MAWB = item.MAWB
                                    });
                                }
                                if (ret == 0)
                                {
                                    throw new ApplicationException(string.Format("Not found Id '{0}-{1}' or pre-booking incomplete.", baggage.DESPATCH_NO, baggage.RECEPTACLE_NO));
                                }
                                else
                                {
                                    baggage.ID = string.Format("{0}|{1}", "1", baggage.ID);
                                }
                            }
                        }
                    }
                }
                return baggage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (this.Connection.State == ConnectionState.Open)
                    this.Connection.Close();
            }
        }
        public int DeleteBaggage(string baggageNo)
        {
            int ret = 0;
            string sql;
            try
            {
                using (SqlConnection conn = new SqlConnection(this.WMSConnectionString()))
                {
                    conn.Open();

                    sql = "DELETE FROM TEMP_ID WHERE ID=@ID ";
                    ret = conn.Execute(sql, new
                    {
                        ID = baggageNo
                    });
                    return ret;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (this.Connection.State == ConnectionState.Open)
                    this.Connection.Close();
            }
        }
        public TEMP_ID ImportBaggage(TEMP_ID baggage)
        {
            List<TEMP_ID> items = new List<TEMP_ID>();
            int qtyBaggage = Convert.ToInt16(baggage.PACKUNIT);
            if (Convert.ToInt16(baggage.RECEPTACLE_NO) == 0)
            {
                for (int i = 1; i <= qtyBaggage; i++)
                {
                    TEMP_ID b = (TEMP_ID)baggage.Clone();
                    b.RECEPTACLE_NO = i.ToString("000");
                    items.Add(b);
                }
            }
            else
            {
                items.Add(baggage);
            }
            string sql = string.Empty;
            int ret = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(this.WMSConnectionString()))
                {
                    conn.Open();
                    
                    foreach (var item in items)
                    {
                        string status = (item.PCNO !=string.Empty?"0":"2");
                        if (!string.IsNullOrEmpty( item.PCNO))
                        {
                            sql = "UPDATE TEMP_ID SET " +
                              "SHIPMENT_STATUS=@SHIPMENT_STATUS,EDIT_DATE=getdate() " +
                              "WHERE ID=@ID ";
                               ret = conn.Execute(sql, new
                               {
                                    ID = item.ID,
                                    SHIPMENT_STATUS = status
                               });
                        }
                        else
                        {
                            sql = "UPDATE TEMP_ID SET " +
                            "LOT=@LOT,SHIPMENT_STATUS=@SHIPMENT_STATUS,EDIT_DATE=null " +
                            "WHERE ID=@ID ";
                            ret = conn.Execute(sql, new
                            {
                                ID = item.ID,
                                SHIPMENT_STATUS = status,
                                LOT= item.LOT
                            });
                        }
                      
                        if (ret == 0)
                        {
                            conn.Insert(item);
                            baggage.ID = string.Empty;//string.Format("{0}|{1}", "0", baggage.ID);
                        }
                        else
                        {
                            baggage.ID = string.Format("{0}|{1}", "1", baggage.ID);
                        }
                    }
                }
                return baggage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (this.Connection.State == ConnectionState.Open)
                    this.Connection.Close();
            }
        }
        public OperationResult AddBaggages(List<TEMP_ID> baggages)
        {
            OperationResult operationResult = new OperationResult();
            try
            {

                using (SqlConnection conn = new SqlConnection(this.WMSConnectionString()))
                {
                    conn.Open();
                    conn.Insert(baggages);
                }
                operationResult.Success = true;
                operationResult.Message = "Attachment Added Successfully";
            }
            catch (Exception ex)
            {
                operationResult.Success = false;
                operationResult.Message = "An Error Ocured During saving the new Attachment ";
            }
            finally
            {
                if (this.Connection.State == ConnectionState.Open)
                    this.Connection.Close();
            }
            return operationResult;

        }
        public IEnumerable<TEMP_ID> GetBaggageList(string column, string value1, string value2, int connectionId, string userId, string status)
        {
            var cn = new SqlConnection(this.WMSConnectionString());

            try
            {
                IEnumerable<TEMP_ID> results;
                string sql = string.Empty;
                sql = "SELECT  " +
                    "DESPATCH_NO," +
                    "RECEPTACLE_NO," +
                    "[ORIGIN_POST_CODE]=COALESCE(ORIGIN_POST_CODE,'')," +
                    "[DESTINATION_PORT]=COALESCE(DESTINATION_PORT,'')," +
                    "[MASTER_ID]=COALESCE(MAWB,'')," +
                    "GROSS_WEIGHT," +
                    "LOT," +
                    "PCNO," +
                    "ID, " +
                    "[ADD_DATE]=MAX(ADD_DATE)," +
                    "[EDIT_DATE]=MAX(EDIT_DATE) " +
                    "FROM " +
                    "TEMP_ID " +
                    "WHERE ";
                if (status != "0")
                    sql += "SHIPMENT_STATUS=@ShipmentStatus AND ";
                if (column == "EDIT_DATE" || column == "ADD_DATE" || column == "ISSUED_DATE")
                {
                    sql += "{0} Between @StartDate AND @StopDate ";
                    sql += " Group By " +
                       "DESPATCH_NO," +
                       "RECEPTACLE_NO," +
                       "LOT," +
                       "ORIGIN_POST_CODE," +
                       "ID, " +
                       "MAWB, " +
                       "DESTINATION_PORT," +
                       "GROSS_WEIGHT," +
                       "PCNO ";
                    sql += " Order By EDIT_DATE DESC ";
                    results = cn.Query<TEMP_ID>(string.Format(sql, column), new { StartDate = value1, StopDate = value2, UserId = userId, ShipmentStatus = status });
                }
                else
                {
                    if (value2 == "MANIFEST-MODE")
                    {
                        sql += " CONVERT(VARCHAR,EDIT_DATE,112) BETWEEN CONVERT(VARCHAR,getdate()-15,112) AND CONVERT(VARCHAR,getdate(),112)  AND ";
                    }
                    sql += "{0} LIKE '%' + @SearchValue +'%' ";
                    sql += " Group By " +
                     "DESPATCH_NO," +
                     "RECEPTACLE_NO," +
                     "ID, " +
                     "LOT," +
                     "ORIGIN_POST_CODE," +
                     "DESTINATION_PORT," +
                     "MAWB, " +
                     "GROSS_WEIGHT," +
                     "PCNO ";
                    sql += " Order By EDIT_DATE DESC ";
                    results = cn.Query<TEMP_ID>(string.Format(sql, column), new { SearchValue = value1, UserId = userId, ShipmentStatus = status });
                }
                return results;
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
        public bool DeleteBaggageItems(List<string> bagId,string type)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(this.WMSConnectionString()))
                {
                    conn.Open();
                    try
                    {
                        if (type == "MANIFESTNOVALIDATION")
                            conn.Execute(@"DELETE FROM TEMP_ID WHERE ID = @ID", bagId.Select(x => new { ID = x }).ToArray());
                        else
                            conn.Execute(@"UPDATE TEMP_ID SET PCNO='',SHIPMENT_STATUS='1' WHERE ID = @ID", bagId.Select(x => new { ID = x }).ToArray());
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }

                return true;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                //
            }
        }
        public OperationResult AddCN38Items(string mawb, List<string> bagId)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (IDbConnection conn = new SqlConnection(this.WMSConnectionString()))
                {
                    conn.Open();
                    try
                    {
                        conn.Execute(@"UPDATE TEMP_ID SET MAWB=@MAWB WHERE LOT <> '' AND ID = @ID", bagId.Select(x => new { ID = x, MAWB = mawb }).ToArray());
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
                operationResult.Success = true;
            }
            catch (SqlException ex)
            {
                operationResult.Success = false;
                operationResult.Message = "An Error Ocured During saving the new Attachment ";
            }
            return operationResult;
        }
        public IEnumerable<TEMP_ID> GetMawbItems(string mawb)
        {
            var cn = new SqlConnection(this.WMSConnectionString());

            try
            {
                IEnumerable<TEMP_ID> results;
                string sql = string.Empty;
                sql = "SELECT  " +
                    "LOT," +
                    "DESPATCH_NO=COUNT(DISTINCT DESPATCH_NO) ," +
                    "MAWB," +
                    "[GROSS_WEIGHT]=SUM(GROSS_WEIGHT)," +
                    "[PACKUNIT]=COUNT(DISTINCT ID) " +
                    "FROM TEMP_ID " +
                    "WHERE " +
                    "MAWB=@MAWB AND MAWB <>'' " +
                    "GROUP BY " +
                    "LOT," +
                    "MAWB " +
                    "ORDER BY LOT ";
                results = cn.Query<TEMP_ID>(string.Format(sql), new { MAWB = mawb });

                return results;
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
        public IEnumerable<TEMP_ID> GetCN38Items(string cn38)
        {
            var cn = new SqlConnection(this.WMSConnectionString());

            try
            {
                IEnumerable<TEMP_ID> results;
                string sql = string.Empty;
                sql = "SELECT  " +
                    "DESPATCH_NO," +
                    "RECEPTACLE_NO," +
                    "[ORIGIN_POST_CODE]=COALESCE(ORIGIN_PORT,'')," +
                    "[DESTINATION_POST_CODE]=COALESCE(DESTINATION_PORT,'')," +
                    "[MASTER_ID]=COALESCE(MASTER_ID,'')," +
                    "GROSS_WEIGHT," +
                    "PCNO," +
                    "LOT," +
                    "ID " +
                    "FROM " +
                    "TEMP_ID " +
                    "WHERE " +
                    "LOT=@LOT  ";

                results = cn.Query<TEMP_ID>(string.Format(sql, cn38), new { LOT = cn38 });

                return results;
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
        public bool DeleteCN38Items(string mawb, List<string> cn38)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(this.WMSConnectionString()))
                {
                    conn.Open();
                    try
                    {
                        conn.Execute(@"UPDATE TEMP_ID SET MAWB='' WHERE MAWB =@MAWB AND LOT = @LOT", cn38.Select(x => new { MAWB = mawb, LOT = x }).ToArray());
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }

                return true;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                //
            }
        }
       
        public bool UnManifestItems(string[] bagId)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(this.WMSConnectionString()))
                {
                    conn.Open();
                    try
                    {
                        conn.Execute(@"UPDATE TEMP_ID SET SHIPMENT_STATUS='2',PCNO='' WHERE ID = @ID", bagId.Select(x => new { ID = x }).ToArray());
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }

                return true;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                //
            }
        }
        public IEnumerable<TEMP_ID> GetMawbBaggages(string mawb, string containerno)
        {
            var cn = new SqlConnection(this.WMSConnectionString());

            try
            {
                IEnumerable<TEMP_ID> results;
                string sql = string.Empty;
                sql = "SELECT  " +
                    "DESPATCH_NO," +
                    "RECEPTACLE_NO," +
                    "[ORIGIN_POST_CODE]=COALESCE(ORIGIN_POST_CODE,'')," +
                    "[DESTINATION_PORT]=COALESCE(DESTINATION_PORT,'')," +
                    "[MASTER_ID]=COALESCE(MAWB,'')," +
                    "GROSS_WEIGHT," +
                    "LOT," +
                    "PCNO," +
                    "ID, " +
                    "[ADD_DATE]=MAX(ADD_DATE)," +
                    "[EDIT_DATE]=MAX(EDIT_DATE) " +
                    "FROM " +
                    "TEMP_ID " +
                    "WHERE " +
                    "PCNO = @PCNO AND MAWB = @MAWB " +
                    " Group By " +
                    "DESPATCH_NO," +
                    "RECEPTACLE_NO," +
                    "LOT," +
                    "ORIGIN_POST_CODE," +
                    "ID, " +
                    "MAWB, " +
                    "DESTINATION_PORT," +
                    "GROSS_WEIGHT," +
                    "PCNO ";
                    sql += " Order By EDIT_DATE DESC ";
                    results = cn.Query<TEMP_ID>(sql, new { PCNO = containerno, MAWB = mawb });
                
                return results;
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

        public IEnumerable<TEMP_ID> GetMissingCN35(string mawb)
        {
            var cn = new SqlConnection(this.WMSConnectionString());

            try
            {
                IEnumerable<TEMP_ID> results;
                string sql = string.Empty;
                sql = "SELECT  " +
                    "DESPATCH_NO," +
                    "RECEPTACLE_NO," +
                    "[ORIGIN_POST_CODE]=COALESCE(ORIGIN_POST_CODE,'')," +
                    "[DESTINATION_PORT]=COALESCE(DESTINATION_PORT,'')," +
                    "[MASTER_ID]=COALESCE(MAWB,'')," +
                    "GROSS_WEIGHT," +
                    "LOT," +
                    "PCNO," +
                    "ID, " +
                    "[ADD_DATE]=MAX(ADD_DATE)," +
                    "[EDIT_DATE]=MAX(EDIT_DATE) " +
                    "FROM " +
                    "TEMP_ID " +
                    "WHERE " +
                    "MAWB = @MAWB AND COALESCE(PCNO,'') ='' " +
                    "Group By " +
                    "DESPATCH_NO," +
                    "RECEPTACLE_NO," +
                    "LOT," +
                    "ORIGIN_POST_CODE," +
                    "ID, " +
                    "MAWB, " +
                    "DESTINATION_PORT," +
                    "GROSS_WEIGHT," +
                    "PCNO ";
                sql += " Order By EDIT_DATE DESC ";
                results = cn.Query<TEMP_ID>(sql, new {MAWB = mawb });

                return results;
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


        public bool AddBaggageItems(string[] bagId, string mawb, string containerNo)
        {
            try
            {
                foreach (string bag in bagId)
                {
                    TEMP_ID item = new TEMP_ID() { ID = bag, MAWB = mawb, PCNO = containerNo,PACKUNIT="1" };
                    AddBaggage(item);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<TEMP_ID> GetDespatchItems(string mawb, string handoverdate)
        {
            var cn = new SqlConnection(this.WMSConnectionString());

            try
            {
                IEnumerable<TEMP_ID> results;
                string sql = string.Empty;
                sql = "SELECT DISTINCT " +
                "[TYPE]='2'," +
                "[DESPATCH_NO]=('BKK' +LTRIM(DESPATCH_NO) + '/' + CAST(COUNT(DISTINCT ID) AS VARCHAR)  + '/' + CAST(CAST(SUM(GROSS_WEIGHT) AS DECIMAL(16,1)) AS VARCHAR) +' KG')," +
                "[DESTINATION_PORT]=COALESCE(DESTINATION_PORT,'')," +
                "[MASTER_ID]=COALESCE(MAWB,'')," +
                "[GROSS_WEIGHT]=SUM(GROSS_WEIGHT)," +
                string.Format("[ISSUED_DATE]='{0}',", handoverdate) +
                "[PEICES]=COUNT(DISTINCT ID) " +
                "FROM " +
                "TEMP_ID " +
                "WHERE " +
                "MAWB=@MAWB AND PCNO <> '' " +
                "GROUP BY " +
                "DESPATCH_NO, " +
                "COALESCE(DESTINATION_PORT,''), " +
                "COALESCE(MAWB,'') " +
                "UNION " +
                "SELECT DISTINCT " +
                "[TYPE]='1'," +
                "[DESPATCH_NO]=('BKK' +LTRIM(DESPATCH_NO) + '/' + CAST(COUNT(DISTINCT ID) AS VARCHAR)  + '/' + CAST(CAST(SUM(GROSS_WEIGHT) AS DECIMAL(16,1)) AS VARCHAR) +' KG')," +
                "[DESTINATION_PORT]=COALESCE(DESTINATION_PORT,'')," +
                "[MASTER_ID]=COALESCE(MAWB,'')," +
                "[GROSS_WEIGHT]=SUM(GROSS_WEIGHT)," +
                 string.Format("[ISSUED_DATE]='{0}',", handoverdate) +
                "[PEICES]=COUNT(DISTINCT ID) " +
                "FROM " +
                "TEMP_ID " +
                "WHERE " +
                "MAWB=@MAWB AND PCNO <> '' " +
                "GROUP BY " +
                "DESPATCH_NO, " +
                "COALESCE(DESTINATION_PORT,''), " +
                "COALESCE(MAWB,'') " +
                "ORDER BY 2 ";
                results = cn.Query<TEMP_ID>(sql, new { MAWB = mawb });
              
                return results.ToList();
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
        
        public IEnumerable<TEMP_ID> GetConsigmentItems(string mawb, string handoverdate)
        {
            var cn = new SqlConnection(this.WMSConnectionString());

            try
            {
                IEnumerable<TEMP_ID> results;
                string sql = string.Empty;
                sql = "SELECT DISTINCT " +
                "[CN38]=LOT, " +
                "MAWB, " +
                string.Format("[ISSUED_DATE]='{0}' ", handoverdate) +
                "FROM " +
                "TEMP_ID " +
                "WHERE " +
                "MAWB=@MAWB AND PCNO <> '' ";

                results = cn.Query<TEMP_ID>(sql, new { MAWB = mawb });

                return results.ToList();
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
        public IEnumerable<TEMP_ID> GetManifestItems(string mawb)
        {
            var cn = new SqlConnection(this.WMSConnectionString());

            try
            {
                IEnumerable<TEMP_ID> results;
                string sql = string.Empty;

                sql = "SELECT " +
                "MAWB," +
                "PCNO," +
                "LOT," +
                "DESPATCH_NO," +
                "DESTINATION_PORT," +
                //"[ORIGIN_PORT]=SUBSTRING(ID, 3, 3)," +
                "[GROSS_WEIGHT] = SUM(GROSS_WEIGHT)," +
                "[PEICES] = COUNT(DESPATCH_NO)," +
                "[SERVICE_TYPE] = (CASE WHEN SUBSTRING(LOT,1, 2)= 'TH' THEN 'THAI POST' ELSE 'CHINA POST' END) " +
                "FROM TEMP_ID " +
                "WHERE MAWB =@MAWB " +
                "GROUP BY " +
                "MAWB," +
                "PCNO," +
                "LOT," +
                "DESPATCH_NO," +
                //"SUBSTRING(ID, 3, 3)," +
                "DESTINATION_PORT";


                results = cn.Query<TEMP_ID>(sql, new { MAWB = mawb });

                return results.ToList();
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

        public int UpdateScanningStatus(TEMP_ID bagId)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(this.WMSConnectionString()))
                {
                    conn.Open();
                    try
                    {
                       return conn.Execute(@"UPDATE TEMP_ID SET SHIPMENT_STATUS='2',LOT=@LOT,EDIT_DATE=GETDATE() WHERE ID =@ID", new { ID = bagId.ID,LOT=bagId.LOT });
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                //
            }
        }
    }
}
