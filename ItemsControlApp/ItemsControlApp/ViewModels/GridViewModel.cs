namespace ItemsControlApp
{
	public class GridViewModel : SimpleViewModel
	{
		private const int NumCols = 2;

		public GridViewModel()
		{
			Items.CollectionChanged += delegate
			{
				LayoutItems();
			};
			LayoutItems();
		}

		private void LayoutItems()
		{
			for (int i = 0; i < Items.Count; i++)
			{
				var item = Items[i];
				item.X = i % NumCols;
				item.Y = i / NumCols;
				item.Update();
			}
		}
	}
}
