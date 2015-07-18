using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;

namespace MyTvShowsOrganizer
{
    public static class MoveOrganizeTorrents
    {
        ////Parameters pubs = new Parameters();
        public static bool MoveTorrents2SeriesFolder(string dirTorrents, string dirSeries)  //arg is directory in this case
        {
            //Tranfere os Torrents terminados para ex: c:\My Series\, manda diretorios vazios e arquivos inúteis para a lixeira.
            //importante: o folder final das series deve estar no mesmo drive do folder de torrents, até porque o Move é mais eficiente que Copy

            //string driv = mydrive.Name;
            string TorrentPath = dirTorrents + "\\";
            DriveInfo driveTorrent = new DriveInfo(TorrentPath);
            DriveInfo driveSeries = new DriveInfo(dirSeries);
            string myseriespath = dirSeries + "\\";
            bool recursiv = false;
            bool goAhead = true;

            if (!(driveSeries.Name == driveTorrent.Name))
            {
                DialogResult dr = MsgBox.Show("Torrent Drive (" + driveTorrent.Name + ") is different from Series Drive (" + driveSeries.Name + @").
Moving between different drives is going to take much more time.
It's advisable choose both in the same Drive.
Do you want continue regardeless?", "Are you Sure?", MsgBox.Buttons.OKCancel, MsgBox.Icone.Exclamation, MsgBox.AnimateStyle.SlideDown);
                if (DialogResult.Cancel == dr)
                {
                    goAhead = false;
                }
            }

            if (goAhead)
            {

                MoveArqs(TorrentPath, myseriespath);
                DeleteTrashFiles(TorrentPath);
                Thread.Sleep(1500);

                recursiv = DeleteEmptyFolders(TorrentPath);

                if (recursiv)
                {
                    DeleteEmptyFolders(TorrentPath);
                }
            }
            return goAhead;
        }

        //_______________________________________________________________________________________________//

        private static void MoveArqs(string dirTorrent, string dirSeries)
        {
            int x = 0;
            string myextension;
            string SubSerieName = "";
            string SerieName = "";
            if (!(Directory.Exists(dirSeries)))
            {
                Directory.CreateDirectory(dirSeries);
            }
            List<string> myfiles = new List<string>(Directory.EnumerateFiles(dirTorrent, "*S??E??*", SearchOption.AllDirectories));

            //enumerate only * e ? (any e one caracter)
            foreach (string fil in myfiles)
            {
                if (Regex.IsMatch(fil, "[S,s][0-9][0-9][E,e][0-9][0-9]")) ////the searchpattern (enumeratefiles) not working well. "*S??E??*" is catching the string Skies.
                {
                    SerieName = Path.GetFileName(fil).Replace("'", "").Replace(" ", "."); ;
                    myextension = Path.GetExtension(fil).ToLower();
                    //string myextensioncuted = myextension. Remove(2);
                    if ((myextension == ".mkv") || (myextension == ".mp4") || (myextension == ".avi") || (myextension == ".srt") || (myextension == ".sub") || (myextension == ".idx") || (myextension.Remove(2) == ".r") || (myextension.Remove(2) == ".z"))
                    {
                        //Console.WriteLine("Found: {0}", SerieName); //fil. Substring(fil. LastIndexOf("\\") + 1)

                        //http://msdn.microsoft.com/en-us/library/2k3te2cs(v=vs.90).aspx  wild cards to regex
                        String[] splitname = Regex.Split(SerieName, "[S,s][0-9][0-9][E,e][0-9][0-9]");

                        //SubSerieName = SubSerieName .Remove(string.Compare(

                        SubSerieName = splitname[0];

                        //SubSerieName = Microsoft. VisualBasic. Strings. LSet(SerieName , Regex.(SerieName , ".S*." , Microsoft. VisualBasic. CompareMethod. Text));
                        SubSerieName = SubSerieName.Replace(".", " ").Trim();
                        SubSerieName = dirSeries + SubSerieName;
                        //AppDomain.CurrentDomain.BaseDirectory
                        try
                        {
                            if (!Directory.Exists(SubSerieName))
                            {
                                Directory.CreateDirectory(SubSerieName);
                            }
                            if (!File.Exists(Path.Combine(SubSerieName, SerieName)))
                            {
                                File.Move(fil, Path.Combine(SubSerieName, SerieName));
                                //Console.WriteLine("Moved: {0}", SerieName);
                                x += 1;
                            }
                            else
                            {
                                //File. Delete(Path. Combine(SubSerieName , SerieName));
                                //why visual basic? Because until now there is no method, .net or C#, that send deleted files to recyclebin
                                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(Path.Combine(dirTorrent, SerieName), Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);

                                x += 1;
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            } //next fil
        }



        //-----------------------------------------------------------------------

        private static void DeleteTrashFiles(string dir)
        {
            int x = 0;
            string fileName = "";

            List<string> arqs = new List<string>(Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories));
            foreach (string DeletableFile in arqs)
            {

                fileName = Path.GetFileName(DeletableFile).ToLower();
                if (fileName.Contains(".txt") || fileName.Contains("sample") || fileName.Contains(".nfo"))
                {
                    try
                    {
                        //File. Delete(DeletableFile);
                        Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(DeletableFile, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                        x += 1;
                    }
                    catch
                    {

                    }
                }
            }
        }

        //-----------------------------------------------------------------------

        private static bool DeleteEmptyFolders(string mydir)
        //delete empty folders 
        {
            bool thereAreSubfolders = false;
            List<string> my_dirs = new List<string>(Directory.EnumerateDirectories(mydir, "*", SearchOption.AllDirectories));
            //List<string> myfiles = new List<string>(Directory.EnumerateFiles(dirPath, "*S??E??*", SearchOption.AllDirectories));
            //enumerate only * e ? (any e one caracter)
            //string dir_full_name;
            foreach (string folder in my_dirs)
            {
                DirectoryInfo dir = new DirectoryInfo(folder);
                //dir_full_name = dir.FullName;
                //FileInfo[] my_files = dir.GetFiles();
                //int NofFiles = my_files.GetLength(0);

                try
                {
                    if (dir.GetDirectories().Length == 0)
                    {

                        if (dir.GetFiles().Length == 0) //no files in it
                        {
                            try
                            {
                                dir.Delete();
                            }
                            catch
                            {

                            }

                        }
                    }

                    else  //subfolders preventing delete
                    {
                        DirectoryInfo[] Subdirs = dir.GetDirectories();
                        int nfiles = 0;
                        foreach (DirectoryInfo subdir in Subdirs)
                        {
                            nfiles += subdir.GetFiles().Length;
                        }
                        if (nfiles == 0)
                        {
                            thereAreSubfolders = true;
                        }
                    }

                }
                //Console.WriteLine("{0}\t{1}", NofFiles,dir.Name );

                catch
                {

                }

            } //next folder

            if (thereAreSubfolders)
            {
                return true;
            }
            return false;
        }
    }
}
