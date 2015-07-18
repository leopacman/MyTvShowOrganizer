using System;
using System.Linq;
using System.Management;
using System.Windows.Forms;
using System.IO;

namespace MyTvShowsOrganizer
{
    public partial class About : Form
    {
        System.Windows.Forms.Timer timerTemp;
        public About()
        {
            InitializeComponent();
            Help_WebTranslator();
            this.Opacity = 0;

            timerTemp = new System.Windows.Forms.Timer();
            
            timerTemp.Tick += timerTemp_Tick;
            timerTemp.Interval = 200;
            timerTemp.Start();
            
        }
          

        void timerTemp_Tick(object sender, EventArgs e)
        {
            Parameters.FadeIn(this);
            timerTemp.Stop();
            timerTemp.Dispose();
        }
        
       
        //private string GetUserUniqueId()
        //{

        //    ManagementObjectSearcher objMOS = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration"); //"Win32_NetworkAdapterConfiguration"Win32_Service
        //    ManagementObjectCollection objMOC = objMOS.Get();
        //    string UniqueId = String.Empty;
        //    foreach (ManagementObject objMO in objMOC)
        //    {
        //        if (UniqueId == String.Empty) // only return MAC Address from first card   
        //        {
        //            UniqueId = objMO["MacAddress"].ToString();
        //            break;
        //        }
        //        //objMO.Dispose();
        //    }

        //    UniqueId = UniqueId.Replace(":", "");
        //    return UniqueId;
        //}

        private void button_TellaFriend_Click(object sender, EventArgs e)
        {
            string url = "mailto:someone@somewhere.com?subject=I'm Using This App to Organize and Download Automatically my Torrents&body=It's MyTvShowsOrganizer at SourceForge.net . It's FreeWare, OpenSource, free of adWare and other Trashes. If you want it, Google MyTvShowOrganizer to open it's Web Page: https://sourceforge.net/projects/mytvshoworganizer/ and Download it. If not, continue with your endless suffering (lol). If you Want Some Tips just Text me.";

            Go2Web.OpenLink(url);
            // Go2Web.OpenLink("https://sourceforge.net/p/mytvshoworganizer/");
        }

        private void button_Talk2Programer_Click(object sender, EventArgs e)
        {
            string url = "mailto:mytvshoworganizer@gmail.com?subject=I'm using your App&body=THANKS!";

            Go2Web.OpenLink(url);
        }
        //private void button_Thanks_Click(object sender, EventArgs e)
        //{

        //    string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        //    char[] cArray = GetUserUniqueId().ToCharArray().Reverse().ToArray();
        //    string id = new string(GetUserUniqueId().Reverse().ToArray());


        //}

        private void Help_WebTranslator()
        {
            foreach (Control control in this.flowLayoutPanel_About.Controls)
            {
               
                control.ContextMenuStrip = this.contextMenuStrip_Translate;
       
                toolStripMenuItem_Translate.Click -= toolStripMenuItem_Translate_Click;
                toolStripMenuItem_Translate.Click += toolStripMenuItem_Translate_Click;
            }
        }
        

        private void toolStripMenuItem_Translate_Click(object sender, EventArgs e)
        {
            Control senderControl = contextMenuStrip_Translate.SourceControl;
            string toolTip = this.toolTip_Form_About.GetToolTip(senderControl);
            string title = senderControl.Text.Replace("&", "");

            //if (Form_Main.radioButton_BingTranslator.Checked)
            //{
            TranslationBox.Show(string.Format("[{0}]: {1}", title, toolTip), title, TranslationBox.WebTranslator.Google); // title  + ": " + toolTip , title );
            //}
            //else if (radioButton_GoogleTranslator.Checked)
            //{
            //    TranslateBox.Show(string.Format("[{0}]: {1}", title, toolTip), title, TranslateBox.WebTranslator.Google);
            //}

            //  TranslateBox.Show(string.Format("[{0}]: {1}", title, toolTip), title, webTranslator: TranslateBox.WebTranslator.Bing ); // title  + ": " + toolTip , title );

        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Parameters.FadeOut(this);
            this.Close();
        }

        private void button_Update_Click(object sender, EventArgs e)
        {
            AppUpdate.InstallUpdateSyncWithInfo();
        }

        private void button_Review_Click(object sender, EventArgs e)
        {
            Go2Web.OpenLink("http://sourceforge.net/projects/mytvshoworganizer/reviews?source=navbar");
        }

        private void button_Bugs_Click(object sender, EventArgs e)
        {
            Go2Web.OpenLink("https://sourceforge.net/p/mytvshoworganizer/wiki/Home/");
        }

        private void button_Donate_Click(object sender, EventArgs e)
        {
            Go2Web.OpenLink("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=M2JW3WWJV9ZKW");
        }

        private void button_WhatsNew_Click(object sender, EventArgs e)
        {
            Go2Web.OpenLink("https://sourceforge.net/projects/mytvshoworganizer/files/");
        }
              
    }
}
