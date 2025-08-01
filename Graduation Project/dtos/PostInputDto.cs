using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.dtos
{
    public class PostInputDto
    {
        [Required]
        [FromForm(Name = "Content")]
        public string Content {  get; set; }
        [FromForm(Name = "Image")]
        public IFormFile? Image { get; set; }
    }
}
