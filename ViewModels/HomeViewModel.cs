using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OnlineShop.Models;
using OnlineShop.Services;

namespace OnlineShop.ViewModels
{
    public class HomeViewModel
    {
        [DisplayName("搜尋:")]
        public string Search { get; set; }

        //顯示部落格資料
        public List<Members> DataList { get; set; }

        //分頁內容
        public ForPaging Paging { get; set; }
    }
}