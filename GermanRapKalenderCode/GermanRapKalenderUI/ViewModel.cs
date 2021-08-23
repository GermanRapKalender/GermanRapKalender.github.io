using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GermanRapKalenderClassLibaries;
using GermanRapKalenderClassLibaries.Helper;
using GermanRapKalenderClassLibaries.Scraping;

namespace GermanRapKalenderUI
{
	public class ViewModel
	{
		public ObservableCollection<CalenderEntry> CalenderEntries { get; set; }

		public CalenderEntry SelectedRelease { get; set; }

		private DelegateCommand<CalenderEntry> _deleteRow;

		public DelegateCommand<CalenderEntry> DeleteRow =>
			_deleteRow ?? (_deleteRow = new DelegateCommand<CalenderEntry>(DeleteCommandName));

		public void Change(int index, string link)
		{
			CalenderEntry tmp = this.CalenderEntries[index];

			this.CalenderEntries.RemoveAt(index);

			tmp.Links = link;

			this.CalenderEntries.Insert(index, tmp);

			CSVHelper.Save(Globals.CurrCSVFile, MainWindow.MW.viewModel.CalenderEntries, true);
		}

		void DeleteCommandName(CalenderEntry parameter)
		{
			MyRemove(parameter);
		}


		public bool DoesContainAlready(IList<CalenderEntry> myList, CalenderEntry potentialNewRelease)
		{
			foreach (CalenderEntry existingRelease in myList)
			{
				if ((existingRelease.Artist == potentialNewRelease.Artist) && (existingRelease.Title == potentialNewRelease.Title))
				{
					return true;
				}
			}
			return false;
		}


		public void MyAdd(CalenderEntry myRelease, bool SkipSaving = false)
		{
			if (!DoesContainAlready(this.CalenderEntries, myRelease))
			{
				this.CalenderEntries.Add(myRelease);
				if (!SkipSaving)
				{
					CSVHelper.Save(Globals.CurrCSVFile, MainWindow.MW.viewModel.CalenderEntries, Settings.AutoSave);
				}
			}
		}


		public void MyAdd(IList<CalenderEntry> newReleases)
		{
			foreach (CalenderEntry myRelease in newReleases)
			{
				MyAdd(myRelease, true);
			}
			CSVHelper.Save(Globals.CurrCSVFile, MainWindow.MW.viewModel.CalenderEntries, Settings.AutoSave);
		}



		public IList<CalenderEntry> MyAdd(IList<CalenderEntry> myExistingRelease, CalenderEntry newRelease)
		{
			IList<CalenderEntry> rtrn = myExistingRelease;

			if (!DoesContainAlready(rtrn, newRelease))
			{
				rtrn.Add(newRelease);
			}

			return rtrn;
		}


		public IList<CalenderEntry> MyAdd(IList<CalenderEntry> myExistingRelease, IList<CalenderEntry> newReleases)
		{
			IList<CalenderEntry> rtrn = myExistingRelease;

			foreach (CalenderEntry myRelease in newReleases)
			{
				if (!DoesContainAlready(rtrn, myRelease))
				{
					rtrn.Add(myRelease);
				}
			}

			return rtrn;
		}


		public void MyRemove(CalenderEntry newRelease)
		{
			if (newRelease is null)
				return;

			if (this.CalenderEntries.Contains(newRelease))
				this.CalenderEntries.Remove(newRelease);
		}

		public void Clear()
		{
			while (this.CalenderEntries.Count > 0)
			{
				this.CalenderEntries.RemoveAt(0);
			}
		}
	}
}
