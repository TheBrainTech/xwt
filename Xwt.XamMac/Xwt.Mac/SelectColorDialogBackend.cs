//
// SelectColorDialogBackend.cs
//
// Author:
//       David Karlaš <david.karlas@gmail.com>
//
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
using System.ComponentModel;
using System.IO;
using Xwt.Backends;
using Xwt.Drawing;
using System.Runtime.InteropServices;
using AppKit;
using Foundation;

namespace Xwt.Mac
{
	public class SelectColorDialogBackend : ISelectColorDialogBackend
	{
		private NSColorPanel colorPanel;

		public SelectColorDialogBackend()
		{
			colorPanel = NSColorPanel.SharedColorPanel;
		}

		private SelectColorDialog frontend;

		public bool Run(IWindowFrameBackend parent, string title, bool supportsAlpha, SelectColorDialog frontend)
		{
			colorPanel.ShowsAlpha = supportsAlpha;
			colorPanel.OrderFront(null);

			this.frontend = frontend;
			NSNotificationCenter.DefaultCenter.AddObserver(NSColorPanel.ColorChangedNotification, OnColorChanged);
			return true;
		}

		void OnColorChanged(NSNotification notification) {
			frontend.Color = this.Color;
		}

		public void Dispose() {
		}

		public Color Color
		{
			get
			{
				return colorPanel.Color.ToXwtColor();
			}
			set
			{
				colorPanel.Color = value.ToNSColor();
			}
		}
	}
}
