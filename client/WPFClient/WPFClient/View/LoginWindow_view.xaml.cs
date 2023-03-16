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
using WPFClient.Controller;

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

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Login_controller classObj = new Login_controller();
            classObj.Button_Click_controller(this);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Login_controller classObj = new Login_controller();
            classObj.TextBox_TextChanged_controller(this);
        }


        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            Login_controller classObj = new Login_controller();
            classObj.ShowPassword_Checked_controller(this);
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            Login_controller classObj = new Login_controller();
            classObj.ShowPassword_Unchecked_controller(this);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Login_controller classObj = new Login_controller();
            classObj.PasswordBox_PasswordChanged_controller(this);
        }
    }
}
