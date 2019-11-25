/// File: Helper.cs
/// Purpose: Defines helper and extension functions used in the program. 
/// Version: 1.2
/// Date: 11/24/2019
/// 
/// Changes
/// =======
/// Removed function names from Log() calls as changes to Logger.cs
/// 

/* 
Copyright (c) 2019, All rights are reserved by Team WolverCode
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
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XKeyboard.Core;

namespace XKeyboard
{
    public static class Helper
    {
        /// <summary>
        /// Closes the current instance of application and starts a new one via windows command prompt. 
        /// </summary>
        public static void Restart()
        {
            Logger.Write("Restarting...", MessagePriority.Mid, MessageKind.Info);
            Logger.Log("Creating restart script...");
            var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            if (path.Contains(" "))
                path = "\"" + path + "\"";
            string cmd = $@"@echo off
title=XKeyboard - Restarting...
ping localhost -n 3 > nul
start {path}
echo Restarted XKeyboard!
ping localhost -n 1 > nul
del %0
exit";
            string tmpFile = Path.GetTempFileName()+".bat";
            File.WriteAllText(tmpFile, cmd);
            Logger.Log("Restart script created and saved as "+tmpFile);
            System.Diagnostics.Process.Start(tmpFile);
            Logger.Log("Script was invoked, shutting down application.");
            App.Current.Shutdown();
        }
        /// <summary>
        /// Adds the current executable to windows registry for startup
        /// </summary>
        public static void AddToStartup()
        {
            //Add the executable to HKEY_CURRENT_USER/SOFTWARE/MICROSOFT/WINDOWS/CURRENTVERSION/RUN
            try
            {
                Logger.Log("Opening windows registry for write access...");
                var rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                if (rk == null)
                {
                    Logger.Log("Failed to create registry entry. Unknown error.", MessagePriority.High, MessageKind.Error);
                    return;
                }
                if (rk.GetValue(ConfigKeys.StartupKey) != null)
                {
                    Logger.Log("A value with same key already exist, deleting...");
                    rk.DeleteValue(ConfigKeys.StartupKey);
                }
                Logger.Log("Creating the entry...");
                rk.SetValue(ConfigKeys.StartupKey, System.Reflection.Assembly.GetExecutingAssembly().Location);
                Logger.Log("Closing registry...");
                rk.Close();
                Logger.Log("Done. Added to startup. ");
            }
            catch (Exception ex)
            {
                Logger.Write("Error: " + ex.Message, MessagePriority.High, MessageKind.Error);
            }
        }
        /// <summary>
        /// Removes the current executable from windows registry startup
        /// </summary>
        public static void RemoveFromStartup()
        {
            //Remove an entry from: HKEY_CURRENT_USER/SOFTWARE/MICROSOFT/WINDOWS/CURRENTVERSION/RUN
            try
            {
                Logger.Log("Opening windows registry for write access...");
                var rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                if (rk == null)
                {
                    Logger.Log("The key doesn't exist, ignored...");
                    return;
                }
                if (rk.GetValue(ConfigKeys.StartupKey) != null)
                {
                    Logger.Log("Deleting registry key...");
                    rk.DeleteValue(ConfigKeys.StartupKey);
                }
                Logger.Log("Closing registry...");
                rk.Close();
                Logger.Log("Done. The application was removed from windows startup. ");
            }
            catch (Exception ex)
            {
                Logger.Write("Error: " + ex.Message, MessagePriority.High, MessageKind.Error);
            }
        }
        /// <summary>
        /// Returns true if the current executable is already added in registry startup.
        /// </summary>
        /// <returns></returns>
        public static bool IsStartup()
        {
            try
            {
                Logger.Log("Opening windows registry for read-only access...");
                var rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                if (rk == null)
                {
                    Logger.Log("Failed to open registry. Unknown error.", MessagePriority.High, MessageKind.Error);
                    return false;
                }
                if (rk.GetValue(ConfigKeys.StartupKey) != null)
                {
                    Logger.Log("A value with the key already exist, returning...");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Write("Error: " + ex.Message, MessagePriority.High, MessageKind.Error);
                return false;
            }
        }
    }
    public static class Extensions
    {
        /// <summary>
        /// Retuns the source of the Image object (for use in WPF elements)
        /// </summary>
        /// <param name="img">The image</param>
        /// <returns></returns>
        public static ImageSource GetImageSrc(this System.Drawing.Image img)
        {
            //Create new memory stream to store the object in. 
            using (var ms = new MemoryStream())
            {
                //Copy the original image to memory stream. 
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                //Create new ImageSource object from memorystream. 
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = ms;
                bi.EndInit();
                return bi;
            }
        }
        /// <summary>
        /// Returns the source of an ICON object (for use in WPF elements)
        /// </summary>
        /// <param name="ico"></param>
        /// <returns></returns>
        public static ImageSource GetImageSrc (this System.Drawing.Icon ico)
        {
            using (var ms = new MemoryStream())
            {
                ico.Save(ms);
                ms.Position = 0;
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = ms;
                bi.EndInit();
                return bi;
            }
        }
        /// <summary>
        /// Pads a string on both left and right with calculated number of characters. 
        /// </summary>
        /// <param name="str">The string to fill with characters. </param>
        /// <param name="totalLength">The total length of string. Must be greater than the length of string. </param>
        /// <param name="fillChar">The character to use. </param>
        /// <returns></returns>
        public static string Fill(this string str, int totalLength, char fillChar)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            if (totalLength < 1)
                return str;
            if (str.Length >= totalLength)
                return str;
            if (fillChar == '\0')
                return str;
            //Number of characters to add
            int charsLeft =  totalLength-str.Length;
            //If they are less than 2, increment it with one.
            if (charsLeft < 2)
                charsLeft++;
            //if they are greater then 2, make them even. 
            if (charsLeft % 2 != 0)
                charsLeft++;
            //Divide them in two
            int r = charsLeft / 2, l = charsLeft - r;
            //Add half on left
            var leftStr = str.PadLeft(str.Length + l, fillChar);
            //Add other half on right and return. 
            return leftStr.PadRight(str.Length + l + r, fillChar);
        }
    }
}
