using OTRS_AD_Script_Creator.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace OTRS_AD_Script_Creator
{
	public class intro : Form
	{
		private IContainer components;

		private PictureBox Gif_pictureBox;

		private PictureBox Jpg_pictureBox;

		private Timer gif_timer;

		private Label Name_label;

		private LinkLabel linkLabel;

		public intro()
		{
			this.InitializeComponent();
			this.gif_timer.Start();
			base.PreviewKeyDown += new PreviewKeyDownEventHandler(this.intro_PreviewKeyDown);
			this.Gif_pictureBox.PreviewKeyDown += new PreviewKeyDownEventHandler(this.Gif_pictureBox_PreviewKeyDown);
		}

		private void intro_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			base.Close();
		}

		private void Gif_pictureBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			base.Close();
		}

		private void gif_timer_Tick(object sender, EventArgs e)
		{
			this.Jpg_pictureBox.Visible = true;
			if (this.gif_timer.Interval == 3000)
			{
				base.Close();
			}
			this.gif_timer.Interval = 3000;
		}

		private void OnMouseClick(object sender, MouseEventArgs e)
		{
			base.Close();
		}

		private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://otrsconfig.site");
		}

		private void Jpg_pictureBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			base.Close();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(intro));
			this.Gif_pictureBox = new PictureBox();
			this.Jpg_pictureBox = new PictureBox();
			this.gif_timer = new Timer(this.components);
			this.Name_label = new Label();
			this.linkLabel = new LinkLabel();
			((ISupportInitialize)this.Gif_pictureBox).BeginInit();
			((ISupportInitialize)this.Jpg_pictureBox).BeginInit();
			base.SuspendLayout();
			this.Gif_pictureBox.BorderStyle = BorderStyle.FixedSingle;
			this.Gif_pictureBox.Image = Resources.otrs_intro;
			this.Gif_pictureBox.Location = new Point(0, 0);
			this.Gif_pictureBox.Name = "Gif_pictureBox";
			this.Gif_pictureBox.Size = new Size(260, 148);
			this.Gif_pictureBox.TabIndex = 0;
			this.Gif_pictureBox.TabStop = false;
			this.Gif_pictureBox.MouseClick += new MouseEventHandler(this.OnMouseClick);
			this.Jpg_pictureBox.Image = Resources.otrs_ad_pic;
			this.Jpg_pictureBox.Location = new Point(0, 0);
			this.Jpg_pictureBox.Name = "Jpg_pictureBox";
			this.Jpg_pictureBox.Size = new Size(260, 148);
			this.Jpg_pictureBox.TabIndex = 1;
			this.Jpg_pictureBox.TabStop = false;
			this.Jpg_pictureBox.Visible = false;
			this.Jpg_pictureBox.MouseClick += new MouseEventHandler(this.OnMouseClick);
			this.Jpg_pictureBox.PreviewKeyDown += new PreviewKeyDownEventHandler(this.Jpg_pictureBox_PreviewKeyDown);
			this.gif_timer.Interval = 5000;
			this.gif_timer.Tick += new EventHandler(this.gif_timer_Tick);
			this.Name_label.AutoSize = true;
			this.Name_label.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.Name_label.Location = new Point(12, 151);
			this.Name_label.Name = "Name_label";
			this.Name_label.Size = new Size(132, 13);
			this.Name_label.TabIndex = 2;
			this.Name_label.Text = "by Daniel Abou Chleih";
			this.Name_label.TextAlign = ContentAlignment.MiddleCenter;
			this.linkLabel.AutoSize = true;
			this.linkLabel.Location = new Point(150, 151);
			this.linkLabel.Name = "linkLabel";
			this.linkLabel.Size = new Size(105, 13);
			this.linkLabel.TabIndex = 3;
			this.linkLabel.TabStop = true;
			this.linkLabel.Text = "http://otrsconfig.site";
			this.linkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(260, 168);
			base.Controls.Add(this.linkLabel);
			base.Controls.Add(this.Name_label);
			base.Controls.Add(this.Jpg_pictureBox);
			base.Controls.Add(this.Gif_pictureBox);
			base.FormBorderStyle = FormBorderStyle.None;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.KeyPreview = true;
			base.Name = "intro";
			this.Text = "intro";
			((ISupportInitialize)this.Gif_pictureBox).EndInit();
			((ISupportInitialize)this.Jpg_pictureBox).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
