using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the username and password from the textboxes on the UI
            string username = entered_name.Text;
            string password = entered_password.Password;
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
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                    if (loginResponse.roleType == "Manager")
                    {
                        MessageBox.Show("Login successful! Welcome " + loginResponse.firstName + " " + loginResponse.lastName + ", you logged in as a MANAGER");
                        Manager_view manager_view = new Manager_view();
                        this.Close();
                        manager_view.Show();
                    }
                    else if (loginResponse.roleType == "Technician")
                    {
                        MessageBox.Show("Login successful! Welcome " + loginResponse.firstName + " " + loginResponse.lastName + ", you logged in as a TECHNICIAN");
                        Technician_view technician_view = new Technician_view();
                        this.Close();
                        technician_view.Show();
                    }
                    else if (loginResponse.roleType == "Storekeeper")
                    {
                        MessageBox.Show("Login successful! Welcome " + loginResponse.firstName + " " + loginResponse.lastName + ", you logged in as a STOREKEEPER");
                        Storekeeper_view storekeeper_view = new Storekeeper_view();
                        this.Close();
                        storekeeper_view.Show();
                    }
                    
                }
                else
                {
                    MessageBox.Show("Login failed. Please check your username and password.");
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
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

        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            entered_password.PasswordChar = '\0'; // null character, displays plain text
            password_text.Visibility = Visibility.Visible;
            password_text.Text = entered_password.Password;
            entered_password.Visibility = Visibility.Collapsed;
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            entered_password.PasswordChar = '*'; // bullet character, hides password text
            password_text.Visibility = Visibility.Collapsed;
            entered_password.Visibility = Visibility.Visible;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = entered_password.Password;
            // do something with the entered password...
        }
    }
}
