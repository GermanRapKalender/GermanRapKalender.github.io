using DIRM;
using DIRM.Scraping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIRM.Scraping
{
	class YoutubeScrape
	{
		public static async Task<string> GetLinkFromSearch(DIRM.Helper.CalenderEntry myRelease)
		{
			if (myRelease.CalenderEntryType != Helper.CalenderEntryTypes.Single)
			{
				return "";
			}
			try
			{
				return await YoutubeAPI.GetLinkFromSearch(myRelease);
			}
			catch
			{
				if (!Globals.YoutubeSlowWarningThrown)
				{
					MainWindow.MW.SetUpAnnoucement("Will use slow(er) Youtube Scraping since the API RateLimit is reached for today.\nThis resets daily.\n");
					Globals.YoutubeSlowWarningThrown = true;
				}
				return await YoutubeSearch.GetLinkFromSearch(myRelease);
			}
		}
	}
}
