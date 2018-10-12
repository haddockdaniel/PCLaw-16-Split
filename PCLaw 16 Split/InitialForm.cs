using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCLaw_16_Split
{
    public partial class InitialForm : Form
    {
        public InitialForm()
        {
            InitializeComponent();
        }

        bool convertAR = false;
        bool convertTrust = false;
        bool convertWIPdisb = false;
        bool convertWIPfees = false;

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.Hide();
            var lawyerform = new LawyerForm(textBoxServer.Text, textBoxDB.Text, convertWIPfees, convertWIPdisb, convertAR, convertTrust);
            lawyerform.Closed += (s, args) => this.Close();
            lawyerform.Show();
        }

        private void checkBoxWIPFees_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxWIPFees.Checked == true)
                convertWIPfees = true;
            else
                convertWIPfees = false;
        }

        private void checkBoxWIPDisb_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxWIPDisb.Checked == true)
                convertWIPdisb = true;
            else
                convertWIPdisb = false;
        }

        private void checkBoxAR_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAR.Checked == true)
                convertAR = true;
            else
                convertAR = false;
        }

        private void checkBoxTrust_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxTrust.Checked == true)
                convertTrust = true;
            else
                convertTrust = false;
        }
    }
}
