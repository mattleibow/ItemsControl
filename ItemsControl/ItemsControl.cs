using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;

namespace Xamarin.Forms.Extended
{
	public class ItemsControl : Layout<View>
	{
		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(ItemsControl), default(IEnumerable), propertyChanged: OnItemsSourceChanged);

		public static readonly BindableProperty ItemTemplateProperty =
			BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(ItemsControl), default(DataTemplate), propertyChanged: OnItemsTemplateChanged);

		public static readonly BindableProperty ItemsPanelProperty =
			BindableProperty.Create(nameof(ItemsPanel), typeof(Layout<View>), typeof(ItemsControl), default(Layout<View>), propertyChanged: OnItemsPanelChanged);

		public static readonly BindableProperty DisplayMemberPathProperty =
			BindableProperty.Create(nameof(DisplayMemberPath), typeof(string), typeof(ItemsControl), default(string));

		public IEnumerable ItemsSource
		{
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public DataTemplate ItemTemplate
		{
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}

		public Layout<View> ItemsPanel
		{
			get { return (Layout<View>)GetValue(ItemsPanelProperty); }
			set { SetValue(ItemsPanelProperty, value); }
		}

		public string DisplayMemberPath
		{
			get { return (string)GetValue(DisplayMemberPathProperty); }
			set { SetValue(DisplayMemberPathProperty, value); }
		}

		public Layout<View> ItemsPanelRoot { get; private set; }

		protected override void LayoutChildren(double x, double y, double width, double height)
		{
			for (int i = 0; i < LogicalChildren.Count; i++)
			{
				if (LogicalChildren[i] is View view)
				{
					LayoutChildIntoBoundingRegion(view, new Rectangle(x, y, width, height));
				}
			}
		}

		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			var widthRequest = WidthRequest;
			var heightRequest = HeightRequest;
			var sizeRequest = default(SizeRequest);

			if ((widthRequest == -1.0 || heightRequest == -1.0) && ItemsPanelRoot != null)
			{
				sizeRequest = ItemsPanelRoot.Measure(widthConstraint, heightConstraint, MeasureFlags.IncludeMargins);
			}

			return new SizeRequest
			{
				Request = new Size
				{
					Width = (widthRequest != -1.0) ? widthRequest : sizeRequest.Request.Width,
					Height = (heightRequest != -1.0) ? heightRequest : sizeRequest.Request.Height
				},
				Minimum = sizeRequest.Minimum
			};
		}

		private void UpdateItemsPanel(Layout<View> oldLayout = null, Layout<View> newLayout = null)
		{
			newLayout = newLayout ?? new StackLayout();
			var hadRoot = ItemsPanelRoot == null;

			// clean up the old root
			if (oldLayout != null)
			{
				// move the items to the new root
				foreach (var child in oldLayout.Children.ToArray())
				{
					newLayout.Children.Add(child);
				}

				// remove the old layout from this view
				if (Children.Contains(oldLayout))
				{
					Children.Remove(oldLayout);
				}
			}

			// add the new root to this view
			if (!Children.Contains(newLayout))
			{
				Children.Add(newLayout);
			}

			// set the new root property
			ItemsPanelRoot = newLayout;

			// if there was no root, we must make sure we add the items
			if (!hadRoot)
			{
				UpdateItems();
			}
		}

		private void UpdateItems()
		{
			// if there is no root, we need to create one
			if (ItemsPanelRoot == null)
			{
				UpdateItemsPanel();
			}

			// clear the old children
			// TODO: maybe support reusing/moving the views
			if (ItemsPanelRoot.Children.Count > 0)
			{
				ItemsPanelRoot.Children.Clear();
			}

			// add the new views
			if (ItemsSource != null)
			{
				foreach (var item in ItemsSource)
				{
					var content = GetItem(item);
					ItemsPanelRoot.Children.Add(content);
				}
			}
		}

		private View GetItem(object context)
		{
			var template = ItemTemplate;

			// we have a selector
			if (template is DataTemplateSelector selector)
			{
				template = selector.SelectTemplate(context, this);
			}

			// we have a template
			var item = (View)template?.CreateContent();

			// we have nothing
			if (item == null)
			{
				item = new Label();
				item.SetBinding(Label.TextProperty, string.IsNullOrWhiteSpace(DisplayMemberPath) ? "." : DisplayMemberPath);
			}

			// set the context
			item.BindingContext = context;

			return item;
		}

		private void OnItemsSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateItems();
		}

		private static void OnItemsPanelChanged(BindableObject bindable, object oldValue, object newValue)
		{
			((ItemsControl)bindable).UpdateItemsPanel(oldValue as Layout<View>, newValue as Layout<View>);
		}

		private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var control = ((ItemsControl)bindable);

			if (oldValue is INotifyCollectionChanged oldNotify)
				oldNotify.CollectionChanged -= control.OnItemsSourceChanged;

			control.UpdateItems();

			if (newValue is INotifyCollectionChanged newNotify)
				newNotify.CollectionChanged += control.OnItemsSourceChanged;
		}

		private static void OnItemsTemplateChanged(BindableObject bindable, object oldValue, object newValue)
		{
			((ItemsControl)bindable).UpdateItems();
		}
	}
}
