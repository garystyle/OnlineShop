using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OnlineShop.Models;

namespace OnlineShop.ViewModels
{
    public class ItemCreateViewModel
    {
        [DisplayName("商品圖片")]
        [FileExtensions(ErrorMessage = "所上傳檔案不是圖片")]
        public HttpPostedFileBase ItemImage { get; set; }

        //新增商品內容
        public Item NewData { get; set; }
    }
}