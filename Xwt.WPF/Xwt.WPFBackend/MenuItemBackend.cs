// 
// MenuItemBackend.cs
//  
// Author:
//       Carlos Alberto Cortez <calberto.cortez@gmail.com>
//       Eric Maupin <ermau@xamarin.com>
// 
// Copyright (c) 2011 Carlos Alberto Cortez
// Copyright (c) 2012 Xamarin, Inc.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SWC = System.Windows.Controls;
using SWMI = System.Windows.Media.Imaging;
using Xwt.Backends;


namespace Xwt.WPFBackend
{
	public class MenuItemBackend : Backend, IMenuItemBackend
	{
		private static Dictionary<ConsoleKey, string> consoleKeyToStringMap = new Dictionary<ConsoleKey, string>();

		object item;
		SWC.MenuItem menuItem;
		MenuBackend subMenu;
		MenuItemType type;
		IMenuItemEventSink eventSink;
		string label;
		bool useMnemonic;

		private KeyAccelerator accelerator;
		public KeyAccelerator Accelerator {
			get {
				return accelerator;
			}
			set {
				accelerator = value;
				menuItem.InputGestureText = GetInputGestureText(accelerator);
			}
		}

		static MenuItemBackend() {
			InitializeConsoleKeyToStringMap();
		}

		private static void InitializeConsoleKeyToStringMap() {
			//consoleKeyToStringMap.Add(ConsoleKey.Backspace, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Tab, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Clear, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Enter, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Pause, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Escape, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Spacebar, "");
			//consoleKeyToStringMap.Add(ConsoleKey.PageUp, "");
			//consoleKeyToStringMap.Add(ConsoleKey.PageDown, "");
			//consoleKeyToStringMap.Add(ConsoleKey.End, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Home, "");
			consoleKeyToStringMap.Add(ConsoleKey.LeftArrow, "Left");
			consoleKeyToStringMap.Add(ConsoleKey.UpArrow, "Up");
			consoleKeyToStringMap.Add(ConsoleKey.RightArrow, "Right");
			consoleKeyToStringMap.Add(ConsoleKey.DownArrow, "Down");
			//consoleKeyToStringMap.Add(ConsoleKey.Select, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Print, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Execute, "");
			//consoleKeyToStringMap.Add(ConsoleKey.PrintScreen, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Insert, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Delete, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Help, "");
			//consoleKeyToStringMap.Add(ConsoleKey.D0, "");
			//consoleKeyToStringMap.Add(ConsoleKey.D1, "");
			//consoleKeyToStringMap.Add(ConsoleKey.D2, "");
			//consoleKeyToStringMap.Add(ConsoleKey.D3, "");
			//consoleKeyToStringMap.Add(ConsoleKey.D4, "");
			//consoleKeyToStringMap.Add(ConsoleKey.D5, "");
			//consoleKeyToStringMap.Add(ConsoleKey.D6, "");
			//consoleKeyToStringMap.Add(ConsoleKey.D7, "");
			//consoleKeyToStringMap.Add(ConsoleKey.D8, "");
			//consoleKeyToStringMap.Add(ConsoleKey.D9, "");
			consoleKeyToStringMap.Add(ConsoleKey.A, "A");
			consoleKeyToStringMap.Add(ConsoleKey.B, "B");
			consoleKeyToStringMap.Add(ConsoleKey.C, "C");
			consoleKeyToStringMap.Add(ConsoleKey.D, "D");
			consoleKeyToStringMap.Add(ConsoleKey.E, "E");
			consoleKeyToStringMap.Add(ConsoleKey.F, "F");
			consoleKeyToStringMap.Add(ConsoleKey.G, "G");
			consoleKeyToStringMap.Add(ConsoleKey.H, "H");
			consoleKeyToStringMap.Add(ConsoleKey.I, "I");
			consoleKeyToStringMap.Add(ConsoleKey.J, "J");
			consoleKeyToStringMap.Add(ConsoleKey.K, "K");
			consoleKeyToStringMap.Add(ConsoleKey.L, "L");
			consoleKeyToStringMap.Add(ConsoleKey.M, "M");
			consoleKeyToStringMap.Add(ConsoleKey.N, "N");
			consoleKeyToStringMap.Add(ConsoleKey.O, "O");
			consoleKeyToStringMap.Add(ConsoleKey.P, "P");
			consoleKeyToStringMap.Add(ConsoleKey.Q, "Q");
			consoleKeyToStringMap.Add(ConsoleKey.R, "R");
			consoleKeyToStringMap.Add(ConsoleKey.S, "S");
			consoleKeyToStringMap.Add(ConsoleKey.T, "T");
			consoleKeyToStringMap.Add(ConsoleKey.U, "U");
			consoleKeyToStringMap.Add(ConsoleKey.V, "V");
			consoleKeyToStringMap.Add(ConsoleKey.W, "W");
			consoleKeyToStringMap.Add(ConsoleKey.X, "X");
			consoleKeyToStringMap.Add(ConsoleKey.Y, "Y");
			consoleKeyToStringMap.Add(ConsoleKey.Z, "Z");
			//consoleKeyToStringMap.Add(ConsoleKey.LeftWindows, "");
			//consoleKeyToStringMap.Add(ConsoleKey.RightWindows, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Applications, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Sleep, "");
			//consoleKeyToStringMap.Add(ConsoleKey.NumPad0, "");
			//consoleKeyToStringMap.Add(ConsoleKey.NumPad1, "");
			//consoleKeyToStringMap.Add(ConsoleKey.NumPad2, "");
			//consoleKeyToStringMap.Add(ConsoleKey.NumPad3, "");
			//consoleKeyToStringMap.Add(ConsoleKey.NumPad4, "");
			//consoleKeyToStringMap.Add(ConsoleKey.NumPad5, "");
			//consoleKeyToStringMap.Add(ConsoleKey.NumPad6, "");
			//consoleKeyToStringMap.Add(ConsoleKey.NumPad7, "");
			//consoleKeyToStringMap.Add(ConsoleKey.NumPad8, "");
			//consoleKeyToStringMap.Add(ConsoleKey.NumPad9, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Multiply, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Add, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Separator, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Subtract, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Decimal, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Divide, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F1, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F2, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F3, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F4, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F5, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F6, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F7, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F8, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F9, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F10, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F11, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F12, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F13, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F14, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F15, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F16, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F17, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F18, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F19, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F20, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F21, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F22, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F23, "");
			//consoleKeyToStringMap.Add(ConsoleKey.F24, "");
			//consoleKeyToStringMap.Add(ConsoleKey.BrowserBack, "");
			//consoleKeyToStringMap.Add(ConsoleKey.BrowserForward, "");
			//consoleKeyToStringMap.Add(ConsoleKey.BrowserRefresh, "");
			//consoleKeyToStringMap.Add(ConsoleKey.BrowserStop, "");
			//consoleKeyToStringMap.Add(ConsoleKey.BrowserSearch, "");
			//consoleKeyToStringMap.Add(ConsoleKey.BrowserFavorites, "");
			//consoleKeyToStringMap.Add(ConsoleKey.BrowserHome, "");
			//consoleKeyToStringMap.Add(ConsoleKey.VolumeMute, "");
			//consoleKeyToStringMap.Add(ConsoleKey.VolumeDown, "");
			//consoleKeyToStringMap.Add(ConsoleKey.VolumeUp, "");
			//consoleKeyToStringMap.Add(ConsoleKey.MediaNext, "");
			//consoleKeyToStringMap.Add(ConsoleKey.MediaPrevious, "");
			//consoleKeyToStringMap.Add(ConsoleKey.MediaStop, "");
			//consoleKeyToStringMap.Add(ConsoleKey.MediaPlay, "");
			//consoleKeyToStringMap.Add(ConsoleKey.LaunchMail, "");
			//consoleKeyToStringMap.Add(ConsoleKey.LaunchMediaSelect, "");
			//consoleKeyToStringMap.Add(ConsoleKey.LaunchApp1, "");
			//consoleKeyToStringMap.Add(ConsoleKey.LaunchApp2, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Oem1, "");
			//consoleKeyToStringMap.Add(ConsoleKey.OemPlus, "");
			//consoleKeyToStringMap.Add(ConsoleKey.OemComma, "");
			//consoleKeyToStringMap.Add(ConsoleKey.OemMinus, "");
			//consoleKeyToStringMap.Add(ConsoleKey.OemPeriod, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Oem2, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Oem3, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Oem4, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Oem5, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Oem6, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Oem7, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Oem8, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Oem102, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Process, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Packet, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Attention, "");
			//consoleKeyToStringMap.Add(ConsoleKey.CrSel, "");
			//consoleKeyToStringMap.Add(ConsoleKey.ExSel, "");
			//consoleKeyToStringMap.Add(ConsoleKey.EraseEndOfFile, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Play, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Zoom, "");
			//consoleKeyToStringMap.Add(ConsoleKey.NoName, "");
			//consoleKeyToStringMap.Add(ConsoleKey.Pa1, "");
			//consoleKeyToStringMap.Add(ConsoleKey.OemClear, "");
		}

		private string GetInputGestureText(KeyAccelerator accelerator) {
			StringBuilder sb = new StringBuilder();

			if(accelerator.KeyInfo.Modifiers.HasFlag(ConsoleModifiers.Control)) {
				sb.Append("Ctrl");
			}
			if(accelerator.KeyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift)) {
				if(sb.Length > 0) { sb.Append("+"); }
				sb.Append("Shift");
			}
			if(accelerator.KeyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt)) {
				if(sb.Length > 0) { sb.Append("+"); }
				sb.Append("Alt");
			}

			if(sb.Length > 0) { sb.Append("+"); }

			if(consoleKeyToStringMap.ContainsKey(accelerator.KeyInfo.Key)) {
				sb.Append(consoleKeyToStringMap[accelerator.KeyInfo.Key]);
			}
			else {
				sb.Append(accelerator.KeyInfo.Key);
			}

			return sb.ToString();
		}

		public MenuItemBackend ()
			: this (new SWC.MenuItem())
		{
		}

		protected MenuItemBackend (object item)
		{
			this.item = item;
			this.menuItem = item as SWC.MenuItem;
			useMnemonic = true;
		}

		public void Initialize (IMenuItemEventSink eventSink)
		{
			this.eventSink = eventSink;
		}

		public object Item {
			get { return this.item; }
		}

		public SWC.MenuItem MenuItem {
			get { return this.menuItem; }
		}

		public IMenuItemEventSink EventSink {
			get { return eventSink; }
		}

		public bool Checked {
			get { return this.menuItem.IsCheckable && this.menuItem.IsChecked; }
			set {
				if (!this.menuItem.IsCheckable)
					return;
				this.menuItem.IsChecked = value;
			}
		}

		public string Label {
			get { return label; }
			set {
				label = value;
				menuItem.Header = UseMnemonic ? value : value.Replace ("_", "__");
			}
		}

		public bool UseMnemonic {
			get { return useMnemonic; }
			set
			{
				useMnemonic = value;
				Label = label;
			}
		}

		public bool Sensitive {
			get { return this.menuItem.IsEnabled; }
			set { this.menuItem.IsEnabled = value; }
		}

		public string ToolTip {
			get { return (string)this.menuItem.ToolTip; }
			set { this.menuItem.ToolTip = value; }
		}

		public bool Visible {
			get { return this.menuItem.IsVisible; }
			set { this.menuItem.Visibility = (value) ? Visibility.Visible : Visibility.Collapsed; }
		}

		public void SetImage (ImageDescription imageBackend)
		{
			if (imageBackend.IsNull)
				this.menuItem.Icon = null;
			else
				this.menuItem.Icon = new ImageBox (Context) { ImageSource = imageBackend };
		}

		public void SetSubmenu (IMenuBackend menu)
		{
			if (menu == null) {
				this.menuItem.Items.Clear ();
				if (subMenu != null) {
					subMenu.RemoveFromParentItem ();
					subMenu = null;
				}

				return;
			}

			var menuBackend = (MenuBackend)menu;
			menuBackend.RemoveFromParentItem ();

			foreach (var itemBackend in menuBackend.Items)
				this.menuItem.Items.Add (itemBackend.Item);

			menuBackend.ParentItem = this;
			subMenu = menuBackend;
		}

		public void SetType (MenuItemType type)
		{
			switch (type) {
				case MenuItemType.RadioButton:
				case MenuItemType.CheckBox:
					this.menuItem.IsCheckable = true;
					break;
				case MenuItemType.Normal:
					this.menuItem.IsCheckable = false;
					break;
			}

			this.type = type;
		}

		public override void EnableEvent (object eventId)
		{
			if (menuItem == null)
				return;

			if (eventId is MenuItemEvent) {
				switch ((MenuItemEvent)eventId) {
					case MenuItemEvent.Clicked:
						this.menuItem.Click += MenuItemClickHandler;
						break;
				}
			}
		}

		public override void DisableEvent (object eventId)
		{
			if (menuItem == null)
				return;

			if (eventId is MenuItemEvent) {
				switch ((MenuItemEvent)eventId) {
					case MenuItemEvent.Clicked:
						this.menuItem.Click -= MenuItemClickHandler;
						break;
				}
			}
		}

		void MenuItemClickHandler (object sender, EventArgs args)
		{
			Context.InvokeUserCode (eventSink.OnClicked);
		}
	}
}
