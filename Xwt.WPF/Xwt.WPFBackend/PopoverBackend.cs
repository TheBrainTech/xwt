//
// PopoverBackend.cs
//
// Author:
//       Alan McGovern <alan@xamarin.com>
//
// Copyright (c) 2012 Xamarin, Inc.
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
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace Xwt.WPFBackend
{
	public class PopoverBackend : Backend, IPopoverBackend
	{
		const int VERTICAL_MARGIN = 2;
		const int CARET_WIDTH = 40;
		const int CARET_HEIGHT = 10;
		const int BORDER_RADIUS = 7;
		const int BORDER_PADDING = 7;
		const int BORDER_THICKNESS = 1;
		static readonly SolidColorBrush STROKE_COLOR = Brushes.Gray;
		static readonly SolidColorBrush FILL_COLOR = Brushes.White;

		public bool IsVisible {
			get { return this.NativeWidget.Child.IsVisible; }
		}

		public Xwt.Popover.Position ActualPosition {
			get; set;
		}

		Grid grid;

		public Xwt.Drawing.Color BackgroundColor {
			get {
				return Border.Background.ToXwtColor ();
			}
			set {
				Border.Background = new SolidColorBrush (value.ToWpfColor ());
			}
		}

		IPopoverEventSink EventSink {
			get; set;
		}

		new Popover Frontend {
			get { return (Popover)base.frontend; }
		}

		System.Windows.Controls.Primitives.Popup NativeWidget {
			get; set;
		}

		public PopoverBackend ()
		{

			grid = new Grid() { 
				Margin = new Thickness(0, VERTICAL_MARGIN, 0, 0) };

			string xamlCaretPath = string.Format("M 0,{0} C 0,{0} {1},{0} {2},0 C {2},0 {3},{0} {4},{0}", CARET_HEIGHT, CARET_WIDTH / 4, CARET_WIDTH / 2, CARET_WIDTH * 3 / 4, CARET_WIDTH);
			Geometry caretGeometry = PathGeometry.Parse(xamlCaretPath);
			Path caretPath = new Path() {
				Stroke = STROKE_COLOR,
				Fill = FILL_COLOR,
				Data = caretGeometry,
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Center
			};

			string xamlBoundaryPath = string.Format("M 0,{0} L {1},{0}", CARET_HEIGHT + 1, CARET_WIDTH);
			Geometry boundaryGeometry = PathGeometry.Parse(xamlBoundaryPath);
			Path boundaryPath = new Path() {
				Stroke = FILL_COLOR,
				Data = boundaryGeometry,
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			
			border = new System.Windows.Controls.Border {
				BorderBrush = STROKE_COLOR,
				CornerRadius = new CornerRadius(BORDER_RADIUS),
				Padding = new Thickness(BORDER_PADDING),
				BorderThickness = new Thickness(BORDER_THICKNESS),
				Margin = new Thickness(0, CARET_HEIGHT - 1, 0, 0),
				VerticalAlignment = VerticalAlignment.Top,
				Background = FILL_COLOR
			};
			BackgroundColor = Xwt.Drawing.Color.FromBytes (230, 230, 230, 230);

			NativeWidget = new System.Windows.Controls.Primitives.Popup {
				AllowsTransparency = true,
				Child = grid,
				Placement = System.Windows.Controls.Primitives.PlacementMode.Custom,
				StaysOpen = false,
				Margin = new System.Windows.Thickness (10),
			};

			grid.Children.Add(border);
			grid.Children.Add(caretPath);
			grid.Children.Add(boundaryPath);

			NativeWidget.CustomPopupPlacementCallback = (popupSize, targetSize, offset) => {
				var location = new System.Windows.Point (targetSize.Width / 2 - popupSize.Width / 2, 0);
				if (ActualPosition == Popover.Position.Top)
					location.Y = targetSize.Height;
				else
					location.Y = -popupSize.Height;

				return new[] {
					new System.Windows.Controls.Primitives.CustomPopupPlacement (location, System.Windows.Controls.Primitives.PopupPrimaryAxis.Horizontal)
				};
			};
			NativeWidget.Closed += NativeWidget_Closed;
		}

		public void Initialize (IPopoverEventSink sink)
		{
			EventSink = sink;
		}

		public void Show (Xwt.Popover.Position orientation, Xwt.Widget reference, Xwt.Rectangle positionRect, Widget child)
		{
			ActualPosition = orientation;
			Border.Child = (System.Windows.FrameworkElement)Context.Toolkit.GetNativeWidget (child);
			NativeWidget.CustomPopupPlacementCallback = (popupSize, targetSize, offset) => {
				System.Windows.Point location;
				if (ActualPosition == Popover.Position.Top)
					location = new System.Windows.Point (positionRect.Left, positionRect.Bottom);
				else
					location = new System.Windows.Point (positionRect.Left, positionRect.Top - popupSize.Height);

				return new[] {
					new System.Windows.Controls.Primitives.CustomPopupPlacement (location, System.Windows.Controls.Primitives.PopupPrimaryAxis.Horizontal)
				};
			};
			NativeWidget.PlacementTarget = (System.Windows.FrameworkElement)Context.Toolkit.GetNativeWidget (reference);
			NativeWidget.IsOpen = true;
		}

		void NativeWidget_Closed (object sender, EventArgs e)
		{
			Border.Child = null;
			EventSink.OnClosed ();
		}

		public void Hide ()
		{
			NativeWidget.IsOpen = false;
			Border.Child = null;
		}

		public void Dispose ()
		{
			if (NativeWidget != null)
				NativeWidget.Closed -= NativeWidget_Closed;
		}
	}
}