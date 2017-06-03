using Prism.Mvvm;

namespace PEExplorer.ViewModels {

    abstract class TabViewModelBase : BindableBase {
        public abstract string Icon { get; }
        public abstract string Text { get; }

        public virtual bool CanClose => true;

        protected MainViewModel MainViewModel { get; }

        protected TabViewModelBase(MainViewModel vm) {
            MainViewModel = vm;
        }

		public string ToDecHex(ulong n) => $"{n} (0x{n:X})";

	}

}
