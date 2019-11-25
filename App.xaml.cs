using System.Windows;

namespace XKeyboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Occures when the application is started up. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //Call static function to initiate the program
            Program.Initiate();
            base.OnStartup(e);
        }
    }
}
