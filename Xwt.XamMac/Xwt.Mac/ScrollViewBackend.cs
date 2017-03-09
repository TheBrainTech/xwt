// 
// ScrollViewBackend.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc
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

#if MONOMAC
using nint = System.Int32;
using nfloat = System.Single;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using CGSize = System.Drawing.SizeF;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
#else
using AppKit;
using CoreGraphics;
#endif

namespace Xwt.Mac
{
	public class ScrollViewBackend: ViewBackend<NSScrollView,IScrollViewEventSink>, IScrollViewBackend
	{
		IWidgetBackend child;
		ScrollPolicy verticalScrollPolicy;
		ScrollPolicy horizontalScrollPolicy;
		NSClipView contentView;
		IDisposable documentView;

		const int maxBounce = 500;
		NSView bounceViewTop;
		NSView bounceViewBottom;
		NSView bounceViewLeft;
		NSView bounceViewRight;

		public override void Initialize ()
		{
			ViewObject = new CustomScrollView ();
			((CustomScrollView)ViewObject).EventSink = EventSink;
			Widget.HasHorizontalScroller = true;
			Widget.HasVerticalScroller = true;
			Widget.AutoresizesSubviews = true;
			Widget.DrawsBackground = false;
		}

		protected override void Dispose (bool disposing)
		{
			if(Widget != null) {
				if(documentView != null) {
					documentView.Dispose();
				}
				if(contentView != null) {
					contentView.Dispose();
				}
				if(child != null) {
					child.Dispose();
				}
				Widget.Dispose();
			}
			base.Dispose(disposing);
		}

		public void ScrollToPoint(Point point) {
			(Widget.DocumentView as NSView).ScrollPoint(new CGPoint((float)point.X, (float)point.Y));
		}

		protected override Size GetNaturalSize ()
		{
			return EventSink.GetDefaultNaturalSize ();
		}
		
		public void SetChild (IWidgetBackend child)
		{
			this.child = child;
			ViewBackend backend = (ViewBackend) child;
			if (backend.EventSink.SupportsCustomScrolling ()) {
				var vs = new ScrollAdjustmentBackend (Widget, true);
				var hs = new ScrollAdjustmentBackend (Widget, false);
				CustomClipView clipView = new CustomClipView (hs, vs);
				Widget.ContentView = clipView;
				contentView = clipView;
				var dummy = new DummyClipView ();
				dummy.AddSubview (backend.Widget);
				backend.Widget.Frame = new CGRect (0, 0, clipView.Frame.Width, clipView.Frame.Height);
				clipView.DocumentView = dummy;
				documentView = dummy;
				backend.EventSink.SetScrollAdjustments (hs, vs);
				vertScroll = vs;
				horScroll = hs;
			}
			else {
				NormalClipView clipView = new NormalClipView ();
				clipView.Scrolled += OnScrolled;

				// the following Wiget._____View = statemetns cause this object never to be garbage collected since the Widget gets a reference to this
				// it is because of these that Dispose() must be called on ScrollView objects in order for the memory to be released
				Widget.ContentView = clipView;
				Widget.DocumentView = backend.Widget;

				contentView = clipView;
				documentView = backend.Widget;

				UpdateChildSize ();
			}
			if(backend.BackgroundColor.Brightness < 0.5) {
				Widget.ScrollerKnobStyle = NSScrollerKnobStyle.Light;
			} else {
				Widget.ScrollerKnobStyle = NSScrollerKnobStyle.Dark;
			}
			// make the "bounce" area of the scroll view the same color as the content
			CGColor backgroundColor = new CGColor((float)backend.BackgroundColor.Red, (float)backend.BackgroundColor.Green, (float)backend.BackgroundColor.Blue);
			bounceViewTop = new NSView();
			bounceViewTop.WantsLayer = true;
			bounceViewTop.Layer.BackgroundColor = backgroundColor;
			Widget.ContentView.AddSubview(bounceViewTop);
			bounceViewBottom = new NSView();
			bounceViewBottom.WantsLayer = true;
			bounceViewBottom.Layer.BackgroundColor = backgroundColor;
			Widget.ContentView.AddSubview(bounceViewBottom);
			bounceViewLeft = new NSView();
			bounceViewLeft.WantsLayer = true;
			bounceViewLeft.Layer.BackgroundColor = backgroundColor;
			Widget.ContentView.AddSubview(bounceViewLeft);
			bounceViewRight = new NSView();
			bounceViewRight.WantsLayer = true;
			bounceViewRight.Layer.BackgroundColor = backgroundColor;
			Widget.ContentView.AddSubview(bounceViewRight);
		}
		
		public ScrollPolicy VerticalScrollPolicy {
			get {
				return verticalScrollPolicy;
			}
			set {
				verticalScrollPolicy = value;
				Widget.HasVerticalScroller = verticalScrollPolicy != ScrollPolicy.Never;
			}
		}

		public ScrollPolicy HorizontalScrollPolicy {
			get {
				return horizontalScrollPolicy;
			}
			set {
				horizontalScrollPolicy = value;
				Widget.HasHorizontalScroller = horizontalScrollPolicy != ScrollPolicy.Never;
			}
		}

		IScrollControlBackend vertScroll;
		public IScrollControlBackend CreateVerticalScrollControl ()
		{
			if (vertScroll == null)
				vertScroll = new ScrollControlBackend (ApplicationContext, Widget, true);
			return vertScroll;
		}

		IScrollControlBackend horScroll;
		public IScrollControlBackend CreateHorizontalScrollControl ()
		{
			if (horScroll == null)
				horScroll = new ScrollControlBackend (ApplicationContext, Widget, false);
			return horScroll;
		}

		void OnScrolled (object o, EventArgs e)
		{
			if (vertScroll is ScrollControlBackend)
				((ScrollControlBackend)vertScroll).NotifyValueChanged ();
			if (horScroll is ScrollControlBackend)
				((ScrollControlBackend)horScroll).NotifyValueChanged ();
		}

		public Rectangle VisibleRect {
			get {
				CGRect rectF = Widget.ContentView.VisibleRect();
				return new Rectangle(rectF.X, rectF.Y, rectF.Width, rectF.Height);
			}
		}
		
		public bool BorderVisible {
			get {
				return false;
			}
			set {
			}
		}

		void UpdateChildSize ()
		{
			if (child == null)
				return;

			if (Widget.ContentView is CustomClipView) {
			} else {
				NSView view = (NSView)Widget.DocumentView;
				ViewBackend c = (ViewBackend)child;
				Size s;
				if (horizontalScrollPolicy == ScrollPolicy.Never) {
					s = c.Frontend.Surface.GetPreferredSize (SizeConstraint.WithSize (Widget.ContentView.Frame.Width), SizeConstraint.Unconstrained);
				}
				else if (verticalScrollPolicy == ScrollPolicy.Never) {
					s = c.Frontend.Surface.GetPreferredSize (SizeConstraint.Unconstrained, SizeConstraint.WithSize (Widget.ContentView.Frame.Width));
				}
				else {
					s = c.Frontend.Surface.GetPreferredSize ();
				}
				var w = Math.Max (s.Width, Widget.ContentView.Frame.Width);
				var h = Math.Max (s.Height, Widget.ContentView.Frame.Height);
				view.Frame = new CGRect (view.Frame.X, view.Frame.Y, (nfloat)w, (nfloat)h);
				if(bounceViewTop != null) {
					bounceViewTop.Frame = new CGRect(-maxBounce, -maxBounce, (nfloat)w + maxBounce * 2, (nfloat)maxBounce);
					bounceViewBottom.Frame = new CGRect(-maxBounce, h, (nfloat)w + maxBounce * 2, (nfloat)maxBounce);
					bounceViewLeft.Frame = new CGRect(-maxBounce, 0, (nfloat)maxBounce, h);
					bounceViewRight.Frame = new CGRect(w, 0, (nfloat)maxBounce, h);
				}
			}
		}
		
		public void SetChildSize (Size s)
		{
			UpdateChildSize ();
		}

		public override Drawing.Color BackgroundColor {
			get {
				return Widget.BackgroundColor.ToXwtColor ();
			}
			set {
				base.BackgroundColor = value;
				Widget.BackgroundColor = value.ToNSColor ();
			}
		}

	}
	
	class CustomScrollView: NSScrollView, IViewObject
	{
		public NSView View {
			get {
				return this;
			}
		}

		public ViewBackend Backend { get; set; }
		
		public override bool IsFlipped {
			get {
				return true;
			}
		}

		public IScrollViewEventSink EventSink { get; set;}

//		public override void ScrollWheel(NSEvent theEvent) {
//			base.ScrollWheel(theEvent);
//			EventSink.OnMouseScrolled(new MouseScrolledEventArgs((long)theEvent.Timestamp, (double)theEvent.ScrollingDeltaX, (double)theEvent.ScrollingDeltaY, ScrollDirection.Down));
//		}

		public override void ReflectScrolledClipView(NSClipView cView) {
			base.ReflectScrolledClipView(cView);
			EventSink.OnVisibleRectChanged();
		}
	}

	class DummyClipView: NSView
	{
		public override bool IsFlipped {
			get {
				return true;
			}
		}
	}
	
	class CustomClipView: NSClipView
	{
		ScrollAdjustmentBackend hScroll;
		ScrollAdjustmentBackend vScroll;
		double currentX;
		double currentY;
		float ratioX = 1, ratioY = 1;

		public CustomClipView (ScrollAdjustmentBackend hScroll, ScrollAdjustmentBackend vScroll)
		{
			this.hScroll = hScroll;
			this.vScroll = vScroll;
			CopiesOnScroll = false;
		}

		public double CurrentX {
			get {
				return hScroll.LowerValue + (currentX / ratioX);
			}
			set {
				ScrollToPoint (new CGPoint ((nfloat)(value - hScroll.LowerValue) * ratioX, (nfloat)currentY));
			}
		}

		public double CurrentY {
			get {
				return vScroll.LowerValue + (currentY / ratioY);
			}
			set {
				ScrollToPoint (new CGPoint ((nfloat)currentX, (nfloat)(value - vScroll.LowerValue) * ratioY));
			}
		}

		public override bool IsFlipped {
			get {
				return true;
			}
		}

		public override void SetFrameSize (CGSize newSize)
		{
			base.SetFrameSize (newSize);
			var v = DocumentView.Subviews [0];
			v.Frame = new CGRect (v.Frame.X, v.Frame.Y, newSize.Width, newSize.Height);
		}
		
		public override void ScrollToPoint (CGPoint newOrigin)
		{
			base.ScrollToPoint (newOrigin);
			var v = DocumentView.Subviews [0];

			currentX = newOrigin.X >= 0 ? newOrigin.X : 0;
			currentY = newOrigin.Y >= 0 ? newOrigin.Y : 0;
			if (currentX + v.Frame.Width > DocumentView.Frame.Width)
				currentX = DocumentView.Frame.Width - v.Frame.Width;
			if (currentY + v.Frame.Height > DocumentView.Frame.Height)
				currentY = DocumentView.Frame.Height - v.Frame.Height;

			v.Frame = new CGRect ((nfloat)currentX, (nfloat)currentY, v.Frame.Width, v.Frame.Height);

			hScroll.NotifyValueChanged ();
			vScroll.NotifyValueChanged ();
		}

		public void UpdateDocumentSize ()
		{
			var vr = DocumentVisibleRect ();
			ratioX = hScroll.PageSize != 0 ? (float)vr.Width / (float)hScroll.PageSize : 1;
			ratioY = vScroll.PageSize != 0 ? (float)vr.Height / (float)vScroll.PageSize : 1;
			DocumentView.Frame = new CGRect (0, 0, (nfloat)(hScroll.UpperValue - hScroll.LowerValue) * ratioX, (nfloat)(vScroll.UpperValue - vScroll.LowerValue) * ratioY);
		}
	}

	class NormalClipView: NSClipView
	{
		public event EventHandler Scrolled;

		public override void ScrollToPoint (CGPoint newOrigin)
		{
			base.ScrollToPoint (newOrigin);
			if (Scrolled != null)
				Scrolled (this, EventArgs.Empty);
		}
	}
}

