using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;


namespace ShutTheBox
{
    public partial class HighScores : PhoneApplicationPage
    {
        private HS scores;

        public HighScores()
        {
            scores = new HS();
            InitializeComponent();
            GoogleAnalytics.EasyTracker.GetTracker().SendView("Scores");
            listToDic();
        }

        private void listToDic()
        {
            Debug.WriteLine("Setting List to Values");
            scores.LoadFile(scores.filename);
            if (scores.scoreList.Values != null)
            {

                lB.ItemsSource = scores.scoreList.Values;
            }

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Reset Scores", "userclick", null, 0);

            scores.delete(scores.filename);
            scores.scoreList = null;
            lB.ItemsSource = null;
            scores = null;
            scores = new HS();
        }
    }
}