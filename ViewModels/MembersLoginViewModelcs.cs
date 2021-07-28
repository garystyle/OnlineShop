using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.ViewModels
{
    public class MembersLoginViewModels
    {
        [DisplayName("會員帳號")]
        [Required(ErrorMessage = "請輸入會員帳號")]
        public string Account { get; set; }

        [DisplayName("會員密碼")]
        [Required(ErrorMessage = "請輸入密碼")]
        public string Password { get; set; }
    }
}