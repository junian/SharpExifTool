using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharpExifTool;

namespace WebDemo.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public IFormFile? Image { get; set; }

        public string? Result { get; private set; }

        public string? ErrorMessage { get; private set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Image is null || Image.Length == 0)
            {
                ErrorMessage = "Please select an image.";
                return Page();
            }

            var extension = Path.GetExtension(Image.FileName);
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}{extension}");

            try
            {
                await using (var stream = System.IO.File.Create(tempPath))
                {
                    await Image.CopyToAsync(stream);
                }

                using var exifTool = new ExifTool();
                var metadata = await exifTool.ExtractAllMetadataAsync(tempPath, "-a", "-u");

                var result = new StringBuilder();
                result.AppendLine($"File: {Image.FileName}");
                result.AppendLine();

                if (metadata.Count == 0)
                {
                    result.AppendLine("No metadata found.");
                }
                else
                {
                    foreach (var item in metadata)
                    {
                        result.AppendLine($"{item.Key}: {item.Value}");
                    }
                }

                Result = result.ToString();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
            }
            finally
            {
                try
                {
                    System.IO.File.Delete(tempPath);
                }
                catch
                {
                    // Ignore cleanup errors.
                }
            }

            return Page();
        }
    }
}
