using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;

namespace MyTvShowsOrganizer
{
    public static class Go2Web
    {
        public static void Go2TvCalendar()
        {
            if (!Parameters.IsInternetConnectionOk)
            {
                Parameters.InternetBadMessage();
            }
            else
            {
                OpenLink("http://www.pogdesign.co.uk/cat/");
            }
        }

        public static void GoogleIt(string whatSite, string txtboxShowsName, string plusWords = " ")
        {
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            if (!Parameters.IsInternetConnectionOk)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                Parameters.InternetBadMessage();

            }
            else
            {
                //TextBox txtbox = (TextBox)txtboxShowsName;
                string myserie = txtboxShowsName;
                myserie = CleanString(myserie);

                string myhyper = "";

                //exclusion word type to torrent metasearcher not welcome here
                if (myserie.Contains(" -"))
                {
                    myserie = myserie.Remove(myserie.IndexOf(" -"), myserie.Length - myserie.IndexOf(" -"));
                }
                myserie = myserie.Replace(" ", "+");
                plusWords = plusWords.Replace(" ", "+");

                //myhyper = "https://www.google.com/search?q=" + "automatically+download+episodes+torrents+tv+mytvshoworganizer";
               // OpenLink(myhyper);
                //Thread.Sleep(1000);
                myhyper = "https://www.google.com/search?q=" + whatSite + "+" + "%22" + myserie + "%22" + "+" + plusWords;

                OpenLink(myhyper);
            }
        }


        public static void GetMySubtitle_bsplayerSite(List<string> listOfSeries, List<string> listOfSeasons,  int languageIndex)//List<string> listOfEpisodes,
        {
            // no episode needed to opensubtitles, consider eliminate listofepisodes 

            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            if (!Parameters.IsInternetConnectionOk)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                Parameters.InternetBadMessage();
            }
            else
            {
                string mySerie = "";
               // string mySE = "";
                string hyper = null;
                string mySeason = "";
               // string myEpisode = "";
                int n = -1;
                string[] languageMatrix = new string[]
                        {
#region languages
"all",
"eng",
"afr",
"alb",
"ara",
"arm",
"baq",
"bel",
"ben",
"bos",
"bre",
"bul",
"bur",
"cat",
"chi",
"zht",
"zhe",
"hrv",
"cze",
"dan",
"dut",
"epo",
"est",
"fin",
"fre",
"glg",
"geo",
"ger",
"ell",
"heb",
"hin",
"hun",
"ice",
"ind",
"ita",
"jpn",
"kaz",
"khm",
"kor",
"lav",
"lit",
"ltz",
"mac",
"may",
"mal",
"mni",
"mon",
"mne",
"nor",
"oci",
"per",
"pol",
"por",
"pob",
"rum",
"rus",
"scc",
"sin",
"slo",
"slv",
"spa",
"swa",
"swe",
"syr",
"tgl",
"tam",
"tel",
"tha",
"tur",
"ukr",
"urd",
"vie",
 };
#endregion

                foreach (string myseriechecked in listOfSeries)
                {
                    n += 1;
                    mySerie = myseriechecked;

                    //google exclusion word type on metasearcher not welcome here. Consider disregard this in future.
                    mySerie = CleanString(mySerie);
                    if (mySerie.Contains(" -"))
                    {
                        mySerie = mySerie.Remove(mySerie.IndexOf(" -"), mySerie.Length - mySerie.IndexOf(" -"));
                    }
                    mySeason = listOfSeasons[n];
                    //myEpisode = listOfEpisodes[n];

                    
                    mySeason = mySeason.PadLeft(2, '0');

                   // myEpisode = (Convert.ToInt16(myEpisode) - 1).ToString(); //last episode.

                   // myEpisode = myEpisode.PadLeft(2, '0');
                   // mySE = "S" + mySeason + "E" + myEpisode;
                    mySerie = mySerie.Replace(" ", "+");


                    //http://bsplayer-subtitles.com/index.php?cmd=search&p=exploresub&q=the+big+bang+theory+s08e02&lang=ALL
                    //http://www.opensubtitles.org/en/search/sublanguageid-eng/searchonlytvseries-on/season-2/episode-3/moviename-the+last+ship
                  // http://www.opensubtitles.org/en/search/sublanguageid-eng/searchonlytvseries-on/season-2/moviename-penny+dreadful/sort-5/asc-0
                    //hyper = "http://www.subtitles4free.net/search-subtitles-" + mySerie + "+" + mySE + "-0-" + languageMatrix[languageIndex] + "-all-1.htm";
                    //hyper = "http://bsplayer-subtitles.com/index.php?cmd=search&p=exploresub&q=" + mySerie + "+" + mySE + "&lang=" + languageMatrix[languageIndex];
                    hyper = "http://www.opensubtitles.org/en/search/sublanguageid-" + languageMatrix[languageIndex].ToLower() + "/searchonlytvseries-on" + "/season-" + Convert.ToInt16(mySeason).ToString() + "/moviename-" + mySerie + "/sort-5/asc-0";

                    OpenLink(hyper);
                    Thread.Sleep(2000);
                }
            }
        }

        private static string CleanString(string name)
        {
            //remove invalid caracteres from series name {/ -}
            name = String.Join("", name.Split(Path.GetInvalidFileNameChars()));
            return name;
        }

        public static void OpenLink(string link)
        {
            try
            {
                System.Diagnostics.Process.Start(link);
            }
            catch (Exception)
            {
                MsgBox.Show("The application can not open the Link: " + link + ". Make sure you have a Default Program Defined to Open this kind of Link and Try again.", "No default Program! Impossible open Link!", MsgBox.Buttons.OK, MsgBox.Icone.Error, MsgBox.AnimateStyle.SlideDown);
                //throw;
            }
        }
    }
}
