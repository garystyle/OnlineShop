using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineShop.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace OnlineShop.Services
{
    public class CartService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["SHOP"].ConnectionString;

        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 取得於購物車內的商品陣列
        public List<CartBuy> GetItemFromCart(string Cart)
        {
            List<CartBuy> cartBuyList = new List<CartBuy>();

            string sql = @"select * from CartBuy as m inner join Item as d on m.Item_Id = d.Id where Cart_Id=@Cart_Id";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Cart_Id", Cart));

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    CartBuy cartBuy = new CartBuy();
                    cartBuy.Cart_Id = dr["Cart_Id"].ToString();
                    cartBuy.Item_Id = Convert.ToInt32(dr["Item_Id"]);
                    cartBuy.Item.Id = Convert.ToInt32(dr["Id"]);
                    cartBuy.Item.Image = dr["Image"].ToString();
                    cartBuy.Item.Name = dr["Name"].ToString();
                    cartBuy.Item.Price = Convert.ToInt32(dr["Price"]);
                    cartBuyList.Add(cartBuy);
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

            return cartBuyList;
        }
        #endregion

        #region 確認商品是否於購物車中的方法
        public bool CheckInCart(string Cart, int Item_Id)
        {
            CartBuy cartBuy = new CartBuy();

            string sql = @"select * from CartBuy as m inner join Item as d on m.Item_Id = d.Id where Cart_Id=@Cart_Id and Item_Id=@Item_Id ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Cart_Id", Cart));
                cmd.Parameters.Add(new SqlParameter("@Item_Id", Item_Id));

                SqlDataReader dr = cmd.ExecuteReader();

                dr.Read();

                cartBuy.Cart_Id = dr["Cart_Id"].ToString();
                cartBuy.Item_Id = Convert.ToInt32(dr["Item_Id"]);
                cartBuy.Item.Id = Convert.ToInt32(dr["Id"]);
                cartBuy.Item.Image = dr["Image"].ToString();
                cartBuy.Item.Name = dr["Name"].ToString();
                cartBuy.Item.Price = Convert.ToInt32(dr["Price"]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return (cartBuy != null);
        }
        #endregion

        #region 商品放入購物車
        public void AddtoCart(string Cart, int Item_Id)
        {
            string sql = @"insert into CartBuy(Cart_Id,Item_Id) values(@Cart_Id,@Item_Id) ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Cart_Id", Cart));
                cmd.Parameters.Add(new SqlParameter("@Item_Id", Item_Id));

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

        #region 將商品從購物車取出
        public void RemoveForCart(string Cart, int Item_Id)
        {
            string sql = @"delete from CartBuy where Cart_Id=@Cart_Id and Item_Id=@Item_Id ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Cart_Id", Cart));
                cmd.Parameters.Add(new SqlParameter("@Item_Id", Item_Id));

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

        #region 確認購物車是否有保存(會員)
        public bool CheckCartSave(string Account, string Cart)
        {
            //根據帳號與購物車編號取得CartSave資料表內資料
            CartSave cartSave = new CartSave();

            string sql = @"select * from CartSave as m inner join Members as d on m.Account = d.Account where m.Account=@Account and Cart_Id=@Cart_Id ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Account", Account));
                cmd.Parameters.Add(new SqlParameter("@Cart_Id", Cart));

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows) //這裡有點bug 再想
                {
                    dr.Read();

                    cartSave.Account = dr["Account"].ToString();
                    cartSave.Cart_Id = dr["Cart_Id"].ToString();
                    cartSave.Member.Name = dr["Name"].ToString();
                }

            }
            catch (Exception e)
            {
                cartSave = null;
            }
            finally
            {
                conn.Close();
            }
            //判斷是否有資料，已確認是否在購物車中
            return (cartSave != null);
        }
        #endregion

        #region 取得購物車編號
        public string GetCartSave(string Account)
        {
            //根據帳號取得CartSave資料表內資料
            CartSave cartSave = new CartSave();

            string sql = @"select * from CasrSave as m inner join Members as d on m.Account = d.Account where m.Account=@Account ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Account", Account));

                SqlDataReader dr = cmd.ExecuteReader();

                dr.Read();

                cartSave.Account = dr["Account"].ToString();
                cartSave.Cart_Id = dr["Cart_Id"].ToString();
                cartSave.Member.Name = dr["Name"].ToString();
            }
            catch (Exception)
            {
                cartSave = null;
            }
            finally
            {
                conn.Close();
            }

            if (cartSave != null)
            {
                return cartSave.Cart_Id;
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region 保存購物車
        public void SaveCart(string Account, string cart)
        {
            string sql = @"insert into CartSave(Account,Cart_Id) values(@Account,@Cart_Id) ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Account", Account));
                cmd.Parameters.Add(new SqlParameter("@Cart_Id", cart));

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

        #region 取消保存購物車
        public void SaveCartRemove(string Account)
        {
            string sql = @"delete from CartSave where Account=@Account ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Account", Account));

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