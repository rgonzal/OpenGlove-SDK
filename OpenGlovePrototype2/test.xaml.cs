﻿using OpenGloveSDKBackend;
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

namespace OpenGlovePrototype2
{
    /// <summary>
    /// Interaction logic for welcome.xaml
    /// </summary>
    public partial class welcome : Window
    {
        private OpenGloveSDKCore sdkCore;

        public welcome()
        {
            InitializeComponent();
            sdkCore = OpenGloveSDKCore.getCore();
        }

        private void buttonRefreshPorts_Click(object sender, RoutedEventArgs e)
        {
            this.listViewPorts.Items.Clear();
            sdkCore.GetOpenGlove();
            string[] ports = sdkCore.GetOpenGlove().GetPortNames();

            foreach (var port in ports)
            {
                this.listViewPorts.Items.Add(port);
            }
            this.buttonActivate.IsEnabled = true;
        }

        
        private void buttonActivate_Click(object sender, RoutedEventArgs e)
        {
            string port = (string)listViewPorts.SelectedItem;
            //Establecer comunicacion
            sdkCore.Connect(port);
            this.buttonActivate.IsEnabled = false;
            this.buttonStop.IsEnabled = true;
            this.buttonVibrate.IsEnabled = true;
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            sdkCore.Disconnect();
            this.buttonActivate.IsEnabled = true;
            this.buttonStop.IsEnabled = false;
            this.buttonVibrate.IsEnabled = false;
        }

        private void buttonVibrate_Click(object sender, RoutedEventArgs e)
        {
            //Activar motores
            sdkCore.StartTest();
            this.buttonVibrate.IsEnabled = false;
            this.buttonStopVibrate.IsEnabled = true;
        }

        private void buttonStopVibrate_Click(object sender, RoutedEventArgs e)
        {
            //Desactivar motores
            sdkCore.StopTest();
            this.buttonVibrate.IsEnabled = true;
            this.buttonStopVibrate.IsEnabled = false;
        }
    }
}