// 
// TextEntryBackend.cs
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
using Xwt.Backends;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;


namespace Xwt.Mac
{
	public class TextEntryBackend: ViewBackend<NSView,ITextEntryEventSink>, ITextEntryBackend
	{
		public TextEntryBackend ()
		{
		}
		
		internal TextEntryBackend (MacComboBox field)
		{
			ViewObject = field;
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			if (ViewObject is MacComboBox) {
				((MacComboBox)ViewObject).SetEntryEventSink (EventSink);
			} else {
				ViewObject = new CustomAlignedContainer (new CustomTextField (EventSink, ApplicationContext));
				MultiLine = false;
			}
		}
		
		protected override void OnSizeToFit ()
		{
			Container.SizeToFit ();
		}

		CustomAlignedContainer Container {
			get { return base.Widget as CustomAlignedContainer; }
		}

		public new NSTextField Widget {
			get { return (ViewObject is MacComboBox) ? (NSTextField)ViewObject : (NSTextField) Container.Child; }
		}

		protected override Size GetNaturalSize ()
		{
			var s = base.GetNaturalSize ();
			return new Size (EventSink.GetDefaultNaturalSize ().Width, s.Height);
		}

		#region ITextEntryBackend implementation
		public string Text {
			get {
				return Widget.StringValue;
			}
			set {
				Widget.StringValue = value ?? string.Empty;
			}
		}

		public Alignment TextAlignment {
			get {
				return Widget.Alignment.ToAlignment ();
			}
			set {
				Widget.Alignment = value.ToNSTextAlignment ();
			}
		}

		public bool ReadOnly {
			get {
				return !Widget.Editable;
			}
			set {
				Widget.Editable = !value;
			}
		}

		public bool ShowFrame {
			get {
				return Widget.Bordered;
			}
			set {
				Widget.Bordered = value;
			}
		}
		
		public string PlaceholderText {
			get {
				return ((NSTextFieldCell) Widget.Cell).PlaceholderString;
			}
			set {
				((NSTextFieldCell) Widget.Cell).PlaceholderString = value;
			}
		}

		public bool MultiLine {
			get {
				if (Widget is MacComboBox)
					return false;
				return Widget.Cell.UsesSingleLineMode;
			}
			set {
				if (Widget is MacComboBox)
					return;
				if (value) {
					Widget.Cell.UsesSingleLineMode = false;
					Widget.Cell.Scrollable = false;
					Widget.Cell.Wraps = true;
				} else {
					// Widget.Cell.UsesSingleLineMode = true causes the vertical alignment of large fonts to be wrong - the top of text gets cut off
//					Widget.Cell.UsesSingleLineMode = true; 
					Widget.Cell.Scrollable = true;
					Widget.Cell.Wraps = false;
				}
				Container.ExpandVertically = value;
			}
		}

		public override void SetFocus ()
		{
			Widget.BecomeFirstResponder ();
		}

		public override bool HasFocus {
			get {
				if (NSApplication.SharedApplication.KeyWindow == null) {
					return false;
				}
				if(NSApplication.SharedApplication.KeyWindow.FirstResponder == Widget) {
					return true;
				}

				NSTextView textView = NSApplication.SharedApplication.KeyWindow.FirstResponder as NSTextView;

				if(textView != null && textView.WeakDelegate == Widget) {
					return true;
				}

				return false;
			}
		}
		#endregion

		public override Xwt.Drawing.Color BackgroundColor {
			get { return Widget.BackgroundColor.ToXwtColor(); }
			set {
				((NSTextFieldCell) Widget.Cell).BackgroundColor = value.ToNSColor();
				Widget.BackgroundColor = value.ToNSColor();
			}
		}

		public override Xwt.Drawing.Color TextColor {
			get { return Widget.TextColor.ToXwtColor(); }
			set {
				((NSTextFieldCell) Widget.Cell).TextColor = value.ToNSColor();
				Widget.TextColor = value.ToNSColor();
			}
		}

	}
	
	class CustomTextField: NSTextField, IViewObject
	{
		ITextEntryEventSink eventSink;
		ApplicationContext context;
		
		public CustomTextField (ITextEntryEventSink eventSink, ApplicationContext context)
		{
			this.context = context;
			this.eventSink = eventSink;
			this.Delegate = new CustomTextFieldDelegate(eventSink);
		}

		private class CustomTextFieldDelegate : NSTextFieldDelegate {
			private ITextEntryEventSink eventSink;

			private static readonly Selector moveUpSelector = new Selector("moveUp:");
			private static readonly Selector moveDownSelector = new Selector("moveDown:");
			private static readonly Selector scrollPageUpSelector = new Selector("scrollPageUp:");
			private static readonly Selector scrollPageDownSelector = new Selector("scrollPageDown:");
			private static readonly Selector insertNewlineSelector = new Selector("insertNewline:");
			private static readonly Selector cancelOperationSelector = new Selector("cancelOperation:");

			public CustomTextFieldDelegate(ITextEntryEventSink eventSink) {
				this.eventSink = eventSink;
			}

			public override bool DoCommandBySelector(NSControl control, NSTextView textView, Selector commandSelector) {
				if(commandSelector == moveUpSelector) {
					eventSink.OnKeyPressed(new KeyEventArgs(Key.Up, default(ModifierKeys), false, 0));
				}
				else if(commandSelector == moveDownSelector) {
					eventSink.OnKeyPressed(new KeyEventArgs(Key.Down, default(ModifierKeys), false, 0));
				}
				else if(commandSelector == scrollPageUpSelector) {
					eventSink.OnKeyPressed(new KeyEventArgs(Key.PageUp, default(ModifierKeys), false, 0));
				}
				else if(commandSelector == scrollPageDownSelector) {
					eventSink.OnKeyPressed(new KeyEventArgs(Key.PageDown, default(ModifierKeys), false, 0));
				}
				else if(commandSelector == insertNewlineSelector) {
					eventSink.OnKeyPressed(new KeyEventArgs(Key.Return, default(ModifierKeys), false, 0));
				}
				else if(commandSelector == cancelOperationSelector) {
					eventSink.OnKeyPressed(new KeyEventArgs(Key.Escape, default(ModifierKeys), false, 0));
				}

				return false;
			}
		}
		
		public NSView View {
			get {
				return this;
			}
		}

		public override bool PerformKeyEquivalent(NSEvent theEvent) {
			if (theEvent.Type == NSEventType.KeyDown) {
				NSApplication app = NSApplication.SharedApplication;
				if ((theEvent.ModifierFlags & NSEventModifierMask.DeviceIndependentModifierFlagsMask) == NSEventModifierMask.CommandKeyMask) {
					string ch = theEvent.CharactersIgnoringModifiers;
					if (ch == "x") {
						return app.SendAction(new Selector("cut:"), this.Window.FirstResponder, this);
					}
					if (ch == "c") {
						return app.SendAction(new Selector("copy:"), this.Window.FirstResponder, this);
					}
					if (ch == "v") {
						return app.SendAction(new Selector("paste:"), this.Window.FirstResponder, this);
					}
					if (ch == "a") {
						return app.SendAction(new Selector("selectAll:"), this.Window.FirstResponder, this);
					}
				}
			}
			return base.PerformKeyEquivalent(theEvent);
		}

		public ViewBackend Backend { get; set; }
		
		public override void DidChange (MonoMac.Foundation.NSNotification notification)
		{
			base.DidChange (notification);
			context.InvokeUserCode (delegate {
				eventSink.OnChanged ();
			});
		}
	}
}

