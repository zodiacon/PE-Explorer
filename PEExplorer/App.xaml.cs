﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using PEExplorer.ViewModels;
using Zodiacon.WPF;
using System.Runtime.CompilerServices;
using NLog;

namespace PEExplorer {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App  {

        MainViewModel _mainViewModel;
        public static Logger AppLogger = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            AppLogger.Info("Initializing...");
            var container = new CompositionContainer(
                new AggregateCatalog(
                    new AssemblyCatalog(Assembly.GetExecutingAssembly()),
                    new AssemblyCatalog(typeof(IDialogService).Assembly)));

			var defaults = new UIServicesDefaults();

			container.ComposeExportedValue(container);
			container.ComposeExportedValue(defaults.DialogService);
			container.ComposeExportedValue(defaults.FileDialogService);
			container.ComposeExportedValue(defaults.MessageBoxService);

			var vm = _mainViewModel = container.GetExportedValue<MainViewModel>();
            var win = new MainWindow { DataContext = vm };
            win.Show();
			if (e.Args.Length > 0)
				vm.OpenInternal(e.Args[0], false);
        }

        protected override void OnExit(ExitEventArgs e) {
            _mainViewModel.Close();
            base.OnExit(e);
        }
    }



}
