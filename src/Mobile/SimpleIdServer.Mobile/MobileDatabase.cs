using SimpleIdServer.Mobile.Models;
using SQLite;

namespace SimpleIdServer.Mobile
{
    public class MobileDatabase
    {
        private SQLiteAsyncConnection _database;
        private readonly string _dbPath;
        
        private async Task Init()
        {
            if (_database != null) return;


            var secureStorage = SecureStorage.Default;
            var sqlitePassword = await secureStorage.GetAsync("sqlitePassword");
            if(sqlitePassword == null)
            {
                sqlitePassword = Guid.NewGuid().ToString();
                await secureStorage.SetAsync("sqlitePassword", sqlitePassword);
            }

            var options = new SQLiteConnectionString(_dbPath, true, sqlitePassword);
            _database = new SQLiteAsyncConnection(options);
            await _database.CreateTableAsync<CredentialRecord>();
        }

        public MobileDatabase(string dbPath)
        {
            _dbPath = dbPath;
        }

        public async Task<List<CredentialRecord>> GetCredentialRecord()
        {
            await Init();
            return await _database.Table<CredentialRecord>().ToListAsync();
        }

        public async Task AddCredentialRecord(CredentialRecord credentialRecord)
        {
            await Init();
            await _database.InsertAsync(credentialRecord);
        }

        public async Task UpdateCredentialRecord(CredentialRecord credentialRecord)
        {
            await Init();
            await _database.UpdateAsync(credentialRecord);
        }
    }
}
