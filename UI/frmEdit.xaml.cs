/// File: frmEdit.cs
/// Purpose: Defines the Font Editor form
/// Version: 1.2
/// Date Modified: 11/24/2019
/// 
/// Changes
/// =======
/// No changes were made to this file. 
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

using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using XKeyboard.Core;
using XKeyboard.Core.FontManagement;

namespace XKeyboard.UI
{
    /// <summary>
    /// Interaction logic for frmEdit.xaml
    /// </summary>
    public partial class frmEdit : Window
    {
        //The current font being edited. 
        public XFont xFont;
        //The form was created for new font set. 
        public bool CreateMode;
        public frmEdit()
        {
            InitializeComponent();
            //Decide which icon to use for form depends on current keyboard state. 
            switch (Program.kManager.Mode)
            {
                case KeyboardMode.Enabled:
                    this.Icon = Properties.Resources.x256_enabled.GetImageSrc();
                    break;
                case KeyboardMode.Disabled:
                    this.Icon = Properties.Resources.x256_disabled.GetImageSrc();
                    break;
                case KeyboardMode.Intercept:
                    this.Icon = Properties.Resources.x256_intercept.GetImageSrc();
                    break;
            }
            this.Icon = Properties.Resources.x256.GetImageSrc();
        }
        public frmEdit(XFont xfont, bool createMode) : this()
        {
            xFont = xfont;
            this.CreateMode = createMode;
            this.Loaded += FrmEdit_Loaded;
        }
        /// <summary>
        /// Occures when the form is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmEdit_Loaded(object sender, RoutedEventArgs e)
        {
            //Check if xFont object is initialized, if not: call Initialize. 
            if (!xFont.IsInitialized())
                xFont.InitKeys(Program.kManager, xFont.CharSet());
            //Bind the listView with Font's Keys dictionary. 
            listView.ItemsSource = xFont.Keys;  //Bind listview directly with Key sets
            txtAutofill.Text = "";
            txtAuthor.Text = xFont.Author;
            txtDescription.Text = xFont.Description;
            txtName.Text = xFont.Name;
            this.InvalidateVisual();
        }
        /// <summary>
        /// Occures when any of the form button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmButtonClick(object sender, RoutedEventArgs e)
        {
            //Check which button is pressed. 
            switch (((Button)sender).Name)
            {
                default:
                case nameof(btnAddNewKey):  //If its Add Key button. 
                    //Verify input
                    if (string.IsNullOrEmpty(txtAddKey.Text) || string.IsNullOrEmpty(txtAddKeyValue.Text))
                        return;
                    //If INPUT KEY's LENGTH is greater than 1, throw error because the input key must be a character. 
                    if (txtAddKey.Text.Length > 1)
                    {
                        Logger.Notify("Please write a single character to add in the font set. ", MessageKind.Error);
                        return;
                    }
                    //Check if the font already contains the key code
                    if (xFont.Contains(txtAddKey.Text[0]))
                        return;//return 
                    //Add a new key in the font set with custom parameters. 
                    this.xFont.Keys.Add(new XKey(0, txtAddKey.Text[0], txtAddKeyValue.Text));
                    this.RefreshList();
                    break;
                case nameof(btnRemoveSelectedKey):
                    xFont.Keys.Remove(listView.SelectedItem as XKey);
                    RefreshList();
                    break;
                case nameof(btnAutoFill):
                    //Replace all the keys of items.
                    var afText = txtAutofill.Text.Split(' ');
                    
                    for (int i = 0; i < afText.Length; i++)
                    {
                        this.xFont.Keys[i].TargetValue = afText[i].ToString();
                    }
                    this.RefreshList();
                    break;
                case nameof(btnSave):
                    Save();
                    break;
                case nameof(btnSaveAs):
                    SaveAs();
                    break;
            }
        }
        /// <summary>
        /// Saves the current xfont set being modified.
        /// </summary>
        void Save()
        {
            //If the font file doesn't exist, show save as dialog. Otherwise continue
            if (string.IsNullOrEmpty(xFont.File()))
                SaveAs();
            else
            {
                try
                {
                    //Apply font information
                    xFont.Description = txtDescription.Text;
                    xFont.Name = txtName.Text;
                    xFont.Author = txtAuthor.Text;
                    xFont.DateModified = DateTime.Now;
                    //Delete the old font file, or copy it for backup. 
                    if (System.IO.File.Exists(xFont.File()))
                        System.IO.File.Move(xFont.File(), xFont.File() + ".bak");
                    xFont.Export(xFont.File());
                    if (System.IO.File.Exists(xFont.File() + "_"))
                        System.IO.File.Delete(xFont.File() + "_");
                }
                catch (Exception ex)
                {
                    Logger.Log("Error saving font set at: "+xFont.File()+". Exception: " + ex.Message, MessagePriority.High, MessageKind.Error);
                    //Backup old file. 
                    Logger.Log("Failed to save font. ");
                    if (System.IO.File.Exists(xFont.File() + ".bak"))
                        System.IO.File.Move(xFont.File() + ".bak", xFont.File());
                }
            }
        }
        /// <summary>
        /// Prompts the user for the output file and saves the current font set.
        /// </summary>
        void SaveAs()
        {
            var sfd = new SaveFileDialog();
            sfd.Title = "XKeyboard - Export font set";
            sfd.Filter = "eXtensible Markup Language file|*.xml";
            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;
           //Show the save file dialog
            if(sfd.ShowDialog() == true)
            {
                xFont.SetFile(sfd.FileName);
                Save();
            }
        }
        /// <summary>
        /// Unbinds the listview and rebinds it with the font object to refresh keysets.
        /// </summary>
        void RefreshList()
        {
            this.listView.ItemsSource = null;
            this.listView.Items.Clear();
            this.listView.ItemsSource = xFont.Keys;
            this.listView.InvalidateVisual();
        }
    }
}
