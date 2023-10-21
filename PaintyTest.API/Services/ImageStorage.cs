namespace PaintyTest.API.Services;

public static class ImageStorage
{
    public static string ImageStorageDirectoryPath => AppConfig.ImageStorageDirectoryPath;

    public static bool DirectoryExists => Directory.Exists(ImageStorageDirectoryPath);

    public static void CreateDirectoryIfNotExists()
    {
        if (!DirectoryExists)
        {
            Directory.CreateDirectory(ImageStorageDirectoryPath);
        }
    }

    public static bool ImagaeFileExists(string imagePath, string? subdirectory = null)
    {
        return Path.Exists(Path.Join(ImageStorageDirectoryPath, subdirectory ?? "", imagePath));
    }

    /// <summary>
    /// </summary>
    /// <param name="imagePath">Путь до файла относительно директории хранилища</param>
    /// <returns></returns>
    public static FileStream GetImageFile(string imagePath)
    {
        var filePath = Path.Join(ImageStorageDirectoryPath, imagePath);
        return File.Open(filePath, FileMode.Open);
    }

    /// <summary>
    /// </summary>
    /// <param name="imagePath">Путь до файла относительно директории хранилища</param>
    /// <returns></returns>
    public static FileStream CreateImageFile(string imagePath, string? subdirectory = null)
    {
        var filePath = "";
        if (subdirectory is not null)
        {
            var subDirPath = Path.Join(ImageStorageDirectoryPath, subdirectory);
            Directory.CreateDirectory(subDirPath);
            filePath = Path.Join(subDirPath, imagePath);
        }
        else
        {
            filePath = Path.Join(ImageStorageDirectoryPath, imagePath);
        }

        return File.Create(filePath);
    }

    /// <summary>
    /// </summary>
    /// <param name="imagePath">Путь до файла относительно директории хранилища</param>
    /// <returns></returns>
    public static void DeleteImageFile(string imagePath)
    {
        var fullPath = Path.Join(ImageStorageDirectoryPath, imagePath);
        if (!Path.Exists(fullPath))
        {
            throw new FileNotFoundException();
        }

        File.Delete(fullPath);
    }
}