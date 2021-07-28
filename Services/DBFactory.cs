using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace OnlineShop.Services
{
    /// <summary>
    /// 工廠方法
    /// </summary>
    public interface IDBFactory
    {
        BaseDBAccess CreateDBAccess(string Query);
    }

    /// <summary>
    /// 創建MSSQL資料庫工廠 由用戶端改工廠
    /// </summary>
    public class SQLDBFactory : IDBFactory
    {
        public BaseDBAccess CreateDBAccess(string Query)
        {
            return new SQLDBAccess(Query);
        }
    }

    /// <summary>
    /// 用設定檔簡單工廠 用戶端就不用改
    /// </summary>
    public static class DBAccessFactory
    {
        private readonly static string SQLType = ConfigurationManager.AppSettings["DBType"];

        public static IDBFactory CreateDBFactory()
        {
            IDBFactory result = null;

            switch (SQLType)
            {
                case "SQLServer":
                    result = new SQLDBFactory();
                    break;
                default:
                    break;
            }

            return result;
        }

    }
}
