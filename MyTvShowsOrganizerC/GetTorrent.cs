using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MyTvShowsOrganizer
{
    class GetTorrent
    {

        SHDocVw.InternetExplorer iee = new SHDocVw.InternetExplorer();

        //these vars will come from main as arguments to this class.

        internal string TorrSearcherName;
        internal int nCheckedBoxes;
        internal bool IEisHidden;
        internal bool QuitIe;  //determines if Ie must be closed after running.
        //internal List<int> MarkedCboxesIndexes;
        //I could use 9 dynamic lists (generic.collection) with n items each (n<=20) or a single static 2 dimension array[20,9]. Array wins.

        internal string[,] Cbox5TboxesOldValues = new string[Parameters.nOfRows, Parameters.nOfColumns + 1]; //{ { "checkboxIndex", "SeriesName", "Season", "Episode", "Resolution", "Plus", "Lastdownload", "next episode", "status" } }; 

        internal class CurrentState
        {
            //these vars will return with new values to main. Will be accessed there, therefore, must be internal
            internal System.Drawing.Color torrentFontColour;
            internal int arrayCurrentRow; //in panel1

            internal string[,] Cbox5TboxesNewValues = new string[Parameters.nOfRows, Parameters.nOfColumns + 1];

            internal CurrentState Clone() //will be passed to main instead original one. Will prevent error when progresschanged (which runs independently on main thread) is slower than worker.
            {
                CurrentState deepClone = (CurrentState)this.MemberwiseClone();
                deepClone.torrentFontColour = new System.Drawing.Color();
                deepClone.torrentFontColour = this.torrentFontColour;

                for (int i = 0; i < 20; i++)
                {
                    {
                        for (int j = 0; j < 7; j++)
                        {
                            deepClone.Cbox5TboxesNewValues[i, j] = this.Cbox5TboxesNewValues[i, j];
                        }
                    }
                }

                //this.Cbox5TboxesNew.CopyTo(deepClone.Cbox5TboxesNew, 0);
                return deepClone;
            }
        }

        private void SaveIEid(string ieProcessName)
        //will save processId of the InternetExplorer interop object to further kill if necessary
        {
            int n = 0;
            int[] IeId = new int[2];
            foreach (System.Diagnostics.Process proc in System.Diagnostics.Process.GetProcesses())
            {
                if (proc.ProcessName.Contains(ieProcessName) && !proc.ProcessName.Contains("*32") && string.IsNullOrEmpty(proc.MainWindowTitle))  //iee.visible = false
                {
                    IeId[n] = proc.Id;
                    if (n == 1) break;
                    n++;
                }
            }

            string savedFile = Path.Combine(Parameters.myProgPath, "IeId.inf");
            if (File.Exists(savedFile))
            {
                File.Delete(savedFile);
                File.Create(savedFile).Dispose();

            }
            String array2str = string.Join(",", IeId);
            File.WriteAllText(savedFile, array2str);
        }


        internal void CallMagnetLink_in_Torrent_Indexer(BackgroundWorker worker, DoWorkEventArgs e)
        {

            CurrentState currentstate = new CurrentState();

            string myHyper = null;
            string mySerie = null;
            //string mySerie2 = null;
            string myPlusString = null;
            string myEpisode = null;
            string mySeason = null;
            string mySE = null;
            string myResolution = null;
            string myTag;
            string codhtml;

            bool magnetExists;
            bool pointTorrentExists;
            bool isCallProcessStarted;

            int timeElapse = 0;

            // SHDocVw.InternetExplorer iee = new SHDocVw.InternetExplorer();
            string ieeName = iee.FullName;
            ieeName = Path.GetFileNameWithoutExtension(ieeName).ToLower();
            SaveIEid(ieeName); //save Iee process Id for further Kill, if necessary (error).

            currentstate.Cbox5TboxesNewValues = Cbox5TboxesOldValues;
            //shows all if visible and nothing if hidden
            iee.FullScreen = IEisHidden;
            iee.AddressBar = !IEisHidden;
            iee.Visible = !IEisHidden;
            string twoInOne = ""; //2 episodes in 1 file warning

            for (int showRow = 0; showRow < nCheckedBoxes; showRow++)
            {

                if (worker.CancellationPending || Parameters.isGetTorrentStoped)
                {
                   // worker.CancelAsync();
                   e.Cancel = true;
                   // Parameters.isGetTorrentStoped = true;
                    break;  //scape for
                }
                else
                {
                    codhtml = "";
                    magnetExists = false;
                    pointTorrentExists = false;
                    mySerie = Cbox5TboxesOldValues[showRow, 1];
                    isCallProcessStarted = false;

                    if (!(string.IsNullOrEmpty(mySerie.Trim())))
                    {
                        currentstate.torrentFontColour = Colours.showsFontGettingTorrent; //SpringGreen;
                        currentstate.arrayCurrentRow = showRow; //checkbox.text 
                        worker.ReportProgress(0, currentstate.Clone());
                        //sending a clone instead original (state class) will prevent error (state has been changed before completed) when reportprogress/progresschanged take more time to complete than next report. 

                        Thread.Sleep(500);

                        //mySeason = MarkedTextBoxes[1].Text;
                        mySeason = Cbox5TboxesOldValues[showRow, 2].Trim();
                        myEpisode = Cbox5TboxesOldValues[showRow, 3].Trim();
                        myResolution = Cbox5TboxesOldValues[showRow, 4].ToLower().Trim();
                        if (!(myResolution.Contains("p") || myResolution.Contains("k"))) myResolution = "";
                        myPlusString = Cbox5TboxesOldValues[showRow, 5].ToLower().Trim();
                        if (myPlusString == "none") myPlusString = "";

                        mySeason = mySeason.PadLeft(2, '0');
                        myEpisode = myEpisode.PadLeft(2, '0');
                        //if (mySeason.Length == 1)
                        //{
                        //    mySeason = "0" + mySeason;
                        //}
                        //if (myEpisode.Length == 1)
                        //{
                        //    myEpisode = "0" + myEpisode;
                        //}

                        mySerie = Parameters.ClearStr(mySerie);
                        mySE = "S" + mySeason + "E" + myEpisode;

                        string searcher = TorrSearcherName;

                        bool isSpecialCase = false;

                        //string myPlusString2 = "";
                        string[] codhtmlAndTime = new string[2];

                        switch (searcher)
                        {

                            case "https://kat.cr/":

                                //https://kat.cr/usearch/the%20returned%20(us)%20s01e03%20720p%20category/?field=seeders&sorder=desc
                                //("https://kat.cr/usearch/" + mySerie + " 720p OR 1080p OR HDTV" + "/?field=time_add&sorder=desc")

                                //if (myPlusString != "")
                                //{
                                //    myPlusString2 = "%20" + myPlusString.Replace(" ", "%20"); //myplusstring2 is needed, dont change
                                //}
                                //mySerie2 = mySerie.Replace(" ", "%20");

                                myHyper = searcher + "usearch/" + mySerie + " " + mySE + " " + myResolution + " " + myPlusString + "/?field=seeders&sorder=desc";
                                myHyper = myHyper.ToLower();

                                break;

                            case "https://torrentz.eu/":

                                //https://torrentz.eu/search?f=The+Big+Bang+Theory+S08E18+720p //no seeders order
                                //https://torrentz.eu/search?q=The%2BBig%2BBang%2BTheory%2BS08E18%2B720p //with auto seeders order

                                //if (myPlusString != "")
                                //{
                                //    myPlusString2 = "+" + myPlusString.Replace(" ", "+"); //myplusstring2 is needed, dont change
                                //}
                                //mySerie2 = mySerie.Replace(" ", "+");
                                //myHyper = searcher + "searchA?f=" + mySerie2 + "+" + mySE + "+" + myResolution + myPlusString2;// + "/seeds:desc";
                                myHyper = searcher + "search?q=" + mySerie.Replace(" ", "+") + "+" + mySE + "+" + myResolution + "+" + myPlusString.Replace(" ", "+");// + "/seeds:desc";
                                isSpecialCase = true;

                                break;

                            case "http://www.torrenthound.com/":

                                if (myPlusString != "")//will replace excluded word from myPlus because not working
                                {
                                    myPlusString = Regex.Replace(myPlusString, @"-.{1,30}(?=\b)", "").Trim(); //\b is boundaries (space and final of the string)
                                }
                                // mySerie2 = mySerie.Replace(" ", "+");
                                myHyper = searcher + "search/1/" + mySerie.Replace(" ", "+") + "+" + mySE + "+" + myResolution + "+" + myPlusString.Replace(" ", "+") + "/seeds:desc";

                                break;

                            case "https://www.torrentday.com/":   //who knows in the future....
                                //https://www.torrentday.com/browse.php?search=The+Big+Bang+Theory+S08E17+&cata=yes#/browse.php?&search=The+Big+s08e16&&s=4&t=2

                                if (myPlusString != "")
                                {
                                    myPlusString = Regex.Replace(myPlusString, @"-.{1,30}(?=\b)", "").Trim(); //\b is boundaries (space and final of the string)
                                }
                                //mySerie2 = mySerie.Replace(" ", "+");
                                myHyper = searcher + "browse.php?search=" + mySerie.Replace(" ", "+") + "+" + mySE + "+" + myResolution + "+" + myPlusString.Replace(" ", "+") + "+&cata=yes#/browse.php?&search=" + mySerie.Replace(" ", "+") + "+" + mySE + "+" + myResolution + "+" + myPlusString.Replace(" ", "+") + "&&s=4&t=2";

                                break;


                            case "http://metasearch.torrentproject.com/":
                                //  http://metasearch.torrentproject.com/#!search=helix+720p+s02e10

                                if (myPlusString != "")
                                {
                                    myPlusString = Regex.Replace(myPlusString, @"-.{1,30}(?=\b)", "").Trim(); //\b is boundaries (space and final of the string)
                                }
                                //mySerie2 = mySerie.Replace(" ", "+");
                                // exclude not working
                                myHyper = searcher + "#!search=" + mySerie.Replace(" ", "+") + "+" + mySE + "+" + myResolution + "+" + myPlusString.Replace(" ", "+");// + "/seeds:desc";
                                isSpecialCase = true;

                                break;

                            case "http://thepiratebay.cd/":
                                // http://thepiratebay.cd/search/the%20equalizer%201080p/0/7/0
                                //http://thepiratebay.cr/search.php?q=mom+s02e05+720p&video=on&page=0&orderby=99
                                //http://thepiratebay.cd/search/grimm%20s02e05%20720p/0/7/0

                                if (myPlusString != "")
                                {
                                    myPlusString = Regex.Replace(myPlusString, @"-.{1,30}(?=\b)", "").Trim(); //\b is boundaries (space and final of the string)
                                }
                                //mySerie2 = mySerie.Replace(" ", "%20");
                                myHyper = searcher + "search/" + mySerie + "%20" + mySE + "%20" + myResolution + myPlusString + "/0/7/0";

                                break;

                            //https://isohunt.to/   magnet at second page. not good results on specific searching. not advanced filter in command line.
                            //https://isohunt.to/torrents/?ihq=supernatural+s10e16+1080p+dd5
                            //https://1337x.to/torrent/1034364/Its-Always-Sunny-in-Philadelphia-S10E01-720p-HDTV-x264-KILLERS/ //magnet at second page
                            //https://rarbg.to/  needs login
                            //http://extratorrent.cc/search/?search=always+sunny+s10e01&new=1&x=30&y=14  magnet at second page
                            //https://www.torlock.com/all/torrents/always-sunny-s10e01-720p.html no magnet
                            //http://yourbittorrent.com/ no magnet
                            //https://www.monova.org/search?term=The+Last+Ship+S02E03 magnet at second page
                            //http://torrentreactor.com/torrents-search/its+always+sunny+in+philadelphia+s10e01?type=all&period=none&categories= ?????? weird
                            //http://www.seedpeer.eu/search/always-sunny-s10e10/2/1.html magnet at second page
                            //http://www.torrentdownloads.me/search/?search=always+sunny+s10e01  no magnet
                            //http://www.torrents.net/find/always+sunny+s10e01+720p/  lack of results, .torrent in first page
                            //http://www.torrentfunk.com/torrent/10328052/its.always.sunny.in.philadelphia.s10e01..x264-killers.mp4.html no magnet
                            //https://www.limetorrents.cc/search/all/always-sunny-s10e01/ magnet at second but hidden, bat quality results
                            //http://bitsnoop.com/search/all/always+sunny+s10e01/c/d/1/ magnet at second
                            //https://getstrike.net/torrents/  no search string on addressbar

                        }

                        mshtml.HTMLDocument doc2;

                        // ________________________________________________ special case begins

                        //the next switch is for sites like torrentz
                        //torrentz special case needs 2 others extra pages.

                        if (isSpecialCase)
                        {
                            switch (searcher)
                            {
                                case "https://torrentz.eu/":
                                    {
                                        codhtmlAndTime = IeeNavigator(myHyper);

                                        codhtml = codhtmlAndTime[0].ToLower();
                                        timeElapse = Convert.ToInt16(codhtmlAndTime[1]);

                                        if (!Regex.IsMatch(codhtml, "no torrents found", RegexOptions.IgnoreCase) && timeElapse < 30)
                                        {
                                            bool breakFor = false;
                                            string firstZHyperlink;

                                            //<a href="/03b3dec13dfc4867324db5732e6a9e5fae62f859"><b>The</b> <b>Big</b> <b>Bang</b> <b>Theory</b> <b>S08E17</b> <b>720p</b> HDTV X264 DIMENSION GloDLS</a> &#187; tv hd</dt><dd><span class="v" style="color:#fff;background-color:#A2EB80">1</span><span class="a"><span title="Fri, 06 Mar 2015 02:40:25">21 days</span></span><span class="s">436 MB</span> <span class="u">427</span><span class="d">16</span></dd></dl>

                                            // https://torrentz.eu/03b3dec13dfc4867324db5732e6a9e5fae62f859   
                                            //https://torrentz.eu/97896a7c048b47ed359a851a2c3541dec39df5fe
                                            firstZHyperlink = Regex.Match(codhtml, @"<a href=""/\w{40}"">", RegexOptions.Multiline).Value;
                                            //.......................................

                                            if (!string.IsNullOrEmpty(firstZHyperlink))
                                            {
                                                firstZHyperlink = searcher + firstZHyperlink.Substring(10).Remove(40);

                                                codhtmlAndTime = IeeNavigator(firstZHyperlink);
                                                codhtml = codhtmlAndTime[0].ToLower();
                                                timeElapse = Convert.ToInt16(codhtmlAndTime[1]);

                                                if (timeElapse < 30)
                                                {
                                                    string myHref = "";
                                                    // string[] magnetSitesAtTorrentz = new string[] { "//kickass.to/", "//katproxy.com/", "//rarbg.com/", "//extratorrent.cc/", "//www.torrenthound.com/", "//www.torrentreactor.net/", "//www.torrents.net/", "//torrentproject.se/", "//www.btscene.cc/", "//bitsnoop.com/" };
                                                    string[] magnetSitesAtTorrentz = new string[] { "kickass", "kat", "rarbg", "extratorrent", "torrenthound", "torrentreactor", "torrents.net", "torrentproject", "btscene", "bitsnoop", "piratebay", "getstrike" };
                                                    //http://katproxy.com/the-gambler-2014-720p-brrip-x264-yify-t10431631.html
                                                    //https://rarbg.com/torrents/filmi/download/pw3dnsm/torrent.html
                                                    /*Make sure that kickass.to has magnet links*/
                                                    //1337x.to has not magnet indeed"
                                                    //  <a href="https://kickass.to/the-big-bang-theory-s08e17-the-colonization-application-720p-web-dl-dd5-1-h-264-yfn-t10311006.html" rel="e">

                                                    doc2 = (mshtml.HTMLDocument)iee.Document;
                                                    mshtml.IHTMLElementCollection magnettags = doc2.getElementsByTagName("a");

                                                    foreach (mshtml.IHTMLElement tag in magnettags)
                                                    {
                                                        if (breakFor)
                                                        {
                                                            break;
                                                        }

                                                        myHref = "";
                                                        myHref = tag.getAttribute("href").ToString();
                                                        if (!myHref.Contains("torrentz"))
                                                        {
                                                            foreach (string magnetSite in magnetSitesAtTorrentz)
                                                            {
                                                                if (myHref.Contains(magnetSite))
                                                                {
                                                                    myHyper = myHref;
                                                                    breakFor = true;

                                                                    break; // the correct "a" tag with desired link will be on top of the web list or will not be at all. 
                                                                }
                                                            }
                                                        } //next tag
                                                    }//try next magnetSite
                                                }
                                            }  // no match
                                            if (!breakFor)
                                            {
                                                codhtml = "no torrents found"; //no magnetsite reliable goto next show
                                            }

                                        } //no torrents found at torrentz
                                    }
                                    break;

                                case "http://metasearch.torrentproject.com/":  //not working, not reliable searcher, Helix has problem, no advanced search on command line. 
                                    {
                                        codhtmlAndTime = IeeNavigator(myHyper);
                                        codhtml = codhtmlAndTime[0].ToLower();
                                        timeElapse = Convert.ToInt16(codhtmlAndTime[1]);

                                        bool breakFor = false;
                                        if (timeElapse < 30)
                                        {
                                            // string magnetSiteFound = "";
                                            string[] magnetSitesAtTorrentP = new string[] { "kickass", "kat", "getstrike", "torrentbox", "bitsnoop", "piratebay" };
                                            //must be searchstring with word 'search' in it for match good result.
                                            // "http://torrentproject.com/", "http://www.btscene.cc/" has magnet but not word search in searchstring
                                            string myHref;
                                            doc2 = (mshtml.HTMLDocument)iee.Document;
                                            mshtml.IHTMLElementCollection magnettags = doc2.getElementsByTagName("a");

                                            foreach (mshtml.IHTMLElement tag in magnettags)
                                            {
                                                if (breakFor)
                                                {
                                                    break;
                                                }

                                                myHref = "";
                                                myHref = tag.getAttribute("href").ToString();
                                                //mshtml.IHTMLElement hr = (mshtml.IHTMLElement)tag.getAttribute("href");
                                                //myHref = hr.innerHTML;
                                                if (!myHref.Contains("search"))
                                                {
                                                    foreach (string magnetSite in magnetSitesAtTorrentP)
                                                    {
                                                        if (myHref.Contains(magnetSite))
                                                        {
                                                            myHyper = myHref;
                                                            breakFor = true;

                                                            break; // the correct "a" tag with desired link will be on top of the web list or will not be at all. 
                                                        }
                                                    }
                                                } //next tag
                                            }//try next magnetSite
                                        }
                                        if (!breakFor)
                                        {
                                            codhtml = "no torrents found"; //no magnetsite reliable goto next show
                                        }
                                    }

                                    break;

                                //case "isohunt....." forget it. The site is not good, no command stringsearch with more seeded order, among other things.

                                default:

                                    break;
                            }// here myhyper will have the torrent searcher hyperlink via torrentz.eu
                        } //isnotaspecialcase
                        //______________________________________________special case end


                        if (!Regex.IsMatch(codhtml, "no torrents found", RegexOptions.IgnoreCase)) //will be "" or !=  de "" depending on isSpecialCase
                        {
                            codhtmlAndTime = IeeNavigator(myHyper);
                            codhtml = codhtmlAndTime[0].ToLower();
                            timeElapse = Convert.ToInt16(codhtmlAndTime[1]);


                            if (timeElapse < 30 && !Regex.IsMatch(codhtml, "This page can", RegexOptions.IgnoreCase) && !Regex.IsMatch(codhtml, "forgot your password", RegexOptions.IgnoreCase)) //if webpage error  (can't be displayed  && !string.IsNullOrEmpty(codhtml) forgot your password (torrentday)
                            {
                                if (mySerie.Contains(" -"))
                                {
                                    mySerie = mySerie.Remove(mySerie.IndexOf(" -")).Trim();
                                }
                                mySerie = mySerie.Replace("-", " "); //shows like brooklyn nine-nine needs to loose "-".

                                //string[] mySerieSplited = Regex.Split(mySerie, " ");
                                if (Regex.Split(mySerie, " ").Length > 1)  //split on spaces // none space then 1
                                {
                                    // mySerie = mySerieSplited[mySerieSplited.Length - 1];  //will be used below to certify that myserie (last piece) is in a url. 
                                    mySerie = Regex.Match(mySerie, @"(?<=\s)\w*$").Value;  // space (prefix, but not include) alfanumchar* $in the end of the string
                                }

                                // <a title="torrent magnet link" class="imagnet icon16" href="magnet:?xt=urn:btih:4162f2007f545fc1ef248bcaf04f18fe91e8166a&amp;dn=12+monkeys+s01e12+paradox+1080p+web+dl+dd5+1+h+264+bs+rartv&amp;tr=http%3a%2f%2ftracker.trackerfix.com%2fannounce&amp;tr=udp%3a%2f%2fopen.demonii.com%3a1337"><i class="ka ka16 ka-magnet"></i></a>
                                magnetExists = Regex.IsMatch(codhtml, @"magnet:\?x", RegexOptions.IgnoreCase); //magnet:\\?x

                                pointTorrentExists = Regex.IsMatch(codhtml, "[^(www)]\\.torrent", RegexOptions.IgnoreCase); //no www.torrent.... yes wwww*.torrent (to avoid www.torrentproject.com instead (.torrent files)

                                if (magnetExists || pointTorrentExists)
                                {
                                    //<a href="magnet:?xt=urn:btih:2f45164d053101b1aa718b069a8e55d6ddd0ee04&dn=Texas.Chainsaw.3D.2013.1080p.BluRay.Half-OU.DTS-HDMaNiAcS&tr=udp%3A%2F%2Ftracker.openbittorrent.com%3A80&tr=udp%3A%2F%2Ftracker.internalbt.com%3A80&tr=udp%3A%2F%2Ftracker.istole.it%3A6969&tr=udp%3A%2F%2Ftracker.ccc.de%3A80" title="Download this torrent using magnet">

                                    doc2 = (mshtml.HTMLDocument)iee.Document;
                                    mshtml.IHTMLElementCollection magnettags = doc2.getElementsByTagName("a");
                                    //string mytag;
                                    //magnetExists = false;
                                    //pointTorrentExists = false;
                                    bool myTagIsTorrent;
                                    bool JumpNextTag = false;

                                    foreach (mshtml.IHTMLElement tag in magnettags)
                                    {

                                        myTag = "";
                                        try
                                        {
                                            myTag = tag.getAttribute("href").ToString();
                                        }
                                        catch (Exception) //case "a" have no "href" attribute.
                                        {
                                            //throw;
                                        }

                                        myTagIsTorrent = Regex.IsMatch(myTag, "[^(www)]\\.torrent", RegexOptions.IgnoreCase); //no www, yes .torrent
                                        if (Regex.IsMatch(myTag, @"magnet:\?x", RegexOptions.IgnoreCase) || myTagIsTorrent)
                                        {
                                            /*kickass and others may not show correct season and episode if it is not available yet. Only piratebay will do it correctly. So one more if to be sure.*/
                                            //string myTagLower = myTag.ToLower();


                                            if (magnetExists && myTagIsTorrent) //if magnetExists but  mytag is .torrent then keep looking for magnets and ignore .torrents
                                            {
                                                JumpNextTag = true;
                                            }
                                            else
                                            {
                                                JumpNextTag = false;
                                            }

                                            if (!JumpNextTag)
                                            {
                                                if (Regex.IsMatch(myTag, mySE, RegexOptions.IgnoreCase) && Regex.IsMatch(myTag, mySerie, RegexOptions.IgnoreCase) && Regex.IsMatch(myTag, myResolution, RegexOptions.IgnoreCase))
                                                {

                                                    string[] pluses = Regex.Split(myPlusString, " ");

                                                    bool goAhead = true;

                                                    foreach (string plus in pluses)
                                                    {
                                                        string plus2;
                                                        if (Regex.IsMatch(plus, "^-."))
                                                        {
                                                            plus2 = plus.Substring(1);
                                                            goAhead = !Regex.IsMatch(myTag, plus2, RegexOptions.IgnoreCase);
                                                        }
                                                        else
                                                        {
                                                            goAhead = Regex.IsMatch(myTag, plus, RegexOptions.IgnoreCase);
                                                        }
                                                        if (!goAhead) break;
                                                    }

                                                    if (goAhead)
                                                    {
                                                        twoInOne = Regex.Match(myTag.ToUpper(), mySE + @"\D{1,2}" + (Convert.ToInt16(myEpisode) + 1).ToString().PadLeft(2, '0')+@"\D{1}").Value;//\D non digit 
                                                        if (!string.IsNullOrEmpty(twoInOne))
                                                        { //verify the ocurrence of 2 episodes united in one release, like s05e01-02
                                                            twoInOne = mySerie + "." + twoInOne;

                                                            MsgBox.Show("Take Note: It seems this Torrent has 2 episodes In one File: " + twoInOne, "2 Episodes in 1 File Detected. The NextEpisode is going to be E" + (Convert.ToInt16(myEpisode) + 2).ToString().PadLeft(2, '0') + ". Press OK to continue.", MsgBox.Buttons.OK, MsgBox.Icone.Info, MsgBox.AnimateStyle.ZoomIn);
                                                        }

                                                        try
                                                        {
                                                            Go2Web.OpenLink(myTag);
                                                            isCallProcessStarted = true;
                                                        }
                                                        catch (Exception)
                                                        {
                                                            MsgBox.Show("The system doesn't know what to do with internet links of type: torrent or magnetlink. This program needs a Torrent Program (like utorrent) able to open torrents/magnetlinks to work properly. Verify if it is installed in your system and/or if it is correctly associated with Torrent/MagnetLink files (see options / preferences / check association o startup).", "Impossible to send Torrent/Magnet links to Torrent Program.", MsgBox.Buttons.OK, MsgBox.Icone.Error, MsgBox.AnimateStyle.ZoomIn);

                                                            e.Cancel = true;

                                                            break;
                                                        }
                                                    }
                                                }

                                                break; // the correct "a" tag with magnet link will be on top of the web list or will not be at all. 
                                            }
                                        }
                                    } //next tag
                                }
                                else //magnetlink false
                                {
                                    
                                }
                            }
                            else
                            {
                                //iee is busy  DialogResult vbresult = 
                                MsgBox.Show("The Web Site: " + TorrSearcherName + ", is not responding or needs LogIn (torrentday). Try changing the Torrent Indexer before Retry.", "Web Site is Not Responding.", MsgBox.Buttons.OK, MsgBox.Icone.Info, MsgBox.AnimateStyle.SlideDown);
                                //if (vbresult == DialogResult.Cancel)
                                {
                                    //main.TextBoxForeColorRow(MarkedTextBoxes, System.Drawing.Color.DarkRed);
                                    currentstate.torrentFontColour = Colours.showsFontGettingTorrentNotFound;
                                    worker.ReportProgress(0, currentstate);
                                    break;
                                }
                            } //time out

                        } //specialcase Not Ok
                        DateTime agora = DateTime.Now;
                        if (isCallProcessStarted)
                        {
                            Int16 nep = 1;
                            if (!string.IsNullOrEmpty(twoInOne))
                            {
                                nep = 2;
                            }
                            currentstate.Cbox5TboxesNewValues[showRow, 3] = (Convert.ToInt16(Cbox5TboxesOldValues[showRow, 3]) + nep).ToString(); //add 1 to episode tbox
                            currentstate.Cbox5TboxesNewValues[showRow, 6] = " :-D " + mySE + "|" + "0" + "|" + agora.ToString("yyyy/MM/dd");
                            currentstate.torrentFontColour = Colours.showsFontGettingTorrentSuccess;
                        }
                        else
                        {
                            string txt5 = Cbox5TboxesOldValues[showRow, 6];
                            if (txt5.Length < 5)
                                txt5 = " :-D " + mySE + "|" + "0" + "|" + agora.ToString("yyyy/MM/dd");
                            //+ " " + Format(Now, "ddd") 'was empty
                            currentstate.Cbox5TboxesNewValues[showRow, 6] = " :-o " + txt5.Substring(5);

                            currentstate.torrentFontColour = Colours.showsFontGettingTorrentNotFound;
                            //state.markedCheckbox = checkedBox;
                            //worker.ReportProgress(0, state);
                        }
                        worker.ReportProgress(0, currentstate.Clone());
                        Thread.Sleep(200);
                        //iee.Stop();
                    }
                }// cancellation

                //If the program run too fast he might be considered a DDos attack by the indexer website.
                Thread.Sleep(Parameters.pauseBetweenWebSitesRequisitions);

            }   //next show

            File.Delete(Path.Combine(Parameters.myProgPath, "IeId.inf")); // method terminate normally, no need process id file for late kill. //if quit fails because a crash, the hidden explore.exe process will be Killed when the method/program runs again (closeAllIe).
            if (QuitIe)
            {
                iee.Quit();
            }
        }


        private string[] IeeNavigator(string hyperLink)
        {//SHDocVw.InternetExplorer iee,
            string[] codAndTime = new string[2];
            string codhtml = "";
            int elapseTime = 0;

            iee.Navigate("https://www.google.com/search?q=" + "automatically+download+Tv+shows+episodes+torrents+mytvshoworganizer");//to clean last html code.

            Thread.Sleep(1000);
            while (iee.Busy && elapseTime < 11)
            {
                Thread.Sleep(1000);
                elapseTime++;
            }

            iee.Navigate(hyperLink);

            if (elapseTime == 10)//
            {
                if (!Parameters.firstRun)//shows one time only
                {
                    DialogResult dr = MsgBox.Show(@"Very Slow Internet!
Do you Want to Continue?", "Warning", MsgBox.Buttons.YesNo, MsgBox.Icone.Info, MsgBox.AnimateStyle.ZoomIn);
                    Parameters.firstRun = true;  //Why first run? only saving a little ram reusing a global variable that is rarely used.

                    if (dr == DialogResult.No)
                    {
                        Parameters.isGetTorrentStoped = true;
                        codAndTime[0] = "no torrents found"; //will abbreviate the exit of the worker.
                        codAndTime[1] = "10";

                        return codAndTime;
                    }
                }
            }

            elapseTime = 0;
            Thread.Sleep(1000);

            mshtml.HTMLDocument doc2 = (mshtml.HTMLDocument)iee.Document;
            codhtml = null;
            //iee.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE ||
            // (webBrowser2.ReadyState != WebBrowserReadyState.Interactive  && string.IsNullOrEmpty(codhtml) && elapseTime < 10)
            //((iee.Busy || iee.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_INTERACTIVE || string.IsNullOrEmpty(codhtml)) && elapseTime < 30)

            Boolean stop = false;//= ((iee.Busy || iee.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_INTERACTIVE) || string.IsNullOrEmpty(codhtml)) && elapseTime < 20;
            //quando o tempo acabar vai ser falso;
            //se codhtml for vazio sempre vai continuar até que o tempo de passe
            //se estiver ocupado ou naõ estiver iterativo; basta um deles ser verdadeiro para continuar a verificação 


            while (!stop)
            {
                elapseTime++;
                try
                {
                    codhtml = doc2.body.innerHTML;
                }
                catch
                {

                }
                Thread.Sleep(1000);
                stop = ((iee.ReadyState == SHDocVw.tagREADYSTATE.READYSTATE_INTERACTIVE || iee.ReadyState == SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE) && !string.IsNullOrEmpty(codhtml)) || elapseTime == 20;
            }

            Thread.Sleep(2000);
            codhtml = doc2.body.innerHTML; //inner returns all between the body tags.
            codAndTime[0] = codhtml;
            codAndTime[1] = elapseTime.ToString();

            return codAndTime;

        }
    }
}
