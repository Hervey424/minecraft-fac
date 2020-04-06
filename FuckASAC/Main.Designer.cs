namespace FuckASAC
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.label1 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblTip = new System.Windows.Forms.Label();
            this.btnCopy = new System.Windows.Forms.Button();
            this.cbServer = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rb1710 = new System.Windows.Forms.RadioButton();
            this.rb1122 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "输入地址：";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(16, 102);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "开　启";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(107, 102);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "关  闭";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblTip
            // 
            this.lblTip.AutoSize = true;
            this.lblTip.Location = new System.Drawing.Point(14, 70);
            this.lblTip.Name = "lblTip";
            this.lblTip.Size = new System.Drawing.Size(281, 12);
            this.lblTip.TabIndex = 6;
            this.lblTip.Text = "请在游戏中把服务器地址设置为127.0.0.1:本地端口";
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(322, 65);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 8;
            this.btnCopy.Text = "点击复制";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Visible = false;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // cbServer
            // 
            this.cbServer.FormattingEnabled = true;
            this.cbServer.Location = new System.Drawing.Point(78, 8);
            this.cbServer.Name = "cbServer";
            this.cbServer.Size = new System.Drawing.Size(318, 20);
            this.cbServer.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "游戏版本：";
            // 
            // rb1710
            // 
            this.rb1710.AutoSize = true;
            this.rb1710.Checked = true;
            this.rb1710.Location = new System.Drawing.Point(78, 38);
            this.rb1710.Name = "rb1710";
            this.rb1710.Size = new System.Drawing.Size(59, 16);
            this.rb1710.TabIndex = 11;
            this.rb1710.TabStop = true;
            this.rb1710.Text = "1.7.10";
            this.rb1710.UseVisualStyleBackColor = true;
            // 
            // rb1122
            // 
            this.rb1122.AutoSize = true;
            this.rb1122.Location = new System.Drawing.Point(143, 38);
            this.rb1122.Name = "rb1122";
            this.rb1122.Size = new System.Drawing.Size(59, 16);
            this.rb1122.TabIndex = 12;
            this.rb1122.Text = "1.12.2";
            this.rb1122.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 144);
            this.Controls.Add(this.rb1122);
            this.Controls.Add(this.rb1710);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbServer);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.lblTip);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "反作弊破解 V1.2 - 交流群:543258992";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblTip;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.ComboBox cbServer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rb1710;
        private System.Windows.Forms.RadioButton rb1122;
    }
}

