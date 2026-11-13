using System.Text;
using SharpExifTool;

namespace WinFormsDemo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private async void OnSelectImageClicked(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Select an image",
                Filter = "Image files|*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.bmp;*.gif;*.webp;*.heic;*.dng;*.cr2;*.cr3;*.nef;*.arw|All files|*.*",
                CheckFileExists = true
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            var path = dialog.FileName;

            try
            {
                selectImageButton.Enabled = false;
                selectedFileLabel.Text = Path.GetFileName(path);
                resultTextBox.Text = "Reading metadata...";

                var metadata = await Task.Run(() =>
                {
                    using var exifTool = new ExifTool();
                    return exifTool.ExtractAllMetadata(path, "-a", "-u");
                });

                var result = new StringBuilder();
                result.AppendLine($"File: {path}");
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

                resultTextBox.Text = result.ToString();
            }
            catch (Exception ex)
            {
                resultTextBox.Text = $"Failed to read metadata:{Environment.NewLine}{Environment.NewLine}{ex}";
            }
            finally
            {
                selectImageButton.Enabled = true;
            }
        }
    }
}
