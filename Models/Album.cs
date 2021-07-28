using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Album
    {
        [DisplayName("編號")]
        public int Alb_Id { get; set; }

        [DisplayName("檔名")]
        public string FileName { get; set; }
        
        [DisplayName("路徑")]
        public string Url { get; set; }
        
        [DisplayName("大小(Byte)")]
        public int Size { get; set; }
        //檔案類型
        public string Type { get; set; }
        
        [DisplayName("上傳者")]
        public string Account { get; set; }
        
        [DisplayName("上傳時間")]
        public DateTime CreateTime { get; set; }
        //Member資料表
        public Members Member { get; set; } = new Members();
    }
}