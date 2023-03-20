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
using WPFClient.Utilities;
using WPFClient.Model;
using WPFClient.View;
using static WPFClient.Controller.Login_controller;

namespace WPFClient.Controller
{
    internal class Technician_controller
    {
        public async Task Button_Click_Create_orderer_controller(Techinican_create_orderer obj)
        {
            string orderer_name = obj.New_orderer_textbox.Text;
            string description = obj.Description_textbox.Text;


            //both textbox are filled
            if (orderer_name != "" && description != "")
            {
                using (var client = RestHelper.GetRestClient())
                {
                    var newOrderer = new
                    {
                        ordererName = orderer_name,
                        description = description,
                        userId = userid,
                        projectId = 0
                    };
                    var json = JsonConvert.SerializeObject(newOrderer);
                    var response = await client.PostAsync("api/Orderer", new StringContent(json, Encoding.UTF8, "application/json"));
                    string status = response.StatusCode.ToString();
                    if (response.IsSuccessStatusCode)
                    {
                        // Orderer created successfully
                        MessageBox.Show("Orderer created successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Orderer creation failed!");

                    }
                }
            }
            else
            {
                MessageBox.Show("Fill both textbox!");
            }
        }
    }
}
