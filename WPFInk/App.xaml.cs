using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace WPFInk
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Application currApp = Application.Current;
            //currApp.StartupUri = new Uri("VideoOperation.xaml", UriKind.RelativeOrAbsolute);
            currApp.StartupUri = new Uri("VideoSummarizationControl.xaml", UriKind.RelativeOrAbsolute);
        }
    }
}
