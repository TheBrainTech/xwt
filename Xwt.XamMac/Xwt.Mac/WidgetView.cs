// 
// WidgetView.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
//       Hywel Thomas <hywel.w.thomas@gmail.com>
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
using AppKit;
using CoreGraphics;
using Xwt.Backends;

namespace Xwt.Mac
{
	/// <summary>
	/// Handles Events generated by NSView and TrackingArea
	/// and dispatches these using context and eventSink
	/// </summary>
	public class WidgetView: NSView, IViewObject
	{
		IWidgetEventSink eventSink;
		protected ApplicationContext context;

		NSTrackingArea trackingArea;	// Captures Mouse Entered, Exited, and Moved events

		public WidgetView (IWidgetEventSink eventSink, ApplicationContext context)
		{
			this.context = context;
			this.eventSink = eventSink;
			DrawsBackground = true;
		}

		public ViewBackend Backend { get; set; }

		public NSView View {
			get { return this; }
		}

		public bool DrawsBackground { get; set; }

		public override bool IsFlipped {
			get {
				return true;
			}
		}
		
		public override bool MouseDownCanMoveWindow {
			get {
				return Backend.MouseDownCanMoveWindow;
			}
		}

		public override bool AcceptsFirstResponder ()
		{
			if(Backend is TextEntryBackend) {
				// new fix for Tab key navigation of TextEntry requiring two presses of Tab key
				// changed because removing this function makes it impossible to add focus to canvas based buttons
				return false;
			}
			return Backend.CanGetFocus;
		}

		public override void DrawRect (CGRect dirtyRect)
		{
			if (DrawsBackground) {
				CGContext ctx = NSGraphicsContext.CurrentContext.GraphicsPort;

				//fill BackgroundColor
				ctx.SetFillColor (Backend.Frontend.BackgroundColor.ToCGColor ());
				ctx.FillRect (Bounds);
			}
		}

		public override void UpdateTrackingAreas ()
		{
			this.UpdateEventTrackingArea (ref trackingArea);
		}

		public override void RightMouseDown (NSEvent theEvent)
		{
			if (!this.HandleMouseDown (theEvent))
				base.RightMouseDown (theEvent);
		}

		public override void RightMouseUp (NSEvent theEvent)
		{
			if (!this.HandleMouseUp (theEvent))
				base.RightMouseUp (theEvent);
		}

		public override void MouseDown (NSEvent theEvent)
		{
			if (!this.HandleMouseDown (theEvent))
				base.MouseDown (theEvent);
		}

		public override void MouseUp (NSEvent theEvent)
		{
			if (!this.HandleMouseUp (theEvent))
				base.MouseUp (theEvent);
		}

		public override void OtherMouseDown (NSEvent theEvent)
		{
			if (!this.HandleMouseDown (theEvent))
				base.OtherMouseDown (theEvent);
		}

		public override void OtherMouseUp (NSEvent theEvent)
		{
			if (!this.HandleMouseUp (theEvent))
				base.OtherMouseUp (theEvent);
		}

		public override void OtherMouseDown(NSEvent theEvent) {
			var p = ConvertPointFromView(theEvent.LocationInWindow, null);
			ButtonEventArgs args = new ButtonEventArgs();
			args.X = p.X;
			args.Y = p.Y;
			switch(theEvent.ButtonNumber) {
			case 2:
				args.Button = PointerButton.Middle;
				break;
			case 3:
				args.Button = PointerButton.ExtendedButton1;
				break;
			case 4:
				args.Button = PointerButton.ExtendedButton2;
				break;
			}
			args.MultiplePress = (int)theEvent.ClickCount;
			context.InvokeUserCode(delegate {
				eventSink.OnButtonPressed(args);
			});
		}

		public override void OtherMouseUp(NSEvent theEvent) {
			var p = ConvertPointFromView(theEvent.LocationInWindow, null);
			ButtonEventArgs args = new ButtonEventArgs();
			args.X = p.X;
			args.Y = p.Y;
			switch(theEvent.ButtonNumber) {
			case 2:
				args.Button = PointerButton.Middle;
				break;
			case 3:
				args.Button = PointerButton.ExtendedButton1;
				break;
			case 4:
				args.Button = PointerButton.ExtendedButton2;
				break;
			}
			args.MultiplePress = (int)theEvent.ClickCount;
			context.InvokeUserCode(delegate {
				eventSink.OnButtonReleased(args);
			});
		}

		public override void MouseEntered (NSEvent theEvent)
		{
			this.HandleMouseEntered (theEvent);
		}

		public override void MouseExited (NSEvent theEvent)
		{
			this.HandleMouseExited (theEvent);
		}

		public override void MouseMoved (NSEvent theEvent)
		{
			if (!this.HandleMouseMoved (theEvent))
				base.MouseMoved (theEvent);
		}

		public override void RightMouseDragged (NSEvent theEvent)
		{
			if (!this.HandleMouseMoved (theEvent))
				base.RightMouseDragged (theEvent);
		}

		public override void MouseDragged (NSEvent theEvent)
		{
			if (!this.HandleMouseMoved (theEvent))
				base.MouseDragged (theEvent);
		}

		public override void OtherMouseDragged (NSEvent theEvent)
		{
			if (!this.HandleMouseMoved (theEvent))
				base.OtherMouseDragged (theEvent);
		}

		public override void KeyDown (NSEvent theEvent)
		{
			if (!this.HandleKeyDown (theEvent))
				base.KeyDown (theEvent);
		}

		public override void KeyUp (NSEvent theEvent)
		{
			if (!this.HandleKeyUp (theEvent))
				base.KeyUp (theEvent);
		}

		public override void SetFrameSize (CGSize newSize)
		{
			bool changed = !newSize.Equals (Frame.Size);
			base.SetFrameSize (newSize);
			if (changed) {
				context.InvokeUserCode (eventSink.OnBoundsChanged);
			}
		}

		public override void ResetCursorRects ()
		{
			base.ResetCursorRects ();
			if (Backend.Cursor != null)
				AddCursorRect (Bounds, Backend.Cursor);
		}
	}
}

