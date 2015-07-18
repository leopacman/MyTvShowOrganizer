using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace MyTvShowsOrganizer
{
    public partial class Form_Main : Form
    {

        #region ##### Enter  Crtl+M M and Crtl+M Ctrl+L to Show Code #### Crtl - and Ctrl Shif - to forward and backward position  #############

        public Control myFocus;

        #endregion

        #region Main ############################################################################

        private SplashScreen splashScreen;
        private bool SSdone = false;
        BackgroundWorker CodHtmlWorker = new BackgroundWorker();
        private static Point lastMousePos;

        public Form_Main()
        {

            string iniShowsFile = Path.Combine(Parameters.myProgPath, "MyTvShowsOrganizer1.ini");
            string iniCommentFile = Path.Combine(Parameters.myProgPath, "MyTvShowsComments1.ini");

            if (!File.Exists(iniShowsFile)) //if not exists then this is the first run. firstrun() will create this file.
            {
                //if first run then load resource samples.
                Parameters.firstRun = true;
                FirstRun(iniShowsFile); //will open an intance of formMain to display disclaimer but firstrun will be
                Parameters.firstRun = false;
            }

            if (!Parameters.firstRun)  //when form1_main instance in FirstRun() is called (below) this will prevent splashscreen to be opened because firstRun will be true.
            {
                //preload info from internet and store it in globals variables while splashscreen is been shown.

                CodHtmlWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.CodHtmlWorker_DoWork);
                CodHtmlWorker.RunWorkerAsync();
                ////open splashScreen
                this.splashScreen = new SplashScreen();
                if (Parameters.showSplash) this.Load += new EventHandler(HandleFormLoadInitiateSplashScreen);
            }

            if (Environment.OSVersion.Platform != PlatformID.Win32NT && (Environment.OSVersion.Version.Major > 6 || (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 2)))
            {
                Application.EnableVisualStyles();  //only applies that if this is an OS newer than windows 8
            }   //will enable 3D effect on controls for newer OS versions

            InitializeComponent();

            SubtitlesLanguages();
            HandlersConstructors();
            autoCompleteResolution();

            ComboBox_Searcher.SelectedIndex = 0; //initiate with 0 in case first run.
            //tabcontrol will be hidden by createblindcontrol
            this.tabControl1.ItemSize = new Size(18, 1);

            //CloseIE CIE = new CloseIE();
            //System.Threading.Thread ThreadCloseeIe = new System.Threading.Thread(CIE.CloseAllIE);
            //ThreadCloseeIe.Start();

            Title_Text();

            LoadTabPage(iniShowsFile, "tabPage1");
            LoadComments(iniCommentFile);
            ObliqueComments();
            RadioButton1.Checked = true;
            AutoApplyTooltips();
            //to force deselect textbox1.
            textBox1.SelectAll();
            textBox1.DeselectAll();

            LoadTabPage("Config.ini", "tabPage5");

            //Parameters.webTranslatorOn = this.checkBox_TP5_TranslatorOn.Checked;
            Toggle_Translation(); //to applicate enables.
            // if (this.checkBox_TP5_TranslatorOn.Checked;) Help_WebTranslator_Contructor();

            if (checkBox_TP5_Backup.Checked && !Parameters.isDebugging) BackupSavedFiles();

            Parameters.MaxNumberOfPages = Convert.ToInt16(this.textBox_TP5_NofPages.Text);

            AddNewShow_DefineBrowserSize();

            // Move form windows using Panel_title mouse events with a form that has formborderstyle=none
            this.panel_Title.MouseDown += Panel_Title_MouseDown;
            this.panel_Title.MouseMove += Panel_Title_MouseMove;

        }

        private void autoCompleteResolution()
        {
            for (int showIndex = Parameters.resolutionColumn; showIndex <= Parameters.nTotTboxes; showIndex += Parameters.nOfColumns)
            {
                TextBox resolutionTBox = (TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + showIndex.ToString()];
                resolutionTBox.AutoCompleteCustomSource.AddRange(new string[] {
            "720p",
            "1080p",
            "Any"});
            }
        }

        private void Help_WebTranslator_TurnOn(Boolean TurnOn)
        {
            foreach (Control tabPage in tabControl1.Controls)
            {
                foreach (Control control in tabPage.Controls)
                {
                    if (!(control is FlowLayoutPanel))
                    {
                        if (control.Controls.Count == 0 && !(control is ListBox))
                        {
                            if (TurnOn)
                            {
                                control.ContextMenuStrip = this.contextMenuStrip_Translate;
                            }
                            else
                            {
                                control.ContextMenuStrip = null;
                            }
                        }
                        else
                        {
                            foreach (Control item in control.Controls)
                            {
                                if (item is Button || item is Label || item is CheckBox || item is RadioButton)
                                {
                                    if (!item.Name.Contains("button_GetTorrent"))  //not in GetTorrent button
                                    {

                                        if (TurnOn)
                                        {
                                            item.ContextMenuStrip = this.contextMenuStrip_Translate;
                                        }
                                        else
                                        {
                                            item.ContextMenuStrip = null;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (TurnOn)
            {
                toolStripMenuItem_Translate.Click -= toolStripMenuItem_Translate_Click;
                toolStripMenuItem_Translate.Click += toolStripMenuItem_Translate_Click;
            }
            else
            {
                toolStripMenuItem_Translate.Click -= toolStripMenuItem_Translate_Click;
            }
        }

        private void HandlersConstructors()
        {

            //eventHandlers contructions
            // this.control.CheckedChanged += new System.EventHandler(this.control_CheckedChanged);
            foreach (RadioButton rButton in this.flowLayoutPanel_RButtons.Controls)
            {
                rButton.CheckedChanged += RadioButton_CheckedChanged;
            }
            foreach (CheckBox cBox in this.flowLayoutPanel_Cboxes.Controls)
            {
                cBox.CheckedChanged += checkBox_CheckedChanged;
                this.ToolTip1.SetToolTip(cBox, "Mouse Right Button to Open Menu");
            }

            this.ToolTip1.SetToolTip(this.textBox81, "Mouse Right Button to ADD new Series");

            for (int showIndex = Parameters.showsColumn; showIndex <= Parameters.nTotTboxes; showIndex += Parameters.nOfColumns)
            {
                TextBox showTBox = (TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + showIndex.ToString()];
                showTBox.MouseDown += textBoxShows_MouseDown;
                this.ToolTip1.SetToolTip(showTBox, "Mouse Right Button to ADD new Series");
            }

        }

        private void toolStripMenuItem_Translate_Click(object sender, EventArgs e)
        {
            Control senderControl = contextMenuStrip_Translate.SourceControl;
            string toolTip = ToolTip1.GetToolTip(senderControl);
            string title = senderControl.Text.Replace("&", "");

            if (radioButton_BingTranslator.Checked)
            {
                TranslationBox.Show(string.Format("[{0}]: {1}", title, toolTip), title, TranslationBox.WebTranslator.Bing); // title  + ": " + toolTip , title );
            }
            else if (radioButton_GoogleTranslator.Checked)
            {
                TranslationBox.Show(string.Format("[{0}]: {1}", title, toolTip), title, TranslationBox.WebTranslator.Google);
            }

        }


        private void Title_Text()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                this.label_Title.Text = this.ProductName + " V." + AppUpdate.CurrentVer.ToString();
            }
            else
            {
                this.label_Title.Text = this.ProductName + " D." + "2000.00.00.0";
            }
            this.label_Title.Font = new Font("Trebuchet MS", 12F, FontStyle.Bold);
        }

        private void AddNewShow_DefineBrowserSize()
        {//to acomodate fullDH and SD monitors
            if (checkBox_TP5_BrowserSize.Checked)
            {
                Parameters.AddNewShowSizeTabControlSize = 1230;
                Parameters.AddNewShowWebBrowserSize = 1030;
                Parameters.AddNewShowWebBrowserPosition = 190;
            }
            else
            {
                Parameters.AddNewShowSizeTabControlSize = 1000;
                Parameters.AddNewShowWebBrowserSize = 798;
                Parameters.AddNewShowWebBrowserPosition = 184;
            }
        }

        private void CodHtmlWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Parameters.IsInternetConnectionOk = Parameters.IsInternetConnectionUp();
            if (Parameters.IsInternetConnectionOk)
            {
                if (!Parameters.isDebugging)
                {
                    Parameters.CodHtmlNextAiringStr = Parameters.CodHtmlNextAiringStringExtractor();
                    Parameters.CodHtmlStatusStr = Parameters.CodHtmlStatusStringExtractor();
                }
            }
            Parameters.CloseIex();
        }

        private void BackupSavedFiles()
        {
            string date2Name = DateTime.Today.ToString("ddd"); //weekday (seven backups)
            date2Name = Parameters.ClearStr(date2Name);
            string[] myFilesArray = Directory.GetFiles(Parameters.myProgPath, "*.ini");
            Directory.CreateDirectory(Parameters.myProgPath + "\\BackUp\\" + date2Name);
            foreach (string eachfile in myFilesArray)
            {
                File.Copy(eachfile, Parameters.myProgPath + "\\BackUp\\" + date2Name + "\\" + Path.GetFileName(eachfile), true);
            }
        }

        private void HandleFormLoadInitiateSplashScreen(object sender, EventArgs e)
        {
            this.Hide();
            Thread thread = new Thread(this.ShowSplashScreen);
            thread.Start();
            SplashScreenWorker SplashWorker = new SplashScreenWorker();

            SplashWorker.ProgressChanged += (o, ex) =>
            {
                this.splashScreen.UpdateProgress(ex.Progress);
            };
            SplashWorker.HardWorkDone += (o, ex) =>
            {
                SSdone = true;
                this.Opacity = 0.1;
                this.TopMost = true;
                this.Show();
                this.Activate();
                Parameters.FadeIn(this);
                if (!Parameters.isDebugging) Display_Next_airing();
                alwaysontop();
            };
            SplashWorker.DoHardWork();
        }

        private void ShowSplashScreen()
        {
            splashScreen.TopMost = true;
            splashScreen.Show();
            while (!SSdone)
            {
                Application.DoEvents();
            }
            splashScreen.Close();
            this.splashScreen.Dispose();
        }

        private void FirstRun(string iniFile)
        {
            //opens a temporary instance of mainform to show 'disclaim message' at firstRun. will run never again.
            string iniFileContents = MyTvShowsOrganizer.Properties.Resources.MyTvShowsOrganizer1;
            Directory.CreateDirectory(Parameters.myProgPath);
            File.Create(iniFile).Dispose();
            File.WriteAllText(iniFile, iniFileContents);

            Form_Main tempMainForm = new Form_Main();
            tempMainForm.Show();

            MsgBox.Show(@"This is the First Time you run MyTvShowsOrganizer.
          # To Translate The Purpose of any Control, Click it with Mouse Right Button
to Open Menu, or over the link [En ─› Ҩἒὧℓ₯] when reading a Message Box.
          # To Add new TV-Series Click with Mouse Right Button on a Free Spot (first Column).
          # To Download a Torrent, Click 'NextEpisode' (to mark episodes
          that are ready to download) and after, 'GetTorrent' button.
         
Be my Guest to warn about Bugs and make Suggestions at 
https://sourceforge.net/p/mytvshoworganizer/wiki/Home/.
Have you Fun!

If you have intention to download Torrents, remember to support
Tv-Series producers, subscribing at your local cable-tv or a
paid provider of on-demand media such as Netflix, cloudLoad,
Amazon Prime, itunes etc.", "What You Need to Know.", MsgBox.Buttons.OK, MsgBox.Icone.Info, MsgBox.AnimateStyle.FadeIn);

            Thread.Sleep(500);
            BlinkControl(tempMainForm.label_NextEpisode, 350, 3);
            Thread.Sleep(300);
            BlinkControl(tempMainForm.button_GetTorrent, 350, 3);
            Thread.Sleep(200);
            tempMainForm.Close();
            tempMainForm.Dispose();

        }

        //alternate close button X_Click
        protected override void OnFormClosing(FormClosingEventArgs e)
        {

            base.OnFormClosing(e);
         
            //Not e.Cancel AndAlso 
            if ((e.CloseReason > 0))
            {
                if (this.checkBox_TP5_SaveOnExit.Checked)
                {
                    SaveTabPage("MyTvShowsOrganizer" + label_NofPage.Text + ".ini", "tabPage1");
                }

            }
            Parameters.CloseIex(); //close hidden ie windows if opened end not closed because previous internet error.
        }

        //Close Button
        private void button_Close_Click(object sender, EventArgs e)
        {
            CloseThisProgram();
        }

        private void button_CloseX_Click(object sender, EventArgs e)
        {
            CloseThisProgram();
        }

        private void button_Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void CloseThisProgram()
        {
            if (CodHtmlWorker.IsBusy)
            {
                return;
            }
            else if (backgroundWorker_GetTorrent.IsBusy)
            {
                //killing Internet Explorer will force BackgrounDWorkerGettorrent to close.
                Parameters.isGetTorrentStoped = true;
                GetTorrent_Stop();
                return;
            }
            this.Close();
        }

        //to implement move FormMain window without formstyle none

        private void Panel_Title_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastMousePos = new Point(e.X, e.Y);
            }
        }

        private void Panel_Title_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastMousePos.X;
                this.Top += e.Y - lastMousePos.Y;
            }
        }

        private void TabControl1_SelectedIndexChanged(Object sender, EventArgs e)
        {  //implement autosize capability

            switch (tabControl1.SelectedIndex)
            {
                case 0: //main page

                    this.tabControl1.Size = new Size(877, 647);
                    this.panel_Title.Size = new Size(875, 34);
                    this.CenterToScreen();
                    Title_Text();
                    break;

                case 5://page 6  add new show

                    this.panel_Title.Size = new Size(Parameters.AddNewShowSizeTabControlSize, 34);
                    this.tabControl1.Size = new Size(Parameters.AddNewShowSizeTabControlSize, 647); //1000 1225
                    this.CenterToScreen();
                    Title_Text();
                    break;

                case 7:
                    this.panel_Title.Size = new Size(Parameters.AddNewShowSizeTabControlSize + 300, 34);
                    this.tabControl1.Size = new Size(Parameters.AddNewShowSizeTabControlSize + 300, 647); //1000 1225
                    Title_Text();
                    this.CenterToScreen();
                    break;

                case 6: //page 7 ranking
                    this.panel_Title.Size = new Size(596, 34);
                    this.tabControl1.Size = new Size(600, 647); //1000 1225
                    this.CenterToScreen();
                    Title_Text();
                    break;

                default: //other page
                    this.label_Title.Font = new Font("Trebuchet MS", 9.75F, FontStyle.Bold);
                    this.label_Title.Text = this.ProductName;
                    this.panel_Title.Size = new Size(271, 34);
                    this.tabControl1.Size = new Size(275, 647);
                    break;
            }

            Parameters.FadeIn(this);
        }

        #endregion Main

        #region Change tabPage fadein fadeout effects

        private void Go2Page(int page)
        {
            Parameters.FadeOut(this);
            tabControl1.SelectedIndex = page;
        }

        #endregion

        #region I & X (select/deselect and inverter) ###########################################################################


        private void button113_I_Click(object sender, EventArgs e)
        {
            //inverts seletion on checkedboxes
            if (backgroundWorker_GetTorrent.IsBusy) return;
            InvertChecked();
        }

        //UncheckAll_Click
        private void button_X_Click(object sender, EventArgs e)
        {
            if (backgroundWorker_GetTorrent.IsBusy) return;
            UncheckAll();
        }


        private void InvertChecked()
        {
            //invert only if isnotempty series
            int seriesTboxIndex;
            foreach (CheckBox Cbox in flowLayoutPanel_Cboxes.Controls)
            {
                seriesTboxIndex = Parameters.RowIndex2TboxIndex(Convert.ToInt16(Cbox.Text));
                TextBox seriesTbox = (TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + seriesTboxIndex.ToString()];

                if (!string.IsNullOrEmpty(seriesTbox.Text))
                {
                    if (Cbox.Checked)
                    {
                        Cbox.Checked = false;
                    }
                    else
                    {
                        Cbox.Checked = true;
                    }
                }
            }
        }

        private void UncheckAll()
        {
            foreach (CheckBox Cbox in flowLayoutPanel_Cboxes.Controls)
            {
                Cbox.Checked = false;
            }
        }


        #endregion

        #region Clear Rows data ##################################################################

        private void button_Clear_Rows_Click(object sender, EventArgs e)
        {
            if (backgroundWorker_GetTorrent.IsBusy) return;
            ClearRows();
        }


        private void ClearRows()
        {
            DialogResult resp = MsgBox.Show("Click Yes to Clear Marked Rows; Click No to Undo Last Action of cleaning IN THIS Page.", "Cleaning Data", MsgBox.Buttons.YesNoCancel);
            if (resp == DialogResult.Yes)
            {
                if (!IsAnycheckBox_Checked())
                {
                    MsgBox.Show("You need Mark some Checkboxes to Erase Data.", "Impossible to Continue", MsgBox.Buttons.OK);
                }
                else //textbox/row blank
                {
                    CleanSelectedRows();
                }
            }

            else if (resp == DialogResult.No)
            {
                if (File.Exists(Path.Combine(Parameters.myProgPath, "MyTvShowsOrganizer" + label_NofPage.Text + ".undo")))
                {
                    FileInfo my_file_info = new FileInfo(Path.Combine(Parameters.myProgPath, "MyTvShowsOrganizer" + label_NofPage.Text + ".undo"));
                    DateTime my_file_date_of_creation = my_file_info.CreationTime;
                    TimeSpan datediff = (DateTime.Now - my_file_date_of_creation).Duration();
                    DialogResult resp2 = MsgBox.Show(string.Format("The  Data to undo for this Page has {0:f2} hours. The Entire Page is going to be restored, as it was, before the last cleaning action. Do you Want continue even so?", datediff.TotalHours), "BE ADVISED!", MsgBox.Buttons.YesNo);
                    //no
                    if (resp2 == DialogResult.Yes)
                    {
                        undoclearrows();
                    }
                }
                else
                {
                    MsgBox.Show("There is no Undo Action for this Page!", "Sorry!", MsgBox.Buttons.OK);
                }
            }

        }

        private void undoclearrows()
        {
            //restore the respective Page copy and savebutton.click
            LoadTabPage("MyTvShowsOrganizer" + label_NofPage.Text + ".undo", "tabPage1");
        }

        private void CleanSelectedRows()
        {
            //clear data in checked rows
            File.Copy(Path.Combine(Parameters.myProgPath, "MyTvShowsOrganizer" + label_NofPage.Text + ".ini"), Path.Combine(Parameters.myProgPath, "MyTvShowsOrganizer" + label_NofPage.Text + ".undo"), true);
            FileInfo myfile = new FileInfo(Path.Combine(Parameters.myProgPath, "MyTvShowsOrganizer" + label_NofPage.Text + ".undo"));
            myfile.CreationTime = DateTime.Now;
            //to use on undo clear function

            foreach (Control CheckB in flowLayoutPanel_Cboxes.Controls)
            {
                CheckBox CheckBc = (CheckBox)CheckB;
                if (CheckBc.Checked)
                {
                    ClearThisRow(Convert.ToInt16(CheckBc.Text));
                }
            }
        }

        private void ClearThisRow(int rowIndex)
        {
            int TxtBoxCommentIndex = Parameters.RowIndex2TboxIndex(rowIndex);
            //limpando panel 1
            for (int x = 1; x <= Parameters.nOfColumns; x++)
            {
                ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(TxtBoxCommentIndex - 1 + x)]).Clear();
            }
            TextBoxesRow2Default(rowIndex);
            ToolTip1.SetToolTip((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(TxtBoxCommentIndex)], "Mouse Right Button to ADD new Show");
            ClearThisComment((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(TxtBoxCommentIndex)]);
            CheckBox cBox = (CheckBox)flowLayoutPanel_Cboxes.Controls["checkBox" + Convert.ToString(rowIndex)];
            cBox.Checked = false;
        }


        #endregion

        #region Page Changing ###########################################################


        //ChangePageUp_Click
        private void button_PageUp_Click(object sender, EventArgs e)
        {

            if (backgroundWorker_GetTorrent.IsBusy) return;
            int Page = Convert.ToInt16(label_NofPage.Text);
            LoadNextPage(1, Page);
            RestoreColorRows(this.flowLayoutPanel_Tboxes);
            LoadComments(ThisCommentFile());
            ObliqueComments();
            AutoApplyTooltips();
        }

        //ChangePageDown_Click
        private void button_PageDown_Click(object sender, EventArgs e)
        {
            if (backgroundWorker_GetTorrent.IsBusy) return;
            int Page = Convert.ToInt16(label_NofPage.Text);
            LoadNextPage(-1, Page);
            RestoreColorRows(flowLayoutPanel_Tboxes);
            LoadComments(ThisCommentFile());
            ObliqueComments();
            AutoApplyTooltips();
        }

        private void ObliqueComments()
        {
            //apply italic to fonts on texboxes that have comments
            for (int showIndex = 1; showIndex <= Parameters.nTotTboxes; showIndex += Parameters.nOfColumns)
            {
                TextBox mytxtbox = (TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(showIndex)];
                string myTooltip = ToolTip1.GetToolTip(mytxtbox);
                if (myTooltip != "" && !myTooltip.Contains("ADD"))
                {
                    if (mytxtbox.Font.Style != FontStyle.Italic)
                    {
                        mytxtbox.Font = new Font("Trebuchet MS", 9f, FontStyle.Bold | FontStyle.Italic);
                        mytxtbox.Refresh();
                    }
                }
                else if (mytxtbox.Font.Style != FontStyle.Regular)
                {
                    mytxtbox.Font = new Font("Trebuchet MS", 9f, FontStyle.Bold);
                    mytxtbox.Refresh();
                }
            }
        }

        private void AutoApplyTooltips()
        {
            //apply tooltips with same value of .text over Next Episode Column
            TextBox seriesTBox;
            for (int textboxIndex = Parameters.nextEpisodeColumn; textboxIndex <= Parameters.nTotTboxes; textboxIndex += Parameters.nOfColumns)
            {
                seriesTBox = (TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(textboxIndex)];
                ToolTip1.SetToolTip(seriesTBox, seriesTBox.Text);
            }
        }

        private void RestoreColorRows(FlowLayoutPanel mypanel)
        {
            if (true)
            {

            }
            foreach (Control myControl in mypanel.Controls)
            {
                TextBox myTbox = (TextBox)myControl;
                if (myTbox.ForeColor != Colours.showsFont)
                {
                    myTbox.ForeColor = Colours.showsFont;
                    myTbox.Font = new Font("Trebuchet MS", 9f, FontStyle.Bold);
                }

            }
        }

        private string ThisCommentFile()
        {
            return "MyTvShowsComments" + label_NofPage.Text + ".ini";
        }

        private void LoadNextPage(int increment, int Page)
        {
            SaveTabPage("MyTvShowsOrganizer" + Convert.ToString(Page) + ".ini", "tabPage1");
            if (increment == 1 && Page == Parameters.MaxNumberOfPages)
            {
                label_NofPage.Text = "1";
            }
            else if (increment == -1 && Page == 1)
            {
                label_NofPage.Text = Parameters.MaxNumberOfPages.ToString();
            }
            else
            {
                label_NofPage.Text = Convert.ToString(Page + increment);
            }
            if (File.Exists(Path.Combine(Parameters.myProgPath, "MyTvShowsOrganizer" + label_NofPage.Text + ".ini")))
            {
                LoadTabPage("MyTvShowsOrganizer" + label_NofPage.Text + ".ini", "tabPage1");
            }
            else
            {
                LoadBlanks();
            }
        }


        #endregion

        #region Up Down Row data  ##############################################################################

        //uprow  UpRow_Clic

        private void button_Up_Click(object sender, EventArgs e)
        {

            if (backgroundWorker_GetTorrent.IsBusy) return;
            UpDownRow(myFocus, 1);
            myFocus = ActiveControl;
        }

        //downrow DownRow_Click
        private void button_down_Click(object sender, EventArgs e)
        {
            if (backgroundWorker_GetTorrent.IsBusy) return;
            UpDownRow(myFocus, -1);
            myFocus = ActiveControl;
        }

        private void UpDownRow(Control myfocus, int UpDown)
        {
            //Control myfocus2 = (Control)myFocus;
            int myrow = Convert.ToInt16(myfocus.Text);
            TextBoxesRow2Default(myrow);
            //Invert rows
            UpDown_Rows(myfocus, UpDown);
            //Invert comments
            UpDows_comment(myfocus, UpDown);
        }

        //radiobutton  up=1 down=-1
        private void UpDown_Rows(Control MyFocusedShow, int up_down)
        {
            int my_row_index = Convert.ToInt16(MyFocusedShow.Text);
            //if limit rows (1,20) then
            if (up_down == 1 && my_row_index == 1)
            {
                up_down = -Parameters.nOfRows + 1;
            }
            else if (up_down == -1 && my_row_index == Parameters.nOfRows)
            {
                up_down = Parameters.nOfRows - 1;
            }
            //up row - elevates a row
            int my_index_txtbox = 0;
            string[] mylist = new string[Parameters.nOfColumns + 2];
            //textboxes + 1 checkbox
            int y = 0;
            MyFocusedShow = (RadioButton)MyFocusedShow;
            //store values from marked row
            my_index_txtbox = Parameters.RowIndex2TboxIndex(my_row_index);
            y = 0;
            for (int x = 1; x <= Parameters.nOfColumns; x++)
            {
                mylist[y] = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(my_index_txtbox + y)]).Text;
                y += 1;
            }
            mylist[y + 1] = Convert.ToString(((CheckBox)flowLayoutPanel_Cboxes.Controls["Checkbox" + Convert.ToString(my_row_index)]).Checked);
            y = 0;
            for (int x = 1; x <= Parameters.nOfColumns; x++)
            {
                ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(my_index_txtbox + y)]).Text = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(my_index_txtbox + y - (up_down * Parameters.nOfColumns))]).Text;
                //-  Parameters.nTextBoxPerLinepanel1
                y = y + 1;
            }
            ((CheckBox)flowLayoutPanel_Cboxes.Controls["Checkbox" + Convert.ToString(my_row_index)]).Checked = ((CheckBox)flowLayoutPanel_Cboxes.Controls["Checkbox" + Convert.ToString(my_row_index - up_down)]).Checked;
            y = 0;
            for (int x = 1; x <= Parameters.nOfColumns; x++)
            {
                ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(my_index_txtbox + y - (up_down * Parameters.nOfColumns))]).Text = mylist[y];
                y = y + 1;
            }
            ((CheckBox)flowLayoutPanel_Cboxes.Controls["Checkbox" + Convert.ToString(my_row_index - up_down)]).Checked = Convert.ToBoolean(mylist[y + 1]);
            ((RadioButton)flowLayoutPanel_RButtons.Controls["RadioButton" + Convert.ToString(my_row_index - up_down)]).Checked = true;
            ((RadioButton)flowLayoutPanel_RButtons.Controls["RadioButton" + Convert.ToString(my_row_index - up_down)]).Focus();
            AutoApplyTooltips();
        }

        private void UpDows_comment(Control myfocus, int up_down)
        {
            int indexrow = Convert.ToInt16(myfocus.Text);
            int IndexTboxRow = Parameters.RowIndex2TboxIndex(indexrow);
            string commentStr = ToolTip1.GetToolTip((Control)flowLayoutPanel_Tboxes.Controls["textBox" + IndexTboxRow]);
            string indexTextRowUp = Convert.ToString(IndexTboxRow - (up_down * Parameters.nOfColumns));
            string commentRowUp = ToolTip1.GetToolTip((Control)flowLayoutPanel_Tboxes.Controls["textBox" + indexTextRowUp]);
            //Morgan Law to ==== (Not commentrow = "" OrElse Not commentrowup = "") 
            if (!(string.IsNullOrEmpty(commentStr) && string.IsNullOrEmpty(commentRowUp)))
            {
                SaveComment("MyTvShowsComments" + label_NofPage.Text + ".ini", IndexTboxRow.ToString(), commentRowUp);
                SaveComment("MyTvShowsComments" + label_NofPage.Text + ".ini", indexTextRowUp, commentStr);
                ToolTip1.SetToolTip((Control)flowLayoutPanel_Tboxes.Controls["textBox" + indexTextRowUp], commentStr);
                ToolTip1.SetToolTip((Control)flowLayoutPanel_Tboxes.Controls["textBox" + IndexTboxRow], commentRowUp);
            }
            ObliqueComments();
        }


        #endregion

        #region radioButtons Handles #################################################################


        //will handles radiobuttons handles


        private void RadioButtonColorChanger(object mysender, bool senderCheckState)
        {
            myFocus = (Control)mysender;
            ChangeColorRow(myFocus, senderCheckState);
        }


        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButtonColorChanger(sender, ((RadioButton)sender).Checked);
        }


        #endregion

        #region CheckBoxes Handles  ####################################################################################






        //handles checkbox handles
        private void CheckboxColorChanger(object mysender, bool senderval)
        {
            ChangeColorRow(mysender, senderval);
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckboxColorChanger(sender, ((CheckBox)sender).Checked);
        }



        #endregion

        #region Get Subtitles ############################################

        //will open one subtitle page for checked series at subtitle site.

        private void toolStripMenuItem_Subtitle_Click(object sender, EventArgs e)
        {

            UncheckAll();
            // Control objFocus = (Control)myFocus;
            int myRow = Convert.ToInt16(myFocus.Text);
            CheckBox selected = (CheckBox)flowLayoutPanel_Cboxes.Controls[myRow - 1];
            selected.Checked = true;
            GetSubtitles();
        }

        private void Button103_GetSubtitle_Click(object sender, EventArgs e)
        {
            GetSubtitles();
        }

        private void GetSubtitles()
        {

            if (IsAnycheckBox_Checked())
            {
                //Color IniColor = button_GetSubtitle.BackColor;
                button_GetSubtitle.BackColor = Colours.buttonsBusyBackGround;
                button_GetSubtitle.Refresh();

                List<string> tbxSeries = new List<string>();
                List<string> tbxSeasons = new List<string>();
                // List<string> tbxEpisodes = new List<string>();
                int indexTbox;
                // string language = comboBox_TP5_Language.SelectedText;
                foreach (CheckBox checkedbox in flowLayoutPanel_Cboxes.Controls)
                {

                    if (checkedbox.Checked)
                    {
                        indexTbox = Parameters.RowIndex2TboxIndex(Convert.ToInt16(checkedbox.Text));
                        //in checkedbox.text there is a hidden number representing the row
                        tbxSeries.Add(((TextBox)flowLayoutPanel_Tboxes.Controls["TextBox" + Convert.ToString(indexTbox)]).Text);

                        string season = ((TextBox)flowLayoutPanel_Tboxes.Controls["TextBox" + Convert.ToString(indexTbox + Parameters.seasonColumn - 1)]).Text;
                        if (((TextBox)flowLayoutPanel_Tboxes.Controls["TextBox" + Convert.ToString(indexTbox + Parameters.nextEpisodeColumn - 1)]).Text.Contains("Waiting"))
                        {

                            season = (Convert.ToInt16(season) - 1).ToString();
                            tbxSeasons.Add(season);
                        }
                        else
                        {
                            tbxSeasons.Add(season);
                        }

                        //  tbxEpisodes.Add(((TextBox)flowLayoutPanel_Tboxes.Controls["TextBox" + Convert.ToString(indexTbox + Parameters.episodeColumn - 1)]).Text);
                    }
                }

                if (tbxSeries.Count > 0)
                {
                    Go2Web.GetMySubtitle_bsplayerSite(tbxSeries, tbxSeasons, comboBox_TP5_Language.SelectedIndex);
                }
                button_GetSubtitle.BackColor = Colours.buttonBackGround;
            }
            else
            {
                MsgBox.Show("Select some TV-Shows and try again.", "No Tv-Show is Checked", MsgBox.Buttons.OK, MsgBox.Icone.Exclamation, MsgBox.AnimateStyle.FadeIn);
            }
        }

        #endregion

        #region Move Data Between Pages  ##########################################################################

        //MoveBetweenPages_Click
        private void button_Move_Click(object sender, EventArgs e)
        {
            if (backgroundWorker_GetTorrent.IsBusy) return;

            if (IsAnycheckBox_Checked())
            {
                Move_Page_Constructor();
                GetReady2MoveDataBetweenPages();
            }

            else
            {
                MsgBox.Show("No Series is Marked to be moved! Mark some of them and try again.");
            }
        }


        private void toolStripMenuItem_MoveShow_Click(object sender, EventArgs e)
        {
            if (backgroundWorker_GetTorrent.IsBusy) return;
            Move_Page_Constructor();

            UncheckAll();
            //Control objFocus = (Control)myFocus;
            int myRow = Convert.ToInt16(myFocus.Text);
            CheckBox selected = (CheckBox)flowLayoutPanel_Cboxes.Controls[myRow - 1];
            selected.Checked = true;
            GetReady2MoveDataBetweenPages();


        }


        private void GetReady2MoveDataBetweenPages()
        {
            //getting ready to move data 
            string toBeMovedFile;
            int row2BeMoved = 0;
            int mytxtboxIndex = 0;
            int nchecked = 0;
            string mycomment;
            TextBox mycommenttxtbox;
            int nextTxtBoxIndex = 0;
            TextBox textBoxP1;
            //copy data that gonna be moved
            toBeMovedFile = Path.Combine(Parameters.myProgPath, "mytvshowsorganizermove.undo");
            string originalFile = "MyTvShowsOrganizer" + label_NofPage.Text + ".ini";
            //save before changing
            SaveTabPage(originalFile, "tabPage1");

            if (File.Exists(toBeMovedFile))
            {
                File.Delete(toBeMovedFile);
            }

            File.Create(toBeMovedFile).Dispose();
            foreach (CheckBox markedcheckBox in flowLayoutPanel_Cboxes.Controls)
            {
                if (markedcheckBox.Checked)
                {
                    nchecked += 1;
                    row2BeMoved = Convert.ToInt16(markedcheckBox.Text);
                    mytxtboxIndex = Parameters.RowIndex2TboxIndex(row2BeMoved);
                    nextTxtBoxIndex = mytxtboxIndex;

                    //all textboxes in a row.
                    for (int col = 1; col <= Parameters.nOfColumns; col++)
                    {
                        textBoxP1 = (TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(nextTxtBoxIndex)];
                        File.AppendAllText(toBeMovedFile, textBoxP1.Text + "#");
                        nextTxtBoxIndex += 1;
                    }
                    mycommenttxtbox = (TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(mytxtboxIndex)];
                    mycomment = ToolTip1.GetToolTip(mycommenttxtbox);

                    File.AppendAllText(toBeMovedFile, mycomment + "#");
                    if (!string.IsNullOrEmpty(mycomment))
                    {
                        //clear original comment
                        SaveComment("MyTvShowsComments" + label_NofPage.Text + ".ini", Convert.ToString(mytxtboxIndex), "None");
                        ToolTip1.SetToolTip(mycommenttxtbox, "");
                    }
                }
            }

            if (nchecked > 0)
            {
                //register number of rows moved
                File.AppendAllText(toBeMovedFile, "@" + Convert.ToString(nchecked));
                CleanSelectedRows();
                SaveTabPage(originalFile, "tabPage1");
                label_TP2_PageN.Text = "1";
                string savedString = "";
                button_TP2_Move.Enabled = false;
                if (File.Exists(Path.Combine(Parameters.myProgPath, "MyTvShowsOrganizer" + label_TP2_PageN.Text + ".ini")))
                {
                    LoadPartialArq("MyTvShowsOrganizer" + label_TP2_PageN.Text + ".ini");
                    button_TP2_Move.Enabled = true;
                }
                savedString = File.ReadAllText(toBeMovedFile);
                label_TP2_Items2BeMovedN.Text = savedString.Substring(savedString.IndexOf("@") + 1);
                label_TP2_ItemsToBeAllocatedN.Text = label_TP2_Items2BeMovedN.Text;
                savedString = null;


                Go2Page(1);



            }
        }



        #endregion

        #region NotifyIcon handles ########################################################


        private void notifyIcon_Minimize_to_Tray(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized && checkBox_TP5_MinimizeToTray.Checked)
            {
                NotifyIcon1.Visible = true;
                NotifyIcon1.ShowBalloonTip(3000);
                this.ShowInTaskbar = false;
            }
        }

        private void NotifyIcon_ShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            NotifyIcon1.Visible = false;
        }

        private void NotifyIcon_CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            NotifyIcon1.Visible = false;
        }



        #endregion

        #region Clear All comments in selected Checkboxes ######################################################
        
        //Clear_All_Comments_Click
        private void button107_Clr_Comment_Click(object sender, EventArgs e)
        {
            Clear_All_Comments(true);
        }
        
        private void Clear_All_Comments(bool alert)
        {
            //Will delete the file containing commentaries from this page
            DialogResult vbresult = DialogResult.OK;
            if (alert)
            {
                vbresult = MsgBox.Show("This is going to delete all comments in this Page. Continue?", "Atention", MsgBox.Buttons.OKCancel);
            }

            if (vbresult == DialogResult.OK)
            {
                string file2delete = Path.Combine(Parameters.myProgPath, "MyTvShowsComments" + label_NofPage.Text + ".ini");
                if (File.Exists(file2delete))
                {
                    File.Delete(file2delete);
                    SaveBlanksComments(file2delete);
                    LoadComments(file2delete);
                    ObliqueComments();
                }
            }
        }

        #endregion

        #region contextmenustriper Textbox of Show names GoogleIt ##########################################

        private void ContextMenuStrip_TxtBox_Shows_Opening(object sender, CancelEventArgs e)
        {
            //before open menu shows only applicable commands (empty vs full)
            TextBox seriesTBox;
            seriesTBox = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;

            foreach (ToolStripItem menuItem in ContextMenuStrip_TxtBox_Shows.Items)
            {
                switch (string.IsNullOrEmpty(seriesTBox.Text))
                {
                    case true: //free spot only shows ADD adn Paste
                        if (menuItem.AccessibleDescription == "show" || menuItem.AccessibleDescription == "both")
                        {
                            menuItem.Visible = true;
                        }
                        else
                        {
                            menuItem.Visible = false;
                        }
                        break;

                    case false: //not free spot shows the rest
                        if (string.IsNullOrEmpty(menuItem.AccessibleDescription) || menuItem.AccessibleDescription == "both")
                        {
                            menuItem.Visible = true;
                        }
                        //else if (menuItem.AccessibleDescription == "LastEp")
                        //{
                        //    menuItem.Visible = false; //true
                        //    //menuItem.Text = Get_LastSeasonLastEp(seriesTBox);
                        //}
                        else
                        {
                            menuItem.Visible = false;
                        }
                        break;
                }
            }
        }

        private void ToolStripMenuItem_InsertComment_Click(object sender, EventArgs e)
        {

            Comments_Page_Constructor();
            TextBox seriesTBox;
            seriesTBox = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;
            int tBoxIndex = Parameters.RowIndex2TboxIndex(Convert.ToInt16(myFocus.Text));
            ((RadioButton)flowLayoutPanel_RButtons.Controls["RadioButton" + Convert.ToString(Parameters.TboxIndex2RowIndex(tBoxIndex))]).Checked = true;

            CommentInsertionBox(seriesTBox, tBoxIndex);
        }

        private void ToolStripMenuItem_ClearComment_Click(object sender, EventArgs e)
        {
            TextBox seriesTBox;
            seriesTBox = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;
            ClearThisComment(seriesTBox);
        }

        private void ClearThisComment(TextBox seriesTBox)
        {
            if (ToolTip1.GetToolTip(seriesTBox) != null)
            {
                int tBoxIndex = Convert.ToInt16(seriesTBox.Name.Substring(7));
                //ToolTip1.SetToolTip(seriesTBox, null);

                SaveComment("MyTvShowsComments" + label_NofPage.Text + ".ini", Convert.ToString(tBoxIndex), "");
            }
        }

        private void toolStripMenuItem_ClearData_Click(object sender, EventArgs e)
        {
            if (backgroundWorker_GetTorrent.IsBusy) return;

            //Control objFocus = (Control)myFocus;
            int rowIndex = Convert.ToInt16(myFocus.Text);

            TextBox tBox = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;

            ClearThisRow(rowIndex);
            ClearThisComment(tBox);
            ObliqueComments();
        }

        private void ToolStripMenuItem_ViewSummary_Click(object sender, EventArgs e)
        {
            TextBox tBoxShowsName = null;
            tBoxShowsName = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;
            Go2Web.GoogleIt("www.pogdesign.co.uk", tBoxShowsName.Text, "summary");
        }

        private void toolStripMenu_Trailer_Click(object sender, EventArgs e)
        {
            TextBox tBoxShowsName = null;
            tBoxShowsName = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;

            string codhtml = Parameters.CodHtmlGoogle2AnySite("https://www.google.com/search?q=" + "site:youtube.com" + "+" + "%22" + tBoxShowsName.Text + "%22" + "+Trailer+Official");
            //www.youtube.com/watch?v=xdm7Z3TQhDg
            string myPattern = @"www.youtube.com/watch\?v=\w{11}";
            string finalHyper = Regex.Match(codhtml, myPattern).Value;
            Go2Web.OpenLink(finalHyper); //only sites with a good regex patters will work with codhtmlgoogle2anysite (direct approach)
            // Go2Web.GoogleIt("www.youtube.com", tBoxShowsName.Text, "trailer official");
        }

        private void ToolStripMenuItem_Imdb_Click(object sender, EventArgs e)
        {
            TextBox tBoxShowsName = null;
            tBoxShowsName = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;

            string codhtml = Parameters.CodHtmlGoogle2AnySite("https://www.google.com/search?q=" + "site:imdb.com" + "+" + "%22" + tBoxShowsName.Text + "%22" + "+ Tv Series");
            string myPattern = @"www.imdb.com/title/tt[0-9]{7}/";

            string finalHyper = Regex.Match(codhtml, myPattern).Value;
            // www.imdb.com/title/tt0319931 /
            Go2Web.OpenLink(finalHyper);

            //Go2Web.GoogleIt("www.imdb.com", tBoxShowsName.Text, "Tv Series");
        }

        private void toolStripMenuItem_Predictions_Click(object sender, EventArgs e)
        {

            ShowsPrectition(false);

        }


        private void ToolStripMenuItem_Wikipedia_Click(object sender, EventArgs e)
        {
            TextBox tBoxShowsName = null;
            tBoxShowsName = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;
            Go2Web.GoogleIt("wikipedia.org", tBoxShowsName.Text, "Tv Series");
        }

        private void ToolStripMenuItem_TvRage_Click(object sender, EventArgs e)
        {
            TextBox tBoxShowsName = null;
            tBoxShowsName = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;
            Go2Web.GoogleIt("www.tvrage.com", tBoxShowsName.Text, "tv");
        }

        private void ToolStripMenuItem_RottenTomatoes_Click(object sender, EventArgs e)
        {
            TextBox tBoxShowsName = null;
            tBoxShowsName = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;
            Go2Web.GoogleIt("www.rottentomatoes.com", tBoxShowsName.Text);
        }

        private void toolStripMenuItem_EpGuides_Click(object sender, EventArgs e)
        {
            TextBox tBoxShowsName = null;
            tBoxShowsName = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;
            Go2Web.GoogleIt("www.epguides.com", tBoxShowsName.Text);
        }

        private void ToolStripMenuItem_Tvcom_Click(object sender, EventArgs e)
        {
            TextBox tBoxShowsName = null;
            tBoxShowsName = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;
            Go2Web.GoogleIt("www.tv.com", tBoxShowsName.Text, "episodes");
        }


        private void ToolStripMenuItem_TorrentZ_Click(object sender, EventArgs e)
        {
            if (!Parameters.IsInternetConnectionOk)
            {
                Parameters.InternetBadMessage();
            }
            else
            {
                //https://torrentz.eu/searchA?f=Marvel+s+Agents+of+S.H.I.E.L.D+S02%2A+720p

                //int rowIndex = Convert.ToInt16(myFocus.Text);
                TextBox TBoxShowsName;
                TBoxShowsName = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;
                //string season = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + (Parameters.RowIndex2TboxIndex(rowIndex) + Parameters.seasonColumn-1).ToString()]).Text;

                //season = season.PadLeft(2, "0");

                string mySerie = TBoxShowsName.Text;

                mySerie = Parameters.ClearStr(mySerie);
                mySerie = mySerie.Replace(" ", "+"); //quote for space works fine on thepiratebay
                Go2Web.OpenLink("https://torrentz.eu/searchA?f=" + mySerie + "+720p+|+1080p+|+hdtv"); //"+S" + season
            }
        }

        private void ToolStripMenuItem_thePirateBay_Click(object sender, EventArgs e)
        {
            if (!Parameters.IsInternetConnectionOk)
            {
                Parameters.InternetBadMessage();
            }
            else
            {
                //https://thepiratebay.cr/search/the%20walking%20dead%20s02*/0/7/0

                //int rowIndex = Convert.ToInt16(myFocus.Text);
                TextBox TBoxShowsName;
                TBoxShowsName = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;
                //string season = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + (Parameters.RowIndex2TboxIndex(rowIndex) + Parameters.seasonColumn - 1).ToString()]).Text;
                //season = season.PadLeft(2, '0');
                string mySerie = TBoxShowsName.Text;
                mySerie = Parameters.ClearStr(mySerie);
                // mySerie = mySerie.Replace(" ", "%20");//quote for space works fine on thepiratebay
                Go2Web.OpenLink("https://thepiratebay.cr/search/" + mySerie + " S0*" + "/0/7/0"); //+ "720p""S" + season + "*" +
            }
        }


        private void ToolStripMenuItem_KickAss_Click(object sender, EventArgs e)
        {

            if (!Parameters.IsInternetConnectionOk)
            {
                Parameters.InternetBadMessage();
            }
            else
            {
                //+ mySerie2 + "%20" + mySE + "%20" + myResolution + "%20" + myPlusString + "%20category%3Atv/?field=seeders&sorder=desc"
                //https://kat.cr/usearch/walking%20dead%20s03*%20720p/
                TextBox TBoxShowsName;

                TBoxShowsName = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;
                // string season = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + (Parameters.RowIndex2TboxIndex(rowIndex) + Parameters.seasonColumn - 1).ToString()]).Text;

                //season = season.PadLeft(2, '0');

                string mySerie = TBoxShowsName.Text;

                mySerie = Parameters.ClearStr(mySerie);
                // mySerie = mySerie.Replace(" ", "%20"); //quote for space works fine on thepiratebay
                Go2Web.OpenLink("https://kat.cr/usearch/" + mySerie + " 720p OR 1080p OR HDTV" + "/?field=time_add&sorder=desc");
            }
        }

        private void ToolStripMenuItem_InitializeNewSeason_Click(object sender, EventArgs e)
        {
            //will add +1 at season column and 1 on episode. Will append in comments the PreviousSeasonLastDownload for future reference. Resets to new next season.
            DialogResult vbresult = DialogResult.OK;
            vbresult = MsgBox.Show("Do you Want Manually Set this show to next season? This is going to set Series to the Next Season: S0xE01.", "Season is Over?", MsgBox.Buttons.OKCancel, MsgBox.Icone.Question, MsgBox.AnimateStyle.SlideDown);
            if (vbresult == DialogResult.OK)
            {
                TextBox seriesTxtBox;
                seriesTxtBox = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;
                InitializeNewSeason(seriesTxtBox);
            }
        }

        private void InitializeNewSeason(TextBox seriesTxtBox)
        {

            // int rowIndex = Convert.ToInt16(myFocus.Text); //not here
            int TboxShowIndex = Convert.ToInt16(seriesTxtBox.Name.Substring(7));//Parameters.RowIndex2TboxIndex(rowIndex); not here
            TextBox seasonTbox = (TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(TboxShowIndex + Parameters.seasonColumn - 1)];
            TextBox lastDownloadTbox = (TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(TboxShowIndex + Parameters.lastDownloadColumn - 1)];
            int season = Convert.ToInt16(seasonTbox.Text);

            seasonTbox.Text = Convert.ToString(season + 1);//
            ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(TboxShowIndex + Parameters.episodeColumn - 1)]).Text = "1";
            ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(TboxShowIndex + Parameters.nextEpisodeColumn - 1)]).Text = "E1" + " .WaitingNextSeason";
            ((RadioButton)flowLayoutPanel_RButtons.Controls["RadioButton" + Convert.ToString(Parameters.TboxIndex2RowIndex(TboxShowIndex))]).Checked = true;
            string mycommentfiledestiny = "MyTvShowsComments" + label_NofPage.Text + ".ini";
            SaveComment(mycommentfiledestiny, TboxShowIndex.ToString(), Get_LastSeasonLastEp(lastDownloadTbox.Text), appendit: true);//, appendit: true
        }

        private String Get_LastSeasonLastEp(string lastDownloadEp)
        {
            string seriesLastEpisode;
            {
                seriesLastEpisode = Regex.Match(lastDownloadEp, @"(?<= S[0-9]{2}E)[0-9]{2}(?=\|){1}", RegexOptions.IgnoreCase).Value;
                seriesLastEpisode = "Episodes of Last Season: " + seriesLastEpisode;
            }

            return seriesLastEpisode;
        }

        private void ToolStripMenuItem_PasteFromClipBoard_Click(object sender, EventArgs e)
        {
            TextBox seriesTxtBox;
            seriesTxtBox = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;
            seriesTxtBox.Text = Clipboard.GetText();
        }


        #endregion

        #region Classify Rows By name and status ##################################################################


        //Classify_Click
        private void label_TvShow_Classify_Click(object sender, EventArgs e)
        { //classify tabpage.
            if (backgroundWorker_GetTorrent.IsBusy || CodHtmlWorker.IsBusy) return;
            Color labelColor = label_TvShow.BackColor;
            label_TvShow.BackColor = Colours.buttonsBusyBackGround;
            label_TvShow.Refresh();
            Thread.Sleep(100);
            string SavedFile = Path.Combine(Parameters.myProgPath, "MyTvShowsOrganizer" + label_NofPage.Text + ".ini");
            if (File.Exists(SavedFile))
            {
                if (Parameters.IsInternetConnectionOk)
                {
                    CheckShowStatus();
                }
                else if (!this.radioButton_TP5_NameOnly.Checked) //Name only option in preferences. status must be checked but internet is offline
                {
                    Parameters.InternetBadMessage();
                }

                Classify_TabPage(Parameters.showsColumn, Path.Combine(Parameters.myProgPath, "MyTvShowsComments" + label_NofPage.Text + ".ini"), SavedFile);
                AutoApplyTooltips();
            }
            label_TvShow.BackColor = labelColor;
        }


        private void Classify_TabPage(int column, string SavedComment, string SavedFile)
        {

            //create/delete old file (ini). Creates a sorteddictionary of old order (show, row). recreate file .ini with new order. 

            File.Delete(SavedFile);
            File.Create(SavedFile).Dispose();
            SortedDictionary<string, int> colListHead = new SortedDictionary<string, int>();
            SortedDictionary<string, int> colListTail = new SortedDictionary<string, int>();
            int firstColumnRowIndex = 0;
            string statusColumnRowTxt = null;
            TextBox seriesTBox;
            string commentValue = null;
            Dictionary<string, string> commentList = new Dictionary<string, string>();

            string commentFileContent = File.ReadAllText(SavedComment);

            string[] commentStringList = Regex.Split(commentFileContent, "###");
            //string tBoxIndex;
            string[] dicComments;
            foreach (string myItem in commentStringList)
            {
                if (!string.IsNullOrEmpty(myItem))
                {
                    dicComments = Regex.Split(myItem, "=");
                    //seriesTBox = ((TextBox)flowLayoutPanel_Tbxes.Controls["textBox" + dicComments[0]]);
                    commentList.Add(dicComments[0], dicComments[1]);
                }
            }


            for (int textBoxIndex = column; textBoxIndex <= Parameters.nTotTboxes; textBoxIndex += Parameters.nOfColumns)
            {
                //columns is not necessarilly the first
                //firstcolumnrowIndex returns rowIndex despite the column in same row.
                firstColumnRowIndex = (int)Math.Truncate((decimal)((textBoxIndex - 1) / Parameters.nOfColumns)) * Parameters.nOfColumns + 1;
                //collist contem o valor da textbox da columna a ser ordenada e o número da primeira textbox da linha correspondente
                seriesTBox = (TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(textBoxIndex)];
                if (!string.IsNullOrEmpty(seriesTBox.Text))
                {
                    commentValue = ToolTip1.GetToolTip(seriesTBox);

                    if (!string.IsNullOrEmpty(commentValue))
                    {
                        try
                        {
                            commentList.Add(seriesTBox.Text, commentValue);
                        }
                        catch (Exception)
                        {
                            //throw;
                        }

                    }
                    statusColumnRowTxt = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(firstColumnRowIndex + Parameters.statusColumn - 1)]).Text;
                    //Returns or Returning (beggining of new order)


                    try //duplicate series name will cause error.
                    {
                        switch (this.radioButton_TP5_NameOnly.Checked)
                        {
                            case true:

                                colListHead.Add(seriesTBox.Text, firstColumnRowIndex);
                                break;

                            case false:
                                if (statusColumnRowTxt.Contains("eturn"))  //Return and return
                                {
                                    colListHead.Add(seriesTBox.Text, firstColumnRowIndex);
                                }
                                else
                                {
                                    colListTail.Add(seriesTBox.Text, firstColumnRowIndex);
                                    //(ending of new order)
                                }
                                break;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            int m = 1;
            //string oldValue = null;

            //clear old comments position


            m = OrderBy(colListHead, SavedComment, SavedFile, m);
            m = OrderBy(colListTail, SavedComment, SavedFile, m);

            LoadBlanks();

            LoadTabPage(SavedFile, "tabPage1");
            SaveTabPage(SavedFile, "tabPage1");
            //save blank rows if exists
            //retreave old comments in new position
            Clear_All_Comments(alert: false);
            //clean after load new order
            TextBox oldValueTbox;
            foreach (KeyValuePair<string, string> paircomment in commentList)
            {
                for (int x = 1; x <= Parameters.nTotTboxes; x += Parameters.nOfColumns)
                {
                    oldValueTbox = (TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(x)];
                    if (paircomment.Key == oldValueTbox.Text)
                    {
                        SaveComment(SavedComment, oldValueTbox.Name.Substring(7), paircomment.Value);
                    }
                }
            }
            //load ordered version of comments
            LoadComments(SavedComment);

        }

        private int OrderBy(SortedDictionary<string, int> showsNames, string SavedComment, string SavedFile, int n)
        {
            string oldValue;
            //TextBox seriesTBox;
            foreach (KeyValuePair<string, int> pair in showsNames)
            {

                for (int x = pair.Value; x <= pair.Value + Parameters.nOfColumns - 1; x++)
                {
                    oldValue = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(x)]).Text;
                    File.AppendAllText(SavedFile, "textBox" + Convert.ToString(n) + "=" + oldValue + "***");
                    n += 1;
                }
            }
            return n;
        }


        #endregion

        #region Check for New Episodes #####################################################################


        //Check_New_Episode_Click
        private void label_NextEpisode_Click(object sender, EventArgs e)
        {
            if (backgroundWorker_GetTorrent.IsBusy) return;
            VerifyNewEpisodes();
        }

        private void VerifyNewEpisodes()
        {
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            Color labelColor = label_NextEpisode.BackColor;
            label_NextEpisode.BackColor = Colours.buttonsBusyBackGround;
            label_NextEpisode.Refresh();

            while (CodHtmlWorker.IsBusy) //if user has just opened program 
            {
                Thread.Sleep(500);
            }

            if (Parameters.IsInternetConnectionOk)
            {
                if (Parameters.CodHtmlNextAiringStr == "#off-line#")
                {
                    MsgBox.Show(@"TvCalendar WebSite is off-line. 'NextEpisode' function is not
going to work now. But You can try 'Get Torrents' regardless.", "WebSite is Offline", MsgBox.Buttons.OK, MsgBox.Icone.Info, MsgBox.AnimateStyle.SlideDown);
                    BlinkControl(button_TvCalendar, 200, 3);
                }
                else
                {
                    Display_Next_airing();
                    Update_Days_from_last_download();
                    AutoApplyTooltips();
                }
            }
            else
            {
                Parameters.InternetBadMessage();
            }

            label_NextEpisode.BackColor = labelColor;

            flowLayoutPanel_Tboxes.Refresh();
            Thread.Sleep(350);
            if (IsAnycheckBox_Checked())
            {
                BlinkControl(button_GetTorrent, 250, 2);
            }
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void Display_Next_airing(int timeVisualEfect = 30)
        {

            if (Parameters.CodHtmlNextAiringStr != "#off-line#")
            {

                System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

                string codhtml = null;
                int ini = 0;
                int ini1 = 0;
                int ini0 = 0;
                int ini1fini = 0;
                int ini3 = 0;
                int ini3fini = 0;
                string myserie = null;
                string name_episode = null;
                string until_date = null;
                string epdate_date = null;
                string season = null;
                string episode = null;
                int NextEpDay = 0;
                int intEpisode = 0;
                string NairingSeason = null;
                string NxtairingEpisode = null;
                int daysfromlastdownload = 0;
                string txtfromlastdownload = null;
                Boolean runStatus = false;


                if (Parameters.CodHtmlNextAiringStr == "pogdesign")
                //prevent more than one access per session over pogdesign next-airing webpage.
                {
                    codhtml = Parameters.CodHtmlNextAiringStringExtractor();
                    Parameters.CodHtmlNextAiringStr = codhtml;
                }
                else
                {
                    codhtml = Parameters.CodHtmlNextAiringStr;
                }

                //System.Net.WebClient ccc = new System.Net.WebClient();

                //codhtml = ccc.DownloadString("http://www.pogdesign.co.uk/cat/next-airing");

                //<div class="showlist  ">
                //  <span class="shname"><strong><a href="/cat/Anger-Management-summary">Anger Management</a></strong></span>
                //  <span class="epname"><a href="/cat/Anger-Management/Season-2/Episode-67">Charlie and the Psychic Therapist</a></span> 
                //  <span class="epdate">19th Aug '14</span>
                //  <span class="epuntil">6 days </span>
                //  <span class="eptime">2:00am   - 2:30am</span> 
                //</div> 

                myserie = "NA";
                string nextEpTBoxIndex;
                TextBox nextEpTBox;
                TextBox mySeriesTBox;
                bool breakFor = false;

                for (int seriesIndex = Parameters.showsColumn; seriesIndex <= Parameters.nTotTboxes; seriesIndex += Parameters.nOfColumns)
                {

                    nextEpTBoxIndex = Convert.ToString(seriesIndex + Parameters.nextEpisodeColumn - 1);
                    nextEpTBox = (TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + nextEpTBoxIndex];
                    CheckBox thischeckBox = (CheckBox)flowLayoutPanel_Cboxes.Controls["checkBox" + Convert.ToString(Parameters.TboxIndex2RowIndex(seriesIndex))];

                    if (nextEpTBox.ForeColor == Colours.showsFontGettingTorrentNotFound)// if red always inplies checked=false;
                    {
                        thischeckBox.Checked = false;
                    }
                    else
                    {
                        nextEpTBox.Font = new Font("Trebuchet MS", 9f, FontStyle.Regular); //FontStyle.Italic |FontStyle.Regular
                        nextEpTBox.Refresh();
                        Thread.Sleep(timeVisualEfect);

                        name_episode = "";
                        until_date = "";
                        epdate_date = "";
                        mySeriesTBox = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(seriesIndex)]);
                        myserie = mySeriesTBox.Text;

                        if (thischeckBox.CheckState != CheckState.Indeterminate)  //false only if recursive download is not activate (indeterminate value)
                        {
                            thischeckBox.Checked = false;
                        }

                        //textbox blank
                        if (!string.IsNullOrEmpty(myserie))
                        {
                            season = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(seriesIndex + Parameters.seasonColumn - 1)]).Text;
                            episode = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(seriesIndex + Parameters.episodeColumn - 1)]).Text;

                            //torrent exclusion word type on metasearcher is welcome but not welcome on pogdesign
                            if (myserie.Contains(" -")) myserie = myserie.Remove(myserie.IndexOf(" -")).Trim();

                            myserie = myserie.Trim();
                            myserie = myserie.Replace("& ", "&amp; ");

                            if (string.IsNullOrEmpty(season))  //new series added
                            {
                                //if (!Get_SpellCheckOk(myserie)) //if user enters shows name manually.
                                //{
                                //    MsgBox.Show("'" + myserie + "'" + " Does't Spell Right! Right-Click On Spot to 'Add New Show' from a Catolog.", "OOOps", MsgBox.Buttons.OK, MsgBox.Icone.Info, MsgBox.AnimateStyle.FadeIn);
                                //    breakFor = true;
                                //}
                                //else
                                //{
                                ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(seriesIndex + Parameters.seasonColumn - 1)]).Text = "1";
                                ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(seriesIndex + Parameters.episodeColumn - 1)]).Text = "1";
                                ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(seriesIndex + Parameters.resolutionColumn - 1)]).Text = "720p";
                                ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(seriesIndex + Parameters.plusColumn - 1)]).Text = "None";
                                ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(seriesIndex + Parameters.lastDownloadColumn - 1)]).Text = " :-D " + "s00E00" + "|" + "0" + "|" + DateTime.Today.ToString("yyyy/MM/dd");

                                episode = "0";
                                season = "0";

                                runStatus = true;
                                //}
                            }
                            // myserie = myserie.Replace(":", "");
                            if (!breakFor)
                            {
                                intEpisode = Convert.ToInt16(episode);
                                txtfromlastdownload = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(seriesIndex + Parameters.lastDownloadColumn - 1)]).Text;
                                daysfromlastdownload = 0;
                                if (txtfromlastdownload.Contains("|"))
                                {
                                    daysfromlastdownload = Convert.ToInt16(Regex.Match(txtfromlastdownload, @"(?<=\|)[0-9]{1,3}(?=\|)", RegexOptions.IgnoreCase).Value);// 
                                }

                                ini = codhtml.IndexOf(myserie, 0, StringComparison.InvariantCultureIgnoreCase);
                                //find myserie in html code

                                ini0 = -1;
                                //verify if there is more than one occurence of myserie (is usual and short series names can cause this), if so check if it is good. This will eliminate the need for < signal .
                                while (ini > 0 && ini0 == -1)
                                {
                                    ini0 = codhtml.Remove(ini).IndexOf("summary", ini - 10, StringComparison.InvariantCultureIgnoreCase); //mutant
                                    if ((ini0 == -1)) // "summary word must be 10 caracters before series name to be good, if not, try another
                                    {
                                        ini = codhtml.IndexOf(myserie, ini + 1, StringComparison.InvariantCultureIgnoreCase);
                                    }
                                }

                                ini1 = codhtml.IndexOf("Season-", ini + 1); //mutant
                                ini3 = codhtml.IndexOf("epuntil", ini1 + 1); //mutant
                                //registro encontrado

                                if (Math.Abs(ini1 - ini) < 200 && Math.Abs(ini3 - ini1) < 400)
                                {
                                    //<div class="showlist  ">
                                    //  <span class="shname"><strong><a href="/cat/Anger-Management-summary">Anger Management</a></strong></span>
                                    //  <span class="epname"><a href="/cat/Anger-Management/Season-2/Episode-67">Charlie and the Psychic Therapist
                                    //</a></span> 
                                    //  <span class="epdate">19th Aug '14</span>
                                    //  <span class="epuntil">6 days </span>
                                    //  <span class="eptime">2:00am   - 2:30am</span> 
                                    //</div>   
                                    ini1fini = codhtml.IndexOf("</a></span>", ini1); //mutant
                                    name_episode = codhtml.Remove(ini1fini).Substring(ini1);
                                    name_episode = name_episode.Replace(">", ":").Replace("\"", "").Replace("/", "");
                                    name_episode = name_episode.Replace("Episode-", "E").Replace("Season-", "S");
                                    NairingSeason = name_episode.Remove(name_episode.IndexOf("S") + 3).Replace("E", "").Replace("S", "");
                                    NxtairingEpisode = name_episode.Remove(name_episode.IndexOf("E", 1) + 3).Substring(name_episode.IndexOf("E", 1) + 1).Replace(":", "");
                                    //entrada recem criada
                                    if (season == "0")
                                    {
                                        string nxtEpisode = NxtairingEpisode;
                                        if (thischeckBox.CheckState == CheckState.Indeterminate) //will occurs after Addnewshow. recursive gettorrent initiates at first episode
                                        {
                                            nxtEpisode = "1";
                                        }
                                        ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(seriesIndex + 2)]).Text = nxtEpisode;
                                        ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(seriesIndex + 1)]).Text = NairingSeason;
                                        episode = NxtairingEpisode;
                                        season = NairingSeason;
                                    }
                                    //<span class="epuntil">1 day </span>
                                    //<span class="epdate">9th Apr '14</span>
                                    ini3fini = codhtml.IndexOf("<", ini3);
                                    until_date = codhtml.Remove(ini3fini).Substring(ini3 + 9);
                                    ini3 = codhtml.IndexOf("epdate", ini1); //mutant
                                    ini3fini = codhtml.IndexOf("<", ini3);
                                    epdate_date = codhtml.Remove(ini3fini).Substring(ini3 + 8);
                                    epdate_date = "." + epdate_date.Remove(8) + ".";

                                    NextEpDay = Convert.ToInt16(until_date.Remove(2));
                                    //é certo que está disponível, se season for maior é absurdo e deve ser erro humano.
                                    if (thischeckBox.CheckState != CheckState.Indeterminate) //resursive download on
                                    {
                                        if (Convert.ToInt16(NairingSeason) > Convert.ToInt16(season) || Convert.ToInt16(NxtairingEpisode) > intEpisode)
                                        {
                                            thischeckBox.Checked = true;
                                        }
                                    }
                                }
                                else // next ep date not available
                                {
                                    string nextEpTboxTxt = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(seriesIndex + Parameters.nextEpisodeColumn - 1)]).Text;
                                    //resolve o problema do último episodio antes da midseason não marcar para download
                                    if (string.IsNullOrEmpty(nextEpTboxTxt))
                                    {
                                        until_date = "E1 .Mid/EndSeason";
                                    }
                                    else
                                    {
                                        if (nextEpTboxTxt.Contains(".End Season"))// && !IsLastEpisode(myserie, Convert.ToString(intEpisode + 1)))  //is not last only if its (lastep) torrent is already taken and episode is lastep + 1
                                        {
                                            //nextEpTboxTxt.Contains("2.Eps.in.1.Day") &&
                                            if ((IsLastEpisode(myserie, episode) || IsLastEpisode(myserie, (intEpisode + 1).ToString())))
                                            {
                                                until_date = nextEpTboxTxt;
                                                thischeckBox.Checked = true;
                                            }
                                            else //if ( IsLastEpisode(myserie, episode))
                                            {
                                                InitializeNewSeason(mySeriesTBox);
                                                until_date = "E1 .WaitingNextSeason";
                                            }
                                        }
                                        else
                                        {
                                            int result = 0;
                                            string firstchar = null;
                                            firstchar = nextEpTboxTxt.Remove(1);
                                            int.TryParse(firstchar, out result);
                                            //é númerico se result >0
                                            //é do tipo Exx
                                            if (result > 0)
                                            {
                                                until_date = "E" + Convert.ToString(intEpisode + 1) + " ." + "Mid Season";
                                                if (IsLastEpisode(myserie, episode))
                                                {
                                                    //epdate_date = "E1 .WaitingNextSeason";
                                                    until_date = "E" + episode + " ." + "End Season";
                                                }
                                                else if (IsLastEpisode(myserie, (intEpisode + 1).ToString()))
                                                { //to circunvent the times when the 2 lastest episodes are airing in the same day. Episode will not show as last, only Episode+1. There is no next airing date to the last episode.

                                                    if (Parameters.sameDayTdPosition == 0) //are in the same day
                                                    {
                                                        until_date = "E" + Convert.ToString(intEpisode + 1) + " ." + "End Season (2.Eps.in.1.Day)";
                                                    }
                                                    else //in different days
                                                    {
                                                        until_date = "E" + Convert.ToString(intEpisode + 1) + " ." + "End Season";
                                                    }
                                                }
                                                //ultimo episodio da temporada ou midseason
                                                thischeckBox.Checked = true;
                                                runStatus = true;
                                            }
                                            else
                                            {
                                                //end of season detected
                                                if (daysfromlastdownload < 70)
                                                {
                                                    until_date = nextEpTboxTxt;
                                                    int Eep = Convert.ToInt16(nextEpTboxTxt.Substring(1, 2));
                                                    //when midseason this will mark as not downloaded
                                                    if (Eep > Convert.ToInt16(episode))
                                                    {
                                                        thischeckBox.Checked = true;
                                                    }
                                                }
                                                else if (daysfromlastdownload < 365)
                                                {
                                                    until_date = "E1 .WaitingNextSeason";
                                                }
                                                else
                                                {
                                                    until_date = "E1 .Cancelled?!?!";
                                                }
                                            }
                                        }
                                    }
                                }
                                nextEpTBox.Text = until_date + epdate_date + name_episode;

                            } //breakfor
                        }
                        else //myseries blank
                        {
                            ((CheckBox)flowLayoutPanel_Cboxes.Controls["checkBox" + Convert.ToString(Parameters.TboxIndex2RowIndex(seriesIndex))]).Checked = false; //uncheck in case a blank row is checked
                        }
                        nextEpTBox.Font = new Font("Trebuchet MS", 9f, FontStyle.Bold);
                        nextEpTBox.Refresh();
                    }

                } //next textbox

                if (runStatus)
                {
                    Display_Status();  //only runs if new show is added
                }
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }
        }

        private Boolean IsLastEpisode(string myserie, string episode)
        {

            string codhtml;
            if (Parameters.CodHtmlTvCalendarStr == "pogdesign")
            //prevent more than one access per session over pogdesign next-airing webpage.
            {
                codhtml = Parameters.CodHtmlTvCalendarStringExtractor();
                Parameters.CodHtmlTvCalendarStr = codhtml;
            }
            else
            {
                codhtml = Parameters.CodHtmlTvCalendarStr;
            }

            //     <p class="lastep"><a href="/cat/The-Returned-US-summary" rel="q/70498">The Returned (US)
            //       </a> 
            //       <br /><a href="/cat/The-Returned-US/Season-1/Episode-10">S: 1 - Ep: 10</a> 
            //       <br /> A&E - 11:00pm 
            //     </p>       
            //</div>

            codhtml = codhtml.ToLower();
            //<td id="d  posição da proxima td
            myserie = Parameters.ClearStr(myserie);
            myserie = myserie.Replace(" ", "-").Replace(".", "").ToLower();

            int ini = 0;
            int fini = 0;
            int inter = 0;
            string codhtmlInter;
            // Parameters.TwoEpsInSameDay = false;
            foreach (Match mySerieMatch in Regex.Matches(codhtml, myserie))
            {
                //problem - 2eps.in.a.day .

                ini = codhtml.IndexOf(myserie, ini + 1);
                inter = codhtml.Remove(ini).IndexOf("lastep", ini - 50); //mutant
                fini = codhtml.IndexOf("</p>", ini); //mutant

                if (fini > -1)
                {
                    if (inter == -1)
                    {
                        Parameters.sameDayTdPosition = codhtml.IndexOf("<td id=", fini); //mutant //series ok, lastep not 
                    }
                    else
                    {
                        codhtmlInter = codhtml.Substring(ini).Remove(fini - inter).ToLower();

                        if (codhtmlInter.Contains("episode-" + episode)) //mutant
                        {
                            Parameters.sameDayTdPosition -= codhtml.IndexOf("<td id=", fini); //mutant //will return 0 if 2 eps are in the same day (before de same '<td id=')
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        private Boolean Get_SpellCheckOk(string mySerie)
        {//deprecated because shows name column is readonly, clone of tvcalendar catalog
            string codhtml;
            if (Parameters.CodHtmlShowListStr == "pogdesign")
            //prevent more than one access per session over pogdesign next-airing webpage.
            {
                Parameters.CodHtmlShowListStr = Parameters.CodHtmlShowListExtractor();
            }
            codhtml = Parameters.CodHtmlShowListStr;
            if (codhtml.Contains(mySerie)) //(Regex.IsMatch(codhtml, myserietions.IgnoreCase)) 
            {
                return true;
            }
            return false;
        }


        private void Update_Days_from_last_download()
        {
            // PublicVarsConstMethods pubs = new PublicVarsConstMethods();
            System.DateTime lastdate;
            string txt = null;
            string txt2 = null;
            string txt3 = null;
            string txt4 = null;
            //Dim daysdif As long
            TimeSpan daysdif;
            int daysint = 0;

            for (int lastDownColIndex = Parameters.lastDownloadColumn; lastDownColIndex < Parameters.nTotTboxes + 1; lastDownColIndex += Parameters.nOfColumns)
            {
                //:-D S01E15-01-16/03/2014
                txt = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(lastDownColIndex)]).Text;
                if (txt.Contains("|"))
                {
                    txt2 = txt.Substring(txt.LastIndexOf("|") + 1);
                    lastdate = Convert.ToDateTime(txt2);
                    daysdif = DateTime.Today - lastdate;
                    daysint = Math.Abs(daysdif.Days); //never can be negative
                    txt3 = txt.Remove(txt.IndexOf("|"));
                    txt4 = txt3 + "|" + Convert.ToString(daysint) + "|" + txt2;
                    ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(lastDownColIndex)]).Text = txt4;
                }
            }
        }

        private void ComboBox_Searcher_SelectionChangeCommitted(object sender, EventArgs e)
        {//commited will execute only when list box has been opened and closed, therefor will not run on Startup. So dont change that.
            if (backgroundWorker_GetTorrent.IsBusy) return;
            //to implement recursive gettorrent where darkred (no torrent found) was unchecked. They will be rechecked if searcher has been changed.
            button_GetTorrent.Focus();
            RestoreColorRows(this.flowLayoutPanel_Tboxes);

            Display_Next_airing(0);
        }

        #endregion

        #region Check Status for all Shows #####################################################################


        private void label_Status_Click(object sender, EventArgs e)
        {
            if (backgroundWorker_GetTorrent.IsBusy || CodHtmlWorker.IsBusy) return;
            CheckShowStatus();
        }

        private void CheckShowStatus()
        {
            Color labelColor = label_Status.BackColor;
            label_Status.BackColor = Colours.buttonsBusyBackGround;
            label_Status.Refresh();
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            if (Parameters.IsInternetConnectionOk)
            {
                Display_Status();
            }
            else
            {
                Parameters.InternetBadMessage();
            }
            label_Status.BackColor = labelColor;
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }


        private void Display_Status()
        {
            string baseString0 = "red'>";  //mutant
            string baseString1 = "</span>"; //mutant

            string codhtml = "";
            int ini = 0;
            int midi = 0;
            int fini = 0;
            string mySerie = null;
            string statusValue = "";
            int seriesIndex = 0;
            int neededcondic = 0;
            int myserieLen = 0;
            int ini0 = 0;
            bool founded = false;
            TextBox StatusTxtBox = null;
            int cancelledPos = 0;
            string nextEpTxtBoxText = null;

            if (Parameters.CodHtmlStatusStr == "epguides")
            //prevent more than one access per session over epguides hiatus webpage.
            {
                codhtml = Parameters.CodHtmlStatusStringExtractor();
                Parameters.CodHtmlStatusStr = codhtml;
            }
            else
            {
                codhtml = Parameters.CodHtmlStatusStr;
            }

            for (seriesIndex = Parameters.showsColumn; seriesIndex <= Parameters.nTotTboxes; seriesIndex += Parameters.nOfColumns)
            {
                statusValue = "";
                // Thread.Sleep(20);
                founded = false;
                mySerie = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(seriesIndex)]).Text.Trim();
                //textbox blank

                if (!string.IsNullOrEmpty(mySerie))
                {
                    StatusTxtBox = (TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(seriesIndex + Parameters.statusColumn - 1)];
                    StatusTxtBox.Font = new Font("Trebuchet MS", 9f, FontStyle.Regular);
                    StatusTxtBox.Refresh();
                    Thread.Sleep(30);

                    if (mySerie.Contains(" -")) mySerie = mySerie.Remove(mySerie.IndexOf(" -"));
                    mySerie = mySerie.Trim().ToLower();

                    mySerie = mySerie.Replace("& ", "&amp;").Replace("<", "");
                    myserieLen = mySerie.Length;

                    nextEpTxtBoxText = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(seriesIndex + Parameters.nextEpisodeColumn - 1)]).Text;
                    //here
                    if (string.IsNullOrEmpty(nextEpTxtBoxText.TrimStart()))
                    {
                        nextEpTxtBoxText = "E";
                    }
                    if (nextEpTxtBoxText.Substring(0, 1) != "E")
                    {
                        statusValue = "Airing/Returning";
                    }

                    else
                    {
                        statusValue = "ToBeAnounced";
                        //below cancelled Position (on code), the shows was cancelled indeed.
                        cancelledPos = codhtml.IndexOf("cancelled/ended", 1, StringComparison.InvariantCultureIgnoreCase);

                        if (Regex.Split(mySerie, " ").Length > 2)  //if more than 2 words cut the first one.A single word will return length = 1. 
                        {
                            mySerie = mySerie.Substring(mySerie.IndexOf(" ")).Trim(); //increases the chance to find a match on epguides code.
                        }

                        ini = codhtml.IndexOf(mySerie, 1, StringComparison.InvariantCultureIgnoreCase);
                        //.net 

                        if (ini > -1)
                        {
                            ini0 = ini;
                            do
                            {
                                midi = codhtml.IndexOf(baseString0, ini + 1);
                                neededcondic = midi - ini - mySerie.Length;
                                if (neededcondic < 100 && neededcondic > -1)
                                {
                                    founded = true;
                                }
                                else
                                {
                                    ini = codhtml.IndexOf(mySerie, ini + 1, StringComparison.InvariantCultureIgnoreCase);
                                }
                            } while (!(ini == ini0 || founded));
                            //até que todas as ocorrencias não sejam ou sejam achadas
                            //existe a informaçaõ
                            if (founded)
                            {
                                fini = codhtml.IndexOf(baseString1, midi + 1);
                                statusValue = codhtml.Substring(midi + baseString0.Length, fini - midi - baseString0.Length).Trim();
                                if (string.IsNullOrEmpty(statusValue))
                                {
                                    statusValue = "Show On Hiatus";
                                }//existe na pagina mas não há dados ainda
                            }
                            else
                            {
                                if (cancelledPos > -1)
                                {
                                    if (ini > cancelledPos)
                                    {
                                        statusValue = "Ended/Cancelled";
                                    }
                                    else
                                    {
                                        statusValue = "Cancelled?!?!";
                                    }
                                }
                            }
                        }
                    }

                    StatusTxtBox.Text = statusValue;
                    StatusTxtBox.Font = new Font("Trebuchet MS", 9f, FontStyle.Bold);
                    StatusTxtBox.Refresh();
                }
            }
        }

        #endregion

        #region Change Color of rows ######################################################


        private void TextBoxesRow2Default(int myrow)
        {
            if (((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + (Parameters.RowIndex2TboxIndex(myrow).ToString())]).ForeColor != Colours.showsFont)
            {
                System.Collections.Generic.List<TextBox> MarkedTextBoxes = new System.Collections.Generic.List<TextBox>();
                for (int y = 0; y < Parameters.nOfColumns; y++)
                {
                    MarkedTextBoxes.Add((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(Parameters.RowIndex2TboxIndex(myrow) + y)]);
                }
                TextBoxForeColorRow(MarkedTextBoxes, Colours.showsFont);
            }
        }


        private void TextBoxForeColorRow(List<TextBox> TxtBoxesList, Color MyColor)
        {
            //
            for (int x = 0; x <= Parameters.nOfColumns - 1; x++)
            {
                TxtBoxesList[x].ForeColor = MyColor;
                TxtBoxesList[x].Refresh();
            }
            //panel1.Refresh();
        }


        //radiobutton and/or checkbox change
        private void ChangeColorRow(object mysender, bool senderCheckState)
        {
            //
            Control mycontrolrow = (Control)mysender;
            int nrow = Convert.ToInt16(mycontrolrow.Text);
            if (mysender is RadioButton)
            {
                if (senderCheckState)
                {
                    TextBoxBackColorRow(nrow, Colours.showsBackRadio);
                }
                else if (((CheckBox)flowLayoutPanel_Cboxes.Controls["Checkbox" + Convert.ToString(nrow)]).Checked)
                {
                    TextBoxBackColorRow(nrow, Colours.showsBackChecked);
                }
                else
                {
                    TextBoxBackColorRow(nrow, Colours.showsBackGround);
                }
                //sender is checkbox
            }
            else
            {
                if (!((RadioButton)flowLayoutPanel_RButtons.Controls["radiobutton" + Convert.ToString(nrow)]).Checked)
                {
                    if (senderCheckState)
                    {
                        TextBoxBackColorRow(nrow, Colours.showsBackChecked);
                    }
                    else
                    {
                        TextBoxBackColorRow(nrow, Colours.showsBackGround);

                    }
                }
            }
        }


        private void TextBoxBackColorRow(int nRow, Color MyColor)
        {
            int tBoxindex = Parameters.RowIndex2TboxIndex(nRow);
            TextBox tbox;
            for (int x = 0; x < Parameters.nOfColumns; x++)
            {
                tbox = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(tBoxindex + x)]);
                tbox.BackColor = MyColor;
                tbox.Refresh();
            }
        }


        #endregion

        #region Save and Load Tabpages ##########################################################


        private void SaveTabPage(string savedFile, string TabPg)
        {
            savedFile = Path.Combine(Parameters.myProgPath, savedFile);
            Directory.CreateDirectory(Parameters.myProgPath);//If the directory already exists, this method does nothing.        
            File.Create(savedFile).Dispose(); //if file exists will be overwritten

            foreach (TabPage myTabPage in this.tabControl1.TabPages)
            {
                if (myTabPage.Name == TabPg)
                {
                    foreach (Control myControl in myTabPage.Controls)
                    {
                        if (myControl is FlowLayoutPanel || myControl is Panel)
                        {
                            if (myControl.AccessibleDescription == "savethis")
                            {
                                foreach (Control subControl in myControl.Controls)
                                {
                                    if ((subControl is TextBox || subControl is CheckBox || subControl is ComboBox || subControl is RadioButton))
                                    {
                                        if (subControl is TextBox)
                                        {
                                            File.AppendAllText(savedFile, ((TextBox)subControl).Name + "=" + ((TextBox)subControl).Text + "***");
                                        }
                                        else if (subControl is CheckBox)
                                        {
                                            File.AppendAllText(savedFile, ((CheckBox)subControl).Name + "=" + Convert.ToString(((CheckBox)subControl).Checked) + "***");
                                        }
                                        else if (subControl is ComboBox)
                                        {
                                            File.AppendAllText(savedFile, ((ComboBox)subControl).Name + "=" + Convert.ToString(((ComboBox)subControl).SelectedIndex) + "***");
                                        }
                                        else //if (subControl is RadioButton)
                                        {
                                            File.AppendAllText(savedFile, ((RadioButton)subControl).Name + "=" + Convert.ToString(((RadioButton)subControl).Checked) + "***");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LoadBlanks()
        {

            foreach (TextBox tBox in flowLayoutPanel_Tboxes.Controls)
            {
                tBox.Text = "";
                UncheckAll();
            }
        }

        private void LoadTabPage(string SavedFile, string tabPageN)
        {

            SavedFile = Path.Combine(Parameters.myProgPath, SavedFile);
            //string controlType = "";

            if (File.Exists(SavedFile))
            {
                string savestring = "";
                int ini = 0;
                int midi = 0;
                int fini = 0;
                savestring = File.ReadAllText(SavedFile);
                foreach (TabPage myTabPage in this.tabControl1.TabPages)
                {
                    if (myTabPage.Name == tabPageN)
                    {
                        foreach (Control myControl in myTabPage.Controls)
                        {
                            if (myControl is FlowLayoutPanel || myControl is Panel)
                            {
                                //string nam = myControl.Name;
                                if (myControl.AccessibleDescription == "savethis")
                                {
                                    foreach (Control subControl in myControl.Controls)
                                    {
                                        // controlType = mycontrol.GetType().ToString();

                                        if (subControl is TextBox || subControl is CheckBox || subControl is ComboBox || subControl is RadioButton)
                                        {
                                            if (subControl is TextBox)
                                            {
                                                ini = savestring.IndexOf(((TextBox)subControl).Name + "=", StringComparison.InvariantCultureIgnoreCase);
                                                if (ini > -1)
                                                {
                                                    midi = savestring.IndexOf("=", ini);
                                                    fini = savestring.IndexOf("***", midi);
                                                    ((TextBox)subControl).Text = savestring.Remove(fini).Substring(midi + 1);
                                                }
                                            }
                                            else if (subControl is CheckBox)
                                            {
                                                ini = savestring.IndexOf(((CheckBox)subControl).Name + "=", StringComparison.InvariantCultureIgnoreCase);
                                                if (ini > -1)
                                                {
                                                    midi = savestring.IndexOf("=", ini);
                                                    fini = savestring.IndexOf("***", midi);
                                                    ((CheckBox)subControl).Checked = Convert.ToBoolean(savestring.Remove(fini).Substring(midi + 1));
                                                }
                                            }
                                            else if (subControl is ComboBox)
                                            {
                                                ini = savestring.IndexOf(((ComboBox)subControl).Name + "=", StringComparison.InvariantCultureIgnoreCase);
                                                if (ini > -1)
                                                {
                                                    midi = savestring.IndexOf("=", ini);
                                                    fini = savestring.IndexOf("***", midi);
                                                    ((ComboBox)subControl).SelectedIndex = Convert.ToInt16(savestring.Remove(fini).Substring(midi + 1));
                                                }
                                            }
                                            else //if (subControl is RadioButton)
                                            {
                                                ini = savestring.IndexOf(((RadioButton)subControl).Name + "=", StringComparison.InvariantCultureIgnoreCase);
                                                if (ini > -1)
                                                {
                                                    midi = savestring.IndexOf("=", ini);
                                                    fini = savestring.IndexOf("***", midi);
                                                    ((RadioButton)subControl).Checked = Convert.ToBoolean(savestring.Remove(fini).Substring(midi + 1));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        #endregion

        #region load save comments ##################################################################################

        private void button_TP3_Save_Click(object sender, System.EventArgs e)
        {
            string filecomments = "MyTvShowsComments" + label_NofPage.Text + ".ini";
            string tboxIndex = label_TP3_Commentary.AccessibleDescription;
            SaveComment(filecomments, tboxIndex, this.richTextBox_TP3_Commentary.Text);
            Go2Page(0);
        }

        private void button_TP3_Cancel_Click(object sender, EventArgs e)
        {
            Go2Page(0);
            Comments_Page_Destructor();
        }

        private void Comments_Page_Constructor()
        {
            Comments_Page_Destructor();

            button_TP3_Save.Click += button_TP3_Save_Click;
            button_TP3_Cancel.Click += button_TP3_Cancel_Click;
        }

        private void Comments_Page_Destructor()
        {
            button_TP3_Save.Click -= button_TP3_Save_Click;
            button_TP3_Cancel.Click -= button_TP3_Cancel_Click;
        }



        private void SaveComment(string SavedCommentFile, string my_tboxIndex, string my_comment, bool appendit = false)
        {
            SavedCommentFile = Path.Combine(Parameters.myProgPath, SavedCommentFile);
            FileInfo my_file = new FileInfo(SavedCommentFile);

            if (!File.Exists(SavedCommentFile))
            {
                my_file.Directory.Create();
                //If the directory already exists, this method does nothing.
                File.Create(SavedCommentFile).Dispose();
                SaveBlanksComments(SavedCommentFile);
            }
            //Dim my_comment As String = Form3_Comment.RichTextBox1.Text
            string my_content = File.ReadAllText(SavedCommentFile);
            int iniposition = my_content.IndexOf("###" + my_tboxIndex + "=");
            int finiposition = my_content.IndexOf("###", iniposition + 1);
            int posLastEp1;

            if (appendit) //appendit is for new season reset function
            {

                string my_content_sub = my_content.Remove(finiposition).Substring(iniposition);
                int posLastEp0 = my_content_sub.IndexOf("***");

                if (posLastEp0 > -1)
                {

                    posLastEp1 = my_content.IndexOf("***", iniposition);
                    int posLastEp2 = my_content.IndexOf("***", posLastEp1 + 1);
                    my_content = my_content.Remove(posLastEp1, posLastEp2 - posLastEp1 + 3);
                }

                finiposition = my_content.IndexOf("###", iniposition + 1); //new finiposition
                my_content = my_content.Insert(my_content.IndexOf("=", iniposition) + 1, "***" + my_comment + "***");//finiposition
            }
            else
            {
                string my_content_sub = my_content.Remove(finiposition).Substring(iniposition);

                my_content = my_content.Remove(iniposition, finiposition - iniposition);
                my_content = my_content.Insert(iniposition, "###" + my_tboxIndex + "=" + my_comment);
            }

            File.WriteAllText(SavedCommentFile, my_content);
            LoadComments("MyTvShowsComments" + label_NofPage.Text + ".ini");
            ObliqueComments();
        }

        private void SaveBlanksComments(string SavedCommentFile)
        {
            SavedCommentFile = Path.Combine(Parameters.myProgPath, SavedCommentFile);
            //New file with blanks
            for (int tboxIndex = Parameters.showsColumn; tboxIndex <= Parameters.nTotTboxes; tboxIndex += Parameters.nOfColumns)
            {
                File.AppendAllText(SavedCommentFile, "###" + (tboxIndex.ToString() + "=" + "")); //shows names column
                File.AppendAllText(SavedCommentFile, "###" + (tboxIndex + Parameters.lastDownloadColumn - 1).ToString() + "=" + ""); //last downlo column
            }
            File.AppendAllText(SavedCommentFile, "###");

            LoadComments(SavedCommentFile);
        }


        private void LoadComments(string SavedCommentFile)
        {
            SavedCommentFile = Path.Combine(Parameters.myProgPath, SavedCommentFile);
            if (!File.Exists(SavedCommentFile))
            {
                SaveBlanksComments(SavedCommentFile);
                return;
            }
            string my_content = File.ReadAllText(SavedCommentFile);
            string my_comment = "";
            int iniposition = 0;
            int finiposition = 0;
            int equalposition = 0;

            for (int TboxIndex = Parameters.showsColumn; TboxIndex <= Parameters.nTotTboxes; TboxIndex += Parameters.nOfColumns)
            {
                TextBox tBox = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + TboxIndex.ToString()]);
                iniposition = my_content.IndexOf("###" + Convert.ToString(TboxIndex), iniposition);
                equalposition = my_content.IndexOf("=", iniposition);
                finiposition = my_content.IndexOf("###", iniposition + 1);
                my_comment = my_content.Substring(equalposition + 1, finiposition - equalposition - 1);

                //int lastSeasonEpisode = my_comment.IndexOf("***"); // dont load lastepisode info
                //if (lastSeasonEpisode > -1)
                //{
                //    my_comment = my_comment.Remove(my_comment.IndexOf("***"));
                //}

                TextBox myTxBox = (TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Convert.ToString(TboxIndex)];
                if (my_comment != "None" && my_comment != "") // PUT add new show message in empty spots
                {
                    ToolTip1.SetToolTip(myTxBox, my_comment);
                }
                else if (string.IsNullOrEmpty(myTxBox.Text))
                {
                    ToolTip1.SetToolTip(myTxBox, "Mouse Right Button to ADD new Show");
                }
                else
                {
                    ToolTip1.SetToolTip(myTxBox, "");
                }
                iniposition = finiposition - 1;

            }
        }


        private void CommentInsertionBox(TextBox txtboxshows, int tboxIndex)
        {
            // will open tabpag3 index 2 
            Go2Page(2);
            label_TP3_Commentary.Text = "Commentary" + " - " + txtboxshows.Text;
            label_TP3_Commentary.AccessibleDescription = tboxIndex.ToString();
            richTextBox_TP3_Commentary.Text = ToolTip1.GetToolTip(txtboxshows);
            richTextBox_TP3_Commentary.Select();

        }


        #endregion

        #region Move between Pages ##########################################################################


        //   MoveInterPages 


        private void LoadPartialArq(string MyFile)
        { //loads only first column
            if (File.Exists(Path.Combine(Parameters.myProgPath, MyFile)))
            {
                string savestring = File.ReadAllText(Path.Combine(Parameters.myProgPath, MyFile));
                int ini = 0;
                int midi = 0;
                int fini = 0;
                int name200; //textBox in Tabpage2 201, 209....
                CheckBox cBox;
                int cBoxIndex;
                foreach (TextBox mytextbox in flowLayoutPane_TP2_TBoxs.Controls)
                {
                    name200 = Convert.ToInt16(mytextbox.Name.Substring("textBox".Length)) - 200;
                    ini = savestring.IndexOf("textBox" + Convert.ToString(name200) + "=", StringComparison.InvariantCultureIgnoreCase);
                    if (ini > -1)
                    {
                        midi = savestring.IndexOf("=", ini);
                        fini = savestring.IndexOf("***", midi);
                        mytextbox.Text = savestring.Remove(fini).Substring(midi + 1);
                        cBoxIndex = Parameters.TboxIndex2RowIndex(name200);
                        cBox = (CheckBox)flowLayoutPanel_TP2_ChBxs.Controls["checkBox" + (cBoxIndex + Parameters.nOfRows).ToString()];
                        if (mytextbox.Text.Trim() != "")
                        {
                            cBox.Enabled = false;
                            cBox.FlatStyle = FlatStyle.Flat;
                        }
                        else
                        {
                            cBox.Enabled = true;
                            cBox.FlatStyle = FlatStyle.Standard;
                        }
                    }
                }

                foreach (CheckBox checkBx in flowLayoutPanel_TP2_ChBxs.Controls)
                {
                    checkBx.Checked = false;
                }
            }
            else
            {
                LoadPartialBlanks();
            }
        }

        private void LoadPartialBlanks()
        {
            foreach (TextBox mytextbox in flowLayoutPanel_TP2_ChBxs.Controls)
            {
                mytextbox.Text = "";
            }
        }

        //UpMovePage_Click
        private void button_TP2_PageUp_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt16(label_TP2_PageN.Text);
            LoadPartialNext(1, page);
        }
        //DownMovePage_Click
        private void button_TP2_PageDown_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt16(label_TP2_PageN.Text);
            LoadPartialNext(-1, page);
        }

        private void LoadPartialNext(int increment, int page)
        {
            if (increment == 1 && page == Parameters.MaxNumberOfPages)
            {
                label_TP2_PageN.Text = "1";
            }
            else if (increment == -1 && page == 1)
            {
                label_TP2_PageN.Text = Parameters.MaxNumberOfPages.ToString();
            }
            else
            {
                label_TP2_PageN.Text = Convert.ToString(page + increment);
            }
            if (File.Exists(Path.Combine(Parameters.myProgPath, "MyTvShowsOrganizer" + label_TP2_PageN.Text + ".ini")))
            {
                LoadPartialArq("MyTvShowsOrganizer" + label_TP2_PageN.Text + ".ini");
                button_TP2_Move.Enabled = true;
            }
            else
            {

                LoadPartialBlanks();
                button_TP2_Move.Enabled = false;
            }
        }

        //move
        private void MoveDataBetweenPages()
        {
            if (Convert.ToInt16(label_TP2_ItemsToBeAllocatedN.Text) != 0)
            {
                MsgBox.Show("Itens to be allocated must be 0. Mark or UnMark and Try Again.", "OOOPs", MsgBox.Buttons.OK, MsgBox.Icone.Warning, MsgBox.AnimateStyle.ZoomIn);
            }
            else
            {
                string movedDataSavedFile = Path.Combine(Parameters.myProgPath, "mytvshowsorganizermove.undo");
                int nofmoves = Convert.ToInt16(label_TP2_Items2BeMovedN.Text);
                string myFileDestiny = Path.Combine(Parameters.myProgPath, "MyTvShowsOrganizer" + label_TP2_PageN.Text + ".ini");
                string mycommentfiledestiny = "MyTvShowsComments" + label_TP2_PageN.Text + ".ini";
                int mytboxIndex = 0;
                string datatobemoved = "";
                string datatobechanged = "";
                string myDataValue = "";
                int myRow = 0;
                int nextcontrolindex = 0;
                //Dim datatobechanged As String = ""
                datatobemoved = File.ReadAllText(movedDataSavedFile);
                datatobechanged = File.ReadAllText(myFileDestiny);
                File.Delete(myFileDestiny);
                // old data is deleted
                File.Create(Path.Combine(Parameters.myProgPath, myFileDestiny)).Dispose();
                // old file recreated empt_--__y
                int ini = 0;
                int fini = 0;
                int col = 0;
                ini = 0;

                foreach (CheckBox mycheckbox in flowLayoutPanel_TP2_ChBxs.Controls)
                {
                    if (mycheckbox.Checked)
                    {
                        myRow = Convert.ToInt16(mycheckbox.Text);
                        mytboxIndex = Parameters.RowIndex2TboxIndex(myRow);
                        nextcontrolindex = mytboxIndex;
                        for (col = 1; col < Parameters.nOfColumns + 1; col++)
                        {
                            fini = datatobemoved.IndexOf("#", ini);
                            myDataValue = datatobemoved.Remove(fini).Substring(ini);
                            ini = fini + 1;
                            File.AppendAllText(myFileDestiny, "textBox" + Convert.ToString(nextcontrolindex) + "=" + myDataValue + "***");
                            nextcontrolindex += 1;
                        }

                        fini = datatobemoved.IndexOf("#", ini);
                        myDataValue = datatobemoved.Remove(fini).Substring(ini);
                        ini = fini + 1;
                        if (!string.IsNullOrEmpty(myDataValue))
                        {
                            SaveComment(mycommentfiledestiny, Convert.ToString(mytboxIndex), myDataValue);
                        }
                    }

                }
                File.AppendAllText(myFileDestiny, datatobechanged);
                //old data is restored after new data
                //needed because the LoadArq function finds the first ocurrency of each textbox and Will be more than one testbox mycontrolindex.
                LoadPartialArq(Path.Combine(Parameters.myProgPath, "MyTvShowsOrganizer" + label_TP2_PageN.Text + ".ini"));

                MsgBox.Show(Convert.ToString(nofmoves) + " Series Successfully moved!", "Done", MsgBox.Buttons.OK, MsgBox.Icone.Info, MsgBox.AnimateStyle.SlideDown);

                LoadTabPage("MyTvShowsOrganizer" + label_NofPage.Text + ".ini", "tabPage1");
                Go2Page(0);
            }
        }

        //moveClick
        private void button_TP2_Move_Click(object sender, EventArgs e)
        {
            MoveDataBetweenPages();
            Move_Page_Destructor();

        }

        //cancelMoveClick
        private void button_TP2_Cancel_Click(object sender, EventArgs e)
        {
            //undo move by coping over
            File.Copy(Path.Combine(Parameters.myProgPath, "MyTvShowsOrganizer" + label_NofPage.Text + ".undo"), Path.Combine(Parameters.myProgPath, "MyTvShowsOrganizer" + label_NofPage.Text + ".ini"), true);
            LoadTabPage(Path.Combine(Parameters.myProgPath, "MyTvShowsOrganizer" + label_NofPage.Text + ".ini"), "tabPage1");
            Go2Page(0);
            Move_Page_Destructor();

        }

        private void Move_Page_Constructor()
        {

            //events constructors
            Move_Page_Destructor();
            foreach (CheckBox cBox in this.flowLayoutPanel_TP2_ChBxs.Controls)
            {
                cBox.CheckedChanged += checkBox_MoveShows_CheckedChanged;
            }
            button_TP2_PageUp.Click += button_TP2_PageUp_Click;
            button_TP2_PageDown.Click += button_TP2_PageDown_Click;
            button_TP2_Move.Click += button_TP2_Move_Click;
            button_TP2_Cancel.Click += button_TP2_Cancel_Click;

        }

        private void Move_Page_Destructor()
        {

            //dectructor of 20 eventhandler
            foreach (CheckBox cBox in this.flowLayoutPanel_TP2_ChBxs.Controls)
            {
                cBox.CheckedChanged -= checkBox_MoveShows_CheckedChanged;
            }

            button_TP2_PageUp.Click -= button_TP2_PageUp_Click;
            button_TP2_PageDown.Click -= button_TP2_PageDown_Click;
            button_TP2_Move.Click -= button_TP2_Move_Click;
            button_TP2_Cancel.Click -= button_TP2_Cancel_Click;

        }

        private void Addlabel_2beAllocated(object box)
        {
            CheckBox ckbox = (CheckBox)box;
            int num = Convert.ToInt16(label_TP2_ItemsToBeAllocatedN.Text);
            if (ckbox.Checked)
            {
                label_TP2_ItemsToBeAllocatedN.Text = Convert.ToString(num - 1);
            }
            else
            {
                label_TP2_ItemsToBeAllocatedN.Text = Convert.ToString(num + 1);
            }
            BlinkControl(label_TP2_ItemsToBeAllocatedN, 100, 1);
        }


        private void checkBox_MoveShows_CheckedChanged(object sender, EventArgs e)
        {
            object obj = sender;
            Addlabel_2beAllocated(obj);
        }


        #endregion

        #region Organize Torrents ########################################################################################


        //organize button on TabPag1
        private void button_Organize_Click(object sender, EventArgs e)
        {
            if (backgroundWorker_GetTorrent.IsBusy) return;


            Organizer_Page_Constructor();

            Go2Page(3); //tabpage4
            LoadTabPage("OrganizeMove.ini", "tabPage4");

            RecountNofFile();

        }

        //tabpage4 go back button
        private void button_TP4_GoBack_Click(object sender, EventArgs e)
        {
            SaveTabPage("OrganizeMove.ini", "tabPage4");
            Go2Page(0);
            Organizer_Page_Destructor();
        }

        //tabpage4 organize button
        private void button_TP4_Organize_Click(object sender, EventArgs e)
        {
            Organize_Shows_Folders();
            // Organizer_Page_Destructor(); // not Here 
        }

        private void Organizer_Page_Constructor()
        {

            Organizer_Page_Destructor();
            button_TP4_GoBack.Click += button_TP4_GoBack_Click;
            button_TP4_Organize.Click += button_TP4_Organize_Click;

            button_TP4_BrowseTorrentFolder.Click += button_TP4_BrowseTorrentFolder_Click;
            button_TP4_BrowseSeriesFolder.Click += button_TP4_BrowseSeriesFolder_Click;

        }

        private void Organizer_Page_Destructor()
        {
            button_TP4_GoBack.Click -= button_TP4_GoBack_Click;
            button_TP4_Organize.Click -= button_TP4_Organize_Click;

            button_TP4_BrowseTorrentFolder.Click -= button_TP4_BrowseTorrentFolder_Click;
            button_TP4_BrowseSeriesFolder.Click -= button_TP4_BrowseSeriesFolder_Click;
        }

        //tabpag4 browser buttons 3 e 4
        private void button_TP4_BrowseTorrentFolder_Click(object sender, EventArgs e)
        {

            if (BrowseFolder(textBox_TP4_TorrentFolder))
            {
                SaveTabPage("OrganizeMove.ini", "tabPage4");
                RecountNofFile();
            }
            else
            {
                MsgBox.Show("The chosen folder returns \"Access Denied\". You must choose another.", "OOOps", MsgBox.Buttons.OK, MsgBox.Icone.Warning, MsgBox.AnimateStyle.ZoomIn);
            }

        }

        private void button_TP4_BrowseSeriesFolder_Click(object sender, EventArgs e)
        {
            if (BrowseFolder(textBox_TP4_SeriesFolder))
            {
                SaveTabPage("OrganizeMove.ini", "tabPage4");
                RecountNofFile();
            }

            else
            {
                MsgBox.Show("The chosen folder returns \"Access Denied\". You must choose another.", "OOOps", MsgBox.Buttons.OK, MsgBox.Icone.Warning, MsgBox.AnimateStyle.SlideDown);
            }

        }

        private void Organize_Shows_Folders()
        {

            Color buttoncolor = button_TP4_Organize.BackColor;
            button_TP4_Organize.BackColor = Colours.buttonsBusyBackGround;
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            button_TP4_Organize.Refresh();

            string dirTorrents = textBox_TP4_TorrentFolder.Text;
            string dirSeries = textBox_TP4_SeriesFolder.Text;
            //LoadTabPage("OrganizeMove.ini", "tabPage4");
            if (dirTorrents.Trim() == "" || dirSeries.Trim() == "" || dirTorrents == dirSeries)
            {
                MsgBox.Show("There are no address to this folder or they are equal.", "Impossible to Continue!", MsgBox.Buttons.OK, MsgBox.Icone.Shield, MsgBox.AnimateStyle.FadeIn);
            }
            else
            {
                SaveTabPage("OrganizeMove.ini", "tabPage4");

                if (((NofFilesInFolder(dirTorrents, "*.S??E??.*") + (NofFilesInFolder(dirTorrents, "* S??E?? *"))) == 0))
                {
                    MsgBox.Show("No Episode in the Folder of Torrents!", "Nothing to Organize.", MsgBox.Buttons.OK, MsgBox.Icone.Info, MsgBox.AnimateStyle.SlideDown);
                }
                else
                {
                    bool success = MoveOrganizeTorrents.MoveTorrents2SeriesFolder(dirTorrents, dirSeries);
                    int nInTorrentFolder = RecountNofFile();
                    if (nInTorrentFolder > 0 && success)
                    {
                        MsgBox.Show("Some files are protected and must be moved manually. The common reason is because they are in use by your torrent program", "Warning", MsgBox.Buttons.OK, MsgBox.Icone.Warning, MsgBox.AnimateStyle.ZoomIn);
                    }
                }
            }
            System.Windows.Forms.Cursor.Current = Cursors.Default;
            button_TP4_Organize.BackColor = buttoncolor;

        }


        private bool BrowseFolder(TextBox Tbox)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();

            folderDlg.ShowNewFolderButton = true;
            //Environment.SpecialFolder root = folderDlg.RootFolder;
            // Show the FolderBrowserDialog. 

            DialogResult result = folderDlg.ShowDialog();

            if (result == DialogResult.OK)
            {

                try
                {
                    //List will cause an exception if folder is "Access Denied".
                    List<string> TestSecurityAccess = new List<string>(Directory.EnumerateFiles(folderDlg.SelectedPath, "*.S??E??.*", SearchOption.AllDirectories));

                }
                catch (Exception)
                {
                    return false;
                }

                Tbox.Text = folderDlg.SelectedPath;
            }
            return true;
        }

        protected internal int NofFilesInFolder(string folder, string StrPattern)
        {
            if (Directory.Exists(folder))
            {
                try
                {
                    List<string> myfiles = new List<string>(Directory.EnumerateFiles(folder, StrPattern, SearchOption.AllDirectories));
                    int nOfFiles = myfiles.Count;
                    return nOfFiles;
                }
                catch (Exception)
                {
                    MsgBox.Show("Some files are protected and must be causing an Error. The commom reason is because they are in use by your torrent program.", "Warning", MsgBox.Buttons.OK, MsgBox.Icone.Warning, MsgBox.AnimateStyle.ZoomIn);
                }
            }

            return 0;
        }
        protected internal int RecountNofFile()
        {
            //2 string patterns because S??E?? can catch SUPER etc. 
            label_TP4_EpisodesInTorrentFolder.Text = "Episodes in Torrent Folder: " +
                (NofFilesInFolder(textBox_TP4_TorrentFolder.Text, "*.S??E??.*") + NofFilesInFolder(textBox_TP4_TorrentFolder.Text, "* S??E?? *")).ToString();
            label_TP4_EpisodesInSeriesFolder.Text = "Episodes in Series Folder: " +
                (NofFilesInFolder(textBox_TP4_SeriesFolder.Text, "*.S??E??.*") + NofFilesInFolder(textBox_TP4_SeriesFolder.Text, "* S??E?? *")).ToString();
            string eInTorrFolder = label_TP4_EpisodesInTorrentFolder.Text;
            eInTorrFolder = eInTorrFolder.Substring(eInTorrFolder.IndexOf(":") + 1).Trim();
            return Convert.ToInt16(eInTorrFolder);

        }


        #endregion

        #region Get Torrent - backgroundworker assync Thread. ############################################################

        private System.Windows.Forms.Timer timer_RecursiveInitiator;
        private System.Windows.Forms.Timer timer_GetAll_Torrents;

        private void Button_GetTorrent_Click(object sender, EventArgs e)
        { //clicking twice will cancel worker

            if (backgroundWorker_GetTorrent.IsBusy) return;
            if (backgroundWorker_GetTorrent.CancellationPending) return;

            if (Parameters.IsInternetConnectionOk)
            {
                this.button_GetTorrent.Cursor = Cursors.AppStarting;

                if (!IsAnycheckBox_Checked())
                {
                    Display_Next_airing(timeVisualEfect: 0);
                }
                Parameters.getAllIniPage = Convert.ToInt16(label_NofPage.Text); //needed to be here
                InitiateGetTorrentWorker();
            }
            else
            {
                Parameters.InternetBadMessage();
            }
        }

        private void timer_Recursive_Constructor()
        {
            //timer will enable torrent download recursivity.
            this.timer_RecursiveInitiator = new System.Windows.Forms.Timer(this.components);
            this.timer_RecursiveInitiator.Interval = 500;
            this.timer_RecursiveInitiator.Tick += new System.EventHandler(this.timer_RecursiveInitiator_Tick);

            // timer_RecursiveInitiator.Enabled = true;
        }

        private void timer_GetAll_Constructor()
        {

            //Parameters.getAllCurrentPage = 0;
            this.timer_GetAll_Torrents = new System.Windows.Forms.Timer(this.components);
            this.timer_GetAll_Torrents.Interval = 1000;
            this.timer_GetAll_Torrents.Tick += new System.EventHandler(this.timer_GetAll_Tick);
        }

        private void InitiateGetTorrentWorker()
        {
            // This method runs on the main thread. 

            this.button_GetTorrent.Cursor = Cursors.AppStarting;
            this.panel_Footer.Cursor = Cursors.AppStarting;
            button_GetTorrent.ForeColor = Colours.buttonFontGettingTorrent;
            button_GetTorrent.Refresh();
            this.Activate();

            GetTorrent GetTorr = new GetTorrent();
            //GetTorr.TorrSearcherIndex = ComboBox1_Searcher.SelectedIndex;
            GetTorr.TorrSearcherName = ComboBox_Searcher.SelectedItem.ToString();
            GetTorr.IEisHidden = checkBox_TP5_HideIe.Checked;
            GetTorr.QuitIe = checkBox_TP5_CloseIE.Checked;

            int ntextbox;
            int k = 0;
            foreach (CheckBox checkedBox in flowLayoutPanel_Cboxes.Controls)
            {
                if (checkedBox.Checked)
                {
                    ntextbox = Parameters.RowIndex2TboxIndex(Convert.ToInt16(checkedBox.Text));
                    GetTorr.Cbox5TboxesOldValues[k, 0] = checkedBox.Text;
                    for (int i = 1; i < 9; i++)
                    {
                        GetTorr.Cbox5TboxesOldValues[k, i] = ((TextBox)flowLayoutPanel_Tboxes.Controls["TextBox" + Convert.ToString(ntextbox + i - 1)]).Text;
                    }
                    k++;
                }
            }
            GetTorr.nCheckedBoxes = k;

            this.ControlBox = false; //prevents close mainform while worker is running
            backgroundWorker_GetTorrent.RunWorkerAsync(GetTorr);

        }

        private void backgroundWorker_GetTorrent_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // This method runs on the main thread simultaneously to worker.
            int nTextbox;
            int arrayCurrentRow;

            GetTorrent.CurrentState state2 = (GetTorrent.CurrentState)e.UserState;
            string colorName = state2.torrentFontColour.Name;
            //CheckBox cb = ((CheckBox)panel2.Controls[state2.cBox.Name]);
            arrayCurrentRow = state2.arrayCurrentRow;
            //cb.Checked = state2.cBox.Checked;
            nTextbox = Parameters.RowIndex2TboxIndex(Convert.ToInt16(state2.Cbox5TboxesNewValues[arrayCurrentRow, 0]));

            switch (colorName)
            {
                case "SpringGreen": //change only color
                    for (int i = 1; i < Parameters.nOfColumns + 1; i++)
                    {
                        TextBox tb = ((TextBox)flowLayoutPanel_Tboxes.Controls["TextBox" + (nTextbox + i - 1).ToString()]);
                        tb.ForeColor = state2.torrentFontColour;
                        tb.Refresh();
                    }
                    this.Activate();
                    break;
                default: //change color and value

                    for (int i = 1; i < Parameters.nOfColumns + 1; i++)
                    {
                        TextBox tb = ((TextBox)flowLayoutPanel_Tboxes.Controls["TextBox" + (nTextbox + i - 1).ToString()]);
                        tb.Text = state2.Cbox5TboxesNewValues[arrayCurrentRow, i];
                        tb.ForeColor = state2.torrentFontColour;
                        tb.Refresh();
                    }
                    break;
            }
        }

        private void backgroundWorker_GetTorrent_DoWork(object sender, DoWorkEventArgs e)
        {

            // Get the BackgroundWorker object that raised this event.
            BackgroundWorker worker;

            worker = (BackgroundWorker)sender;  //sender is backgroundworker1
            //change type of sender from generic object to background worker.is not conselho advisable make sender a backgroundworker instead Object in method parameters. Said by microsoft itself.

            GetTorrent GT = (GetTorrent)e.Argument;
            if (GT.nCheckedBoxes != 0) //no checked boxes then no need to call main method , complete will run anyway
            {
                GT.CallMagnetLink_in_Torrent_Indexer(worker, e);
            }
        }

        private void backgroundWorker_GetTorrent_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //even if cancelled this will run
            // SaveTabPage("MyTvShowsOrganizer" + label_NofPage.Text + ".ini", "tabPage1");

            this.Activate();

            this.panel_Footer.Cursor = Cursors.Default;
            this.button_GetTorrent.Cursor = Cursors.Default;
            this.ControlBox = true; //restore controlbox after complete worker.

            button_GetTorrent.ForeColor = Colours.buttonFont;
            button_GetTorrent.Refresh();
            this.Activate();
            if (Parameters.isGetTorrentStoped)
            {
                Parameters.isGetTorrentStoped = false;
            }
            else
            {
                if (AnycheckBox_IsRecursiveOn()) // Verify again (after next-airing)....if user turned on the recursive function on a CheckBox (show) while worker is running...
                {
                    Display_Next_airing(0); //clean checkboxes (in any occasion the user check other checkboxes (whit indeterminate/recursive flag) while worker is running.

                    try
                    {
                        timer_RecursiveInitiator.Enabled = true;
                    }
                    catch
                    {
                        timer_Recursive_Constructor();
                        timer_RecursiveInitiator.Enabled = true;
                    }

                }
                else if (this.toolStripMenuItem_GetAllPages.Checked) //if no more recursive then change page
                {
                    do
                    {
                        Thread.Sleep(300);
                        this.Refresh();
                        LoadNextPage(1, Convert.ToInt16(label_NofPage.Text));
                        RestoreColorRows(this.flowLayoutPanel_Tboxes);
                        this.Refresh();
                        Parameters.getAllCurrentPage = Convert.ToInt16(label_NofPage.Text);
                        Display_Next_airing(0);
                    } while (!(Parameters.isGetTorrentStoped || Parameters.getAllIniPage == Parameters.getAllCurrentPage || IsAnycheckBox_Checked())); //morgan for
                    //(!Parameters.isGetTorrentStoped && Parameters.getAllIniPage != Parameters.getAllCurrentPage && !IsAnycheckBox_Checked());
                    try
                    {
                        timer_GetAll_Torrents.Enabled = true;
                    }
                    catch
                    {
                        timer_GetAll_Constructor();
                        timer_GetAll_Torrents.Enabled = true; //1000
                    }
                }

                else
                {
                    Display_Next_airing(0); //clean checkboxes (in any occasion the user check other checkboxes (whit indeterminate/recursive flag) while worker is running.
                }
            }

            //BlinkControl(button_GetTorrent, 200, 2);
            button_GetTorrent.Enabled = true;
            ComboBox_Searcher.Enabled = true;
        }

        private void toolStripMenuItem_CBoxesRecursiveDownload_Click(object sender, EventArgs e)
        { //if checkbox state is ideterminate then turn on recursive gettorrent.
            CheckBox cBox;
            cBox = (CheckBox)contextMenuStrip_CBoxes_RecursiveTorrent.SourceControl;

            if (cBox.CheckState == CheckState.Indeterminate)
            {
                cBox.CheckState = CheckState.Unchecked;
            }
            else
            {
                cBox.CheckState = CheckState.Indeterminate;
            }
            BlinkControl(button_GetTorrent, 200, 3);
        }

        private bool AnycheckBox_IsRecursiveOn()
        {
            //will determine if torrentBackgroundWOrker will run again (recursivelly)
            foreach (CheckBox checkedbox in flowLayoutPanel_Cboxes.Controls)
            {
                if (checkedbox.CheckState == CheckState.Indeterminate)
                {
                    Parameters.isGetTorrentStoped = false;
                    return true;
                }
            }
            return false;
        }

        private void toolStripMenuItem_RecursiveUnckeck_Click(object sender, EventArgs e)
        {
            CheckBox Cbox = (CheckBox)contextMenuStrip_CBoxes_RecursiveTorrent.SourceControl;
            Cbox.Checked = false;
        }

        private void timer_RecursiveInitiator_Tick(object sender, EventArgs e)
        {
            timer_RecursiveInitiator.Enabled = false;
            if (Parameters.isGetTorrentStoped)
            {
                Parameters.isGetTorrentStoped = false;
                button_GetTorrent.Enabled = true;
                ComboBox_Searcher.Enabled = true;
                return;
            }

            InitiateGetTorrentWorker();  //run again
        }

        private void timer_GetAll_Tick(object sender, EventArgs e)
        {

            if (Parameters.getAllIniPage != Parameters.getAllCurrentPage)
            {
                timer_GetAll_Torrents.Enabled = false;

                if (Parameters.isGetTorrentStoped == true)
                {
                    Parameters.isGetTorrentStoped = false;
                    return;
                }
                Display_Next_airing();
                Thread.Sleep(100);
                // InitiateOperationGetTorrent();

                InitiateGetTorrentWorker();
            }
            else
            {
                timer_GetAll_Torrents.Enabled = false;
                toolStripMenuItem_GetAllPages.Checked = false;
                BlinkControl(button_GetTorrent, 200, 3);
            }
        }


        private void toolStripMenuItem_GetAllPages_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripMenuItem_GetAllPages.Checked)
            {
                button_GetTorrent.ForeColor = Colours.buttonFontGettingTorrentGetAll;
                Parameters.isGetTorrentStoped = false;
            }
            else
            {
                button_GetTorrent.ForeColor = Colours.buttonFont;
                //Parameters.isGetTorrentStoped = true;
            }

        }


        private void toolStripMenuItem_GetTorrentStop_Click(object sender, EventArgs e)
        {
            toolStripMenuItem_GetTorrentStop.Checked = false; //will ever unckeck this because no matters if checked or not.
            Parameters.isGetTorrentStoped = true;
            //if (toolStripMenuItem_GetAllPages.Checked)
            //{
            toolStripMenuItem_GetAllPages.Checked = false;  //will shot handler above
            // }
            //if (backgroundWorker_GetTorrent.IsBusy)
            //{
            GetTorrent_Stop();
            //}
        }

        private void GetTorrent_Stop()
        {

            // Parameters.isGetTorrentStoped = true; //tranfered to event handle getall.checked

            ComboBox_Searcher.Enabled = false;
            button_GetTorrent.Enabled = false;
            this.backgroundWorker_GetTorrent.CancelAsync();
        }

        #endregion Backgroundworker get torrent

        #region General Purpose Methods ##############################################################################

        private void SubtitlesLanguages()
        {

            this.comboBox_TP5_Language.Items.AddRange(new object[] {
"ALL",
"English",
"Afrikaans",
"Albanian",
"Arabic",
"Armenian",
"Basque",
"Belarusian",
"Bengali",
"Bosnian",
"Breton",
"Bulgarian",
"Burmese",
"Catalan",
"Chinese (simplified)",
"Chinese (traditional)",
"Chinese bilingual",
"Croatian",
"Czech",
"Danish",
"Dutch",
"Esperanto",
"Estonian",
"Finnish",
"French",
"Galician",
"Georgian",
"German",
"Greek",
"Hebrew",
"Hindi",
"Hungarian",
"Icelandic",
"Indonesian",
"Italian",
"Japanese",
"Kazakh",
"Khmer",
"Korean",
"Latvian",
"Lithuanian",
"Luxembourgish",
"Macedonian",
"Malay",
"Malayalam",
"Manipuri",
"Mongolian",
"Montenegrin",
"Norwegian",
"Occitan",
"Persian",
"Polish",
"Portuguese",
"Portuguese (BR)",
"Romanian",
"Russian",
"Serbian",
"Sinhalese",
"Slovak",
"Slovenian",
"Spanish",
"Swahili",
"Swedish",
"Syriac",
"Tagalog",
"Tamil",
"Telugu",
"Thai",
"Turkish",
"Ukrainian",
"Urdu",
"Vietnamese"
});
        }

        private void BlinkControl(Control myControl, int myMsec, int nTimes)
        {
            Color oldColor = myControl.BackColor;
            for (int x = 1; x <= nTimes; x++)
            {
                myControl.BackColor = Color.Green;
                myControl.Refresh();
                Thread.Sleep(myMsec);
                myControl.BackColor = oldColor;
                myControl.Refresh();
                Thread.Sleep(myMsec);
            }
        }

        private void label6_LastDownload_Click(object sender, EventArgs e)
        {
            //for admin purpose only
            if (backgroundWorker_GetTorrent.IsBusy) return;
            Write_In_All_TxtBoxes_Permition();
        }


        private void Write_In_All_TxtBoxes_Permition()
        {
            //easter egg to administration purpose only
            foreach (TextBox box in flowLayoutPanel_Tboxes.Controls)
            {
                box.ReadOnly = false;
            }
        }

        private void alwaysontop()
        {
            if (checkBox_TP5_AlwaysOnTop.Checked)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
        }


        private bool IsAnycheckBox_Checked()
        {
            //bool checkOk = false;
            foreach (CheckBox checkedbox in flowLayoutPanel_Cboxes.Controls)
            {
                if (checkedbox.Checked)
                {
                    return true;
                }
            }
            return false;
        }

        //tabpage1 
        private void Button_About_Click(object sender, EventArgs e)
        { //tell a friend
            Form aboutForm = new About();
            aboutForm.ShowDialog();
        }

        private void button_About_MouseDown(object sender, MouseEventArgs e)
        {
            button_About.BackgroundImage = global::MyTvShowsOrganizer.Properties.Resources.logo2green;
            button_About.Refresh();
        }

        private void button_About_MouseUp(object sender, MouseEventArgs e)
        {
            button_About.BackgroundImage = global::MyTvShowsOrganizer.Properties.Resources.logo;
            button_About.Refresh();
        }

        //HelpButton_Click
        private void Button_Help_Click(object sender, EventArgs e)
        {
            Help.MainHelp();
        }


        private bool FindInShowsListBox(string searchArg)
        {
            searchArg = searchArg.ToLower();
            listBox_TP6_ShowsNames.Select();
            int selected = listBox_TP6_ShowsNames.SelectedIndex;
            int nOfItems = listBox_TP6_ShowsNames.Items.Count;

            selected = listBox_TP6_ShowsNames.SelectedIndex;// +1;

            for (int i = 0; i < nOfItems; i++)
            {
                if (selected == nOfItems - 1) selected = 0;
                else selected++;
                string show = listBox_TP6_ShowsNames.Items[selected].ToString().ToLower();

                if (show.Contains(searchArg))
                {
                    //select this item in the ListBox.
                    listBox_TP6_ShowsNames.SelectedIndex = selected;
                    return true;
                }
            }
            return false;
        }


        private bool FindInListViewRanking(string searchArg)
        {
            searchArg = searchArg.ToLower();
            this.listView_TorrentRanking.Focus();

            if (this.listView_TorrentRanking.SelectedIndices.Count == 0)
            {
                this.listView_TorrentRanking.Items[0].Selected = true;
                this.listView_TorrentRanking.EnsureVisible(0);
            }

            int nOfItems = this.listView_TorrentRanking.Items.Count;

            int selected = this.listView_TorrentRanking.SelectedIndices[0];
            this.listView_TorrentRanking.Focus(); //without this the item is not going to select.

            for (int i = 0; i < nOfItems; i++)
            {
                if (selected == nOfItems - 1) selected = 0;
                else selected++;
                string show = this.listView_TorrentRanking.Items[selected].SubItems[1].Text.ToLower();

                if (show.Contains(searchArg))
                {
                    //select this item in the ListBox.
                    this.listView_TorrentRanking.Items[selected].Selected = true;
                    this.listView_TorrentRanking.EnsureVisible(selected);
                    return true;
                }

            }
            return false;
        }

        #endregion

        #region Preferences/Configuration Page  #############################################################################

        private void button_Config_Click(object sender, EventArgs e)
        {
            if (backgroundWorker_GetTorrent.IsBusy) return;
            Config_Page_Constructor();
            Go2Page(4); //tabpage5

        }

        private void Config_Page_Constructor()
        {
            Config_Page_Destructor();

            button_TP5_Cancel.Click += button_TP5_Cancel_Click;
            button_TP5_Ok.Click += button_TP5_Ok_Click;
            checkBox_TP5_HideIe.CheckedChanged += checkBox_TP5_HideIe_CheckedChanged;
            checkBox_TP5_BrowserSize.CheckedChanged += checkBox_TP5_BrowserSize_CheckedChanged;
            checkBox_TP5_AlwaysOnTop.CheckedChanged += checkBox_TP5_AlwaysOnTop_CheckedChanged;
            checkBox_TP5_TranslatorOn.CheckedChanged += checkBox_TP5_TranslatorOn_CheckedChanged;
            //  radioButton_BingTranslator.CheckedChanged += radioButton_BingTranslator_CheckedChanged;
        }

        private void Config_Page_Destructor()
        {
            button_TP5_Cancel.Click -= button_TP5_Cancel_Click;
            button_TP5_Ok.Click -= button_TP5_Ok_Click;
            checkBox_TP5_HideIe.CheckedChanged -= checkBox_TP5_HideIe_CheckedChanged;
            checkBox_TP5_BrowserSize.CheckedChanged -= checkBox_TP5_BrowserSize_CheckedChanged;
            checkBox_TP5_AlwaysOnTop.CheckedChanged -= checkBox_TP5_AlwaysOnTop_CheckedChanged;
            checkBox_TP5_TranslatorOn.CheckedChanged -= checkBox_TP5_TranslatorOn_CheckedChanged;
            //   radioButton_BingTranslator.CheckedChanged -= radioButton_BingTranslator_CheckedChanged;

        }

        //void radioButton_BingTranslator_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (radioButton_BingTranslator.Enabled)
        //    {
        //        Parameters.


        //    }
        //    else
        //    {
        //        radioButton_BingTranslator.Enabled = true;
        //        radioButton_GoogleTranslator.Enabled = true;
        //        //toolStripMenuItem_Translate.Click -= toolStripMenuItem_Translate_Click;
        //        contextMenuStrip_Translate.Enabled = true;
        //        toolStripMenuItem_Translate.Click += toolStripMenuItem_Translate_Click;
        //    }webTranslatorOn
        //}



        private void checkBox_TP5_TranslatorOn_CheckedChanged(object sender, EventArgs e)
        {
            Toggle_Translation();
        }

        private void Toggle_Translation()
        {
            if (checkBox_TP5_TranslatorOn.Checked)
            {
                Help_WebTranslator_TurnOn(true);
                radioButton_BingTranslator.Enabled = true;
                radioButton_GoogleTranslator.Enabled = true;

            }
            else
            {
                Help_WebTranslator_TurnOn(false);
                radioButton_BingTranslator.Enabled = false;
                radioButton_GoogleTranslator.Enabled = false;
            }


            //out because messagebox tranlation cant be turned off
            if (radioButton_BingTranslator.Checked)
            {
                Parameters.bingTranslatorOn = true;
            }
            else
            {
                Parameters.bingTranslatorOn = false;
            }

        }



        private void checkBox_TP5_BrowserSize_CheckedChanged(object sender, EventArgs e)
        {
            AddNewShow_DefineBrowserSize();
        }

        private void checkBox_TP5_AlwaysOnTop_CheckedChanged(object sender, EventArgs e)
        {
            alwaysontop();
        }

        private void button_TP5_Cancel_Click(object sender, EventArgs e)
        {
            LoadTabPage("MyTvShowsOrganizer" + label_NofPage.Text + ".ini", "tabPage1"); //undo any changes user has just made
            Go2Page(0);
            Config_Page_Destructor();
        }

        private void button_TP5_Ok_Click(object sender, EventArgs e)
        {
            if (TextBox_MaxMinNofPagesIsOk())
            {
                SaveTabPage("Config.ini", "tabPage5");

                Parameters.MaxNumberOfPages = Convert.ToInt16(this.textBox_TP5_NofPages.Text);
                if (Convert.ToInt16(label_NofPage.Text) > Parameters.MaxNumberOfPages)
                {
                    label_NofPage.Text = Parameters.MaxNumberOfPages.ToString();
                    LoadTabPage("MyTvShowsOrganizer" + label_NofPage.Text + ".ini", "tabPage1");
                }
                Go2Page(0);
                Config_Page_Destructor();
            }
            else
            {
                BlinkControl(label_TP5_NofPagesLimits, 250, 2);
                textBox_TP5_NofPages.Focus();
            }


        }

        private void checkBox_TP5_HideIe_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_TP5_HideIe.Checked)
            {
                checkBox_TP5_CloseIE.Checked = true;
                checkBox_TP5_CloseIE.Enabled = false;
            }
            else
            {
                checkBox_TP5_CloseIE.Enabled = true;
            }
        }

        private bool TextBox_MaxMinNofPagesIsOk()
        {
            int nOfPages;
            try
            {
                nOfPages = Convert.ToInt16(textBox_TP5_NofPages.Text);

                if (nOfPages > 20)
                {
                    textBox_TP5_NofPages.Text = "20";
                }
                else if (nOfPages < 2)
                {
                    textBox_TP5_NofPages.Text = "2";
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                textBox_TP5_NofPages.Text = "10";
            }
            return false;
        }

        #endregion

        #region Handlers for MouseDown Shows' Txtboxes. Modifies context menu (extended or short)

        private void RightClickMarker(TextBox tBox)
        {
            int rowIndex = Parameters.TboxIndex2RowIndex(Convert.ToInt16(tBox.Name.Substring(7)));
            ((RadioButton)flowLayoutPanel_RButtons.Controls["RadioButton" + rowIndex.ToString()]).Checked = true;
        }

        private void textBoxShows_MouseDown(object sender, MouseEventArgs e)
        {
            TextBox tBox = (TextBox)sender;
            RightClickMarker(tBox);
        }

        #endregion

        #region WebBrowser Internal

        //GoToTvCalendar_Click tappage1
        private System.Windows.Forms.Button button_InternalWebBrowser_Close;
        private System.Windows.Forms.Button button_InternalWebBrowser_Forward;
        private System.Windows.Forms.Button button_InternalWebBrowser_Backward;

        private void Button_TvCalendar_Click(object sender, EventArgs e)
        {
            try
            {
                if (timer_AddShowWebLoad_Disposer.Enabled)
                { //in the event of addshowwebbrowser was just closed. Disposeall before open again
                    timer_AddShowWebLoad_Disposer.Enabled = false;
                    AddNewShow_DisposeAll();
                }
            }
            catch
            {
            }

            Open_Internal_WebBrowser("http://www.pogdesign.co.uk/cat/");
        }

        private void Open_Internal_WebBrowser(string link)
        {

            Create_WebBrowser1();
            this.webBrowser1.IsWebBrowserContextMenuEnabled = true;
            webBrowser1.Navigate(link);
            Color internalWebButtonsColor = Color.DarkOrange;
            webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.Size = new Size(Parameters.AddNewShowWebBrowserSize + 188, 640);
            webBrowser1.BringToFront();
          

            this.button_InternalWebBrowser_Close = new Button();
            button_InternalWebBrowser_Close.Size = new Size(100, 21);
            button_InternalWebBrowser_Close.Text = "Close";
            button_InternalWebBrowser_Close.BackColor = internalWebButtonsColor;
            button_InternalWebBrowser_Close.Padding = new Padding(0, 0, 0, 2);
            button_InternalWebBrowser_Close.Location = new System.Drawing.Point(580, 6);
            button_InternalWebBrowser_Close.Click -= button_InternalWebBrowser_Close_Click;
            button_InternalWebBrowser_Close.Click += button_InternalWebBrowser_Close_Click;

            this.button_InternalWebBrowser_Forward = new Button();
            button_InternalWebBrowser_Forward.Size = new Size(50, 20);
            button_InternalWebBrowser_Forward.Text = ">>>";
            button_InternalWebBrowser_Forward.BackColor = internalWebButtonsColor;
            button_InternalWebBrowser_Forward.Padding = new Padding(0, 0, 0, 3);
            button_InternalWebBrowser_Forward.Location = new System.Drawing.Point(805, 6);
            button_InternalWebBrowser_Forward.Click -= button_InternalWebBrowser_Forward_Click;
            button_InternalWebBrowser_Forward.Click += button_InternalWebBrowser_Forward_Click;
           
            this.button_InternalWebBrowser_Backward = new Button();
            button_InternalWebBrowser_Backward.Size = new Size(50, 20);
            button_InternalWebBrowser_Backward.Text = "<<<";
            button_InternalWebBrowser_Backward.BackColor = internalWebButtonsColor;
            button_InternalWebBrowser_Backward.Padding = new Padding(0, 0, 0, 3);
            button_InternalWebBrowser_Backward.Location = new System.Drawing.Point(400, 6);
            button_InternalWebBrowser_Backward.Click -= button_InternalWebBrowser_Backward_Click;
            button_InternalWebBrowser_Backward.Click += button_InternalWebBrowser_Backward_Click;

            Go2Page(5);

            panel_Title.Controls.Add(button_InternalWebBrowser_Close);
            panel_Title.Controls.Add(button_InternalWebBrowser_Forward);
            ToolTip1.SetToolTip(button_InternalWebBrowser_Forward, "Alt + Right");
            panel_Title.Controls.Add(button_InternalWebBrowser_Backward);
            ToolTip1.SetToolTip(button_InternalWebBrowser_Backward, "Alt + Left or Backspace");

        }

        private void button_InternalWebBrowser_Backward_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void button_InternalWebBrowser_Forward_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void Create_WebBrowser1()
        {

            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.tabPage6.Controls.Add(this.webBrowser1);
            this.webBrowser1.DocumentText = Parameters.blankWebPage;
            webBrowser1.Update();
            this.webBrowser1.AllowWebBrowserDrop = false;
            this.webBrowser1.WebBrowserShortcutsEnabled = true;
            this.webBrowser1.Margin = new System.Windows.Forms.Padding(0);
            //this.webBrowser1.MinimumSize = new Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            //this.webBrowser1.Url = new System.Uri("", System.UriKind.Relative);

        }

        private void button_InternalWebBrowser_Close_Click(object sender, EventArgs e)
        {
            this.Opacity = 0;
            button_InternalWebBrowser_Backward.Dispose();
            button_InternalWebBrowser_Close.Dispose();
            button_InternalWebBrowser_Forward.Dispose();

            Go2Page(0); //tabpage1
            webBrowser1.Dispose();
        }

        #endregion

        #region ADD new Show

        /* listbox genres and listbox summaries are hidden and below webbrowser */

        private System.Windows.Forms.Timer timer_AddShowWebLoad01; //works with timer2 to load webpages only after 2 seconds after an item is selected.
        private System.Windows.Forms.Timer timer_AddShowWebLoad02; //idem
        private System.Windows.Forms.Timer timer_AddShowWebLoad03; //when right-click menu is open, 5 seconds is elapse before selected show webpage is opened.
        private System.Windows.Forms.Timer timer_AddShowCountDownDate; // will get the countdown time to new shows from id=countdown
        private System.Windows.Forms.Timer timer_AddShowWebLoad_Disposer; //after adding a new show, this timer will dispose all others timers and webbrowser after 30 seconds. (to save memory)
        private System.Windows.Forms.WebBrowser webBrowser1;



        private void toolStripMenuItem_TxtBoxAddShow_Click(object sender, EventArgs e)
        {
            button_TP6_Add.Enabled = true;
            toolStripMenuItem_AddNewShow_Add.Enabled = true;
            listBox_TP6_ShowsNames.Enabled = true;

            OpenAddNewShowWebBrowser();
        }

        private void OpenAddNewShowWebBrowser()
        {

            if (backgroundWorker_GetTorrent.IsBusy) return;

            if (Parameters.areNewShowDisposableControlDisposed)
            {
                Create_AddNewShow_DisposableControls();
                AddNewShow_Page_Constructor();
            }
            else
            {
                timer_AddShowWebLoad_Disposer.Enabled = false;
                this.webBrowser1.Size = new Size(Parameters.AddNewShowWebBrowserSize, 620); //needed because user might change browsersize preference meanwhile.
            }


            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            if (Parameters.NewShows_CurrentGenreItem.Name == "") //was not  loaded yet
            {
                GetShowList();

                Parameters.NewShows_CurrentGenreText = "All";
                label_TP6_Filter.Text = "Filter: " + Parameters.NewShows_CurrentGenreText;
                Parameters.listBoxNewShowNamesNofItems = listBox_TP6_ShowsNames.Items.Count;
                listBox_TP6_ShowsNames.SelectedIndex = Parameters.NewShows_RandomIndex(Parameters.listBoxNewShowNamesNofItems); //to randomize the shows that is first opened. Because I dont want change tvCalendar statistics.
                toolStripMenuItem_AddNewShowGenre.DropDownItems[0].ForeColor = Colours.showsFontGettingTorrentNotFound;
                Parameters.NewShows_CurrentGenreItem = (ToolStripMenuItem)toolStripMenuItem_AddNewShowGenre.DropDownItems[0];
            }
            else
            {
                if (webBrowser1.Url.ToString() != Parameters.listBoxNewShowPreviousUrl)  //reload only if url is different of previous
                {
                    webBrowser1.Navigate(Parameters.listBoxNewShowPreviousUrl);
                }
            }

            TextBox tBox_ShowName = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;
            Parameters.TboxNewShowsAdded = tBox_ShowName.Name;
            ToolTip1.SetToolTip(tBox_ShowName, ""); //not need clearthiscomment, 

            Go2Page(5);
            tabControl1.Refresh();
            listBox_TP6_ShowsNames.Focus();
            System.Windows.Forms.Cursor.Current = Cursors.Default;
            listBox_TP6_ShowsNames.Focus();

        }

        private void Create_AddNewShow_DisposableControls()
        {

            Create_WebBrowser1();
            this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser1.ContextMenuStrip = contextMenuStrip_AddNewShow;
            this.webBrowser1.Size = new Size(Parameters.AddNewShowWebBrowserSize, 620);
            this.webBrowser1.Location = new Point(Parameters.AddNewShowWebBrowserPosition, 7); //184 190
            webBrowser1.SendToBack();


            this.timer_AddShowWebLoad01 = new System.Windows.Forms.Timer(this.components);
            this.timer_AddShowWebLoad02 = new System.Windows.Forms.Timer(this.components);
            this.timer_AddShowWebLoad03 = new System.Windows.Forms.Timer(this.components);
            this.timer_AddShowCountDownDate = new System.Windows.Forms.Timer(this.components);
            this.timer_AddShowWebLoad_Disposer = new System.Windows.Forms.Timer(this.components);
            // 
            // timer_AddShowWebLoad01
            // 
            this.timer_AddShowWebLoad01.Interval = 1000;
            this.timer_AddShowWebLoad01.Tick += new System.EventHandler(this.timer_AddShowWebLoad01_Tick);
            // 
            // timer_AddShowWebLoad02
            // 
            this.timer_AddShowWebLoad02.Interval = 1000;
            this.timer_AddShowWebLoad02.Tick += new System.EventHandler(this.timer_AddShowWebLoad02_Tick);
            // 
            // timer_AddShowWebLoad03
            // 
            this.timer_AddShowWebLoad03.Interval = 5000;
            this.timer_AddShowWebLoad03.Tick += new System.EventHandler(this.timer_AddShowWebLoad03_Tick);
            // 
            // timer_AddShowCountDownDate
            // 
            this.timer_AddShowCountDownDate.Interval = 2000;
            this.timer_AddShowCountDownDate.Tick += new System.EventHandler(this.timer_AddShowCountDownDate_Tick);
            // 
            // timer_AddShowWebLoad_disposer
            // 
            this.timer_AddShowWebLoad_Disposer.Interval = 15000;
            this.timer_AddShowWebLoad_Disposer.Tick += new System.EventHandler(this.timer_AddShowWebLoad_Disposer_Tick);

            //add shows genres click eventhandlers
            foreach (var genreDropDownItem in this.toolStripMenuItem_AddNewShowGenre.DropDownItems)
            {
                if (genreDropDownItem is ToolStripMenuItem) //not a ToolStripSeparator
                {
                    ToolStripDropDownItem genreItem = (genreDropDownItem as ToolStripDropDownItem);
                    // this.toolStripMenuItem_Genre12.Click += new System.EventHandler(this.toolStripMenuItem_Genre12_Click);
                    genreItem.Click -= toolStripMenuItem_AddNewShow_Genre_Click; //unregister if already created (no problem if it is non existent)
                    genreItem.Click += toolStripMenuItem_AddNewShow_Genre_Click; //create new
                }
            }
        }

        private void timer_AddShowWebLoad_Disposer_Tick(object sender, EventArgs e)
        { //dispose all . save some RAM 

            AddNewShow_DisposeAll();
        }

        private void AddNewShow_DisposeAll()
        {
            Ranking_Page_Destructor();
            AddNewShow_Page_Destructor();

            timer_AddShowWebLoad01.Dispose();
            timer_AddShowWebLoad02.Dispose();
            timer_AddShowWebLoad03.Dispose();
            timer_AddShowCountDownDate.Dispose();

            webBrowser1.Dispose();

            Parameters.areNewShowDisposableControlDisposed = true;
            Parameters.NewShows_CurrentGenreItem.Name = "";
            Parameters.CodHtmlShowListStr = "pogdesign";
            listBox_TP6_ShowsGenres.Items.Clear();
            listBox_TP6_ShowsNames.Items.Clear();
            listBox_TP6_ShowsSummaries.Items.Clear();

            //destructor of 20 handlersevents
            foreach (var genreDropDownItem in this.toolStripMenuItem_AddNewShowGenre.DropDownItems)
            {
                if (genreDropDownItem is ToolStripMenuItem) //not a ToolStripSeparator
                {
                    ToolStripDropDownItem genreItem = (genreDropDownItem as ToolStripDropDownItem);
                    // this.toolStripMenuItem_Genre12.Click += new System.EventHandler(this.toolStripMenuItem_Genre12_Click);
                    genreItem.Click -= toolStripMenuItem_AddNewShow_Genre_Click;

                }
            }

            timer_AddShowWebLoad_Disposer.Dispose();

        }

        private void AddNewShow_Page_Constructor()
        {

            listBox_TP6_ShowsNames.SelectedIndexChanged += listBox_TP6_ShowsNames_SelectedIndexChanged;
            button_TP6_Add.Click += button_TP6_Add_Click;
            button_TP6_GoBack.Click += button_TP6_GoBack_Click;
            button_TP6_GoFind.Click += button_TP6_GoFind_Click;
            button_TP6_Ranking.Click += button_TP6_Ranking_Click;
            button_TP6_TvCalendar.Click += button_TP6_TvCalendar_Click;
            listBox_TP6_ShowsNames.MouseHover += listBox_ShowsNames_MouseHover;
            listBox_TP6_ShowsNames.MouseLeave += listBox_ShowsNames_MouseLeave;
            listBox_TP6_ShowsNames.MouseDown += listBox_ShowsNames_MouseDown;
            textBox_TP6_FindInShowsNamesList.KeyDown += textBox_TP6_FindInShowsNamesList_KeyDown;

            toolStripMenuItem_AddNewShow_Add.Click += toolStripMenuItem_AddNewShow_Add_Click;
            toolStripMenuItem_AddNewShow_IMDB.Click += toolStripMenuItem_AddNewShow_IMDB_Click;
            toolStripMenuItem_AddNewShow_Trailer.Click += toolStripMenuItem_AddNewShow_Trailer_Click;
            toolStripMenuItem_AddNewShow_Prediction.Click += toolStripMenuItem_AddNewShow_Prediction_Click;
            toolStripMenuItem_AddNewShow_TvRanking.Click += toolStripMenuItem_AddNewShow_TvRanking_Click;
            toolStripMenuItem_AddNewShow_ViewTorrentAvailability.Click += toolStripMenuItem_AddNewShow_ViewTorrentAvailability_Click;
            toolStripMenuItem_AddNewShow_Summary.Click += toolStripMenuItem_AddNewShow_Summary_Click;
            toolStripMenuItem_AddNewShow_TvCalendar.Click += toolStripMenuItem_AddNewShow_TvCalendar_Click;
            ;

            contextMenuStrip_AddNewShow.Opening += contextMenuStrip_AddNewShow_Opening;
            contextMenuStrip_AddNewShow.Closed += contextMenuStrip_AddNewShow_Closed;

        }

        private void AddNewShow_Page_Destructor()
        {
            listBox_TP6_ShowsNames.SelectedIndexChanged -= listBox_TP6_ShowsNames_SelectedIndexChanged;
            button_TP6_Add.Click -= button_TP6_Add_Click;
            button_TP6_GoBack.Click -= button_TP6_GoBack_Click;
            button_TP6_GoFind.Click -= button_TP6_GoFind_Click;
            button_TP6_Ranking.Click -= button_TP6_Ranking_Click;
            button_TP6_TvCalendar.Click -= button_TP6_TvCalendar_Click;
            listBox_TP6_ShowsNames.MouseHover -= listBox_ShowsNames_MouseHover;
            listBox_TP6_ShowsNames.MouseLeave -= listBox_ShowsNames_MouseLeave;
            listBox_TP6_ShowsNames.MouseDown -= listBox_ShowsNames_MouseDown;
            textBox_TP6_FindInShowsNamesList.KeyDown -= textBox_TP6_FindInShowsNamesList_KeyDown;

            toolStripMenuItem_AddNewShow_Add.Click -= toolStripMenuItem_AddNewShow_Add_Click;
            toolStripMenuItem_AddNewShow_IMDB.Click -= toolStripMenuItem_AddNewShow_IMDB_Click;
            toolStripMenuItem_AddNewShow_Trailer.Click -= toolStripMenuItem_AddNewShow_Trailer_Click;
            toolStripMenuItem_AddNewShow_Prediction.Click -= toolStripMenuItem_AddNewShow_Prediction_Click;
            toolStripMenuItem_AddNewShow_TvRanking.Click -= toolStripMenuItem_AddNewShow_TvRanking_Click;
            toolStripMenuItem_AddNewShow_ViewTorrentAvailability.Click -= toolStripMenuItem_AddNewShow_ViewTorrentAvailability_Click;
            toolStripMenuItem_AddNewShow_Summary.Click -= toolStripMenuItem_AddNewShow_Summary_Click;
            toolStripMenuItem_AddNewShow_TvCalendar.Click -= toolStripMenuItem_AddNewShow_TvCalendar_Click;

            contextMenuStrip_AddNewShow.Opening -= contextMenuStrip_AddNewShow_Opening;
            contextMenuStrip_AddNewShow.Closed -= contextMenuStrip_AddNewShow_Closed;

        }

        void button_TP6_TvCalendar_Click(object sender, EventArgs e)
        {
            AddNewShow_TvCalendar();
        }

        private void AddNewShow_TvCalendar()
        {

            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            webBrowser1.Navigate("http://www.pogdesign.co.uk/cat/");
            Parameters.listBoxNewShowPreviousUrl = "http://www.pogdesign.co.uk/cat/";
            Thread.Sleep(250);
            System.Windows.Forms.Cursor.Current = Cursors.Default;

        }
        void listBox_TP6_ShowsNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            //While the list of shows are changing indexes, timer2 will be ever stoped and webpage will not load.
            //only after 2 seconds in standby (1 for timer1 e 1 for timer2), webpage will load.

            if (!Parameters.NewShows_Filtering)//to prevente error when removing items during filtering
            {
                timer_AddShowWebLoad01.Enabled = true;
                timer_AddShowWebLoad02.Enabled = false;
                if (listBox_TP6_ShowsNames.SelectedIndex == -1) //true when user clicks on listbox but not over an item.
                {
                    listBox_TP6_ShowsNames.SelectedIndex = listBox_TP6_ShowsGenres.SelectedIndex; //retreave last name selected
                }
                listBox_TP6_ShowsGenres.SelectedIndex = listBox_TP6_ShowsNames.SelectedIndex;
                toolStripMenuItem_AddNewShowGenre.Text = "Genre:" + listBox_TP6_ShowsGenres.SelectedItem.ToString() + "  |Filter: --";
            }
        }

        private void toolStripMenuItem_AddNewShow_Add_Click(object sender, EventArgs e)
        {
            AddSelectedShow();
        }

        private void button_TP6_Add_Click(object sender, EventArgs e)
        {
            AddSelectedShow();
        }

        private void button_TP6_GoBack_Click(object sender, EventArgs e)
        {
            Go2Page(0); //tabpage1
            Parameters.listBoxNewShowPreviousUrl = webBrowser1.Url.ToString(); //save to open the same page if user add new
            webBrowser1.DocumentText = Parameters.blankWebPage;
            timer_AddShowWebLoad_Disposer.Enabled = true; //initiate countdown to dispose timers and webbrowser
            Parameters.areNewShowDisposableControlDisposed = false;  //will prevent recreation of timers if timer4 interval not elapse when user tries add new Show.

        }

        private void button_TP6_GoFind_Click(object sender, EventArgs e)
        {
            FindInShowsListBox(textBox_TP6_FindInShowsNamesList.Text);
        }

        void textBox_TP6_FindInShowsNamesList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FindInShowsListBox(textBox_TP6_FindInShowsNamesList.Text);
            }
            textBox_TP6_FindInShowsNamesList.Focus();
        }

        private void button_TP6_Ranking_Click(object sender, EventArgs e)
        {  //add new show page

            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            Ranking_Page_Constructor();
            //storp timers to prevent error.
            timer_AddShowWebLoad01.Enabled = false;
            timer_AddShowWebLoad02.Enabled = false;

            Control myControl = (Control)sender;
            Control parentControl = myControl.Parent;

            string presearch = listBox_TP6_ShowsNames.SelectedItem.ToString();

            Initiate_Torrent_Ranking_Analizes(5, myControl.Left, myControl.Bottom, myControl.Width, parentControl, presearch);

            listView_TorrentRanking.ContextMenuStrip = contextMenuStrip_Ranking;
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void AddSelectedShow()
        {

            Parameters.listBoxNewShowPreviousUrl = webBrowser1.Url.ToString();//save to open the same page if user add new
            string showName = listBox_TP6_ShowsNames.SelectedItem.ToString();
            ((TextBox)flowLayoutPanel_Tboxes.Controls[Parameters.TboxNewShowsAdded]).Text = showName;
            ((CheckBox)flowLayoutPanel_Cboxes.Controls["checkBox" + Parameters.TboxIndex2RowIndex(Convert.ToInt16(Parameters.TboxNewShowsAdded.Substring(7)))]).CheckState = CheckState.Indeterminate; //will mark to recursive torrent download;
            Go2Page(0); //tabpage1
            this.Refresh();
            Display_Next_airing();

            timer_AddShowWebLoad_Disposer.Enabled = true; //initiate countdown to dispose timers.
            Parameters.areNewShowDisposableControlDisposed = false;  //will prevent recreation of timers if timer4 interval not elapse when user tries add new Show.
            int tBoxIndex = Convert.ToInt16(Parameters.TboxNewShowsAdded.Substring(7));
            string nextEp = ((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + (tBoxIndex + Parameters.nextEpisodeColumn - 1).ToString()]).Text;

            webBrowser1.DocumentText = Parameters.blankWebPage;

            if (webBrowser1.AccessibleName.Contains("seconds") && nextEp.Contains("Mid/EndSeason")) //countdown to next episode from tvcalendar summary
            {
                // SaveComment("MyTvShowsComments" + label_NofPage.Text + ".ini", Parameters.TboxNewShowsAdded.Substring(7), webBrowser1.AccessibleName,true);
                //((TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + (tBoxIndex + Parameters.nextEpisodeColumn - 1).ToString()]).Text = webBrowser1.AccessibleName;

                MsgBox.Show(@"The Next Episode of " + showName + @" is going to air in:
" + webBrowser1.AccessibleName + @".
Next Episode function only return values that are less than 30 days. 
If You want Download Previous Episodes, Enter the appropriate Episode 
and Season before Clicking 'GetTorrent'.", "TV-Show is not Currently Airing", MsgBox.Buttons.OK, MsgBox.Icone.Info, MsgBox.AnimateStyle.ZoomIn);
            }
            else if (nextEp.Contains("Mid/EndSeason"))
            {
                MsgBox.Show(showName + @" is Currently in: Mid of Season / End of Season.
There is No Date Available to the Next Episode. 
" + webBrowser1.AccessibleName + @".
If You want Download Previous Episodes, Enter the 
appropriate Episode and Season before Clicking 'GetTorrent'.", "TV-Show is not Currently Airing", MsgBox.Buttons.OK, MsgBox.Icone.Info, MsgBox.AnimateStyle.FadeIn);
            }
        }

        private void GetShowList()
        {

            string myPatternName = @"(?<=alt="").*(?="" />)";  /*match prefix(but exclude from capture alt=") match sufix " /> but exclude from capture. Will match the value of  'alt' tag  About A Boy and Adventures Time */
            string myPatternSummary = @"(?<=href=""\.).*-summary";
            string myPatternGenre = @"(?<=Genre:).*(?= //)";
            //// http://www.pogdesign.co.uk/cat/A-Place-To-Call-Home-summary

            string codhtml;
            if (Parameters.CodHtmlShowListStr == "pogdesign")
            //prevent more than one access per session over pogdesign next-airing webpage.
            {
                Parameters.CodHtmlShowListStr = Parameters.CodHtmlShowListExtractor();
                codhtml = Parameters.CodHtmlShowListStr;
            }
            else
            {
                codhtml = Parameters.CodHtmlShowListStr;
            }

            //<div id="check819" class="selectgrp greybar" > 
            //    <label class="label_check" >
            //        <img src="/cat/imgs/thumbvert/noimage.jpg" data-original="/cat/imgs/thumbvert/Adventure-Time.jpg" class="buttimg lazy" alt="Adventure Time" />
            //            <input class="checkbox" type="checkbox" value="819"  style="display:none;" /><strong>Adventure Time</strong> 
            //            <span class="status">Genre: Action // <span>Status: Returning</span></span> 
            //            <a href="./Adventure-Time-summary" title="View Show Summary for Adventure Time" class="slink">View Adventure Time Summary</a>	
            //    </label>
            //</div>
            listBox_TP6_ShowsGenres.Items.Clear();
            listBox_TP6_ShowsSummaries.Items.Clear();
            listBox_TP6_ShowsNames.Items.Clear();
            foreach (Match imgAltTag_ShowName in Regex.Matches(codhtml, myPatternName))
            {
                listBox_TP6_ShowsNames.Items.Add(imgAltTag_ShowName.Value.Replace("&amp;", "&").Trim());
            }

            Parameters.listBoxNewShowNamesNofItems = listBox_TP6_ShowsNames.Items.Count;

            foreach (Match imgAltTag_ShowSummary in Regex.Matches(codhtml, myPatternSummary))
            {
                listBox_TP6_ShowsSummaries.Items.Add(imgAltTag_ShowSummary.Value);
            }
            foreach (Match imgAltTag_ShowGenre in Regex.Matches(codhtml, myPatternGenre))
            {
                listBox_TP6_ShowsGenres.Items.Add(imgAltTag_ShowGenre.Value.Replace("&nbsp;", " ").Replace("Scripted", "Adventure").Trim());
            }

            Parameters.NewShows_CurrentGenreText = "All";
            label_TP6_Filter.Text = "Filter: " + Parameters.NewShows_CurrentGenreText;
        }

        private void toolStripMenuItem_AddNewShow_ViewTorrentAvailability_Click(object sender, EventArgs e)
        {
            // https://kat.cr/usearch/American%20Odyssey%20720p%20x264%20OR%20hdtv%20category%3Atv/?field=seeders&sorder=desc
            // American Odyssey 720p x264 OR hdtv category:tv
            string showsName = listBox_TP6_ShowsNames.SelectedItem.ToString();
            showsName = Parameters.ClearStr(showsName);
            //showsName.Replace(" ", "%20");
            webBrowser1.Navigate(@"https://kat.cr/usearch/" + showsName + " 720p OR 1080p OR HDTV" + "/?field=time_add&sorder=desc");
        }//" 720p OR 1080p OR HDTV" + "/?field=time_add&sorder=desc"

        private void toolStripMenuItem_AddNewShow_IMDB_Click(object sender, EventArgs e)
        { //only works for IMDB because good regex 
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            string showsName = listBox_TP6_ShowsNames.SelectedItem.ToString();
            string codhtml = Parameters.CodHtmlGoogle2AnySite("https://www.google.com/search?q=" + "site:imdb.com" + "+" + "%22" + showsName + "%22" + "+ Tv Series");
            string myPattern = @"www.imdb.com/title/tt[0-9]{7}/";

            string finalHyper = Regex.Match(codhtml, myPattern).Value; //works only with well defined pattern (alfa-numeric code)
            // www.imdb.com/title/tt0319931 / 
            webBrowser1.Navigate(finalHyper);
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void toolStripMenuItem_AddNewShow_Trailer_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            string showsName = listBox_TP6_ShowsNames.SelectedItem.ToString();
            string codhtml = Parameters.CodHtmlGoogle2AnySite("https://www.google.com/search?q=" + "site:youtube.com" + "+" + "%22" + showsName + "%22" + "+Trailer+Official");

            //www.youtube.com/watch?v=xdm7Z3TQhDg
            string myPattern = @"www.youtube.com/watch\?v=\w{11}";

            string finalHyper = Regex.Match(codhtml, myPattern).Value;
            // www.imdb.com/title/tt0319931 /
            webBrowser1.Navigate(finalHyper);
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void toolStripMenuItem_AddNewShow_Prediction_Click(object sender, EventArgs e)
        { //Opens tvbythenumbers predictions page.
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            ShowsPrectition(useInternalWebBrowser: true);
            System.Windows.Forms.Cursor.Current = Cursors.Default;

        }

        private void toolStripMenuItem_AddNewShow_TvRanking_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            double decade;
            decade = DateTime.Today.Year;
            decade = Math.Floor((decade - 2000) / 10) * 10 + 2000;
            string decadeStr = Convert.ToInt16(decade).ToString() + "s";
            webBrowser1.Navigate("http://www.tv.com/shows/decade/" + decadeStr + "/");
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void ShowsPrectition(Boolean useInternalWebBrowser)
        {
            //href="http://tvbythenumbers.zap2it.com/2015/04/27/bubble-watch-2015-edition/394451/"
            //TextBox tBoxShowsName = null;
            //tBoxShowsName = (TextBox)ContextMenuStrip_TxtBox_Shows.SourceControl;
            string searchString = "https://www.google.com/search?q=" + "site:tvbythenumbers.zap2it.com" + "+" + "%22" + "bubble-watch" + "%22" + "+ edition";
            string codhtml = Parameters.CodHtmlGoogle2AnySite(searchString);
            string myPattern = @"tvbythenumbers\.zap2it\.com/\d{4}/\d{2}/\d{2}/bubble-watch-\d{4}-edition/\d{6}/";

            string finalHyper = "http://" + Regex.Match(codhtml, myPattern).Value;
            // www.imdb.com/title/tt0319931 /

            if (finalHyper != "") //if regex is not good (direct aprouch)
            {
                if (useInternalWebBrowser)
                {
                    webBrowser1.Navigate(finalHyper);
                }
                else
                {
                    Go2Web.OpenLink(finalHyper);
                }
            }
            else
            {
                Go2Web.GoogleIt("tvbythenumbers.zap2it.com", "", "bubble watch " + DateTime.Today.ToString("yyyy") + " edition");
            }
        }

        private void toolStripMenuItem_AddNewShow_Summary_Click(object sender, EventArgs e)
        { //go back to summary
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            string newWebPage2Open = SummaryWebPageUrl();
            webBrowser1.Navigate(newWebPage2Open);
            Parameters.listBoxNewShowPreviousUrl = newWebPage2Open;
            Thread.Sleep(250);
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void toolStripMenuItem_AddNewShow_TvCalendar_Click(object sender, EventArgs e)
        {

            AddNewShow_TvCalendar();

        }

        private string SummaryWebPageUrl()
        {

            listBox_TP6_ShowsSummaries.SelectedIndex = listBox_TP6_ShowsNames.SelectedIndex;
            return "http://www.pogdesign.co.uk/cat" + listBox_TP6_ShowsSummaries.SelectedItem.ToString();
        }

        private void listBox_ShowsNames_MouseDown(object sender, MouseEventArgs e)
        //to select (FOCUS) an item with rightclick and then open menu. Without this no focus change.
        {
            listBox_TP6_ShowsNames.SelectedIndex = listBox_TP6_ShowsNames.IndexFromPoint(e.X, e.Y);
        }

        /*timers 1 and 2 will alternate (on off) to prevent excessive webpages opening. Ex. when user uses direction keys to navigate listbox items, we dont want to open each item*/
        private void timer_AddShowWebLoad01_Tick(object sender, EventArgs e)
        {
            timer_AddShowWebLoad01.Enabled = false;
            timer_AddShowWebLoad02.Enabled = true;
        }

        private void timer_AddShowWebLoad02_Tick(object sender, EventArgs e)
        { //this timer open new selected shows from list
            timer_AddShowWebLoad02.Enabled = false;

            string newWebPage2Open = SummaryWebPageUrl();
            if (newWebPage2Open != Parameters.listBoxNewShowPreviousUrl)  //if is the same, dont open again
            {
                System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                webBrowser1.Navigate(newWebPage2Open);
                Parameters.listBoxNewShowPreviousUrl = newWebPage2Open;

                Thread.Sleep(250);
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                if (newWebPage2Open.Contains("summary")) timer_AddShowCountDownDate.Enabled = true;
            }

            //when webpage is load, timer1 and 2 will be stoped.
        }

        void timer_AddShowCountDownDate_Tick(object sender, EventArgs e)
        {
            HtmlElement nextDateCountDown = webBrowser1.Document.GetElementById("countdown");
            if (nextDateCountDown == null)
            {
                webBrowser1.AccessibleName = "";
            }
            else
            {
                webBrowser1.AccessibleName = nextDateCountDown.InnerHtml; //countdown like days, hours, seconds to the next episode
            }
            string codhtml = webBrowser1.DocumentText;
            //  <div style=" border-right: none;"><span>Number of Episodes Tracked :</span> 1 Episodes</div>  

            if (codhtml.Contains("Number of Episodes Tracked"))
            {//(?<=href=""/\w{40}"">).{3,70}(?=</a>)
                webBrowser1.AccessibleName += " | Episodes Tracked:" + Regex.Match(codhtml, @"(?<=Number of Episodes Tracked :</span>).{10,20}(?=</div>)").Value;
            }
            timer_AddShowCountDownDate.Enabled = false;
        }

        private void timer_AddShowWebLoad03_Tick(object sender, EventArgs e)
        {
            //will open after 5+1 seconds even if right-click
            timer_AddShowWebLoad03.Enabled = false; //auto stop
            timer_AddShowWebLoad02.Enabled = true;
        }

        private void contextMenuStrip_AddNewShow_Opening(object sender, CancelEventArgs e)
        {
            //right-click context menu
            Thread.Sleep(100);// because 2 events run at same time and this must be second.
            //if contextmenu 'add show' opens (in a differt right-click show listed), then, prevent auto webload.
            timer_AddShowWebLoad01.Enabled = false; //dont open immediatelly but...
            timer_AddShowWebLoad03.Enabled = true; //...initiate contdown 5 seconds (timer 3) to open regardless.
        }

        private void contextMenuStrip_AddNewShow_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {//if another item is choosen before timer3 interval elapsing then
            timer_AddShowWebLoad03.Enabled = false;
        }

        /* to mouse wheel do its work without click */
        private void listBox_ShowsNames_MouseHover(object sender, EventArgs e)
        {
            listBox_TP6_ShowsNames.Focus();
        }

        private void listBox_ShowsNames_MouseLeave(object sender, EventArgs e)
        {
            //misteriously, this only works with 3 focus.
            webBrowser1.Focus();
            Thread.Sleep(100);
            listBox_TP6_ShowsNames.Focus();
            Thread.Sleep(100);
            webBrowser1.Focus();
        }

        //filter by genre menu

        private void NewShowsClicksHandlesInitiate(ToolStripMenuItem myGenre)
        {
            if (Parameters.NewShows_CurrentGenreItem.Name != "")
            {
                Parameters.NewShows_CurrentGenreItem.ForeColor = Color.Gray;
            }
            myGenre.ForeColor = Colours.showsFontGettingTorrentNotFound;
            Parameters.NewShows_CurrentGenreItem = myGenre;
            GenreFilter(myGenre.Text);
        }

        private void toolStripMenuItem_AddNewShow_Genre_Click(object sender, EventArgs e)
        { //generic event for all show genres. This stay out of constructor
            ToolStripMenuItem myGenre = (ToolStripMenuItem)sender;
            NewShowsClicksHandlesInitiate(myGenre);
        }

        private void GenreFilter(string myGenre)
        {
            //Parameters.NewShows_CurrentGenre == "All" && 

            if (Parameters.NewShows_CurrentGenreText != myGenre)
            {
                Parameters.NewShows_Filtering = true; //prevents error on listbox_showsnames event handle after remove item. 

                if (Parameters.NewShows_CurrentGenreText != "All")
                {
                    GetShowList(); //renew all listboxes 
                }

                switch (myGenre)
                {
                    case "All":

                        break;

                    default:

                        int nGenresIndexes = Parameters.listBoxNewShowNamesNofItems - 1;
                        int[] indexes2BeKeeped = new int[nGenresIndexes];
                        int n = 0;

                        for (int genreListIndex = 0; genreListIndex < nGenresIndexes; genreListIndex++)
                        {
                            if (listBox_TP6_ShowsGenres.Items[genreListIndex].ToString() == myGenre)
                            {
                                indexes2BeKeeped[n] = genreListIndex;
                                n++;
                            }
                        }

                        if (n == 0)
                        {
                            MsgBox.Show("Sorry, no series with " + myGenre + " genre...", "Nothing to Show", MsgBox.Buttons.OK, MsgBox.Icone.Exclamation, MsgBox.AnimateStyle.ZoomIn);
                            GenreFilter("All");

                        }
                        else
                        {
                            Array.Resize(ref indexes2BeKeeped, n);
                            Parameters.listBoxNewShowNamesNofItems = n;
                            FilterListBoxShows(listBox_TP6_ShowsGenres, indexes2BeKeeped);
                            FilterListBoxShows(listBox_TP6_ShowsSummaries, indexes2BeKeeped);
                            FilterListBoxShows(listBox_TP6_ShowsNames, indexes2BeKeeped);
                            Parameters.NewShows_CurrentGenreText = myGenre; //prevents repeat process for the current genre.
                            label_TP6_Filter.Text = "Filter: " + Parameters.NewShows_CurrentGenreText;
                        }

                        break;
                }

                Parameters.NewShows_Filtering = false;

                listBox_TP6_ShowsNames.SelectedIndex = Parameters.NewShows_RandomIndex(Parameters.listBoxNewShowNamesNofItems);
                listBox_TP6_ShowsGenres.SelectedIndex = listBox_TP6_ShowsNames.SelectedIndex;
            }
        }

        private void FilterListBoxShows(ListBox myListBox, int[] indexes2BeKeeped)
        {
            int n = 0;
            string[] selectedItems = new string[indexes2BeKeeped.Length];

            foreach (int myIndex in indexes2BeKeeped)
            {
                selectedItems[n] = myListBox.Items[myIndex].ToString();
                n++;
            }
            myListBox.Items.Clear();
            myListBox.Items.AddRange(selectedItems);
        }

        #endregion

        #region Torrent Ranking
        //torrent ranking of an indexer site (torrentz)

        //http://thepiratebay.cd/browse/205/0/7  -- 0 == pag1   ;   1==pag2

        // https://kat.cr/tv/2/   page 2
        //https://kat.cr/tv/1/ page 1
        //<a href="/game-of-thrones-s05e07-hdtv-x264-asap-ettv-t10693526.html" class="cellMainLink">Game of Thrones S05E07 HDTV x264-ASAP[ettv]</a>
        //                                       <span class="font11px lightgrey block">
        //                       Posted by <i class="ka ka-verify" style="font-size: 16px;color:orange;"></i> <a class="plain" href="/user/ettv/">ettv</a> in <span id="cat_10693526"><strong><a href="/tv/">TV</a></strong></span>                	                </span>
        //                   </div>
        //   </td>
        //                           <td class="nobr center">350.62 <span>MB</span></td>
        //   <td class="center">3</td>
        //   <td class="center">4&nbsp;days</td>
        //   <td class="green center">56529</td>
        //   <td class="red lasttd center">5455</td>
        //   </tr>   WEBCLIENT NÃO FUNCIONA over kickass

        //THE PIRATEBAY - PROBLEMs
        //<div class="detName">			<a href="/torrent/11969664/Mr.Robot.S01E01.HDTV.x264.PROPER-LOL[ettv]" class="detLink" title="Details for Mr.Robot.S01E01.HDTV.x264.PROPER-LOL[ettv]">Mr.Robot.S01E01.HDTV.x264.PROPER-LOL[ettv]</a>
        //</div>
        //        </td>
        //        <td align="right">10544</td>  PROBLEMA
        //        <td align="right">4069</td>

        private void button_Ranking_Click(object sender, EventArgs e)
        {

            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            Ranking_Page_Constructor();

            Control myControl = (Control)sender;
            Control parentControl = myControl.Parent;

            //Control objFocus = (Control)myFocus;
            int myRow = Convert.ToInt16(myFocus.Text); //ok

            TextBox showTxB = (TextBox)flowLayoutPanel_Tboxes.Controls["textBox" + Parameters.RowIndex2TboxIndex(myRow).ToString()];

            Initiate_Torrent_Ranking_Analizes(0, myControl.Left, myControl.Bottom, myControl.Width, parentControl, showTxB.Text);
            listView_TorrentRanking.ContextMenuStrip = null;

            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void Ranking_Page_Constructor()
        {
            Ranking_Page_Destructor();
            button_TP7_SaveAs.Click += button_TP7_SaveAs_Click;
            button_TP7_GoBack.Click += button_TP7_GoBack_Click;
            button_TP7_GoFind.Click += button_TP7_GoFind_Click;
            listView_TorrentRanking.ColumnClick += listView_TorrentRanking_ColumnClick;
            textBox_TP7_FindInRanking.KeyDown += textBox_TP7_FindInRanking_KeyDown;
            toolStripMenuItem_Ranking_Summary.Click += toolStripMenuItem_Ranking_Summary_Click;
        }

        private void Ranking_Page_Destructor()
        {
            button_TP7_SaveAs.Click -= button_TP7_SaveAs_Click;
            button_TP7_GoBack.Click -= button_TP7_GoBack_Click;
            button_TP7_GoFind.Click -= button_TP7_GoFind_Click;
            listView_TorrentRanking.ColumnClick -= listView_TorrentRanking_ColumnClick;
            textBox_TP7_FindInRanking.KeyDown -= textBox_TP7_FindInRanking_KeyDown;
            toolStripMenuItem_Ranking_Summary.Click -= toolStripMenuItem_Ranking_Summary_Click;
        }

        private void button_TP7_GoFind_Click(object sender, EventArgs e)
        {
            FindInListViewRanking(textBox_TP7_FindInRanking.Text);
        }

        private void textBox_TP7_FindInRanking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FindInListViewRanking(textBox_TP7_FindInRanking.Text);
            }
            // textBox_TP7_FindInRanking.Focus();//not here. Cant change focus, because listview looses selection.
        }

        private void button_TP7_SaveAs_Click(object sender, EventArgs e)
        {

            //List<string> listItems = listView_TorrentRanking.Items.Cast<ListViewItem>().Select(x => x.Text).ToList();

            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            /// string listViewTable = string.Join(";", from item in listView_TorrentRanking.Items.Cast<ListViewItem.ListViewSubItem>() select item.Text);
            //int offset = Convert.ToInt64(listView1.SelectedItems[0].SubItems[2].ToString());

            stringBuilder.AppendLine("#;Shows;Peers;");
            foreach (ListViewItem item in this.listView_TorrentRanking.Items)
            {
                foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                {
                    stringBuilder.Append(subItem.Text + ";");
                }
                stringBuilder.AppendLine();
            }
            string listViewTable = stringBuilder.ToString();
            listViewTable = Regex.Replace(listViewTable, ";\r", "\r"); //last ; in each line will be removed
            string fileName = Path.Combine(Parameters.myProgPath + @"\Ranking\", "TorrentRanking" + ".Week" + Parameters.weekOfTheYear(DateTime.Today).ToString("00") + ".txt");
            File.Create(fileName).Dispose();
            File.WriteAllText(fileName, listViewTable);

            DialogResult dr = MsgBox.Show("Ranking Table was just saved as " + fileName + ". Do you Want Open it?).", "Saving...", MsgBox.Buttons.YesNo, MsgBox.Icone.Question, MsgBox.AnimateStyle.SlideDown);

            if (dr == DialogResult.Yes)
            {
                Go2Web.OpenLink(fileName);
            }
        }

        private void button_TP7_GoBack_Click(object sender, EventArgs e)
        {
            Go2Page(Parameters.torrentRankingGoBackParentIndex);
            //listView_TorrentRanking.ContextMenuStrip = null;
            textBox_TP7_FindInRanking.Text = "";
            Ranking_Page_Destructor();
        }

        private int sortColumn = -1;

        private void listView_TorrentRanking_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = e.Column;
                // Set the sort order to ascending by default.
                listView_TorrentRanking.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (listView_TorrentRanking.Sorting == SortOrder.Ascending)
                    listView_TorrentRanking.Sorting = SortOrder.Descending;
                else
                    listView_TorrentRanking.Sorting = SortOrder.Ascending;
            }

            // Call the sort method to manually sort.
            listView_TorrentRanking.Sort();
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.listView_TorrentRanking.ListViewItemSorter = new ListViewItemComparer(e.Column, listView_TorrentRanking.Sorting);
        }

        private void toolStripMenuItem_Ranking_Summary_Click(object sender, EventArgs e)
        {

            Ranking_Summary_GoBack();
            // Ranking_Page_Destructor();// transfered to timerdestructor
        }

        private void Ranking_Summary_GoBack()
        {

            ListView.SelectedListViewItemCollection itemsCollection = listView_TorrentRanking.SelectedItems;
            string selectedShowName;
            string partialNameStr;
            selectedShowName = itemsCollection[0].SubItems[1].Text.ToLower();

            bool found = FindInShowsListBox(selectedShowName);

            if (!found)
            {

                string[] partialName = Regex.Split(selectedShowName, " ");

                int nOfParts = partialName.Length;

                for (int i = 1; i <= nOfParts; i++)
                {
                    partialNameStr = partialName[nOfParts - i];
                    if (partialNameStr.Length > 4)
                    {
                        found = FindInShowsListBox(partialNameStr);
                        if (found)
                        {
                            textBox_TP6_FindInShowsNamesList.Text = partialNameStr;
                            break;
                        }
                    }
                }
            }

            if (!found)  //go to for the first letter.
            {
                int index;
                index = listBox_TP6_ShowsNames.FindString(selectedShowName.Substring(0, 1)); //go to first letter of the name. rarely occurs.
                listBox_TP6_ShowsNames.SelectedIndex = index;
            }

            Go2Page(5);
        }

        private void SaveCodhtmlRanking(string codHtmlRanking)
        {
            //will save on HD codhtmlranking to rapid further reference. One file for each week of the year.

            Directory.CreateDirectory(Parameters.myProgPath + "\\Ranking\\");
            string weekSave = Path.Combine(Parameters.myProgPath + "\\Ranking", "Week" + Parameters.weekOfTheYear(DateTime.Today).ToString("00") + ".txt");
            string normalSave = Path.Combine(Parameters.myProgPath + "\\Ranking", "Ranking" + ".txt");
            File.Create(weekSave).Dispose(); //to be arquived
            File.Create(normalSave).Dispose(); //to be used allways
            File.WriteAllText(weekSave, codHtmlRanking);
            File.WriteAllText(normalSave, codHtmlRanking);

        }

        internal ProgressBar progressBar_Ranking;

        private void Initiate_Torrent_Ranking_Analizes(int tabPage2GoBack, int parentControlLeft, int parentControlBottom, int parentControlWidth, Control parentControl, string preSearch = "")
        {
            int nOfWebPagesToCollect = 20; //number of pages to collect from indexer site (below)
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            if (!Parameters.torrentPageAlreadyExtrated) //within same session no need to repopulate listview
            {

                string codHtmlRanking = "";
                string rankingWeekFileName = Path.Combine(Parameters.myProgPath + "\\Ranking", "Week" + Parameters.weekOfTheYear(DateTime.Today).ToString("00") + ".txt");
                string defaultFileName = Path.Combine(Parameters.myProgPath + "\\Ranking", "Ranking" + ".txt");
                if (File.Exists(rankingWeekFileName))  //within same week. is gonna use week file.
                {
                    codHtmlRanking = File.ReadAllText(rankingWeekFileName);
                }

                else if (File.Exists(defaultFileName))
                {
                    DialogResult dr;

                    dr = MsgBox.Show(@"The Ranking Data is Old. You Can Get New Data or Use the Previous List.
Do you Want Get New Data From Web? Is going to take Some Time.", "Ranking Data is Old", MsgBox.Buttons.YesNoCancel, MsgBox.Icone.Question, MsgBox.AnimateStyle.ZoomIn);


                    if (DialogResult.No == dr)
                    {
                        codHtmlRanking = File.ReadAllText(defaultFileName);
                    }
                    else if (DialogResult.Cancel == dr) { return; }
                }
                else
                {
                    DialogResult dr;
                    dr = MsgBox.Show(@"The Ranking Data comes from Internet and Takes Some Time (about 2 minutes). Want to Continue?", "New Ranking Data is Required", MsgBox.Buttons.YesNo, MsgBox.Icone.Question, MsgBox.AnimateStyle.ZoomIn);

                    if (DialogResult.No == dr)
                    {
                        return;
                    }

                }
                this.progressBar_Ranking = new System.Windows.Forms.ProgressBar();

                // 
                // progressBar_Ranking
                // 
                this.progressBar_Ranking.BackColor = Color.Gray;
                this.progressBar_Ranking.ForeColor = Color.Green;

                this.progressBar_Ranking.Location = new Point(parentControlLeft + 5, parentControlBottom - 11);
                this.progressBar_Ranking.Step = 1;
                this.progressBar_Ranking.Name = "progressBar_Ranking";
                this.progressBar_Ranking.Size = new Size(parentControlWidth - 10, 7);
                this.progressBar_Ranking.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
                this.progressBar_Ranking.TabIndex = 22;

                parentControl.Controls.Add(this.progressBar_Ranking);

                this.progressBar_Ranking.Maximum = nOfWebPagesToCollect + 5;
                this.progressBar_Ranking.BringToFront();
                this.progressBar_Ranking.Refresh();
                Thread.Sleep(200);

                if (codHtmlRanking == "") //new data has been required
                {
                    codHtmlRanking += CodHtmlTvTorrentsPageCodeExtractor(0);  //maybe turn this in a backgroundworker 
                    if (codHtmlRanking == "#off-line#")
                    {
                        MsgBox.Show("Sorry, WebSite or Internet is off-line. It's Going to Use old Data if available. Try Again Late", "Internet Off-line", MsgBox.Buttons.OK, MsgBox.Icone.Exclamation, MsgBox.AnimateStyle.SlideDown);
                    }
                    else
                    {
                        for (int i = 1; i <= nOfWebPagesToCollect; i++)
                        {
                            codHtmlRanking += CodHtmlTvTorrentsPageCodeExtractor(i);  //maybe turn this in a backgroundworker
                            progressBar_Ranking.Value = i;
                            progressBar_Ranking.Refresh();
                        }
                        SaveCodhtmlRanking(codHtmlRanking);
                    }
                }

                Torrent_Ranking_Extractor(codHtmlRanking);
            }

            Thread.Sleep(250);

            label_TP7_Ranking.Text = "Torrent Ranking. Top " + listView_TorrentRanking.Items.Count.ToString() + " Shows.";
            Go2Page(6);
            listView_TorrentRanking.Refresh();
            listView_TorrentRanking.Select();
            if (preSearch != "")
            { //try to find listbox show name in rankinglistview.
                this.listView_TorrentRanking.EnsureVisible(0);
                preSearch = Parameters.ClearStr(preSearch).Replace("'", "").Replace(".", " ").Replace("&", "and").Trim();
                textBox_TP7_FindInRanking.Text = preSearch;
                FindInListViewRanking(Parameters.ClearStr(preSearch));
            }
            System.Windows.Forms.Cursor.Current = Cursors.Default;
            Parameters.torrentRankingGoBackParentIndex = tabPage2GoBack;
        }

        private void Torrent_Ranking_Extractor(string codHtmlRanking)
        {//kickass torrent ranking

            ListViewItem item1;

            //string codHtml = "";
            //1. construir string com n paginas (appended) dos resultados da web Torrentz.eu tvshows

            Parameters.torrentPageAlreadyExtrated = true;
            //kat   WEBCLIENT does not work
            //string showsNamePattern = @"(?<=cellMainLink"">).{3,45}(?=S\d{2}E\d{2})"; //sem o prefixo e sem o sufixo
            //string showsSeedsPattern = @"(?<=green center"">)d{1,6}(?=</td>)"; //sem o prefixo e sem o sufixo

            //torrentz ok
            // <dl><dt><a href="/c1f9f6bade11a4c46028b118053452460ab94be1">Game of Thrones S05E03 WEBRip XviD FUM ettv</a> &#187; tv divx xvid</dt><dd><span class="v" style="color:#fff;background-color:#79CC53">6</span><span class="a"><span title="Sun, 12 Apr 2015 03:46:00">1 month</span></span><span class="s">449 MB</span> <span class="u">10,366</span><span class="d">308</span></dd></dl>

            //torrentz
            string showsNamePattern = @"(?<=href=""/\w{40}"">).{3,70}(?=</a>)"; //sem o prefixo e sem o sufixo
            string showsSeedsPattern = @"(?<=class=""u"">).{1,7}(?=</span>)"; //sem o prefixo e sem o sufixo
            string showsLeechesPattern = @"(?<=class=""d"">).{1,7}(?=</span>)";
            progressBar_Ranking.Value = progressBar_Ranking.Maximum - 4;
            progressBar_Ranking.Refresh();
            MatchCollection allShowsNamesInPage = Regex.Matches(codHtmlRanking, showsNamePattern);
            progressBar_Ranking.Value = progressBar_Ranking.Maximum - 3;
            progressBar_Ranking.Refresh();
            MatchCollection allShowsSeedsInPage = Regex.Matches(codHtmlRanking, showsSeedsPattern);
            progressBar_Ranking.Value = progressBar_Ranking.Maximum - 2;
            progressBar_Ranking.Refresh();
            MatchCollection allShowsLeechesInPage = Regex.Matches(codHtmlRanking, showsLeechesPattern);
            progressBar_Ranking.Value = progressBar_Ranking.Maximum - 1;
            progressBar_Ranking.Refresh();
            //sum seeds and leechs to get total of Peers

            int nOfShows = allShowsNamesInPage.Count;
            int[] allShowsPeersInPage = new int[nOfShows];

            for (int i = 0; i < nOfShows; i++)
            { //int32 required
                allShowsPeersInPage[i] = Convert.ToInt32(allShowsSeedsInPage[i].Value.Replace(",", "")) + Convert.ToInt32(allShowsLeechesInPage[i].Value.Replace(",", ""));
            }

            Dictionary<string, int> showsDic = new Dictionary<string, int>();

            string cleanedShowName = "";
            for (int i = 0; i < nOfShows; i++)
            {
                cleanedShowName = Regex.Match(allShowsNamesInPage[i].Value.ToUpper(), @".{3,70}(?=S\d{2}E\d{2})").Value;
                cleanedShowName = Regex.Replace(cleanedShowName, @"<.{1,3}>", "");
                cleanedShowName = cleanedShowName.Trim();
                if (cleanedShowName != "")
                {
                    try  //will catch when key is repeteaded. No repeated entry gonna be added.
                    {
                        showsDic.Add(cleanedShowName, allShowsPeersInPage[i]);
                    }
                    catch
                    {
                    }
                }
            }

            int n = 1;
            //int sleepTime = Convert.ToInt16(nOfShows / 6);
            //progressBar_Ranking.Value += 0;

            foreach (KeyValuePair<string, int> pairShowPeer in showsDic)
            {
                item1 = new ListViewItem(n.ToString("000"), 0);

                item1.SubItems.Add(pairShowPeer.Key);  //showsname
                item1.SubItems.Add(pairShowPeer.Value.ToString("000000")); //Peers.
                this.listView_TorrentRanking.Items.Add(item1);
                n++;
                if (progressBar_Ranking.Value == progressBar_Ranking.Maximum)
                {
                    progressBar_Ranking.Value = 0;
                }
                else
                {
                    progressBar_Ranking.Value += 1;
                    progressBar_Ranking.Refresh();
                    Thread.Sleep(15);
                }
            }

            progressBar_Ranking.Dispose();
        }

        private string CodHtmlTvTorrentsPageCodeExtractor(int page)
        {

            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            try
            { //torrent ranking funtion. gets n pages of torrentz general searching tv-torrents

                // http://thepiratebay.cd/browse/205/0/7  not possible bad seeders regex
                //https://kat.cr/tv/2/?field=seeders&sorder=desc not possibel, webclient not working
                // https://torrentz.eu/verifiedP?f=tv&p=  working
                System.Net.WebClient ccc = new System.Net.WebClient();  //+ "/?field=seeders&sorder=desc"
                //string codhtml = ccc.DownloadString("https://kat.cr/tv/" + page.ToString() );
                //string codhtml = ccc.DownloadString("http://thepiratebay.cd/browse/205/" + page.ToString() + "/7");

                ccc.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.15) Gecko/20110303 Firefox/3.6.15";
                // https://torrentz.eu/verifiedP?f=tv&p=1
                //string codhtml = ccc.DownloadString("https://kat.cr/tv/" + page.ToString() + "/?field=seeders&sorder=desc");
                //string codhtml = ccc.DownloadString("http://thepiratebay.cd/browse/205/" + page.ToString() + "/7");

                string codhtml = ccc.DownloadString("https://torrentz.eu/verifiedP?f=tv&p=" + page.ToString());
                Thread.Sleep(2500);

                ccc.Dispose();
                return codhtml;
            }
            catch
            {
            }
            return "#off-line#";
        }

        class ListViewItemComparer : System.Collections.IComparer
        { //to orger by columns in listview
            private int col;
            private SortOrder order;
            public ListViewItemComparer()
            {
                col = 0;
                order = SortOrder.Ascending;
            }
            public ListViewItemComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }
            public int Compare(object x, object y)
            {
                int returnVal = -1;
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
                // Determine whether the sort order is descending.
                if (order == SortOrder.Descending)
                    // Invert the value returned by String.Compare.
                    returnVal *= -1;
                return returnVal;
            }
        }

        #endregion

        //########################################################################################################################
    }
}