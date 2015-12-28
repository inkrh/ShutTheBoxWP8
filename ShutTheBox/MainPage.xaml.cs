using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ShutTheBox.Resources;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Advertising.Mobile.UI;
using System.Windows.Threading;
using System.IO;
using System.Threading.Tasks;

namespace ShutTheBox
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Dictionary<int, bool> boxes = new Dictionary<int, bool>();
        private Boolean hasrolled;
        // private int btn;
        private int dieTotal;
        private int?[] last = new int?[2];
        DispatcherTimer newTimer = new DispatcherTimer();

        private HS saveScore;

        private Dictionary<int, string> facts = new Dictionary<int, string>();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            GetNumbers();
            SetBoard();



        }

        private void GetNumbers()
        {
            getFacts();

        }

        private async void getFacts()
        {
            for (int i = 5; i < 51; i++)
            {


                string number = Convert.ToString(i);
                string url = "http://numbersapi.com/" + number + "/trivia?fragment";

                var response = await httpRequest(url);
                System.Threading.Thread.Sleep(55);
                if (response != null)
                {

                    Debug.WriteLine(number + " - " + response.ToString());
                    if (!facts.Keys.Contains(Convert.ToInt32(number)))
                    {
                        facts.Add(Convert.ToInt32(number), response.ToString());
                    }
                }
            }
        }



        public async Task<string> httpRequest(string url)
        {
            string received = null;

            Uri uri = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);


            try
            {
                using (var response = (HttpWebResponse)(await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null)))
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(responseStream))
                        {

                            received = await sr.ReadToEndAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception getting response : " + e.Message);
                GoogleAnalytics.EasyTracker.GetTracker().SendException("Exception getting response : " + e.Message, false);
            }

            return received;
        }





        private void SetBoard()
        {

            dieTotal = 0;
            hasrolled = false;

            Die1.Visibility = Visibility.Visible;
            Die2.Visibility = Visibility.Visible;
            Reset.Visibility = Visibility.Collapsed;
            GoogleAnalytics.EasyTracker.GetTracker().SendView("Game Start");

            total.Text = "Roll To Start";

            for (int i = 1; i <= 9; i++)
            {
                boxes.Add(i, true);
            }
        }

        private void FlapClick(object sender, RoutedEventArgs e)
        {


            if (!hasrolled)
            {
                return;
            }

            if (dieTotal > 0 && OwtLeft())
            {
                int which = Convert.ToInt32(((Button)e.OriginalSource).Content);
                if (boxes[which])
                {
                    if (dieTotal - which >= 0)
                    {
                        boxes[which] = false;
                        ((Button)e.OriginalSource).Opacity = 0;
                        dieTotal = dieTotal - which;
                        if (dieTotal == 0)
                        {
                            if (!OwtLeft())
                            {
                                GameEnd();

                                return;
                            }

                            hasrolled = false;
                            total.Text = "Roll Again";
                            Die1.Content = "Roll";
                            Die2.Content = "Roll";
                            CurrentScore.Text = "Current Score: " + Score();
                        }
                        else
                        {
                            CurrentScore.Text = "Current Score: " + Score();
                            total.Text = "To Use: " + dieTotal.ToString();
                            if (!OwtLeft())
                            {
                                GameEnd();

                                return;
                            }

                            if (NoMoreOptions(dieTotal))
                            {
                                GameEnd();
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void RollMe(object sender, RoutedEventArgs e)
        {
            if (!OwtLeft())
            {
                GameEnd();
                return;
            }

            if (hasrolled)
            {
                if (dieTotal > 0)
                {
                    return;
                }
            }

            int d1 = 0;
            int d2 = 0;

            hasrolled = true;

            Random rand = new Random();
            d1 = rand.Next(1, 7);
            d2 = rand.Next(1, 7);
            dieTotal = d1 + d2;

            Die1.Content = d1.ToString();
            Die2.Content = d2.ToString();
            total.Text = "To Use: " + dieTotal.ToString();

            //if (last[0] > 0 && last[1] > 0)
            //{
            //    lastRoll.Text = "Previous Roll = " + last[0].ToString() + " & " + last[1].ToString();
            //}

            last[0] = d1;
            last[1] = d2;
            CurrentScore.Text = "Current Score: " + Score();

            if (NoMoreOptions(dieTotal))
            {
                GameEnd();
                return;
            }
        }

        //private void updateDice(Button Die, int d2)
        //{
        //    Die.Background = withButter[d2];
        //}

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            newTimer.Stop();

            dieTotal = 0;
            hasrolled = false;
            Die1.Visibility = Visibility.Visible;
            Die2.Visibility = Visibility.Visible;
            Reset.Visibility = Visibility.Collapsed;
            //lastRoll.Text = "";
            //lastRoll.Visibility = Visibility.Visible;

            total.Text = "Roll To Start";
            //lastRoll.Text = string.Empty;
            CurrentScore.Text = string.Empty;
            Die1.Content = "Roll";
            Die2.Content = "Roll";

            for (int i = 1; i <= 9; i++)
            {
                boxes[i] = true;
            }

            _1.Opacity = 1;
            _2.Opacity = 1;
            _3.Opacity = 1;
            _4.Opacity = 1;
            _5.Opacity = 1;
            _6.Opacity = 1;
            _7.Opacity = 1;
            _8.Opacity = 1;
            _9.Opacity = 1;

            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Reset Board", "userclick", null, 0);


        }

        private void GameEnd()
        {
            newTimer = new DispatcherTimer();
            newTimer.Interval = TimeSpan.FromMilliseconds(250);
            newTimer.Tick += OnTimerTick;
            newTimer.Start();

            saveScore = new HS();
            saveScore.AddScore(Score());
            //saveScore.SaveFile(saveScore.filename);

            total.Text = "Remaining: " + dieTotal.ToString();

            CurrentScore.Text = "No Moves Left. Score: " + Score();


            //lastRoll.Visibility = Visibility.Collapsed;
            last[0] = null;
            last[1] = null;
            Die1.Visibility = Visibility.Collapsed;
            Die2.Visibility = Visibility.Collapsed;
            Reset.Visibility = Visibility.Visible;
            // track a custom event
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Game Ended", "Game Over : " + Score(), null, 0);


        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            int rn = new Random().Next(0, 14);
            switch (rn)
            {
                case 1:
                    _1.Opacity = new Random().NextDouble();
                    break;
                case 2:
                    _2.Opacity = new Random().NextDouble();
                    break;
                case 3:
                    _3.Opacity = new Random().NextDouble();
                    break;
                case 4:
                    _4.Opacity = new Random().NextDouble();
                    break;
                case 5:
                    _5.Opacity = new Random().NextDouble();
                    break;
                case 6:
                    _6.Opacity = new Random().NextDouble();
                    break;
                case 7:
                    _7.Opacity = new Random().NextDouble();
                    break;
                case 8:
                    _8.Opacity = new Random().NextDouble();
                    break;
                case 9:
                    _9.Opacity = new Random().NextDouble();
                    break;
                default:
                    break;
            }
        }


        private bool OwtLeft()
        {
            if (!boxes.ContainsValue(true))
            {
                return false;
            }

            return true;
        }

        private bool NoMoreOptions(int rollValue)
        {
            int ble = 0;
            if (rollValue == 1)
            {
                if (boxes[1])
                {
                    return false;
                }
            }

            if (dieTotal > 9)
            {
                rollValue = 9;
            }

            for (int i = 1; i <= rollValue; i++)
            {
                if (boxes[i])
                {
                    ble = ble + 1;
                }
            }

            if (ble > 0)
            {
                return false;
            }

            return true;
        }

        private string Score()
        {


            string outpu = string.Empty;

            foreach (KeyValuePair<int, bool> pair in boxes)
            {
                if (pair.Value)
                {
                    outpu = outpu + pair.Key.ToString();
                }
            }

            if (outpu == String.Empty)
            {
                outpu = "0";
            }

            int t = Convert.ToInt32(outpu) + dieTotal;

            if (t == 0)
            {
                return "0 - Perfect Game";

            }

            if (t == 42)
            {
                Random fourtwo = new Random();
                if (fourtwo.Next(1, 44) == 42)
                {
                    return "42 - H2G2";
                }
            }

            if (t < 6)
            {
                return Convert.ToString(t) + " - So Close";
            }

            if (facts.ContainsKey(t))
            {
                return Convert.ToString(t) + " - " + facts[t];
            }

            outpu = Convert.ToString(t);

            return outpu;
        }



        private void Instructions_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Instructions.xaml", UriKind.Relative));

        }

        private void About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void Scores_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/HighScores.xaml", UriKind.Relative));
        }

    }
}