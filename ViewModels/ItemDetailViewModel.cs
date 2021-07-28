using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineShop.Models;

namespace OnlineShop.ViewModels
{
    public class ItemDetailViewModel
    {
        //新增商品內容
        public Item Data { get; set; }

        //是否在購物車中
        public bool InCart { get; set; }
    }
}