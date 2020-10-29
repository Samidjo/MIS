﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MIS.Application.Commands;
using MIS.Application.Queries;
using MIS.Application.ViewModels;
using MIS.Domain.Services;
using MIS.Infomat.PrintForms;
using MIS.Infomat.Windows;
using Serilog;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MIS.Infomat.Controls
{
    /// <summary>
    /// Логика взаимодействия для TimeItemsControl.xaml
    /// </summary>
    public partial class TimeItemsControl : UserControl
    {
        private readonly PatientViewModel _patient;
        private readonly ResourceViewModel _resource;

        private readonly MainWindow _mainWindow;

        private readonly IMediator _mediator;
        private readonly IPrintService _printService;

        internal TimeItemsControl()
        {
            throw new ArgumentNullException($"Fields '{nameof(_patient)}', '{nameof(_resource)}' can't be empty!");
        }

        internal TimeItemsControl(PatientViewModel patient, ResourceViewModel resource)
        {
            _patient = patient;
            _resource = resource;

            var app = System.Windows.Application.Current as App;

            _mediator = app.ServiceProvider.GetService<IMediator>();
            _printService = app.ServiceProvider.GetService<IPrintService>();

            _mainWindow = app.MainWindow as MainWindow;

            InitializeComponent();
        }

        private void UserControl_Loaded(Object sender, RoutedEventArgs e)
        {
            datesHeader.Content = _mediator.Send(
                new DateHeaderQuery()
            ).Result;

            datesList.ItemsSource = _mediator.Send(
                new DateListItemsQuery(_resource)
            ).Result;
        }

        private void DateListItemButton_Click(Object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button button && button.DataContext is DateItemViewModel dateItem)
            {
                if (dateItem.Times == null)
                {
                    dateItem.Times = _mediator.Send(
                        new TimeListItemsQuery(dateItem.Date, _resource.ResourceID)
                    ).Result;

                    dateItem.IsEnabled = dateItem.Times.Any(ti => ti.IsEnabled);
                }

                if (dateItem.IsEnabled)
                {
                    timeHeader.Content = $"{dateItem.Date:dd MMMM yyyy г.}";
                    timesList.ItemsSource = dateItem.Times;
                }
                else
                {
                    button.IsEnabled = false;
                    if (button.Content is TextBlock textBlock)
                    {
                        textBlock.Foreground = Brushes.DarkGray;
                    }
                }
            }
        }

        private void TimeListItemButton_Click(Object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button button && button.DataContext is TimeItemViewModel timeItem)
            {
                try
                {
                    VisitItemViewModel visitItem = _mediator.Send(
                        new VisitCreateCommand(timeItem.TimeItemID, _patient.ID, _patient.Code, _patient.DisplayName)
                    ).Result;

                    _patient.VisitItems.Add(visitItem);

                    _printService.Print(
                        new VisitPrintForm(visitItem)
                    );

                    _mainWindow.PrevWorkflow<ActionsControl>();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "При записи на приём произошла ошибка");

                    button.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void PrevButton_Click(Object sender, RoutedEventArgs e)
        {
            _mainWindow.PrevWorkflow();
        }
    }
}