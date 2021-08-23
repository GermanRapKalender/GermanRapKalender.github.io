using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GermanRapKalenderClassLibaries.Helper
{
	public class CSVHelper
	{
		static char CharSeperator = '=';

		static string HeadingLine = "Date" + CharSeperator + "CalenderEntryType" + CharSeperator + "Artist" + CharSeperator + "Title" + CharSeperator + "Info" + CharSeperator + "Link";

		public static IList<Helper.CalenderEntry> Read(string FilePath)
		{
			IList<Helper.CalenderEntry> myReleaseList = new List<Helper.CalenderEntry>();

			string[] filecontent = Helper.FileHandling.ReadFileEachLine(FilePath);
			if (filecontent.Length > 1)
			{
				if (filecontent[0] == HeadingLine)
				{
					for (int i = 1; i <= filecontent.Length - 1; i++)
					{
						string[] Line = filecontent[i].Split(CharSeperator);
						if (Line.Length >= 5)
						{
							CalenderEntryTypes tmp = CalenderEntryTypes.Event;
							foreach (CalenderEntryTypes CET in Enum.GetValues(typeof(CalenderEntryTypes)))
							{
								if (Line[1].ToLower() == CET.ToString().ToLower())
								{
									tmp = CET;
									break;
								}
							}

							myReleaseList.Add(new Helper.CalenderEntry
							{
								Date = Line[0],
								CalenderEntryType = tmp,
								Artist = Line[2],
								Title = Line[3],
								Info = Line[4],
								Links = Line[5]
							});
						}
					}
				}
			}
			return myReleaseList;
		}



		public static void Save(string FilePath, IList<CalenderEntry> ListOfReleases, bool DoWeActuallyWantToSave)
		{
			if (DoWeActuallyWantToSave)
			{
				List<string> temp = new List<string>();
				temp.Add(HeadingLine);
				foreach (CalenderEntry myRelease in ListOfReleases)
				{
					temp.Add(myRelease.Date + CharSeperator + myRelease.CalenderEntryType.ToString() + CharSeperator + myRelease.Artist + CharSeperator + myRelease.Title + CharSeperator + myRelease.Info + CharSeperator + myRelease.Links);
				}
				Helper.FileHandling.WriteStringToFileOverwrite(FilePath, temp.ToArray());
			}
		}

		public static void Export()
		{

		}


		public enum ImportTypes
		{
			Overwrite,
			Add
		}

		public static void Import()
		{

		}
	}
}
