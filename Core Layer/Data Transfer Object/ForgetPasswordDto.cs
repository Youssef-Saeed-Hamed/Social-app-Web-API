using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Data_Transfer_Object
{
    public class ForgetPasswordDto
    {
        [EmailAddress(ErrorMessage = "This Is Not an Email")]
        public string Email { get; set; }
    }
}
