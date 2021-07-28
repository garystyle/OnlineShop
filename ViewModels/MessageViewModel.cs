using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineShop.Models;
using OnlineShop.Services;

namespace OnlineShop.ViewModels
{
    public class MessageViewModel
    {
        //顯示留言
        public List<Message> DataList { get; set; }

        //分頁內容
        public ForPaging Paging { get; set; }

        //文章編號
        public int A_Id { get; set; }
    }
}