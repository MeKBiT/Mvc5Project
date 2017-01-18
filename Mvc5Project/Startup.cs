using Microsoft.Owin;
using Mvc5Project.Controllers;
using Owin;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;

[assembly: OwinStartupAttribute(typeof(Mvc5Project.Startup))]
namespace Mvc5Project
{
    public partial class Startup
    {

        private Timer threadingTimer;
        string connection = AccountController.GetConnectionString("DefaultConnection");
        string command = null;
        string parameterName = null;
        string methodName = null;

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //StartTimer();
        }
        private void DeleteUserFromDatabase(string userid)
        {
            List<CommAndParams> commAndParam = new List<CommAndParams>();
            commAndParam.Add(new CommAndParams() { command = "DELETE FROM AspNetUserLogins WHERE UserId = @UserId", parameterName = "@UserId" });
            commAndParam.Add(new CommAndParams() { command = "DELETE FROM AspNetUserClaims WHERE UserId = @UserId", parameterName = "@UserId" });
            commAndParam.Add(new CommAndParams() { command = "DELETE FROM AspNetUserRoles WHERE UserId = @UserId", parameterName = "@UserId" });
            commAndParam.Add(new CommAndParams() { command = "DELETE FROM AspNetUsers WHERE Id = @Id", parameterName = "@Id" });
            foreach (var cap in commAndParam)
            {
                command = cap.command;
                parameterName = cap.parameterName;
                using (SqlConnection myConnection = new SqlConnection(connection))
                using (SqlCommand cmd = new SqlCommand(command, myConnection))
                {
                    cmd.Parameters.AddWithValue(parameterName, userid);
                    myConnection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            cmd.ExecuteNonQuery();
                            myConnection.Close();
                        }
                    }
                }
            }
        }

        private void DeleteUser()
        {
            using (SqlConnection myConnection = new SqlConnection(connection))
            using (SqlCommand cmd = new SqlCommand(command, myConnection))
            {
                cmd.Parameters.AddWithValue(parameterName, false);
                myConnection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        // Read advances to the next row.
                        if (reader.Read())
                        {
                            if (methodName == "DeleteUncorfirmedAccounts")
                            {
                                DateTime emailLinkDate = (DateTime)reader["EmailLinkDate"];
                                if (emailLinkDate.AddMinutes(1) < DateTime.Now)
                                {
                                    DeleteById();
                                }
                            }
                            else if (methodName == "DeleteById")
                            {
                                string userid = reader["Id"].ToString();
                                DeleteUserFromDatabase(userid);
                            }
                        }
                        myConnection.Close();
                    }
                }
            }
        }

        private void DeleteById()
        {
            command = "SELECT Id AS Id FROM AspNetUsers WHERE EmailConfirmed = @EmailConfirmed";
            parameterName = "@EmailConfirmed";
            methodName = "DeleteById";
            DeleteUser();
        }
        private void DeleteUncorfirmedAccounts(object sender)
        {
            command = "SELECT EmailLinkDate AS EmailLinkDate FROM AspNetUsers WHERE EmailConfirmed = @EmailConfirmed";
            parameterName = "@EmailConfirmed";
            methodName = "DeleteUncorfirmedAccounts";
            DeleteUser();
        }

        private void StartTimer()
        {
            if (threadingTimer == null)
            {
                //raise timer callback 
                string str = DateTime.Now.ToLongTimeString(); threadingTimer = new Timer(new TimerCallback(DeleteUncorfirmedAccounts), str, 1000, 1000);
            }
        }

    }
    public class CommAndParams
    {
        public string command { get; set; }
        public string parameterName { get; set; }
    }

}
