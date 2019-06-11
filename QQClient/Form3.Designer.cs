namespace New_QQ
{
    partial class Form3
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button1_logIn = new System.Windows.Forms.Button();
            this.button2_logOut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(260, 94);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // button1_logIn
            // 
            this.button1_logIn.Location = new System.Drawing.Point(120, 112);
            this.button1_logIn.Name = "button1_logIn";
            this.button1_logIn.Size = new System.Drawing.Size(75, 23);
            this.button1_logIn.TabIndex = 1;
            this.button1_logIn.Text = "群发";
            this.button1_logIn.UseVisualStyleBackColor = true;
            this.button1_logIn.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2_logOut
            // 
            this.button2_logOut.Location = new System.Drawing.Point(201, 112);
            this.button2_logOut.Name = "button2_logOut";
            this.button2_logOut.Size = new System.Drawing.Size(75, 23);
            this.button2_logOut.TabIndex = 2;
            this.button2_logOut.Text = "关闭";
            this.button2_logOut.UseVisualStyleBackColor = true;
            this.button2_logOut.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 142);
            this.Controls.Add(this.button2_logOut);
            this.Controls.Add(this.button1_logIn);
            this.Controls.Add(this.richTextBox1);
            this.Name = "Form3";
            this.Text = "Form3";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button1_logIn;
        private System.Windows.Forms.Button button2_logOut;
    }
}