namespace PaintyTest.Data.Dto;

public class ImageDto
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = default!;

    public Guid UserId { get; set; } = default!;
}