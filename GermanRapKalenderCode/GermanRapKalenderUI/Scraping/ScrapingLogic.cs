using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GermanRapKalenderClassLibaries.Helper;
using GermanRapKalenderClassLibaries.Scraping;


namespace GermanRapKalenderUI.Scraping
{
	class ScrapingLogic
	{
		public static void ExceptionTest()
		{
			string tmp = "asdf";
			Globals.DebugPopup(tmp[999].ToString());
		}

		public static async void ScrapeSpotify()
		{
			MainWindow.MW.btn_GetSpotifyLinks.IsEnabled = false;
			MainWindow.MW.btn_GetSpotifyLinks.Content += "...";

			List<Task<string>> tmp = new List<Task<string>>();
			List<int> ugly = new List<int>();
			for (int i = 0; i <= MainWindow.MW.viewModel.CalenderEntries.Count - 1; i++)
			{
				if (!MainWindow.MW.viewModel.CalenderEntries[i].Links.ToLower().Contains("spotify"))
				{
					Task<string> tmpTask = SpotifyScraper.GetLinkFromSearch(MainWindow.MW.viewModel.CalenderEntries[i]);
					tmp.Add(tmpTask);
					ugly.Add(i);
				}
			}

			for (int i = 0; i <= tmp.Count - 1; i++)
			{
				string SpotifyLink = await tmp[i];
				if (!String.IsNullOrWhiteSpace(SpotifyLink))
				{
					MainWindow.MW.viewModel.Change(ugly[i], MainWindow.MW.viewModel.CalenderEntries[ugly[i]].Links + " " + SpotifyLink);
				}
			}

			MainWindow.MW.btn_GetSpotifyLinks.Content = MainWindow.MW.btn_GetSpotifyLinks.Content.ToString().TrimEnd('.').TrimEnd('.').TrimEnd('.');
			MainWindow.MW.btn_GetSpotifyLinks.IsEnabled = true;
		}

		public static async void ScrapeYoutube()
		{
			MainWindow.MW.btn_GetYoutubeLinks.IsEnabled = false;
			MainWindow.MW.btn_GetYoutubeLinks.Content += "...";

			bool UseSlowScrape = false;

			// Yeah cant make this quicker, getting rate limited
			for (int i = 0; i <= MainWindow.MW.viewModel.CalenderEntries.Count - 1; i++)
			{
				if (!MainWindow.MW.viewModel.CalenderEntries[i].Links.ToLower().Contains("youtu") && MainWindow.MW.viewModel.CalenderEntries[i].CalenderEntryType == CalenderEntryTypes.Single) // catches youtube.com and youtu.be
				{
					string YoutubeLink = "";

					if (UseSlowScrape)
					{
						YoutubeLink = await YoutubeSearch_Scraper.GetLinkFromSearch(MainWindow.MW.viewModel.CalenderEntries[i]);
					}
					else
					{
						try
						{
							YoutubeLink = await YoutubeAPI_Scraper.GetLinkFromSearch(MainWindow.MW.viewModel.CalenderEntries[i]);
						}
						catch
						{
							Logger.Log("Gonna use the slow youtube scrape instead of the API, because the API is rate limited");
							UseSlowScrape = true;
							if (!Globals.YoutubeSlowWarningThrown)
							{
								MainWindow.MW.SetUpAnnoucement("Will use slow(er) Youtube Scraping since the API RateLimit is reached for today.\nThis resets daily.\n");
								Globals.YoutubeSlowWarningThrown = true;
							}
							YoutubeLink = await YoutubeSearch_Scraper.GetLinkFromSearch(MainWindow.MW.viewModel.CalenderEntries[i]);
						}
					}

					if (!String.IsNullOrWhiteSpace(YoutubeLink))
					{
						MainWindow.MW.viewModel.Change(i, MainWindow.MW.viewModel.CalenderEntries[i].Links + " " + YoutubeLink);
					}
				}
			}

			MainWindow.MW.btn_GetYoutubeLinks.Content = MainWindow.MW.btn_GetYoutubeLinks.Content.ToString().TrimEnd('.').TrimEnd('.').TrimEnd('.');
			MainWindow.MW.btn_GetYoutubeLinks.IsEnabled = true;
		}

		public static void ExportForReddit()
		{
			DateTime myDT = (DateTime)MainWindow.MW.dp.SelectedDate;
			string CurrFriday = myDT.ToString("dd.MM.yyyy");
			string LastSaturday = myDT.AddDays(-6).ToString("dd.MM.yyyy");

			IList<CalenderEntry> myList;
			myList = MainWindow.MW.viewModel.CalenderEntries;

			if (Settings.AlwaysExportAlphabetically)
			{
				myList = myList.OrderBy(p => p.Artist).ToList();
			}

			// Open Notepad with correct shit
			List<string> temp = new List<string>();
			temp.Add("**[Release-Friday] - Die Releases am " + CurrFriday + "**");
			temp.Add("");
			temp.Add("---");
			temp.Add("");
			temp.Add("Einen wunderschönen guten Tag zusammen, ich begrüße Sie.  ");
			temp.Add("Im Nachfolgenden sind die Releases der letzten Woche (" + LastSaturday + " - " + CurrFriday + ") aufgelistet.");
			temp.Add("");
			temp.Add("---");
			temp.Add("");
			temp.Add("**Singles**");
			temp.Add("");
			temp.Add("**Artist**|**Title**|**Info**");
			temp.Add(":--|:--|:--");
			foreach (CalenderEntry myRelease in myList)
			{
				if (myRelease.CalenderEntryType == CalenderEntryTypes.Single)
				{
					temp.Add(myRelease.Artist + "|" + myRelease.Title.Replace("[", @"\[").Replace("]", @"\]") + "|" + myRelease.GetInfoTabFromRelease());
				}
			}
			temp.Add("");
			temp.Add("---");
			temp.Add("");
			temp.Add("**Alben**");
			temp.Add("");
			temp.Add("**Artist**|**Title**|**Info**");
			temp.Add(":--|:--|:--");
			foreach (CalenderEntry myRelease in myList)
			{
				if (myRelease.CalenderEntryType == CalenderEntryTypes.Album)
				{
					temp.Add(myRelease.Artist + "|" + myRelease.Title.Replace("[", @"\[").Replace("]", @"\]") + "|" + myRelease.GetInfoTabFromRelease());
				}
			}
			temp.Add("");
			temp.Add("---");
			temp.Add("");
			temp.Add("Wie immer, sollte irgendwas nicht gelistet seien, lasst es mich in den Kommentaren wissen und habt nen schönen Tag!");

			FileHandling.WriteStringToFileOverwrite(Globals.TEMPFile, temp.ToArray());
			try
			{
				Process.Start("notepad.exe", Globals.TEMPFile);
			}
			catch { }
		}


		public static async void ScrapeReleases(bool OnlyShowSource = false, bool OwnUrl = false)
		{
			Logger.Log("Lets Scrape");

			DateTime myDT = (DateTime)MainWindow.MW.dp.SelectedDate;
			MainWindow.MW.btn_GetFromDate.Content += "...";

			List<string> ListOfLinks = new List<string>();

			Logger.Log(String.Format("Starting to scrape: OnlyShowSource=\"{0}\", OwnUrl=\"{1}\", myDT=\"{2}\"", OnlyShowSource, OwnUrl, myDT.ToString("yyyy_MM_dd")));

			if (OwnUrl)
			{
				Popups.PopupTextBox tmp = new Popups.PopupTextBox("Enter Link:", "Link here");
				tmp.ShowDialog();
				if (tmp.MyReturnString != "")
				{
					Logger.Log("Custom Link: \"" + tmp.MyReturnString + "\"");
					ListOfLinks.Add(tmp.MyReturnString);
				}
			}
			else
			{
				Logger.Log("Trying to scrape RSS");

				ListOfLinks = await RSS_Scraper.GetLinksAsync(myDT);

				Logger.Log("Done scraping RSS");
			}



			if (ListOfLinks.Count == 0)
			{
				Logger.Log("Havent found a release Link. (" + myDT.ToString("yyyy - MM - dd") + ")");
				new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupOkError, "Havent found a release Post Link for that date. (" + myDT.ToString("dd.MM.yyyy") + ")").ShowDialog();
			}
			else
			{
				Logger.Log("Found " + ListOfLinks.Count + " Link(s). (" + myDT.ToString("yyyy - MM - dd") + ")");
				int counter = 0;
				foreach (string MyLink in ListOfLinks)
				{
					Logger.Log("ReleaseLink[" + counter + "]: " + MyLink);

					List<CalenderEntry> ReleaseList = new List<CalenderEntry>();

					try
					{
						string myWebSource = await DeinUpdate_Scraper.GetWebsiteSource(MyLink);

						if (OnlyShowSource)
						{

							FileHandling.WriteStringToFileOverwrite(Globals.TEMPFile, new[] { myWebSource });
							try
							{
								Process.Start("notepad.exe", Globals.TEMPFile);
							}
							catch { }

							break;
						}
						Logger.Log("Trying to get all Releases from this DeinUpdate post: " + MyLink);
						ReleaseList = await DeinUpdate_Scraper.GetReleasesFromLinkAsync(myWebSource, myDT);
					}
					catch (Exception ex)
					{
						Logger.Log(ex.Message);
						new Popups.Popup(Popups.Popup.PopupWindowTypes.PopupOkError, ex.Message).ShowDialog();
					}


					MainWindow.MW.viewModel.MyAdd(ReleaseList);
				}
			}


			MainWindow.MW.btn_GetFromDate.Content = MainWindow.MW.btn_GetFromDate.Content.ToString().TrimEnd('.').TrimEnd('.').TrimEnd('.');
			Logger.Log("Done scraping");
		}
	}
}
