using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class CartSave
    {
        //帳號
        public string Account { get; set; }
        //購物車編號
        public string Cart_Id { get; set; }
        //Member資料表
        public Members Member { get; set; } = new Members();
    }
}