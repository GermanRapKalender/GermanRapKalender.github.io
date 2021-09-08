using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIRM.Helper
{
	class CSVHelper
	{
		static string HeadingLine = "Artist" + CharSeperator + "Title" + CharSeperator + "ReleaseKind" + CharSeperator + "Info" + CharSeperator + "Link";

		static char CharSeperator = '=';

		public static IList<Helper.CalenderEntry> Read(string FilePath)
		{
			MainWindow.MW.viewModel.CalenderEntries.Clear();

			IList<Helper.CalenderEntry> myReleaseList = new List<Helper.CalenderEntry>();

			try
			{
				string[] filecontent = Helper.FileHandling.ReadFileEachLine(FilePath);
				if (filecontent.Length > 1)
				{
					if (filecontent[0] == HeadingLine)
					{
						for (int i = 1; i <= filecontent.Length - 1; i++)
						{
							string[] Line = filecontent[i].Split(CharSeperator);
							if (Line.Length >= 6)
							{
								CalenderEntryTypes tmp;

								if (Line[3].ToLower() == "album")
								{
									tmp = CalenderEntryTypes.Album;
								}
								else if (Line[3].ToLower() == "single")
								{
									tmp = CalenderEntryTypes.Single;
								}
								else
								{
									tmp = CalenderEntryTypes.Event;
								}

								CalenderEntry CE = new Helper.CalenderEntry
								{
									Date = Line[0],
									Artist = Line[1],
									Title = Line[2],
									CalenderEntryType = tmp,
									Info = Line[4],
									Link = Line[5]
								};

								myReleaseList.Add(CE);
							}
						}
					}
				}
			}
			catch
			{

			}
			return myReleaseList;

		}


		public static void Save(bool pManualSafe = false)
		{
			Save(Globals.CurrCSVFile, MainWindow.MW.viewModel.CalenderEntries, pManualSafe);
		}

		public static void Save(string FilePath, IList<CalenderEntry> ListOfReleases, bool pManualSave = false)
		{
			if (pManualSave || Settings.AutoSave)
			{
				if (ListOfReleases.Count > 0)
				{
					List<string> temp = new List<string>();
					temp.Add(HeadingLine);
					foreach (CalenderEntry myRelease in ListOfReleases)
					{
						temp.Add(myRelease.Date + CharSeperator + myRelease.Artist + CharSeperator + myRelease.Title + CharSeperator + myRelease.CalenderEntryType.ToString() + CharSeperator + myRelease.Info + CharSeperator + myRelease.Link);
					}
					Helper.FileHandling.WriteStringToFileOverwrite(FilePath, temp.ToArray());
				}
			}
		}
	}
}
