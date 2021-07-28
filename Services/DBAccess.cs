using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace OnlineShop.Services
{
    /// <summary>
    /// 資料庫存取基底類別
    /// </summary>
    public abstract class BaseDBAccess
    {
        public BaseDBAccess(string Query)
        {
            cmd = new SqlCommand(Query, conn);
        }
        
        protected static string ConnectionString { get; set; } = ConfigurationManager.ConnectionStrings["SHOP"].ConnectionString;

        protected readonly SqlConnection conn = new SqlConnection(ConnectionString);

        protected SqlCommand cmd = null;

        public List<SqlParameter> ParameterList = new List<SqlParameter>();

        public abstract DataTable ExecuteSelectQuery();

        public abstract void ExecuteSingleCommand();

        protected abstract void SetParameters();
    }

    /// <summary>
    /// MSSQL 資料庫類別
    /// </summary>
    public class SQLDBAccess : BaseDBAccess
    {
        public SQLDBAccess(string Query) : base(Query)
        {            
        }
        
        public override DataTable ExecuteSelectQuery()
        {
            SetParameters();

            DbDataAdapter myDataAdapter = new SqlDataAdapter(cmd);
            DataTable ResultTable = new DataTable("ResultTable");

            try
            {
                myDataAdapter.Fill(ResultTable);
            }
            catch (Exception e)
            {
                throw new Exception("執行DB時發生錯誤 : " + e.InnerException);
            }
            finally
            {
                this.conn.Close();
            }

            return ResultTable;
        }

        public override void ExecuteSingleCommand()
        {
            SetParameters();

            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();
            }

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("執行DB時發生錯誤 : " + e.InnerException);
            }
            finally
            {
                this.cmd.Connection.Close();
                this.cmd.Dispose();
            }

        }

        protected override void SetParameters()
        {
            cmd.Parameters.AddRange(ParameterList.ToArray());
        }
    }
}