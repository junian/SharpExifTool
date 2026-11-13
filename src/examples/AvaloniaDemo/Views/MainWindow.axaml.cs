using System;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using SharpExifTool;

namespace AvaloniaDemo.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OnSelectImageClicked(object? sender, RoutedEventArgs e)
    {
        try
        {
            SelectedFileText.Text = "Opening file picker...";
            ResultTextBox.Text = string.Empty;

            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select an image",
                AllowMultiple = false,
                FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
            });

            if (files.Count == 0)
            {
                SelectedFileText.Text = "No image selected.";
                return;
            }

            var path = files[0].TryGetLocalPath();

            if (string.IsNullOrEmpty(path))
            {
                SelectedFileText.Text = "The selected file is not a local file.";
                return;
            }

            SelectImageButton.IsEnabled = false;
            BusyIndicator.IsVisible = true;
            SelectedFileText.Text = files[0].Name;
            ResultTextBox.Text = "Reading metadata...";

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

            ResultTextBox.Text = result.ToString();
        }
        catch (Exception ex)
        {
            ResultTextBox.Text = $"Failed to read metadata:{Environment.NewLine}{Environment.NewLine}{ex}";
        }
        finally
        {
            SelectImageButton.IsEnabled = true;
            BusyIndicator.IsVisible = false;
        }
    }
}
