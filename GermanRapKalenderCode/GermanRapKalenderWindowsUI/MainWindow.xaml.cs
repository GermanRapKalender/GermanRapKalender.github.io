/*

DIRM = Das ist Räuber Musik

Basically gets all the Titles and Artists from a release from the RSS of: deinupdate



ToDo:


Clean up UI code and styles. At least a bit...
error message for that guy on discord...

Colors, Styles etc...for MainWindow and popups.
Clean up code, add logging, etc etc...Maybe. If i feel like it. Devs be lazy.




[SCRAPED] Change date for Release function 
[SCRAPED] Compare Release Dates with functions
[SCRAPED] Manage artist 
[SCRAPED] Backend change to dll or console line application I can call and get data from
[SCRAPED] Load everything in ram and search...

[MAYBE FOR CURRENTLY LOADED DATE] Filesystemwarcher 
[DONE AND FIXED] Check whats fucking the scraper at some times...
[DONE] DatePicker thingy changed with disabled buttons when not friday.
[DONE] Extra Button with contextMenu
[DONE] Open csv in notepad, open Path with CSVs
[DONE] Added Date stuff
[DONE] fixed saving empty files
[DONE, NEEDS TESTING] Export / Import / Spotify / Youtube buttons to have contextMenu to determine what to do. 
	[Implemented] Export / Import - Everything or just the specific day.
	[UI Implemented, Backend Implemented, MiddleEnd Async UI implemented] Spotify / Youtube - Everything or just the specific day.
Generate CSVs for Neini
re-point installer stuff to new repo. Plus new UUID and stuff


Crash while scraping all days
Generating Neini stuff works, but trimming is fucked somewhere...
Some youtube links no video ID (only happens AFTER a save and reload??? what the fuck is going on?)
disable dp while scraping

*/

using CodeHollow.FeedReader;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace DIRM
{
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
				Helper.Logger.Log(ex);
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
				Helper.Logger.Log(ex);
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
				Helper.Logger.Log(ex);
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
			viewModel.CalenderEntries.Clear();
			IList<Helper.CalenderEntry> ILCE = Helper.CSVHelper.Read(Globals.CurrCSVFile);
			viewModel.MyAdd(ILCE);

			if (myDT.DayOfWeek != DayOfWeek.Friday)
			{
				btn_GetFromDate.IsEnabled = false;
				btn_GetFromDate.ToolTip = "Disabled because its not Friday";
			}
			else
			{
				btn_GetFromDate.IsEnabled = true;
				btn_GetFromDate.ToolTip = "Scrape Releases from DeinUpdate";
			}
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
				Helper.CSVHelper.Save();
				Helper.FileHandling.deleteFile(Globals.CurrCSVFile);
			}
		}

		private void dg_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
		{
			Helper.CSVHelper.Save();
		}

		private void dg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			Helper.CSVHelper.Save();
		}

		private void dg_CurrentCellChanged(object sender, EventArgs e)
		{
			Helper.CSVHelper.Save();
		}

		private void dg_AddingNewItem(object sender, AddingNewItemEventArgs e)
		{
			Helper.CalenderEntry myObject = new Helper.CalenderEntry();

			myObject.Date = ((DateTime)dp.SelectedDate).ToString("yyyy_MM_dd");

			e.NewItem = myObject;

			Helper.CSVHelper.Save();
		}

		private void dg_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			Helper.CSVHelper.Save();
		}

		private void dg_Sorting(object sender, DataGridSortingEventArgs e)
		{
			Helper.CSVHelper.Save();
		}

		private void btn_Save_Click(object sender, RoutedEventArgs e)
		{
			Helper.CSVHelper.Save(true);
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
				Helper.Logger.Log(ex);
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
				Helper.Logger.Log(ex);
			}
		}



		private void btn_ExportCSV_Click(object sender, RoutedEventArgs e)
		{
			ExportContextMenu();
		}

		public void ExportSingleCSV()
		{
			FileInfo file = new FileInfo("DIRM_" + ((DateTime)MainWindow.MW.dp.SelectedDate).ToString("yyyy_MM_dd") + ".csv");
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "CSV Files|*.csv*";
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			saveFileDialog.FileName = file.Name;
			saveFileDialog.DefaultExt = file.Extension;
			saveFileDialog.AddExtension = true;
			saveFileDialog.Title = "Save all Releases for this Date as a CSV";

			if (saveFileDialog.ShowDialog() == true)
			{
				if (Helper.FileHandling.doesFileExist(saveFileDialog.FileName))
				{
					Helper.FileHandling.deleteFile(saveFileDialog.FileName);
				}
				Helper.CSVHelper.Save(saveFileDialog.FileName, this.viewModel.CalenderEntries, true);
			}

		}

		private void btn_ImportCSV_Click(object sender, RoutedEventArgs e)
		{
			ImportContextMenu();
		}

		public void ImportSingleCSV()
		{
			string FilePathChosenByUserToImport = Helper.FileHandling.OpenDialogExplorer(Helper.FileHandling.PathDialogType.File, "Select the CSV File you want to import to this specific date", @"C:\", false, "CSV Files|*.csv*");

			if (!String.IsNullOrWhiteSpace(FilePathChosenByUserToImport))
			{
				MainWindow.MW.viewModel.MyAdd(Helper.CSVHelper.Read(FilePathChosenByUserToImport));
			}
		}


		private void btn_Extra_Click(object sender, RoutedEventArgs e)
		{
			ContextMenu cm = new ContextMenu();

			MenuItem mi = new MenuItem();
			mi.Header = "Open current CSV";
			mi.Click += MI_OpenCurrentCSV_Click;
			cm.Items.Add(mi);

			MenuItem mi2 = new MenuItem();
			mi2.Header = "Open CSV Folder";
			mi2.Click += MI_OpenCSVFolder_Click;
			cm.Items.Add(mi2);

			MenuItem mi3 = new MenuItem();
			mi3.Header = "Generate stuff for Neini";
			mi3.Click += MI_GenerateNeini_Click;
			cm.Items.Add(mi3);
			cm.IsOpen = true;
		}



		private void MI_OpenCurrentCSV_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("notepad.exe", Globals.CurrCSVFile);
		}
		private void MI_OpenCSVFolder_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("explorer.exe", Globals.CSVSubfolderPath);
		}
		private void MI_GenerateNeini_Click(object sender, RoutedEventArgs e)
		{
			string dialogresult = Helper.FileHandling.OpenDialogExplorer(Helper.FileHandling.PathDialogType.Folder, "Pick the Folder you want to generate Neinis CSVs to.\nTHIS PATH WILL BE CLEARED.", Globals.ProjectInstallationPath);

			if (!string.IsNullOrWhiteSpace(dialogresult))
			{
				if ((Helper.FileHandling.GetFilesFromFolderAndSubFolder(dialogresult)).Length > 0)
				{
					Popups.Popup yesno = new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupYesNo, "This will DELETE the entire contents of that folder.\nContinue / Proceed?");
					yesno.ShowDialog();
					if (yesno.DialogResult == true)
					{
						Helper.FileHandling.DeleteFolder(dialogresult);
						Helper.FileHandling.createPath(dialogresult);
					}
					else
					{
						MI_GenerateNeini_Click(null, null);
					}
				}
				List<string> ListOfFilesCreated = new List<string>();
				Helper.FileHandling.createPath(dialogresult.TrimEnd('\\') + @"\Dates");
				Helper.FileHandling.createPath(dialogresult.TrimEnd('\\') + @"\Artists");

				string[] Files = Helper.FileHandling.GetFilesFromFolder(Globals.CSVSubfolderPath);
				IList<Helper.CalenderEntry> myList = new List<Helper.CalenderEntry>();
				for (int i = 0; i <= Files.Length - 1; i++)
				{
					string RegexPattern = @"^[0-9]{4}_[0-9]{2}_[0-9]{2}.csv$";
					Regex MyRegex = new Regex(RegexPattern);
					Match MyMatch = MyRegex.Match(Helper.FileHandling.PathSplitUp(Files[i])[1]);
					if (MyMatch.Success)
					{
						string tmp = @"Dates\" + Helper.FileHandling.PathSplitUp(Files[i])[1];
						Helper.FileHandling.copyFile(Files[i], dialogresult.TrimEnd('\\') + @"\" + tmp);
						ListOfFilesCreated.Add(tmp);
					}


					IList<Helper.CalenderEntry> tmpce = Helper.CSVHelper.Read(Files[i]);
					myList = MainWindow.MW.viewModel.MyAdd(myList, tmpce);
				}


				Dictionary<string, IList<Helper.CalenderEntry>> sth = new Dictionary<string, IList<Helper.CalenderEntry>>();

				foreach (Helper.CalenderEntry myCE in myList)
				{
					string[] splitstrings = { ",", "&", "feat" };
					string[] myArtists = myCE.Artist.Replace("feat.", "feat").Split(splitstrings, StringSplitOptions.RemoveEmptyEntries);

					foreach (string Artist in myArtists)
					{
						string RealArtist = Helper.FileHandling.RemoveLeadingTrailingSpaces(Artist);
						if (!string.IsNullOrWhiteSpace(RealArtist))
						{
							if (!sth.ContainsKey(Artist))
							{
								sth.Add(Artist, new List<Helper.CalenderEntry>());
							}
							sth[Artist].Add(myCE);
						}
					}
				}

				foreach (KeyValuePair<string, IList<Helper.CalenderEntry>> entry in sth)
				{
					string tmp = @"Artists\" + entry.Key + ".csv";
					if (!tmp.Contains("?"))
					{
						Helper.CSVHelper.Save(dialogresult.TrimEnd('\\') + @"\" + tmp, entry.Value);
						ListOfFilesCreated.Add(tmp);
					}
				}

				Helper.CSVHelper.Save(dialogresult.TrimEnd('\\') + @"\Everything.csv", myList);
				ListOfFilesCreated.Add("Everything.csv");

				Helper.FileHandling.WriteStringToFileOverwrite(dialogresult.TrimEnd('\\') + @"\AllFileNames.csv", ListOfFilesCreated.ToArray());

				dp_SelectedDateChanged(null, null);
			}
		}

		private void btn_GetSpotifyLinks_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			ContextMenu cm = new ContextMenu();

			MenuItem mi = new MenuItem();
			mi.Header = "Get Spotify Links for today";
			mi.Click += MI_SpotifyToday_Click;
			cm.Items.Add(mi);

			MenuItem mi2 = new MenuItem();
			mi2.Header = "Get Spotify Links for ALL days";
			mi2.Click += MI_SpotifyEveryday_Click;
			cm.Items.Add(mi2);

			cm.IsOpen = true;
		}

		private void MI_SpotifyToday_Click(object sender, RoutedEventArgs e)
		{
			btn_GetSpotifyLinks_Click(null, null);
		}

		private void MI_SpotifyEveryday_Click(object sender, RoutedEventArgs e)
		{
			Scraping.ScrapingLogic.ScrapeAllSpotify();
		}


		private void btn_GetYoutubeLinks_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			ContextMenu cm = new ContextMenu();

			MenuItem mi = new MenuItem();
			mi.Header = "Get Youtube Links for today";
			mi.Click += MI_YoutubeToday_Click;
			cm.Items.Add(mi);

			MenuItem mi2 = new MenuItem();
			mi2.Header = "Get Youtube Links for ALL days";
			mi2.Click += MI_YoutubeEveryday_Click;
			cm.Items.Add(mi2);

			cm.IsOpen = true;
		}

		private void MI_YoutubeToday_Click(object sender, RoutedEventArgs e)
		{
			btn_GetYoutubeLinks_Click(null, null);
		}

		private void MI_YoutubeEveryday_Click(object sender, RoutedEventArgs e)
		{
			Scraping.ScrapingLogic.ScrapeAllYoutube();
		}
	}
}
