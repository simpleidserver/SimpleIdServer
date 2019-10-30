using System.Collections.Generic;
using System.Linq;
using UseUMAToProtectAPI.Api.Domains;

namespace UseUMAToProtectAPI.Api.Persistence
{
    public class PictureRepository : IPictureRepository
    {
        private readonly List<Picture> _pictures;

        public PictureRepository()
        {
            _pictures = new List<Picture>();
        }

        public void AddPicture(Picture picture)
        {
            _pictures.Add(picture);
        }

        public IEnumerable<Picture> FindAllExceptUser(string userIdentifier)
        {
            return _pictures.Where(p => p.UserId != userIdentifier);
        }

        public IEnumerable<Picture> FindByCreator(string userIdentifier)
        {
            return _pictures.Where(p => p.UserId == userIdentifier);
        }

        public Picture FindPictureByIdentifier(string id)
        {
            return _pictures.FirstOrDefault(p => p.Identifier == id);
        }
    }
}