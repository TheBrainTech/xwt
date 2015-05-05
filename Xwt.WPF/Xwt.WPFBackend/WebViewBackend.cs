//
// WebViewBackend.cs
//
// Author:
//       Cody Russell <cody@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc.
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
using System.Windows;
using Xwt.Backends;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Reflection;

namespace Xwt.WPFBackend
{
	public class WebViewBackend : WidgetBackend, IWebViewBackend
	{
		private WebBrowser webBrowser;
		private string url;
		private bool scriptErrorsSuppressed = false;

		public WebViewBackend ()
		{
			webBrowser = new WebBrowser ();
			Widget = webBrowser;

			webBrowser.LoadCompleted += WebBrowser_LoadCompleted;
		}

		private void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e) {
			if(!scriptErrorsSuppressed) {
				SupressScriptErrors();
				scriptErrorsSuppressed = true;
			}
		}

		private void SupressScriptErrors() {
			FieldInfo field = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
			if(field != null) {
				object axIWebBrowser2 = field.GetValue(webBrowser);
				if(axIWebBrowser2 != null) {
					// The property IWebBrowser2:Silent specifies whether the browser control shows script errors in dialogs or not. Set it to true.
					axIWebBrowser2.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, axIWebBrowser2, new object[] { true });
				}
			}
		}

		internal WebViewBackend (WebBrowser browser)
		{
			Widget = browser;
		}

		public string Url {
			get { return url; }
			set {
				url = value;
				webBrowser.Navigate (url);
			}
		}
	}
}

