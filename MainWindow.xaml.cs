/// File: MainWindow.cs
/// Purpose: Defines the Font Manager window
/// Version: 1.0.0.1 (beta)
/// Date: 23, Feb, '19. 

/* 
Copyright (c) 2019, All rights are reserved by AB, 70 Technologies. 
https://www.Tech70.cf
This program is licensed under the Apache License, Version 2.0 (the "License");
you may not download, install or use this program except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*/ 

using System.Windows;
using System.Windows.Controls;
using XKeyboard.Core;
using XKeyboard.Core.FontManagement;
using XKeyboard.UI;

namespace XKeyboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Set the form's ICON according to the current keyboard state. 
            switch (Program.kManager.KeyboardState)
            {
                case KeyboardState.Enabled:
                    this.Icon = Properties.Resources.x256_enabled.GetImageSrc();
                    break;
                case KeyboardState.Disabled:
                    this.Icon = Properties.Resources.x256_disabled.GetImageSrc();
                    break;
                case KeyboardState.Intercept:
                    this.Icon = Properties.Resources.x256_intercept.GetImageSrc();
                    break;
            }
            //Load available fonts in the list. 
            LoadFonts();
        }
        /// <summary>
        /// Loads all the available fonts in the listview. 
        /// </summary>
        void LoadFonts()
        {
            //Get the list of available font objects 
            var lst = Program.fManager.GetFonts();
            foreach (var f in lst)
            {
                /////NOTE/////
                    /// THE LIST BOX IS BINDED WITH THE XItem OBJECT
                ///////////////
                var i = new UI.XItem()
                {
                    Author = f.Author,
                    DateModified = f.DateModified,
                    Description = f.Description,
                    FileName = f.File(),
                    IsSelected = false,
                    Name = f.Name,
                };
                //If current applied font is same, or their file name matches (which is, if current font is not null and matches)
                //Set the font entry as selected one. 
                    if (Program.fManager?.CurrentFont != null && (Program.fManager.CurrentFont == f || Program.fManager.CurrentFont.File() == f.File()))
                        i.IsSelected = true;
                listFonts.Items.Add(i);
            }
            // 
            lst.Clear();
            lst = null;
        }
        /// <summary>
        /// Occures when a button on the form is clicked. 
        /// </summary>
        /// <param name="sender">The button which was clicked. </param>
        /// <param name="e"></param>
        private void btnFormClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                default:
                    break;
                case nameof(btnCreate):
                    CreateFont();
                    break;
                case nameof(btnDelete):
                    DeleteFont();
                    break;
                case nameof(btnEdit):
                    EditFont();
                    break;
            }
        }
        /// <summary>
        /// Opens the font editor dialog for the selected font in the list
        /// </summary>
        private void EditFont()
        {
            //Create new instance of Font Editor form. 
            var f = new frmEdit(XFont.Load(((XItem)listFonts.SelectedItem).FileName), false);
            //Store current keyboard state
            var o = Program.kManager.KeyboardState;
            if (o != KeyboardState.Enabled) //If current state is enabled, temporarily disable it. 
            {
                Logger.Notify("KManager temporarily switched the keyboard mode for the application to work correctly. ", MessageKind.Info);
                Program.kManager.KeyboardState = KeyboardState.Enabled;
                /////NOTE/////
                    /// This application cannot recieve any keyboard input as long as the keyboardmode is set to intercept!
                ///////////////
            }
            f.ShowDialog();
            //Restore keyboard state. 
            if (o != KeyboardState.Enabled)
                Logger.Notify("KManager restored the keyboard mode to " + (o == KeyboardState.Intercept ? "intercept keys." : " disabled. "), MessageKind.Info);
            Program.kManager.KeyboardState = o;
            //Reload font (if specified)
            if (Program.fManager.CurrentFont != null)
                Program.fManager.CurrentFont = XFont.Load(Program.fManager.CurrentFont.File());
        }
        /// <summary>
        /// Deletes the current selected font and removes it from the list. 
        /// </summary>
        private void DeleteFont()
        {
            if (listFonts.SelectedItems.Count < 1)
                return;
            if(MessageBox.Show("Are you sure you want to delete the selected fontset? This can't be undone!", "XKeyboard - Delete fontset", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                System.IO.File.Delete(((XItem)listFonts.SelectedItem).FileName);
                listFonts.Items.Remove(listFonts.SelectedItem);
            }
        }
        /// <summary>
        /// Opens the font editor window with an empty font set for creating new fonts. 
        /// </summary>
        private void CreateFont()
        {
            //Insantiate new font editor with standard charset. 
            var f = new frmEdit(new XFont("Un-named font set", "N/A", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz "), true);
            /* See comments in EditFont() */
            var o = Program.kManager.KeyboardState;
            if (o != KeyboardState.Enabled)
            {
                Logger.Notify("KManager temporarily switched the keyboard mode for the application to work correctly. ", MessageKind.Info);
                Program.kManager.KeyboardState = KeyboardState.Enabled;
            }
            f.ShowDialog();
            if(o!= KeyboardState.Enabled)
                Logger.Notify("KManager restored the keyboard mode to "+(o== KeyboardState.Intercept?"intercept keys.":" disabled. "), MessageKind.Info);
            Program.kManager.KeyboardState = o;
        }
    }
}
