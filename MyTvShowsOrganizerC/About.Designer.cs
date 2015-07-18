namespace MyTvShowsOrganizer
{
    partial class About
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.flowLayoutPanel_About = new System.Windows.Forms.FlowLayoutPanel();
            this.button_TellaFriend = new System.Windows.Forms.Button();
            this.button_Update = new System.Windows.Forms.Button();
            this.button_Donate = new System.Windows.Forms.Button();
            this.button_WhatsNew = new System.Windows.Forms.Button();
            this.button_Bugs = new System.Windows.Forms.Button();
            this.button_Review = new System.Windows.Forms.Button();
            this.button_Close = new System.Windows.Forms.Button();
            this.contextMenuStrip_Translate = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_Translate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip_Form_About = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanel_About.SuspendLayout();
            this.contextMenuStrip_Translate.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel_About
            // 
            this.flowLayoutPanel_About.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.flowLayoutPanel_About.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flowLayoutPanel_About.Controls.Add(this.button_TellaFriend);
            this.flowLayoutPanel_About.Controls.Add(this.button_Update);
            this.flowLayoutPanel_About.Controls.Add(this.button_Donate);
            this.flowLayoutPanel_About.Controls.Add(this.button_WhatsNew);
            this.flowLayoutPanel_About.Controls.Add(this.button_Bugs);
            this.flowLayoutPanel_About.Controls.Add(this.button_Review);
            this.flowLayoutPanel_About.Controls.Add(this.button_Close);
            this.flowLayoutPanel_About.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel_About.Location = new System.Drawing.Point(10, 12);
            this.flowLayoutPanel_About.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel_About.Name = "flowLayoutPanel_About";
            this.flowLayoutPanel_About.Padding = new System.Windows.Forms.Padding(8);
            this.flowLayoutPanel_About.Size = new System.Drawing.Size(308, 310);
            this.flowLayoutPanel_About.TabIndex = 0;
            // 
            // button_TellaFriend
            // 
            this.button_TellaFriend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.button_TellaFriend.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_TellaFriend.ForeColor = System.Drawing.Color.White;
            this.button_TellaFriend.Location = new System.Drawing.Point(11, 11);
            this.button_TellaFriend.Name = "button_TellaFriend";
            this.button_TellaFriend.Size = new System.Drawing.Size(281, 35);
            this.button_TellaFriend.TabIndex = 0;
            this.button_TellaFriend.Text = "&Tell a Friend";
            this.toolTip_Form_About.SetToolTip(this.button_TellaFriend, "Tell a Friend About MyTvShowOrganizer. Click to Open your Default Mail Program.");
            this.button_TellaFriend.UseVisualStyleBackColor = false;
            this.button_TellaFriend.Click += new System.EventHandler(this.button_TellaFriend_Click);
            // 
            // button_Update
            // 
            this.button_Update.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.button_Update.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Update.ForeColor = System.Drawing.Color.White;
            this.button_Update.Location = new System.Drawing.Point(11, 52);
            this.button_Update.Name = "button_Update";
            this.button_Update.Size = new System.Drawing.Size(281, 35);
            this.button_Update.TabIndex = 3;
            this.button_Update.Text = "&Check For Updates";
            this.toolTip_Form_About.SetToolTip(this.button_Update, "Check If There is a New Version Available.");
            this.button_Update.UseVisualStyleBackColor = false;
            this.button_Update.Click += new System.EventHandler(this.button_Update_Click);
            // 
            // button_Donate
            // 
            this.button_Donate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.button_Donate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Donate.ForeColor = System.Drawing.Color.White;
            this.button_Donate.Location = new System.Drawing.Point(11, 93);
            this.button_Donate.Name = "button_Donate";
            this.button_Donate.Size = new System.Drawing.Size(281, 35);
            this.button_Donate.TabIndex = 5;
            this.button_Donate.Text = "&Donate USD 1.99 (PayPal)";
            this.toolTip_Form_About.SetToolTip(this.button_Donate, "Go to PayPal WebPage of MyTvShowOrganizer.");
            this.button_Donate.UseVisualStyleBackColor = false;
            this.button_Donate.Click += new System.EventHandler(this.button_Donate_Click);
            // 
            // button_WhatsNew
            // 
            this.button_WhatsNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.button_WhatsNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_WhatsNew.ForeColor = System.Drawing.Color.White;
            this.button_WhatsNew.Location = new System.Drawing.Point(11, 134);
            this.button_WhatsNew.Name = "button_WhatsNew";
            this.button_WhatsNew.Size = new System.Drawing.Size(281, 35);
            this.button_WhatsNew.TabIndex = 8;
            this.button_WhatsNew.Text = "Version History";
            this.toolTip_Form_About.SetToolTip(this.button_WhatsNew, "What is New in Latest Version.");
            this.button_WhatsNew.UseVisualStyleBackColor = false;
            this.button_WhatsNew.Click += new System.EventHandler(this.button_WhatsNew_Click);
            // 
            // button_Bugs
            // 
            this.button_Bugs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.button_Bugs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Bugs.ForeColor = System.Drawing.Color.White;
            this.button_Bugs.Location = new System.Drawing.Point(11, 175);
            this.button_Bugs.Name = "button_Bugs";
            this.button_Bugs.Size = new System.Drawing.Size(281, 35);
            this.button_Bugs.TabIndex = 6;
            this.button_Bugs.Text = "&Bugs / Suggestions";
            this.toolTip_Form_About.SetToolTip(this.button_Bugs, "Go to Wiki page to Inform Bugs or Suggestion.");
            this.button_Bugs.UseVisualStyleBackColor = false;
            this.button_Bugs.Click += new System.EventHandler(this.button_Bugs_Click);
            // 
            // button_Review
            // 
            this.button_Review.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.button_Review.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Review.ForeColor = System.Drawing.Color.White;
            this.button_Review.Location = new System.Drawing.Point(11, 216);
            this.button_Review.Name = "button_Review";
            this.button_Review.Size = new System.Drawing.Size(281, 35);
            this.button_Review.TabIndex = 4;
            this.button_Review.Text = "&Write a Review";
            this.toolTip_Form_About.SetToolTip(this.button_Review, "Do a Review of MyTvShowOrganizer.");
            this.button_Review.UseVisualStyleBackColor = false;
            this.button_Review.Click += new System.EventHandler(this.button_Review_Click);
            // 
            // button_Close
            // 
            this.button_Close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.button_Close.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Close.ForeColor = System.Drawing.Color.White;
            this.button_Close.Location = new System.Drawing.Point(11, 257);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(281, 35);
            this.button_Close.TabIndex = 2;
            this.button_Close.Text = "Cl&ose";
            this.button_Close.UseVisualStyleBackColor = false;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // contextMenuStrip_Translate
            // 
            this.contextMenuStrip_Translate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.contextMenuStrip_Translate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Translate});
            this.contextMenuStrip_Translate.Name = "contextMenuStrip_Translate";
            this.contextMenuStrip_Translate.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip_Translate.Size = new System.Drawing.Size(194, 42);
            // 
            // toolStripMenuItem_Translate
            // 
            this.toolStripMenuItem_Translate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.toolStripMenuItem_Translate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.toolStripMenuItem_Translate.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMenuItem_Translate.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem_Translate.Image = global::MyTvShowsOrganizer.Properties.Resources.binggreensmall2_32;
            this.toolStripMenuItem_Translate.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripMenuItem_Translate.Name = "toolStripMenuItem_Translate";
            this.toolStripMenuItem_Translate.Size = new System.Drawing.Size(193, 38);
            this.toolStripMenuItem_Translate.Text = "En → Ҩἒὧℓ₯";
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(50)))), ((int)(((byte)(30)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(324, 331);
            this.ControlBox = false;
            this.Controls.Add(this.flowLayoutPanel_About);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "About";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.flowLayoutPanel_About.ResumeLayout(false);
            this.contextMenuStrip_Translate.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_About;
        private System.Windows.Forms.Button button_TellaFriend;
        private System.Windows.Forms.Button button_Close;
        private System.Windows.Forms.Button button_Update;
        private System.Windows.Forms.Button button_Review;
        private System.Windows.Forms.Button button_Donate;
        private System.Windows.Forms.Button button_Bugs;
        private System.Windows.Forms.ToolTip toolTip_Form_About;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Translate;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Translate;
        private System.Windows.Forms.Button button_WhatsNew;
    }
}