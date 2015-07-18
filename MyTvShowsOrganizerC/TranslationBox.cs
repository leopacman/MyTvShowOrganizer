using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MyTvShowsOrganizer
{
    public partial class TranslationBox : Form
    {
        private const int CS_DROPSHADOW = 0x00020000;
        private static TranslationBox _TranslateBox;

        private string _translatorUrl;

        private Panel _plHeader = new Panel();
        private Panel _plFooter = new Panel();
        //private Panel _plIcon = new Panel();
        // private Panel _plTranslator = new Panel();
        //private PictureBox _picIcon = new PictureBox();
        private System.Windows.Forms.WebBrowser _webBrowserTranslator = new WebBrowser();

        private FlowLayoutPanel _flpButtons = new FlowLayoutPanel();
        private Label _lblTitle;
        //private Label _lblMessage;
        //private LinkLabel _lnkLblTranslate;
        private List<Button> _buttonCollection = new List<Button>();
        //private static DialogResult _buttonResult = new DialogResult();
        // private static Timer _timer;
        private static Timer _timer_CursorOverButton;
        private static Point lastMousePos;
        private static Timer _timer_Translate;

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //private static extern bool MessageBeep(uint type);
        private static Color TranslateBoxBorderColor = Color.DarkGray;
        private static Color tempBorderColor = TranslateBoxBorderColor;

        private TranslationBox()
        {

            this.Icon = global::MyTvShowsOrganizer.Properties.Resources.binggreensmall2_64_xgy_icon;
            this.Text = "[En ─› Ҩἒὧℓ₯]";
            Graphics g = this.CreateGraphics();
            Font titleFont = new System.Drawing.Font("Segoe UI", 13, FontStyle.Bold);

            // string hyper = "https://www.bing.com/translator/";// "https://www.bing.com/translator/";https://www.google.com.br/
            //Font translateLnkFont = new System.Drawing.Font("Arial", 9, FontStyle.Bold);
            this.Size = new Size(1050, 600);

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            // this.BackColor = Color.FromArgb(10, 10, 10);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Padding = new System.Windows.Forms.Padding(1);
            this.TopMost = true;

            _plHeader.BackColor = Color.FromArgb(30, 30, 30);
            _plHeader.Dock = DockStyle.Top;
            _plHeader.Height = ((int)(g.MeasureString("Sample", titleFont).Height + 3) * 2);
            _plHeader.Padding = new Padding(((int)(g.MeasureString("Sample", titleFont).Height / 2 + 1)));


            _lblTitle = new Label();
            _lblTitle.ForeColor = Color.White;
            _lblTitle.Font = titleFont;
            //this.Margin = new System.Windows.Forms.Padding(2);
            _lblTitle.Dock = DockStyle.Left;
            //SizeF sizeTitle = g.MeasureString("Sample", titleFont);
            //_lblTitle.Height = ((int)(g.MeasureString("Sample", titleFont).Height + 1));
            _lblTitle.AutoSize = true;
            _plHeader.Controls.Add(_lblTitle);


            //SizeF sizeTitle = g.MeasureString("Sample", titleFont);
            // _lblTitle.Height = 20;// ((int)(g.MeasureString("Sample", titleFont).Height + 1) * 2);

            //_lblMessage = new Label();
            //_lblMessage.ForeColor = Color.White;
            //_lblMessage.Font = messageFont;
            //_lblMessage.Dock = DockStyle.Fill;

            _flpButtons.FlowDirection = FlowDirection.RightToLeft;
            _flpButtons.Dock = DockStyle.Fill;
            
            //_plTranslator.BackColor = Color.FromArgb(30, 30, 30);
            //_plTranslator.Dock = DockStyle.Top;
            //_plTranslator.Height = 500;

            _plFooter.Dock = DockStyle.Bottom;
            _plFooter.Padding = new Padding(10);
            _plFooter.BackColor = Color.FromArgb(30, 40, 30);
            _plFooter.Height = 50;
            _plFooter.Controls.Add(_flpButtons);

            //webBrowserTranslator.AllowWebBrowserDrop = false;
            _webBrowserTranslator.IsWebBrowserContextMenuEnabled = false;
            _webBrowserTranslator.WebBrowserShortcutsEnabled = true;
            _webBrowserTranslator.Name = "webBrowserTranslator";
            _webBrowserTranslator.ScriptErrorsSuppressed = true;

            Controls.Add(_webBrowserTranslator);
            _webBrowserTranslator.Dock = DockStyle.Fill;

            //_webBrowserTranslator.Url = new Uri(_TranslateBox._translatorUrl);

            // this.webBrowser2.DocumentText = Parameters.newShowImageWebPage;
            // webBrowser2.Refresh();
            //_lnkLblTranslate = new LinkLabel();
            ////_lnkLblTranslate.ForeColor = Color.Blue;
            //_lnkLblTranslate.Font = translateLnkFont;
            //_lnkLblTranslate.Dock = DockStyle.Top;
            //_lnkLblTranslate.Text = "[En ─› Ҩἒὧℓ₯]";
            //// _lnkLblTranslate.Image =  global::MyTvShowsOrganizer.Properties.Resources.binggreensmall;
            ////_lnkLblTranslate.ImageAlign = ContentAlignment.MiddleLeft ;
            //_lnkLblTranslate.Click += _lnkLblTranslate_Click;

            //_plTranslate.Dock = DockStyle.Bottom;
            //_plTranslate.Padding = new Padding(20, 0, 0, 0);
            //_plTranslate.Height = ((int)(g.MeasureString(_lnkLblTranslate.Text, translateLnkFont).Height + 2));
            //_plTranslate.Controls.Add(_lnkLblTranslate);

            //_picIcon.Width = 32;
            //_picIcon.Height = 32;
            //_picIcon.Location = new Point(30, 50);
            //_plIcon.Dock = DockStyle.Left;
            //// _plIcon.Padding = new Padding(20);
            //_plIcon.Width = 65;
            //_plIcon.Controls.Add(_picIcon);

            List<Control> controlCollection = new List<Control>();
            //controlCollection.Add(this);
            controlCollection.Add(_plHeader);
            controlCollection.Add(_lblTitle);
            controlCollection.Add(_flpButtons);
            //controlCollection.Add(_plHeader);
            //controlCollection.Add(_plTranslate);
            //controlCollection.Add(_plFooter);
            //controlCollection.Add(_plIcon);
            //controlCollection.Add(_picIcon);
            //controlCollection.Add(webBrowserTranslator);


            foreach (Control control in controlCollection)
            {
                control.MouseDown += TranslateBox_MouseDown;
                control.MouseMove += TranslateBox_MouseMove;
            }

            this.Controls.Add(_plHeader);
            _plHeader.BringToFront();
            _lblTitle.BringToFront();
            //this.Controls.Add(_plIcon);
            //this.Controls.Add(_plTranslate);
            this.Controls.Add(_plFooter);
            _plFooter.BringToFront();


        }

        private void Send2Translator(string message)
        {
            _TranslateBox._plFooter.Cursor = Cursors.WaitCursor;
            _TranslateBox._plHeader.Cursor = Cursors.WaitCursor;
            Clipboard.SetText(message, TextDataFormat.Text);
            _webBrowserTranslator.DocumentCompleted += webBrowserTranslator_DocumentCompleted;
        }



        void webBrowserTranslator_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //int n = 0;


            //while (_webBrowserTranslator.ReadyState == WebBrowserReadyState.Complete || n==10)
            //{
            //    System.Threading.Thread.Sleep(500);
            //    n++;
            //}

            _timer_Translate = new Timer();
            _webBrowserTranslator.Focus();
            _timer_Translate.Interval = 3000;

            _timer_Translate.Tick += timer_Translate_Tick;

            _webBrowserTranslator.DocumentCompleted -= webBrowserTranslator_DocumentCompleted;

            //   System.Threading.Thread.Sleep(1000);

            _timer_Translate.Enabled = true;

        }


        private void timer_Translate_Tick(object sender, EventArgs e)  //needed for bing 
        {

            //  System.Threading.Thread.Sleep(10000);
            System.Windows.Forms.SendKeys.Send("^v");

            _TranslateBox._plFooter.Cursor = Cursors.Default;
            _TranslateBox._plHeader.Cursor = Cursors.Default;

            _timer_Translate.Enabled = false;

        }


        private static void TranslateBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastMousePos = new Point(e.X, e.Y);
            }
        }


        private static void TranslateBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _TranslateBox.Left += e.X - lastMousePos.X;
                _TranslateBox.Top += e.Y - lastMousePos.Y;
            }
        }

        private static void TimerCursorOverButtonContructor()
        {
            _timer_CursorOverButton = new Timer();
            _timer_CursorOverButton.Interval = 50;
            _timer_CursorOverButton.Tick += _timer_CursorOverButton_Tick;

        }
        
        //public static DialogResult Show(string message)
        //{
        //    TimerCursorOverButtonContructor();

        //    _TranslateBox = new TranslateBox();
        //    //_TranslateBox._lblMessage.Text = message;
        //    //_TranslateBox._plIcon.Hide();
                //    TranslateBox.InitButtons(Buttons.OK);
                //    _timer_CursorOverButton.Start();
        

        //    _TranslateBox.Show();
        //    //MessageBeep(0);

        //    _timer_CursorOverButton.Dispose();
        
        //    _TranslateBox.Send2Translator(message);// Translate();
        
        //    return _buttonResult;
        //}
        //public static DialogResult Show(string message, string title, Buttons buttons)
        //{
        //    TimerCursorOverButtonContructor();

        //    _TranslateBox = new TranslateBox();
        //    //_TranslateBox._lblMessage.Text = message;

        //    //_TranslateBox._lblTitle.Text = title;
        //    //_TranslateBox._plIcon.Hide();

        //    TranslateBox.InitButtons(buttons);

        //    _TranslateBox.Size = TranslateBox.MessageSize(message, title);

        //    _timer_CursorOverButton.Start();
        //    _TranslateBox.ShowDialog();
        //    //MessageBeep(0);
        //    _timer_CursorOverButton.Dispose();
        //    return _buttonResult;
        //}

        //public static DialogResult Show(string message, string title, Buttons buttons, Icone icon)
        //{
        //    TimerCursorOverButtonContructor();

        //    _TranslateBox = new TranslateBox();
        //    //_TranslateBox._lblMessage.Text = message;
        //    //_TranslateBox._lblTitle.Text = title;

        //    TranslateBox.InitButtons(buttons);
        //    TranslateBox.InitIcon(icon);

        //    _TranslateBox.Size = TranslateBox.MessageSize(message, title, hasIcon: true);

        //    _timer_CursorOverButton.Start();
        //    _TranslateBox.ShowDialog();

        //    //MessageBeep(0);
        //    _timer_CursorOverButton.Dispose();
        //    return _buttonResult;
        //}
        public static void Show(string message, string title, WebTranslator webTranslator)
        {
            TimerCursorOverButtonContructor();

            _TranslateBox = new TranslationBox();
            //_TranslateBox._lblMessage.Text = message;

            _TranslateBox._lblTitle.Text = title;
            // _TranslateBox.Size = TranslateBox.MessageSize(message, title);
            //_TranslateBox._plIcon.Hide();
            
            TranslationBox.InitButtons(Buttons.OK);
            TranslationBox.InitWebTranslatorUrl(webTranslator);

            _timer_CursorOverButton.Start();

            _TranslateBox._webBrowserTranslator.Url = new Uri(_TranslateBox._translatorUrl);

            _TranslateBox.Send2Translator(message);// Translate();
            _TranslateBox.StartPosition = FormStartPosition.CenterParent;
            _TranslateBox.Opacity = 0.1;
            _TranslateBox.ShowDialog();
            //MessageBeep(0);

            // _timer_CursorOverButton.Dispose();  //dispose in another place

            // return _buttonResult;
        }


        //public static void  Show(string message, string title, WebTranslator webTranslator, AnimateStyle style)//  Buttons buttons, Icone icon,
        //{
        //    TimerCursorOverButtonContructor();
        //    TranslateBox.InitButtons(Buttons.OK);


        //    _TranslateBox = new TranslateBox();
        //    //_TranslateBox._lblMessage.Text = message;
        //    _TranslateBox._lblTitle.Text = title;
        //    _TranslateBox.Height = 300;

        //    //TranslateBox.InitButtons(buttons);
        //    //TranslateBox.InitIcon(icon);



        //    _timer = new Timer();
        //    Size formSize = new Size(_TranslateBox.Width, _TranslateBox.Height);

        //    switch (style)
        //    {
        //        case TranslateBox.AnimateStyle.SlideDown:
        //            _TranslateBox.Size = new Size(formSize.Width, 0);
        //            _timer.Interval = 20;
        //            _timer.Tag = new AnimateTranslateBox(formSize, style);
        //            break;

        //        case TranslateBox.AnimateStyle.FadeIn:
        //            //tempBorderColor = _TranslateBox.BackColor; //to animated effect do work more smoth (without blinking)
        //            _TranslateBox.Size = formSize;
        //            _TranslateBox.Opacity = 0;
        //            _timer.Interval = 50;
        //            _timer.Tag = new AnimateTranslateBox(formSize, style);
        //            break;

        //        case TranslateBox.AnimateStyle.ZoomIn:
        //            _TranslateBox.Size = new Size(formSize.Width + 100, formSize.Height + 100);
        //            _timer.Tag = new AnimateTranslateBox(formSize, style);
        //            _timer.Interval = 20;
        //            break;
        //    }

        //    _timer.Tick += timer_Tick;
        //    _timer.Start();

        //    _timer.Disposed += timer_Disposed; //to initiate contdown to cursoroverbutton timer

        //    _TranslateBox.Show();

        //    //MessageBeep(0);
        //    _timer_CursorOverButton.Dispose();

        //   // return _buttonResult;
        //}

        //static void timer_Disposed(object sender, EventArgs e)
        //{
        //    //when timer disposed msbBox will be fully formed and positioned
        //    _timer_CursorOverButton.Start();

        //}

        static void _timer_CursorOverButton_Tick(object sender, EventArgs e)
        {

            Parameters.FadeIn(_TranslateBox);
            Button myButton = (Button)_TranslateBox._flpButtons.Controls[0];
            Point buttonLocation = _TranslateBox._flpButtons.PointToScreen(myButton.Location);
            Point buttonCentre = new Point(buttonLocation.X + myButton.Width / 2, buttonLocation.Y + myButton.Height / 2);
            Point p = myButton.PointToScreen(buttonCentre);
            Cursor.Position = buttonCentre;
            _timer_CursorOverButton.Stop();

        }

        //static void timer_Tick(object sender, EventArgs e)
        //{
        //    Timer timer = (Timer)sender;
        //    AnimateTranslateBox animate = (AnimateTranslateBox)timer.Tag;

        //    switch (animate.Style)
        //    {
        //        case TranslateBox.AnimateStyle.SlideDown:
        //            if (_TranslateBox.Height < animate.FormSize.Height)
        //            {
        //                _TranslateBox.Height += 17;
        //                _TranslateBox.Invalidate();
        //            }
        //            else
        //            {
        //                _timer.Stop();
        //                _timer.Dispose();

        //            }
        //            break;

        //        case TranslateBox.AnimateStyle.FadeIn:
        //            if (_TranslateBox.Opacity < 1)
        //            {
        //                _TranslateBox.Opacity += 0.1;
        //                _TranslateBox.Invalidate();
        //            }
        //            else
        //            {
        //                _timer.Stop();
        //                tempBorderColor = TranslateBoxBorderColor;
        //                Graphics g = _TranslateBox.CreateGraphics();
        //                _TranslateBox.DrawRetangle(g);
        //                _timer.Dispose();

        //            }
        //            break;

        //        case TranslateBox.AnimateStyle.ZoomIn:
        //            if (_TranslateBox.Width > animate.FormSize.Width)
        //            {
        //                _TranslateBox.Width -= 17;
        //                _TranslateBox.Invalidate();
        //            }
        //            if (_TranslateBox.Height > animate.FormSize.Height)
        //            {
        //                _TranslateBox.Height -= 17;
        //                _TranslateBox.Invalidate();
        //            }
        //            if (_TranslateBox.Width <= animate.FormSize.Width && _TranslateBox.Height <= animate.FormSize.Height)
        //            {
        //                _timer.Stop();
        //                _timer.Dispose();
        //            }
        //            break;
        //    }
        //}

        private static void InitButtons(Buttons buttons)
        {
            switch (buttons)
            {
                case TranslationBox.Buttons.OK:
                    _TranslateBox.InitOKButton();
                    break;

                //case TranslateBox.Buttons.AbortRetryIgnore:
                //    _TranslateBox.InitAbortRetryIgnoreButtons();
                //    break;



                //case TranslateBox.Buttons.OKCancel:
                //    _TranslateBox.InitOKCancelButtons();
                //    break;

                //case TranslateBox.Buttons.RetryCancel:
                //    _TranslateBox.InitRetryCancelButtons();
                //    break;

                //case TranslateBox.Buttons.YesNo:
                //    _TranslateBox.InitYesNoButtons();
                //    break;

                //case TranslateBox.Buttons.YesNoCancel:
                //    _TranslateBox.InitYesNoCancelButtons();
                //    break;
            }


            foreach (Button btn in _TranslateBox._buttonCollection)
            {
                btn.ForeColor = Color.White; // FromArgb(170, 170, 170);
                btn.Font = new System.Drawing.Font("Segoe UI", 9, FontStyle.Bold);
                btn.Padding = new Padding(1);
                btn.FlatStyle = FlatStyle.Standard;
                btn.Height = 25;
                btn.BackColor = Color.FromArgb(30, 30, 30);
                // btn.FlatAppearance.BorderColor = Color.FromArgb(99, 99, 98);

                _TranslateBox._flpButtons.Controls.Add(btn);
            }
        }

        private static void InitWebTranslatorUrl(WebTranslator webTranslator)
        {
            switch (webTranslator)
            {
                case TranslationBox.WebTranslator.Bing:
                    _TranslateBox._translatorUrl = "https://www.bing.com/translator/";
                    break;
                case TranslationBox.WebTranslator.Google:
                    _TranslateBox._translatorUrl = "https://translate.google.com/";
                    break;

            }
        }

        //private static void InitIcon(Icone icon)
        //{
        //    switch (icon)
        //    {
        //        case TranslateBox.Icone.Application:
        //            _TranslateBox._picIcon.Image = SystemIcons.Application.ToBitmap();
        //            break;

        //        case TranslateBox.Icone.Exclamation:
        //            _TranslateBox._picIcon.Image = SystemIcons.Exclamation.ToBitmap();
        //            break;

        //        case TranslateBox.Icone.Error:
        //            _TranslateBox._picIcon.Image = SystemIcons.Error.ToBitmap();
        //            break;

        //        case TranslateBox.Icone.Info:
        //            _TranslateBox._picIcon.Image = SystemIcons.Information.ToBitmap();
        //            break;

        //        case TranslateBox.Icone.Question:
        //            _TranslateBox._picIcon.Image = SystemIcons.Question.ToBitmap();
        //            break;

        //        case TranslateBox.Icone.Shield:
        //            _TranslateBox._picIcon.Image = SystemIcons.Shield.ToBitmap();
        //            break;

        //        case TranslateBox.Icone.Warning:
        //            _TranslateBox._picIcon.Image = SystemIcons.Warning.ToBitmap();
        //            break;
        //    }
        //}

        //private void InitAbortRetryIgnoreButtons()
        //{
        //    Button btnAbort = new Button();
        //    btnAbort.Text = "Abort";
        //    btnAbort.Click += ButtonClick;

        //    Button btnRetry = new Button();
        //    btnRetry.Text = "Retry";
        //    btnRetry.Click += ButtonClick;

        //    Button btnIgnore = new Button();
        //    btnIgnore.Text = "Ignore";
        //    btnIgnore.Click += ButtonClick;

        //    this._buttonCollection.Add(btnAbort);
        //    this._buttonCollection.Add(btnRetry);
        //    this._buttonCollection.Add(btnIgnore);
        //}

        private void InitOKButton()
        {
            Button btnOK = new Button();
            btnOK.Text = "Ok";
            btnOK.Click += ButtonClick;

            this._buttonCollection.Add(btnOK);
        }

        //private void InitOKCancelButtons()
        //{
        //    Button btnOK = new Button();
        //    btnOK.Text = "Ok";
        //    btnOK.Click += ButtonClick;

        //    Button btnCancel = new Button();
        //    btnCancel.Text = "Cancel";
        //    btnCancel.Click += ButtonClick;


        //    this._buttonCollection.Add(btnOK);
        //    this._buttonCollection.Add(btnCancel);
        //}

        //private void InitRetryCancelButtons()
        //{
        //    Button btnRetry = new Button();
        //    btnRetry.Text = "Retry";
        //    btnRetry.Click += ButtonClick;

        //    Button btnCancel = new Button();
        //    btnCancel.Text = "Cancel";
        //    btnCancel.Click += ButtonClick;


        //    this._buttonCollection.Add(btnRetry);
        //    this._buttonCollection.Add(btnCancel);
        //}

        //private void InitYesNoButtons()
        //{
        //    Button btnYes = new Button();
        //    btnYes.Text = "Yes";
        //    btnYes.Click += ButtonClick;

        //    Button btnNo = new Button();
        //    btnNo.Text = "No";
        //    btnNo.Click += ButtonClick;


        //    this._buttonCollection.Add(btnYes);
        //    this._buttonCollection.Add(btnNo);
        //}

        //private void InitYesNoCancelButtons()
        //{
        //    Button btnYes = new Button();
        //    btnYes.Text = "Yes";
        //    btnYes.Click += ButtonClick;

        //    Button btnNo = new Button();
        //    btnNo.Text = "No";
        //    btnNo.Click += ButtonClick;

        //    Button btnCancel = new Button();
        //    btnCancel.Text = "Cancel";
        //    btnCancel.Click += ButtonClick;

        //    this._buttonCollection.Add(btnYes);
        //    this._buttonCollection.Add(btnNo);
        //    this._buttonCollection.Add(btnCancel);

        //}

        private static void ButtonClick(object sender, EventArgs e)
        {
            //Button btn = (Button)sender;

            //switch (btn.Text)
            //{
            //    //case "Abort":
            //    //    _buttonResult = DialogResult.Abort;
            //    //    break;

            //    //case "Retry":
            //    //    _buttonResult = DialogResult.Retry;
            //    //    break;

            //    //case "Ignore":
            //    //    _buttonResult = DialogResult.Ignore;
            //    //    break;

            //    case "Ok":
            //        _buttonResult = DialogResult.OK;
            //        break;

            //    //case "Cancel":
            //    //    _buttonResult = DialogResult.Cancel;
            //    //    break;

            //    //case "Yes":
            //    //    _buttonResult = DialogResult.Yes;
            //    //    break;

            //    //case "No":
            //    //    _buttonResult = DialogResult.No;
            //    //    break;
            //}

            Parameters.FadeOut(_TranslateBox);
            _TranslateBox.Close();

        }

        //private static Size MessageSize(string message, string title, bool hasIcon = false)
        //{
        //    // TranslateBox TranslateBox = new TranslateBox();
        //    Graphics gg = (Graphics)_TranslateBox.Tag;
        //    // Graphics gg = _TranslateBox.CreateGraphics();
        //    SizeF size;
        //    int widthLine;
        //    int heightLines;

        //    string[] groupsUserDefined = Regex.Split(message, "\r\n"); //to detect if programer used line feed

        //    int nLines = groupsUserDefined.Length;

        //    int iconIncrement = 0;
        //    if (hasIcon)
        //    {
        //        iconIncrement = _TranslateBox._plIcon.Width;
        //        _TranslateBox._lnkLblTranslate.Padding = new Padding(_TranslateBox._plIcon.Width, 0, 0, 0); //to show translate message with same padding.
        //    }

        //    string masLenStr;
        //    if (nLines > 1)  //programer are using carriers returns and feed lines in big messages. This will show dialog as programed.
        //    {
        //        masLenStr = MaxLenghtString(groupsUserDefined); //the biggest string among all, defined by programer.
        //    }
        //    else  //one line only. This code will split it.
        //    {

        //        int mesLen = message.Length;
        //        int stringPiece = 60;
        //        if (mesLen > 1500)
        //        {
        //            stringPiece = mesLen / 25;  //max 25 lines (to fit 768pixels monitors). The growth will be in width.
        //        }

        //        string[] groupsAutoDefined = (from Match m in Regex.Matches(message, @".{1," + stringPiece + @"}(\s|$)") select m.Value).ToArray(); //space or endofstring 
        //        masLenStr = MaxLenghtString(groupsAutoDefined);
        //        nLines = groupsAutoDefined.Length; //regex will escape the last line so +1.
        //    }

        //    size = gg.MeasureString(masLenStr, _TranslateBox._lblMessage.Font);

        //    widthLine = (int)size.Width + 1;
        //    heightLines = (int)((decimal)size.Height * nLines) + 1; //round to top


        //    //if title text is wider than... it is determinant
        //    size = gg.MeasureString(title, _TranslateBox._lblTitle.Font);
        //    if ((int)size.Width > widthLine) //whenever title is wider than message
        //    {
        //        widthLine = (int)size.Width;
        //    }

        //    //if translate text is wider than... it is determinant
        //    size = gg.MeasureString(_TranslateBox._lnkLblTranslate.Text, _TranslateBox._lnkLblTranslate.Font);
        //    if ((int)size.Width > widthLine) //whenever title is wider than message
        //    {
        //        widthLine = (int)size.Width;
        //    }

        //    int incrementFactorH = 10; //auto padding etc...

        //    int padding = 2 * _TranslateBox._plHeader.Padding.Left; //2 for width/height
        //    int titleH = _TranslateBox._lblTitle.Height;
        //    int footer = _TranslateBox._plFooter.Height;
        //    int translate = _TranslateBox._plTranslate.Height;

        //    int widthTot = widthLine + incrementFactorH + padding + iconIncrement;
        //    int heightTot = heightLines + titleH + footer + padding + translate;

        //    return new Size(widthTot, heightTot);
        //}


        //private static string MaxLenghtString(string[] group)
        //{
        //    Graphics g = _TranslateBox.CreateGraphics();
        //    SizeF size;

        //    float len = 0;
        //    string maxStr = "";
        //    foreach (string item in group)
        //    {
        //        size = g.MeasureString(item, _TranslateBox._lblMessage.Font);
        //        if (size.Width > len)
        //        {
        //            maxStr = item;
        //            len = size.Width;
        //        }
        //    }
        //    return maxStr;
        //}


        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            DrawRetangle(g);

        }

        public void DrawRetangle(Graphics g)
        {

            Rectangle rect = new Rectangle(new Point(0, 0), new Size(this.Width - 1, this.Height - 1));
            Pen pen = new Pen(tempBorderColor);
            g.DrawRectangle(pen, rect);

        }

        public enum Buttons
        {
            //AbortRetryIgnore = 1,
            OK = 2,
            //OKCancel = 3,
            //RetryCancel = 4,
            //YesNo = 5,
            //YesNoCancel = 6
        }

        //public enum Icone
        //{
        //    Application = 1,
        //    Exclamation = 2,
        //    Error = 3,
        //    Warning = 4,
        //    Info = 5,
        //    Question = 6,
        //    Shield = 7,
        //    Search = 8
        //}

        //public enum AnimateStyle
        //{
        //    SlideDown = 1,
        //    FadeIn = 2,
        //    ZoomIn = 3
        //}
        public enum WebTranslator
        {
            Bing = 1,
            Google = 2,
        }


    }
    //class AnimateTranslateBox
    //{
    //    public Size FormSize;
    //    public TranslateBox.AnimateStyle Style;

    //    public AnimateTranslateBox(Size formSize, TranslateBox.AnimateStyle style)
    //    {
    //        this.FormSize = formSize;
    //        this.Style = style;
    //    }
    //}
}
