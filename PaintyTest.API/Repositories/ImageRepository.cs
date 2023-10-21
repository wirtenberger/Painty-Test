using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PaintyTest.API.Contracts.Repositories;
using PaintyTest.Data;
using PaintyTest.Data.Entities;

namespace PaintyTest.API.Repositories;

public class ImageRepository : IImageRepository
{
    private readonly AppDbContext _appDbContext;

    private DbSet<Image> ImageSet => _appDbContext.Images;

    public ImageRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public Task<List<Image>> GetAllAsync(Expression<Func<Image, bool>>? filter = null)
    {
        if (filter == null)
        {
            return ImageSet.ToListAsync();
        }

        return ImageSet.Where(filter).ToListAsync();
    }

    public async Task<Image?> GetByIdAsync(Guid id)
    {
        return await ImageSet.FindAsync(id);
    }

    public async Task<Image> CreateAsync(Image image)
    {
        var addedImage = await ImageSet.AddAsync(image);
        return addedImage.Entity;
    }

    public Task<Image> UpdateAsync(Image image)
    {
        return Task.FromResult(ImageSet.Update(image).Entity);
    }

    public async Task<Image> DeleteAsync(Guid id)
    {
        var image = (await GetByIdAsync(id))!;
        return ImageSet.Remove(image).Entity;
    }
}