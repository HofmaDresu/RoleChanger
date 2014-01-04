using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAccess;

namespace RoleChanger
{
    public partial class Form1 : Form
    {
        private readonly RoleManager roleManager = new RoleManager();

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
            var userList = roleManager.GetUserList();

            UserList.DataSource = userList;
        }

        private void PopulateRoleList()
        {
            var roleList = roleManager.GetRoleList();

            RolesBox.DataSource = roleList;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i < RolesBox.Items.Count; i++)
            {
                RolesBox.SetItemChecked(i, false);
            }

            var userName = UserList.Items[((ComboBox)sender).SelectedIndex];

            var roleList = roleManager.GetUserRoles(userName);

            for (var i = 0; i < RolesBox.Items.Count; i++ )
            {
                if (roleList.Contains(RolesBox.Items[i].ToString())) RolesBox.SetItemChecked(i, true);
            }

        }

        private void Save_Click(object sender, EventArgs e)
        {
            var userName = UserList.SelectedItem.ToString();
            const string removeRoleSP = "sp_droprolemember";
            const string addRoleSP = "sp_addrolemember";

            AlterUserRoles(removeRoleSP, userName, RolesBox.Items);
            AlterUserRoles(addRoleSP, userName, RolesBox.CheckedItems);
        }

        private void AlterUserRoles(string roleSp, string userName, IEnumerable checkBoxItems)
        {
            foreach (var t in checkBoxItems)
            {
                if (t.ToString() == "public")
                {
                    continue;
                }

                roleManager.RunRoleSpForUser(roleSp, t, userName);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
