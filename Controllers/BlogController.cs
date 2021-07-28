using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using OnlineShop.Services;
using OnlineShop.ViewModels;

namespace OnlineShop.Controllers
{
    public class BlogController : Controller
    {
        private readonly MembersDBService membersService = new MembersDBService();

        private readonly ArticleDBService articleService = new ArticleDBService();

        private readonly MessageDBService messageService = new MessageDBService();

        // GET: Blog
        public ActionResult Index(string Account)
        {
            BlogViewModel Data = new BlogViewModel();
            Data.Member = membersService.GetDatabyAccount(Account);

            return View(Data);
        }

        #region 文章列表
        public ActionResult ArticleList(string Search, string Account, int Page = 1)
        {
            ArticleIndexViewModel Data = new ArticleIndexViewModel();

            Data.Search = Search;
            Data.Paging = new ForPaging(Page);
            Data.Account = Account;
            Data.DataList = articleService.GetDataList(Data.Paging, Data.Search, Data.Account);

            return PartialView(Data);
        }
        #endregion

        #region 文章頁面
        public ActionResult Article(int A_Id)
        {
            ArticleViewModel Data = new ArticleViewModel();

            articleService.AddWatch(A_Id);
            Data.article = articleService.GetArticleDataById(A_Id);
            ForPaging paging = new ForPaging(0); //確定是否有留言資料
            Data.DataList = messageService.GetDataList(paging, A_Id);

            return View(Data);
        }
        #endregion

        #region 新增文章
        [Authorize]
        public ActionResult CreateArticle()
        {
            return PartialView();
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateArticle([Bind(Include = "Title,Content")]Article Data)
        {
            Data.Account = User.Identity.Name;
            articleService.InsertArticle(Data);
            return RedirectToAction("Index", new { Account = User.Identity.Name });
        }
        #endregion

        #region 修改文章
        [Authorize]
        public ActionResult EditArticle(int A_Id)
        {
            Article Data = new Article();
            Data = articleService.GetArticleDataById(A_Id);
            return PartialView(Data);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditArticle(int A_Id, Article Data)
        {
            //判斷是否可以修改此文章，有回文則不行
            if (articleService.CheckUpdate(A_Id))
            {
                articleService.UpdateArticle(Data);
            }

            return RedirectToAction("Article", new { A_Id = A_Id });
        }
        #endregion

        #region 刪除文章
        [Authorize]
        public ActionResult DeleteArticle(int A_Id)
        {
            articleService.DeleteArticle(A_Id);
            return RedirectToAction("Index", new { Account = User.Identity.Name });
        }
        #endregion

        #region 顯示人氣
        public ActionResult ShowPopularity(string Account)
        {
            ArticleIndexViewModel Data = new ArticleIndexViewModel();

            Data.DataList = articleService.GetPopularList(Account);

            return View(Data);
        }
        #endregion

        #region 留言頁面
        public ActionResult Message(int A_Id = 1)
        {
            ViewData["A_Id"] = A_Id;
            return PartialView();
        }
        #endregion

        #region 留言陣列
        public ActionResult MessageList(int A_Id, int Page = 1)
        {
            MessageViewModel Data = new MessageViewModel();
            Data.Paging = new ForPaging(Page);
            Data.A_Id = A_Id;
            Data.DataList = messageService.GetDataList(Data.Paging, Data.A_Id);
            return PartialView(Data);
        }
        #endregion

        #region 新增留言
        [Authorize]
        public ActionResult CreateMessage(int A_Id)
        {
            ViewData["A_Id"] = A_Id;
            return PartialView();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddMessage(int A_Id, [Bind(Include = "Content")]Message Data)
        {
            Data.A_Id = A_Id;
            Data.Account = User.Identity.Name;

            messageService.InsertMessage(Data);

            return RedirectToAction("MessageList", new { A_Id = A_Id });
        }
        #endregion

        #region 修改留言
        [Authorize]
        public ActionResult UpdateMessage(int A_Id, int M_Id, string Content)
        {
            Message message = new Message();
            message.A_Id = A_Id;
            message.M_Id = M_Id;
            message.Content = Content;
            messageService.UpdateMessage(message);
            //重新導向文章頁面
            return RedirectToAction("Article", new { A_Id = A_Id });
        }
        #endregion

        #region 刪除留言
        [Authorize]
        public ActionResult DeleteMessagee(int A_Id, int M_Id)
        {
            messageService.DeleteMessage(A_Id, M_Id);

            return RedirectToAction("Article", new { A_Id = A_Id });
        }
        #endregion
    }
}