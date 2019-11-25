/// File: Native.cs
/// Purpose: Defines the native win32 functions used in the program to set keyboard hooks
/// Version: 1.2
/// Date Modified: 11/24/2019

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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace XKeyboard.Core
{
    /// <summary>
    /// This class defines the native methods from WinAPI which are used in this program.
    /// </summary>
    class Native
    {
        public const int WH_KEYBOARD_LL = 13;  
        public const int WM_KEYDOWN = 0x0100;

        /// <summary>
        /// Creates a lowlevel keyboard hook and returns the hook id.
        /// </summary>
        /// <returns>Hook ID</returns>
        public static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            //Get the current running process.
            using (Process curProcess = Process.GetCurrentProcess())
            //Get the current running process's MainModule. Usually the main application context.
            using (ProcessModule curModule = curProcess.MainModule)
            {
                //Call the native method declared below.
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        /// <summary>
        /// This is the native method delegate or a Native method "Signature" used to call methods from Windows API or DLLs from Windows Itself.
        /// See: Win32API
        /// </summary>
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Sets the windows hook with specific id and handler.
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        /// <summary>
        /// Unhooks (releases) the hook with hook ID or pointer.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        /// <summary>
        /// Calls the next hook in order for application to continue recieving messages.
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Gets the handle of a module specified by its name.
        /// This can be replaced with C#'s managed class.
        /// </summary>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        
    }
}
