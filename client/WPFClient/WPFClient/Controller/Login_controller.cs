using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using WPFClient.Model;

namespace WPFClient.Controller
{
    internal class Login_controller
    {
        ////BUTTONS--------------------------------------------------------------------------------
        //user login with a http request
        public async void Button_Click_Login_controller(LoginWindow obj)
        {
            // Retrieve the username and password from the textboxes on the UI
            string username = obj.entered_name.Text;
            string password = obj.entered_password.Password;
            SHA256 sha256 = SHA256.Create();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);
            string passwordHash = Convert.ToBase64String(hashBytes);

            // Create an HttpClient instance to send the request
            using (var client = new HttpClient())
            {
                // Set the base address of your REST server
                Uri restServerAddress = new Uri("https://localhost:7243/");
                client.BaseAddress = restServerAddress;

                // Send a GET request to your server with the user's credentials
                var response = await client.GetAsync($"api/User/login?userName={username}&password={passwordHash}");

                // If the response is successful, show a login successful message
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<User_model>(responseContent);
                    if (loginResponse.RoleType == "Manager")
                    {
                        MessageBox.Show("Login successful! Welcome " + loginResponse.FirstName + " " + loginResponse.LastName + ", you logged in as a MANAGER");
                        Manager_view manager_view = new Manager_view();
                        obj.Close();
                        manager_view.Show();
                    }
                    else if (loginResponse.RoleType == "Technician")
                    {
                        MessageBox.Show("Login successful! Welcome " + loginResponse.FirstName + " " + loginResponse.LastName + ", you logged in as a TECHNICIAN");
                        Technician_view technician_view = new Technician_view();
                        obj.Close();
                        technician_view.Show();
                    }
                    else if (loginResponse.RoleType == "Storekeeper")
                    {
                        MessageBox.Show("Login successful! Welcome " + loginResponse.FirstName + " " + loginResponse.LastName + ", you logged in as a STOREKEEPER");
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

        //Show button on
        public void ShowPassword_Checked_controller(LoginWindow obj)
        {
            obj.entered_password.PasswordChar = '\0'; // null character, displays plain text
            obj.password_text.Visibility = Visibility.Visible;
            obj.password_text.Text = obj.entered_password.Password;
            obj.entered_password.Visibility = Visibility.Collapsed;
        }

        //show button off
        public void ShowPassword_Unchecked_controller(LoginWindow obj)
        {
            obj.entered_password.PasswordChar = '*'; // bullet character, hides password text
            obj.password_text.Visibility = Visibility.Collapsed;
            obj.entered_password.Visibility = Visibility.Visible;
        }
    }
}
