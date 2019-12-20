/// File: Definitions.cs
/// Purpose: Defines the objects for binding and wpf user interface. 
/// Version: 1.3
/// Date: 12/20/2019

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

using System;
using System.Windows.Media;

namespace XKeyboard.UI
{
    /// <summary>
    /// This class defines the ListViewItem used in the list of fonts in main form.
    /// </summary>
    public class XItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string FileName { get; set; }
        public DateTime DateModified { get; set; }
        /// <summary>
        /// This property is only set if the item is selected.
        /// </summary>
        public bool IsSelected { get; set; }
        public System.Windows.Visibility Visibility
        {
            get
            {
                if (IsSelected)
                    return System.Windows.Visibility.Visible;
                return System.Windows.Visibility.Hidden;
            }
        }
        public string Info
        {
            get
            {
                return "Date Modified: " +DateModified;
            }
        }
        public string Owner
        {
            get
            {
                return "Author: "+Author;
            }
        }
        public ImageSource TickSrc
        {
            get
            {
                return Properties.Resources.tick.GetImageSrc();
            }
        }
    }
}
