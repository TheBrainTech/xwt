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
using SWC = System.Windows.Controls;
using Xwt.Backends;
using mshtml;

namespace Xwt.WPFBackend
{
	public class WebViewBackend : WidgetBackend, IWebViewBackend
	{
		string url;

		public WebViewBackend ()
		{
			Widget = new SWC.WebBrowser ();
			((SWC.WebBrowser)Widget).LoadCompleted += WebViewBackend_LoadCompleted;
		}

		void WebViewBackend_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e) {
			string DisableScriptError = "window.onerror = function() {return true;} ";
			HTMLDocument doc2 = (HTMLDocument)((SWC.WebBrowser)Widget).Document;
			IHTMLScriptElement scriptErrorSuppressed = (IHTMLScriptElement)doc2.createElement("SCRIPT");
			scriptErrorSuppressed.type = "text/javascript";
			scriptErrorSuppressed.text = DisableScriptError;
			IHTMLElementCollection nodes = doc2.getElementsByTagName("head");
			foreach (IHTMLElement elem in nodes) {
				HTMLHeadElement head = (HTMLHeadElement)elem;
				head.appendChild((IHTMLDOMNode)scriptErrorSuppressed);
			}
		}

		internal WebViewBackend (SWC.WebBrowser browser)
		{
			Widget = browser;
		}

		public string Url {
			get { return url; }
			set {
				url = value;
				((SWC.WebBrowser)Widget).Navigate (url);
			}
		}
	}

}

