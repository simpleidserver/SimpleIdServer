using FormBuilder.Models;
using FormBuilder.Stores;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.EF.Stores;

public class FormStore : IFormStore
{
    private readonly FormBuilderDbContext _dbContext;

    public FormStore(FormBuilderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<FormRecord> Get(string id, CancellationToken cancellationToken)
        => _dbContext.Forms.Include(f => f.AvailableStyles).SingleOrDefaultAsync(f => f.Id == id, cancellationToken);

    public Task<List<FormRecord>> GetAll(CancellationToken cancellationToken)
        => _dbContext.Forms.ToListAsync(cancellationToken);

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
