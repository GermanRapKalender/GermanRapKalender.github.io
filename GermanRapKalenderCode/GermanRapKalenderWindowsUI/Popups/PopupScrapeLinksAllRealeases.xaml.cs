using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;

namespace DIRM.Popups
{
	public partial class PopupScrapeLinksAllRealeases : Window
	{

		private void Window_SourceInitialized(object sender, EventArgs e)
		{
			if (MainWindow.MW.IsVisible)
			{
				this.Left = MainWindow.MW.Left + (MainWindow.MW.Width / 2) - (this.Width / 2);
				this.Top = MainWindow.MW.Top + (MainWindow.MW.Height / 2) - (this.Height / 2);
			}
		}


		public enum PopupScrapeLinksAllRealeasesTypes
		{
			Spotify,
			Youtu,
			// KEEP THIS YOUTU IM USING THIS TO SCRAPE...AND WANT TO CATCH YOUTUBE.COM AND YOUTU.BE LINKS
		}

		/// <summary>
		/// ProgressType of Instance
		/// </summary>
		PopupScrapeLinksAllRealeasesTypes PopupScrapeLinksAllRealeasesType;



		public PopupScrapeLinksAllRealeases(PopupScrapeLinksAllRealeasesTypes pPopupScrapeLinksAllRealeasesType)
		{
			// Sorry you have to look at this spaghetti
			// Basically, based on the pProgressType the other params have different meanings or are not used etc. Kinda messy...really sucks

			if (MainWindow.MW.IsVisible)
			{
				this.Owner = MainWindow.MW;
				this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}

			if (MainWindow.MW.IsVisible)
			{
				this.Owner = MainWindow.MW;
				this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}

			InitializeComponent();


			// Setting all Properties needed later
			PopupScrapeLinksAllRealeasesType = pPopupScrapeLinksAllRealeasesType;

			myLBL.Content = "Scraping all Releases for " + PopupScrapeLinksAllRealeasesType.ToString() + " Links.";
			myLBL_A.Content = "Looking for all files...give me a minute.";

			// Lets do some shit
			StartWork();
		}


		/// <summary>
		/// Starting the Task 
		/// </summary>
		[STAThread]
		public async void StartWork()
		{
			// Awaiting the Task of the Actual Work
			await Task.Run(new Action(ActualWork));

			// Close this
			this.Close();
		}

		/// <summary>
		/// Task of the actual work being done
		/// </summary>
		[STAThread]
		public void ActualWork()
		{
			string[] allFiles = Helper.FileHandling.GetFilesFromFolder(Globals.CSVSubfolderPath);


			for (int i = 0; i <= allFiles.Length - 1; i++)
			{
				Application.Current.Dispatcher.Invoke((Action)delegate
				{
					long progress = (100 * (i + 1) / allFiles.Length);
					myPB_A.Value = progress;
					myLBL_A.Content = "On File " + (i + 1).ToString() + " / " + allFiles.Length;
				});



				string RegexPattern = @"^[0-9]{4}_[0-9]{2}_[0-9]{2}.csv$";
				Regex MyRegex = new Regex(RegexPattern);
				Match MyMatch = MyRegex.Match(Helper.FileHandling.PathSplitUp(allFiles[i])[1]);
				if (MyMatch.Success)
				{
					IList<Helper.CalenderEntry> myEntries = Helper.CSVHelper.Read(allFiles[i]);
					for (int j = 0; j <= myEntries.Count - 1; j++)
					{
						Application.Current.Dispatcher.Invoke((Action)delegate
						{
							long progress = (100 * (j + 1) / myEntries.Count);
							myPB_A.Value = progress;
							myLBL_A.Content = "On File " + (j + 1).ToString() + " / " + myEntries.Count;
						});


						if (!myEntries[j].Link.ToLower().Contains(PopupScrapeLinksAllRealeasesType.ToString().ToLower())) 
						{
							string NewLink = "";
							if (PopupScrapeLinksAllRealeasesType == PopupScrapeLinksAllRealeasesTypes.Youtu)
							{
								NewLink =Scraping.YoutubeScrape.GetLinkFromSearch(MainWindow.MW.viewModel.CalenderEntries[j]).Result;
							}
							else if (PopupScrapeLinksAllRealeasesType == PopupScrapeLinksAllRealeasesTypes.Spotify)
							{
								NewLink = Scraping.SpotifyScraper.GetLinkFromSearch(MainWindow.MW.viewModel.CalenderEntries[j]).Result;
							}

							if (!String.IsNullOrWhiteSpace(NewLink))
							{
								myEntries[j].Link += " " + NewLink;
							}
						}
					}

					Helper.CSVHelper.Save(allFiles[i], myEntries, true);
				}

			}
		}

		/// <summary>
		/// Method which makes the Window draggable, which moves the whole window when holding down Mouse1 on the background
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove(); // Pre-Defined Method
		}

	}
}

