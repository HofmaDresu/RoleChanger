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
        private readonly string _connectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["TargetDatabase"].ConnectionString;

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
            var con = new SqlConnection(_connectionString);
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
            var con = new SqlConnection(_connectionString);
            con.Open();
            var roleReader = new SqlCommand("SELECT name FROM sys.database_principals where type in ('R')", con).ExecuteReader();

            while (roleReader.Read())
            {
                if (roleReader["name"].ToString() == "public")
                {
                    continue;
                }

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

            var con = new SqlConnection(_connectionString);
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

        private void Save_Click(object sender, EventArgs e)
        {
            var userName = UserList.SelectedItem.ToString();
            var activeRoles = new List<string>();

            foreach (var t in RolesBox.CheckedItems)
            {
                if (t.ToString() == "public")
                {
                    continue;
                }

                var con = new SqlConnection(_connectionString);
                con.Open();
                //TODO: Fix to use parameters
                new SqlCommand(string.Format("EXEC sp_addrolemember '{0}', '{1}'", t, userName), con).ExecuteNonQuery();
                activeRoles.Add(t.ToString());
                con.Close();
            }

            foreach (var t in RolesBox.Items)
            {
                if (t.ToString() == "public" || activeRoles.Contains(t.ToString()))
                {
                    continue;
                }

                var con = new SqlConnection(_connectionString);
                con.Open();
                //TODO: Fix to use parameters
                new SqlCommand(string.Format("EXEC sp_droprolemember '{0}', '{1}'", t, userName), con).ExecuteNonQuery();
                activeRoles.Add(t.ToString());
                con.Close();
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
