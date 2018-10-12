using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace PCLaw_16_Split
{
    public static class SplitFunctions
    {

        public static string RunSQLCommand(string sQuery, string conString)
        {
            int iResult = 0;
            string sError = "";

            try
            {

                SqlConnection Conn = new SqlConnection(conString);
                Conn.Open();
                SqlCommand Cmd = new SqlCommand(sQuery, Conn);
                iResult = Cmd.ExecuteNonQuery();


                //Clean up
                Conn.Dispose();
                Conn = null;

            }
            catch (SqlException objError)
            {
                sError = objError.Message;
                // System.Windows.Forms.MessageBox.Show(sError);
                // System.Diagnostics.Debug.Assert(false);

            }

            catch (Exception objError)
            {
                //write error to the windows event log                  
                //WriteToEventLog(objError);
                sError = objError.ToString();
                //PLXMLLnk_LinkLog_CloseLog   (); 
                //PLXMLLnk_LinkLog_Show       (); 
                // System.Windows.Forms.MessageBox.Show(sError);
                //System.Diagnostics.Debug.Assert(false);
            }
            return sError;
        }
    }
}
