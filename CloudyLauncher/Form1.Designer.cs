namespace CloudyLauncher
{
  partial class Form1
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
        components.Dispose();

      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.label1 = new System.Windows.Forms.Label();
      this.customImageButton2 = new CloudyLauncher.CustomImageButton();
      this.customImageButton1 = new CloudyLauncher.CustomImageButton();
      this.customProgressBar1 = new CloudyLauncher.CustomProgressBar();
      this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
      this.SuspendLayout();
      //
      // label1
      //
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.BackColor = System.Drawing.Color.Transparent;
      this.label1.Font = new System.Drawing.Font("Segoe UI Semilight", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(215)))), ((int)(((byte)(227)))));
      this.label1.Location = new System.Drawing.Point(9, 318);
      this.label1.Margin = new System.Windows.Forms.Padding(0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(195, 25);
      this.label1.TabIndex = 1;
      this.label1.Text = "verifying...";
      this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
      //
      // customImageButton2
      //
      this.customImageButton2.ImageClick = global::CloudyLauncher.Properties.Resources.quit_hover;
      this.customImageButton2.ImageDefault = global::CloudyLauncher.Properties.Resources.quit_default;
      this.customImageButton2.ImageHover = global::CloudyLauncher.Properties.Resources.quit_click;
      this.customImageButton2.Location = new System.Drawing.Point(620, 319);
      this.customImageButton2.Name = "customImageButton2";
      this.customImageButton2.Size = new System.Drawing.Size(20, 20);
      this.customImageButton2.TabIndex = 3;
      //
      // customImageButton1
      //
      this.customImageButton1.ImageClick = global::CloudyLauncher.Properties.Resources.play_hover;
      this.customImageButton1.ImageDefault = global::CloudyLauncher.Properties.Resources.play_default;
      this.customImageButton1.ImageHover = global::CloudyLauncher.Properties.Resources.play_click;
      this.customImageButton1.Location = new System.Drawing.Point(173, 319);
      this.customImageButton1.Name = "customImageButton1";
      this.customImageButton1.Size = new System.Drawing.Size(20, 20);
      this.customImageButton1.TabIndex = 2;
      //
      // customProgressBar1
      //
      this.customProgressBar1.BackColor = System.Drawing.Color.White;
      this.customProgressBar1.BackgroundColor = System.Drawing.Color.White;
      this.customProgressBar1.ForegroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(82)))), ((int)(((byte)(182)))));
      this.customProgressBar1.Location = new System.Drawing.Point(210, 329);
      this.customProgressBar1.Maximum = 100F;
      this.customProgressBar1.Name = "customProgressBar1";
      this.customProgressBar1.Size = new System.Drawing.Size(393, 2);
      this.customProgressBar1.TabIndex = 0;
      this.customProgressBar1.Value = 100F;
      //
      // backgroundWorker1
      //
      this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
      //
      // Form1
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
      this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
      this.ClientSize = new System.Drawing.Size(802, 436);
      this.Controls.Add(this.customImageButton2);
      this.Controls.Add(this.customImageButton1);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.customProgressBar1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.Name = "Form1";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "CloudyLauncher";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.ResumeLayout(false);
    }

    #endregion

    private CustomProgressBar customProgressBar1;
    private System.Windows.Forms.Label label1;
    private CustomImageButton customImageButton1;
    private CustomImageButton customImageButton2;
    private System.ComponentModel.BackgroundWorker backgroundWorker1;

  }
}

