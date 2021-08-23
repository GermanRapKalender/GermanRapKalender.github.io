using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GermanRapKalenderClassLibaries.Scraping
{
	public class Youtube_Scraper
	{
		public static async Task<string> GetLinkFromSearch(Helper.CalenderEntry myRelease)
		{
			try
			{
				return await YoutubeAPI_Scraper.GetLinkFromSearch(myRelease);
			}
			catch
			{
				return await YoutubeSearch_Scraper.GetLinkFromSearch(myRelease);
			}
		}
	}
}
