using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace ItemsControlApp
{
	public class SimpleViewModel : BindableObject
	{
		private readonly Random random = new Random();

		public SimpleViewModel()
		{
			Items = new ObservableCollection<DataItem>
			{
				new DataItem("One"),
				new DataItem("Two"),
				new DataItem("Three"),
				new DataItem("Four"),
				new DataItem("Five"),
			};

			AddItemCommand = new Command(AddItem);
			RemoveItemCommand = new Command(RemoveItem);
		}

		public ObservableCollection<DataItem> Items { get; set; }

		public ICommand AddItemCommand { get; }

		public ICommand RemoveItemCommand { get; }

		public void AddItem()
		{
			Items.Add(new DataItem(random.Next(10000).ToString()));
		}

		private void RemoveItem(object param)
		{
			if (param is DataItem item)
			{
				Items.Remove(item);
			}
		}
	}
}
