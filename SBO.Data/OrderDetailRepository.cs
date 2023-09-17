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
    public class OrderDetailRepository : DataRepository<OrderDetail>, IOrderDetailRepository
    {
        IStockBalanceRepository _stock;
        IHelperRepository _helper;
        public OrderDetailRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<OrderDetail> sqlGenerator,IStockBalanceRepository stock,IHelperRepository helper)
            : base( cache,connection, sqlGenerator)
        {
            _stock = stock;
            _helper = helper;
        }

        public bool InsertOrderItem(OrderDetail item)
        {
            try
            {
                this.Connection.ConnectionString = this.WMSConnectionString();
                item.OrderLineNumber =GetNextOrderLineNumber(item.OrderKey);
                return this.Insert(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateOrderItem(OrderDetail item)
        {
            try
            {
                this.Connection.ConnectionString = this.WMSConnectionString();
                this.Update(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteOrderItem(OrderDetail item)
        {
            try
            {
                this.Connection.ConnectionString = this.WMSConnectionString();
                this.Delete(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<OrderDetail> GetOrderDetail(string orderKey)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = "SELECT " +
                       "o.OrderDate, " +
                       "o.DeliveryDate, " +
                       "o.OrderKey," +
                       "o.ExternOrderKey," +
                       "[ShippingInstructions1]=o.ShippingInstructions1," +
                       "o.StorerKey," +
                       "od.OrderLineNumber," +
                       "[Sku]=(CASE WHEN od.Sku='DUMMY SKU' THEN od.Notes2 ELSE od.Sku END)," +
                       "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=od.StorerKey AND SKU=od.Sku)," +
                       "[Notes1]=(SELECT DESCR FROM SKU WHERE STORERKEY=od.StorerKey AND SKU=od.Sku)," +
                       "od.OpenQty," +
                       "od.QtyAllocated," +
                       "od.QtyPicked, " +
                       "od.ShippedQty," +
                       "od.PackKey," +
                       "od.UOM, " +
                       "od.UnitPrice, " +
                       "od.NetWeight,  " +
                       "od.GrossWeight  " +
                       "FROM " +
                       "OrderDetail od LEFT JOIN Orders o ON od.OrderKey=o.OrderKey " +
                       "WHERE o.OrderKey = @OrderKey";
                if (orderKey == string.Empty)
                {
                    IEnumerable<OrderDetail> noItems = new List<OrderDetail>();
                    return noItems;
                }
                else
                    return cn.Query<OrderDetail>(sql, new { OrderKey = orderKey.Trim() });
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
        public string GetNextOrderLineNumber(string orderKey)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = "SELECT " +
                      "[OrderLineNumber]=COALESCE(MAX(OrderLineNumber),0) +1 " +
                      "FROM " +
                      "OrderDetail " +
                      "WHERE OrderKey = @OrderKey";

               
                int line = cn.Query<int>(sql, new { OrderKey = orderKey.Trim() }).SingleOrDefault();
                return string.Format("{0:00000}", line);
               
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
        public IEnumerable<OrderDetail> GetOrderRemaining(string orderKey,string orderLineNumber)
        {
            string sql;
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {

                sql = "SELECT " +
                "O.OrderKey," +
                "OD.OrderLineNumber," +
                "OD.StorerKey," +
                "OD.Sku," +
                "OD.UOM," +
                "OD.PackKey," +
                "[OpenQty]=(OD.OPENQTY-(QtyAllocated+QtyPicked)) " +
                "FROM " +
                "ORDERS O LEFT JOIN ORDERDETAIL OD ON O.ORDERKEY=OD.ORDERKEY " +
                "WHERE O.ORDERKEY=@ORDERKEY ";
                if (orderLineNumber !=string.Empty)
                    sql += "AND OD.ORDERLINENUMBER = @ORDERLINENUMBER ";
                sql +="ORDER BY O.OrderKey,OD.OrderLineNumber";
                if (orderLineNumber ==string.Empty)
                    return cn.Query<OrderDetail>(sql, new { ORDERKEY = orderKey });
                else
                    return cn.Query<OrderDetail>(sql, new { ORDERKEY = orderKey, ORDERLINENUMBER=orderLineNumber });
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
        public void AllocateOrderItems(string orderKey,string orderLineNumber)
        {
            try
            {
                System.Decimal pullQty = 0;
                int rowCount;
                Int64 originalPull = 0;
                Int64 totalQty = 0;
                PickDetail pd;
                IEnumerable<OrderDetail> items = GetOrderRemaining(orderKey, orderLineNumber);

                foreach (OrderDetail item in items)
                {
                    pd = new PickDetail();
                    pullQty = item.OpenQty;
                    originalPull = (Int64)pullQty;
                    if (pullQty > 0)
                    {
                        IEnumerable<LotxLocxId> stockItems = _stock.GetInventoryBySku(item.StorerKey, item.Sku);
                        rowCount = 1;

                        foreach (LotxLocxId r in stockItems)
                        {
                            Decimal qtyBalance = r.QtyAvaliable;
                            string pickHeaderKey = _helper.GetDocumentNo(DocumentType.PickHeaderKey);
                            string pickDetailKey = _helper.GetDocumentNo(DocumentType.PickDetailKey);
                            string caseID = _helper.GetDocumentNo(DocumentType.CartonID);

                            string lot;
                            string loc;
                            string id;
                            int pickQty;
                            while (pullQty > 0 && qtyBalance > 0)
                            {

                                lot = r.Lot.ToString().Trim();
                                loc = r.Loc.ToString().Trim();
                                id = r.Id.ToString().Trim();

                                Int64 qtyTemp = (Int64)(pullQty - qtyBalance);

                                if (qtyTemp < 0)
                                {
                                    pickQty = (int)pullQty;
                                }
                                else
                                {
                                    pickQty = (int)qtyBalance;
                                }

                                if (pickQty > 0)
                                {
                                    using (var cn = new SqlConnection(this.WMSConnectionString()))
                                    {
                                        cn.Open();
                                        using (var tran = cn.BeginTransaction())
                                        {
                                            try
                                            {
                                                cn.Execute("usp_Mobile_Pick_Allocate_Process",
                                                    new
                                                    {
                                                        OrderKey = item.OrderKey,
                                                        OrderLineNumber = item.OrderLineNumber,
                                                        PickHeaderKey = pickHeaderKey,
                                                        PickDetailKey = pickDetailKey,
                                                        CaseID = caseID,
                                                        Lot = lot,
                                                        Storerkey = item.StorerKey,
                                                        Sku = item.Sku,
                                                        AltSku = string.Empty,
                                                        UOM = item.UOM,
                                                        UOMQty = 1,
                                                        Qty = pickQty,
                                                        QtyMoved = 0,
                                                        Status = "0",
                                                        DropID = string.Empty,
                                                        Loc = loc,
                                                        ID = id,
                                                        PackKey = item.PackKey,
                                                        UpdateSource = "0",
                                                        CartonGroup = "STD",
                                                        CartonType = string.Empty,
                                                        ToLoc = pd.ToLoc,
                                                        DoReplenish = string.Empty,
                                                        ReplenishZone = string.Empty,
                                                        DoCartonize = string.Empty,
                                                        PickMethod = string.Empty,
                                                        WaveKey = string.Empty,
                                                        EffectiveDate = DateTime.Now,
                                                        AddDate = DateTime.Now,
                                                        AddWho = "dbo",
                                                        EditDate = DateTime.Now,
                                                        EditWho = "dbo",
                                                        Serialno = string.Empty,
                                                        Pickwho = item.AddWho,
                                                        Pickdate = DateTime.Now,
                                                        NewSerialno = string.Empty,
                                                        DNnumber = pd.DNnumber,
                                                        PalletId = string.Empty,
                                                        Notes = string.Empty,
                                                        BoxNumber = string.Empty,
                                                        TotalBoxCount = string.Empty
                                                    },
                                                tran, commandType: CommandType.StoredProcedure);
                                                tran.Commit();
                                            }
                                            catch
                                            {
                                                tran.Rollback();
                                                throw;
                                            }
                                            finally
                                            {
                                                if (cn.State == ConnectionState.Open)
                                                    cn.Close();
                                            }
                                        }
                                    }

                                }

                                if (pullQty != 0)
                                {
                                    pullQty -= Convert.ToDecimal(pickQty);
                                }

                                qtyBalance -= Convert.ToDecimal(pickQty);
                                rowCount += 1;
                                totalQty += pickQty;
                            }

                            if (pullQty == 0) { break; }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void BatchAllocateItems(string orderKey,IEnumerable<LotxLocxId> orderItems,string userId)
        {
            try
            {
                List<OrderDetail> items = new List<OrderDetail>();

                foreach (var order in orderItems)
                {
                    OrderDetail item = new OrderDetail();
                    item.OrderKey = orderKey;
                    item.ExternOrderKey = string.Empty;
                    item.StorerKey = order.StorerKey;
                    item.Sku = order.Sku;
                    item.OpenQty = order.QtyOrder;
                    item.PackKey = "STD";
                    item.UOM = "EA";
                    item.AddWho = "sa";
                    item.AddDate = DateTime.Now;
                    item.EditWho = "sa";
                    item.EditDate = DateTime.Now;
                  
                    bool ret =InsertOrderItem(item);
                    if (ret)
                        AllocateOrderItems(item.OrderKey, item.OrderLineNumber);
                    
                }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

    }
    
}
