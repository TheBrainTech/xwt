using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Xwt {
	public class KeyboardKey {
		private static List<KeyboardKey> allKeys = new List<KeyboardKey>();

		public static ReadOnlyCollection<KeyboardKey> AllKeys {
			get {
				return allKeys.AsReadOnly();
			}
		}

		public string ConfigurationString { get; set; }
		public int MacInputKeyCode { get; set; }
		public char MacMenuCharacter { get; set; }
		public char MacInputCharacter { get; set; }
		public ConsoleKey ConsoleKey { get; set; }

		static KeyboardKey() {
			Initialize();
		}

		private KeyboardKey() { }

		private static void Initialize() {
			allKeys.Add(new KeyboardKey { ConfigurationString = "A",     MacInputKeyCode = -1,  MacInputCharacter = 'A',       MacMenuCharacter = 'A',         ConsoleKey = ConsoleKey.A });
			allKeys.Add(new KeyboardKey { ConfigurationString = "B",     MacInputKeyCode = -1,  MacInputCharacter = 'B',       MacMenuCharacter = 'B',         ConsoleKey = ConsoleKey.B });
			allKeys.Add(new KeyboardKey { ConfigurationString = "C",     MacInputKeyCode = -1,  MacInputCharacter = 'C',       MacMenuCharacter = 'C',         ConsoleKey = ConsoleKey.C });
			allKeys.Add(new KeyboardKey { ConfigurationString = "D",     MacInputKeyCode = -1,  MacInputCharacter = 'D',       MacMenuCharacter = 'D',         ConsoleKey = ConsoleKey.D });
			allKeys.Add(new KeyboardKey { ConfigurationString = "E",     MacInputKeyCode = -1,  MacInputCharacter = 'E',       MacMenuCharacter = 'E',         ConsoleKey = ConsoleKey.E });
			allKeys.Add(new KeyboardKey { ConfigurationString = "F",     MacInputKeyCode = -1,  MacInputCharacter = 'F',       MacMenuCharacter = 'F',         ConsoleKey = ConsoleKey.F });
			allKeys.Add(new KeyboardKey { ConfigurationString = "G",     MacInputKeyCode = -1,  MacInputCharacter = 'G',       MacMenuCharacter = 'G',         ConsoleKey = ConsoleKey.G });
			allKeys.Add(new KeyboardKey { ConfigurationString = "H",     MacInputKeyCode = -1,  MacInputCharacter = 'H',       MacMenuCharacter = 'H',         ConsoleKey = ConsoleKey.H });
			allKeys.Add(new KeyboardKey { ConfigurationString = "I",     MacInputKeyCode = -1,  MacInputCharacter = 'I',       MacMenuCharacter = 'I',         ConsoleKey = ConsoleKey.I });
			allKeys.Add(new KeyboardKey { ConfigurationString = "J",     MacInputKeyCode = -1,  MacInputCharacter = 'J',       MacMenuCharacter = 'J',         ConsoleKey = ConsoleKey.J });
			allKeys.Add(new KeyboardKey { ConfigurationString = "K",     MacInputKeyCode = -1,  MacInputCharacter = 'K',       MacMenuCharacter = 'K',         ConsoleKey = ConsoleKey.K });
			allKeys.Add(new KeyboardKey { ConfigurationString = "L",     MacInputKeyCode = -1,  MacInputCharacter = 'L',       MacMenuCharacter = 'L',         ConsoleKey = ConsoleKey.L });
			allKeys.Add(new KeyboardKey { ConfigurationString = "M",     MacInputKeyCode = -1,  MacInputCharacter = 'M',       MacMenuCharacter = 'M',         ConsoleKey = ConsoleKey.M });
			allKeys.Add(new KeyboardKey { ConfigurationString = "N",     MacInputKeyCode = -1,  MacInputCharacter = 'N',       MacMenuCharacter = 'N',         ConsoleKey = ConsoleKey.N });
			allKeys.Add(new KeyboardKey { ConfigurationString = "O",     MacInputKeyCode = -1,  MacInputCharacter = 'O',       MacMenuCharacter = 'O',         ConsoleKey = ConsoleKey.O });
			allKeys.Add(new KeyboardKey { ConfigurationString = "P",     MacInputKeyCode = -1,  MacInputCharacter = 'P',       MacMenuCharacter = 'P',         ConsoleKey = ConsoleKey.P });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Q",     MacInputKeyCode = -1,  MacInputCharacter = 'Q',       MacMenuCharacter = 'Q',         ConsoleKey = ConsoleKey.Q });
			allKeys.Add(new KeyboardKey { ConfigurationString = "R",     MacInputKeyCode = -1,  MacInputCharacter = 'R',       MacMenuCharacter = 'R',         ConsoleKey = ConsoleKey.R });
			allKeys.Add(new KeyboardKey { ConfigurationString = "S",     MacInputKeyCode = -1,  MacInputCharacter = 'S',       MacMenuCharacter = 'S',         ConsoleKey = ConsoleKey.S });
			allKeys.Add(new KeyboardKey { ConfigurationString = "T",     MacInputKeyCode = -1,  MacInputCharacter = 'T',       MacMenuCharacter = 'T',         ConsoleKey = ConsoleKey.T });
			allKeys.Add(new KeyboardKey { ConfigurationString = "U",     MacInputKeyCode = -1,  MacInputCharacter = 'U',       MacMenuCharacter = 'U',         ConsoleKey = ConsoleKey.U });
			allKeys.Add(new KeyboardKey { ConfigurationString = "V",     MacInputKeyCode = -1,  MacInputCharacter = 'V',       MacMenuCharacter = 'V',         ConsoleKey = ConsoleKey.V });
			allKeys.Add(new KeyboardKey { ConfigurationString = "W",     MacInputKeyCode = -1,  MacInputCharacter = 'W',       MacMenuCharacter = 'W',         ConsoleKey = ConsoleKey.W });
			allKeys.Add(new KeyboardKey { ConfigurationString = "X",     MacInputKeyCode = -1,  MacInputCharacter = 'X',       MacMenuCharacter = 'X',         ConsoleKey = ConsoleKey.X });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Y",     MacInputKeyCode = -1,  MacInputCharacter = 'Y',       MacMenuCharacter = 'Y',         ConsoleKey = ConsoleKey.Y });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Z",     MacInputKeyCode = -1,  MacInputCharacter = 'Z',       MacMenuCharacter = 'Z',         ConsoleKey = ConsoleKey.Z });

			allKeys.Add(new KeyboardKey { ConfigurationString = "0",     MacInputKeyCode = -1,  MacInputCharacter = '0',       MacMenuCharacter = '0',         ConsoleKey = ConsoleKey.D0 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "1",     MacInputKeyCode = -1,  MacInputCharacter = '1',       MacMenuCharacter = '1',         ConsoleKey = ConsoleKey.D1 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "2",     MacInputKeyCode = -1,  MacInputCharacter = '2',       MacMenuCharacter = '2',         ConsoleKey = ConsoleKey.D2 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "3",     MacInputKeyCode = -1,  MacInputCharacter = '3',       MacMenuCharacter = '3',         ConsoleKey = ConsoleKey.D3 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "4",     MacInputKeyCode = -1,  MacInputCharacter = '4',       MacMenuCharacter = '4',         ConsoleKey = ConsoleKey.D4 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "5",     MacInputKeyCode = -1,  MacInputCharacter = '5',       MacMenuCharacter = '5',         ConsoleKey = ConsoleKey.D5 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "6",     MacInputKeyCode = -1,  MacInputCharacter = '6',       MacMenuCharacter = '6',         ConsoleKey = ConsoleKey.D6 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "7",     MacInputKeyCode = -1,  MacInputCharacter = '7',       MacMenuCharacter = '7',         ConsoleKey = ConsoleKey.D7 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "8",     MacInputKeyCode = -1,  MacInputCharacter = '8',       MacMenuCharacter = '8',         ConsoleKey = ConsoleKey.D8 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "9",     MacInputKeyCode = -1,  MacInputCharacter = '9',       MacMenuCharacter = '9',         ConsoleKey = ConsoleKey.D9 });

			allKeys.Add(new KeyboardKey { ConfigurationString = "`",     MacInputKeyCode = -1,  MacInputCharacter = '`',       MacMenuCharacter = '`',         ConsoleKey = ConsoleKey.Oem3 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "-",     MacInputKeyCode = -1,  MacInputCharacter = '-',       MacMenuCharacter = '-',         ConsoleKey = ConsoleKey.OemMinus });
			allKeys.Add(new KeyboardKey { ConfigurationString = "=",     MacInputKeyCode = -1,  MacInputCharacter = '=',       MacMenuCharacter = '=',         ConsoleKey = ConsoleKey.OemPlus });

			allKeys.Add(new KeyboardKey { ConfigurationString = "[",     MacInputKeyCode = -1,  MacInputCharacter = '[',       MacMenuCharacter = '[',         ConsoleKey = ConsoleKey.Oem4 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "]",     MacInputKeyCode = -1,  MacInputCharacter = ']',       MacMenuCharacter = ']',         ConsoleKey = ConsoleKey.Oem6 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "\\",    MacInputKeyCode = -1,  MacInputCharacter = '\\',      MacMenuCharacter = '\\',        ConsoleKey = ConsoleKey.Oem5 });
			allKeys.Add(new KeyboardKey { ConfigurationString = ";",     MacInputKeyCode = -1,  MacInputCharacter = ';',       MacMenuCharacter = ';',         ConsoleKey = ConsoleKey.Oem1 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "'",     MacInputKeyCode = -1,  MacInputCharacter = '\'',      MacMenuCharacter = '\'',        ConsoleKey = ConsoleKey.Oem7 });
			allKeys.Add(new KeyboardKey { ConfigurationString = ",",     MacInputKeyCode = -1,  MacInputCharacter = ',',       MacMenuCharacter = ',',         ConsoleKey = ConsoleKey.OemComma });
			allKeys.Add(new KeyboardKey { ConfigurationString = ".",     MacInputKeyCode = -1,  MacInputCharacter = '.',       MacMenuCharacter = '.',         ConsoleKey = ConsoleKey.OemPeriod });
			allKeys.Add(new KeyboardKey { ConfigurationString = "/",     MacInputKeyCode = -1,  MacInputCharacter = '/',       MacMenuCharacter = '/',         ConsoleKey = ConsoleKey.Oem2 });

			allKeys.Add(new KeyboardKey { ConfigurationString = "Up",    MacInputKeyCode = -1,  MacInputCharacter = (char)30,  MacMenuCharacter = (char)63232, ConsoleKey = ConsoleKey.UpArrow });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Down",  MacInputKeyCode = -1,  MacInputCharacter = (char)31,  MacMenuCharacter = (char)63233, ConsoleKey = ConsoleKey.DownArrow });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Left",  MacInputKeyCode = -1,  MacInputCharacter = (char)28,  MacMenuCharacter = (char)63234, ConsoleKey = ConsoleKey.LeftArrow });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Right", MacInputKeyCode = -1,  MacInputCharacter = (char)29,  MacMenuCharacter = (char)63235, ConsoleKey = ConsoleKey.RightArrow });

			allKeys.Add(new KeyboardKey { ConfigurationString = "Del",   MacInputKeyCode = -1,  MacInputCharacter = (char)127, MacMenuCharacter = (char)63272, ConsoleKey = ConsoleKey.Delete });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Home",  MacInputKeyCode = -1,  MacInputCharacter = (char)1,   MacMenuCharacter = (char)63273, ConsoleKey = ConsoleKey.Home });
			allKeys.Add(new KeyboardKey { ConfigurationString = "End",   MacInputKeyCode = -1,  MacInputCharacter = (char)4,   MacMenuCharacter = (char)63275, ConsoleKey = ConsoleKey.End });
			allKeys.Add(new KeyboardKey { ConfigurationString = "PgUp",  MacInputKeyCode = -1,  MacInputCharacter = (char)11,  MacMenuCharacter = (char)63276, ConsoleKey = ConsoleKey.PageUp });
			allKeys.Add(new KeyboardKey { ConfigurationString = "PgDn",  MacInputKeyCode = -1,  MacInputCharacter = (char)12,  MacMenuCharacter = (char)63277, ConsoleKey = ConsoleKey.PageDown });

			allKeys.Add(new KeyboardKey { ConfigurationString = "Backspace", MacInputKeyCode = 51,  MacInputCharacter = '\0', MacMenuCharacter = (char)63274, ConsoleKey = ConsoleKey.Backspace });

			allKeys.Add(new KeyboardKey { ConfigurationString = "F1",    MacInputKeyCode = 122, MacInputCharacter = '\0',      MacMenuCharacter = (char)63236, ConsoleKey = ConsoleKey.F1 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "F2",    MacInputKeyCode = 120, MacInputCharacter = '\0',      MacMenuCharacter = (char)63237, ConsoleKey = ConsoleKey.F2 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "F3",    MacInputKeyCode = 99,  MacInputCharacter = '\0',      MacMenuCharacter = (char)63238, ConsoleKey = ConsoleKey.F3 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "F4",    MacInputKeyCode = 118, MacInputCharacter = '\0',      MacMenuCharacter = (char)63239, ConsoleKey = ConsoleKey.F4 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "F5",    MacInputKeyCode = 96,  MacInputCharacter = '\0',      MacMenuCharacter = (char)63240, ConsoleKey = ConsoleKey.F5 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "F6",    MacInputKeyCode = 97,  MacInputCharacter = '\0',      MacMenuCharacter = (char)63241, ConsoleKey = ConsoleKey.F6 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "F7",    MacInputKeyCode = 98,  MacInputCharacter = '\0',      MacMenuCharacter = (char)63242, ConsoleKey = ConsoleKey.F7 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "F8",    MacInputKeyCode = 100, MacInputCharacter = '\0',      MacMenuCharacter = (char)63243, ConsoleKey = ConsoleKey.F8 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "F9",    MacInputKeyCode = 101, MacInputCharacter = '\0',      MacMenuCharacter = (char)63244, ConsoleKey = ConsoleKey.F9 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "F10",   MacInputKeyCode = 109, MacInputCharacter = '\0',      MacMenuCharacter = (char)63245, ConsoleKey = ConsoleKey.F10 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "F11",   MacInputKeyCode = 103, MacInputCharacter = '\0',      MacMenuCharacter = (char)63426, ConsoleKey = ConsoleKey.F11 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "F12",   MacInputKeyCode = 111, MacInputCharacter = '\0',      MacMenuCharacter = (char)63247, ConsoleKey = ConsoleKey.F12 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "F13",   MacInputKeyCode = 105, MacInputCharacter = '\0',      MacMenuCharacter = (char)63248, ConsoleKey = ConsoleKey.F13 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "F14",   MacInputKeyCode = 107, MacInputCharacter = '\0',      MacMenuCharacter = (char)63249, ConsoleKey = ConsoleKey.F14 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "F15",   MacInputKeyCode = 113, MacInputCharacter = '\0',      MacMenuCharacter = (char)63250, ConsoleKey = ConsoleKey.F15 });

			allKeys.Add(new KeyboardKey { ConfigurationString = "Esc",   MacInputKeyCode = 53,  MacInputCharacter = '\0',      MacMenuCharacter = (char)27,    ConsoleKey = ConsoleKey.Escape });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Clear", MacInputKeyCode = 71,  MacInputCharacter = '\0',      MacMenuCharacter = (char)63289, ConsoleKey = ConsoleKey.Clear });

			allKeys.Add(new KeyboardKey { ConfigurationString = "Space", MacInputKeyCode = -1,  MacInputCharacter = ' ',       MacMenuCharacter = ' ',         ConsoleKey = ConsoleKey.Spacebar });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Tab",   MacInputKeyCode = -1,  MacInputCharacter = '\t',      MacMenuCharacter = '\t',        ConsoleKey = ConsoleKey.Tab });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Enter", MacInputKeyCode = -1,  MacInputCharacter = '\r',      MacMenuCharacter = '\r',        ConsoleKey = ConsoleKey.Enter });

			allKeys.Add(new KeyboardKey { ConfigurationString = "Num 0", MacInputKeyCode = -1,  MacInputCharacter = '\0',      MacMenuCharacter = (char)0,     ConsoleKey = ConsoleKey.NumPad0 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Num 1", MacInputKeyCode = -1,  MacInputCharacter = '\0',      MacMenuCharacter = (char)0,     ConsoleKey = ConsoleKey.NumPad1 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Num 2", MacInputKeyCode = -1,  MacInputCharacter = '\0',      MacMenuCharacter = (char)0,     ConsoleKey = ConsoleKey.NumPad2 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Num 3", MacInputKeyCode = -1,  MacInputCharacter = '\0',      MacMenuCharacter = (char)0,     ConsoleKey = ConsoleKey.NumPad3 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Num 4", MacInputKeyCode = -1,  MacInputCharacter = '\0',      MacMenuCharacter = (char)0,     ConsoleKey = ConsoleKey.NumPad4 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Num 5", MacInputKeyCode = -1,  MacInputCharacter = '\0',      MacMenuCharacter = (char)0,     ConsoleKey = ConsoleKey.NumPad5 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Num 6", MacInputKeyCode = -1,  MacInputCharacter = '\0',      MacMenuCharacter = (char)0,     ConsoleKey = ConsoleKey.NumPad6 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Num 7", MacInputKeyCode = -1,  MacInputCharacter = '\0',      MacMenuCharacter = (char)0,     ConsoleKey = ConsoleKey.NumPad7 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Num 8", MacInputKeyCode = -1,  MacInputCharacter = '\0',      MacMenuCharacter = (char)0,     ConsoleKey = ConsoleKey.NumPad8 });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Num 9", MacInputKeyCode = -1,  MacInputCharacter = '\0',      MacMenuCharacter = (char)0,     ConsoleKey = ConsoleKey.NumPad9 });

			allKeys.Add(new KeyboardKey { ConfigurationString = "Num /", MacInputKeyCode = -1,  MacInputCharacter = '\0',      MacMenuCharacter = (char)0,     ConsoleKey = ConsoleKey.Divide });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Num *", MacInputKeyCode = -1,  MacInputCharacter = '\0',      MacMenuCharacter = (char)0,     ConsoleKey = ConsoleKey.Multiply });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Num -", MacInputKeyCode = -1,  MacInputCharacter = '\0',      MacMenuCharacter = (char)0,     ConsoleKey = ConsoleKey.Subtract });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Num +", MacInputKeyCode = -1,  MacInputCharacter = '\0',      MacMenuCharacter = (char)0,     ConsoleKey = ConsoleKey.Add });
			allKeys.Add(new KeyboardKey { ConfigurationString = "Num .", MacInputKeyCode = -1,  MacInputCharacter = '\0',      MacMenuCharacter = (char)0,     ConsoleKey = ConsoleKey.Decimal });
		}
	}
}
