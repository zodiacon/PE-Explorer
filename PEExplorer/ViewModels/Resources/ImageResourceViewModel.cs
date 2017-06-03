﻿using System.Windows.Media;
using PEExplorer.Core;

namespace PEExplorer.ViewModels.Resources {
    class ImageResourceViewModel : ResourceViewModel {
        public ImageResourceViewModel(ResourceID id, ResourceTypeViewModel type) :
            base(id, type) {
        }

        ImageSource _image;
        public ImageSource Icon => _image ?? (_image = Type.ResourceManager.GetIconImage(ResourceId, true));
        public ImageSource Cursor => _image ?? (_image = Type.ResourceManager.GetIconImage(ResourceId, false));

        public ImageSource Bitmap => _image ?? (_image = Type.ResourceManager.GetBitmapImage(ResourceId));
        public override bool CustomViewPossible => true;

    }
}
