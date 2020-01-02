namespace Client
{
    partial class MenuForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.SinglePlayBtn = new System.Windows.Forms.Button();
            this.ExitBtn = new System.Windows.Forms.Button();
            this.MultiPlayBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SinglePlayBtn
            // 
            this.SinglePlayBtn.Location = new System.Drawing.Point(332, 124);
            this.SinglePlayBtn.Name = "SinglePlayBtn";
            this.SinglePlayBtn.Size = new System.Drawing.Size(125, 53);
            this.SinglePlayBtn.TabIndex = 0;
            this.SinglePlayBtn.Text = "혼자하기";
            this.SinglePlayBtn.UseVisualStyleBackColor = true;
            this.SinglePlayBtn.Click += new System.EventHandler(this.SinglePlayBtn_Click);
            // 
            // ExitBtn
            // 
            this.ExitBtn.Location = new System.Drawing.Point(332, 270);
            this.ExitBtn.Name = "ExitBtn";
            this.ExitBtn.Size = new System.Drawing.Size(125, 53);
            this.ExitBtn.TabIndex = 1;
            this.ExitBtn.Text = "종료하기";
            this.ExitBtn.UseVisualStyleBackColor = true;
            this.ExitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
            // 
            // MultiPlayBtn
            // 
            this.MultiPlayBtn.Location = new System.Drawing.Point(332, 196);
            this.MultiPlayBtn.Name = "MultiPlayBtn";
            this.MultiPlayBtn.Size = new System.Drawing.Size(125, 53);
            this.MultiPlayBtn.TabIndex = 2;
            this.MultiPlayBtn.Text = "둘이하기";
            this.MultiPlayBtn.UseVisualStyleBackColor = true;
            this.MultiPlayBtn.Click += new System.EventHandler(this.MultiPlayBtn_Click);
            // 
            // MenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MultiPlayBtn);
            this.Controls.Add(this.ExitBtn);
            this.Controls.Add(this.SinglePlayBtn);
            this.Name = "MenuForm";
            this.Text = "Omok";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SinglePlayBtn;
        private System.Windows.Forms.Button ExitBtn;
        private System.Windows.Forms.Button MultiPlayBtn;
    }
}

