using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OnlineShop.CustomAttribute
{
    public class ChineseAttributecs : ValidationAttribute
    {
        private string _format = string.Empty;

        private bool result = false;

        public ChineseAttributecs(string format)
        {
            _format = format;
        }

        public override bool IsValid(object value)
        {
            if (value == null) return false;

            Match match = Regex.Match((string)value, _format);

            result = match.Success ? true : false;

            return result;
        }
    }
}