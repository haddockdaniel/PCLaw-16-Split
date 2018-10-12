using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace PCLaw_16_Split
{
    public partial class LawyerForm : Form
    {
        public LawyerForm(string server, string db, bool wipfees, bool wipdisb, bool ar, bool trust)
        {
            InitializeComponent();

            convertAR = ar;
            convertTrust = trust;
            convertWIPdisb = wipdisb;
            convertWIPfees = wipfees;


            lawyerSorter = new ListViewColumnSorter();
            listViewLawyer.View = View.Details;
            listViewLawyer.ListViewItemSorter = lawyerSorter;
            listViewLawyer.Columns.Add("Name", 180);
            listViewLawyer.Columns.Add("ID", 35);


            string queryString = "SELECT LawyerID, LawInfLawyerName FROM LawInf where LawInfStatus = 0";
            connectionString = "Data Source=" + server + ";Initial Catalog=" + db + "; Integrated Security=SSPI;";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        listViewLawyer.Items.Add(new ListViewItem(new string[] { reader["LawInfLawyerName"].ToString().Trim(), reader["LawyerID"].ToString().Trim() }));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Getting Lawyers. Message: " + ex.Message);
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }




        }


        public ListViewColumnSorter lawyerSorter;
        public List<string> lawyerList = new List<string>();
        bool convertAR;
        bool convertTrust;
        bool convertWIPdisb;
        bool convertWIPfees;
        string connectionString = "";

        private void listViewLawyer_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == lawyerSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lawyerSorter.Order == System.Windows.Forms.SortOrder.Ascending)
                    lawyerSorter.Order = System.Windows.Forms.SortOrder.Descending;
                else
                    lawyerSorter.Order = System.Windows.Forms.SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lawyerSorter.SortColumn = e.Column;
                lawyerSorter.Order = System.Windows.Forms.SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listViewLawyer.Sort();
        }

        private void buttonClearLawyers_Click(object sender, EventArgs e)
        {
            listViewLawyer.SelectedItems.Clear();
        }

        private void buttonLawyerSelect_Click(object sender, EventArgs e)
        {
            int index = -1;
            ListView.SelectedIndexCollection indexes = this.listViewLawyer.SelectedIndices;
            foreach (int ind in indexes)
            {
                index = int.Parse(this.listViewLawyer.Items[ind].SubItems[1].Text);
                lawyerList.Add(index.ToString());
            }//end outer foreach

            this.Hide();
            var progress = new ProgressForm(connectionString, convertWIPfees, convertWIPdisb, convertAR, convertTrust, lawyerList);
            progress.Closed += (s, args) => this.Close();
            progress.Show();







        }
    }
}
