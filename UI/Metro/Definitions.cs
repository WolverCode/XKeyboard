/// File: Metro.Definitions.cs
/// Purpose: Defines the metro message box parameters. 
/// Version: 1.2 
/// Date: 23, Feb, 2019

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

using System.Windows.Media;

namespace XKeyboard.UI.Metro
{
    /// <summary>
    /// MessageBox's Default Buttons
    /// </summary>
    public enum DefaultButton
    {
        OK,
        Yes,
        No,
        Ignore,
        Cancel,
        Abort,
        Retry
    }
    /// <summary>
    /// Class defines the extra (or advanced) features of MessageBoxEx
    /// </summary>
    public class MessageBoxExOptions
    {
        #region PRIVATE
        MessageBoxEx _mbox;
        SolidColorBrush _background;
        SolidColorBrush _foreground_title;
        SolidColorBrush _foreground_message;
        SolidColorBrush _foreground_buttons;
        SolidColorBrush _background_buttons;
        bool _topmost;
        bool _resizable;
        bool _flash;
        System.Media.SystemSound _sound;
        #endregion
        #region PUBLIC
        public SolidColorBrush Background { get { return _background; } set { _background = value; } }
        public SolidColorBrush TitleForeground { get { return _foreground_title; } set { _foreground_title = value; } }
        public SolidColorBrush MessageForeground { get { return _foreground_message; } set { _foreground_message = value; } }
        public SolidColorBrush ButtonsForeground { get { return _foreground_buttons; } set { _foreground_buttons = value; } }
        public SolidColorBrush ButtonsBackground { get { return _background_buttons; } set { _background_buttons = value; } }
        public bool TopMost { get { return _topmost; } set { _topmost = value; } }
        public bool Resizable { get { return _resizable; } set { _resizable = value; } }
        public System.Media.SystemSound Sound { get { return _sound; } set { _sound = value; } }
        public bool Flash { get { return _flash; } set { _flash = value; } }
        public MessageBoxEx MessageBox { get { return _mbox; } set { _mbox = value; Assign(value); } }
        #endregion
        public MessageBoxExOptions()
        {
        }
        /// <summary>
        /// Assigns all the setted parameters to a message box.
        /// </summary>
        /// <param name="mbox">The messagebox to apply the parameters to</param>
        public void Assign(MessageBoxEx mbox)
        {
            //Associate and Assign properties to MessageBox
            if (mbox == null)
                throw new System.ArgumentNullException("mbox");
            mbox.OPTIONS = this;
            //Set the TOP-MOST property.
            mbox.Topmost = this.TopMost;
            //Apply colors
            mbox.Background = this.Background;
            mbox.Foreground = this.TitleForeground;
            mbox.lblTitle.Foreground = this.TitleForeground;
            mbox.txtDescription.Foreground = this.MessageForeground;
            mbox.btnAbort.Background = this.ButtonsBackground;
            mbox.btnIgnoreCancel.Background = this.ButtonsBackground;
            mbox.btnNo.Background = this.ButtonsBackground;
            mbox.btnRetry.Background = this.ButtonsBackground;
            mbox.btnYesOk.Background = this.ButtonsBackground;
            mbox.ResizeMode = (this.Resizable) ? System.Windows.ResizeMode.CanResizeWithGrip : System.Windows.ResizeMode.NoResize;
            //Play sound on message box. if specified.
            mbox.Loaded += (x, xx) =>
            {
                if (this.Sound != null)
                    this.Sound.Play();
                if (Flash)
                {
                    /////TO-DO/////
                        /// Implement the code to flash the window.
                    ///////////////
                }
            };
        }
    }
}
