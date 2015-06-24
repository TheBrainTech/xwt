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

namespace Xwt.WPFBackend {
	public class SelectColorDialogBackend
		: Backend, ISelectColorDialogBackend {
		private ColorDialogExtension dialog;
		private static readonly Size DEFAULT_SIZE = new Size (240, 340);

		public bool Run(IWindowFrameBackend parent, string title, bool supportsAlpha, Action<Color> colorChangedCallback) {
			//TODO: Support alpha + create custom WPF solution?
			dialog = new ColorDialogExtension((int)this.ScreenPosition.X, (int)this.ScreenPosition.Y, title);
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

		public Size Size {
			get 
			{
				return DEFAULT_SIZE;
			}
		}

		public Point ScreenPosition { get; set; }

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

		public class ColorDialogExtension : ColorDialog {
			#region private const
			//Windows Message Constants
			private const Int32 WM_INITDIALOG = 0x0110;

			//uFlag Constants
			private const uint SWP_NOSIZE = 0x0001;
			private const uint SWP_SHOWWINDOW = 0x0040;
			private const uint SWP_NOZORDER = 0x0004;
			private const uint UFLAGS = SWP_NOSIZE | SWP_SHOWWINDOW;
			#endregion

			#region private readonly
			//Windows Handle Constants
			private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
			private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
			private static readonly IntPtr HWND_TOP = new IntPtr(0);
			private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
			#endregion

			#region private vars
			//Module vars
			private int x;
			private int y;
			private string title = null;
			#endregion

			#region private static methods imports
			//WinAPI definitions

			/// <summary>
			/// Sets the window text.
			/// </summary>
			/// <param name="hWnd">The h WND.</param>
			/// <param name="text">The text.</param>
			/// <returns></returns>
			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			private static extern bool SetWindowText(IntPtr hWnd, string text);

			/// <summary>
			/// Sets the window pos.
			/// </summary>
			/// <param name="hWnd">The h WND.</param>
			/// <param name="hWndInsertAfter">The h WND insert after.</param>
			/// <param name="x">The x.</param>
			/// <param name="y">The y.</param>
			/// <param name="cx">The cx.</param>
			/// <param name="cy">The cy.</param>
			/// <param name="uFlags">The u flags.</param>
			/// <returns></returns>
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
			#endregion

			#region public constructor
			/// <summary>
			/// Initializes a new instance of the <see cref="ColorDialogExtension"/> class.
			/// </summary>
			/// <param name="x">The X position</param>
			/// <param name="y">The Y position</param>
			/// <param name="title">The title of the windows. If set to null(by default), the title will not be changed</param>
			public ColorDialogExtension(int x, int y, String title = null) {
				this.x = x;
				this.y = y;
				this.title = title;
			}
			#endregion

			#region protected override methods
			/// <summary>
			/// Defines the common dialog box hook procedure that is overridden to add specific functionality to a common dialog box.
			/// </summary>
			/// <param name="hWnd">The handle to the dialog box window.</param>
			/// <param name="msg">The message being received.</param>
			/// <param name="wparam">Additional information about the message.</param>
			/// <param name="lparam">Additional information about the message.</param>
			/// <returns>
			/// A zero value if the default dialog box procedure processes the message; a nonzero value if the default dialog box procedure ignores the message.
			/// </returns>
			protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam) {
				//We do the base initialization
				IntPtr hookProc = base.HookProc(hWnd, msg, wparam, lparam);
				//When we init the dialog
				if (msg == WM_INITDIALOG) {
					//We change the title
					if (!String.IsNullOrEmpty(title)) {
						SetWindowText(hWnd, title);
					}
					//We move the position
					SetWindowPos(hWnd, HWND_TOPMOST, x, y, 0, 0, UFLAGS);

				}
				return hookProc;
			}
			#endregion
		}

	}
}
