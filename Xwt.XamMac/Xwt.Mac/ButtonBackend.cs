// 
// ButtonBackend.cs
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
using Xwt.Drawing;

#if MONOMAC
using nint = System.Int32;
using nfloat = System.Single;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using CGRect = System.Drawing.RectangleF;
#else
using AppKit;
using CoreGraphics;
#endif

namespace Xwt.Mac
{
	public class ButtonBackend: ViewBackend<NSButton,IButtonEventSink>, IButtonBackend
	{
		ButtonType currentType;
		ButtonStyle currentStyle = ButtonStyle.Normal;

		public ButtonBackend ()
		{
		}

		#region IButtonBackend implementation
		public override void Initialize ()
		{
			ViewObject = new MacButton (EventSink, ApplicationContext);
			Widget.SetButtonType (NSButtonType.MomentaryPushIn);
		}

		Drawing.Color textColor = Drawing.Colors.Black;
		public override Xwt.Drawing.Color TextColor {
			get {
				//Foundation.NSRange range;
				//var attributes = Widget.AttributedTitle.GetCoreTextAttributes(0, out range);
				//CoreGraphics.CGColor color = attributes.ForegroundColor;
				// HACK: return internally tracked color because retrieving the color from the attributed title using the above code causes a crash
				return textColor;
			}
			set {
				var paragraphStyle = new NSMutableParagraphStyle() {
					Alignment = NSTextAlignment.Center
				};
				var title = new Foundation.NSAttributedString(
					Widget.Title,
					foregroundColor: value.ToNSColor(),
					paragraphStyle: paragraphStyle
				);
				Widget.AttributedTitle = title;
				textColor = value;
			}
		}

		public void EnableEvent (Xwt.Backends.ButtonEvent ev)
		{
			((MacButton)Widget).EnableEvent (ev);
		}

		public void DisableEvent (Xwt.Backends.ButtonEvent ev)
		{
			((MacButton)Widget).DisableEvent (ev);
		}
		
		public void SetContent (string label, bool useMnemonic, ImageDescription image, ContentPosition imagePosition)
		{
			switch (((Button)Frontend).Type) {
			case ButtonType.Help:
			case ButtonType.Disclosure:
				return;
			}
			if (useMnemonic)
				label = label.RemoveMnemonic ();
			Widget.Title = label ?? "";
			TextColor = textColor; // color must be reapplied when title is changed
			if (string.IsNullOrEmpty (label))
				imagePosition = ContentPosition.Center;
			if (!image.IsNull) {
				var img = image.ToNSImage ();
				Widget.Image = (NSImage)img;
				Widget.Cell.ImageScale = NSImageScale.None;
				switch (imagePosition) {
				case ContentPosition.Bottom: Widget.ImagePosition = NSCellImagePosition.ImageBelow; break;
				case ContentPosition.Left: Widget.ImagePosition = NSCellImagePosition.ImageLeft; break;
				case ContentPosition.Right: Widget.ImagePosition = NSCellImagePosition.ImageRight; break;
				case ContentPosition.Top: Widget.ImagePosition = NSCellImagePosition.ImageAbove; break;
				case ContentPosition.Center: Widget.ImagePosition = string.IsNullOrEmpty (label) ? NSCellImagePosition.ImageOnly : NSCellImagePosition.ImageOverlaps; break;
				}
			}
			SetButtonStyle (currentStyle);
			ResetFittingSize ();
		}
		
		public virtual void SetButtonStyle (ButtonStyle style)
		{
			currentStyle = style;
			if (currentType == ButtonType.Normal) {
				switch (style) {
				case ButtonStyle.Normal:
					if (Widget.Image != null
					    || Frontend.MinHeight > 0
					    || Frontend.HeightRequest > 0
					    || Widget.Title.Contains (Environment.NewLine))
						Widget.BezelStyle = NSBezelStyle.RegularSquare;
					else
						Widget.BezelStyle = NSBezelStyle.Rounded;
					#if MONOMAC
					Messaging.void_objc_msgSend_bool (Widget.Handle, selSetShowsBorderOnlyWhileMouseInside.Handle, false);
					#else
					Widget.ShowsBorderOnlyWhileMouseInside = false;
					#endif
					break;
				case ButtonStyle.Borderless:
				case ButtonStyle.Flat:
					Widget.BezelStyle = NSBezelStyle.ShadowlessSquare;
					#if MONOMAC
					Messaging.void_objc_msgSend_bool (Widget.Handle, selSetShowsBorderOnlyWhileMouseInside.Handle, true);
					#else
					Widget.ShowsBorderOnlyWhileMouseInside = true;
					#endif
					break;
				case ButtonStyle.AlwaysBorderless:
					Widget.BezelStyle = NSBezelStyle.ShadowlessSquare;
					Widget.Bordered = false;
					Widget.SetButtonType (NSButtonType.MomentaryChange);
					break;
				case ButtonStyle.CompactFlatMomentary:
					Widget.BezelStyle = NSBezelStyle.ShadowlessSquare;
					#if MONOMAC
					Messaging.void_objc_msgSend_bool (Widget.Handle, selSetShowsBorderOnlyWhileMouseInside.Handle, true);
					#else
					Widget.ShowsBorderOnlyWhileMouseInside = true;
					#endif
					break;
				case ButtonStyle.CompactFlatToggle:
					Widget.BezelStyle = NSBezelStyle.ShadowlessSquare;
					Widget.SetButtonType (NSButtonType.OnOff);
					break;
				}
			}
		}
		
#if MONOMAC
		protected static Selector selSetShowsBorderOnlyWhileMouseInside = new Selector ("setShowsBorderOnlyWhileMouseInside:");
#endif

		public void SetButtonType (ButtonType type)
		{
			currentType = type;
			switch (type) {
			case ButtonType.Disclosure:
				Widget.BezelStyle = NSBezelStyle.Disclosure;
				Widget.Title = "";
				break;
			case ButtonType.Help:
				Widget.BezelStyle = NSBezelStyle.HelpButton;
				Widget.Title = "";
				break;
			default:
					SetButtonStyle (currentStyle);
				break;
			}
		}

		public bool IsToggled {
			get {
				return Widget.State == NSCellStateValue.On;
			}
			set {
				Widget.State = value ? NSCellStateValue.On : NSCellStateValue.Off;
				if(currentStyle == ButtonStyle.CompactFlatToggle) {
					#if MONOMAC
					Messaging.void_objc_msgSend_bool (Widget.Handle, selSetShowsBorderOnlyWhileMouseInside.Handle, !value);
					#else
					Widget.ShowsBorderOnlyWhileMouseInside = !value;
					#endif
				}
			}
		}
			
		public bool IsDefault {
			get {
				return Widget.KeyEquivalent == "\r";
			}
			set {
				if(value == true) {
					Widget.KeyEquivalent = "\r";
				} else {
					Widget.KeyEquivalent = "";
				}
			}
		}
		
		#endregion

		public override Color BackgroundColor {
			get { 
				if(this.Widget.Bordered) {
					return ((MacButton)Widget).BackgroundColor;
				} else {
					return base.BackgroundColor;
				}
			}
			set {
				if(this.Widget.Bordered) {
					((MacButton)Widget).BackgroundColor = value;
				} else {
					base.BackgroundColor = value;
				}
			}
		}
	}
	
	class MacButton: NSButton, IViewObject
	{
		//
		// This is necessary since the Activated event for NSControl in AppKit does 
		// not take a list of handlers, instead it supports only one handler.
		//
		// This event is used by the RadioButton backend to implement radio groups
		//
		internal event Action <MacButton> ActivatedInternal;
		IButtonEventSink eventSink;

		public MacButton (IntPtr p): base (p)
		{
		}
		
		public MacButton (IButtonEventSink eventSink, ApplicationContext context)
		{
			this.eventSink = eventSink;
			Cell = new ColoredButtonCell ();
			BezelStyle = NSBezelStyle.Rounded;
			Activated += delegate {
				context.InvokeUserCode (delegate {
					eventSink.OnClicked ();
				});
				OnActivatedInternal ();
			};
		}
		
		public MacButton ()
		{
			Activated += delegate {
				OnActivatedInternal ();
			};

		}

		public MacButton (IRadioButtonEventSink eventSink, ApplicationContext context)
		{
			Activated += delegate {
				context.InvokeUserCode (delegate {
					eventSink.OnClicked ();
				});
				OnActivatedInternal ();
			};
		}

		public ViewBackend Backend { get; set; }
		
		public NSView View {
			get { return this; }
		}
		
		public void EnableEvent (ButtonEvent ev)
		{
		}

		public void DisableEvent (ButtonEvent ev)
		{
		}

		void OnActivatedInternal ()
		{
			if (ActivatedInternal == null)
				return;

			ActivatedInternal (this);
		}

		public override void ResetCursorRects ()
		{
			base.ResetCursorRects ();
			if (Backend.Cursor != null)
				AddCursorRect (Bounds, Backend.Cursor);
		}

		public Color BackgroundColor {
			get {
				return ((ColoredButtonCell)Cell).Color.GetValueOrDefault ();
			}
			set {
				((ColoredButtonCell)Cell).Color = value;
			}
		}

		class ColoredButtonCell : NSButtonCell
		{
			public Color? Color { get; set; }

			public override void DrawBezelWithFrame (CGRect frame, NSView controlView)
			{
				controlView.DrawWithColorTransform(Color, delegate { base.DrawBezelWithFrame (frame, controlView); });
			}
		}

		public override void KeyUp(NSEvent theEvent) {
			base.KeyUp(theEvent);
			//radio button eventSink could be null
			if (eventSink != null) 
				eventSink.OnKeyReleased(theEvent.ToXwtKeyEventArgs());
		}
	}
}

