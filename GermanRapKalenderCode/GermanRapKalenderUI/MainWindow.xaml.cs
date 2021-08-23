using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GermanRapKalenderClassLibaries;
using GermanRapKalenderClassLibaries.Helper;
using GermanRapKalenderClassLibaries.Scraping;
using Microsoft.Win32;

namespace GermanRapKalenderUI
{
	/*
	UI (previously DIRM https://github.com/TwosHusbandS/DasIstRaueberMusik)
	*/

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{


		// Lets get dirty here...
		public static MainWindow MW;

		/// <summary>
		/// Constructor of our Main Window
		/// </summary>
		public MainWindow()
		{
			// Initializes WPF Shit
			InitializeComponent();

			MW = this;

			Globals.Init();

			InitUI();
		}


		private void btn_GetFromDate_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Scraping.ScrapingLogic.ScrapeReleases();
			}
			catch (Exception ex)
			{
				Logger.Log(ex);
			}
		}


		private void btn_GetFromDate_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			ContextMenu cm = new ContextMenu();

			MenuItem mi = new MenuItem();
			mi.Header = "Open Website Source in NotePad";
			mi.Click += MI_WebsiteSource_Click;
			cm.Items.Add(mi);

			MenuItem mi2 = new MenuItem();
			mi2.Header = "Custom URL to parse from";
			mi2.Click += MI_CustomURL_Click;
			cm.Items.Add(mi2);

			cm.IsOpen = true;
		}


		private void MI_WebsiteSource_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Scraping.ScrapingLogic.ScrapeReleases(true, false);
			}
			catch (Exception ex)
			{
				Logger.Log(ex);
			}
		}

		private void MI_CustomURL_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Scraping.ScrapingLogic.ScrapeReleases(false, true);
			}
			catch (Exception ex)
			{
				Logger.Log(ex);
			}
		}


		private void btn_Export_Click(object sender, RoutedEventArgs e)
		{
			Scraping.ScrapingLogic.ExportForReddit();
		}

		private void btn_About_Click(object sender, RoutedEventArgs e)
		{
			if (PageState == PageStates.AboutSettings)
			{
				PageState = PageStates.Main;
			}
			else
			{
				PageState = PageStates.AboutSettings;
			}
		}

		private void btn_Exit_Click(object sender, RoutedEventArgs e)
		{
			if (PageState == PageStates.AboutSettings)
			{
				PageState = PageStates.Main;
			}
			else
			{
				Popups.Popup ppp = new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupYesNo, "Do you want to exit?");
				ppp.ShowDialog();
				if (ppp.DialogResult == true)
				{
					this.Close();
				}
			}
		}


		private void dp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			DateTime myDT = (DateTime)dp.SelectedDate;
			lbl_dg_Header.Content = "Die Releases vom: " + myDT.ToString("dd.MM.yyyy");
			MainWindow.MW.viewModel.CalenderEntries.Clear();
			MainWindow.MW.viewModel.MyAdd(CSVHelper.Read(Globals.CurrCSVFile));
		}

		private void btn_Clear_Click(object sender, RoutedEventArgs e)
		{
			// // Exception Test
			//try
			//{
			//	Scraping.ScrapingLogic.ExceptionTest();
			//}
			//catch (Exception ex)
			//{
			//	Helper.Logger.Log(ex);
			//}

			Popups.Popup yesno = new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupYesNo, "Are you sure?");
			yesno.ShowDialog();
			if ((bool)yesno.DialogResult)
			{
				this.viewModel.CalenderEntries.Clear();
				CSVHelper.Save(Globals.CurrCSVFile, MainWindow.MW.viewModel.CalenderEntries, Settings.AutoSave);
				FileHandling.deleteFile(Globals.CurrCSVFile);
			}
		}

		private void dg_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
		{
			CSVHelper.Save(Globals.CurrCSVFile, MainWindow.MW.viewModel.CalenderEntries, Settings.AutoSave);
		}

		private void dg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			CSVHelper.Save(Globals.CurrCSVFile, MainWindow.MW.viewModel.CalenderEntries, Settings.AutoSave);
		}

		private void dg_CurrentCellChanged(object sender, EventArgs e)
		{
			CSVHelper.Save(Globals.CurrCSVFile, MainWindow.MW.viewModel.CalenderEntries, Settings.AutoSave);
		}

		private void dg_AddingNewItem(object sender, AddingNewItemEventArgs e)
		{
			CSVHelper.Save(Globals.CurrCSVFile, MainWindow.MW.viewModel.CalenderEntries, Settings.AutoSave);
		}

		private void dg_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			CSVHelper.Save(Globals.CurrCSVFile, MainWindow.MW.viewModel.CalenderEntries, Settings.AutoSave);
		}

		private void dg_Sorting(object sender, DataGridSortingEventArgs e)
		{
			CSVHelper.Save(Globals.CurrCSVFile, MainWindow.MW.viewModel.CalenderEntries, Settings.AutoSave);
		}

		private void btn_Save_Click(object sender, RoutedEventArgs e)
		{
			CSVHelper.Save(Globals.CurrCSVFile, MainWindow.MW.viewModel.CalenderEntries, true);
		}


		private void btn_Hamburger_Click(object sender, RoutedEventArgs e)
		{
			if (HamburgerMenuState == HamburgerMenuStates.Hidden)
			{
				HamburgerMenuState = HamburgerMenuStates.Visible;
			}
			else
			{
				HamburgerMenuState = HamburgerMenuStates.Hidden;
			}
		}


		private async void btn_GetSpotifyLinks_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Scraping.ScrapingLogic.ScrapeSpotify();
			}
			catch (Exception ex)
			{
				Logger.Log(ex);
			}
		}

		private void btn_GetYoutubeLinks_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Scraping.ScrapingLogic.ScrapeYoutube();
			}
			catch (Exception ex)
			{
				Logger.Log(ex);
			}

		}



		private void btn_ExportCSV_Click(object sender, RoutedEventArgs e)
		{
			FileInfo file = new FileInfo("GermanRapKalender_" + ((DateTime)MainWindow.MW.dp.SelectedDate).ToString("yyyy_MM_dd") + ".csv");
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "CSV Files|*.csv*";
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			saveFileDialog.FileName = file.Name;
			saveFileDialog.DefaultExt = file.Extension;
			saveFileDialog.AddExtension = true;
			saveFileDialog.Title = "Save all Releases for this Date as a CSV";

			if (saveFileDialog.ShowDialog() == true)
			{
				if (FileHandling.doesFileExist(saveFileDialog.FileName))
				{
					FileHandling.deleteFile(saveFileDialog.FileName);
				}
				CSVHelper.Save(saveFileDialog.FileName, this.viewModel.CalenderEntries, true);
			}

		}

		private void btn_ImportCSV_Click(object sender, RoutedEventArgs e)
		{
			string FilePathChosenByUserToImport = Helper.FolderSelectDialog.OpenDialogExplorer(Helper.FolderSelectDialog.PathDialogType.File, "Select the CSV File you want to import to this specific date", @"C:\", false, "CSV Files|*.csv*");

			if (!String.IsNullOrWhiteSpace(FilePathChosenByUserToImport))
			{
				MainWindow.MW.viewModel.MyAdd(CSVHelper.Read(FilePathChosenByUserToImport));
			}
		}
	}
}
