
using System;
using System.Deployment.Application;
using System.Windows.Forms;


namespace MyTvShowsOrganizer
{
    public static class AppUpdate
    {  //manual update. There is also an automatic update (every 2 days) that is more reliable.

        public static Version CurrentVer
        {
            get
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
        }

        public static void InstallUpdateSyncWithInfo()
        {
            DialogResult dr = DialogResult.No;
            if (Parameters.IsInternetConnectionOk)
            {
                bool doUpdate = true;
                bool errorCatched = false;


                doUpdate = true;
                if (doUpdate)
                {
                    UpdateCheckInfo info = null;
                    if (ApplicationDeployment.IsNetworkDeployed)
                    {
                        ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                        try
                        {
                            info = ad.CheckForDetailedUpdate();
                        }
                        catch (DeploymentDownloadException)//Exception
                        {
                            errorCatched = true;

                            dr = MsgBox.Show(@"A new version was found but it was not possible download automatically.
Notice that you will need to Uninstall the old version and install again 
the new version from https://sourceforge.net/projects/mytvshoworganizer/.
Do You Want Go There Now?", "Sorry...", MsgBox.Buttons.YesNo, MsgBox.Icone.Info, MsgBox.AnimateStyle.SlideDown);

                        }
                        catch (InvalidDeploymentException)
                        {
                            errorCatched = true;
                            dr = MsgBox.Show(@"The application Cannot check for a new version of the application.
The ClickOnce deployment may have been corrupted.
Please Try Uninstall and install again the new version
from https://sourceforge.net/projects/mytvshoworganizer/.
Do You Want Go There Now?", "Sorry...", MsgBox.Buttons.YesNo, MsgBox.Icone.Info, MsgBox.AnimateStyle.SlideDown); //+ ide.Message

                        }
                        catch (InvalidOperationException)
                        {
                            errorCatched = true;
                            dr = MsgBox.Show(@"This application cannot be updated for a Unknown reason.
Please Try Uninstall and install again the new version
from https://sourceforge.net/projects/mytvshoworganizer/.
Do You Want Go There Now?", "Sorry...", MsgBox.Buttons.YesNo, MsgBox.Icone.Info, MsgBox.AnimateStyle.SlideDown);  //+
                        }
                        catch (TrustNotGrantedException)
                        {
                            errorCatched = true;
                            dr = MsgBox.Show(@"Can't Update.
It is an error of type: Trust not granted.
Please Try Uninstall and install again the new version
from https://sourceforge.net/projects/mytvshoworganizer/.
Do You Want Go There Now?", "Sorry...", MsgBox.Buttons.YesNo, MsgBox.Icone.Info, MsgBox.AnimateStyle.SlideDown);  //+ 

                        }

                        if (!errorCatched)
                        {
                            if (info.UpdateAvailable)
                            {
                                if (!info.IsUpdateRequired)
                                {
                                    dr = MsgBox.Show(@"An update is available. Would you like to update the application now?", "Update Available", MsgBox.Buttons.OKCancel, MsgBox.Icone.Info, MsgBox.AnimateStyle.SlideDown);
                                    if (DialogResult.Cancel == dr)
                                    {
                                        doUpdate = false;
                                    }
                                }
                                else
                                {
                                    MsgBox.Show(@"This application has detected a mandatory
                                    update from your current version to " + info.MinimumRequiredVersion.ToString() + @".
                                    The application is going to install the update and restart.",
                                      "Mandatory Update Available", MsgBox.Buttons.OK, MsgBox.Icone.Info, MsgBox.AnimateStyle.SlideDown);
                                }
                                if (doUpdate)
                                {
                                    try
                                    {
                                        ad.Update();
                                        MsgBox.Show(@"The application has been updated,
                                        and it is going to restart.", "Congratulations", MsgBox.Buttons.OK, MsgBox.Icone.Info, MsgBox.AnimateStyle.SlideDown);
                                        Application.Restart();
                                    }
                                    catch (DeploymentDownloadException)//dde
                                    {
                                        dr = MsgBox.Show(@"There was an Error while Downloading.
Cannot install the latest version of the application.
Please Try Uninstall and install again the new version
from https://sourceforge.net/projects/mytvshoworganizer/.
Do You Want Go There Now?", "Downloading Error", MsgBox.Buttons.YesNo, MsgBox.Icone.Info, MsgBox.AnimateStyle.SlideDown); //+ dde.Message

                                    }
                                }
                            }
                            else
                            {
                                MsgBox.Show("You have the latest version.", "No update necessary.", MsgBox.Buttons.OK, MsgBox.Icone.Info, MsgBox.AnimateStyle.SlideDown);
                                return; //dont show last message
                            }
                        } //error catched

                        if (dr == DialogResult.Yes)
                        {
                            Go2Web.OpenLink("https://sourceforge.net/projects/mytvshoworganizer/");
                        }

                    }
                    else
                    {

                        if (Parameters.isDebugging)
                        { //for testing only


                            ////
                            MsgBox.Show(@"A new version was found but it can't download automatically.Notice that you will need to Uninstall old version and install again the new version from https://sourceforge.net/projects/mytvshoworganizer/. Do You Want Go There Now?");

                            dr = MsgBox.Show(@"An update is available. Would you like to update the application now?", @"Update Available Testing Very Long Title.Testing Very Long Title.Testing Very Long TitleUpdate Available Testing Very Long Title.Testing Very LongTUpdate Available Testing Very Long Title.Testing Very Long T ", MsgBox.Buttons.OKCancel);

                            dr = MsgBox.Show(@"An update is available. Would you like to update the application now?", @"Update Available Testing Very Long Title.Testing Very Long Title.
Testing Very Long TitleUpdate Available Testing Very Long Title.Testing Very Long
TUpdate Available Testing Very Long Title.Testing Very Long T ", MsgBox.Buttons.OKCancel);

                            MsgBox.Show("You have the latest version.", "No update necessary.", MsgBox.Buttons.OK, MsgBox.Icone.Info, MsgBox.AnimateStyle.SlideDown);

                           
                            dr = MsgBox.Show(@"Cannot check for a new version of the application.The ClickOnce deployment may be corrupted. Please Try Uninstall and install again the new version from https://sourceforge.net/projects/mytvshoworganizer/. Do You Want Go There Now?Cannot check for a new version of the application.The ClickOnce deployment may be corrupted. Please Try Uninstall and install again the new version from https://sourceforge.net/projects/mytvshoworganizer/. Do You Want Go There Now?Cannot check for a new version of the application.The ClickOnce deployment may be corrupted. Please Try Uninstall and install again the new version from https://sourceforge.net/projects/mytvshoworganizer/. Do You Want Go There Now?Cannot check for a new version of the application.The ClickOnce deployment may be corrupted. Please Try Uninstall and install again the new rrupted. Please Try Uninstall and install again the new version from https://sourceforge.net/projects/mytvshoworganizer/. Do You Want Go There Now?Cannot check for a new version of the application.The ClickOnce deployment may be corrupted. Please Try Uninstall and install again the new version from https://sourceforge.net/projects/mytvshoworganizer/. Do You Want Go There Now?Cannot check for a new version of the application.The ClickOnce deployment may be corrupted. Please Try Uninstall and install again the new version from https://sourceforge.net/projects/mytvshoworganizer/. Do You Want Go There Now?Cannot check for a new version of the application.The ClickOnce deployment may be corrupted.Do You Want Go There Now?", "Sorry...", MsgBox.Buttons.YesNo, MsgBox.Icone.Info, MsgBox.AnimateStyle.ZoomIn); //+ ide.Message


                            dr = MsgBox.Show(@"Cannot check for a new version of the application.The ClickOnce deployment may be corrupted. Please Try Uninstall and install again the new version from https://sourceforge.net/projects/mytvshoworganizer/. Do You Want Go There Now?Cannot check for a new version of the application.The ClickOnce deployment may be corrupted. Please Try Uninstall and install again the new version from https://sourceforge.net/projects/mytvshoworganizer/. Do You Want Go There Now?Cannot check for a new version of the application.The ClickOnce deployment may be corrupted. Please Try Uninstall and install again the new version from https://sourceforge.net/projects/mytvshoworganizer/. Do You Want Go There Now?Cannot check for a new version of the application.The ClickOnce deployment may be corrupted. Please Try Uninstall and install again the new rrupted. Please Try Uninstall and install again the new version from https://sourceforge.net/projects/mytvshoworganizer/. Do You Want Go There Now?Cannot check for a new version of the application.The ClickOnce deployment may be corrupted. Please Try Uninstall and install again the new version from https://sourceforge.net/projects/mytvshoworganizer/. Do You Want Go There Now?Cannot check for a new version of the application.The ClickOnce deployment may be corrupted. Please Try Uninstall and install again the new version from https://sourceforge.net/projects/mytvshoworganizer/. Do You Want Go There Now?Cannot check for a new version of the application.The ClickOnce deployment may be corrupted.Do You Want Go There Now?", "Sorry...", MsgBox.Buttons.YesNo, MsgBox.Icone.Info, MsgBox.AnimateStyle.FadeIn); //+ ide.Message

                                     
                        }
                    }

                }  //cancel clicked
            } //no internet
            else
            {
                Parameters.InternetBadMessage();
            }
        }
    }
}
