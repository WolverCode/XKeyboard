/// File: Program.cs
/// Purpose: Defines Program Entry Point
/// Version: 1.3
/// Date Modified: 12/20/2019
/// 
/// Changes
/// =======
/// Added XConfig.dll. Shifted to a separate library. 
/// Shifted global error handlers to the top to catch initialization errors. 
/// Removed function names from Log calls. (see Logger.cs)


/* 
Copyright (c) 2019, All rights are reserved by Team WolverCode 
https://www.wolvercode.com

This program is licensed under the Apache License, Version 2.0 (the "License");
you may not download, install or use this program except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*/ 

using System;
using System.Windows.Input;
using XConfig;
using XKeyboard.Core;
using XKeyboard.Core.FontManagement;
using static XKeyboard.Core.Logger;

namespace XKeyboard
{
    /// <summary>
    /// This class defines the static program functions.
    /// </summary>
    static class Program
    {
        // FontManager stores current font set and is used to access custom characters. 
        public static FontManager fManager;
        // KManager manages the low-level keyboard procedures and hooks. It also controls the keyboard and modifiers and reports the input
        public static KManager kManager;
        // ConfigManager is used save and retrieve application configuration (i.e. last active font, keyboard state, etc.)
        public static ConfigManager cManager;
        //Stores last key state. In AutoCap mode, this is true when user presses SPACE and becomes false after one key press. In ReverseCap mode, this is reverse of AutoCap
        //In AlterCap mode, this flag keeps swapping. 
        static bool _toCap = false; 
        /// <summary>
        /// Initializes the Font, Keyboard, Config and Notification manager. 
        /// </summary>
        public static void Initiate()
        {
            //Subscribe to Application shutdown event. 
            App.Current.Exit += (xx, ee) =>
                        {
                            Log($"Application shutdown was requested. Unregistering keyboard hook...");
                            kManager.Unregister();
                            Log($"Saving settings...");
                            SaveSettings();
                            Log($"Closing logger. Good bye!");
                            Logger.Save();  //Save the log file
                            NotificationManager.NotifyIcon.Visible = false;
                        };
            //Subscribe to Global Exception Handlers. (in case there are un-expected runtime errors)
            AppDomain.CurrentDomain.FirstChanceException += (oo, ee) =>
            {
                Log($"A first chance of exception was detected on the object of type: {oo.GetType()}. Exception: {ee.Exception}", MessagePriority.Low, MessageKind.Warning);
            };
            AppDomain.CurrentDomain.UnhandledException += (oo, ee) =>
            {
                Log($"An unhandled error occured in the application on an object of type {oo.GetType()}. {(ee.IsTerminating ? "The application must be terminated in order to avoid any miss-behaviour or data loss. " : "The application can continue however, it is recommended to close the application and start again. ")} Exception: {ee.ExceptionObject as Exception}", MessagePriority.High, MessageKind.Error);
            };
            //Initialize logger. 
            Logger.Init();
            Log("Initializing FontManager...");
            fManager = new FontManager(new XSerializer());
            Log("Instializing NotificationManager...");
            NotificationManager.Initiate();
            Log("Initializing XConfig...");
            cManager = new ConfigManager("config.cfg", new BSerializer(), true);    //Using BinarySerializer because the class contains complicated data objects. 
            //Set up Keyboard Manager.
            Log("Initializing KeyboadManager...");
            kManager = new KManager();
            //Register to low level keyboard hook
            kManager.Register();
            //Subscribe to input event from KManager. 
            kManager.KeyIntercept += Key_Intercepted;
            kManager.KeyForward += KManager_KeyForward;
            //Load settings.
            Log("Loading last configuration...");
            cManager.AutoSave = cManager.Get<bool>(ConfigKeys.Autosave);
            //Load recent font.
            string lastFont = cManager.Get<string>(ConfigKeys.LastFont);
            if (string.IsNullOrEmpty(lastFont) == false && System.IO.File.Exists(lastFont))
            {
                Log("Loading recent font: " + lastFont);
                fManager.CurrentFont = XFont.Load(lastFont);
            }
            //Set keyboard state to last recent state 
            kManager.Mode = cManager.Get<KeyboardMode>(ConfigKeys.KeyboardState);
            //Set beep on keyboard block to last setting.
            kManager.Beep = cManager.Get<bool>(ConfigKeys.InputBlockBeep);
            
        }

        private static void KManager_KeyForward(int keyCode, KManager km)
        {
            if (keyCode == 13   && km.Mode == KeyboardMode.AutoCapitalization)
                _toCap = true;
        }

        /// <summary>
        /// Saves the current settings in application configuration file using ConfigManager.
        /// </summary>
        public static void SaveSettings()
        {
            cManager.Set(ConfigKeys.Autosave, cManager.AutoSave);
            cManager.Set(ConfigKeys.InputBlockBeep, kManager?.Beep);
            cManager.Set(ConfigKeys.KeyboardState, kManager?.Mode);
            cManager.Set(ConfigKeys.LastFont, fManager.CurrentFont?.File());
            cManager.Save();
        }
        /// <summary>
        /// Called when a key is intercepted (blocked and modified). 
        /// </summary>
        /// <param name="keyCode">The numeric code of the input key.</param>
        /// <param name="km">The keyboard manager handling the event.</param>
        private static void Key_Intercepted(int keyCode, KManager km)
        {
            //Check if current font is undefined, if so: switch keyboard mode and return. 
            if(fManager?.CurrentFont == null && km.Mode == KeyboardMode.Intercept)
            {
                kManager.Mode = KeyboardMode.Enabled;
                Logger.Write("KManager switched the keyboard mode because no font set was specified for key interception. ", MessagePriority.Mid, MessageKind.Info);
                return;
            }
            //Filter the key. I.e. convert 'A' to 'a' if SHIFT or CAPSLOCK isn't toggled and vice versa. 
            var fKey = FilterKey((char)keyCode);
            if (_toCap) fKey = char.ToUpper(fKey);
            //Apply special mode here. 
            switch (km.Mode)
            {
                case KeyboardMode.AutoCapitalization:
                    if (fKey == ' ')
                        _toCap = true;
                    else
                        _toCap = false;
                    break;
                case KeyboardMode.AlterCapitalization:
                    if (fKey != ' ')
                        _toCap = !_toCap;
                    break;
                default:
                    break;
            }
            //Get the associated custom key with the keycode and forward the custom key (character)
            if (fManager.CurrentFont == null)
                kManager.SendKey(new XKey(-1, fKey, fKey.ToString()));
            else
                kManager.SendKey(fManager.CurrentFont.GetValueSafe(fKey));
        }
        static char FilterKey(char key)
        {
            // Check if there is any modifier key, if there is: Convert the character to uppercase or lowercase(if capslock is on)
            if (Keyboard.GetKeyStates(Key.LeftShift) == KeyStates.Down || Keyboard.GetKeyStates(Key.RightShift) == KeyStates.Down)
            {
                if (Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.Toggled)
                    return char.ToLower(key);
                return char.ToUpper(key);
            }
            else if (Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.Toggled)
                return char.ToUpper(key);
            return char.ToLower(key);
        }
    }
    /// <summary>
    /// This class defines the constant keys used to save and retreive settings.
    /// </summary>
    public static class ConfigKeys
    {
        public const string Autosave = "AutoSave";
        public const string LastFont = "LastFont";
        public const string InputBlockBeep = "InputBlockBeep";
        public const string KeyboardState = "KeyboardState";
        public const string StartupKey = "XKeyboard";
    }
}
