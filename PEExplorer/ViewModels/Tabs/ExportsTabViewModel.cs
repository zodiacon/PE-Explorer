﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Data;
using System.Windows.Input;
using PEExplorer.Core;
using PEExplorer.Views;
using Prism.Commands;
using Zodiacon.WPF;

namespace PEExplorer.ViewModels.Tabs {
	[Export, PartCreationPolicy(CreationPolicy.NonShared)]
	class ExportsTabViewModel : TabViewModelBase {
		[ImportingConstructor]
		public ExportsTabViewModel(MainViewModel mainViewModel) : base(mainViewModel) {
			DisassembleCommand = new DelegateCommand(() => {
				var symbol = SelectedItem;
				var vm = DialogService.CreateDialog<DisassemblyViewModel, DisassemblyView>(symbol.Name);
				var address = (int)symbol.Address;
				MainViewModel.Accessor.ReadArray(MainViewModel.PEHeader.RvaToFileOffset(address), _bytes, 0, _bytes.Length);
				vm.Disassemble(_bytes, address, MainViewModel.PEHeader.IsPE64);
				vm.Show();
			}, () => SelectedItem != null && string.IsNullOrEmpty(SelectedItem.ForwardName)).ObservesProperty(() => SelectedItem);
		}

		public override string Icon => "/icons/export1.ico";

		public override string Text => "Exports";

		IEnumerable<ExportedSymbol> _exports;

		public unsafe IEnumerable<ExportedSymbol> Exports {
			get {
				if(_exports == null) {
					_exports = MainViewModel.PEParser.GetExports();
				}
				return _exports;
			}
		}

		private string _searchText;

		public string SearchText {
			get { return _searchText; }
			set {
				if(SetProperty(ref _searchText, value)) {
					var view = CollectionViewSource.GetDefaultView(Exports);
					if(string.IsNullOrWhiteSpace(value))
						view.Filter = null;
					else {
						var lower = value.ToLower();
						view.Filter = o => {
							var symbol = (ExportedSymbol)o;
							return symbol.Name.ToLower().Contains(lower) || (symbol.ForwardName != null && symbol.ForwardName.ToLower().Contains(lower));
						};
					}
				}
			}
		}

		[Import]
		IDialogService DialogService;

		static byte[] _bytes = new byte[1 << 12];

		public ICommand DisassembleCommand { get; }

		private ExportedSymbol _selectedItem;

		public ExportedSymbol SelectedItem {
			get => _selectedItem;
		    set => SetProperty(ref _selectedItem, value);
		}

	}
}
