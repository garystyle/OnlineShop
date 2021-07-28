using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using OnlineShop.Services;
using System.Data;
using System.Data.SqlClient;

namespace OnlineShop.Controllers
{
    public class DbTestController : ApiController
    {
        //IDBFactory dBFactory = new SQLDBFactory();
        public IDBFactory dBFactory = DBAccessFactory.CreateDBFactory();
        public BaseDBAccess DBAccess = null;

        //http://localhost:63695/api/DbTest/ExecuteSelect
        [HttpGet]
        public void ExecuteSelect()
        {
            string sql = @"select * from Members ";

            DBAccess = dBFactory.CreateDBAccess(sql);

            DataTable DT = DBAccess.ExecuteSelectQuery();
        }

        //http://localhost:63695/api/DbTest/ExecuteSelectByParameter
        [HttpGet]
        public void ExecuteSelectByParameter()
        {
            string sql = @"select * from Members where Account=@Account";

            DBAccess = dBFactory.CreateDBAccess(sql);

            DBAccess.ParameterList.Add(new SqlParameter("@Account", "gary60430"));

            DataTable DT = DBAccess.ExecuteSelectQuery();
        }

        //http://localhost:63695/api/DbTest/ExecuteSingleCommand
        [HttpGet]
        public void ExecuteSingleCommand()
        {
            string sql = @"insert into Members(Account,Password,Name,Image,Email,AuthCode,IsAdmin) 
                                      values (@Account,@Password,@Name,@Image,@Email,@AuthCode,@IsAdmin)";

            DBAccess = dBFactory.CreateDBAccess(sql);

            DBAccess.ParameterList.Add(new SqlParameter("@Account", "gary604301"));
            DBAccess.ParameterList.Add(new SqlParameter("@Password", "123456"));
            DBAccess.ParameterList.Add(new SqlParameter("@Name", "哈哈"));
            DBAccess.ParameterList.Add(new SqlParameter("@Image", "unlucky-desktop1.jpg"));
            DBAccess.ParameterList.Add(new SqlParameter("@Email", "gary60430@gamil.com"));
            DBAccess.ParameterList.Add(new SqlParameter("@AuthCode", ""));
            DBAccess.ParameterList.Add(new SqlParameter("@IsAdmin", "0"));

            DBAccess.ExecuteSingleCommand();

        }

        //public IQueryable<string> Get()
        //{
        //    return new string[] { "value1", "value2" }.AsQueryable();
        //}
    }
}
