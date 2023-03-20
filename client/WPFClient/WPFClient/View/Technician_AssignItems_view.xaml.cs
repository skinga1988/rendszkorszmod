﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WPFClient.View
{
    /// <summary>
    /// Interaction logic for Technician_AssignItems_view.xaml
    /// </summary>
    public partial class Technician_AssignItems_view : Window
    {
        public ObservableCollection<StockItem_model> Products { get; set; }
        public int SelectedId { get; set; }
        public Technician_AssignItems_view()
        {
            InitializeComponent();
            DataContext = this;  
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Technician_controller controller = new Technician_controller();
            await controller.LoadAssignItemsData(this);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Technician_view view = new Technician_view();
            view.Show();
            Close();
        }

        private async void assignButton_Click(object sender, RoutedEventArgs e)
        {
            Technician_controller controller = new Technician_controller();
            await controller.AssignItems(this);
            Technician_view view = new Technician_view();
            view.Show();
            Close();
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void productComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Technician_controller technician_Controller = new Technician_controller();
            await technician_Controller.GetAvailableCount(this);
        }
    }
}
