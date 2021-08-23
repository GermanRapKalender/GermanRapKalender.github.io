using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GermanRapKalenderClassLibaries.Scraping
{
	public class YoutubeAPI_Scraper
	{
		static bool ManualStaticConstructorRunAlready = false;

		static YouTubeService myYoutubeService = null;

		public static void Init()
		{
			string MyAPIConfigContent = Helper.FileHandling.ReadResourceFile(@"Scraping\MyAPIConfig.ini");

			myYoutubeService = new YouTubeService(new BaseClientService.Initializer()
			{
				ApiKey = Helper.FileHandling.GetXMLTagContent(MyAPIConfigContent, "YOUTUBE_API_KEY"),
				ApplicationName = "GermanRapKalender"
			});
		}


		public static async Task<string> GetLinkFromSearch(Helper.CalenderEntry myRelease)
		{
			if (!ManualStaticConstructorRunAlready)
			{
				Init();
				ManualStaticConstructorRunAlready = true;
			}

			if (myRelease.CalenderEntryType == Helper.CalenderEntryTypes.Album)
			{
				return "";
			}

			var searchListRequest = myYoutubeService.Search.List("snippet");
			searchListRequest.Q = myRelease.SearchString;
			searchListRequest.MaxResults = 50;

			var searchListResponse = await searchListRequest.ExecuteAsync();

			string myYoutubeLink = "";
			int bestComparison = 9999;

			foreach (var item in searchListResponse.Items)
			{
				switch (item.Id.Kind)
				{
					case "youtube#video":
						int currComparison = Helper.FileHandling.getLevenshteinDistance(item.Snippet.Title, myRelease.SearchString);
						if (currComparison < bestComparison)
						{
							myYoutubeLink = "https://www.youtube.com/watch?v=" + item.Id.VideoId;
							bestComparison = currComparison;
						}
						break;
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
