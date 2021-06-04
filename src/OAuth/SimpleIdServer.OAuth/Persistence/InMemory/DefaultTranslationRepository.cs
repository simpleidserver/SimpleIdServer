using SimpleIdServer.OAuth.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence.InMemory
{
    public class DefaultTranslationRepository : ITranslationRepository
    {
        private ICollection<OAuthTranslation> _translations;

        public DefaultTranslationRepository(ICollection<OAuthTranslation> translations)
        {
            _translations = translations;
        }

        public Task<IEnumerable<OAuthTranslation>> GetTranslations(IEnumerable<string> translationCodes, string language, CancellationToken cancellationToken)
        {
            return Task.FromResult(_translations.Where(t => translationCodes.Contains(t.Key) && t.Language == language));
        }

        public Task<IEnumerable<OAuthTranslation>> GetTranslations(IEnumerable<string> translationCodes, CancellationToken cancellationToken)
        {
            return Task.FromResult(_translations.Where(t => translationCodes.Contains(t.Key)));
        }
    }
}
