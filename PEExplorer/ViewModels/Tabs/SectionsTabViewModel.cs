﻿using PEExplorer.Core;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace PEExplorer.ViewModels.Tabs {
	[Export, PartCreationPolicy(CreationPolicy.NonShared)]
	class SectionsTabViewModel : TabViewModelBase {

		[ImportingConstructor]
		public SectionsTabViewModel(MainViewModel vm) : base(vm) {
		}

		public override string Icon => "/icons/sections.ico";
		public override string Text => "Sections";

		ICollection<SectionData> _sections;
		public ICollection<SectionData> Sections => _sections ?? (_sections = MainViewModel.PEParser.GetSectionHeaders());

	}
}
