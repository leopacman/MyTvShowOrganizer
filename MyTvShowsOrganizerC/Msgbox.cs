using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;


namespace MyTvShowsOrganizer
{

    public partial class MsgBox : Form
    {
        private const int CS_DROPSHADOW = 0x00020000;
        private static MsgBox _msgBox;
        private Panel _plHeader = new Panel();
        private Panel _plFooter = new Panel();
        private Panel _plIcon = new Panel();
        private Panel _plTranslate = new Panel();
        private PictureBox _picIcon = new PictureBox();
        //

        private FlowLayoutPanel _flpButtons = new FlowLayoutPanel();
        private Label _lblTitle;
        private Label _lblMessage;
        private LinkLabel _lnkLblTranslate;
        private List<Button> _buttonCollection = new List<Button>();
        private static DialogResult _buttonResult = new DialogResult();
        private static Timer _timer;
        private static Timer _timer_CursorOverButton;
        private static Point lastMousePos;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool MessageBeep(uint type);
        private static Color MsgBoxBorderColor = Color.DarkGray;
        private static Color tempBorderColor = MsgBoxBorderColor;


        private MsgBox()
        {
            this.Icon = global::MyTvShowsOrganizer.Properties.Resources.logoIcon;
            this.Text = "MsgBox";
            Graphics g = this.CreateGraphics();
            this.Tag = g;
            Font messageFont = new System.Drawing.Font("Segoe UI", 11);
            Font titleFont = new System.Drawing.Font("Segoe UI", 16);
            Font translateLnkFont = new System.Drawing.Font("Arial", 11, FontStyle.Bold);

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.BackColor = Color.FromArgb(10, 10, 10);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Padding = new System.Windows.Forms.Padding(1);
            //this.Width = 400;
           
            
            _lblTitle = new Label();
            _lblTitle.ForeColor = Color.White;
            _lblTitle.Font = titleFont;
            _lblTitle.Dock = DockStyle.Top;
            //SizeF sizeTitle = g.MeasureString("Sample", titleFont);
            _lblTitle.Height = ((int)(g.MeasureString("Sample", titleFont).Height + 1) * 2);

            _lblMessage = new Label();
            _lblMessage.ForeColor = Color.White;
            _lblMessage.Font = messageFont;
            _lblMessage.Dock = DockStyle.Fill;

            _flpButtons.FlowDirection = FlowDirection.RightToLeft;
            _flpButtons.Dock = DockStyle.Fill;

            _plHeader.Dock = DockStyle.Fill;
            _plHeader.Padding = new Padding(20);
            _plHeader.Controls.Add(_lblMessage);
            _plHeader.Controls.Add(_lblTitle);

            _plFooter.Dock = DockStyle.Bottom;
            _plFooter.Padding = new Padding(15);
            _plFooter.BackColor = Color.FromArgb(30, 40, 30);
            _plFooter.Height = 70;
            _plFooter.Controls.Add(_flpButtons);

            _lnkLblTranslate = new LinkLabel();
            //_lnkLblTranslate.ForeColor = Color.Blue;
            _lnkLblTranslate.Font = translateLnkFont;
            _lnkLblTranslate.Dock = DockStyle.Top;
            _lnkLblTranslate.Text = "[En ─› Ҩἒὧℓ₯]";
            // _lnkLblTranslate.Image =  global::MyTvShowsOrganizer.Properties.Resources.binggreensmall;
            //_lnkLblTranslate.ImageAlign = ContentAlignment.MiddleLeft ;
            // _lnkLblTranslate.Click -= _lnkLblTranslate_Click;
            _lnkLblTranslate.Click += _lnkLblTranslate_Click;

            _plTranslate.Dock = DockStyle.Bottom;
            _plTranslate.Padding = new Padding(20, 0, 0, 0);
            _plTranslate.Height = ((int)(g.MeasureString(_lnkLblTranslate.Text, translateLnkFont).Height + 2));
            _plTranslate.Controls.Add(_lnkLblTranslate);


            _picIcon.Width = 32;
            _picIcon.Height = 32;
            _picIcon.Location = new Point(30, 50);
            _plIcon.Dock = DockStyle.Left;
            // _plIcon.Padding = new Padding(20);
            _plIcon.Width = 65;
            _plIcon.Controls.Add(_picIcon);

            List<Control> controlCollection = new List<Control>();
            //controlCollection.Add(this);
            controlCollection.Add(_lblTitle);
            controlCollection.Add(_lblMessage);
            controlCollection.Add(_flpButtons);
            controlCollection.Add(_plHeader);
            controlCollection.Add(_plTranslate);
            // controlCollection.Add(_plFooter);
            controlCollection.Add(_plIcon);
            //controlCollection.Add(_picIcon);

            foreach (Control control in controlCollection)
            {
                control.MouseDown += MsgBox_MouseDown;
                control.MouseMove += MsgBox_MouseMove;
            }

            this.Controls.Add(_plHeader);
            this.Controls.Add(_plIcon);
            this.Controls.Add(_plTranslate);
            this.Controls.Add(_plFooter);

        }


        private static void _lnkLblTranslate_Click(object sender, EventArgs e)
        {

            if (Parameters.bingTranslatorOn)  //external class, remove and use one of the translator.
            {
                
                TranslationBox.Show(_msgBox._lblTitle.Text + ": " + _msgBox._lblMessage.Text, _msgBox._lblTitle.Text, TranslationBox.WebTranslator.Bing);
            }
            else
            {
                TranslationBox.Show(_msgBox._lblTitle.Text + ": " + _msgBox._lblMessage.Text, _msgBox._lblTitle.Text, TranslationBox.WebTranslator.Google);
            }

        }


        private static void MsgBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastMousePos = new Point(e.X, e.Y);
            }
        }


        private static void MsgBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _msgBox.Left += e.X - lastMousePos.X;
                _msgBox.Top += e.Y - lastMousePos.Y;
            }
        }

        private static void TimerCursorOverButtonContructor()
        {
            _timer_CursorOverButton = new Timer();
            _timer_CursorOverButton.Interval = 50;
            _timer_CursorOverButton.Tick += timer_CursorOverButton_Tick;
            
        }


        public static DialogResult Show(string message)
        {
            TimerCursorOverButtonContructor();

            _msgBox = new MsgBox();
            _msgBox._lblMessage.Text = message;
            _msgBox._plIcon.Hide();

            MsgBox.InitButtons(Buttons.OK);
            
            _msgBox.Size = MsgBox.MessageSize(message);
            
            _timer_CursorOverButton.Start();
            _msgBox.ShowDialog();
            _msgBox.Focus();
            MessageBeep(0);

            _timer_CursorOverButton.Dispose();
            return _buttonResult;
        }

        public static DialogResult Show(string message, string title)
        {
            TimerCursorOverButtonContructor();

            _msgBox = new MsgBox();
            _msgBox._lblMessage.Text = message;

            _msgBox._lblTitle.Text = title;

            _msgBox.CenterToScreen();
            _msgBox._plIcon.Hide();

            MsgBox.InitButtons(Buttons.OK);

            _msgBox.Size = MsgBox.MessageSize(message, title);

            _timer_CursorOverButton.Start();
            _msgBox.ShowDialog();
            _msgBox.Focus();
            MessageBeep(0);

            _timer_CursorOverButton.Dispose();
            return _buttonResult;
        }

        public static DialogResult Show(string message, string title, Buttons buttons)
        {
            TimerCursorOverButtonContructor();

            _msgBox = new MsgBox();
            _msgBox._lblMessage.Text = message;

            _msgBox._lblTitle.Text = title;
            _msgBox._plIcon.Hide();

            MsgBox.InitButtons(buttons);

            _msgBox.Size = MsgBox.MessageSize(message, title);
            
            _timer_CursorOverButton.Start();
            _msgBox.ShowDialog();
            _msgBox.Focus();
            MessageBeep(0);
            _timer_CursorOverButton.Dispose();
            return _buttonResult;
        }

        public static DialogResult Show(string message, string title, Buttons buttons, Icone icon)
        {
            TimerCursorOverButtonContructor();

            _msgBox = new MsgBox();
            _msgBox._lblMessage.Text = message;
            _msgBox._lblTitle.Text = title;

            MsgBox.InitButtons(buttons);
            MsgBox.InitIcon(icon);

            _msgBox.Size = MsgBox.MessageSize(message, title, hasIcon: true);

            _timer_CursorOverButton.Start();
            _msgBox.ShowDialog();
            _msgBox.Focus();
            MessageBeep(0);
            _timer_CursorOverButton.Dispose();
            return _buttonResult;
        }

        public static DialogResult Show(string message, string title, Buttons buttons, Icone icon, AnimateStyle style)
        {
            TimerCursorOverButtonContructor();

            _msgBox = new MsgBox();
            _msgBox._lblMessage.Text = message;
            _msgBox._lblTitle.Text = title;
            _msgBox.Height = 0;

            MsgBox.InitButtons(buttons);
            MsgBox.InitIcon(icon);

            _timer = new Timer();
            Size formSize = MsgBox.MessageSize(message, title, hasIcon: true);
            
            switch (style)
            {
                case MsgBox.AnimateStyle.SlideDown:

                    // _msgBox.Size = new Size(formSize.Width, 0);
                    _msgBox.Size = new Size(formSize.Width , 0);
                    _timer.Interval = 15;
                    _timer.Tag = new AnimateMsgBox(formSize, style);
                    break;

                case MsgBox.AnimateStyle.FadeIn:
                    tempBorderColor = _msgBox.BackColor; //to animated effect do work more smoth (without blinking)
                    _msgBox.Size = formSize;
                    _msgBox.Opacity = 0;
                    _timer.Interval = 50;
                    _timer.Tag = new AnimateMsgBox(formSize, style);
                    break;

                case MsgBox.AnimateStyle.ZoomIn:

                    _msgBox.Size = new Size(formSize.Width + 100, formSize.Height + 100);
                    _timer.Tag = new AnimateMsgBox(formSize, style);
                    _timer.Interval = 15;
                    break;
            }

            _timer.Tick += timer_Tick;
            _timer.Start();

            _timer.Disposed += timer_Disposed; //to initiate contdown to cursoroverbutton timer

            _msgBox.ShowDialog();
            _msgBox.Focus();
            MessageBeep(0);
            _timer_CursorOverButton.Dispose();

            return _buttonResult;
        }

        static void timer_Disposed(object sender, EventArgs e)
        {
            //when timer disposed msbBox will be fully formed and positioned
            _timer_CursorOverButton.Start();

        }

        static void timer_CursorOverButton_Tick(object sender, EventArgs e)
        {
            Button myButton = (Button)_msgBox._flpButtons.Controls[0];
            Point buttonLocation = _msgBox._flpButtons.PointToScreen(myButton.Location);
            Point buttonCentre = new Point(buttonLocation.X + myButton.Width / 2, buttonLocation.Y + myButton.Height / 2);
            Point p = myButton.PointToScreen(buttonCentre);
            Cursor.Position = buttonCentre;
            _timer_CursorOverButton.Stop();

        }

        static void timer_Tick(object sender, EventArgs e)
        {
            Timer timer = (Timer)sender;
            AnimateMsgBox animate = (AnimateMsgBox)timer.Tag;
            
            switch (animate.Style)
            {
                case MsgBox.AnimateStyle.SlideDown:
                    if (_msgBox.Height < animate.FormSize.Height)
                    {
                        _msgBox.Height += 17;
                        _msgBox.Invalidate();
                    }
                    else
                    {
                        _timer.Stop();
                        _timer.Dispose();
                    }
                    break;

                case MsgBox.AnimateStyle.FadeIn:
                    if (_msgBox.Opacity < 1)
                    {
                        _msgBox.Opacity += 0.1;
                        _msgBox.Invalidate();
                    }
                    else
                    {
                        _timer.Stop();
                        tempBorderColor = MsgBoxBorderColor;
                        Graphics g = _msgBox.CreateGraphics();
                        _msgBox.DrawRetangle(g);
                        _timer.Dispose();
                    }
                    break;

                case MsgBox.AnimateStyle.ZoomIn:
                    if (_msgBox.Width > animate.FormSize.Width)
                    {
                        _msgBox.Width -= 17;
                        _msgBox.Invalidate();
                    }
                    if (_msgBox.Height > animate.FormSize.Height)
                    {
                        _msgBox.Height -= 17;
                        _msgBox.Invalidate();
                    }
                    if (_msgBox.Width <= animate.FormSize.Width && _msgBox.Height <= animate.FormSize.Height)
                    {
                        _timer.Stop();
                        _timer.Dispose();

                    }
                    break;
            }

        }

        private static void InitButtons(Buttons buttons)
        {
            switch (buttons)
            {
                case MsgBox.Buttons.AbortRetryIgnore:
                    _msgBox.InitAbortRetryIgnoreButtons();
                    break;

                case MsgBox.Buttons.OK:
                    _msgBox.InitOKButton();
                    break;

                case MsgBox.Buttons.OKCancel:
                    _msgBox.InitOKCancelButtons();
                    break;

                case MsgBox.Buttons.RetryCancel:
                    _msgBox.InitRetryCancelButtons();
                    break;

                case MsgBox.Buttons.YesNo:
                    _msgBox.InitYesNoButtons();
                    break;

                case MsgBox.Buttons.YesNoCancel:
                    _msgBox.InitYesNoCancelButtons();
                    break;
            }


            foreach (Button btn in _msgBox._buttonCollection)
            {
                btn.ForeColor = Color.White; // FromArgb(170, 170, 170);
                btn.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
                btn.Padding = new Padding(1);
                btn.FlatStyle = FlatStyle.Standard;
                btn.Height = 30;
                btn.BackColor = Color.FromArgb(30, 30, 30);
                // btn.FlatAppearance.BorderColor = Color.FromArgb(99, 99, 98);

                _msgBox._flpButtons.Controls.Add(btn);

            }
            
        }

        private static void InitIcon(Icone icon)
        {
            switch (icon)
            {
                case MsgBox.Icone.Application:
                    _msgBox._picIcon.Image = SystemIcons.Application.ToBitmap();
                    break;

                case MsgBox.Icone.Exclamation:
                    _msgBox._picIcon.Image = SystemIcons.Exclamation.ToBitmap();
                    break;

                case MsgBox.Icone.Error:
                    _msgBox._picIcon.Image = SystemIcons.Error.ToBitmap();
                    break;

                case MsgBox.Icone.Info:
                    _msgBox._picIcon.Image = SystemIcons.Information.ToBitmap();
                    break;

                case MsgBox.Icone.Question:
                    _msgBox._picIcon.Image = SystemIcons.Question.ToBitmap();
                    break;

                case MsgBox.Icone.Shield:
                    _msgBox._picIcon.Image = SystemIcons.Shield.ToBitmap();
                    break;

                case MsgBox.Icone.Warning:
                    _msgBox._picIcon.Image = SystemIcons.Warning.ToBitmap();
                    break;
            }
        }

        private void InitAbortRetryIgnoreButtons()
        {
            Button btnAbort = new Button();
            btnAbort.Text = "Abort";
            btnAbort.Click += ButtonClick;

            Button btnRetry = new Button();
            btnRetry.Text = "Retry";
            btnRetry.Click += ButtonClick;

            Button btnIgnore = new Button();
            btnIgnore.Text = "Ignore";
            btnIgnore.Click += ButtonClick;

            this._buttonCollection.Add(btnAbort);
            this._buttonCollection.Add(btnRetry);
            this._buttonCollection.Add(btnIgnore);
        }

        private void InitOKButton()
        {
            Button btnOK = new Button();
            btnOK.Text = "Ok";
            btnOK.Click += ButtonClick;

            this._buttonCollection.Add(btnOK);
        }

        private void InitOKCancelButtons()
        {
            Button btnOK = new Button();
            btnOK.Text = "Ok";
            btnOK.Click += ButtonClick;

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Click += ButtonClick;


            this._buttonCollection.Add(btnOK);
            this._buttonCollection.Add(btnCancel);
        }

        private void InitRetryCancelButtons()
        {
            Button btnRetry = new Button();
            btnRetry.Text = "Retry";
            btnRetry.Click += ButtonClick;

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Click += ButtonClick;


            this._buttonCollection.Add(btnRetry);
            this._buttonCollection.Add(btnCancel);
        }

        private void InitYesNoButtons()
        {
            Button btnYes = new Button();
            btnYes.Text = "Yes";
            btnYes.Click += ButtonClick;

            Button btnNo = new Button();
            btnNo.Text = "No";
            btnNo.Click += ButtonClick;


            this._buttonCollection.Add(btnYes);
            this._buttonCollection.Add(btnNo);
        }

        private void InitYesNoCancelButtons()
        {
            Button btnYes = new Button();
            btnYes.Text = "Yes";
            btnYes.Click += ButtonClick;

            Button btnNo = new Button();
            btnNo.Text = "No";
            btnNo.Click += ButtonClick;

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Click += ButtonClick;

            this._buttonCollection.Add(btnYes);
            this._buttonCollection.Add(btnNo);
            this._buttonCollection.Add(btnCancel);

        }

        private static void ButtonClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch (btn.Text)
            {
                case "Abort":
                    _buttonResult = DialogResult.Abort;
                    break;

                case "Retry":
                    _buttonResult = DialogResult.Retry;
                    break;

                case "Ignore":
                    _buttonResult = DialogResult.Ignore;
                    break;

                case "Ok":
                    _buttonResult = DialogResult.OK;
                    break;

                case "Cancel":
                    _buttonResult = DialogResult.Cancel;
                    break;

                case "Yes":
                    _buttonResult = DialogResult.Yes;
                    break;

                case "No":
                    _buttonResult = DialogResult.No;
                    break;
            }

            _msgBox.Dispose();
        }

        private static Size MessageSize(string message, string title = "", bool hasIcon = false)
        {
            // MsgBox msgBox = new MsgBox();
            Graphics gg = (Graphics)_msgBox.Tag;
            // Graphics gg = _msgBox.CreateGraphics();
            SizeF size;
            int widthLine;
            int heightLines;

            string[] groupsUserDefined = Regex.Split(message, "\r\n"); //to detect if programer used line feed

            int nLines = groupsUserDefined.Length;

            int iconIncrement = 0;
            if (hasIcon)
            {
                iconIncrement = _msgBox._plIcon.Width;
                _msgBox._lnkLblTranslate.Padding = new Padding(_msgBox._plIcon.Width, 0, 0, 0); //to show translate message with same padding.
            }

            string masLenStr;
            if (nLines > 1)  //programer are using carriers returns and feed lines in big messages. This will show dialog as programed.
            {
                masLenStr = MaxLenghtString(groupsUserDefined); //the biggest string among all, defined by programer.
            }
            else  //one line only. This code will split it.
            {

                int mesLen = message.Length;
                int stringPiece = 60;
                if (mesLen > 1500)
                {
                    stringPiece = mesLen / 25;  //max 25 lines (to fit 768pixels monitors). The growth will be in width.
                }

                string[] groupsAutoDefined = (from Match m in Regex.Matches(message, @".{1," + stringPiece + @"}(\s|$)") select m.Value).ToArray(); //space or endofstring 
                masLenStr = MaxLenghtString(groupsAutoDefined);
                nLines = groupsAutoDefined.Length; //regex will escape the last line so +1.
            }

            size = gg.MeasureString(masLenStr, _msgBox._lblMessage.Font);

            widthLine = (int)size.Width + 1;
            heightLines = (int)((decimal)size.Height * nLines) + 1; //round to top


            //if title text is wider than... it is determinant
            size = gg.MeasureString(title, _msgBox._lblTitle.Font);
            if ((int)size.Width > widthLine) //whenever title is wider than message
            {
                widthLine = (int)size.Width;
            }

            //if translate text is wider than... it is determinant
            size = gg.MeasureString(_msgBox._lnkLblTranslate.Text, _msgBox._lnkLblTranslate.Font);
            if ((int)size.Width > widthLine) //whenever title is wider than message
            {
                widthLine = (int)size.Width;
            }

            int incrementFactorH = 10; //auto padding etc...

            int padding = 2 * _msgBox._plHeader.Padding.Left; //2 for width/height

            int titleH = 0;

            if (title != "")
            {
                titleH = _msgBox._lblTitle.Height;
            }
            else
            {
                _msgBox._lblTitle.Hide();
            }


            int footer = _msgBox._plFooter.Height;
            int translate = _msgBox._plTranslate.Height;

            int widthTot = widthLine + incrementFactorH + padding + iconIncrement;
            int heightTot = heightLines + titleH + footer + padding + translate;

            return new Size(widthTot, heightTot);
        }


        private static string MaxLenghtString(string[] group)
        {
            Graphics g = _msgBox.CreateGraphics();
            SizeF size;

            float len = 0;
            string maxStr = "";
            foreach (string item in group)
            {
                size = g.MeasureString(item, _msgBox._lblMessage.Font);
                if (size.Width > len)
                {
                    maxStr = item;
                    len = size.Width;
                }
            }
            return maxStr;
        }


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
            AbortRetryIgnore = 1,
            OK = 2,
            OKCancel = 3,
            RetryCancel = 4,
            YesNo = 5,
            YesNoCancel = 6
        }

        public enum Icone
        {
            Application = 1,
            Exclamation = 2,
            Error = 3,
            Warning = 4,
            Info = 5,
            Question = 6,
            Shield = 7,
            Search = 8
        }

        public enum AnimateStyle
        {
            SlideDown = 1,
            FadeIn = 2,
            ZoomIn = 3
        }

    }

    class AnimateMsgBox
    {
        public Size FormSize;
        public MsgBox.AnimateStyle Style;

        public AnimateMsgBox(Size formSize, MsgBox.AnimateStyle style)
        {
            this.FormSize = formSize;
            this.Style = style;
        }
    }
}
