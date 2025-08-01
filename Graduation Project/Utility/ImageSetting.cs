namespace Graduation_Project.Utility
{
    public static class ImageSetting
    {
        public static string Upload (IFormFile file)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Files/Images");

            var fileName = $"{Guid.NewGuid()}-{file.FileName}";

            var filePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(filePath , FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            return Path.Combine(@"Files/Images", fileName);
        }

        public static void Delete(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/Files/Images", fileName);
            if(File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
