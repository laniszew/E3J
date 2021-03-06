﻿using System.Windows.Controls;
using E3J.Utilities;
using E3J.ViewModels;
using PropertyChanged;

namespace E3J.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    [DoNotNotify]
    public partial class MainView : Page
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            MissingFileManager.CheckForRequiredFiles();
            Session.Instance.Initialize();
        }
    }
}