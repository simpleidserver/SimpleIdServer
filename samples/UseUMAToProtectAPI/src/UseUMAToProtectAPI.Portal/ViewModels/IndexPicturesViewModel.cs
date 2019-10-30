using System.Collections.Generic;

namespace UseUMAToProtectAPI.Portal.ViewModels
{
    public class IndexPicturesViewModel
    {
        public IndexPicturesViewModel(ICollection<string> myPictures, ICollection<string> otherPictures)
        {
            MyPictures = myPictures;
            OtherPictures = otherPictures;
        }

        public ICollection<string> MyPictures { get; set; }
        public ICollection<string> OtherPictures { get; set; }
    }
}
