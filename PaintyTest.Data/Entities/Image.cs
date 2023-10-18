using System.ComponentModel.DataAnnotations.Schema;

namespace PaintyTest.Data.Entities;

public class Image
{
    public Guid Id { get; set; } = default!;

    public string FileName { get; set; } = default!;

    /// <summary>
    /// Путь относительно директории хранилища
    /// </summary>
    public string RelativePath { get; set; } = default!;

    public Guid UserId { get; set; } = default!;

    public User User { get; set; } = default!;
}