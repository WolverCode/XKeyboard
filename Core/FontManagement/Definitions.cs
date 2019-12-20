/// File: FontManagement.Definitions.cs
/// Purpose: Defines the FONT class database and it structure of custom font files.
/// Version: 1.3
/// Date Modified: 12/20/2019

/* 
Copyright (c) 2019, All rights are reserved by WolverCode
Visit: https://www.wolvercode.com

This program is licensed under the Apache License, Version 2.0 (the "License");
you may not download, install or use this program except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*/


using System;
using System.Linq;
using System.Collections.Generic;

namespace XKeyboard.Core.FontManagement
{
    /// <summary>
    /// This class provides the custom fonts and characters replaced by the intercepter. 
    /// </summary>
    [Serializable]
    public class XFont 
    {

        public XFont()
        {
            //Parameterless ctor for serialization.
            this.Name = "N/A";
            this.Description = "N/A";
            this.Author = "Unknown";
            //this.DateModified = DateTime.Now;
        }

        public DateTime DateModified;       //Date this object was last modified
        public string Author;               //Author
        public string Name;                 //Name of the current font set
        public string Description;          //Description of the font set. 
        public List<XKey> Keys;             //Dictionary of XKeys (custom characters) in this font set. 
        [NonSerialized]
        private bool isInitialized;         //Indicates whether the font set is already initialized
        [NonSerialized]
        private string file;                //The file of the current font set
        [NonSerialized]
        private string charSet;             //The current character set used. 

        public bool IsInitialized() => this.isInitialized;
        public string File() => this.file;
        public string CharSet() => this.charSet;
        internal void SetFile(string file)
        {
            this.file = file;
        }

        /// <summary>
        /// Returns true if the fontset contains a custom defined character for the specified ascii character. 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool Contains(char character)
        {
            //Get all the XKey objects whose key matches the character
            var x = this.Keys.Where(y => y.Key == character.ToString());
            //Check the count of returned XKeys
            var c = x.Count();
            //If there are none, return. 
            if (c == 0 || c < 0) return false;
            //If there are many, throw error because its UNEXPECTED to have multiple keys defined in one set. 
            if (c > 1) throw new Exception("The font set returned multiple keys for the character "+character+".");
            //The font set contains 1 key whose value matches with character. 
            return true;
        }
        /// <summary>
        /// Returns the XKey whose keycode matches with character value. 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public XKey GetValue(char character)
        {
            var x = Keys.Where(n => n.Key == character.ToString());
            var c = x.Count();
            if (c == 0 || c < 0) return null;
            if (c > 1) throw new Exception("The font set returned multiple keys for the character " + character + ".");
            return x.ElementAt(0);
        }


        /// <summary>
        /// If the character code exists in the font set, returns its defined target value. Otherwise returns the same character 
        /// to avoid missing character errors. 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public XKey GetValueSafe(char character)
        {
            var x = this.GetValue(character);
            if (x == null)
                return new XKey(0, character, character.ToString());
            return x;
        }
        /// <summary>
        /// Creates a new XFont set with a name, description and its output file.
        /// </summary>
        /// <param name="name">The name of the font set</param>
        /// <param name="description">The description for the font set</param>
        /// <param name="file">The output filename for the font set</param>
        public XFont(string name, string description, string charSET)
        {
            this.Description = description;
            this.Name = name;
            this.charSet = charSET;
        }
        /// <summary>
        /// Serializes and exports the current font set in a file.
        /// </summary>
        /// <param name="fileName">The output file name</param>
        public void Export(string fileName)
        {
            /////TO-DO/////
            /// Serialize this object and save it as fileName
            ///////////////
            this.DateModified = DateTime.Now;
            this.file = fileName;
            Program.fManager.Serializer.Serialize<XFont>(this, fileName);
        }
        /// <summary>
        /// Loads the XFont set from the specified file. 
        /// </summary>
        /// <param name="fileName">The file to load the font set from.</param>
        public static XFont Load(string fileName)
        {
            /////TO-DO/////
            /// Deserialize fileName and create the XFont set
            ///////////////
            XFont xf = Program.fManager.Serializer.Deserialize<XFont>(fileName);
            xf.SetFile(fileName);
            return xf;
        }
        /// <summary>
        /// Initializes the empty keys table for the keys with their respective keyCode.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="keyboardManager">The keyboard manager class used to intercept keys to detect keycodes. </param>
        public void InitKeys(KManager keyboardManager, string keys = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ")
        {
            /////TO-DO/////
            /// Start listening for the keys and send all standard keys
            /// 
            /// Algorithm:
            ///     1. Subscribe to the OnKeyDown event handler on LowLevelKeyboard hook.
            ///     2. Send the key stroke here and set the flag
            ///     3. When the event OnKeyDown is fired, match the flag and note the keycode
            ///     4. Add the key with the keycode to dictionary.
            ///////////////
            //if (IsInitialized) return;
            if (string.IsNullOrEmpty(keys))
                keys = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ";
            Logger.Log("XFont.Init(): Detecting keycodes for the charset. ");
            charSet = keys;
            isInitialized = false;
            //Save keyboard state and disable keyboard temp.
            var kbState = keyboardManager.Mode;
            keyboardManager.Mode = KeyboardMode.Disabled; //Disable the keyboard and key interception 
            Logger.Log("XFont.Init(): Keyboard lock was activated to prevent miss-behaviour.");
            char lastKey = ' ';
            int lastKeyCode = 0;
            //Store all the keycodes in the list.
            this.Keys = new  List<XKey>();
            var act = new KManager.OnKeyBlock((i, k) =>
           {
               //Add and note the key.
               char x = '\0';
               x = char.ToUpper(lastKey);
               if (i == x)
               {
                   //THe key is the one sent from app. Note the keycode
                   this.Keys.Add(new XKey(i, lastKey, lastKey.ToString()));
               }
               lastKeyCode = i;
           });
            //Subscribe to keyblock event. 
            keyboardManager.KeyBlock += act;
            Logger.Log("XFont.Init(): Sending keystrokes to detect keycodes...");
            foreach (var c in keys)
            {
                lastKey = c;
                System.Windows.Forms.SendKeys.SendWait(c.ToString());
            }
            Logger.Log("XFont.Init(): Keys noted, unlocking keyboard...");
            keyboardManager.Mode = kbState;
            isInitialized = true;
            keyboardManager.KeyBlock -= act;
        }
    }
    [Serializable]
    public class XKey
    {
        public int KeyCode
        {
            get; set;
        }
        public string Key { get; set; }
        public string TargetValue
        {
            get; set;
        }
        public XKey(int keyCode, char key, string targetValue)
        {
            this.KeyCode = keyCode;
            this.Key = key.ToString();
            this.TargetValue = targetValue;
        }
        public XKey()
        {

        }
        public override string ToString()
        {
            return Key + ":" + TargetValue;
        }
    }
}
