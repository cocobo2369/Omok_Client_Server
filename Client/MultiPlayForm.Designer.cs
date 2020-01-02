namespace Client
{
    partial class MultiPlayForm
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
            this.boardPicture = new System.Windows.Forms.PictureBox();
            this.connectBtn = new System.Windows.Forms.Button();
            this.playBtn = new System.Windows.Forms.Button();
            this.status = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.boardPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // boardPicture
            // 
            this.boardPicture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(216)))));
            this.boardPicture.Location = new System.Drawing.Point(20, 20);
            this.boardPicture.Name = "boardPicture";
            this.boardPicture.Size = new System.Drawing.Size(500, 500);
            this.boardPicture.TabIndex = 0;
            this.boardPicture.TabStop = false;
            this.boardPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.boardPicture_Paint);
            this.boardPicture.MouseDown += new System.Windows.Forms.MouseEventHandler(this.boardPicture_MouseDown);
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(752, 20);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(100, 40);
            this.connectBtn.TabIndex = 1;
            this.connectBtn.Text = "접속하기";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // playBtn
            // 
            this.playBtn.Location = new System.Drawing.Point(644, 126);
            this.playBtn.Name = "playBtn";
            this.playBtn.Size = new System.Drawing.Size(100, 40);
            this.playBtn.TabIndex = 2;
            this.playBtn.Text = "게임시작";
            this.playBtn.UseVisualStyleBackColor = true;
            this.playBtn.Click += new System.EventHandler(this.playBtn_Click);
            // 
            // status
            // 
            this.status.Location = new System.Drawing.Point(562, 179);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(272, 15);
            this.status.TabIndex = 3;
            this.status.Text = "방을 입력하여 접속해주세요.";
            this.status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(644, 31);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(100, 21);
            this.textBox.TabIndex = 4;
            // 
            // MultiPlayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 541);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.status);
            this.Controls.Add(this.playBtn);
            this.Controls.Add(this.connectBtn);
            this.Controls.Add(this.boardPicture);
            this.Name = "MultiPlayForm";
            this.Text = "MultiPlayForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MultiPlayForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.boardPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox boardPicture;
        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.Button playBtn;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.TextBox textBox;
    }
}