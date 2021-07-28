using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OnlineShop.Models;
using OnlineShop.Services;

namespace OnlineShop.ViewModels
{
    public class ArticleIndexViewModel
    {
        [DisplayName("搜尋:")]
        public string Search { get; set; }

        //顯示陣列資料
        public List<Article> DataList { get; set; }

        //分頁內容
        public ForPaging Paging { get; set; }

        //文章列表的帳號
        public string Account { get; set; }
    }
}