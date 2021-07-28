using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Services;
using OnlineShop.ViewModels;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService cartService = new CartService();

        #region 購物車頁面
        // GET: Cart
        [Authorize]
        public ActionResult Index()
        {
            CartBuyViewModel Data = new CartBuyViewModel();
            //取得Session內購物車資料
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : "";

            //取得購物車內的商品資料
            Data.DataList = cartService.GetItemFromCart(Cart);
            //確認購物車是否已保存
            Data.isCartsave = cartService.CheckCartSave(User.Identity.Name, Cart);

            return View(Data);
        }
        #endregion

        #region 保存使用者購物車資料
        [Authorize]
        public ActionResult CartSave()
        {
            string Cart;

            if (HttpContext.Session["Cart"] != null)
            {
                Cart = HttpContext.Session["Cart"].ToString();
            }
            else
            {
                //重新定義購物車
                Cart = DateTime.Now.ToString() + User.Identity.Name;

                HttpContext.Session["Cart"] = Cart;
            }

            cartService.SaveCart(User.Identity.Name, Cart);

            return RedirectToAction("Index");
        }
        #endregion

        #region 取消保存購物車
        [Authorize]
        public ActionResult CartSaveRemove()
        {
            cartService.SaveCartRemove(User.Identity.Name);

            return RedirectToAction("Index");
        }
        #endregion

        #region 將商品從購物車取出
        [Authorize]
        public ActionResult Pop(int Id, string toPage)
        {
            string Cart = HttpContext.Session["Cart"]?.ToString();
            //(HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;

            cartService.RemoveForCart(Cart, Id);

            //判斷傳入的toPage來決定導向
            if (toPage == "Item")
            {
                return RedirectToAction("Item", "Item", new { Id = Id });
            }
            else if (toPage == "ItemBlock")
            {
                return RedirectToAction("ItemBlock", "Item", new { Id = Id });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region 將商品放入購物車
        [Authorize]
        public ActionResult Put(int Id, string toPage)
        {
            if (HttpContext.Session["Cart"] == null)
            {
                HttpContext.Session["Cart"] = DateTime.Now.ToString() + User.Identity.Name;
            }

            cartService.AddtoCart(HttpContext.Session["Cart"].ToString(), Id);

            //判斷傳入的toPage來決定導向
            if (toPage == "Item")
            {
                return RedirectToAction("Item", "Item", new { Id = Id });
            }
            else if (toPage == "ItemBlock")
            {
                return RedirectToAction("ItemBlock", "Item", new { Id = Id });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        #endregion
    }
}