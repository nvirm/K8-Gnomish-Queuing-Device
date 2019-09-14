namespace Gnomish_queuing_device
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btn_autoRefresh = new System.Windows.Forms.Button();
            this.AutoRefresh = new System.Windows.Forms.Timer(this.components);
            this.resultBox = new System.Windows.Forms.PictureBox();
            this.txt_currPosi = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_loglabel = new System.Windows.Forms.Label();
            this.txt_speed = new System.Windows.Forms.Label();
            this.txt_etrlabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.resultBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_autoRefresh
            // 
            this.btn_autoRefresh.BackColor = System.Drawing.SystemColors.WindowText;
            this.btn_autoRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_autoRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_autoRefresh.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btn_autoRefresh.Location = new System.Drawing.Point(327, 247);
            this.btn_autoRefresh.Name = "btn_autoRefresh";
            this.btn_autoRefresh.Size = new System.Drawing.Size(78, 31);
            this.btn_autoRefresh.TabIndex = 1;
            this.btn_autoRefresh.Text = "Start GQD";
            this.btn_autoRefresh.UseVisualStyleBackColor = false;
            this.btn_autoRefresh.Click += new System.EventHandler(this.Btn_autoRefresh_Click);
            // 
            // AutoRefresh
            // 
            this.AutoRefresh.Interval = 15000;
            this.AutoRefresh.Tick += new System.EventHandler(this.AutoRefresh_Tick_1);
            // 
            // resultBox
            // 
            this.resultBox.BackColor = System.Drawing.Color.Lime;
            this.resultBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.resultBox.Location = new System.Drawing.Point(0, -1);
            this.resultBox.Name = "resultBox";
            this.resultBox.Size = new System.Drawing.Size(417, 234);
            this.resultBox.TabIndex = 7;
            this.resultBox.TabStop = false;
            // 
            // txt_currPosi
            // 
            this.txt_currPosi.AutoSize = true;
            this.txt_currPosi.ForeColor = System.Drawing.SystemColors.Control;
            this.txt_currPosi.Location = new System.Drawing.Point(12, 278);
            this.txt_currPosi.Name = "txt_currPosi";
            this.txt_currPosi.Size = new System.Drawing.Size(201, 13);
            this.txt_currPosi.TabIndex = 8;
            this.txt_currPosi.Text = "Position / Starting position / Elapsed time";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(12, 312);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "LOG:";
            // 
            // txt_loglabel
            // 
            this.txt_loglabel.AutoSize = true;
            this.txt_loglabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txt_loglabel.Location = new System.Drawing.Point(50, 312);
            this.txt_loglabel.Name = "txt_loglabel";
            this.txt_loglabel.Size = new System.Drawing.Size(245, 13);
            this.txt_loglabel.TabIndex = 10;
            this.txt_loglabel.Text = "This will show a log message when one is created.";
            // 
            // txt_speed
            // 
            this.txt_speed.AutoSize = true;
            this.txt_speed.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txt_speed.Location = new System.Drawing.Point(12, 265);
            this.txt_speed.Name = "txt_speed";
            this.txt_speed.Size = new System.Drawing.Size(41, 13);
            this.txt_speed.TabIndex = 11;
            this.txt_speed.Text = "Speed:";
            // 
            // txt_etrlabel
            // 
            this.txt_etrlabel.AutoSize = true;
            this.txt_etrlabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txt_etrlabel.Location = new System.Drawing.Point(12, 291);
            this.txt_etrlabel.Name = "txt_etrlabel";
            this.txt_etrlabel.Size = new System.Drawing.Size(29, 13);
            this.txt_etrlabel.TabIndex = 12;
            this.txt_etrlabel.Text = "ETR";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(415, 330);
            this.Controls.Add(this.txt_etrlabel);
            this.Controls.Add(this.txt_speed);
            this.Controls.Add(this.txt_loglabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_currPosi);
            this.Controls.Add(this.resultBox);
            this.Controls.Add(this.btn_autoRefresh);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(683, 369);
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "K8 Gnomish Queuing Device";
            this.TransparencyKey = System.Drawing.Color.Lime;
            ((System.ComponentModel.ISupportInitialize)(this.resultBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Timer AutoRefresh;
        public System.Windows.Forms.Button btn_autoRefresh;
        public System.Windows.Forms.Label txt_loglabel;
        public System.Windows.Forms.Label txt_currPosi;
        public System.Windows.Forms.PictureBox resultBox;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label txt_speed;
        public System.Windows.Forms.Label txt_etrlabel;
    }
}