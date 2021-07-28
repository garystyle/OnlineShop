using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using OnlineShop.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace OnlineShop.Services
{
    public class AlbumDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["SHOP"].ConnectionString;

        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 取得單筆相片資料
        public Album GetDataById(int Alb_Id)
        {
            Album Data = new Album();

            string sql = @"select m.*,d.Name from Album m inner join Members d on m.Account = d.Account where m.Alb_Id =@Alb_Id ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Alb_Id", Alb_Id));

                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();

                Data.Alb_Id = Convert.ToInt32(dr["Alb_Id"]);
                Data.FileName = dr["FileName"].ToString();
                Data.Size = Convert.ToInt32(dr["Size"]);
                Data.Url = dr["Url"].ToString();
                Data.Type = dr["Type"].ToString();
                Data.Account = dr["Account"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                Data.Member.Name = dr["Name"].ToString();

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
        public List<Album> GetDataList(ForPaging Paging)
        {
            SetMaxPaging(Paging);

            List<Album> DataList = new List<Album>();

            string sql = $@"select m.*,d.Name from (select row_number() over(order by CreateTime desc) as sort,* from Album) m 
                           inner join Members as d on m.Account=d.Account where m.sort between 
                             {(Paging.NowPage - 1) * Paging.ItemNum + 1} and {Paging.NowPage * Paging.ItemNum} ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Album Data = new Album();
                    Data.Alb_Id = Convert.ToInt32(dr["Alb_Id"]);
                    Data.FileName = dr["FileName"].ToString();
                    Data.Size = Convert.ToInt32(dr["Size"]);
                    Data.Account = dr["Account"].ToString();
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

        #region 無搜尋值設定最大頁數
        public void SetMaxPaging(ForPaging Paging)
        {
            int row = 0;

            string sql = $@" select * from Album ";

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

        #region 上傳檔案
        public void UploadFile(int Alb_Id, string FileName, string Url, int Size, string Type, string Account)
        {
            string sql = @"insert into Album(Alb_Id,FileName,Url,Size,Type,Account,CreateTime) values(@Alb_Id,@FileName,@Url,@Size,@Type,@Account,@CreateTime)";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Alb_Id", Alb_Id));
                cmd.Parameters.Add(new SqlParameter("@FileName", FileName));
                cmd.Parameters.Add(new SqlParameter("@Url", Url));
                cmd.Parameters.Add(new SqlParameter("@Size", Size));
                cmd.Parameters.Add(new SqlParameter("@Type", Type));
                cmd.Parameters.Add(new SqlParameter("@Account", Account));
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

        #region 計算目前相片最新一筆的Alb_Id
        public int LastAlbumFinder()
        {
            int Id;

            string sql = "select top 1 * from Album order by Alb_Id desc";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();

                Id = Convert.ToInt32(dr["Alb_Id"]);

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

        #region
        public void Delete(int Alb_Id)
        {
            string sql = @"delete from Album where Alb_Id=@Alb_Id ";

            try
            {
                Album Data = GetDataById(Alb_Id);
                File.Delete(Data.Url);

                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Alb_Id", Alb_Id));

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