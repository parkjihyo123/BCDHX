using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BCDHX.Models
{
    public class UserLoginTemp
    {
        public string Username { get; set; }
        public string UserId { get; set; }
        public string Address { get; set; }
    }
    public class TempUserPaymentForAccount
    {
        public string Id { get; set; }
        public decimal price { get; set; }
        public string UserId { get; set; }
        public DateTime PaymentDate { get; set; }
        public int Status { get; set; }
        public string PaymentType { get; set; }
        public string NameAccount { get; set; }
    }

    public class UserViewModel
    {
        public decimal _amount { get; set; }
        public string ID_Account { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Access { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string Amount
        {
            get
            {
                return string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", this._amount);
            }
            set
            {
                this._amount = Convert.ToDecimal(value);
            }
        }
        public string Img { get; set; }
    }
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(250)]
        public string Password { get; set; }

        public int? Access { get; set; }

        [StringLength(250)]
        public string Fullname { get; set; }

        public string Address { get; set; }
        public string ReturnLink { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public string ReturnLink { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
