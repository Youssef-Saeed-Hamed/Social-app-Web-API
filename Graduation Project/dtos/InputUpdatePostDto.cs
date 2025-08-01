using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.dtos
{
    public class InputUpdatePostDto
    {
        [Required]
        public string NewContent { get; set; }
        public IFormFile? Image {  get; set; }

    }
}
