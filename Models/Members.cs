﻿using System;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OnlineShop.CustomAttribute;

namespace OnlineShop.Models
{
    public class Members
    {
        [DisplayName("帳號")]
        [Required(ErrorMessage = "請輸入帳號")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "帳號長度需介於6-30字元")]
        [Remote("AccountCheck", "Members", ErrorMessage = "此帳號已被註冊過")]
        public string Account { get; set; }

        //密碼
        public string Password { get; set; }

        [DisplayName("姓名")]
        [Required(ErrorMessage = "請輸入姓名")]
        [StringLength(20, ErrorMessage = "姓名不可超過20字元")]
        [ChineseAttributecs("^[\u4e00-\u9fa5]+$", ErrorMessage = "請輸入中文")]
        public string Name { get; set; }

        [DisplayName("大頭照")]
        public string Image { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "請輸入Email")]
        [StringLength(200, ErrorMessage = "Email長度不可超過200字元")]
        [EmailAddress(ErrorMessage = "這不是Email格式")]
        public string Email { get; set; }

        //信箱驗證碼
        public string AuthCode { get; set; }

        //管理者
        public bool IsAdmin { get; set; }
    }
}