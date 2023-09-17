using MicroOrm.Pocos.SqlGenerator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSPortal.Core.Model;
using WMSPortal.Data.Repositories;
using Dapper;
using DapperExtensions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WMSPortal.Data
{
    public class HelperRepository : DataRepository<Storer>, IHelperRepository
    {
        public HelperRepository(ICacheProvider cache, IDbConnection connection, ISqlGenerator<Storer> sqlGenerator)
            : base( cache,connection, sqlGenerator)
        {

        }
        public IEnumerable<Storer> GetStorers()
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = "SELECT * FROM STORER";
                return cn.Query<Storer>(sql);
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


        public IEnumerable<Codelkup> GetDeclarationInboundType()
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = string.Format("SELECT [Code]=Code,[Description]=Description FROM CODELKUP WHERE LISTNAME='{0}'",DeclarationType.Inbound.ToString());
                return cn.Query<Codelkup>(sql);
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
        public IEnumerable<Codelkup> GetDeclarationOutboundType()
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = string.Format("SELECT [Code]=Code,[Description]=Description FROM CODELKUP WHERE LISTNAME='{0}'", DeclarationType.Outbound.ToString());
                return cn.Query<Codelkup>(sql);
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

        public string GetSQLInCondition(object[] conditions)
        {
            string inCondition = string.Empty;
            if (conditions == null)
            {
                inCondition = string.Empty;
            }
            else
            {
                foreach (string condition in conditions)
                    inCondition += string.Format((inCondition == string.Empty ? "'{0}'" : ",'{0}'"), condition.Trim());
            }
           
            return inCondition;
        }


        public IEnumerable<Codelkup> GetDeclarationType()
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = string.Format("SELECT [Code]=Code,[Description]=Description FROM CODELKUP WHERE LISTNAME='{0}'", DeclarationType.DecType.ToString());
                return cn.Query<Codelkup>(sql);
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
        public string GetDocumentNo(DocumentType docType)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {

                cn.Open();
                var p = new DynamicParameters();
                p.Add("keyname", docType.ToString(), dbType: DbType.String, direction: ParameterDirection.Input, size: 18);
                p.Add("fieldlength", "10", DbType.Int16, direction: ParameterDirection.Input);
                p.Add("keystring", dbType: DbType.String, direction: ParameterDirection.Output, size: 25);
                p.Add("b_Success", dbType: DbType.Int16, direction: ParameterDirection.Output);
                p.Add("n_err", dbType: DbType.Int16, direction: ParameterDirection.Output);
                p.Add("c_errmsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 250);

                cn.Execute("nspg_getkey", p, commandType: CommandType.StoredProcedure);
                string newId = p.Get<string>("@keystring");
                return newId;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == System.Data.ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }


        public IEnumerable<Codelkup> GetMawbList()
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                string sql = string.Empty;
                sql = string.Format("SELECT [Code]=Code,[Description]=Description FROM CODELKUP WHERE LISTNAME='{0}'", DeclarationType.MAWB.ToString());
                return cn.Query<Codelkup>(sql);
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

        public IEnumerable<StoreProcedure> GetStoreProcedureReport()
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {

                cn.Open();
                return cn.Query<StoreProcedure>("sp_stored_procedures", commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == System.Data.ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        public IEnumerable<StoreProcedure> GetStoreProcedureColumns(string procedureName)
        {
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                cn.Open();
                var p = new DynamicParameters();
                p.Add("@procedure_name", procedureName);
               
                return cn.Query<StoreProcedure>("sp_sproc_columns", p,commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == System.Data.ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        public DataTable GetReportResult(List<StoreProcedure> parameters, string storeprocedureName)
        {
            //string spName = parameters.First().PROCEDURE_NAME.Replace(";1","");
            string spName = storeprocedureName.Replace(";1", "");
            var cn = new SqlConnection(this.WMSConnectionString());
            try
            {
                cn.Open();
                var p = new DynamicParameters();
                IDataReader reader;
                if (parameters == null)
                {
                    reader = cn.ExecuteReader(spName, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    foreach (var parameter in parameters)
                    {
                        p.Add(parameter.COLUMN_NAME, parameter.COLUMN_VALUE);
                    }
                    reader = cn.ExecuteReader(spName, p, commandType: CommandType.StoredProcedure);
                }

                DataTable table = new DataTable();
                table.Load(reader);
                foreach (DataColumn column in table.Columns)
                {
                    string columnName = Regex.Replace(column.ColumnName.ToString().Trim(), @"\s+", "_");
                    columnName = Regex.Replace(columnName, @"\-+", "_");
                    columnName = Regex.Replace(columnName, @"\&+", "_");
                    table.Columns[column.ColumnName].ColumnName = columnName;
                }
                table.AcceptChanges();
                return table;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == System.Data.ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }
        private DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                //dataTable.Columns.Add(prop.Name.Trim());
                string columnName = Regex.Replace(prop.Name.Trim(), @"\s+\$|\s+(?=\w+$)", ",");
                dataTable.Columns.Add(columnName);  
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}
