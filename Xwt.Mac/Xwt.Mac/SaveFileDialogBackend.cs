using System;
using Xwt.Backends;
using MonoMac.AppKit;

namespace Xwt.Mac {
	public class SaveFileDialogBackend : NSSavePanel, ISaveFileDialogBackend
	{
		public SaveFileDialogBackend() {
		}

		#region ISaveFileDialogBackend implementation
		public void Initialize (System.Collections.Generic.IEnumerable<FileDialogFilter> filters, string title) {
			this.CanCreateDirectories = true;
			this.Title = title;
			this.Prompt = "Save";
		}

		public bool Run (IWindowFrameBackend parent)
		{
			var returnValue = this.RunModal ();
			return returnValue == 1;
		}

		public void Cleanup ()
		{
		}

		public string FileName {
			get {
				return this.Url == null ? string.Empty : Url.Path;
			}
			set {
				this.NameFieldStringValue = value;
			}
		}

		public string CurrentFolder {
			get {
				return DirectoryUrl.AbsoluteString;
			}
			set {
				this.DirectoryUrl = new MonoMac.Foundation.NSUrl (value,true);
			}
		}

		public FileDialogFilter ActiveFilter {
			get {
				return null;
			}
			set {
			}
		}

		#endregion

		#region IBackend implementation

		public void InitializeBackend (object frontend, ApplicationContext context)
		{

		}

		public void EnableEvent (object eventId)
		{

		}

		public void DisableEvent (object eventId)
		{

		}

		#endregion
	}
}

