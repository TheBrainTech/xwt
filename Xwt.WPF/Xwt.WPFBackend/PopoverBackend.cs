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
using System.Windows.Input;

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
		static readonly SolidColorBrush STROKE_BRUSH = Brushes.Gray;
		static Color BACKGROUND_COLOR;
		static SolidColorBrush BACKGROUND_BRUSH;

		static PopoverBackend() {
			if(SystemColors.MenuBarColor.R + SystemColors.MenuBarColor.G + SystemColors.MenuBarColor.B < 380) {
				BACKGROUND_COLOR = Color.FromRgb(0, 0, 0);
			} else {
				BACKGROUND_COLOR = Color.FromRgb(235, 235, 235);
			}
			BACKGROUND_BRUSH = new SolidColorBrush(BACKGROUND_COLOR);
		}

		public bool StaysOpen {
			get {
				return NativeWidget.StaysOpen;
			}
			set {
				NativeWidget.StaysOpen = value;
			}
		}

		public bool IsVisible {
			get { return this.NativeWidget.Child.IsVisible; }
		}

		public Xwt.Popover.Position ActualPosition {
			get; set;
		}

		Border border;
		Grid grid;

		public Xwt.Drawing.Color BackgroundColor {
			get {
				return border.Background.ToXwtColor ();
			}
			set {
				border.Background = new SolidColorBrush (value.ToWpfColor ());
			}
		}

		IPopoverEventSink EventSink {
			get; set;
		}

		/// <summary>
		/// Control, if any, that should get the initial keyboard focus when the popover is shown.
		/// The control should be inside the popover, but it doesn't necessarily have to be an Xwt
		/// managed widget.
		/// </summary>
		public UIElement InitialFocus { get; set; }

		/// <summary>
		/// If set to true, then the arrow keys can't be used to move focus between controls.
		/// Regardless of this setting, tab still works to change focus and the arrow keys still
		/// work inside of controls that use them.
		/// </summary>
		public bool DisableArrowKeyNavigation { get; set; }

		new Popover Frontend {
			get { return (Popover)base.frontend; }
		}

		public System.Windows.Controls.Primitives.Popup NativeWidget {
			get; set;
		}

		/// <summary>
		/// Search up the visual tree, finding the PopupRoot for the popup.
		/// </summary>
		/// <returns>PopupRoot or null if not found for some reason</returns>
		public FrameworkElement GetPopupRoot ()
		{
			FrameworkElement element = Border;

			do {
				element = (FrameworkElement) VisualTreeHelper.GetParent (element);
				if (element == null)
					return null;

				if (element.GetType ().Name == "PopupRoot")
					return element;
			} while (true);
		}

		void SetupBorderMargin ()
		{
			if (Frontend.Padding != 0)
				return;

			Border.Padding = new Thickness (1, 1, 1, 1);
			Border.BorderThickness = new Thickness (1);
			Border.Margin = new Thickness (1);
		}

		public PopoverBackend ()
		{
<<<<<<< HEAD

			grid = new Grid() { 
				Margin = new Thickness(0, VERTICAL_MARGIN, 0, 0) };

			string xamlCaretPath = string.Format("M 0,{0} C 0,{0} {1},{0} {2},0 C {2},0 {3},{0} {4},{0}", CARET_HEIGHT, CARET_WIDTH / 4, CARET_WIDTH / 2, CARET_WIDTH * 3 / 4, CARET_WIDTH);
			Geometry caretGeometry = PathGeometry.Parse(xamlCaretPath);
			Path caretPath = new Path() {
				Stroke = STROKE_BRUSH,
				Fill = BACKGROUND_BRUSH,
				Data = caretGeometry,
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Center
			};

			string xamlBoundaryPath = string.Format("M 0,{0} L {1},{0}", CARET_HEIGHT + 1, CARET_WIDTH);
			Geometry boundaryGeometry = PathGeometry.Parse(xamlBoundaryPath);
			Path boundaryPath = new Path() {
				Stroke = BACKGROUND_BRUSH,
				Data = boundaryGeometry,
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			
			border = new System.Windows.Controls.Border {
				BorderBrush = STROKE_BRUSH,
				CornerRadius = new CornerRadius(BORDER_RADIUS),
				Padding = new Thickness(BORDER_PADDING),
				BorderThickness = new Thickness(BORDER_THICKNESS),
				Margin = new Thickness(0, CARET_HEIGHT - 1, 0, 0),
				VerticalAlignment = VerticalAlignment.Top,
				Background = BACKGROUND_BRUSH
=======
			Border = new System.Windows.Controls.Border {
					Padding = new Thickness (15, 10, 15, 15),
					BorderThickness = new Thickness (1),
					Margin = new Thickness (10),
					Effect = new System.Windows.Media.Effects.DropShadowEffect () {
					Color = Colors.Black,
					Direction = 270,
					BlurRadius = 15,
					Opacity = .15,
					ShadowDepth = 1,
				}
>>>>>>> f7ddf3e63b23e3e27feeb7d127f8357cc3284098
			};

			//Set background color of popover
			BackgroundColor = BACKGROUND_BRUSH.ToXwtColor();

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
			NativeWidget.Opened += NativeWidget_Opened;
			NativeWidget.Closed += NativeWidget_Closed;
			NativeWidget.PreviewKeyDown += NativeWidget_PreviewKeyDown;
		}

		public void Initialize (IPopoverEventSink sink)
		{
			EventSink = sink;
		}

		public void Show (Xwt.Popover.Position orientation, Xwt.Widget reference, Xwt.Rectangle positionRect, Widget child)
		{
			ActualPosition = orientation;
<<<<<<< HEAD
			border.Child = (System.Windows.FrameworkElement)Context.Toolkit.GetNativeWidget (child);
=======
			SetupBorderMargin ();
			Border.Child = (System.Windows.FrameworkElement)Context.Toolkit.GetNativeWidget (child);
>>>>>>> f7ddf3e63b23e3e27feeb7d127f8357cc3284098
			NativeWidget.CustomPopupPlacementCallback = (popupSize, targetSize, offset) => {
				System.Windows.Point location;
				if (ActualPosition == Popover.Position.Top)
					location = new System.Windows.Point (-child.ScreenBounds.Width / 2 - BORDER_PADDING + positionRect.Right, positionRect.Bottom);
				else
					location = new System.Windows.Point (positionRect.Left, positionRect.Top - popupSize.Height);

				return new[] {
					new System.Windows.Controls.Primitives.CustomPopupPlacement (location, System.Windows.Controls.Primitives.PopupPrimaryAxis.Horizontal)
				};
			};
			NativeWidget.PlacementTarget = (System.Windows.FrameworkElement)Context.Toolkit.GetNativeWidget (reference);
			NativeWidget.IsOpen = true;

			// Popups are special in that the automation properties need to be set on the PopupRoot, which only exists when the popup is shown
			// See https://social.msdn.microsoft.com/Forums/vstudio/en-US/d4ba12c8-7a87-478e-b064-5620f929a0cf/how-to-set-automationid-and-name-for-popup?forum=wpf
			var accessibleBackend = (AccessibleBackend)Toolkit.GetBackend (Frontend.Accessible);
			if (accessibleBackend != null) {
				FrameworkElement popupRoot = GetPopupRoot ();
				if (popupRoot != null)
					accessibleBackend.InitAutomationProperties (popupRoot);
			}
		}

		void NativeWidget_Opened (object sender, EventArgs e)
		{
			if (DisableArrowKeyNavigation) {
				FrameworkElement popupRoot = GetPopupRoot ();
				if (popupRoot != null)
					KeyboardNavigation.SetDirectionalNavigation (popupRoot, KeyboardNavigationMode.Once);
			}

			if (InitialFocus != null)
				InitialFocus.Focus ();
		}

		void NativeWidget_Closed (object sender, EventArgs e)
		{
			border.Child = null;
			EventSink.OnClosed ();
		}

		void NativeWidget_PreviewKeyDown (object sender, System.Windows.Input.KeyEventArgs e)
		{
			// Close the popup when Escape is hit
			if (e.Key == System.Windows.Input.Key.Escape) {
				NativeWidget.IsOpen = false;
				e.Handled = true;
			}
		}

		public void Hide ()
		{
			NativeWidget.IsOpen = false;
			border.Child = null;
		}

		public void Dispose ()
		{
			if (NativeWidget != null) {
				NativeWidget.Opened -= NativeWidget_Opened;
				NativeWidget.Closed -= NativeWidget_Closed;
			}
		}
	}
}