using System.Collections.Generic;
using UseUMAToProtectAPI.Api.Domains;

namespace UseUMAToProtectAPI.Api.Persistence
{
    public interface IPictureRepository
    {
        void AddPicture(Picture picture);
        Picture FindPictureByIdentifier(string id);
        IEnumerable<Picture> FindByCreator(string userIdentifier);
        IEnumerable<Picture> FindAllExceptUser(string userIdentifier);
    }
}
