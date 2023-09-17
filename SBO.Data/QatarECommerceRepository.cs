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
using MySql.Data.MySqlClient;
using System.Configuration;

namespace WMSPortal.Data
{
    public class QatarECommerceRepository : DataRepository<QatarECommerce>, IQatarECommerceRepository
    {
        public QatarECommerceRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<QatarECommerce> sqlGenerator)
            : base(cache, connection, sqlGenerator)
        {

        }
        private MySqlConnection GetConnection()
        {
            var mySQLECommerceConn = ConfigurationManager.ConnectionStrings["eCommerceConnection"].ConnectionString;
            return new MySqlConnection(mySQLECommerceConn);
        }
        public List<QatarECommerce> GetConsignmentInfo(string issueStartDate, string issueStopDate, string column, string value, int connectionId, string userId)
        {
            List<QatarECommerce> list = new List<QatarECommerce>();
            DateTime? dateBKK;
            Nullable<DateTime> dateDOH;
            Nullable<DateTime> datePickup;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string sql = string.Empty;
                sql = "SELECT * FROM tb_db_ecommerce LEFT JOIN tb_cargo_pickup ON REPLACE(tb_db_ecommerce.MAWB,'-','')=tb_cargo_pickup.MAWB WHERE tb_db_ecommerce.MAWB NOT LIKE 'QR%' AND ";
                sql += string.Format("Date_BKK Between '{0}' AND '{1}'", issueStartDate, issueStopDate);
                if (value !=string.Empty)
                {
                    sql += string.Format(" AND {0} LIKE '%{1}%' ", column, value);
                }
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(reader.GetOrdinal("Date_BKK")))
                            dateBKK = null;
                        else
                            dateBKK = reader.GetDateTime("Date_BKK");

                        if (reader.IsDBNull(reader.GetOrdinal("Date_DOH")))
                            dateDOH = null;
                        else
                            dateDOH = reader.GetDateTime("Date_DOH");

                        if (reader.IsDBNull(reader.GetOrdinal("PickupDate")))
                            datePickup = null;
                        else
                            datePickup = reader.GetDateTime("PickupDate");

                        list.Add(new QatarECommerce()
                        {
                            ConsignmentID = reader.GetString("ConsignmentID"),
                            Status =  reader.GetString("Status"),
                            ShptID =  reader.GetInt64("ShptID"),
                            Product =  reader.GetString("Product"),
                            Priority =  reader.GetInt32("Priority"),
                            GPA =  reader.GetString("GPA"),
                            Pickup_Date = datePickup,
                            Issue_Date =  reader.GetDateTime("Issue_Date"),
                            DN =  reader.GetString("DN"),
                            Cmdty =  reader.GetString("Cmdty"),
                            Origin =  reader.GetString("Origin"),
                            Dest =  reader.GetString("Dest"),
                            Peice =  reader.GetDecimal("Peice"),
                            Weight = reader.GetDecimal("Weight"),
                            Vol = reader.GetDecimal("Vol"),
                            Flight_BKK =  reader.GetString("Flight_BKK"),
                            Date_BKK = dateBKK,
                            MVT_BKK =  reader.GetString("MVT_BKK"),
                            Flight_DOH =  reader.GetString("Flight_DOH"),
                            Date_DOH = dateDOH,
                            MVT_DOH =  reader.GetString("MVT_DOH"),
                            Mawb =  reader.GetString("Mawb"),
                            RateType =  reader.GetString("RateType"),
                            Remark =  reader.GetString("Remark"),
                            ULD =  reader.GetString("ULD")
                        });
                    }
                }
            }

            return list;
        }
    }
}
