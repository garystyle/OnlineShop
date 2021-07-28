using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using OnlineShop.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace OnlineShop.Services
{
    public class ItemService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["SHOP"].ConnectionString;

        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 取得單一商品資料
        public Item GetDataById(int Id)
        {
            Item item = new Item();

            string sql = @"select * from Item where Id=@Id";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Id", Id));

                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();

                item.Id = Convert.ToInt32(dr["Id"]);
                item.Image = dr["Image"].ToString();
                item.Name = dr["Name"].ToString();
                item.Price = Convert.ToInt32(dr["Price"]);

            }
            catch (Exception e)
            {
                item = null;
            }
            finally
            {
                conn.Close();
            }

            return item;
        }
        #endregion

        #region 取得商品編號陣列
        public List<int> GetIdList(ForPaging Paging)
        {
            //計算所需總頁面
            SetMaxingPaging(Paging);

            List<int> IdList = new List<int>();

            string sql = $@"select Id from (select row_number() over(order by Id desc) as sort,* from Item) as m 
                           where m.sort Between {(Paging.NowPage - 1) * Paging.ItemNum + 1} and {Paging.NowPage * Paging.ItemNum}";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    IdList.Add(Convert.ToInt32(dr["Id"]));
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

            return IdList;
        }
        #endregion

        #region 無搜尋值設定最大頁數
        public void SetMaxingPaging(ForPaging pPaging)
        {
            int row = 0;

            string sql = @" select * from Item ";

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

            pPaging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / pPaging.ItemNum));
            pPaging.SetRightPage();
        }
        #endregion

        #region 新增商品
        public void Insert(Item newData)
        {
            //取得最新一筆Id
            newData.Id = LastItemFinder();

            string sql = @"insert into Item(Id,Name,Price,Image) values(@Id,@Name,@Price,@Image) ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Id", newData.Id));
                cmd.Parameters.Add(new SqlParameter("@Name", newData.Name));
                cmd.Parameters.Add(new SqlParameter("@Price", newData.Price));
                cmd.Parameters.Add(new SqlParameter("@Image", newData.Image));

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

        #region 刪除商品
        public void Delete(int Id)
        {
            //先刪除CartBuy的資料才刪除Item
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("delete from CartBuy where Item_Id=@Id ");
            sql.AppendLine("delete from Item where Id=@Id ");

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
                cmd.Parameters.Add(new SqlParameter("@Id", Id));

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

        #region 計算目前商品最新一筆的Id
        public int LastItemFinder()
        {
            int Id;
            string sql = $@"select top 1 from Item order by Id desc ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();

                Id = Convert.ToInt32(dr["Id"]);
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
    }
}