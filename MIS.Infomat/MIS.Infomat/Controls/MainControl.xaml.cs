﻿using MIS.Infomat.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace MIS.Infomat.Controls
{
    /// <summary>
    /// Логика взаимодействия для MainControl.xaml
    /// </summary>
    public partial class MainControl : UserControl
    {
        private readonly MainWindow _mainWindow;

        internal MainControl()
        {
            var app = System.Windows.Application.Current as App;

            _mainWindow = app.MainWindow as MainWindow;

            InitializeComponent();
        }

        private void UserControl_MouseUp(Object sender, MouseButtonEventArgs e)
        {
            if (!_mainWindow.IsServiceTime)
            {
                _mainWindow.NextWorkflow(new PatientControl(), isRemember: false);
            }
        }
    }
}