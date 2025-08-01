using Core_Layer.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Inetrfaces.Services
{
    public interface IModelsAiService
    {
        Task<string> AnalyzeTextAsync(string input , AppUser user );
        Task<string> ExtractTextFromImageAsync(byte[] imageBytes, string fileName);

    }
}
