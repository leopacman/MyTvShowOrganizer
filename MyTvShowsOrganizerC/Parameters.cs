using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System;
using System.Text.RegularExpressions;
//ControllerContext 
//ControllerContext Members
//using System.Web.Mvc;
//using System.Web;

namespace MyTvShowsOrganizer
{
    public static class Parameters
    {
        //constants
        internal const int nOfColumns = 8;
        internal const int nTotTboxes = 160;
        internal const int nOfRows = 20;
        internal const int showsColumn = 1;
        internal const int seasonColumn = 2;
        internal const int episodeColumn = 3;
        internal const int resolutionColumn = 4;
        internal const int plusColumn = 5;
        internal const int lastDownloadColumn = 6;
        internal const int nextEpisodeColumn = 7;
        internal const int statusColumn = 8;

        internal static bool isDebugging = false;
        internal static bool showSplash = true;

        //internal static bool isDebugging = true;
        //internal static bool showSplash = false;

        internal static bool bingTranslatorOn = true;

        internal const int pauseBetweenWebSitesRequisitions = 3000; //milliseconds. This program is not a DDos Attack
        //general properties
        internal static string myProgPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "MyTvShowsOrganizer");
        internal static bool firstRun = false;

        internal static bool isGetTorrentStoped = false;  //is used to stop 'get all pages' function.
        internal static int getAllIniPage;
        internal static int getAllCurrentPage;
        // internal static bool isGetAllChecked = false;

        internal static string listBoxNewShowPreviousUrl = "";
        internal static string blankWebPage = @"<!DOCTYPE HTML><body style=""background:#202020""></body></html>";
        //https://lh4.googleusercontent.com/L_PWrf6-DgCJ3NL8OZvw1QJqGPGa6lHNMkNQ4sTot3ZnG4KjvRhKLMEqS8QJia3aotcnbsIGiZNLhXQ=w1896-h835

        internal static int sameDayTdPosition;

        internal static int AddNewShowSizeTabControlSize;
        internal static int AddNewShowWebBrowserSize;
        internal static int AddNewShowWebBrowserPosition;


        internal static void FadeOut(Form form)
        {
            float x = 1;
            while (x > 0)
            {
                x -= (float)0.1;
                form.Opacity = x;
                form.Refresh();
                System.Threading.Thread.Sleep(10);
            }
        }

        internal static void FadeIn(Form form)
        {
            float x = 0;
            while (x < 1)
            {
                x += (float)0.1;
                form.Opacity = x;
                form.Refresh();
                System.Threading.Thread.Sleep(15);
            }
        }

        public static void CloseIex()
        {
            string savedFile = Path.Combine(Parameters.myProgPath, "IeId.inf");
            if (File.Exists(savedFile))
            {
                string IeId = File.ReadAllText(savedFile);
                try
                {
                    foreach (string id in IeId.Split(','))
                    {
                        int IeIdint = Convert.ToInt32(id);
                        Process.GetProcessById(IeIdint).Kill();
                    }
                }
                catch
                {
                }
            }
        }
            

        // seriesTboxIndex = (Convert.ToInt16(Cbox.Text) - 1) * Parameters.nOfColumns + 1;

        internal static int TboxIndex2RowIndex(int tBoxIndex)
        {
            return (tBoxIndex - 1) / nOfColumns + 1;
        }

        internal static int RowIndex2TboxIndex(int rowIndex)
        {
            return (rowIndex - 1) * nOfColumns + 1;
        }

        internal static string ClearStr(string myString)
        {
            myString = string.Join(" ", myString.Split(Path.GetInvalidFileNameChars()));
            //will clear \/|<>"*:?
            myString = myString.Replace("'", "");
            string exclusionPatter = @"[&()!:,\[\]]";  //replace anomalous chars.
            myString = Regex.Replace(myString, exclusionPatter, " ");
            exclusionPatter = @"(?<=\w)-(?=\w)"; // replace - (minus signal) but not " -" (space+minus signal)(used for exclusion word for torrent search)
            myString = Regex.Replace(myString, exclusionPatter, " ");
            myString = myString.Replace("  ", " ");
            // exclusionPatter = @"(?<=\w)-[\w]$"; // replace - (minus signal) if it is followed for one final alfanum (hawaii five-0)
            // myString = Regex.Replace(myString, exclusionPatter, " ").Trim();
            return myString.Trim();
        }

        private static int _MaxNumberOfPages = 10;
        internal static int MaxNumberOfPages
        {
            get
            {
                return _MaxNumberOfPages;
            }
            set
            {
                _MaxNumberOfPages = value;
            }
        }

        private static bool _IsInternetConnectionOk = true;
        internal static bool IsInternetConnectionOk
        {
            get
            {
                return _IsInternetConnectionOk;
            }
            set
            {
                _IsInternetConnectionOk = value;
            }
        }

        internal static bool IsInternetConnectionUp()
        {
            try
            {
                System.Net.WebClient client = new System.Net.WebClient();
                Stream stream = client.OpenRead("http://www.google.com");
                stream.Dispose();
                client.Dispose();
                return true;
            }
            catch
            {
                MsgBox.Show("Internet Connection Seems Unresponsive. Internet functions will not work without it. After fixing the problem, you must restart this program.", "Internet Connection Problem.", MsgBox.Buttons.OK, MsgBox.Icone.Warning, MsgBox.AnimateStyle.SlideDown);
            }
            return false;
        }

        internal static void InternetBadMessage() //if user continue in despite of internetconnectionok=false, this message will show up to remember it.
        {
            MsgBox.Show("Internet Connection or Specific WebSite is off-line.", "Some functions may not work.", MsgBox.Buttons.OK, MsgBox.Icone.Warning, MsgBox.AnimateStyle.SlideDown);
        }

        private static string _CodHtmlNextAiringStr = "pogdesign";
        internal static string CodHtmlNextAiringStr
        {
            get
            {
                return _CodHtmlNextAiringStr;
            }
            set
            {
                _CodHtmlNextAiringStr = value;
            }
        }

        internal static string CodHtmlNextAiringStringExtractor()
        {
            try
            {
                System.Net.WebClient ccc = new System.Net.WebClient();
                string codhtml = ccc.DownloadString("http://www.pogdesign.co.uk/cat/next-airing");
                ccc.Dispose();
                return codhtml;
            }
            catch
            {
            }

            return "#off-line#";
        }

        private static string _CodHtmlTvCalendarStr = "pogdesign";
        internal static string CodHtmlTvCalendarStr
        {
            get
            {
                return _CodHtmlTvCalendarStr;
            }
            set
            {
                _CodHtmlTvCalendarStr = value;
            }
        }

        internal static string CodHtmlTvCalendarStringExtractor()
        {
            try
            {
                System.Net.WebClient ccc = new System.Net.WebClient();
                string codhtml = ccc.DownloadString("http://www.pogdesign.co.uk/cat/");
                ccc.Dispose();
                return codhtml;
            }
            catch
            {
            }

            return "#off-line#";
        }

        //---------------

        internal static int listBoxNewShowNamesNofItems = 1;
        internal static string TboxNewShowsAdded;
        internal static string NewShows_CurrentGenreText = "All";
        internal static bool NewShows_Filtering = false;
        internal static ToolStripMenuItem NewShows_CurrentGenreItem = new ToolStripMenuItem();
        private static Random rnd = new Random();
        internal static bool areNewShowDisposableControlDisposed = true;
        //internal static string AddNewShowCountDownDate = "";

        // internal static int _NewShows_RandomIndex = rnd.Next(1, 30);
        internal static int NewShows_RandomIndex(int maxValue)
        {
            //_NewShows_RandomIndex = rnd.Next(0, maxValue);
            return rnd.Next(0, maxValue); //_NewShows_RandomIndex;
        }

        private static string _CodHtmlShowListStr = "pogdesign";
        internal static string CodHtmlShowListStr
        {
            get
            {
                return _CodHtmlShowListStr;
            }
            set
            {
                _CodHtmlShowListStr = value;
            }
        }

        internal static string CodHtmlShowListExtractor()
        {
            try
            {
                System.Net.WebClient ccc = new System.Net.WebClient();
                string codhtml = ccc.DownloadString("http://www.pogdesign.co.uk/cat/showselect.php");
                ccc.Dispose();
                return codhtml;
            }
            catch
            {
            }
            return "#off-line#";
        }


        private static string _CodHtmlStatusStr = "epguides";
        internal static string CodHtmlStatusStr
        {
            get
            {
                return _CodHtmlStatusStr;
            }
            set
            {
                _CodHtmlStatusStr = value;
            }
        }

        internal static string CodHtmlStatusStringExtractor()
        {
            try
            {
                System.Net.WebClient ccc = new System.Net.WebClient();
                string codhtml = ccc.DownloadString("http://epguides.com/hiatus/");
                ccc.Dispose();
                return codhtml;
            }
            catch
            {
            }

            return "#off-line#";
        }


        internal static bool torrentPageAlreadyExtrated = false;
        internal static int torrentRankingGoBackParentIndex = 0;
        
        internal static int weekOfTheYear(DateTime day)
        {

            System.Globalization.DateTimeFormatInfo dfi = System.Globalization.DateTimeFormatInfo.CurrentInfo;
            DateTime date1 = day;
            System.Globalization.Calendar cal = dfi.Calendar;

            return cal.GetWeekOfYear(date1, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
        }

        internal static string CodHtmlGoogle2AnySite(string myUrl)
        {
            try
            {
                System.Net.WebClient ccc = new System.Net.WebClient();
                string codhtml = ccc.DownloadString(myUrl);
                ccc.Dispose();
                return codhtml;
            }
            catch
            {
            }

            return "#off-line#";
        }
    }
}
