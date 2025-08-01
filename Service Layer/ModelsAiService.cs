using Core_Layer.Data_Transfer_Object;
using Core_Layer.Entities.Identity;
using Core_Layer.Inetrfaces.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service_Layer
{
    public class ModelsAiService : IModelsAiService
    {
        private readonly HttpClient _httpClient;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ModelsAiService(HttpClient httpClient, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://127.0.0.1:8000");
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string> AnalyzeTextAsync(string input , AppUser user)
        {
            var requestBody = new { sentence = input };
            _httpClient.DefaultRequestHeaders.Authorization = null;

            var response = await _httpClient.PostAsJsonAsync("/predict", requestBody);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PredictionResponseDto>();

            if (result.Prediction == "Bullying")
            {
                if(user.counterOfBullying == 1)
                {
                    user.PunishedUntil = DateTime.UtcNow.AddMinutes(2);
                    user.counterOfBullying = 5;

                }
                else
                {
                    user.counterOfBullying -= 1;
                }
                await _userManager.UpdateAsync(user);

            }

            return result.Prediction;
        }

        public async Task<string> ExtractTextFromImageAsync(byte[] imageBytes, string fileName)
        {
            using var form = new MultipartFormDataContent();
            form.Add(new ByteArrayContent(imageBytes), "file", fileName);

            var response = await _httpClient.PostAsync("NewOcr", form);
            if (!response.IsSuccessStatusCode)
                return "";

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("text").GetString() ?? "";
        }
    }
}
