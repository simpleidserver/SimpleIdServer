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
            await _database.CreateTableAsync<OTPCode>();
            await _database.CreateTableAsync<VerifiableCredentialRecord>();
            await _database.CreateTableAsync<DidRecord>();
            var result = await _database.CreateTableAsync<MobileSettings>();
            if(result == CreateTableResult.Created) await _database.InsertAsync(new MobileSettings { Id = Guid.NewGuid().ToString() });
        }

        public MobileDatabase(string dbPath)
        {
            _dbPath = dbPath;
        }

        public async Task<DidRecord> GetDidRecord()
        {
            await Init();
            var result = await _database.Table<DidRecord>().FirstOrDefaultAsync();
            return result;
        }

        public async Task AddDidRecord(DidRecord didRecord)
        {
            await Init();
            await _database.InsertAsync(didRecord);
        }

        public async Task<List<VerifiableCredentialRecord>> GetVerifiableCredentials()
        {
            await Init();
            return await _database.Table<VerifiableCredentialRecord>().ToListAsync();
        }

        public async Task AddVerifiableCredential(VerifiableCredentialRecord verifiableCredential)
        {
            await Init();
            await _database.InsertAsync(verifiableCredential);
        }

        public async Task RemoveVerifiableCredential(VerifiableCredentialRecord verifiableCredential)
        {
            await Init();
            await _database.DeleteAsync(verifiableCredential);
        }

        public async Task<List<OTPCode>> GetOTPCodes()
        {
            await Init();
            return await _database.Table<OTPCode>().ToListAsync();
        }

        public async Task<List<CredentialRecord>> GetCredentialRecords()
        {
            await Init();
            return await _database.Table<CredentialRecord>().ToListAsync();
        }

        public async Task RemoveCredentialRecord(CredentialRecord credentialRecord)
        {
            await _database.DeleteAsync(credentialRecord);
        }

        public async Task<MobileSettings> GetMobileSettings()
        {
            await Init();
            var result =  await _database.Table<MobileSettings>().FirstOrDefaultAsync();
            return result ?? new MobileSettings();
        }

        public async Task AddCredentialRecord(CredentialRecord credentialRecord)
        {
            await Init();
            await _database.InsertAsync(credentialRecord);
        }

        public async Task AddOTPCode(OTPCode otpCode)
        {
            await Init();
            await _database.InsertAsync(otpCode);
        }

        public async Task RemoveOTPCode(OTPCode otpCode)
        {
            await Init();
            await _database.DeleteAsync(otpCode);
        }

        public async Task UpdateCredentialRecord(CredentialRecord credentialRecord)
        {
            await Init();
            await _database.UpdateAsync(credentialRecord);
        }

        public async Task UpdateMobileSettings(MobileSettings mobileSettings)
        {
            await Init();
            await _database.UpdateAsync(mobileSettings);
        }

        public async Task UpdateOTPCode(OTPCode otpCode)
        {
            await Init();
            await _database.UpdateAsync(otpCode);
        }
    }
}
