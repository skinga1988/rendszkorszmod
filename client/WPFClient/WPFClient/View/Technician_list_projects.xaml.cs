﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFClient.Controller;
using WPFClient.Model;
using WPFClient.Utilities;

namespace WPFClient.View
{
    /// <summary>
    /// Interaction logic for Technician_list_projects.xaml
    /// </summary>
    public partial class Technician_list_projects : Window
    {


        public Technician_list_projects()
        {
            InitializeComponent();

        }


        

        ////BUTTONS----------------------------------------------------------------------------------------------------
        //Back button: back to the technician main menu
        private void Button_Click_Back4(object sender, RoutedEventArgs e)
        {
            Technician_view window = new Technician_view();
            window.Show();
            this.Close();
        }

        private void Button_ContextMenuClosing_Back4(object sender, ContextMenuEventArgs e)
        {
            this.Close();
        }

        //listing projects


        private void ProjectsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void Button_Click_list(object sender, RoutedEventArgs e)
        {
            //await RefreshdataAsync();
            Technician_controller controller = new Technician_controller();
            await controller.GetProjectList(this);
        }


    }
}