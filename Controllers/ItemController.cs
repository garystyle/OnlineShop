using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Services;
using OnlineShop.ViewModels;

namespace OnlineShop.Controllers
{
    public class ItemController : Controller
    {
        private readonly CartService cartService = new CartService();
        private readonly ItemService itemService = new ItemService();

        #region 初始畫面列表
        public ActionResult Index(int Page = 1)
        {
            ItemViewModel Data = new ItemViewModel();

            Data.Paging = new ForPaging(Page);

            Data.IdList = itemService.GetIdList(Data.Paging);

            Data.ItemBlock = new List<ItemDetailViewModel>();

            foreach (var Id in Data.IdList)
            {
                ItemDetailViewModel newBlock = new ItemDetailViewModel();
                //取得商品資料
                newBlock.Data = itemService.GetDataById(Id);
                //取得Session購物車資料
                string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : "";
                //確認是否於購物車中
                newBlock.InCart = cartService.CheckInCart(Cart, Id);

                Data.ItemBlock.Add(newBlock);
            }

            return View(Data);
        }
        #endregion

        #region 商品頁面
        public ActionResult Item(int Id)
        {
            ItemDetailViewModel ViewData = new ItemDetailViewModel();

            ViewData.Data = itemService.GetDataById(Id);
            //取得Session購物車資料
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : "";

            ViewData.InCart = cartService.CheckInCart(Cart, Id);

            return View(ViewData);
        }
        #endregion

        #region 商品列表中每一個商品區塊
        public ActionResult ItemBlock(int Id)
        {
            ItemDetailViewModel ViewData = new ItemDetailViewModel();

            ViewData.Data = itemService.GetDataById(Id);
            //取得Session購物車資料
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : "";

            ViewData.InCart = cartService.CheckInCart(Cart, Id);

            return PartialView(ViewData);
        }
        #endregion

        #region 新增商品
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Add(ItemCreateViewModel Data)
        {
            if (Data.ItemImage != null)
            {
                //取得檔名
                string fileName = Path.GetFileName(Data.ItemImage.FileName);
                string url = Path.Combine(Server.MapPath($@"~/Upload/"), fileName);
                Data.ItemImage.SaveAs(url);
                Data.NewData.Image = fileName;

                itemService.Insert(Data.NewData);

                return RedirectToAction("Index","Home");
            }
            else
            {
                ModelState.AddModelError("ItemImage","請選擇上傳檔案");

                return View(Data);
            }
        }
        #endregion

        #region 刪除商品
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int Id)
        {
            itemService.Delete(Id);

            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}