// 
// SaveFileDialog.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
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

namespace Xwt
{
	[BackendType (typeof(ISaveFileDialogBackend))]
	public class SaveFileDialog: XwtComponent
	{
		bool running;
		FileDialogFilterCollection filters;
		string title = "";
		FileDialogFilter activeFilter;
		string currentFolder;
		string fileName;

		public SaveFileDialog ()
		{
			filters = new FileDialogFilterCollection (AddRemoveItem);
		}
		
		public SaveFileDialog (string title) : this()
		{
			this.title = title;
		}

		ISaveFileDialogBackend Backend {
			get { return (ISaveFileDialogBackend) base.BackendHost.Backend; }
		}

		void AddRemoveItem (FileDialogFilter filter, bool added)
		{
			CheckNotRunning ();
		}

		public string Title {
			get {
				return title ?? "";
			}
			set {
				title = value ?? "";
				if (running)
					Backend.Title = title;
			}
		}

		public string FileName {
			get { return running ? Backend.FileName : fileName; }
			set {
				if (running)
					Backend.FileName = value;
				else
					fileName = value;
			}
		}

		public string CurrentFolder {
			get {
				return running ? Backend.CurrentFolder : currentFolder;
			}
			set {
				if (running)
					Backend.CurrentFolder = value;
				else
					currentFolder = value;
			}
		}

		void CheckNotRunning ()
		{
			if (running)
				throw new InvalidOperationException ("Options can't be modified when the dialog is running");
		}

		public FileDialogFilter ActiveFilter {
			get { return running ? Backend.ActiveFilter : activeFilter; }
			set {
				if (!filters.Contains (value))
					throw new ArgumentException ("The active filter must be one of the filters included in the Filters collection");
				if (running)
					Backend.ActiveFilter = value;
				else
					activeFilter = value;
			}
		}
		/// <summary>
		/// Shows the dialog.
		/// </summary>
		public bool Run ()
		{
			return Run (null);
		}

		/// <summary>
		/// Shows the dialog.
		/// </summary>
		public bool Run (WindowFrame parentWindow)
		{
			try {
				running = true;
				Backend.Initialize (filters, title);
				if (!string.IsNullOrEmpty (currentFolder))
					Backend.CurrentFolder = currentFolder;
				if (activeFilter != null)
					Backend.ActiveFilter = activeFilter;
				if (!string.IsNullOrEmpty (title))
					Backend.Title = title;
				if (!string.IsNullOrEmpty (fileName)) 
					Backend.FileName = fileName;
				return Backend.Run ((IWindowFrameBackend)BackendHost.ToolkitEngine.GetSafeBackend (parentWindow));
			} finally {
				currentFolder = Backend.CurrentFolder;
				activeFilter = Backend.ActiveFilter;
				fileName = Backend.FileName;
				currentFolder = Backend.CurrentFolder;
				running = false;
				Backend.Cleanup ();
			}
		}
	}
}

