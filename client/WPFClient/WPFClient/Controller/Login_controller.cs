﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using WPFClient.Utilities;

namespace WPFClient.Controller
{
    internal class Login_controller
    {
        public async void Button_Click_controller(LoginWindow obj)
        {
            // Retrieve the username and password from the textboxes on the UI
            string username = obj.entered_name.Text;
            string password = obj.entered_password.Password;
            SHA256 sha256 = SHA256.Create();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);
            string passwordHash = Convert.ToBase64String(hashBytes);

            // Create an HttpClient instance to send the request
            using (var client = RestHelper.GetRestClient())
            {
                // Send a GET request to your server with the user's credentials
                var response = await client.GetAsync($"api/User/login?userName={username}&password={passwordHash}");

                // If the response is successful, show a login successful message
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                    if (loginResponse.roleType == "Manager")
                    {
                        MessageBox.Show("Login successful! Welcome " + loginResponse.firstName + " " + loginResponse.lastName + ", you logged in as a MANAGER");
                        Manager_view manager_view = new Manager_view();
                        obj.Close();
                        manager_view.Show();
                    }
                    else if (loginResponse.roleType == "Technician")
                    {
                        MessageBox.Show("Login successful! Welcome " + loginResponse.firstName + " " + loginResponse.lastName + ", you logged in as a TECHNICIAN");
                        Technician_view technician_view = new Technician_view();
                        obj.Close();
                        technician_view.Show();
                    }
                    else if (loginResponse.roleType == "Storekeeper")
                    {
                        MessageBox.Show("Login successful! Welcome " + loginResponse.firstName + " " + loginResponse.lastName + ", you logged in as a STOREKEEPER");
                        Storekeeper_view storekeeper_view = new Storekeeper_view();
                        obj.Close();
                        storekeeper_view.Show();
                    }

                }
                else
                {
                    MessageBox.Show("Login failed. Please check your username and password.");
                }
            }
        }

        public void TextBox_TextChanged_controller(LoginWindow obj)
        {

        }

        // Define a class to represent the login response
        public class LoginResponse
        {
            public int userId { get; set; }
            public string roleType { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string userName { get; set; }
            public string createdDate { get; set; }
            public string password { get; set; }
        }

        public void ShowPassword_Checked_controller(LoginWindow obj)
        {
            obj.entered_password.PasswordChar = '\0'; // null character, displays plain text
            obj.password_text.Visibility = Visibility.Visible;
            obj.password_text.Text = obj.entered_password.Password;
            obj.entered_password.Visibility = Visibility.Collapsed;
        }

        public void ShowPassword_Unchecked_controller(LoginWindow obj)
        {
            obj.entered_password.PasswordChar = '*'; // bullet character, hides password text
            obj.password_text.Visibility = Visibility.Collapsed;
            obj.entered_password.Visibility = Visibility.Visible;
        }

        public void PasswordBox_PasswordChanged_controller(LoginWindow obj)
        {
            string password = obj.entered_password.Password;
            // do something with the entered password...
        }
    }
}
