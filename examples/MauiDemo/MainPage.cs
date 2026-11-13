using System.Text;
using SharpExifTool;

namespace MauiDemo
{
    public class MainPage : ContentPage
    {
        readonly Button _selectImageButton;
        readonly Label _selectedFileLabel;
        readonly Editor _resultEditor;
        readonly ActivityIndicator _busyIndicator;

        public MainPage()
        {
            Title = "ExifTool Demo";

            var title = new Label
            {
                Text = "ExifTool Metadata Viewer",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            _selectImageButton = new Button
            {
                Text = "Select Image",
                HorizontalOptions = LayoutOptions.Fill
            };
            _selectImageButton.Clicked += OnSelectImageClicked;

            _selectedFileLabel = new Label
            {
                Text = "No image selected.",
                LineBreakMode = LineBreakMode.MiddleTruncation
            };

            _resultEditor = new Editor
            {
                IsReadOnly = true,
                AutoSize = EditorAutoSizeOption.Disabled,
                Placeholder = "ExifTool results will appear here."
            };

            _busyIndicator = new ActivityIndicator
            {
                HorizontalOptions = LayoutOptions.Center,
                IsRunning = false
            };

            var resultBorder = new Border
            {
                Padding = new Thickness(8),
                Stroke = Colors.LightGray,
                StrokeThickness = 1,
                Content = _resultEditor
            };

            var layout = new Grid
            {
                Padding = new Thickness(24),
                RowSpacing = 12,
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto)
                },
                Children =
                {
                    title,
                    _selectImageButton,
                    _selectedFileLabel,
                    resultBorder,
                    _busyIndicator
                }
            };

            Grid.SetRow(title, 0);
            Grid.SetRow(_selectImageButton, 1);
            Grid.SetRow(_selectedFileLabel, 2);
            Grid.SetRow(resultBorder, 3);
            Grid.SetRow(_busyIndicator, 4);

            Content = layout;
        }

        async void OnSelectImageClicked(object? sender, EventArgs e)
        {
            try
            {
                _selectedFileLabel.Text = "Opening file picker...";
                _resultEditor.Text = string.Empty;

                var file = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select an image",
                    FileTypes = FilePickerFileType.Images
                });

                if (file is null)
                {
                    _selectedFileLabel.Text = "No image selected.";
                    return;
                }

                _selectImageButton.IsEnabled = false;
                _busyIndicator.IsRunning = true;
                _selectedFileLabel.Text = file.FileName;
                _resultEditor.Text = "Reading metadata...";

                var metadata = await Task.Run(() =>
                {
                    using var exifTool = new ExifTool();
                    return exifTool.ExtractAllMetadata(file.FullPath, "-a", "-u");
                });

                var result = new StringBuilder();
                result.AppendLine($"File: {file.FullPath}");
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

                _resultEditor.Text = result.ToString();
            }
            catch (Exception ex)
            {
                _resultEditor.Text = $"Failed to read metadata:{Environment.NewLine}{Environment.NewLine}{ex}";
            }
            finally
            {
                _busyIndicator.IsRunning = false;
                _selectImageButton.IsEnabled = true;
            }
        }
    }
}
