using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Article
    {
        [DisplayName("文章編號")]
        public int A_Id { get; set; }

        [DisplayName("標題")]
        [Required(ErrorMessage = "請輸入標題")]
        [StringLength(50, ErrorMessage = "標題長度最多50字元")]
        public string Title { get; set; }

        [DisplayName("文章內容")]
        [Required(ErrorMessage = "請輸入文章內容")]
        public string Content { get; set; }
        
        //會員帳號
        [DisplayName("發表者")]
        public string Account { get; set; }

        [DisplayName("新增時間")]
        public DateTime CreateTime { get; set; }

        [DisplayName("瀏覽人數")]
        public int Watch { get; set; }

        //Member資料表
        public Members Member { get; set; } = new Members();
    }
}