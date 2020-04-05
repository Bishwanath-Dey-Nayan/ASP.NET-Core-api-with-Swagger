using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrgAPI.ViewModel
{
    public class Registerviewmodel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string password { get; set; }

        [Required]
        [Compare("password")]
        public string ConfirmPassword { get; set; }
    }
}
