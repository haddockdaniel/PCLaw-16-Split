namespace PCLaw_16_Split
{
    partial class InitialForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InitialForm));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.textBoxDB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxWIPFees = new System.Windows.Forms.CheckBox();
            this.checkBoxWIPDisb = new System.Windows.Forms.CheckBox();
            this.checkBoxAR = new System.Windows.Forms.CheckBox();
            this.checkBoxTrust = new System.Windows.Forms.CheckBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SQL Server";
            // 
            // textBoxServer
            // 
            this.textBoxServer.Location = new System.Drawing.Point(105, 39);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(215, 20);
            this.textBoxServer.TabIndex = 1;
            this.textBoxServer.Text = "localhost";
            // 
            // textBoxDB
            // 
            this.textBoxDB.Location = new System.Drawing.Point(105, 77);
            this.textBoxDB.Name = "textBoxDB";
            this.textBoxDB.Size = new System.Drawing.Size(215, 20);
            this.textBoxDB.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "SQL Database";
            // 
            // checkBoxWIPFees
            // 
            this.checkBoxWIPFees.AutoSize = true;
            this.checkBoxWIPFees.Location = new System.Drawing.Point(31, 122);
            this.checkBoxWIPFees.Name = "checkBoxWIPFees";
            this.checkBoxWIPFees.Size = new System.Drawing.Size(111, 17);
            this.checkBoxWIPFees.TabIndex = 4;
            this.checkBoxWIPFees.Text = "Include WIP Fees";
            this.checkBoxWIPFees.UseVisualStyleBackColor = true;
            this.checkBoxWIPFees.CheckedChanged += new System.EventHandler(this.checkBoxWIPFees_CheckedChanged);
            // 
            // checkBoxWIPDisb
            // 
            this.checkBoxWIPDisb.AutoSize = true;
            this.checkBoxWIPDisb.Location = new System.Drawing.Point(31, 145);
            this.checkBoxWIPDisb.Name = "checkBoxWIPDisb";
            this.checkBoxWIPDisb.Size = new System.Drawing.Size(109, 17);
            this.checkBoxWIPDisb.TabIndex = 5;
            this.checkBoxWIPDisb.Text = "Include WIP Disb";
            this.checkBoxWIPDisb.UseVisualStyleBackColor = true;
            this.checkBoxWIPDisb.CheckedChanged += new System.EventHandler(this.checkBoxWIPDisb_CheckedChanged);
            // 
            // checkBoxAR
            // 
            this.checkBoxAR.AutoSize = true;
            this.checkBoxAR.Location = new System.Drawing.Point(31, 168);
            this.checkBoxAR.Name = "checkBoxAR";
            this.checkBoxAR.Size = new System.Drawing.Size(79, 17);
            this.checkBoxAR.TabIndex = 6;
            this.checkBoxAR.Text = "Include AR";
            this.checkBoxAR.UseVisualStyleBackColor = true;
            this.checkBoxAR.CheckedChanged += new System.EventHandler(this.checkBoxAR_CheckedChanged);
            // 
            // checkBoxTrust
            // 
            this.checkBoxTrust.AutoSize = true;
            this.checkBoxTrust.Location = new System.Drawing.Point(31, 191);
            this.checkBoxTrust.Name = "checkBoxTrust";
            this.checkBoxTrust.Size = new System.Drawing.Size(88, 17);
            this.checkBoxTrust.TabIndex = 7;
            this.checkBoxTrust.Text = "Include Trust";
            this.checkBoxTrust.UseVisualStyleBackColor = true;
            this.checkBoxTrust.CheckedChanged += new System.EventHandler(this.checkBoxTrust_CheckedChanged);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(140, 237);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 8;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // InitialForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 282);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.checkBoxTrust);
            this.Controls.Add(this.checkBoxAR);
            this.Controls.Add(this.checkBoxWIPDisb);
            this.Controls.Add(this.checkBoxWIPFees);
            this.Controls.Add(this.textBoxDB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxServer);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InitialForm";
            this.Text = "Options Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxServer;
        private System.Windows.Forms.TextBox textBoxDB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxWIPFees;
        private System.Windows.Forms.CheckBox checkBoxWIPDisb;
        private System.Windows.Forms.CheckBox checkBoxAR;
        private System.Windows.Forms.CheckBox checkBoxTrust;
        private System.Windows.Forms.Button buttonStart;
    }
}