using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using OnlineShop.Services;
using OnlineShop.ViewModels;


namespace OnlineShop.Controllers
{
    public class AlbumController : Controller
    {
        private readonly AlbumDBService albumService = new AlbumDBService();

        // GET: Album
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        #region 相片列表
        [Authorize(Roles = "Admin")]
        public ActionResult List(int Page = 1)
        {
            AlbumViewModel Data = new AlbumViewModel();

            Data.Paging = new ForPaging(Page);

            Data.FileList = albumService.GetDataList(Data.Paging);

            return PartialView(Data);
        }
        #endregion

        #region 上傳檔案
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return PartialView();
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Upload([Bind(Include = "upload")]AlbumViewModel File)
        {
            //檢查是否有上傳檔案
            if (File.upload != null)
            {
                int Alb_Id = albumService.LastAlbumFinder();

                string fileName = Alb_Id.ToString() + "_" + File.upload.FileName;

                string Url = Path.Combine(Server.MapPath("~/Upload/"), fileName);

                File.upload.SaveAs(Url);

                albumService.UploadFile(Alb_Id, fileName, Url, File.upload.ContentLength, File.upload.ContentType, User.Identity.Name);
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region 顯示相片
        [Authorize(Roles = "Admin")]
        public ActionResult Show(int Alb_Id)
        {
            AlbumViewModel ToShow = new AlbumViewModel();

            ToShow.File = albumService.GetDataById(Alb_Id);

            if (ToShow.File != null)
            {
                //產生圖片路徑
                UrlHelper urlHelper = new UrlHelper(Request.RequestContext);
                urlHelper.Content("~/Upload/" + ToShow.File.FileName);

                return Content(urlHelper.Content("~/Upload/" + ToShow.File.FileName));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 下載檔案
        [Authorize(Roles = "Admin")]
        public ActionResult DownloadFile(int Alb_Id)
        {
            AlbumViewModel Download = new AlbumViewModel();

            Download.File = albumService.GetDataById(Alb_Id);

            if (Download.File != null)
            {
                Stream iStream = new FileStream(Download.File.Url, FileMode.Open, FileAccess.Read, FileShare.Read);

                return File(iStream, Download.File.Type, Download.File.FileName);
            }
            else
            {
                return JavaScript("alert(\"無此檔案\")");
            }
        }
        #endregion

        #region 刪除檔案
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteFile(int Alb_Id)
        {
            albumService.Delete(Alb_Id);

            return RedirectToAction("Index");
        }
        #endregion

        #region 相片輪播
        public ActionResult Carousel()
        {
            AlbumViewModel Data = new AlbumViewModel();

            Data.Paging = new ForPaging(1);

            Data.FileList = albumService.GetDataList(Data.Paging);

            return View(Data);
        }
        #endregion
    }
}