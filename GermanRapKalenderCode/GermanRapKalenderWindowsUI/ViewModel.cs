using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIRM
{
	public class ViewModel 
	{
		public ObservableCollection<Helper.CalenderEntry> CalenderEntries { get; set; }

		public Helper.CalenderEntry SelectedRelease { get; set; }

		private DelegateCommand<Helper.CalenderEntry> _deleteRow;

		public DelegateCommand<Helper.CalenderEntry> DeleteRow =>
			_deleteRow ?? (_deleteRow = new DelegateCommand<Helper.CalenderEntry>(DeleteCommandName));

		public void Change(int index, string link)
		{
			Helper.CalenderEntry tmp = this.CalenderEntries[index];

			this.CalenderEntries.RemoveAt(index);

			tmp.Link = link;

			this.CalenderEntries.Insert(index, tmp);

			Helper.CSVHelper.Save(true);
		}

		void DeleteCommandName(Helper.CalenderEntry parameter)
		{
			MyRemove(parameter);
		}


		public bool DoesContainAlready(IList<Helper.CalenderEntry> myList, Helper.CalenderEntry potentialNewRelease)
		{
			foreach (Helper.CalenderEntry existingRelease in myList)
			{
				if ((existingRelease.Artist == potentialNewRelease.Artist) && (existingRelease.Title == potentialNewRelease.Title))
				{
					return true;
				}
			}
			return false;
		}


		public void MyAdd(Helper.CalenderEntry myRelease, bool SkipSaving = false)
		{
			if (!DoesContainAlready(this.CalenderEntries, myRelease))
			{
				this.CalenderEntries.Add(myRelease);
				if (!SkipSaving)
				{
					Helper.CSVHelper.Save();
				}
			}
		}


		public void MyAdd(IList<Helper.CalenderEntry> newReleases)
		{
			foreach (Helper.CalenderEntry myRelease in newReleases)
			{
				MyAdd(myRelease, true);
			}
			Helper.CSVHelper.Save();
		}



		public IList<Helper.CalenderEntry> MyAdd(IList<Helper.CalenderEntry> myExistingRelease, Helper.CalenderEntry newRelease)
		{
			IList<Helper.CalenderEntry> rtrn = myExistingRelease;

			if (!DoesContainAlready(rtrn, newRelease))
			{
				rtrn.Add(newRelease);
			}

			return rtrn;
		}


		public IList<Helper.CalenderEntry> MyAdd(IList<Helper.CalenderEntry> myExistingRelease, IList<Helper.CalenderEntry> newReleases)
		{
			IList<Helper.CalenderEntry> rtrn = myExistingRelease;

			foreach (Helper.CalenderEntry myRelease in newReleases)
			{
				if (!DoesContainAlready(rtrn, myRelease))
				{
					rtrn.Add(myRelease);
				}
			}

			return rtrn;
		}


		public void MyRemove(Helper.CalenderEntry newRelease)
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
