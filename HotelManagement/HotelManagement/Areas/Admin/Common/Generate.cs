using Microsoft.AspNetCore.Identity;
using System.Text;
using System;
using HotelManagement.Models;
using HotelManagement.Models.Common;

namespace HotelManagement.Areas.Admin.Common
{
    public static class Generate
    {
        private static readonly PasswordHasher<Account> _paswordHasher = new PasswordHasher<Account>();
        public static Dictionary<string, string> _account = new Dictionary<string, string>();

        /// <summary>
        /// Generate Account
        /// </summary>
        /// <param name="_name"></param>
        /// <returns>Account</returns>
        public static Account GenerateAccount(string _name, AccountType _type)
        {
            // Generate username
            List<string> names = new List<string>();
            foreach (string n in _name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                names.Add(n);
            }

            string userName = names.Last().ToLower();
            for (int i = 0; i < names.Count - 1; i++)
            {
                userName += names[i].Substring(0, 1).ToLower();
            }

            userName += new Random().Next(100).ToString("D2");

            // Generate password
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string specialChars = "!@#$%^&*()-_=+[]{};:,.<>?";

            // Kết hợp tất cả các loại ký tự
            string allChars = uppercaseChars + lowercaseChars + numbers + specialChars;
            int length = 10;

            // Tạo mật khẩu
            StringBuilder password = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                // Chọn một ký tự ngẫu nhiên từ tập hợp
                int index = new Random().Next(allChars.Length);
                password.Append(allChars[index]);
            }
            _account[userName] = password.ToString();

            return new Account { Username = userName, Password = _paswordHasher.HashPassword(null, password.ToString()), Type = _type, Active = false };
        }
    }
}
