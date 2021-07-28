using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Services
{
    public class ForPaging
    {
        //當前頁數
        public int NowPage { get; set; }

        //最大頁數
        public int MaxPage { get; set; }

        //分頁項目個數
        public int ItemNum
        {
            get
            {
                return 5;
            }
        }

        public ForPaging()
        {
            this.NowPage = 1;
        }

        public ForPaging(int pPage)
        {
            this.NowPage = pPage;
        }

        //設定正確頁數
        public void SetRightPage()
        {
            //判斷當前頁數是否小於1
            if (this.NowPage < 1)
            {
                this.NowPage = 1;
            }
            else if (this.NowPage > this.MaxPage)
            {
                this.NowPage = this.MaxPage;
            }

            if (this.MaxPage.Equals(0))
            {
                this.NowPage = 1;
            }
        }
    }
}