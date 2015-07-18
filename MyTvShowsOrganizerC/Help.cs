using System.Windows.Forms;
using System.Deployment.Application;

namespace MyTvShowsOrganizer
{
    public static class Help
    {

        public static void MainHelp()
        {

            string myhelpstr = null;
            string myVersion = null;
            if (ApplicationDeployment.IsNetworkDeployed)
            {//Parameters.AssemblyVersion.ToString()
                myVersion = "V." + AppUpdate.CurrentVer;
            }
            else
            {
                myVersion = "D." + "2000.00.00.00";
            }

            myhelpstr =
    @"NOTICE THAT TO MANTAIN A COPY OF TV-SERIES FOR PERSONAL USE, UNLESS YOU HAVE CABLE TV OR OTHER PAID
SERVICE, HAS COPYRIGHT ISSUES IN THE MOST OF COUNTRIES. BE ADVISED!
MyTvShowsOrganizer is a simple and intuitive program to organize and/or download (by a torrent Program) your favorite
Tv-Series. It gets given information about the series (name, season, episode, resolution etc..) and search the web at the
chosen torrent WebSite, using InternetExplorer Browser. Then, the magnetlink/torrent for the occurence more seeded, is  
sent to your torrent program (must be magnetlink compatible such as utorrent) which must be installed.

WIZARD: Add all your favorite Tv-Series Clicking with Mouse Right Button on First Column Cells (blanks); Verify Torrent
availability clicking 'NextEpisode' (at header); then, Click Button 'GetTorrent' to search and download the marked 
ones (this is going to send them to your installed Torrent Program).
FEATURES:
            # Get your torrents automatically with 2 clicks: 'NextEpisode' and 'GetTorrent'.
            # Click with mouse right-button on controls to get Web Translation.
            # Add new Series from a list Clicking with Mouse Right Button on a Free slot (in first Column).
            # Click over Button 'TV-Serie' (on header) to Classify current Page by Name and/or by Status.
            # Click over Button 'Organize' to Move and Organize (in subfolders) all the torrent files
(that contains S??E?? substring in Name), from Torrent Folder to your designated Folder. Subtitle, video
and compressed files will be moved and all other files (like .nfo .txt etc) will be sent to Recycle Bin.
            # Save up to 50 PAGES with 20 Tv-Series (the saved files will be put in your 'Documents Folder'
under MyTvShowsOrganizer SubFolder).
            # Move Series between PAGES clicking on 'Move' button. Usefull to separate series:
'on air' and 'waiting next season'.
            # Click with mouse right-button on series (first column) to open menu and get more functions.
            # Choose your preferences over config button.
 
           IMPORTANT: NOTICE that if you choose to register at poddesign/Tv-Calendar (website) and filter your series
there, this program will only display information for the filtered series. Make sure they are in sync (program and site).
                     
TIP1: If GetTorrent function is returning unpredicted results, because there are similar names of series, use Exclusion
character '-' (minus sign) and the word to be excluded, as you would do on a google search (Only works at Kickass
and Torrentz).
Ex: 'Believe -criss', will exclude 'Criss Angel Believe' series from the results of 'Believe'. Notice that '-criss' must
be in the 'Plus' Column.

Support Tv-Series producers, subscribe to your local cable-tv or a paid provider of on-demand Internet media like Netflix,
CloudLoad, AmazonPrime, Itunes etc.";
           
            MsgBox.Show(myhelpstr, "Your Tv Series Organizer - " + myVersion, MsgBox.Buttons.OK, MsgBox.Icone.Question, MsgBox.AnimateStyle.FadeIn);
        }
    }
}
