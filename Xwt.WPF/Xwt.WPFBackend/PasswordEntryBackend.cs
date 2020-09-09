using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Xwt.WPFBackend;
using Xwt.Backends;
using System.Windows.Documents;
using System.Windows.Media;

namespace Xwt.WPFBackend
{
	class PasswordEntryBackend : WidgetBackend, IPasswordEntryBackend
	{
		PlaceholderTextAdorner Adorner {
			get; set;
		}

		public PasswordEntryBackend ()
		{
			// If this is not in a ScrollContentPresenter, AdornerLayer.GetAdornerLayer will return null
			var adornerDecorator = new AdornerDecorator();
			Widget = adornerDecorator;
			PasswordBox = new PasswordBox();
			adornerDecorator.Child = PasswordBox;
			Adorner = new PlaceholderTextAdorner(PasswordBox);
			AdornerLayer.GetAdornerLayer(PasswordBox).Add(Adorner);
			PasswordBox.VerticalContentAlignment = VerticalAlignment.Center;
		}

		protected PasswordBox PasswordBox { get; private set; }

		public string Password
		{
			get { return PasswordBox.Password; }
			set { PasswordBox.Password = value; }
		}

		public System.Security.SecureString SecurePassword
		{
			get { return PasswordBox.SecurePassword; }
		}

		public string PlaceholderText {
			get { return Adorner.PlaceholderText; }
			set { Adorner.PlaceholderText = value; }
		}

		public override void EnableEvent (object eventId)
		{
			base.EnableEvent (eventId);

			if (eventId is PasswordEntryEvent) 
			{
				switch ((PasswordEntryEvent) eventId) 
				{
					case PasswordEntryEvent.Changed:
						PasswordBox.PasswordChanged += OnPasswordChanged;
						break;
					case PasswordEntryEvent.Activated:
						PasswordBox.KeyDown += OnActivated;
						break;
				}
			}
		}

		public override void DisableEvent (object eventId)
		{
			base.DisableEvent (eventId);

			if (eventId is PasswordEntryEvent)
			{
				switch ((PasswordEntryEvent)eventId)
				{
					case PasswordEntryEvent.Changed:
						PasswordBox.PasswordChanged -= OnPasswordChanged;
						break;
					case PasswordEntryEvent.Activated:
						PasswordBox.KeyDown -= OnActivated;
						break;
				}
			}
		}

		protected new IPasswordEntryEventSink EventSink {
			get { return (IPasswordEntryEventSink) base.EventSink; }
		}

		private void OnActivated(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return)
				Context.InvokeUserCode (EventSink.OnActivated);
		}

		void OnPasswordChanged (object s, RoutedEventArgs e)
		{
			Context.InvokeUserCode (EventSink.OnChanged);
		}

		public override Drawing.Color TextColor {
			get {
				return this.PasswordBox.Foreground.ToXwtColor();
			}
			set {
				this.PasswordBox.Foreground = new SolidColorBrush(value.ToWpfColor());
			}
		}

		public override Drawing.Color BackgroundColor {
			get {
				return this.PasswordBox.Background.ToXwtColor();
			}
			set {
				this.PasswordBox.Background = new SolidColorBrush(value.ToWpfColor());
			}
		}
	}
}
