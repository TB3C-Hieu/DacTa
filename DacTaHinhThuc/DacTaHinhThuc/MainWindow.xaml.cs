using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Drawing;
using System.Windows.Media;
using System;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace DacTaHinhThuc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        FStoCSharp csharp = new FStoCSharp();
        InputHandler InputHandler = new InputHandler();
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"CSScript.txt");
        string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"CPScript.txt");
        enum state { cp, csharp };
        state current = state.csharp;
        File file_handler = new File();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            InputArea.Text = "";
            InputArea.Text = file_handler.ReadFile(file_handler.OpenFile());
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            file_handler.SaveFile(OutputArea.Text);
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            InputArea.Text = OutputArea.Text = "";
        }
        private void CSClick(object sender, RoutedEventArgs e)
        {
            current = state.csharp;
            OutputArea.Text = csharp.toCSharp(path, InputArea.Text);
        }
        private void CPClick(object sender, RoutedEventArgs e)
        {
            current = state.cp;
            OutputArea.Text = csharp.toCSharp(path2, InputArea.Text);
        }
        private void BuildBtn_Click(object sender, RoutedEventArgs e)
        {
            string text = InputArea.Text;
            string text_out = csharp.toCSharp(path, InputArea.Text);
            InputHandler.Run(ExitFileNameTextBox.Text,text_out);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
