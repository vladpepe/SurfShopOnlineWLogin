//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OnlineShoppingStore.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Cryptography;
    using System.Text;

    public partial class Tbl_Admin
    {
        [Required(ErrorMessage = "Please insert your username")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The username is not valid")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please insert your password")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The password is not valid")]
        public string Password { get; set; }
        public int ID { get; set; }


        //Method to encrypt a string using MD5 
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
