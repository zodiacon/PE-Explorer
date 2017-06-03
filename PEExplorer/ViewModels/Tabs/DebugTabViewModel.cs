using System.ComponentModel.Composition;


namespace PEExplorer.ViewModels.Tabs {

	[Export, PartCreationPolicy(CreationPolicy.NonShared)]
	class DebugTabViewModel : TabViewModelBase {
		[ImportingConstructor]
		public DebugTabViewModel(MainViewModel vm) : base(vm) {
		}

		public override string Icon => "/icons/debug.ico";

		public override string Text => "Debug";
	}
}
