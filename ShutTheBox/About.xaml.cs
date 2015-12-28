using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace ShutTheBox
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            GoogleAnalytics.EasyTracker.GetTracker().SendView("About");
        }
        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {



            string memory = Microsoft.Phone.Info.DeviceStatus.ApplicationCurrentMemoryUsage.ToString();
            string memlimit = Microsoft.Phone.Info.DeviceStatus.ApplicationMemoryUsageLimit.ToString();
            string model = Microsoft.Phone.Info.DeviceStatus.DeviceManufacturer.ToString() + " " + Microsoft.Phone.Info.DeviceStatus.DeviceName.ToString();
            string about = model + "\n Available memory " + memlimit + "\n Used memory " + memory;
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Support Email", "Click", model, 0);
            try
            {

                EmailComposeTask emailComposeTask = new EmailComposeTask();
                emailComposeTask.Subject = "Shut The Box WP8 v1.6 Support";
                emailComposeTask.Body = about; ;
                emailComposeTask.To = "robert@inkrh.com";
                emailComposeTask.Show();

            }
            catch (Exception eme)
            {
                
                GoogleAnalytics.EasyTracker.GetTracker().SendException("Exception on EmailComposeTask " +eme.Message, false);
            }
        }
    }
}