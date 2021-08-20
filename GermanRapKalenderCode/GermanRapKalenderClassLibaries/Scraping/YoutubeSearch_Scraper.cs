using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeSearch;

namespace GermanRapKalenderClassLibaries.Scraping
{
	public class YoutubeSearch_Scraper
	{
		public static async Task<string> GetLinkFromSearch(Helper.Event myRelease)
		{
			if (myRelease.ReleaseKind != Helper.EventType.Single)
			{
				return "";
			}

			VideoSearch vs = new VideoSearch();
			List<VideoSearchComponents> items = await vs.GetVideos(myRelease.SearchString, 1);

			string myYoutubeLink = "";
			int bestComparison = 9999;

			foreach (var item in items)
			{
				int currComparison = Helper.FileHandling.getLevenshteinDistance(item.getTitle(), myRelease.SearchString);
				if (currComparison < bestComparison)
				{
					myYoutubeLink = item.getUrl();
					bestComparison = currComparison;
				}
			}

			if (!String.IsNullOrWhiteSpace(myYoutubeLink))
			{
				return myYoutubeLink;
			}

			return "";
		}
	}
}
