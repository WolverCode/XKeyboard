/// File: MessageBoxEx.xaml.cs
/// Purpose: Defines the MessageBoxEx UI handler. 
/// Version: 1.3
/// Date Modified: 12/20/2019
/// 

/* 
Copyright (c) 2019, All rights are reserved by WolverCode
https://www.wolvercode.com

This program is licensed under the Apache License, Version 2.0 (the "License");
you may not download, install or use this program except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*/

using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using XKeyboard.Core;

namespace XKeyboard.UI.Metro
{
    /// <summary>
    /// Defines a modern (metro) styled message box with advanced features.
    /// </summary>
    public partial class MessageBoxEx : Window
    {
        public MessageBoxExOptions OPTIONS;
        private DialogResult Result;
        private MessageBoxButtons btns;
        public MessageBoxEx()
        {
            InitializeComponent();
            
        }
        public MessageBoxEx(IWin32Window parent, string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon, DefaultButton defaultButton, System.Windows.MessageBoxOptions flags=System.Windows.MessageBoxOptions.None, MessageBoxExOptions extraOptions=null) : this()
        {
            //Set up form
            lblTitle.Content = title;
            txtDescription.Text = message;
            txtDescription.TextTrimming = TextTrimming.CharacterEllipsis;
            //Set the buttons according to the buttons parameter.
            this.btns = buttons;
            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    //Only OK button should be visible.
                    HideButtons(btnAbort, btnIgnoreCancel, btnNo, btnRetry);
                    ShowButtons(btnYesOk, btnCancel);
                    btnYesOk.Content = "OK";
                    break;
                case MessageBoxButtons.OKCancel:
                    //Only OK and Cancel button should be visible
                    ShowButtons(btnIgnoreCancel, btnYesOk, btnCancel);
                    HideButtons(btnAbort, btnNo, btnRetry);
                    btnIgnoreCancel.Content = "CANCEL";
                    btnYesOk.Content = "OK";
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    //Only Abort Retry and Ignore buttons should be visible. Not even X button
                    ShowButtons(btnRetry, btnIgnoreCancel, btnAbort);
                    HideButtons(btnCancel, btnNo, btnYesOk);
                    btnIgnoreCancel.Content = "IGNORE";
                    break;
                case MessageBoxButtons.YesNoCancel:
                    //Only Yes, No and Cancel (plus x) buttons should be visible
                    ShowButtons(btnYesOk, btnNo, btnCancel);
                    HideButtons(btnAbort, btnIgnoreCancel, btnRetry);
                    btnYesOk.Content = "YES";
                    break;
                case MessageBoxButtons.YesNo:
                    //Only Yes and No buttons should be visible. (not even x button)
                    ShowButtons(btnYesOk, btnNo);
                    HideButtons(btnRetry, btnIgnoreCancel, btnCancel, btnAbort);
                    btnYesOk.Content = "YES";
                    break;
                case MessageBoxButtons.RetryCancel:
                    //Only retry or cancel buttons should be visible. (not even x)
                    ShowButtons(btnRetry, btnIgnoreCancel);
                    HideButtons(btnAbort, btnCancel, btnNo, btnYesOk);
                    btnIgnoreCancel.Content = "RETRY";
                    break;
                default:
                    ShowButtons(btnYesOk, btnCancel);
                    HideButtons(btnRetry, btnNo, btnIgnoreCancel, btnAbort);
                    btnYesOk.Content = "OK";
                    break;
            }
            //Set default button
            switch (defaultButton)
            {
                case DefaultButton.OK:
                case DefaultButton.Yes:
                    SelectButton(btnYesOk);
                    break;
                case DefaultButton.No:
                    SelectButton(btnNo);
                    break;
                case DefaultButton.Ignore:
                case DefaultButton.Cancel:
                    SelectButton(btnIgnoreCancel);
                    break;
                case DefaultButton.Abort:
                    SelectButton(btnAbort);
                    break;
                case DefaultButton.Retry:
                    SelectButton(btnRetry);
                    break;
                default:
                    SelectButton(btnYesOk);
                    break;
            }
            //Set up ICON
            switch (icon)
            {
                default:
                case MessageBoxIcon.None:
                    break;
                case MessageBoxIcon.Error:
                    this.Background = new SolidColorBrush(Colors.Red);
                    txtDescription.Foreground = new SolidColorBrush(Colors.White);
                    break;
                case MessageBoxIcon.Warning:
                    this.Background = new SolidColorBrush(Colors.Gold);
                    txtDescription.Foreground = new SolidColorBrush(Colors.Black);
                    break;
                case MessageBoxIcon.Information:
                    this.Background = new SolidColorBrush(Colors.Green);
                    txtDescription.Foreground = new SolidColorBrush(Colors.White);
                    break;
            }
            /////TO-DO/////
            /// Extract icons from windows shell.dll or imageres.dll
            ///////////////
            //Set up extended params (background, colors, placement, etc..)
            if (extraOptions != null)
                extraOptions.MessageBox = this;
        }
        /// <summary>
        /// Hides all the buttons passed. 
        /// </summary>
        /// <param name="buttons"></param>
        private void HideButtons(params System.Windows.Controls.Button[] buttons)
        {
            foreach (var btn in buttons)
            {
                btn.Visibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// Shows all the buttons passed. 
        /// </summary>
        /// <param name="buttons"></param>
        private void ShowButtons(params System.Windows.Controls.Button[] buttons)
        {
            foreach (var btn in buttons)
            {
                btn.Visibility = Visibility.Visible;
            }
        }
        private void SelectButton(System.Windows.Controls.Button button)
        {
            button.Focus();
        }
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            switch (((System.Windows.Controls.Button)sender).Name)
            {
                default:
                    break;
                case nameof(btnAbort):
                    this.Result = System.Windows.Forms.DialogResult.Abort;
                    break;
                case nameof(btnCancel):
                    this.Result = System.Windows.Forms.DialogResult.Cancel;
                    break;
                case nameof(btnIgnoreCancel):
                    this.Result = (btns == MessageBoxButtons.AbortRetryIgnore) ? System.Windows.Forms.DialogResult.Ignore : System.Windows.Forms.DialogResult.Cancel;
                    break;
                case nameof(btnNo):
                    this.Result = System.Windows.Forms.DialogResult.No;
                    break;
                case nameof(btnRetry):
                    this.Result = System.Windows.Forms.DialogResult.Retry;
                    break;
                case nameof(btnYesOk):
                    this.Result = (btns == MessageBoxButtons.YesNo || btns == MessageBoxButtons.YesNoCancel) ? System.Windows.Forms.DialogResult.Yes : System.Windows.Forms.DialogResult.OK; 
                    break;
            }
            this.DialogResult = true;
            this.Close();
        }
        public static DialogResult Show(IWin32Window owner, string msg, MessageKind kind, MessageBoxExOptions options = null)
        {
            var mbx = new MessageBoxEx(owner, msg, 
                (kind == MessageKind.Error)?"XKeyboard - Error":(kind == MessageKind.Info)?"XKeyboard - Info": "XKeyboard - Warning", 
                MessageBoxButtons.OK, 
                (kind == MessageKind.Info)? MessageBoxIcon.Information : (kind == MessageKind.Warning) ? MessageBoxIcon.Warning : MessageBoxIcon.Error, DefaultButton.OK);
            //mbo.Assign(mbx);    //Assign the msgboxex to its options, which handles the sound and and UI 
            if (options != null)
                options.Assign(mbx);
            //Set up mbx parameters and msg box settings according to the parameters passed to this function.
            if (mbx.ShowDialog() == true)   //True only if the form was handled succesfully.
                return mbx.Result;
            return System.Windows.Forms.DialogResult.None;
        }
    }
}
