using System.Windows.Forms;

namespace MyTvShowsOrganizer
{

    public partial class SplashScreen : Form
    {

        private delegate void ProgressDelegate(int progress);
        private ProgressDelegate deleg;


        public SplashScreen()
        {

            InitializeComponent();
            //Application.EnableVisualStyles();
            this.textBox1.Text = "";
            this.textBox2.Text = "";
            this.textBox3.Text = "";
            this.textBox4.Text = "";
            //this.textBox5.Text = "";
            //this.progressBar1.Maximum = 100;

            deleg = this.UpdateProgressInternal;
            // this.Opacity = 0.70D;
        }

        private void UpdateProgressInternal(int progress)
        {
            if (this.Handle == null)
            {
                return;
            }
            this.progressBar1.Value = progress;

            switch (progress)
            {
                case 10:
                    this.textBox1.Text = "My";
                    //this.progressBar1.ForeColor = System.Drawing.Color.MediumSeaGreen;
                    this.Opacity = 0.80D;
                    this.pictureBox1.Image = global::MyTvShowsOrganizer.Properties.Resources.logo_image_Splash_3002;
                    break;
                case 20:

                    //this.progressBar1.ForeColor = System.Drawing.Color.Aquamarine;
                    //this.Opacity = 1D;
                    this.pictureBox1.Image = global::MyTvShowsOrganizer.Properties.Resources.logo_image_Splash_3003;
                    break;
                case 30:
                    this.textBox2.Text += "Tv";
                    // this.progressBar1.ForeColor = System.Drawing.Color.SpringGreen;
                    this.Opacity = 1D;
                    this.pictureBox1.Image = global::MyTvShowsOrganizer.Properties.Resources.logo_image_Splash_3002;
                    break;
                case 40:

                    //this.Opacity = 1D;
                    this.pictureBox1.Image = global::MyTvShowsOrganizer.Properties.Resources.logo_image_Splash_3001;
                    // this.progressBar1.ForeColor = System.Drawing.Color.Lime;
                    break;
                case 50:
                    this.textBox3.Text += "Show";
                    // this.Opacity = 1D;
                    this.pictureBox1.Image = global::MyTvShowsOrganizer.Properties.Resources.logo_image_Splash_3002;
                    //    // this.progressBar1.ForeColor = System.Drawing.Color.Lime;
                    break;
                case 60:
                    //    this.textBox4.Text += "Organizer";
                    //    this.Opacity = 1D;
                    this.pictureBox1.Image = global::MyTvShowsOrganizer.Properties.Resources.logo_image_Splash_3003;
                    //    // this.progressBar1.ForeColor = System.Drawing.Color.Lime;
                    break;
                case 70:
                    this.textBox4.Text += "Organizer";
                    //    this.Opacity = 1D;
                    this.pictureBox1.Image = global::MyTvShowsOrganizer.Properties.Resources.logo_image_Splash_3002;
                    //    // this.progressBar1.ForeColor = System.Drawing.Color.Lime;
                    break;
                case 80:
                    // this.textBox5.Text += "nizer";
                    // this.Opacity = 75D;

                    this.pictureBox1.Image = global::MyTvShowsOrganizer.Properties.Resources.logo_image_Splash_3001;
                    //this.Refresh();
                    //    // this.progressBar1.ForeColor = System.Drawing.Color.Lime;
                    break;
                case 90:
                    // this.textBox5.Text += "nizer";
                    //  this.Opacity = 60D;
                    this.pictureBox1.Image = global::MyTvShowsOrganizer.Properties.Resources.logo_image_Splash_3002;
                    //this.Refresh();
                    // this.progressBar1.ForeColor = System.Drawing.Color.Lime;
                    break;

            }

        }

        public void UpdateProgress(int progress)
        {
            this.Invoke(deleg, progress);
        }
    }
}
