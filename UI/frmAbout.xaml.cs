/// File: frmAbout.cs
/// Purpose: Defines the about form
/// Version: 1.2
/// Date Modified: 11/24/2019
/// 
/// Changes
/// =======
/// Changed version info.
/// Added links
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
using System.Windows;
using XKeyboard.Core;

namespace XKeyboard.UI
{
    /// <summary>
    /// Interaction logic for frmAbout.xaml
    /// </summary>
    public partial class frmAbout : Window
    {
        #region FORM TEMPLATE
        #region MVF
        //Indicates that the mouse button is pressed.
        public const int WM_NCLBUTTONDOWN = 0xA1;
        //Pointer to the Caption of the window.
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private void menu_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                ReleaseCapture();
                SendMessage(new System.Windows.Interop.WindowInteropHelper(this).Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion
        new public object Title { get { return lblTitle.Content; } set { lblTitle.Content = value; UpdateLayout(); } }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion
        public frmAbout()
        {
            InitializeComponent();
            imgWC.Source = Properties.Resources.LOGO2_W.GetImageSrc();
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
            lblVersion.Content = "Version: " + System.Windows.Forms.Application.ProductVersion + "\r\nDate Release: 25/11/2019";
            lblCopyright.Text = @"THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

FOR MORE TOOLS AND APPS, VISIT OUR WEBSITE OR CHECK OUT OUR FACEBOOK PAGE!
EMAIL: support@wolvercode.com
";
            imgFB.Source = Properties.Resources.like.GetImageSrc();
            imgWWW.Source = Properties.Resources.mozilla.GetImageSrc();
            //this.Background = new System.Windows.Media.ImageBrush(Properties.Resources.LOGO.GetImageSrc());
            switch (Program.kManager.KeyboardState)
            {
                case Core.KeyboardState.Enabled:
                    imgProg.Source = Properties.Resources.xk256_enabled.GetImageSrc();
                    break;
                case Core.KeyboardState.Disabled:
                    imgProg.Source = Properties.Resources.xk256_disabled.GetImageSrc();
                    break;
                case Core.KeyboardState.Intercept:
                    imgProg.Source = Properties.Resources.xk256_intercepting.GetImageSrc();
                    break;
            }
        }
    }
}
