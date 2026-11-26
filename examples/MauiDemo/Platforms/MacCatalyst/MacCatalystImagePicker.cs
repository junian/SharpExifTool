using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using UIKit;
using UniformTypeIdentifiers;

namespace MauiDemo
{
    /// <summary>
    /// Mac Catalyst file picker that doesn't rely on MAUI's <see cref="FilePicker"/>,
    /// which can return <see langword="null"/> after a file has been selected.
    /// </summary>
    static class MacCatalystImagePicker
    {
        public static Task<FileResult?> PickAsync()
        {
            var tcs = new TaskCompletionSource<FileResult?>();

            // asCopy: true makes the system copy the selected file into the app's
            // container, so no security-scoped bookmark handling is needed.
            var picker = new UIDocumentPickerViewController(new[] { UTType.CreateFromIdentifier("public.image")! }, asCopy: true)
            {
                AllowsMultipleSelection = false
            };

            picker.DidPickDocumentAtUrls += (_, e) =>
            {
                var url = e.Urls.FirstOrDefault();
                var path = url?.Path;

                tcs.TrySetResult(string.IsNullOrEmpty(path) ? null : new FileResult(path));
            };

            picker.WasCancelled += (_, _) => tcs.TrySetResult(null);

            var viewController = Platform.GetCurrentUIViewController()
                ?? throw new InvalidOperationException("Unable to find the current view controller.");
            viewController.PresentViewController(picker, true, null);

            return tcs.Task;
        }
    }
}
