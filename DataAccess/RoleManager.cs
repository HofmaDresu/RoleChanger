using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class RoleManager
    {
        private readonly string _connectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["TargetDatabase"].ConnectionString;


        public List<string> GetUserList()
        {
            var userList = new List<string>();
            var con = new SqlConnection(_connectionString);
            con.Open();
            var userReader =
                new SqlCommand("SELECT name FROM sys.database_principals where type in ('U', 'S') order by name", con).ExecuteReader();

            while (userReader.Read())
            {
                userList.Add(userReader["name"].ToString());
            }
            con.Close();
            return userList;
        }

        public List<string> GetRoleList()
        {
            var roleList = new List<string>();
            var con = new SqlConnection(_connectionString);
            con.Open();
            var roleReader = new SqlCommand("SELECT name FROM sys.database_principals where type in ('R') order by name", con).ExecuteReader();

            while (roleReader.Read())
            {
                if (roleReader["name"].ToString() == "public")
                {
                    continue;
                }

                roleList.Add(roleReader["name"].ToString());
            }
            con.Close();
            return roleList;
        }

        public List<string> GetUserRoles(object userName)
        {
            var roleList = new List<string>();
            var con = new SqlConnection(_connectionString);
            con.Open();
            //TODO: Fix to use parameters
            var roleReader = new SqlCommand(string.Format("EXEC sp_helpuser '{0}'", userName), con).ExecuteReader();

            while (roleReader.Read())
            {
                roleList.Add(roleReader["RoleName"].ToString());
            }
            con.Close();
            return roleList;
        }

        public void RunRoleSpForUser(string roleSp, object t, string userName)
        {
            var con = new SqlConnection(_connectionString);
            con.Open();
            //TODO: Fix to use parameters
            new SqlCommand(string.Format("EXEC {0} '{1}', '{2}'", roleSp, t, userName), con).ExecuteNonQuery();
            con.Close();
        }
    }
}
