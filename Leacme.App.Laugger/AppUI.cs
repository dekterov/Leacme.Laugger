// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Leacme.Lib.Laugger;

namespace Leacme.App.Laugger {

	public class AppUI {

		private StackPanel rootPan = (StackPanel)Application.Current.MainWindow.Content;
		private Library lib = new Library();

		public AppUI() {

			var blb1 = App.TextBlock;
			blb1.TextAlignment = TextAlignment.Center;
			blb1.Text = "Loading... Local System Logs";
			rootPan.Children.AddRange(new List<IControl> { blb1 });

			Dispatcher.UIThread.InvokeAsync(async () => {

				var gridScroll = App.ScrollViewer;
				gridScroll.Height = App.Current.MainWindow.Height - 70;
				gridScroll.Background = Brushes.Transparent;

				App.Current.MainWindow.PropertyChanged += (z, zz) => {
					if (zz.Property.Equals(Window.HeightProperty)) {
						gridScroll.Height = App.Current.MainWindow.Height - 70;
					}
				};
				var gridHolder = new StackPanel() {
					Margin = new Thickness(10, 0)
				};
				gridScroll.Content = gridHolder;

				var logs = await lib.GetLocalLogs();
				foreach (var log in logs) {
					var dg = App.DataGrid;
					dg.Height = 300;
					dg.SetValue(DataGrid.WidthProperty, AvaloniaProperty.UnsetValue);

					dg.AutoGeneratingColumn += (z, zz) => { zz.Column.Header = log.logName; };

					dg.Items = log.logMessages;

					gridHolder.Children.Add(dg);

				}

				rootPan.Children.AddRange(new List<IControl> { gridScroll });
				blb1.Text = "Loaded - Local System Logs";
			});

		}
	}
}