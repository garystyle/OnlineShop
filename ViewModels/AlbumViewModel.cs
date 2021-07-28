using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OnlineShop.Models;
using OnlineShop.Services;

namespace OnlineShop.ViewModels
{
    public class AlbumViewModel
    {
        [DisplayName("上傳圖片")]
        [FileExtensions(ErrorMessage = "所上傳檔案不是圖片")]
        public HttpPostedFileBase upload { get; set; }

        //儲存的檔案陣列
        public List<Album> FileList { get; set; }

        //分頁內容
        public ForPaging Paging { get; set; }

        //單一筆檔案
        public Album File { get; set; }
    }
}