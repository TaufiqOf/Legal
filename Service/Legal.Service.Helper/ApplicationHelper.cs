using System.Reflection;

namespace Legal.Service.Helper;

public static class ApplicationHelper
{
    public enum ModuleName
    {
        ADMIN = 0,
        SHOP = 1,
        CHAT = 2,
    }

    public static string ApplicationName { get; set; } = default!;

    public static string ApplicationVersion { get; set; } = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
}