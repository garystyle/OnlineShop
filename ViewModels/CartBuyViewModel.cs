using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineShop.Models;

namespace OnlineShop.ViewModels
{
    public class CartBuyViewModel
    {
        //購物車商品陣列
        public List<CartBuy> DataList { get; set; }
        
        //購物車是否已保存
        public bool isCartsave { get; set; }
    }
}