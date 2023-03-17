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

        
        ////BUTTONS-------------------------------------------------------------------------------------------
        //Login button
        private void Button_Click_Login(object sender, RoutedEventArgs e)
        {
            Login_controller classObj = new Login_controller();
            classObj.Button_Click_Login_controller(this);
        }

        //Show button on
        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            Login_controller classObj = new Login_controller();
            classObj.ShowPassword_Checked_controller(this);
        }

        //Show button off
        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            Login_controller classObj = new Login_controller();
            classObj.ShowPassword_Unchecked_controller(this);
        }
    }
}
