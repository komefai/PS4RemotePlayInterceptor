namespace PS4RemotePlayInterceptorDemo
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
            this.injectButton = new System.Windows.Forms.Button();
            this.pidLabel = new System.Windows.Forms.Label();
            this.sendStartSignalButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // injectButton
            // 
            this.injectButton.Location = new System.Drawing.Point(12, 47);
            this.injectButton.Name = "injectButton";
            this.injectButton.Size = new System.Drawing.Size(318, 23);
            this.injectButton.TabIndex = 0;
            this.injectButton.Text = "Inject";
            this.injectButton.UseVisualStyleBackColor = true;
            this.injectButton.Click += new System.EventHandler(this.injectButton_Click);
            // 
            // pidLabel
            // 
            this.pidLabel.AutoSize = true;
            this.pidLabel.Location = new System.Drawing.Point(12, 21);
            this.pidLabel.Name = "pidLabel";
            this.pidLabel.Size = new System.Drawing.Size(68, 13);
            this.pidLabel.TabIndex = 1;
            this.pidLabel.Text = "Target PID: -";
            // 
            // sendStartSignalButton
            // 
            this.sendStartSignalButton.Location = new System.Drawing.Point(231, 16);
            this.sendStartSignalButton.Name = "sendStartSignalButton";
            this.sendStartSignalButton.Size = new System.Drawing.Size(99, 23);
            this.sendStartSignalButton.TabIndex = 2;
            this.sendStartSignalButton.Text = "Send Start Signal";
            this.sendStartSignalButton.UseVisualStyleBackColor = true;
            this.sendStartSignalButton.Click += new System.EventHandler(this.sendStartSignalButton_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 82);
            this.Controls.Add(this.sendStartSignalButton);
            this.Controls.Add(this.pidLabel);
            this.Controls.Add(this.injectButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "PS4 Remote Play Interceptor Demo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button injectButton;
        private System.Windows.Forms.Label pidLabel;
        private System.Windows.Forms.Button sendStartSignalButton;
    }
}

