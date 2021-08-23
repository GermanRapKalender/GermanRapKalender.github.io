using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GermanRapKalenderClassLibaries.Scraping
{
	public class SpotifyScraper
	{
		static SpotifyClientConfig MySpotifyClientConfig;
		static SpotifyClient MySpotifyClient;

		static bool ManualStaticConstructorRunAlready = false;

		public static void Init()
		{
			string MyAPIConfigContent = Helper.FileHandling.ReadResourceFile(@"Scraping\MyAPIConfig.ini");

			MySpotifyClientConfig = SpotifyClientConfig
	.CreateDefault()
	.WithAuthenticator(new ClientCredentialsAuthenticator(Helper.FileHandling.GetXMLTagContent(MyAPIConfigContent, "SPOTIFY_CLIENT_ID"), Helper.FileHandling.GetXMLTagContent(MyAPIConfigContent, "SPOTIFY_CLIENT_SECRET")));

			MySpotifyClient = new SpotifyClient(MySpotifyClientConfig);
		}



		public static async Task<string> GetLinkFromSearch(Helper.CalenderEntry myRelease)
		{
			if (!ManualStaticConstructorRunAlready)
			{
				Init();
				ManualStaticConstructorRunAlready = true;
			}

			SearchRequest.Types mySearchType;
			if (myRelease.CalenderEntryType == Helper.CalenderEntryTypes.Album)
			{
				mySearchType = SearchRequest.Types.Album;
			}
			else if (myRelease.CalenderEntryType == Helper.CalenderEntryTypes.Single)
			{
				mySearchType = SearchRequest.Types.Track;
			}
			else 
			{
				return "";
			}

			SearchRequest MySearchRequest = new SearchRequest(mySearchType, myRelease.SearchString);
			ISearchClient MySearchClient = MySpotifyClient.Search;
			SearchResponse MySearchResponse = await MySearchClient.Item(MySearchRequest);

			List<string> mySpotifySearchReturnsTitles = new List<string>();
			List<string> mySpotifySearchReturnsLinks = new List<string>();
			if (mySearchType == SearchRequest.Types.Track)
			{
				foreach (var tmp in MySearchResponse.Tracks.Items)
				{
					mySpotifySearchReturnsTitles.Add(tmp.Name);
					mySpotifySearchReturnsLinks.Add(tmp.ExternalUrls["spotify"]);
				}
			}
			else
			{
				foreach (var tmp in MySearchResponse.Albums.Items)
				{
					mySpotifySearchReturnsTitles.Add(tmp.Name);
					mySpotifySearchReturnsLinks.Add(tmp.ExternalUrls["spotify"]);
				}
			}

			string MyClosestLink = "";
			int bestComparison = 9999;
			for (int i = 0; i <= mySpotifySearchReturnsTitles.Count - 1; i++)
			{
				int currComparison = Helper.FileHandling.getLevenshteinDistance(mySpotifySearchReturnsTitles[i], myRelease.Title);
				if (currComparison < bestComparison)
				{
					MyClosestLink = mySpotifySearchReturnsLinks[i];
					bestComparison = currComparison;
				}
			}

			if (!String.IsNullOrWhiteSpace(MyClosestLink))
			{
				return MyClosestLink;
			}

			return "";
		}

	}
}
