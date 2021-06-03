using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Jwt;
using SimpleIdServer.OAuth.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.EF.Repositories
{
    public class JsonWebKeyRepository : IJsonWebKeyRepository
    {
        private readonly OpenIdDBContext _dbContext;

        public JsonWebKeyRepository(OpenIdDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<JsonWebKey> FindJsonWebKeyById(string kid, CancellationToken cancellationToken)
        {
            return _dbContext.JsonWebKeys.FirstOrDefaultAsync(j => j.Kid == kid, cancellationToken);
        }

        public async Task<List<JsonWebKey>> FindJsonWebKeys(Usages usage, string alg, KeyOperations[] operations, CancellationToken cancellationToken)
        {
            var currentDateTime = DateTime.UtcNow;
            int nbOperations = operations.Count();
            var result = await GetJsonWebKeys().Where(j =>
                (j.ExpirationDateTime == null || currentDateTime < j.ExpirationDateTime) &&
                (j.Use == usage && j.Alg == alg && j.KeyOperationLst.Where(k => operations.Contains(k.Operation)).Count() == nbOperations)
            ).ToListAsync(cancellationToken);
            return result;
        }

        public Task<List<JsonWebKey>> GetActiveJsonWebKeys(CancellationToken cancellationToken)
        {
            var currentDateTime = DateTime.UtcNow;
            return GetJsonWebKeys().Where(j => j.ExpirationDateTime == null || currentDateTime < j.ExpirationDateTime)
                .ToListAsync(cancellationToken);
        }

        public Task<List<JsonWebKey>> GetNotRotatedJsonWebKeys(CancellationToken cancellationToken)
        {
            return GetJsonWebKeys().Where(j => string.IsNullOrWhiteSpace(j.RotationJWKId)).ToListAsync(cancellationToken);
        }

        private IQueryable<JsonWebKey> GetJsonWebKeys()
        {
            return _dbContext.JsonWebKeys.Include(j => j.KeyOperationLst);
        }

        public Task<bool> Add(JsonWebKey data, CancellationToken token)
        {
            _dbContext.JsonWebKeys.Add(data);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(JsonWebKey data, CancellationToken token)
        {
            _dbContext.JsonWebKeys.Remove(data);
            return Task.FromResult(true);
        }

        public Task<bool> Update(JsonWebKey data, CancellationToken token)
        {
            _dbContext.JsonWebKeys.Update(data);
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken token)
        {
            return _dbContext.SaveChangesAsync(token);
        }

        public void Dispose()
        {
        }
    }
}
