using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using OnlineShop.Services;
using OnlineShop.ViewModels;
using System.Web.Configuration;
using OnlineShop.Security;

namespace OnlineShop.Controllers
{
    public class MembersController : Controller
    {
        private readonly MembersDBService membersservice = new MembersDBService();

        private readonly MailService mailservice = new MailService();

        private readonly CartService cartService = new CartService();

        // GET: Members
        public ActionResult Index()
        {
            return View();
        }

        #region 註冊
        public ActionResult Register()
        {
            //判斷使用者是否已登入驗證
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Register(MembersRegisterViewModel RegisterMember)
        {
            //判斷頁面是否都經過驗證
            if (ModelState.IsValid)
            {
                if (RegisterMember.MembersImage != null)
                {
                    if (membersservice.CheckImage(RegisterMember.MembersImage.ContentType))
                    {
                        //取得檔名
                        string fileName = Path.GetFileName(RegisterMember.MembersImage.FileName);
                        string url = Path.Combine(Server.MapPath($@"~/Upload/Members/"), fileName);
                        RegisterMember.MembersImage.SaveAs(url);
                        RegisterMember.newMember.Image = fileName;

                        RegisterMember.newMember.Password = RegisterMember.Password;

                        //取得信箱驗證碼
                        string authCode = mailservice.GetValidateCode();
                        RegisterMember.newMember.AuthCode = authCode;

                        //呼叫service註冊新會員
                        membersservice.Register(RegisterMember.newMember);

                        //取得寫好的驗證信範本內容
                        //string tempMail = System.IO.File.ReadAllText(Server.MapPath("~/Views/Shared/RegisterEmailTemplate.html"));

                        ////宣告Email驗證用的Url
                        //UriBuilder validateUrl = new UriBuilder(Request.Url)
                        //{
                        //    Path = Url.Action("EmailValidate", "Members", new { Account = pRegisterMember.NewMember.Account, AuthCode = authCode })
                        //};

                        ////寄信
                        //string mailBody = mailservice.GetRegisterMailBody(tempMail, pRegisterMember.NewMember.Name, validateUrl.ToString().Replace("%3F", "?"));
                        //mailservice.SendRegisterMail(mailBody, pRegisterMember.NewMember.Email);

                        //儲存訊息
                        TempData["RegisterState"] = "註冊成功，請收信驗證Email";

                        return RedirectToAction("RegisterResult");
                    }
                    else
                    {
                        ModelState.AddModelError("MembersImage", "所上傳檔案不是圖片");
                    }

                }
                else
                {
                    ModelState.AddModelError("MembersImage", "請選擇上傳檔案");
                    return View(RegisterMember);
                }
              
            }
            //未驗證則清空密碼欄位
            RegisterMember.Password = null;
            RegisterMember.PasswordCheck = null;

            //資料回填至View
            return View(RegisterMember);
        }

        /// <summary>
        /// 註冊結果顯示頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult RegisterResult()
        {
            return View();
        }

        /// <summary>
        /// 判斷帳號是否已被註冊過
        /// </summary>
        /// <param name="RegisterMemter"></param>
        /// <returns></returns>
        public JsonResult AccountCheck(MembersRegisterViewModel RegisterMemter)
        {
            return Json(membersservice.AccountCheck(RegisterMemter.newMember.Account), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 接收驗證信連結傳進來
        /// </summary>
        /// <param name="Account"></param>
        /// <param name="AuthCode"></param>
        /// <returns></returns>
        public ActionResult EmailValidate(string Account, string AuthCode)
        {
            ViewData["EmailValidate"] = membersservice.EmailValidate(Account, AuthCode);

            return View();

        }
        #endregion

        #region 登入
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Login(MembersLoginViewModels LoginMember)
        {
            string ValidateStr = membersservice.LoginCheck(LoginMember.Account, LoginMember.Password);

            if (string.IsNullOrEmpty(ValidateStr))
            {
                //無錯誤訊息則登入
                //先清空Session
                HttpContext.Session.Clear();
                //取得購物車保存
                string Cart = cartService.GetCartSave(LoginMember.Account);
                //判斷是否有保存，若有則存入Session
                if (Cart != null)
                {
                    HttpContext.Session["Cart"] = Cart;
                }

                string RoleData = membersservice.GetRole(LoginMember.Account);

                //設定JWT
                JwtService jwtService = new JwtService();
                string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
                
                string Token = jwtService.GenerateToken(LoginMember.Account, RoleData);
                //string Token = "123456789";
                HttpCookie cookie = new HttpCookie(cookieName);
                cookie.Value = Server.UrlEncode(Token);
                //寫到用戶端
                Response.Cookies.Add(cookie);
                Response.Cookies["cookieName"].Expires = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"]));


                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", ValidateStr);

                return View(LoginMember);
            }
        }
        #endregion

        #region 登出
        [Authorize]
        public ActionResult Logout()
        {
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
            //清除cookie
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.Values.Clear();
            Response.Cookies.Set(cookie);

            return RedirectToAction("Login");
        }
        #endregion

        #region 變更密碼
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(MembersChangePasswordViewModel ChangeData)
        {
            if (ModelState.IsValid)
            {
                ViewData["ChangeState"] = membersservice.ChangePassword(User.Identity.Name, ChangeData.Password, ChangeData.NewPassword);
            }
            return View();
        }
        #endregion
    }
}