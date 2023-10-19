namespace PaintyTest.API;

public static class AppConfig
{
    private static string _imageStorageDirectoryPath = string.Empty;

    public static string ImageStorageDirectoryPath
    {
        get => _imageStorageDirectoryPath;
        set
        {
            var resolvedPath = Path.GetFullPath(value);
            _imageStorageDirectoryPath = resolvedPath;
        }
    }
}

public static class AppConfigLoadExtensions
{
    public static void LoadAppConfig(this IConfiguration configuration)
    {
        var config = configuration.GetSection("Config");

        AppConfig.ImageStorageDirectoryPath = config["ImageStorageDirectoryPath"]!;
    }
}