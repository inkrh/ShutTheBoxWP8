using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Http;

namespace ShutTheBox
{
    class HS
    {

        public string filename = "scores.txt";
        public string opie;


        public Dictionary<String, String> scoreList;


        public HS()
        {
            scoreList = new Dictionary<string,string>();
            
            Debug.WriteLine("Initialized HS");
        }

        public void AddScore(String value)
        {
            LoadFile(filename);
            value = value.Trim();
            value = value.TrimEnd('\n', '\r');
            Debug.WriteLine("key : " + DateTime.Now.ToString());
            Debug.WriteLine("value : " + value);

            scoreList.Add(System.DateTime.Now.ToString(), value);
            SaveFile(filename);
        }



        public String dictToString()
        {
            return string.Join(", ", scoreList.Select(m => m.Key + "*" + m.Value).ToArray());
        }


        public void SaveFile(string SAVEFILENAME)
        {
            try
            {
                IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(SAVEFILENAME, FileMode.Create, FileAccess.Write, isoStore))
                {
                    if (isoStream != null)
                    {
                        using (StreamWriter writer = new StreamWriter(isoStream))
                        {
                            

                                writer.WriteLine(dictToString());
                                Debug.WriteLine("Written " + dictToString());

                            
                            writer.Close();
                        }

                        isoStream.Close();
                    }
                }
                isoStore.Dispose();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception : " + e.Message);
                GoogleAnalytics.EasyTracker.GetTracker().SendException("Exception On SaveFile " + e.Message, false);
            }
        }





        public void LoadFile(string SAVEFILENAME)
        {
            string temp = null;

            try
            {
                IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
                if (isoStore.FileExists(SAVEFILENAME))
                {
                    Debug.WriteLine("The file exists!");
                    using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(SAVEFILENAME, FileMode.Open, FileAccess.Read, isoStore))
                    {
                        if (isoStream != null)
                        {
                            using (StreamReader reader = new StreamReader(isoStream))
                            {
                                temp = reader.ReadToEnd();
                                Debug.WriteLine("Read " + temp);
                                reader.Close();
                            }
                            isoStream.Close();
                        }
                    }
                }
                isoStore.Dispose();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception " + e.Message);
                GoogleAnalytics.EasyTracker.GetTracker().SendException("Exception On LoadFile " + e.Message, false);

            }

            if (temp != null)
            {
                try
                {
                    string[] t2 = temp.Split(new Char[] { ',' });
                    foreach (string a in t2)
                    {
                        if (a.Contains("*"))
                        {
                            string[] t3 = a.Split(new Char[] { '*' });
                            t3[0] = t3[0].Trim();
                            t3[0] = t3[0].TrimEnd('\n', '\r');
                            t3[1] = t3[1].Trim();
                            t3[1] = t3[1].TrimEnd('\n', '\r');

                            Debug.WriteLine("Assigning Dictionary " + t3[0] + "   :    " + t3[1]);
                            scoreList.Add(t3[0], t3[1]);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception " + e.Message);
                    GoogleAnalytics.EasyTracker.GetTracker().SendException("Exception On Assigning Dictionary " + e.Message, false);
                }
            }
        }

        public void delete(String SAVEFILENAME)
        {
            try
            {
                IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
                if (isoStore.FileExists(SAVEFILENAME))
                {
                    isoStore.DeleteFile(SAVEFILENAME);
                }
            }
            catch (Exception e)
            {

                Debug.WriteLine("Delete File Exception " + e.Message);
                GoogleAnalytics.EasyTracker.GetTracker().SendException("Exception On DeleteFile " + e.Message, false);
            }


        }
    }
}


        

