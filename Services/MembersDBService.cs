using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using OnlineShop.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace OnlineShop.Services
{
    public class MembersDBService
    {
        //private readonly static string cnstr = ConfigurationManager.ConnectionStrings["SHOP"].ConnectionString;

        //private readonly SqlConnection conn = new SqlConnection(cnstr);

        //新方法
        public IDBFactory dBFactory = DBAccessFactory.CreateDBFactory();
        public BaseDBAccess DBAccess = null;

        #region 註冊
        public void Register(Members newMemder)
        {
            //先將密碼hash編碼
            newMemder.Password = HashPassword(newMemder.Password);

            string sql = @"insert into Members(Account,Password,Name,Image,Email,AuthCode,IsAdmin) values (@Account,@Password,@Name,@Image,@Email,@AuthCode,@IsAdmin)";

            try
            {
                DBAccess = dBFactory.CreateDBAccess(sql);
                
                DBAccess.ParameterList.Add(new SqlParameter("@Account", newMemder.Account));
                DBAccess.ParameterList.Add(new SqlParameter("@Password", newMemder.Password));
                DBAccess.ParameterList.Add(new SqlParameter("@Name", newMemder.Name));
                DBAccess.ParameterList.Add(new SqlParameter("@Image", newMemder.Image));
                DBAccess.ParameterList.Add(new SqlParameter("@Email", newMemder.Email));
                DBAccess.ParameterList.Add(new SqlParameter("@AuthCode", newMemder.AuthCode));
                DBAccess.ParameterList.Add(new SqlParameter("@IsAdmin", "0"));

                DBAccess.ExecuteSingleCommand();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }

        }

        public string HashPassword(string pPassword)
        {
            string hashResult = string.Empty;
            //宣告Hash時所添加的無意義亂數值
            string saltkey = "1q2w3e";

            string saltkeyAndPassword = string.Concat(pPassword, saltkey);

            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            //取得密碼轉換成byte資料
            byte[] passwordData = Encoding.Default.GetBytes(saltkeyAndPassword);
            //取得Hash後 byte資料
            byte[] hashData = sha256.ComputeHash(passwordData);

            hashResult = Convert.ToBase64String(hashData);

            return hashResult;
        }

        /// <summary>
        /// 確認要註冊的帳號是否有被註冊過
        /// </summary>
        /// <param name="pAccount"></param>
        /// <returns></returns>
        public bool AccountCheck(string pAccount)
        {
            Members member = GetDataByAccount(pAccount);

            bool result = (member == null);

            return result;
        }
        #endregion

        #region 信箱驗證
        public string EmailValidate(string pAccount, string pAuthCode)
        {
            Members member = GetDataByAccount(pAccount);

            string validateStr = string.Empty;

            if (member != null)
            {
                if (member.AuthCode == pAuthCode)
                {
                    string sql = @"update Members set AuthCode='' where Account=@Account";

                    try
                    {
                        DBAccess = dBFactory.CreateDBAccess(sql);
                        DBAccess.ParameterList.Add(new SqlParameter("@Account", pAccount));

                        DBAccess.ExecuteSingleCommand();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message.ToString());
                    }

                    validateStr = "帳號信箱驗證成功，現在可以登入了";
                }
                else
                {
                    validateStr = "驗證碼錯誤，請重新確認";
                }
            }
            else
            {
                validateStr = "資料傳送錯誤，請重新確認或再註冊";
            }

            return validateStr;
        }
        #endregion

        #region GetDataByAccount
        private Members GetDataByAccount(string pAccount)
        {
            Members member = new Members();

            string sql = @"select * from Members where Account=@Account";

            try
            {
                DBAccess = dBFactory.CreateDBAccess(sql);
                DBAccess.ParameterList.Add(new SqlParameter("@Account", pAccount));

                System.Data.DataTable DT = DBAccess.ExecuteSelectQuery();

                member.Account = DT.Rows[0]["Account"].ToString();
                member.Password = DT.Rows[0]["Password"].ToString();
                member.Name = DT.Rows[0]["Name"].ToString();
                member.Email = DT.Rows[0]["Email"].ToString();
                member.AuthCode = DT.Rows[0]["AuthCode"].ToString();
                member.IsAdmin = Convert.ToBoolean(DT.Rows[0]["IsAdmin"].ToString());

            }
            catch (Exception)
            {
                member = null;
            }

            return member;
        }

        public Members GetDatabyAccount(string pAccount)
        {
            Members member = new Members();

            string sql = @"select * from Members where Account=@Account";

            try
            {
                DBAccess = dBFactory.CreateDBAccess(sql);
                DBAccess.ParameterList.Add(new SqlParameter("@Account", pAccount));

                System.Data.DataTable DT = DBAccess.ExecuteSelectQuery();

                member.Account = DT.Rows[0]["Account"].ToString();
                member.Name = DT.Rows[0]["Name"].ToString();
                member.Image = DT.Rows[0]["Image"].ToString();
            }
            catch (Exception)
            {
                member = null;
            }

            return member;
        }
        #endregion

        #region 登入
        public string LoginCheck(string Account, string Password)
        {
            Members LoginMember = GetDataByAccount(Account);

            if (LoginMember != null)
            {
                //有經過驗證，驗證碼會被清空
                if (string.IsNullOrWhiteSpace(LoginMember.AuthCode))
                {
                    //確認密碼
                    if (PasswordCheck(LoginMember, Password))
                    {
                        return "";
                    }
                    else
                    {
                        return "密碼輸入錯誤";
                    }
                }
                else
                {
                    return "此帳號尚未經過Email驗證，請去收信";
                }
            }
            else
            {
                return "無此會員帳號，請去註冊";
            }

        }
        /// <summary>
        /// 確認密碼
        /// </summary>
        /// <param name="pMembers"></param>
        /// <param name="pPassword"></param>
        /// <returns></returns>
        public bool PasswordCheck(Members pMembers, string pPassword)
        {
            bool result = pMembers.Password.Equals(HashPassword(pPassword));

            return result;
        }
        #endregion

        #region 變更密碼
        public string ChangePassword(string Account, string Password, string newPassword)
        {
            Members LoginMember = GetDataByAccount(Account);
            //確認舊密碼正確性
            if (PasswordCheck(LoginMember, Password))
            {
                LoginMember.Password = HashPassword(newPassword);

                string sql = @"update Members set Password=@Password where Account=@Account";

                try
                {
                    DBAccess = dBFactory.CreateDBAccess(sql);

                    DBAccess.ParameterList.Add(new SqlParameter("@Password", LoginMember.Password));
                    DBAccess.ParameterList.Add(new SqlParameter("@Account", Account));

                    DBAccess.ExecuteSingleCommand();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message.ToString());
                }

                return "密碼修改成功";
            }
            else
            {
                return "舊密碼輸入錯誤";
            }
        }
        #endregion

        #region 檢查圖片類型
        //可優化為設定檔
        public bool CheckImage(string ContentType)
        {
            switch (ContentType)
            {
                case "image/jpg":
                case "image/jpeg":
                case "image/png":
                    return true;
                default:
                    break;
            }

            return false;
        }
        #endregion

        #region 取得角色
        public string GetRole(string Account)
        {
            string Role = "User";

            Members LoginMember = GetDataByAccount(Account);

            if (LoginMember.IsAdmin)
            {
                Role += ",Admin";
            }

            return Role;
        }
        #endregion

        #region 查詢陣列資料
        //根據搜尋來取得資料陣列的方法
        public List<Members> GetDataList(ForPaging Paging, string Search)
        {
            //宣告要接受全部搜尋資料的物件
            List<Members> DataList = new List<Members>();
            //Sql語法
            if (!string.IsNullOrWhiteSpace(Search))
            {
                //有搜尋條件時
                SetMaxPaging(Paging, Search);
                DataList = GetAllDataList(Paging, Search);
            }
            else
            {
                //無搜尋條件時
                SetMaxPaging(Paging);
                DataList = GetAllDataList(Paging);
            }
            return DataList;
        }

        //無搜尋值的搜尋資料方法
        public List<Members> GetAllDataList(ForPaging paging)
        {
            //宣告要回傳的搜尋資料為資料庫中的Members資料表
            List<Members> DataList = new List<Members>();
            //Sql語法
            string sql = $@" select * from (select row_number() over(order by Account) as sort,* from Members ) m Where m.sort Between {(paging.NowPage - 1) * paging.ItemNum + 1} and {paging.NowPage * paging.ItemNum} ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                DBAccess = dBFactory.CreateDBAccess(sql);

                System.Data.DataTable DT = DBAccess.ExecuteSelectQuery();

                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    Members Data = new Members();
                    Data.Account = DT.Rows[i]["Account"].ToString();
                    Data.Name = DT.Rows[i]["Name"].ToString();
                    DataList.Add(Data);
                }

            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }

            //回傳搜尋資料
            return DataList;
        }

        //有搜尋值的搜尋資料方法
        public List<Members> GetAllDataList(ForPaging paging, string Search)
        {
            //宣告要回傳的搜尋資料為資料庫中的Members資料表
            List<Members> DataList = new List<Members>();
            //Sql語法
            string sql = $@" select * from (select row_number() over(order by Account) as sort,* from Members where Account like '%{Search}%' or Name like '%{Search}%' ) m 
Where m.sort Between {(paging.NowPage - 1) * paging.ItemNum + 1} and {paging.NowPage * paging.ItemNum} ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                DBAccess = dBFactory.CreateDBAccess(sql);

                System.Data.DataTable DT = DBAccess.ExecuteSelectQuery();

                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    Members Data = new Members();
                    Data.Account = DT.Rows[i]["Account"].ToString();
                    Data.Name = DT.Rows[i]["Name"].ToString();
                    DataList.Add(Data);
                }

            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }

            //回傳搜尋資料
            return DataList;
        }

        #region 設定最大頁數方法
        //無搜尋值的設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging)
        {
            //計算列數
            int Row = 0;
            //Sql語法
            string sql = $@" select * from Members ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                DBAccess = dBFactory.CreateDBAccess(sql);

                System.Data.DataTable DT = DBAccess.ExecuteSelectQuery();

                Row = DT.Rows.Count;
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }

            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
        }

        //有搜尋值的設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging, string Search)
        {
            //計算列數
            int Row = 0;
            //Sql語法
            string sql = $@" select * from Members Where Account like '%{Search}%' or Name like '%{Search}%' ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                DBAccess = dBFactory.CreateDBAccess(sql);

                System.Data.DataTable DT = DBAccess.ExecuteSelectQuery();

                Row = DT.Rows.Count;
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }

            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
        }
        #endregion
        #endregion
    }
}