using Xamarin.Forms;

namespace ItemsControlApp
{
	public class DataItem : BindableObject
	{
		public DataItem(string item)
		{
			Item = item;
		}

		public string Item { get; }

		public string Name => $"Item {Item}";

		// these members are just needed for the grid

		public DataItem(string item, int x, int y)
			: this(item)
		{
			X = x;
			Y = y;
		}

		public int X { get; set; }

		public int Y { get; set; }

		public void Update()
		{
			OnPropertyChanged(nameof(X));
			OnPropertyChanged(nameof(Y));
			OnPropertyChanged(nameof(Item));
			OnPropertyChanged(nameof(Name));
		}
	}
}
