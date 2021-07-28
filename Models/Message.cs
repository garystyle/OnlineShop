using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Message
    {
        //文章編號
        public int A_Id { get; set; }
        //留言編號
        public int M_Id { get; set; }
        
        [DisplayName("留言帳號:")]
        public string Account { get; set; }
        
        [DisplayName("留言內容:")]
        public string Content { get; set; }

        [DisplayName("留言時間:")]
        public DateTime CreateTime { get; set; }
        //Member資料表
        public Members Member { get; set; } = new Members();
    }
}