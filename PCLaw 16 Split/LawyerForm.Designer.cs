namespace PCLaw_16_Split
{
    partial class LawyerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LawyerForm));
            this.listViewLawyer = new System.Windows.Forms.ListView();
            this.buttonClearLawyers = new System.Windows.Forms.Button();
            this.buttonLawyerSelect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewLawyer
            // 
            this.listViewLawyer.Location = new System.Drawing.Point(56, 18);
            this.listViewLawyer.Name = "listViewLawyer";
            this.listViewLawyer.Size = new System.Drawing.Size(282, 223);
            this.listViewLawyer.TabIndex = 5;
            this.listViewLawyer.UseCompatibleStateImageBehavior = false;
            this.listViewLawyer.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewLawyer_ColumnClick);
            // 
            // buttonClearLawyers
            // 
            this.buttonClearLawyers.Location = new System.Drawing.Point(133, 261);
            this.buttonClearLawyers.Name = "buttonClearLawyers";
            this.buttonClearLawyers.Size = new System.Drawing.Size(111, 23);
            this.buttonClearLawyers.TabIndex = 4;
            this.buttonClearLawyers.Text = "Clear Selection";
            this.buttonClearLawyers.UseVisualStyleBackColor = true;
            this.buttonClearLawyers.Click += new System.EventHandler(this.buttonClearLawyers_Click);
            // 
            // buttonLawyerSelect
            // 
            this.buttonLawyerSelect.Location = new System.Drawing.Point(153, 314);
            this.buttonLawyerSelect.Name = "buttonLawyerSelect";
            this.buttonLawyerSelect.Size = new System.Drawing.Size(75, 23);
            this.buttonLawyerSelect.TabIndex = 3;
            this.buttonLawyerSelect.Text = "Begin";
            this.buttonLawyerSelect.UseVisualStyleBackColor = true;
            this.buttonLawyerSelect.Click += new System.EventHandler(this.buttonLawyerSelect_Click);
            // 
            // LawyerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 354);
            this.Controls.Add(this.listViewLawyer);
            this.Controls.Add(this.buttonClearLawyers);
            this.Controls.Add(this.buttonLawyerSelect);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LawyerForm";
            this.Text = "Lawyers to Keep";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewLawyer;
        private System.Windows.Forms.Button buttonClearLawyers;
        private System.Windows.Forms.Button buttonLawyerSelect;
    }
}

