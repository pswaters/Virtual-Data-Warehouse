﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using TEAM;

namespace Virtual_Data_Warehouse
{
    public partial class FormAbout : FormBase
    {
        //private readonly FormMain _myParent;

        public FormAbout(FormMain parent)
        {
            MyParent = parent;
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var conn = new SqlConnection {ConnectionString = TeamConfigurationSettings.MetadataConnection.CreateSqlServerConnectionString(false)};

            try
            {
                conn.Open();

                var sqlForWorkCountDown = new StringBuilder();

                sqlForWorkCountDown.AppendLine("SELECT ");
                sqlForWorkCountDown.AppendLine("	MINUTES_TO_WORK_THIS_WEEK, (MINUTES_TO_WORK_THIS_WEEK/60) as HOURS_TO_WORK_THIS_WEEK");
                sqlForWorkCountDown.AppendLine("FROM");
                sqlForWorkCountDown.AppendLine("( ");
                sqlForWorkCountDown.AppendLine("SELECT");
                sqlForWorkCountDown.AppendLine(" CASE");
                sqlForWorkCountDown.AppendLine("	WHEN");
                sqlForWorkCountDown.AppendLine("		DATEDIFF(");
                sqlForWorkCountDown.AppendLine("		   mi,");
                sqlForWorkCountDown.AppendLine("		   GETDATE(),");
                sqlForWorkCountDown.AppendLine("		   dateadd(hour,17,cast(CAST(getdate() as DATE) as datetime)))<=0");
                sqlForWorkCountDown.AppendLine("	THEN 0");
                sqlForWorkCountDown.AppendLine("	ELSE");
                sqlForWorkCountDown.AppendLine("		DATEDIFF(");
                sqlForWorkCountDown.AppendLine("		   mi,");
                sqlForWorkCountDown.AppendLine("		   GETDATE(),");
                sqlForWorkCountDown.AppendLine("		   dateadd(hour,17,cast(CAST(getdate() as DATE) as datetime))) ");
                sqlForWorkCountDown.AppendLine("	END");
                sqlForWorkCountDown.AppendLine("	+");
                sqlForWorkCountDown.AppendLine("	CASE");
                sqlForWorkCountDown.AppendLine("       WHEN 6-datepart(WEEKDAY,GETDATE())<0");
                sqlForWorkCountDown.AppendLine("		THEN 0");
                sqlForWorkCountDown.AppendLine("		ELSE 6-datepart(WEEKDAY,GETDATE())");
                sqlForWorkCountDown.AppendLine("	END*480");
                sqlForWorkCountDown.AppendLine("	as MINUTES_TO_WORK_THIS_WEEK");
                sqlForWorkCountDown.AppendLine(") as sub");

                //  MessageBox.Show(sqlForWorkCountdDown.ToString());
                var workCountDownDatatable = Utility.GetDataTable(ref conn, sqlForWorkCountDown.ToString());

                //var workCountDownDatatable = _myParent.Invoke((MethodInvoker)delegate() { _myParent.GetDataTable(ref connHstg, sqlForWorkCountdDown.ToString()); });

                int minutesToWork = 0;
                int hoursToWork = 0;

                foreach (DataRow row in workCountDownDatatable.Rows)
                {
                    minutesToWork = (int) row["MINUTES_TO_WORK_THIS_WEEK"];
                    hoursToWork = (int) row["HOURS_TO_WORK_THIS_WEEK"];
                }

                MessageBox.Show(
                    "There are only " + minutesToWork + " minutes (" + hoursToWork + " hours) left to work this week!",
                    "Important announcement",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception)
            {
                MessageBox.Show("There is no database connection for the PSA database! Please check the details in the information pane.","An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
   
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Specify that the link was visited. 
            linkLabel1.LinkVisited = true;
            // Navigate to a URL.
            Process.Start("http://www.roelantvos.com");
        }



    }
}
