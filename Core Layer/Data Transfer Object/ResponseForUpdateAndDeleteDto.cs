using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Data_Transfer_Object
{
    public class ResponseForUpdateAndDeleteDto
    {
        public Response Response {  get; set; }
        public string? imagePath { get; set; }
    }
}
