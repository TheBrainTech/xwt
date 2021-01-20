// 
// ComboBoxBackend.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

// 
// ComboBoxBackend.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using AppKit;
using Xwt.Backends;

namespace Xwt.Mac
{
	public class ComboBoxBackend: ViewBackend<NSPopUpButton,IComboBoxEventSink>, IComboBoxBackend
	{
		private MenuDelegate menuDelegate;
		IListDataSource source;

		// On macOS BigSur, pop-up style renders with an extra overlay of the arrows icon on top of everything.
		// Seems to be a bug in macOS as it occurs even when XWT is not used to create the control.
		// PullDown style is not right according to Apple guidelines for the use case but it works well.
		public const bool UsePullDownStyle = true;
		
		public ComboBoxBackend ()
		{
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			ViewObject = new PopUpButton ();
			Widget.Menu = new NSMenu ();
			Widget.Activated += delegate {
				if(UsePullDownStyle) {
					Widget.SetTitle((string)source.GetValue(SelectedRow, 0) ?? "");
				}
				ApplicationContext.InvokeUserCode (EventSink.OnSelectionChanged);
				Widget.SynchronizeTitleAndSelectedItem ();
				ResetFittingSize();
			};

			menuDelegate = new MenuDelegate();
			this.Widget.Menu.Delegate = menuDelegate;
		}

		#region IComboBoxBackend implementation
		public bool IsDropDownOpen {
			get { return menuDelegate.IsMenuOpen; }
		}

		public void SetViews (CellViewCollection views)
		{
		}

		public void SetSource (IListDataSource s, IBackend sourceBackend)
		{
			if (source != null) {
				source.RowInserted -= HandleSourceRowInserted;
				source.RowDeleted -= HandleSourceRowDeleted;
				source.RowChanged -= HandleSourceRowChanged;
				source.RowsReordered -= HandleSourceRowsReordered;
			}
			
			source = s;
			Widget.Menu = new NSMenu ();
			this.Widget.Menu.Delegate = menuDelegate;
			
			if (source != null) {
				source.RowInserted += HandleSourceRowInserted;
				source.RowDeleted += HandleSourceRowDeleted;
				source.RowChanged += HandleSourceRowChanged;
				source.RowsReordered += HandleSourceRowsReordered;

				if(UsePullDownStyle) {
					NSMenuItem empty = new NSMenuItem();
					empty.Title = "";
					Widget.Menu.AddItem(empty);
				}

				for(int n=0; n<source.RowCount; n++) {
					if (EventSink.RowIsSeparator (n))
						Widget.Menu.AddItem (NSMenuItem.SeparatorItem);
					else {
						NSMenuItem it = new NSMenuItem ();
						UpdateItem (it, n);
						Widget.Menu.AddItem (it);
					}
				}
			}
		}

		void HandleSourceRowsReordered (object sender, ListRowOrderEventArgs e)
		{
		}

		void HandleSourceRowChanged (object sender, ListRowEventArgs e)
		{
			int offset = UsePullDownStyle ? 1 : 0;
			NSMenuItem mi = Widget.ItemAtIndex (e.Row + offset);
			if (EventSink.RowIsSeparator (e.Row)) {
				if (!mi.IsSeparatorItem) {
					Widget.Menu.InsertItem (NSMenuItem.SeparatorItem, e.Row + offset);
					Widget.Menu.RemoveItemAt (e.Row + offset + 1);
				}
			}
			else {
				if (mi.IsSeparatorItem) {
					mi = new NSMenuItem ();
					Widget.Menu.InsertItem (mi, e.Row + offset);
					Widget.Menu.RemoveItemAt (e.Row + offset + 1);
				}
				UpdateItem (mi, e.Row);
				Widget.SynchronizeTitleAndSelectedItem ();
			}
			ResetFittingSize ();
		}

		void HandleSourceRowDeleted (object sender, ListRowEventArgs e)
		{
			int offset = UsePullDownStyle ? 1 : 0;
			Widget.RemoveItem (e.Row + offset);
			Widget.SynchronizeTitleAndSelectedItem ();
			ResetFittingSize ();
		}

		void HandleSourceRowInserted (object sender, ListRowEventArgs e)
		{
			int offset = UsePullDownStyle ? 1 : 0;
			NSMenuItem mi;
			if (EventSink.RowIsSeparator (e.Row))
				mi = NSMenuItem.SeparatorItem;
			else {
				mi = new NSMenuItem ();
				UpdateItem (mi, e.Row);
			}
			Widget.Menu.InsertItem (mi, e.Row + offset);
			Widget.SynchronizeTitleAndSelectedItem ();
			ResetFittingSize ();
		}
		
		void UpdateItem (NSMenuItem mi, int index)
		{
			mi.Title = (string) source.GetValue (index, 0) ?? "";
		}

		public int SelectedRow {
			get {
				int offset = UsePullDownStyle ? 1 : 0;
				return (int) Widget.IndexOfSelectedItem - offset;
			}
			set {
				int offset = UsePullDownStyle ? 1 : 0;
				Widget.SelectItem (value + offset);
				if(UsePullDownStyle && source != null && value >= 0) {
					Widget.SetTitle((string)source.GetValue(value, 0) ?? "");
				}
				ApplicationContext.InvokeUserCode (EventSink.OnSelectionChanged);
				Widget.SynchronizeTitleAndSelectedItem ();
				ResetFittingSize();
			}
		}

		public override bool Sensitive {
			get {
				return Widget.Enabled;
			}
			set {
				Widget.Enabled = value;
			}
		}
		#endregion

		private class MenuDelegate : NSMenuDelegate {
			public bool IsMenuOpen { get; private set; }

			public override void MenuDidClose(NSMenu menu) {
				this.IsMenuOpen = false;
			}

			public override void MenuWillOpen(NSMenu menu) {
				this.IsMenuOpen = true;
			}

			public override void MenuWillHighlightItem(NSMenu menu, NSMenuItem item) {
			}
		}
	}

	class PopUpButton : NSPopUpButton, IViewObject
	{

		public PopUpButton() : base() {
			// On macOS Big Sur, when PullsDown is false (the default), an extra copy of the arrows icon at the right of the box is stretched
			// across the entire control. This happens both when created via XWT and if created natively and added to the dialog.
			PullsDown = ComboBoxBackend.UsePullDownStyle;
		}

		public NSView View {
			get {
				return this;
			}
		}

		public ViewBackend Backend { get; set; }

		public override void ResetCursorRects ()
		{
			base.ResetCursorRects ();
			if (Backend.Cursor != null)
				AddCursorRect (Bounds, Backend.Cursor);
		}

		public override bool AllowsVibrancy {
			get {
				// we don't support vibrancy
				if (EffectiveAppearance.AllowsVibrancy)
					return false;
				return base.AllowsVibrancy;
			}
		}
	}
}

