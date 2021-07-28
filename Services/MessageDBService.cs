using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineShop.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace OnlineShop.Services
{
    public class MessageDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["SHOP"].ConnectionString;

        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 取得資料 GetDataList
        public List<Message> GetDataList(ForPaging Paging, int A_Id)
        {
            List<Message> DataList = new List<Message>();

            SetMaxingPaging(Paging, A_Id);
            DataList = GetAllDataList(Paging, A_Id);

            return DataList;
        }
        #endregion

        #region 無搜尋值設定最大頁數
        public void SetMaxingPaging(ForPaging Paging, int A_Id)
        {
            int row = 0;

            string sql = $@" select * from Message where A_Id='{A_Id}' ";

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
        public List<Message> GetAllDataList(ForPaging Paging, int A_Id)
        {
            List<Message> DataList = new List<Message>();

            string sql = $@"select m.*,d.Name from (select row_number() over(order by M_Id) as sort,* from Message where A_Id=@A_Id) m 
                           inner join Members as d on m.Account=d.Account where m.sort between 
                             {(Paging.NowPage - 1) * Paging.ItemNum + 1} and {Paging.NowPage * Paging.ItemNum} ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@A_Id", A_Id));

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Message Data = new Message();
                    Data.M_Id = Convert.ToInt32(dr["M_Id"]);
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
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

        #region 新增文章留言 
        public void InsertMessage(Message newData)
        {
            newData.M_Id = LastMessageFinder(newData.A_Id);

            string sql = @"insert into Message(A_Id,M_Id,Account,Content,CreateTime) values (@A_Id,@M_Id,@Account,@Content,@CreateTime)";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@A_Id", newData.A_Id));
                cmd.Parameters.Add(new SqlParameter("@M_Id", newData.M_Id));
                cmd.Parameters.Add(new SqlParameter("@Content", newData.Content));
                cmd.Parameters.Add(new SqlParameter("@Account", newData.Account));
                cmd.Parameters.Add(new SqlParameter("@CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

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

        #region 計算目前留言最新一筆的M_Id
        public int LastMessageFinder(int A_Id)
        {
            int Id;

            string sql = "select top 1 * from Message where A_Id=@A_Id order by M_Id desc";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@A_Id", A_Id));
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();

                Id = Convert.ToInt32(dr["M_Id"]);

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

        #region 修改留言
        public void UpdateMessage(Message UpdateData)
        {
            string sql = @"update Message set Content=@Content where A_Id=@A_Id and M_Id=@M_Id ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Content", UpdateData.Content));
                cmd.Parameters.Add(new SqlParameter("@A_Id", UpdateData.A_Id));
                cmd.Parameters.Add(new SqlParameter("@M_Id", UpdateData.M_Id));

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

        #region 刪除留言
        public void DeleteMessage(int A_Id,int M_Id)
        {
            string DeleteMessage = @"delete from Message where A_Id=@A_Id and M_Id=@M_Id ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(DeleteMessage, conn);
                cmd.Parameters.Add(new SqlParameter("@A_Id", A_Id));
                cmd.Parameters.Add(new SqlParameter("@M_Id", M_Id));

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