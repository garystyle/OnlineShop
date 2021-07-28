using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineShop.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace OnlineShop.Services
{
    public class ArticleDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["SHOP"].ConnectionString;

        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 取得單筆資料
        public Article GetArticleDataById(int A_Id)
        {
            Article Data = new Article();

            string sql = @"select m.*,d.Name,d.Image from Article m inner join Members d on m.Account = d.Account where m.A_Id =@A_Id ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@A_Id", A_Id));

                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();

                Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                Data.Account = dr["Account"].ToString();
                Data.Title = dr["Title"].ToString();
                Data.Content = dr["Content"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                Data.Watch = Convert.ToInt32(dr["Watch"]);
                Data.Member.Name = dr["Name"].ToString();
                Data.Member.Image = dr["Image"].ToString();
            }
            catch (Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }

            return Data;
        }
        #endregion

        #region 取得資料 GetDataList
        public List<Article> GetDataList(ForPaging Paging, string Search, string Account)
        {
            List<Article> DataList = new List<Article>();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                SetMaxingPaging(Paging, Search, Account);
                DataList = GetAllDataList(Paging, Search, Account);
            }
            else
            {
                SetMaxingPaging(Paging, Account);
                DataList = GetAllDataList(Paging, Account);
            }

            return DataList;
        }
        #endregion

        #region 無搜尋值設定最大頁數
        public void SetMaxingPaging(ForPaging Paging, string Account)
        {
            int row = 0;

            string sql = $@" select * from Article where Account='{Account}' ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    row++;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / Paging.ItemNum));
            Paging.SetRightPage();
        }
        #endregion

        #region 有搜尋值設定最大頁數
        public void SetMaxingPaging(ForPaging Paging, string Search, string Account)
        {
            int row = 0;

            string sql = $@"select * from Article where (Title like '%{Search}%' or Content like '%{Search}%') and Account='{Account}' ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    row++;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / Paging.ItemNum));
            Paging.SetRightPage();
        }
        #endregion

        #region 無搜尋值搜尋資料方法
        public List<Article> GetAllDataList(ForPaging Paging, string Account)
        {
            List<Article> DataList = new List<Article>();

            string sql = $@"select m.*,d.Name from (select row_number() over(order by A_Id) as sort,* from Article where Account='{Account}') m 
                           inner join Members as d on m.Account=d.Account where m.sort between 
                             {(Paging.NowPage - 1) * Paging.ItemNum + 1} and {Paging.NowPage * Paging.ItemNum} ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Article Data = new Article();
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Title = dr["Title"].ToString();
                    Data.Account = dr["Account"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Member.Name = dr["Name"].ToString();

                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return DataList;
        }
        #endregion

        #region 有搜尋值搜尋資料方法
        public List<Article> GetAllDataList(ForPaging Paging, string Search, string Account)
        {
            List<Article> DataList = new List<Article>();

            string sql = $@"select m.*,d.Name from (select row_number() over(order by A_Id) as sort,* from Article 
                            where (Title like '%{Search}%' or content like '%{Search}%') and Account='{Account}') m 
                            inner join Members as d on m.Account=d.Account where m.sort between 
                            {(Paging.NowPage - 1) * Paging.ItemNum + 1} and {Paging.NowPage * Paging.ItemNum} ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Article Data = new Article();
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Title = dr["Title"].ToString();
                    Data.Account = dr["Account"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Member.Name = dr["Name"].ToString();

                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return DataList;
        }
        #endregion

        #region 新增資料 InsertArticle
        public void InsertArticle(Article newData)
        {
            newData.A_Id = LastArticleFinder();

            string sql = @"insert into Article(A_Id,Title,Content,Account,CreateTime,Watch) values (@A_Id,@Title,@Content,@Account,@CreateTime,@Watch)";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@A_Id", newData.A_Id));
                cmd.Parameters.Add(new SqlParameter("@Title", newData.Title));
                cmd.Parameters.Add(new SqlParameter("@Content", newData.Content));
                cmd.Parameters.Add(new SqlParameter("@Account", newData.Account));
                cmd.Parameters.Add(new SqlParameter("@CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                cmd.Parameters.Add(new SqlParameter("@Watch", (object)0));

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 計算目前文章最新一筆的A_Id
        public int LastArticleFinder()
        {
            int Id;

            string sql = "select top 1 * from article order by A_Id desc";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();

                Id = Convert.ToInt32(dr["A_Id"]);

            }
            catch (Exception e)
            {
                Id = 0;
            }
            finally
            {
                conn.Close();
            }

            return Id + 1;
        }
        #endregion

        #region 修改文章
        public void UpdateArticle(Article UpdateData)
        {
            string sql = @"update Article set Content=@Content where A_Id=@A_Id";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Content", UpdateData.Content));
                cmd.Parameters.Add(new SqlParameter("@A_Id", UpdateData.A_Id));

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 檢查是否已回覆
        public bool CheckUpdate(int A_Id)
        {
            Article Data = GetArticleDataById(A_Id);

            int MessageCount = 0;

            string sql = "select * from Message where A_Id=@A_Id ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@A_Id", A_Id));

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    MessageCount++;
                } 
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return (Data != null && MessageCount == 0);
        }
        #endregion

        #region 刪除留言
        public void DeleteArticle(int A_Id)
        {
            //先刪除留言
            string DeleteMessage = @"delete from Message where A_Id=@A_Id ";
            //再刪除文章
            string DeleteArticle = @"delete from Article where A_Id=@A_Id ";

            string CombineSql = DeleteMessage + DeleteArticle;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(CombineSql, conn);
                cmd.Parameters.Add(new SqlParameter("@A_Id", A_Id));

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 人氣查詢
        public List<Article> GetPopularList(string Account)
        {
            List<Article> popularList = new List<Article>();

            string sql = "select top 5 * from Article as m inner join Members as d on m.Account=d.Account where m.Account=@Account order by watch desc ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Account", Account));

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Article Data = new Article();
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Title = dr["Title"].ToString();
                    Data.Account = dr["Account"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Watch = Convert.ToInt32(dr["Watch"]);
                    Data.Member.Name = dr["Name"].ToString();
                    popularList.Add(Data);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return popularList;
        }
        #endregion

        #region 增加觀看人數
        public void AddWatch(int A_Id)
        {
            string sql = @"update Article set Watch = Watch + 1 where A_Id=@A_Id";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@A_Id", A_Id));

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion
    }
}