// <copyright file="AccountViewModels.Windows.cs" author="Mohammad Younes">
// Copyright 2013 Mohammad Younes.
// 
// Released under the MIT license
// http://opensource.org/licenses/MIT
//
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MixedAuth.Models
{
    public class WindowsLoginConfirmationViewModel
    {
        [Required]
		[EmailAddress]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

}