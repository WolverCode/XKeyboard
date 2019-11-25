/// File: Core.NotificationManager.cs
/// Purpose: Defines the NotificationManager used to control notification icon and application interface
/// Version: 1.2
/// Date Modified: 11/24/2019
/// 
/// Changes
/// =======
/// Removed function names from Log() calls. 
/// Fix: NotifyIcon wasn't updating after font change. 
/// 

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
using System.Windows.Forms;
using static XKeyboard.Core.Logger;
namespace XKeyboard.Core
{
    /// <summary>
    /// This class contains code related to the NotifyIcon and Windows Desktop Notification. 
    /// </summary>
    class NotificationManager
    {
        //The underlying notify icon.
        public static System.Windows.Forms.NotifyIcon NotifyIcon;
        //Static window objects used to prevent from showing multiple windows if requested by user. 
        public static MainWindow MAIN_WINDOW;
        public static UI.frmAbout ABOUT_WINDOW;
        /// <summary>
        /// Initialize the NotifyIcon and ContextMenu
        /// </summary>
        public static void Initiate()
        {
            //Instantiate NotifyIcon.
            Log("Initializing NotifyIcon...");
            NotifyIcon = new System.Windows.Forms.NotifyIcon();
            NotifyIcon.Text = "XKeyboard - Running";
            NotifyIcon.Visible = true;
            NotifyIcon.BalloonTipTitle = "XKeyboard - Info";
            NotifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            NotifyIcon.Icon = Properties.Resources.x256_enabled; //Important, set ICON to enable NotifyIcon

            //Instantiate ContextMenu
            /// Context Menu Design
            /// Exit
            /// About
            /// Settings >
            ///             Beep on input block
            ///             Clear Logs
            ///             Start with windows
            /// XKeyboard > 
            ///             --SETTINGS--
            ///             Enabled
            ///             Disabled
            ///             Intercept
            ///             ------------
            ///             Manage Fonts
            ///             Open Folder
            ///             ------------
            ///             Stroked Text
            ///             ....
            /// View Log File
            /// 

            Log("Initializing Context Menu Items...");
            //Exit item
            var itemExit = new MenuItem("Exit");
            var itemRestart = new MenuItem("Restart");
            //Settings Item Menu
            var itemSetting = new MenuItem("Settings");
            var itemSettingBeepInputBlocked = new MenuItem("Beep on input block");
            var itemSettingClearLogs = new MenuItem("Clear Logs");
            var itemSettingStartup = new MenuItem("Start with Windows");
            itemSetting.MenuItems.Add(itemSettingBeepInputBlocked);
            itemSetting.MenuItems.Add(itemSettingClearLogs);
            itemSetting.MenuItems.Add(itemSettingStartup);
            //About menu item
            var itemAbout = new MenuItem("About");
            //Keyboard settings menu item
            var itemKeyboard = new MenuItem("XKeyboard");
            var itemKeyboardEnabled = new MenuItem("Enabled") { Name = "itemKeyboardEnabled" };
            var itemKeyboardDisabled = new MenuItem("Disabled") { Name = "itemKeyboardDisabled" };
            //var itemKeyboardIntercept = new MenuItem("Intercept") { Name = "itemKeyboardIntercept" };
            var itemFontsManage = new MenuItem("Edit Fonts");
            var itemFontsExplore = new MenuItem("Open Folder");
            
            //View logs menu item.
            var itemLogs = new MenuItem("View Logs");
            //Extras menu item.

            //Occures when any of application setting is changed. 
            Action settingsChanged = delegate
            {
                Log("Settings change detected.");
                if (Program.kManager == null) return;
                //Check keyboard state and set the icon accordingly.
                switch (Program.kManager.KeyboardState)
                {
                    case KeyboardState.Enabled:
                        itemKeyboardEnabled.Checked = true;
                        itemKeyboardDisabled.Checked = false;
                        // itemKeyboardIntercept.Checked = false;
                        NotifyIcon.Icon = Properties.Resources.x256_enabled;
                        break;
                    case KeyboardState.Disabled:
                        itemKeyboardEnabled.Checked = false;
                        itemKeyboardDisabled.Checked = true;
                        // itemKeyboardIntercept.Checked = false;
                        NotifyIcon.Icon = Properties.Resources.x256_disabled;
                        break;
                    case KeyboardState.Intercept:
                        itemKeyboardEnabled.Checked = false;
                        itemKeyboardDisabled.Checked = false;
                        // itemKeyboardIntercept.Checked = true;
                        NotifyIcon.Icon = Properties.Resources.x256_intercept;
                        break;
                    default:
                        break;
                }
                NotifyIcon.Visible = false;
                NotifyIcon.Text = "XKeyboard - " +
                ((Program.kManager.KeyboardState == KeyboardState.Intercept) ? "Intercepting" : (Program.kManager.KeyboardState == KeyboardState.Enabled) ? "Enabled" : "Disabled");
                itemSettingBeepInputBlocked.Checked = Program.kManager.Beep;
                itemSettingClearLogs.Enabled = System.IO.File.Exists("logs.txt");
                itemSettingStartup.Checked = Helper.IsStartup();
                NotifyIcon.Visible = true;
            };
            //Assign ContextMenu to NotifyIcon and add items
            NotifyIcon.ContextMenu = new ContextMenu(new MenuItem[] 
            {
                itemKeyboard,
                itemSetting,
                itemLogs,
                itemAbout,
                itemRestart,
                itemExit
            });
            //Occures when context menu is being showed. 
            NotifyIcon.ContextMenu.Popup += (xx, ee) =>
            {
//Menu Items are added here. 
                #region MenuPopup
                itemKeyboard.MenuItems.Clear();
                itemKeyboard.MenuItems.Add(new MenuItem("--XKEYBOARD--") { Enabled = false });
                itemKeyboard.MenuItems.Add(itemKeyboardDisabled);
                itemKeyboard.MenuItems.Add(itemKeyboardEnabled);
                itemKeyboard.MenuItems.Add(new MenuItem("MANAGE".Fill("--XKEYBOARD--".Length, '-')) { Enabled = false });
                itemKeyboard.MenuItems.Add(itemFontsManage);
                itemKeyboard.MenuItems.Add(itemFontsExplore);
                itemKeyboard.MenuItems.Add(new MenuItem("FONTS".Fill("--XKEYBOARD--".Length, '-')) { Enabled = false });
                foreach (var i in Program.fManager.GetFonts())
                {
                    MenuItem mi = new MenuItem();
                    mi.Text = i.Name;
                    //On font item click, load the font
                    mi.Click += delegate
                    {
                        mi.Checked = true;
                        Program.fManager.CurrentFont = i;
                        Program.kManager.KeyboardState = KeyboardState.Intercept;
                    };
                    //Check the current selected font.
                    if (i.File() == Program.fManager?.CurrentFont?.File())
                        mi.Checked = true;
                    itemKeyboard.MenuItems.Add(mi);
                }
                settingsChanged();
                #endregion
            };
            //Occures when about menu item is clicked. 
            itemAbout.Click += delegate
            {
                //Show about form.
                if (ABOUT_WINDOW != null) ABOUT_WINDOW = null;
                ABOUT_WINDOW = new UI.frmAbout();
                ABOUT_WINDOW.Show();
            };
            //Occures when exit menu item is clicked. 
            itemExit.Click += delegate
            {
                App.Current.Shutdown();
            };
            itemRestart.Click += delegate
            {
                Helper.Restart();
            };
            //Settings items clicked. 
            itemSettingBeepInputBlocked.Click += delegate
            {
                Program.kManager.Beep = !Program.kManager.Beep;
                settingsChanged();
            };
            //Occures when clear log item is clicked. 
            itemSettingClearLogs.Click += delegate
            {
                Logger.Save();
                if (System.IO.File.Exists(Application.StartupPath + "\\logs.log"))
                    System.IO.File.Delete(Application.StartupPath + "\\logs.log");
            };
            //Occures when startup item is clicked. 
            itemSettingStartup.Click += delegate
            {
                if (itemSettingStartup.Checked == true && Helper.IsStartup())
                    Helper.RemoveFromStartup();
                else
                    Helper.AddToStartup();
            };
            //oCcures when items in keyboard menu are clicked. 
            var itemKeyboardClick = new EventHandler((oo, ee) =>
            {
                #region keyboard item clicked
                ((MenuItem)oo).Checked = true;
                Program.fManager.CurrentFont = null;
                switch (((MenuItem)oo).Name)
                {
                    default:
                        break;
                    case nameof(itemKeyboardDisabled):
                        //Disable keyboard
                        Program.kManager.KeyboardState = KeyboardState.Disabled;
                        break;
                    case nameof(itemKeyboardEnabled):
                        //Enable keyboard
                        Program.kManager.KeyboardState = KeyboardState.Enabled;
                        break;
                }
                settingsChanged();
                #endregion 
            });
            itemKeyboardDisabled.Click += itemKeyboardClick;
            itemKeyboardEnabled.Click += itemKeyboardClick;
            //Explore Font item menu clicked
            itemFontsExplore.Click += delegate
            {
                if (System.IO.Directory.Exists(Application.StartupPath + "\\Fonts\\"))
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\Fonts\\");
                else
                    Logger.ShowMessage("Font folder is missing. ", MessageKind.Error);
            };
            itemFontsManage.Click += delegate
            {
                if (MAIN_WINDOW != null) MAIN_WINDOW = null;
                MAIN_WINDOW = new MainWindow();
                MAIN_WINDOW.Show();
            };
            //Logs item clicked. 
            itemLogs.Click += delegate
            {
                if (System.IO.File.Exists(Application.StartupPath + "\\logs.log"))
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\logs.log");
            };
            settingsChanged();
        }
        /// <summary>
        /// Shows notification via windows desktop notification. 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="kind"></param>
        public static void Show(string msg, MessageKind kind)
        {
            if (NotifyIcon == null) return; //NotifyIcon isn't initialized yet.
            NotifyIcon.ShowBalloonTip(
                3000,
                (kind == MessageKind.Info ? "XKeyboard - Info" : (kind == MessageKind.Warning ? "XKeyboard - Warning" : "XKeyboard - Error")),
                msg,
                kind == MessageKind.Info ? System.Windows.Forms.ToolTipIcon.Info : kind == MessageKind.Warning ? System.Windows.Forms.ToolTipIcon.Warning : System.Windows.Forms.ToolTipIcon.Error);
        }
    }
}
