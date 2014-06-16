// 
// MenuBackend.cs
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

using System;
using MonoMac.AppKit;
using Xwt.Backends;
using MonoMac.Foundation;
using System.Collections.Generic;


namespace Xwt.Mac
{
	public class MenuBackend: NSMenu, IMenuBackend
	{
		class MenuDelegate : NSMenuDelegate
		{
			IMenuEventSink eventSink;
			ApplicationContext context;

			public MenuDelegate(IMenuEventSink eventSink, ApplicationContext context)
			{
				this.eventSink = eventSink;
				this.context = context;
			}

			public override void MenuWillOpen(NSMenu menu)
			{
				context.InvokeUserCode (delegate {
					eventSink.OnOpening ();
				});
			}

			#region implemented abstract members of NSMenuDelegate

			public override void MenuWillHighlightItem(NSMenu menu, NSMenuItem item)
			{
			}

			#endregion
		}

		IMenuEventSink eventSink;
		List<MenuEvent> enabledEvents;
		ApplicationContext context;

		public void Initialize (IMenuEventSink eventSink)
		{
			this.eventSink = eventSink;
		}

		public void InitializeBackend (object frontend, ApplicationContext context)
		{
			this.context = context;
		}

		public void InsertItem (int index, IMenuItemBackend menuItem)
		{
			base.InsertItem (((MenuItemBackend)menuItem).Item, index);
		}

		public void RemoveItem (IMenuItemBackend menuItem)
		{
			RemoveItem (((MenuItemBackend)menuItem).Item);
		}
		
		public void SetMainMenuMode ()
		{
			for (int n=0; n<Count; n++) {
				var it = ItemAt (n);
				if (it.Menu != null)
					it.Submenu.Title = it.Title;
			}
		}

		public void EnableEvent (object eventId)
		{
			if (eventId is MenuEvent) {
				if(enabledEvents == null) {
					enabledEvents = new List<MenuEvent>();
				}
				enabledEvents.Add ((MenuEvent)eventId);
				if((MenuEvent)eventId == MenuEvent.Opening) {
					this.Delegate = new MenuDelegate(eventSink, context);
				}
			}
		}

		public void DisableEvent (object eventId)
		{
			if (eventId is MenuEvent) {
				enabledEvents.Remove ((MenuEvent)eventId);
				if((MenuEvent)eventId == MenuEvent.Opening) {
					this.Delegate = null;
				}
			}
		}
		
		public void Popup ()
		{
			var evt = NSApplication.SharedApplication.CurrentEvent;
			NSMenu.PopUpContextMenu (this, evt, evt.Window.ContentView, null);
		}
		
		public void Popup (IWidgetBackend widget, double x, double y)
		{
			NSMenu.PopUpContextMenu (this, NSApplication.SharedApplication.CurrentEvent, ((ViewBackend)widget).Widget, null);
		}
	}
}

