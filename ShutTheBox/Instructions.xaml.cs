using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ShutTheBox
{
    public partial class Instructions : PhoneApplicationPage
    {
        public Instructions()
        {
            InitializeComponent();
            GoogleAnalytics.EasyTracker.GetTracker().SendView("Instructions");
        }
    }
}