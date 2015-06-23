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
using System.Windows.Forms;
using System.Windows.Interop;
using Xwt.Backends;
using IWin32Window = System.Windows.Forms.IWin32Window;
using Xwt.Drawing;
using System.Runtime.InteropServices;

namespace Xwt.WPFBackend
{
	public class SelectColorDialogBackend
		: Backend, ISelectColorDialogBackend
	{
		private ColorDialogWithTitle dialog;
		
		public bool Run(IWindowFrameBackend parent, string title, bool supportsAlpha, Action<Color> colorChangedCallback)
		{
			//TODO: Support alpha + create custom WPF solution?
			dialog = new ColorDialogWithTitle();
			dialog.Title = title;
			dialog.Color = System.Drawing.Color.FromArgb((byte)(this.Color.Alpha * 255), (byte)(this.Color.Red * 255), (byte)(this.Color.Green * 255), (byte)(this.Color.Blue * 255));
			bool output;
			if (parent != null)
				output = (this.dialog.ShowDialog(new WpfWin32Window(((WindowFrameBackend)parent).Window)) == DialogResult.OK);
			else
				output = (this.dialog.ShowDialog() == DialogResult.OK);

			this.Color = Color.FromBytes(this.dialog.Color.R, this.dialog.Color.G, this.dialog.Color.B, this.dialog.Color.A);
			colorChangedCallback.Invoke(this.Color);
			this.Close();
			return output;
		}

		public void Close()
		{
			this.dialog.Dispose();
		}

		public Color Color { get; set; }

		private class ColorDialogWithTitle : ColorDialog
		{
			public string Title { get; set; }
			[DllImport("user32.dll")]
			private static extern bool SetWindowText(IntPtr hWnd, string title);
			protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
			{
				IntPtr hookProc = base.HookProc(hWnd, msg, wparam, lparam);
				if (msg == 0x0110 && Title != null)//WM_INITDIALOG
				{
					SetWindowText(hWnd, Title);
				}
				return hookProc;
			}
		}

		private class WpfWin32Window
			: IWin32Window
		{
			public WpfWin32Window(System.Windows.Window window)
			{
				this.helper = new WindowInteropHelper(window);
			}

			public IntPtr Handle
			{
				get { return this.helper.Handle; }
			}

			private readonly WindowInteropHelper helper;
		}
	}
}
