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
            ((System.ComponentModel.ISupportInitialize)(this.resultBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_autoRefresh
            // 
            this.btn_autoRefresh.BackColor = System.Drawing.SystemColors.WindowText;
            this.btn_autoRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_autoRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_autoRefresh.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btn_autoRefresh.Location = new System.Drawing.Point(524, 338);
            this.btn_autoRefresh.Name = "btn_autoRefresh";
            this.btn_autoRefresh.Size = new System.Drawing.Size(123, 31);
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
            this.resultBox.Location = new System.Drawing.Point(12, 12);
            this.resultBox.Name = "resultBox";
            this.resultBox.Size = new System.Drawing.Size(635, 277);
            this.resultBox.TabIndex = 7;
            this.resultBox.TabStop = false;
            // 
            // txt_currPosi
            // 
            this.txt_currPosi.AutoSize = true;
            this.txt_currPosi.ForeColor = System.Drawing.SystemColors.Control;
            this.txt_currPosi.Location = new System.Drawing.Point(9, 343);
            this.txt_currPosi.Name = "txt_currPosi";
            this.txt_currPosi.Size = new System.Drawing.Size(201, 13);
            this.txt_currPosi.TabIndex = 8;
            this.txt_currPosi.Text = "Position / Starting position / Elapsed time";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(9, 356);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "LOG:";
            // 
            // txt_loglabel
            // 
            this.txt_loglabel.AutoSize = true;
            this.txt_loglabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txt_loglabel.Location = new System.Drawing.Point(47, 356);
            this.txt_loglabel.Name = "txt_loglabel";
            this.txt_loglabel.Size = new System.Drawing.Size(245, 13);
            this.txt_loglabel.TabIndex = 10;
            this.txt_loglabel.Text = "This will show a log message when one is created.";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(659, 375);
            this.Controls.Add(this.txt_loglabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_currPosi);
            this.Controls.Add(this.resultBox);
            this.Controls.Add(this.btn_autoRefresh);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
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
    }
}