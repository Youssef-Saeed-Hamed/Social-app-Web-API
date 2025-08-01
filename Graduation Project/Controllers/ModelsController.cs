using Core_Layer.Data_Transfer_Object;
using Core_Layer.Inetrfaces.Services;
using Graduation_Project.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Graduation_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : ControllerBase
    {
        private readonly IModelsAiService _aiServices;

        public ModelsController(IModelsAiService aiServices)
        {
            _aiServices = aiServices;
        }
        [HttpPost]
        public  async Task<ActionResult<ReturnOcrDto>> GetOcrText([FromForm]ImageInputDto dto) 
        {
            using var ms = new MemoryStream();
            await dto.Image.CopyToAsync(ms);
            var imageBytes = ms.ToArray();
            string text = await _aiServices.ExtractTextFromImageAsync(imageBytes, dto.Image.FileName);
            return Ok(new ReturnOcrDto { text = text });
        }
    }
}
