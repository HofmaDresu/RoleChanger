using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoleChanger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetupData();
        }

        private void SetupData()
        {
            PopulateRoleList();
            PopulateUserList();
        }

        private void PopulateUserList()
        {
            var userList = new List<string>();
            var con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["TargetDatabase"].ConnectionString);
            con.Open();
            var userReader = new SqlCommand("SELECT name FROM sys.database_principals where type in ('U', 'S')", con).ExecuteReader();

            while (userReader.Read())
            {
                userList.Add(userReader["name"].ToString());
            }
            con.Close();

            UserList.DataSource = userList;
        }

        private void PopulateRoleList()
        {
            var roleList = new List<string>();
            var con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["TargetDatabase"].ConnectionString);
            con.Open();
            var roleReader = new SqlCommand("SELECT name FROM sys.database_principals where type in ('R')", con).ExecuteReader();

            while (roleReader.Read())
            {
                roleList.Add(roleReader["name"].ToString());
            }
            con.Close();

            RolesBox.DataSource = roleList;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i < RolesBox.Items.Count; i++)
            {
                RolesBox.SetItemChecked(i, false);
            }

            var userName = UserList.Items[((ComboBox)sender).SelectedIndex];

            var con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["TargetDatabase"].ConnectionString);
            con.Open();
            //TODO: Fix to use parameters
            var roleReader = new SqlCommand(string.Format("EXEC sp_helpuser '{0}'", userName), con).ExecuteReader();

            while (roleReader.Read())
            {
                for (var i = 0; i < RolesBox.Items.Count; i++ )
                {
                    if (RolesBox.Items[i].ToString() == roleReader["RoleName"].ToString()) RolesBox.SetItemChecked(i, true);
                }
            }

            con.Close();
        }
    }
}
