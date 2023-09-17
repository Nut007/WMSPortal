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
    public class OrdersRepository : DataRepository<Orders>, IOrdersRepository
    {
        IHelperRepository _helper;
        public OrdersRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<Orders> sqlGenerator, IHelperRepository helper)
            : base(cache, connection, sqlGenerator)
        {
            _helper = helper;
        }
        public IEnumerable<OrderDetail> GetOutboundShipmentToday(string userId)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = "SELECT " +
                    //"o.WarehouseKey," +
                      "o.OrderDate, " +
                      "o.DeliveryDate, " +
                      "o.ShippingInstructions1, " +
                      "o.OrderKey," +
                      "o.ExternOrderKey," +
                      "o.BuyerPO," +
                      "o.StorerKey," +
                      "od.Sku," +
                      "od.OrderLineNumber," +
                      "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=o.StorerKey AND SKU=od.Sku)," +
                      "od.OpenQty," +
                      "od.QtyAllocated," +
                      "od.QtyPicked, " +
                      "od.ShippedQty," +
                      "od.PackKey," +
                      "od.UOM " +
                      "FROM " +
                      "Orders o LEFT JOIN OrderDetail od ON o.OrderKey=od.OrderKey " +
                      "WHERE o.OrderDate Between @StartDate AND @StopDate ";
                return cn.Query<OrderDetail>(sql, new { StartDate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00"), StopDate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59") });
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
        public Orders GetOrders(string ordersKey)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                Orders item = new Orders();

                string sql = string.Empty;
                sql = "SELECT " +
                      "o.OrderDate, " +
                      "o.StorerKey," +
                      "[StorerName]=(SELECT COMPANY FROM STORER WHERE STORERKEY=o.StorerKey)," +
                      "o.DeliveryDate, " +
                      "[ShippingInstructions1]=o.ShippingInstructions1," +
                      "o.OrderKey," +
                      "o.OrderGroup," +
                      "o.ExternOrderKey, " +
                      "o.Route, " +
                      "o.Door, " +
                      "o.Stop, " +
                      "o.OpenQty, " +
                      "o.ConsigneeKey," +
                      "o.C_Company," +
                      "o.C_Address1," +
                      "o.C_Address2," +
                      "o.C_Address3," +
                      "o.C_Address4," +
                      "o.C_City," +
                      "o.C_State," +
                      "o.C_Zip," +
                      "o.C_Country," +
                      "o.C_City," +
                      "o.BuyerPO," +
                      "o.BillToKey," +
                      "o.B_Company," +
                      "o.B_Address1," +
                      "o.B_Address2," +
                      "o.B_Address3," +
                      "o.B_Address4," +
                      "o.B_City," +
                      "o.B_State," +
                      "o.B_Zip," +
                      "o.B_Country, " +
                      "o.Flag3 " +
                      "FROM " +
                      "orders o " +
                      "WHERE o.OrderKey = @OrderKey";
                item = cn.Query<Orders>(sql, new { OrderKey = ordersKey }).SingleOrDefault();
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
        public IEnumerable<PickDetail> GetPickDetail(string orderKey, string orderLineNumber)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = "SELECT " +
                       "p.PickDetailKey, " +
                       "p.Sku, " +
                       "p.Lot, " +
                       "p.Loc, " +
                       "p.ID, " +
                       "p.Qty,  " +
                       "p.UOM,  " +
                       "p.AddDate,  " +
                       "p.EditDate  " +
                       "FROM " +
                       "PickDetail p " +
                       "WHERE p.OrderKey = @OrderKey AND p.OrderLineNumber=@OrderLineNumber";
                return cn.Query<PickDetail>(sql, new { OrderKey = orderKey, OrderLineNumber = orderLineNumber });
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
        public IEnumerable<OrderDetail> GetOutboundShipment(string column, string value1, string value2, string sectionView, string userId)
        {
            string sql = string.Empty;
            string groupBy = string.Empty;
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                IEnumerable<OrderDetail> results;
                if (sectionView == Convert.ToInt16(SectionView.Detail).ToString())
                {
                    groupBy = "GROUP BY o.OrderDate,o.DeliveryDate,o.Flag3,o.BuyerPO, " +
                   "o.OrderKey,o.LoadingPoint,o.ExternOrderKey,o.StorerKey,od.Sku,o.ShippingInstructions1," +
                   "(CASE WHEN od.Sku='DUMMY SKU' THEN od.Notes2 ELSE od.Sku END)";
                    sql = string.Empty;
                    sql = "SELECT " +
                           "o.OrderDate, " +
                           "o.DeliveryDate, " +
                           "o.OrderKey," +
                           "o.ExternOrderKey," +
                           "o.BuyerPO," +
                           "o.StorerKey," +
                           "o.LoadingPoint," +
                           "o.Flag3," +
                           "[Status]=CASE " +
										"WHEN SUM(od.OpenQty)=SUM(od.QtyPicked) THEN 'Completed' ELSE "+
										"(CASE WHEN SUM(od.QtyPicked)>0 THEN 'Released' ELSE 'Open' END) END," +
                           "[ShippingInstructions1]=o.ShippingInstructions1," +
                           "[Sku]=(CASE WHEN od.Sku='DUMMY SKU' THEN od.Notes2 ELSE od.Sku END)," +
                           "[SkuDescription]=(SELECT DESCR FROM SKU WHERE STORERKEY=o.StorerKey AND SKU=od.Sku)," +
                           "[OpenQty]=SUM(od.OpenQty)," +
                           "[QtyAllocated]=SUM(od.QtyAllocated)," +
                           "[QtyPicked]=SUM(od.QtyPicked), " +
                           "[ShippedQty]=SUM(od.ShippedQty) " +
                           "FROM " +
                           "Orders o LEFT JOIN OrderDetail od ON o.OrderKey=od.OrderKey " +
                           "WHERE ";
                    if (column == "OrderDate" || column == "DeliveryDate")
                    {
                        sql += "o.StorerKey IN (SELECT STORERKEY FROM USERSTORERGROUP WHERE USERID=@UserId) AND {0} Between @StartDate AND @StopDate ";
                        sql += groupBy + " ORDER BY o.OrderKey,od.Sku ";

                        results = cn.Query<OrderDetail>(string.Format(sql, column), new { StartDate = value1, StopDate = value2, UserId = userId });
                    }
                    else
                    {
                        sql += "o.StorerKey IN (SELECT STORERKEY FROM USERSTORERGROUP WHERE USERID=@UserId) AND {0} LIKE '%' + @SearchValue +'%' ";
                        sql += groupBy + " ORDER BY o.OrderKey,od.Sku ";
                        results = cn.Query<OrderDetail>(string.Format(sql, column), new { SearchValue = value1, UserId = userId });
                    }
                }
                else
                {
                    groupBy = "GROUP BY o.OrderDate,o.DeliveryDate,o.Flag3,o.BuyerPO, " +
                              "o.OrderKey,o.LoadingPoint,o.ExternOrderKey,o.StorerKey,o.ShippingInstructions1";
                    sql = "SELECT " +
                           "o.OrderDate, " +
                           "o.DeliveryDate, " +
                           "o.OrderKey," +
                           "o.ExternOrderKey," +
                           "o.BuyerPO," +
                           "o.StorerKey," +
                           "o.LoadingPoint," +
                           "o.Flag3," +
                           "[Status]=CASE " +
                                        "WHEN SUM(od.OpenQty)=SUM(od.QtyPicked) THEN 'Completed' ELSE " +
                                        "(CASE WHEN SUM(od.QtyPicked)>0 THEN 'Released' ELSE 'Open' END) END," +
                           "o.ShippingInstructions1," +
                           "[OpenQty]=SUM(od.OpenQty)," +
                           "[QtyAllocated]=SUM(od.QtyAllocated)," +
                           "[QtyPicked]=SUM(od.QtyPicked), " +
                           "[ShippedQty]=SUM(od.ShippedQty) " +
                           "FROM " +
                           "Orders o LEFT JOIN OrderDetail od ON o.OrderKey=od.OrderKey " +
                           "WHERE ";
                    if (column == "OrderDate" || column == "DeliveryDate")
                    {
                        sql += "o.StorerKey IN (SELECT STORERKEY FROM USERSTORERGROUP WHERE USERID=@UserId) AND {0} Between @StartDate AND @StopDate ";
                        sql += groupBy + " ORDER BY o.OrderKey ";

                        results = cn.Query<OrderDetail>(string.Format(sql, column), new { StartDate = value1, StopDate = value2,UserId =userId });
                    }
                    else
                    {
                        sql += "o.StorerKey IN (SELECT STORERKEY FROM USERSTORERGROUP WHERE USERID=@UserId) AND {0} LIKE '%' + @SearchValue +'%' ";
                        sql += groupBy + " ORDER BY o.OrderKey ";
                        results = cn.Query<OrderDetail>(string.Format(sql, column), new { SearchValue = value1, UserId = userId });
                    }

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
        public void SaveShipmentOrder(Orders order)
        {
            try
            {
                //order.OrderDate = DateTime.Now;
                order.AddDate = DateTime.Now;
                order.EditDate = DateTime.Now;
                order.EffectiveDate = DateTime.Now;
                order.ShiptoKey = order.ConsigneeKey;
                order.Priority = string.Empty;
                order.BillToKey = string.Empty;
                order.Door = string.Empty;
                order.Route = string.Empty;
                order.Type = "1";
                order.Stop = "COMP";
                order.AddWho = string.Empty;
                order.EditWho = string.Empty;
                this.Connection.ConnectionString = this.WMSConnectionString();
                if (string.IsNullOrEmpty(order.OrderKey))
                {
                    order.OrderKey = _helper.GetDocumentNo(DocumentType.Order);// "9000011349";
                    this.Insert(order);
                }
                else
                {
                    this.Update(order);
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
        public void DeleteShipmentOrder(string orderKey)
        {
            try
            {
                Orders o = new Orders() { OrderKey = orderKey };
                this.Connection.ConnectionString = this.WMSConnectionString();
                this.Delete(o);
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
        public void PostOrder(string orderKey)
        {
            string sql = string.Empty;
            string groupBy = string.Empty;
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                sql = "UPDATE ORDERS SET FLAG3='1' WHERE OrderKey='{0}' ";
                cn.Execute(string.Format(sql,  orderKey ));
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
        public IEnumerable<PickDetail> GetOrdersTransection(string orderKey)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = "SELECT " +
                       "o.OrderKey, " +
                       "o.ExternOrderKey, " +
                       "o.OrderDate, " +
                       "o.DeliveryDate, " +
                       "[CustomerName]=(SELECT COMPANY FROM STORER WHERE STORERKEY=o.SHIPTOKEY),"+
                       "p.Sku, " +
                       "[Qty]=SUM(p.Qty),  " +
                       "[Serialno]=(CASE WHEN COALESCE(p.Serialno,'')='' THEN 'NULL' ELSE p.Serialno END)  " +
                       "FROM " +
                       "PickDetail p LEFT JOIN Orders o ON p.OrderKey=o.OrderKey " +
                       "WHERE p.OrderKey = @OrderKey " +
                       "GROUP BY " +
                       "o.OrderKey, " +
                       "o.ExternOrderKey, " +
                       "o.OrderDate, " +
                       "o.DeliveryDate, " +
                       "o.SHIPTOKEY, " +
                       "p.Sku, " +
                       "(CASE WHEN COALESCE(p.Serialno,'')='' THEN 'NULL' ELSE p.Serialno END)   ";
                        
                return cn.Query<PickDetail>(sql, new { OrderKey = Convert.ToInt64(orderKey).ToString("0000000000") });
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

        public IEnumerable<OrderDetail> PickingDashboard()
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = "SELECT " +
                      "o.OrderDate, " +
                      "o.DeliveryDate, " +
                      "o.ShippingInstructions1, " +
                      "o.OrderKey," +
                      "o.ExternOrderKey," +
                      "o.BuyerPO," +
                      "o.StorerKey," +
                      "o.DATE5," +
                      "o.KK," + 
                      "[RoutingNotes]=o.RoutingNotes,"+
                      "[Status]=(CASE WHEN SUM(od.QtyAllocated+od.QtyPicked+od.ShippedQty)=0 THEN 'OPEN' ELSE 'PICKING' END) ," +
                      "[OpenQty]=SUM(od.QtyAllocated+od.QtyPicked+od.ShippedQty) " +
                      "FROM " +
                      "Orders o LEFT JOIN OrderDetail od ON o.OrderKey=od.OrderKey " +
                      "WHERE o.OrderDate Between @StartDate AND @StopDate AND (COALESCE(Date3,null) is null OR o.Status <> '9') " +
                      "GROUP BY " +
                      "o.OrderDate, " +
                      "o.RoutingNotes," +
                      "o.DeliveryDate, " +
                      "o.ShippingInstructions1, " +
                      "o.OrderKey," +
                      "o.DATE5," +
                      "o.KK," + 
                      "o.ExternOrderKey," +
                      "o.BuyerPO," +
                      "o.StorerKey " +
                      "HAVING " +
                      "SUM(od.QtyAllocated+od.QtyPicked+od.ShippedQty) > 0";
                return cn.Query<OrderDetail>(sql, new { StartDate = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd 00:00:00"), StopDate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59") });
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

        public OrdersDashboard GetOrdersDashboard()
        {
            OrdersDashboard result = new OrdersDashboard();
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                List<OrdersWeekly> items = new List<OrdersWeekly>();
                var results = cn.QueryMultiple("ORDERS_DASHBOARD", commandType: CommandType.StoredProcedure);
                var tot = results.Read<OrdersDashboard>().FirstOrDefault();
                var ordWeekly = results.Read<OrdersWeekly>();
               
                tot.OriginalQty = ordWeekly.Sum(x => x.TotalOrders);
                tot.ShippedQty = ordWeekly.Sum(x => x.ShippedQty);
                tot.TotalExternOrderKey = ordWeekly.Sum(x => x.TotalExternOrderKey);
                
                if (tot.OrdersWeekly == null) tot.OrdersWeekly = new List<OrdersWeekly>();
                tot.OrdersWeekly.AddRange(ordWeekly);
                if (ordWeekly.Count() == 0)
                {
                    DateTime startDate = DateTime.Now.AddDays(-8);
                    for (int i = 1; i <= 8; i++)
                    {
                        OrdersWeekly item = new OrdersWeekly();
                        item.OrderDate = startDate.AddDays(i);
                        item.TotalOrders = 0;
                        item.TotalExternOrderKey = 0;
                        item.ShippedQty = 0;
                        items.Add(item);
                    }
                }
                else
                {
                    DateTime dat = ordWeekly.Min(x => x.OrderDate).Date;
                    DateTime startDate = (dat > DateTime.Now.AddDays(-8)?dat:DateTime.Now.AddDays(-8));
                  
                    for (int i = 0; i <= 7; i++)
                    {
                        OrdersWeekly item = new OrdersWeekly();
                        item.OrderDate = startDate.AddDays(i);
                        var ow = ordWeekly.Where(x => x.OrderDate.Date == item.OrderDate.Date).SingleOrDefault();
                        if (ow == null)
                        {
                            item.TotalOrders = 0;
                            item.TotalExternOrderKey = 0;
                            item.ShippedQty = 0;
                        }
                        else
                        { 
                            item.TotalOrders = ow.TotalOrders;
                            item.TotalExternOrderKey = ow.TotalExternOrderKey;
                            item.ShippedQty = ow.ShippedQty;
                        }
                        items.Add(item);
                    }
               
                }
                
                tot.OrdersWeekly.AddRange(items);
                return tot;
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


        public OrdersDashboard GetOrdersPerfomance()
        {
            OrdersDashboard result = new OrdersDashboard();
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                List<OrdersWeekly> items = new List<OrdersWeekly>();
                var results = cn.QueryMultiple("ORDERS_PERFORMANCE_DASHBOARD", commandType: CommandType.StoredProcedure);
                //var tot = results.Read<OrdersWeekly>().FirstOrDefault();
                //var ordWeekly = results.Read<OrdersWeekly>();
                var ordWorkerPerform = results.Read<OrdersWeekly>();
                if (ordWorkerPerform.Count() == 0)
                {
                    result.WorkerPerfomanceWeekly = new List<OrdersWeekly>();
                }
                else
                {
                    if (result.WorkerPerfomanceWeekly == null) result.WorkerPerfomanceWeekly = new List<OrdersWeekly>();
                    result.WorkerPerfomanceWeekly.AddRange(ordWorkerPerform);
                }

                var locationUtilization = results.Read<OrdersWeekly>();
                if (locationUtilization.Count() == 0)
                {
                    result.LocationUtilization = new List<OrdersWeekly>();
                }
                else
                {
                    if (result.LocationUtilization == null) result.LocationUtilization = new List<OrdersWeekly>();
                    result.LocationUtilization.AddRange(locationUtilization);
                    result.TotalBin = result.LocationUtilization.Sum(x => x.TotalBin);
                    result.BinUsage = result.LocationUtilization.Sum(x => x.BinUsage);
                    result.QtyBoh = result.LocationUtilization.Sum(x => x.QtyBoh);
                }
                var receiptPerfomance = results.Read<OrdersWeekly>();
                if (receiptPerfomance.Count() == 0)
                {
                    result.ReceiptPerfomance = new List<OrdersWeekly>();
                }
                else
                {
                    if (result.ReceiptPerfomance == null) result.ReceiptPerfomance = new List<OrdersWeekly>();
                    result.ReceiptPerfomance.AddRange(receiptPerfomance);
                }
                var ordersTrans = results.Read<OrdersWeekly>();
                if (ordersTrans.Count() == 0)
                {
                    result.OrdersTransaction = new List<OrdersWeekly>();
                }
                else
                {
                    if (result.OrdersTransaction == null) result.OrdersTransaction = new List<OrdersWeekly>();
                    result.OrdersTransaction.AddRange(ordersTrans);
                }

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
        public IEnumerable<OrderDetail> GetPivotOrders(string startDate,string stopDate)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                return cn.Query<OrderDetail>("PIVOT_PENDING_ORDERS",
                        new { START_DATE = startDate, STOP_DATE = stopDate }, commandType: CommandType.StoredProcedure);
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
