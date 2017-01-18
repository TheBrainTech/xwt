// 
// ClipboardBackend.cs
//  
// Author:
//       Eric Maupin <ermau@xamarin.com>
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
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Xwt.Backends;
using WindowsClipboard = System.Windows.Clipboard;

namespace Xwt.WPFBackend
{
	public class WpfClipboardBackend
		: ClipboardBackend {
		private DataObject currentDataObject = new DataObject();

		public override void Clear() {
			WindowsClipboard.Clear();
		}

<<<<<<< HEAD
		public override void SetData(TransferDataType type, Func<object> dataSource, bool cleanClipboardFirst = true) {
			if(type == null)
				throw new ArgumentNullException("type");
			if(dataSource == null)
				throw new ArgumentNullException("dataSource");

			if(cleanClipboardFirst) {
				WindowsClipboard.Clear();
				currentDataObject = new DataObject();
			}

			if(type == TransferDataType.Html) {
				currentDataObject.SetData(type.ToWpfDataFormat(), GenerateCFHtml(dataSource().ToString()));
=======
		public override void SetData (TransferDataType type, Func<object> dataSource)
		{
			if (type == null)
				throw new ArgumentNullException ("type");
			if (dataSource == null)
				throw new ArgumentNullException ("dataSource");
			if (type == TransferDataType.Html) {
				WindowsClipboard.SetData (type.ToWpfDataFormat (), GenerateCFHtml (dataSource ().ToString ()));
			} else if (type == TransferDataType.Image) {
				var img = dataSource() as Xwt.Drawing.Image;
				if (img != null)
				{
					var src = img.ToBitmap().GetBackend() as WpfImage;
					WindowsClipboard.SetData (type.ToWpfDataFormat (), src.MainFrame);
				}
>>>>>>> f981e414c3bfee29f5dc508cd099be9b67e0bc9e
			} else {
				if(type == TransferDataType.Uri) {
					currentDataObject.SetFileDropList((StringCollection)(dataSource()));
				} else {
					currentDataObject.SetData(type.ToWpfDataFormat(), dataSource());
				}
			}
			WindowsClipboard.SetDataObject(currentDataObject);

		}

		static readonly string emptyCFHtmlHeader = GenerateCFHtmlHeader (0, 0, 0, 0);

		/// <summary>
		/// Generates a CF_HTML cliboard format document
		/// </summary>
		string GenerateCFHtml (string htmlFragment)
		{
			int startHTML     = emptyCFHtmlHeader.Length;
			int startFragment = startHTML;
			int endFragment   = startFragment + System.Text.Encoding.UTF8.GetByteCount (htmlFragment);
			int endHTML       = endFragment;
			return GenerateCFHtmlHeader (startHTML, endHTML, startFragment, endFragment) + htmlFragment;
		}

		/// <summary>
		/// Generates a CF_HTML clipboard format header.
		/// </summary>
		static string GenerateCFHtmlHeader (int startHTML, int endHTML, int startFragment, int endFragment)
		{
			return
				"Version:0.9" + Environment.NewLine +
					string.Format ("StartHTML: {0:d8}", startHTML) + Environment.NewLine +
					string.Format ("EndHTML: {0:d8}", endHTML) + Environment.NewLine +
					string.Format ("StartFragment: {0:d8}", startFragment) + Environment.NewLine +
					string.Format ("EndFragment: {0:d8}", endFragment) + Environment.NewLine;
		}

		public override bool IsTypeAvailable (TransferDataType type)
		{
			if (type == null)
				throw new ArgumentNullException ("type");

			if (type == TransferDataType.Text) {
				if (WindowsClipboard.ContainsFileDropList()) {
					foreach (string s in WindowsClipboard.GetFileDropList()) {
						return true;
					}
				}
			}

			return WindowsClipboard.ContainsData (type.ToWpfDataFormat ());
		}


		public static BitmapSource TryFixAlphaChannel (BitmapSource bitmapImage) {
			double dpi = 96;
			int width = bitmapImage.PixelWidth;
			int height = bitmapImage.PixelHeight;

			int stride = width * (bitmapImage.Format.BitsPerPixel + 7) / 8;
			byte[] pixelData = new byte[stride * height];
			bitmapImage.CopyPixels (pixelData, stride, 0);

			if (bitmapImage.Format == System.Windows.Media.PixelFormats.Bgra32) {
				bool anyNonZeroAlpha = false;
				for (int y = 0; y < height; y++) {
					for (int o = 3; o < stride; o += 4) {
						// Bgra32, so set the Alpha
						if (pixelData[y*stride + o] > 0) {
							anyNonZeroAlpha = true;
							break;
						}
					}
					if (anyNonZeroAlpha) {
						break;
					}
				}
				if (!anyNonZeroAlpha) {
					for (int y = 0; y < height; y++) {
						for (int o = 3; o < stride; o += 4) {
							// Bgra32, so set the Alpha
							pixelData[y*stride + o] = 255;
						}
					}
				}
			}

			return BitmapSource.Create (width, height, dpi, dpi, bitmapImage.Format, bitmapImage.Palette, pixelData, stride);
		}

		public static BitmapSource GetBestPossibletAlphaBitmapFromDataObject(System.Windows.IDataObject ob) {
			var formats = ob.GetFormats();
			BitmapSource bmp = null;

			foreach (string f in formats) {
				if (f != "PNG" && f != "image/png") { // only PNG (GIMP) and image/png (haven't seen this in the wild but could be useful)
					continue;
				}
				var it = ob.GetData(f);
				var ms = it as MemoryStream;
				if (ms != null) {
					BitmapImage result = new BitmapImage();
					result.BeginInit();
					// According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
					// Force the bitmap to load right now so we can dispose the stream.
					result.CacheOption = BitmapCacheOption.OnLoad;
					result.StreamSource = ms;
					result.EndInit();
					result.Freeze();
					bmp = result;
					break;
				}
			}

			if (bmp == null) {
				foreach (string f in formats) {
					if (!f.ToLower().Contains("bitmap")) {
						continue;
					}
					var obj = ob.GetData(f) as BitmapSource;
					if (obj != null) {
						bmp = obj;
						break;
					}
				}
			}

			if (bmp == null) {
				bmp = WindowsClipboard.GetImage();
			}
			bmp = TryFixAlphaChannel(bmp);
			return bmp;
		}

		public override object GetData (TransferDataType type)
		{
			if (type == null)
				throw new ArgumentNullException ("type");

			if (!IsTypeAvailable (type))
				return null;

<<<<<<< HEAD
			if (type == TransferDataType.Text) {
				if (WindowsClipboard.ContainsFileDropList()) {
					foreach (string s in WindowsClipboard.GetFileDropList()) {
						return "file://" + s;
					}
				}
			}

			if(type == TransferDataType.Image) {
				var ob = WindowsClipboard.GetDataObject();
				var bmp = GetBestPossibletAlphaBitmapFromDataObject(ob);
				return ApplicationContext.Toolkit.WrapImage(bmp);
			}

			return WindowsClipboard.GetData (type.ToWpfDataFormat ());
=======
			var data = WindowsClipboard.GetData (type.ToWpfDataFormat ());

			if (type == TransferDataType.Image)
				return ApplicationContext.Toolkit.WrapImage(ImageHandler.LoadFromImageSource((System.Windows.Media.ImageSource)data));
			return data;
>>>>>>> f981e414c3bfee29f5dc508cd099be9b67e0bc9e
		}

		public override IAsyncResult BeginGetData (TransferDataType type, AsyncCallback callback, object state)
		{
			if (type == null)
				throw new ArgumentNullException ("type");
			if (callback == null)
				throw new ArgumentNullException ("callback");

			return Task<object>.Factory.StartNew (s => GetData (type), state)
				.ContinueWith (t => callback (t));
		}

		public override object EndGetData (IAsyncResult ares)
		{
			if (ares == null)
				throw new ArgumentNullException ("ares");

			Task<object> t = ares as Task<object>;
			if (t == null)
				throw new ArgumentException ("ares is the incorrect type", "ares");

			return t.Result;
		}
	}
}
