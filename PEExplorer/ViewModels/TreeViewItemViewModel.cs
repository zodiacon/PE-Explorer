using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace PEExplorer.ViewModels {

    class TreeViewItemViewModel : BindableBase {
        protected MainViewModel MainViewModel { get; }
        public TreeViewItemViewModel(MainViewModel vm) {
            MainViewModel = vm;
        }
        private string _text;

        public string Text {
            get => _text ?? Tab.Text;
            set => SetProperty(ref _text, value);
        }

        private string _icon;

        public string Icon {
            get => _icon ?? Tab.Icon;
            set => SetProperty(ref _icon, value);
        }

        public TabViewModelBase Tab { get; set; }

        ObservableCollection<TreeViewItemViewModel> _items;

        public IList<TreeViewItemViewModel> Items => _items ?? (_items = new ObservableCollection<TreeViewItemViewModel>());

        private bool _isExpanded;

        public bool IsExpanded {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

    }
}
