namespace MauiDemo
{
    public class AppShell : Shell
    {
        public AppShell()
        {
            Title = "MauiDemo";

            Items.Add(new ShellContent
            {
                Title = "Home",
                Route = nameof(MainPage),
                ContentTemplate = new DataTemplate(typeof(MainPage))
            });
        }
    }
}
