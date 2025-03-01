namespace Graduation_Project.Utility
{
    public static class ImageSetting
    {
        public static string Upload (IFormFile file)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), @"Files/Images");

            var fileName = $"{Guid.NewGuid()}-{file.FileName}";

            var filePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(filePath , FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            return filePath;
        }

        public static void Delete(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"Files/Images", fileName);
            if(File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
