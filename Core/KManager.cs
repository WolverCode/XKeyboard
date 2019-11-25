/// File: KManager.cs
/// Purpose: Defines the functions used to manage low level keyboard procedure and hooks. 
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
using System.Windows.Input;
using XKeyboard.Core.FontManagement;

namespace XKeyboard.Core
{
    /// <summary>
    /// This class contains code related to keyboard management (i.e. Keyboard Hooks, Enable, Disable Keyboard)
    /// </summary>
    public class KManager
    {
        public delegate void OnKeyIntercept(int keyCode, KManager km);
        /// <summary>
        /// Occures before a key is intercepted (blocked) by the program. Use this method for intercepting keys.
        /// </summary>
        public event OnKeyIntercept KeyIntercept;
        public delegate void OnKeyForward(int keyCode, KManager km);
        /// <summary>
        /// Occurs when a key is being forwarded to the windows OS. 
        /// </summary>
        public event OnKeyForward KeyForward;
        public delegate void OnKeyBlock(int keyCode, KManager km);
        /// <summary>
        /// Occurs before a key is blocked.
        /// </summary>
        public event OnKeyBlock KeyBlock;

        private Native.LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;       //Stores the keyboard hook id
        private KeyboardState _keyboardState;       //Stores the current keyboard state
        private bool iSent = false;                 //Indicates that the last key was sent by this application it self. 

        /// <summary>
        /// Gets or Sets the current keyboard state (mode)
        /// </summary>
        public KeyboardState KeyboardState
        {
            get
            {
                return _keyboardState;
            }
            set
            {
                _keyboardState = value;
            }
        }
        /// <summary>
        /// Gets or Sets whether to play the BEEP sound when input is disabled. 
        /// </summary>
        public bool Beep
        {
            get; set;
        }

        /// <summary>
        /// This function is called on every key sent by the keyboard by the hook trigger. 
        /// This method decides whether to forward or intercept the keys. 
        /// </summary>
        /// <param name="@params">The key parameters.</param>
        /// <returns></returns>
        private IntPtr InterceptKeyboard(HookCallBackParams @params)
        {
            //Determine which key is pressed and control the keyboard 
            //Decide wether to enable or disable keyboard (i.e. forward keys or intercept or block)
           if (this.KeyboardState == KeyboardState.Disabled)
            {
                if (this.KeyBlock != null)
                    this.KeyBlock.Invoke(@params.keyCode, this);
                if (Beep)
                    System.Media.SystemSounds.Beep.Play();
                return new IntPtr(-1);
            }
            //Check if any of modifier keys are pressed. 
            //Check if the keyboard is set to forward keys, or if the key pressed was not the standard key for replacement.
            if (this.KeyboardState == KeyboardState.Enabled || !IsStandardKey(@params.keyCode) || IsModifiersDown())
            {
                if (this.KeyForward != null)
                    this.KeyForward.Invoke(@params.keyCode, this);
                return Native.CallNextHookEx(_hookID, @params.nCode, @params.wParam, @params.lParam);   //Forward keys
            }
            // Debug.WriteLine("Intercepting key...");
            if (this.KeyIntercept != null)
                this.KeyIntercept.Invoke(@params.keyCode, this);
            else
                Debug.Assert(false, "Failed to intercept. Not implemented. ", "The key was intercepted however, there's no implementation to handle the keycode. (Event KeyIntercept was null or undefined)");
            //To intercept keyboard
            return new IntPtr(-1);
        }
        /// <summary>
        /// Checks whether any control keys are down. 
        /// </summary>
        /// <returns></returns>
        private bool IsModifiersDown()
        {
            var lCtrl = Keyboard.GetKeyStates(Key.LeftCtrl);
            var rCtrl = Keyboard.GetKeyStates(Key.RightCtrl);
            var rShift = Keyboard.GetKeyStates(Key.RightShift);
            var lShift = Keyboard.GetKeyStates(Key.LeftShift);
            var lAlt = Keyboard.GetKeyStates(Key.LeftAlt);
            var rAlt = Keyboard.GetKeyStates(Key.RightAlt);
            if (lCtrl == KeyStates.Down )
                return true;
            if (rCtrl == KeyStates.Toggled)
                return true;
            //Shift keys are ignored because they are checked in program.cs for character filteration. 
            //if (rShift == KeyStates.Down || rShift == KeyStates.Toggled)
            //    return true;
            //if (lShift == KeyStates.Toggled || lShift == KeyStates.Down)
            //    return true;
            if (lAlt == KeyStates.Down)
                return true;
            if (rAlt == KeyStates.Down)
                return true;
            return false;   
        }

        /// <summary>
        /// This method is called whenever a key is pressed or a keyboard hook is triggerd. 
        /// </summary>
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //Verify that the message is actually a key press 
            if (nCode >= 0 && wParam == (IntPtr)Native.WM_KEYDOWN)
            {
                if(iSent)
                {
                    return Native.CallNextHookEx(_hookID, nCode, wParam, lParam);
                }
                //Read the key code from the message. since it was called from native api, we have to marshal the type to valid C# data type.
                int keyCode = Marshal.ReadInt32(lParam);
                //Let the method handle the event, i.e. to decide whether to forward or intercept the key.
                return InterceptKeyboard(new HookCallBackParams(keyCode, nCode, lParam, wParam));
            }
            //lblEnd:
            //Forward the windows message.
            return Native.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
        /// <summary>
        /// Sends a key to system via System.Windows.Forms.SendKeys(). 
        /// </summary>
        /// <param name="xKey">The XKey to send. </param>
        public void SendKey(XKey xKey)
        {
            iSent = true;   //Flag that the last key was sent by this application itself. This is important to prevent infinite key loops.
            System.Windows.Forms.SendKeys.SendWait(xKey.TargetValue);
            iSent = false;
        }
        /// <summary>
        /// Registers the keyboard hook with low-level keyboard procedure. 
        /// </summary>
        public void Register()
        {
            try
            {
                Logger.Write("KManager.Register(): Registering hook...", MessagePriority.Low, MessageKind.Info);
                if(_proc != null || _hookID != IntPtr.Zero)
                {
                    Native.UnhookWindowsHookEx(_hookID);
                    _proc = null;
                    _hookID = IntPtr.Zero;
                }
                _proc = HookCallback;
                _hookID = Native.SetHook(_proc);
                Logger.Log("KManager.Register(): Registered keyboard hook with hook id: " + _hookID);
                Logger.Notify("The application can now intercept or forward keyboard input. ", MessageKind.Info);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message, MessagePriority.High, MessageKind.Error);
            }
        }
        /// <summary>
        /// Unregisters the keyboard hook. 
        /// </summary>
        public void Unregister()
        {
            try
            {
                Logger.Write("KManager.Unregister(): Unregistering keyboard hook using id: " + _hookID);
                Native.UnhookWindowsHookEx(_hookID);
                _proc = null;
                _hookID = IntPtr.Zero;
                Logger.Write("KManager.Unregister(): Unregistered keyboard hook. ");
            }
            catch (Exception ex)
            {
                Logger.Write("Error. " + ex.Message, MessagePriority.High, MessageKind.Error);
            }
        }
        /// <summary>
        /// Returns true if the key code is standard ASCII character code. 
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public bool IsStandardKey(int keyCode)
        {
            /// Return true if the keycode is anywhere between
            /// ALPHABETS:  65 66 67 68 . . . . 87 88 89 90
            ///             A  B  C  D  . . . . W  X  Y  Z
            /// NUMBERS:    48 49 50 51 . . . . 54 55 56 57
            ///             0  1  2  3  . . . . 6  7  8  9
            /// 
            //The key is a Alphabet key.
            if (keyCode >= 65 && keyCode <= 90)
                return true;
            //The key is a number key.
            if (keyCode >= 48 && keyCode <= 57)
                return true;
            //The key is the space key. 
             if (keyCode == 32)
                return true;
            //The key is not a standard key.
            //Debug.WriteLine("Ignored non-standard key: "+keyCode +" with ascii value: "+(char)keyCode);
            return false;
        }
    }
}
   

