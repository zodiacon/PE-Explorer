﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Diagnostics.Runtime.Utilities;
using PEExplorer.Core;
using PEExplorer.ViewModels.Tabs;
using Prism.Commands;
using Prism.Mvvm;
using Zodiacon.WPF;

namespace PEExplorer.ViewModels {
    [Export]
    class MainViewModel : BindableBase {
        public string Title => PathName == null ? null : $"PE Explorer ({PathName})";

        ObservableCollection<TabViewModelBase> _tabs = new ObservableCollection<TabViewModelBase>();
        ObservableCollection<string> _recentFiles = new ObservableCollection<string>();

        public IList<TabViewModelBase> Tabs => _tabs;
        public IList<string> RecentFiles => _recentFiles;

        public void SelectTab(TabViewModelBase tab) {
            if(!Tabs.Contains(tab))
                Tabs.Add(tab);
            SelectedTab = tab;
        }

        private TabViewModelBase _selectedTab;

        public TabViewModelBase SelectedTab {
            get { return _selectedTab; }
            set { SetProperty(ref _selectedTab, value); }
        }

        private string _fileName;
        private PEHeader _peHeader;
        public PEFileHelper PEFile { get; private set; }

        public string PathName { get; set; }
        public PEHeader PEHeader {
            get { return _peHeader; }
            set { SetProperty(ref _peHeader, value); }
        }


        public string FileName {
            get { return _fileName; }
            set { SetProperty(ref _fileName, value); }
        }

        [Import]
        IFileDialogService FileDialogService;

        [Import]
        IMessageBoxService MessageBoxService;

        [Import]
        public CompositionContainer Container { get; private set; }

        ObservableCollection<TreeViewItemViewModel> _treeRoot = new ObservableCollection<TreeViewItemViewModel>();

        public IList<TreeViewItemViewModel> TreeRoot => _treeRoot;

        public ICommand OpenCommand => new DelegateCommand(() => {
            try {
                var filename = FileDialogService.GetFileForOpen("PE Files (*.exe;*.dll;*.ocx;*.obj;*.lib)|*.exe;*.dll;*.ocx;*.obj;*.lib", "Select File");
                if(filename == null) return;
                OpenInternal(filename);
                CloseCommand.Execute(null);
                FileName = Path.GetFileName(filename);
                PathName = filename;
                OnPropertyChanged(nameof(Title));
                MapFile();

                BuildTree();
                RecentFiles.Remove(PathName);
                RecentFiles.Insert(0, PathName);
            }
            catch(Exception ex) {
                MessageBoxService.ShowMessage(ex.Message, "PE Explorer");
            }
        });

        private void BuildTree() {
            TreeRoot.Clear();
            var root = new TreeViewItemViewModel(this) { Text = FileName, Icon = "/icons/data.ico", IsExpanded = true };
            TreeRoot.Add(root);

            var generalTab = Container.GetExportedValue<GeneralTabViewModel>();
            var exportTab = Container.GetExportedValue<ExportsTabViewModel>();
            var importsTab = Container.GetExportedValue<ImportsTabViewModel>();

            root.Items.Add(new TreeViewItemViewModel(this) { Text = "(General)", Icon = "/icons/general.ico", Tab = generalTab });

            if(PEHeader.ExportDirectory.VirtualAddress > 0)
                root.Items.Add(new TreeViewItemViewModel(this) { Text = "Exports (.edata)", Icon = "/icons/export1.ico", Tab = exportTab });
            if(PEHeader.ImportDirectory.VirtualAddress > 0)
                root.Items.Add(new TreeViewItemViewModel(this) { Text = "Imports (.idata)", Icon = "/icons/import2.ico", Tab = importsTab });
            if(PEHeader.ResourceDirectory.VirtualAddress > 0)
                root.Items.Add(new TreeViewItemViewModel(this) {
                    Text = "Resources (.rsrc)",
                    Icon = "/icons/resources.ico",
                    Tab = Container.GetExportedValue<ResourcesTabViewModel>()
                });

            Tabs.Add(generalTab);

            SelectedTab = generalTab;
        }

        public ICommand SelectTabCommand => new DelegateCommand<TreeViewItemViewModel>(item => {
            SelectTab(item.Tab);
        });

        MemoryMappedFile _mmf;
        public MemoryMappedViewAccessor Accessor { get; private set; }

        private void MapFile() {
            _mmf = MemoryMappedFile.CreateFromFile(PathName, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
            Accessor = _mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
            PEFile = new PEFileHelper(PEHeader, Accessor);
        }

        PEFile _file;
        private void OpenInternal(string filename) {
            using(var stm = File.OpenRead(filename)) {
                _file = new PEFile(stm, false);
                PEHeader = _file.Header;
            }
        }

        public ICommand ExitCommand => new DelegateCommand(() => Application.Current.Shutdown());

        public ICommand CloseCommand => new DelegateCommand(() => {
            if(_file != null)
                _file.Dispose();
            FileName = null;
            if(Accessor != null)
                Accessor.Dispose();
            if(_mmf != null)
                _mmf.Dispose();
            _tabs.Clear();
            _treeRoot.Clear();
            OnPropertyChanged(nameof(Title));
        });

        public ICommand CloseTabCommand => new DelegateCommand<TabViewModelBase>(tab => Tabs.Remove(tab));
    }
}
