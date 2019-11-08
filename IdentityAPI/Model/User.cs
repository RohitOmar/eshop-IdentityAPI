using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAPI.Model
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage ="Fullname can't be empty")]
        public string Fullname { get; set; }

        [Required(ErrorMessage ="Username can't be empty")]
        [MinLength(6,ErrorMessage ="Min lenght is 6")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password can't be empty")]
        [MinLength(8, ErrorMessage = "Min lenght is 8")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage ="Email cannot be empty")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Role { get; set; }

        public string Status { get; set; }

    }
}
