using System.Linq.Expressions;
using PaintyTest.Data.Entities;

namespace PaintyTest.Contracts.Repositories;

public interface IImageRepository
{
    Task<List<Image>> GetAllAsync(Expression<Func<Image, bool>>? filter);
    Task<Image?> GetByIdAsync(Guid id);
    Task<Image> CreateAsync(Image image);
    Task<Image> UpdateAsync(Image image);
    Task<Image> DeleteAsync(Guid id);
}