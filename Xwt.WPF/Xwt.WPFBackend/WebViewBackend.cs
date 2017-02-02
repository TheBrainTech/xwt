//
// WebViewBackend.cs
//
// Author:
//       Cody Russell <cody@xamarin.com>
//       Vsevolod Kukol <sevo@sevo.org>
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
<<<<<<< HEAD
using System.Windows;
using Xwt.Backends;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Reflection;
using System.Runtime.InteropServices;

=======
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using Xwt.Backends;
using Xwt.NativeMSHTML;
using Xwt.WPFBackend.Interop;
using SWC = System.Windows.Controls;

>>>>>>> f981e414c3bfee29f5dc508cd099be9b67e0bc9e
namespace Xwt.WPFBackend
{
	public class WebViewBackend : WidgetBackend, IWebViewBackend, IDocHostUIHandler
	{
<<<<<<< HEAD
		const int DISP_E_UNKNOWNNAME_ERROR = -2147352570;
=======
		string url;
		SWC.WebBrowser view;
		bool enableNavigatingEvent, enableLoadingEvent, enableLoadedEvent, enableTitleChangedEvent;
		bool initialized;

		ICustomDoc currentDocument;
		static object mshtmlBrowser;

		static PropertyInfo titleProperty;
		static PropertyInfo silentProperty;
		static MethodInfo stopMethod;
		static FieldInfo mshtmlBrowserField;
		static Type mshtmlDocType;
>>>>>>> f981e414c3bfee29f5dc508cd099be9b67e0bc9e

		private WebBrowser webBrowser;
		private string url;
		private bool scriptErrorsSuppressed = false;
		bool enableNavigatingEvent, enableLoadingEvent, enableLoadedEvent, enableTitleChangedEvent;

		public WebViewBackend ()
		{
<<<<<<< HEAD
			webBrowser = new WebBrowser ();
			Widget = webBrowser;

            webBrowser.Navigated += WebBrowser_Navigated;

			webBrowser.Navigating += HandleNavigating;
			webBrowser.Navigated += HandleNavigated;
			webBrowser.LoadCompleted += HandleLoadCompleted;
		}

        private void WebBrowser_Navigated(object sender, NavigationEventArgs e) {
            if (!scriptErrorsSuppressed) {
                SupressScriptErrors();
                scriptErrorsSuppressed = true;
            }
        }

        private void SupressScriptErrors() {
            FieldInfo field = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null) {
                object axIWebBrowser2 = field.GetValue(webBrowser);
                if (axIWebBrowser2 != null) {
                    // The property IWebBrowser2:Silent specifies whether the browser control shows script errors in dialogs or not. Set it to true.
                    axIWebBrowser2.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, axIWebBrowser2, new object[] { true });
                }
            }
        }
=======
			view = browser;
			view.Navigating += HandleNavigating;
			view.Navigated += HandleNavigated;
			view.LoadCompleted += HandleLoadCompleted;
			view.Loaded += HandleViewLoaded;
			Widget = view;
			view.Navigate ("about:blank"); // force Document initialization
			Title = string.Empty;
		}

		void UpdateDocumentRef()
		{
			if (currentDocument != view.Document)
			{
				var doc = view.Document as ICustomDoc;
				if (doc != null)
				{
					doc.SetUIHandler(this);
					if (mshtmlDocType == null)
						mshtmlDocType = view.Document.GetType();
				}
				if (currentDocument != null)
					currentDocument.SetUIHandler(null);
				currentDocument = doc;
			}

			// on initialization we load "about:blank" to initialize the document,
			// in that case we load the requested url
			if (currentDocument != null && !initialized)
			{
				initialized = true;
				if (!string.IsNullOrEmpty (url))
					view.Navigate(url);
			}
		}

		void HandleViewLoaded(object sender, System.Windows.RoutedEventArgs e)
		{
			// get the MSHTML.IWebBrowser2 instance field
			if (mshtmlBrowserField == null)
				mshtmlBrowserField = typeof(SWC.WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);

			if (mshtmlBrowser == null)
				mshtmlBrowser = mshtmlBrowserField.GetValue(view);

			if (silentProperty == null)
				silentProperty = mshtmlBrowserField?.FieldType?.GetProperty("Silent");

			if (stopMethod == null)
				stopMethod = mshtmlBrowserField?.FieldType?.GetMethod("Stop");

			// load requested url if the view is still not initialized
			// otherwise it would already have been loaded
			if (!initialized && !string.IsNullOrEmpty(url))
			{
				initialized = true;
				view.Navigate(url);
			}

			DisableJsErrors();
			UpdateDocumentRef();
		}
>>>>>>> f981e414c3bfee29f5dc508cd099be9b67e0bc9e

		public string Url {
			get {
				return url; }
			set {
<<<<<<< HEAD
				url = value;
				webBrowser.Navigate (url);
=======
				url = value;
				if (initialized && view.IsLoaded)
					view.Navigate(url);
>>>>>>> f981e414c3bfee29f5dc508cd099be9b67e0bc9e
			}
		}

		public string Title
		{
			get; private set;
		}

		public double LoadProgress { get; protected set; }

		public bool CanGoBack {
			get {
				return webBrowser.CanGoBack;
			}
		}

		public bool CanGoForward {
			get {
				return webBrowser.CanGoForward;
			}
		}

		public bool ContextMenuEnabled { get; set; }

		public bool ScrollBarsEnabled { get; set; }

		public bool DrawsBackground { get; set; }

		public string CustomCss { get; set; }

		public void GoBack ()
		{
			webBrowser.GoBack ();
		}

		public void GoForward ()
		{
			webBrowser.GoForward ();
		}

		public void Reload ()
		{
			webBrowser.Refresh ();
		}

		public void StopLoading ()
<<<<<<< HEAD
		{
			webBrowser.InvokeScript ("eval", "document.execCommand('Stop');");
=======
		{
			if (stopMethod != null)
				stopMethod.Invoke(mshtmlBrowser, null);
			else
				view.InvokeScript ("eval", "document.execCommand('Stop');");
>>>>>>> f981e414c3bfee29f5dc508cd099be9b67e0bc9e
		}

		public void LoadHtml (string content, string base_uri)
		{
<<<<<<< HEAD
			webBrowser.NavigateToString (content);
		}

		public void Unload()
		{
			webBrowser.Dispose();
			this.Dispose();
=======
			view.NavigateToString (content);
			url = string.Empty;
		}

		string GetTitle()
		{
			if (titleProperty == null)
			{
				// Get the property with the document Title,
				// property name depends on .NET/mshtml Version
				titleProperty = mshtmlDocType?.GetProperty("Title") ?? mshtmlDocType?.GetProperty("IHTMLDocument2_title");
			}

			string title = null;
			if (titleProperty != null)
			{
				try
				{
					title = titleProperty.GetValue(view.Document, null) as string;
				}
				catch {
					// try to get the title using a script, if reflection fails
					try
					{
						title = (string)view.InvokeScript("eval", "document.title.toString()");
					}
					#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
					catch { }
					#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
				}
			}

			return title;
>>>>>>> f981e414c3bfee29f5dc508cd099be9b67e0bc9e
		}

		protected new IWebViewEventSink EventSink {
			get { return (IWebViewEventSink)base.EventSink; }
		}

		public override void EnableEvent (object eventId)
		{
			base.EnableEvent (eventId);
			if (eventId is WebViewEvent) {
				switch ((WebViewEvent)eventId) {
				case WebViewEvent.NavigateToUrl:
					enableNavigatingEvent = true;
					break;
				case WebViewEvent.Loading:
					enableLoadingEvent = true;
					break;
				case WebViewEvent.Loaded:
					enableLoadedEvent = true;
					break;
				case WebViewEvent.TitleChanged:
					enableTitleChangedEvent = true;
					break;
				}
			}
		}

		public override void DisableEvent (object eventId)
		{
			base.DisableEvent (eventId);
			if (eventId is WebViewEvent) {
				switch ((WebViewEvent)eventId) {
				case WebViewEvent.NavigateToUrl:
					enableNavigatingEvent = false;
					break;
				case WebViewEvent.Loading:
					enableLoadingEvent = false;
					break;
				case WebViewEvent.Loaded:
					enableLoadedEvent = false;
					break;
				case WebViewEvent.TitleChanged:
					enableTitleChangedEvent = false;
					break;
				}
			}
		}

		void HandleNavigating (object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
		{
			if (enableNavigatingEvent) {
				var newurl = string.Empty;
				if (e.Uri != null)
					newurl = e.Uri.AbsoluteUri;
				Context.InvokeUserCode (delegate {
					e.Cancel = EventSink.OnNavigateToUrl (newurl);
				});
			}
		}

		void HandleNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
		{
			LoadProgress = 0;
			if (e.Uri != null && view.IsLoaded)
				this.url = e.Uri.AbsoluteUri;
			if (enableLoadingEvent)
				Context.InvokeUserCode(delegate
				{
					EventSink.OnLoading();
				});
		}

		void HandleLoadCompleted (object sender, System.Windows.Navigation.NavigationEventArgs e)
		{
			UpdateDocumentRef();

			LoadProgress = 1;

			if (enableLoadedEvent)
				Context.InvokeUserCode (EventSink.OnLoaded);
<<<<<<< HEAD
			try
			{
				Title = (string)webBrowser.InvokeScript("eval", "document.title.toString()");
			}
			catch (COMException ex)
			{
				if (ex.ErrorCode == DISP_E_UNKNOWNNAME_ERROR)
				{
					//Some web sites appear to disable the Eval method
					Console.WriteLine("DISP_E_UNKNOWN error from WebViewBackend");
				}
				else
				{
					throw;
				}
			}
			if (enableTitleChangedEvent && (prevTitle != Title))
				Context.InvokeUserCode (EventSink.OnTitleChanged);
			prevTitle = Title;
=======
>>>>>>> f981e414c3bfee29f5dc508cd099be9b67e0bc9e
		}

		static void DisableJsErrors()
		{
			if (silentProperty != null)
				silentProperty.SetValue(mshtmlBrowser, true, null);
		}

		#region IDocHostUIHandler implementation

		int IDocHostUIHandler.ShowContextMenu(DOCHOSTUICONTEXTMENU dwID, ref POINT ppt, object pcmdtReserved, object pdispReserved)
		{
			return (int)(ContextMenuEnabled ? HResult.S_FALSE : HResult.S_OK);
		}

		void IDocHostUIHandler.GetHostInfo(ref DOCHOSTUIINFO pInfo)
		{
			pInfo.dwFlags = DOCHOSTUIFLAG.DOCHOSTUIFLAG_DPI_AWARE;
			if (!ScrollBarsEnabled)
				pInfo.dwFlags = pInfo.dwFlags | DOCHOSTUIFLAG.DOCHOSTUIFLAG_SCROLL_NO | DOCHOSTUIFLAG.DOCHOSTUIFLAG_NO3DOUTERBORDER;
			if (!string.IsNullOrEmpty(CustomCss))
				pInfo.pchHostCss = CustomCss;
		}

		void IDocHostUIHandler.ShowUI(uint dwID, ref object pActiveObject, ref object pCommandTarget, ref object pFrame, ref object pDoc)
		{
		}

		void IDocHostUIHandler.HideUI()
		{
		}

		void IDocHostUIHandler.UpdateUI()
		{
			var newTitle = GetTitle();
			if (newTitle != Title)
			{
				Title = newTitle;
				if (enableTitleChangedEvent)
					Context.InvokeUserCode(EventSink.OnTitleChanged);
			}
		}

		void IDocHostUIHandler.EnableModeless(bool fEnable)
		{
		}

		void IDocHostUIHandler.OnDocWindowActivate(bool fActivate)
		{
		}

		void IDocHostUIHandler.OnFrameWindowActivate(bool fActivate)
		{
		}

		void IDocHostUIHandler.ResizeBorder(ref RECT prcBorder, object pUIWindow, bool fFrameWindow)
		{
		}

		int IDocHostUIHandler.TranslateAccelerator(ref MSG lpMsg, ref Guid pguidCmdGroup, uint nCmdID)
		{
			return (int)HResult.S_FALSE;
		}

		void IDocHostUIHandler.GetOptionKeyPath(out string pchKey, uint dw)
		{
			pchKey = null;
		}

		int IDocHostUIHandler.GetDropTarget(object pDropTarget, out object ppDropTarget)
		{
			ppDropTarget = pDropTarget;
			return (int)HResult.S_FALSE;
		}

		void IDocHostUIHandler.GetExternal(out object ppDispatch)
		{
			ppDispatch = null;
		}

		int IDocHostUIHandler.TranslateUrl(uint dwTranslate, string pchURLIn, out string ppchURLOut)
		{
			ppchURLOut = pchURLIn;
			return (int)HResult.S_FALSE;
		}
		int IDocHostUIHandler.FilterDataObject(IDataObject pDO, out IDataObject ppDORet)
		{
			ppDORet = null;
			return (int)HResult.S_FALSE;
		}

		#endregion
	}
}

