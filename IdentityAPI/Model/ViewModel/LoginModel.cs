using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAPI.Model
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username can't be empty")]
        [MinLength(6, ErrorMessage = "Min lenght is 6")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password can't be empty")]
        [MinLength(8, ErrorMessage = "Min lenght is 8")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
