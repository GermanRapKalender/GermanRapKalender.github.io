using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIRM.Helper
{
	public enum CalenderEntryTypes
	{
		Album,
		Single,
		Event
	}


	public class CalenderEntry 
	{
		public string _artist { get; set; } = "";
		public string _title { get; set; } = "";
		public string _link { get; set; } = "";
		public string _info { get; set; } = "";
		public string _date { get; set; } = "";

		public CalenderEntryTypes CalenderEntryType { get; set; }

		public string Date
		{
			get { return _date; }
			set
			{
				_date  = value;
			}
		}

		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
			}
		}

		public string Artist
		{
			get { return _artist; }
			set
			{
				_artist = value;
			}
		}

		public string Link
		{
			get
			{
				return _link;
			}
			set
			{
				_link = value;

			}
		}

		public string Info
		{
			get
			{
				return _info;
			}
			set
			{
				_info = value;

			}
		}


		public string SearchString
		{
			get
			{
				return this.Artist.Replace(" &", "&").Replace("&","") + " " + this.Title;
			}
		}





		public string GetInfoTabFromRelease()
		{
			string rtrn = "";
			string[] links = this.Link.Split(' ');

			foreach (string link in links)
			{
				if (!String.IsNullOrWhiteSpace(link))
				{
					string tmplink = link;
					tmplink = tmplink.Replace(" ", "");
					tmplink = tmplink.Replace(",", "");

					if (tmplink.ToLower().Contains("spotify"))
					{
						rtrn += "[Spotify](" + tmplink + ") - ";
					}
					else if (tmplink.ToLower().Contains("youtu"))
					{
						rtrn += "[Youtube](" + tmplink + ") - ";
					}
					else if (tmplink.ToLower().Contains("youtu"))
					{
						rtrn += "[Link](" + tmplink + ") - ";
					}
					else if (tmplink.ToLower().Contains("instagram"))
					{
						rtrn += "[Instagram](" + tmplink + ") - ";
					}
					else if (tmplink.ToLower().Contains("twitter"))
					{
						rtrn += "[Twitter](" + tmplink + ") - ";
					}
					else if (tmplink.ToLower().Contains("facebook"))
					{
						rtrn += "[Facebook](" + tmplink + ") - ";
					}
					else 
					{
						rtrn += "[Link](" + tmplink + ") - ";
					}
				}
			}

			rtrn = rtrn.TrimEnd(' ').TrimEnd('-');

			if (!String.IsNullOrWhiteSpace(this.Info))
			{
				rtrn += this.Info;
			}

			if (String.IsNullOrEmpty(rtrn))
			{
				rtrn += " ";
			}

			return rtrn;
		}


		//public Release(string pArtist, string pTitle, ReleaseKinds pReleaseKind)
		//{
		//	this.Artist = pArtist;
		//	this.Title = pTitle;
		//	this.ReleaseKind = pReleaseKind;
		//}

		//public DataGridItem(string pName, bool pIsAwesome)
		//{
		//	this.Name = pName;
		//	this.IsAwesome = pIsAwesome;
		//}
	}
}
