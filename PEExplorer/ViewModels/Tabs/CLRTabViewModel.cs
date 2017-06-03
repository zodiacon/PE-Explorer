using System.ComponentModel.Composition;

namespace PEExplorer.ViewModels.Tabs {

    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    class CLRTabViewModel : TabViewModelBase {
        [ImportingConstructor]
        public CLRTabViewModel(MainViewModel vm) : base(vm) {
        }

        public override string Icon => "/icons/cpu.ico";

        public override string Text => "CLR";
    }

}
