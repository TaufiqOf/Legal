namespace Legal.Service.Helper;

public static class DirectoryHelper
{
    public static string GetStoragePath(string? folder = null)
    {
        var result = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        result = Path.Combine(result, ApplicationHelper.ApplicationName);

        if (!Directory.Exists(result))
        {
            Directory.CreateDirectory(result);
        }

        if (!string.IsNullOrEmpty(folder))
        {
            result = Path.Combine(result, folder);
        }

        if (!Directory.Exists(result))
        {
            Directory.CreateDirectory(result);
        }

        return result;
    }

    public static string GetApplicationTemporaryPath(string? folder = null)
    {
        if (string.IsNullOrEmpty(folder))
        {
            return GetTemporaryPath(ApplicationHelper.ApplicationName);
        }

        return GetTemporaryPath(Path.Combine(ApplicationHelper.ApplicationName, folder));
    }

    public static string GetTemporaryPath(string? folder = null)
    {
        if (string.IsNullOrEmpty(folder))
        {
            return Path.GetTempPath();
        }

        var result = Path.Combine(Path.GetTempPath(), folder);
        if (!Directory.Exists(result))
        {
            Directory.CreateDirectory(result);
        }

        return result;
    }
}