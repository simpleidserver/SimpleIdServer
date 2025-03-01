using SimpleIdServer.IdServer.Store.EF;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Startup.Conf.Migrations;

public class DataSeeder<T> where T : StoreDbContext
{
    public DataSeeder(T dbContext)
    {
        DbContext = dbContext;
    }

    protected T DbContext { get; private set; }

    public async Task Apply()
    {
        // Ensure the table exists.
        // Call the UP.
        // Insert the record.
    }
}
