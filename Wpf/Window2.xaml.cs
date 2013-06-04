﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wpf
{
    /// <summary>
    /// Логика взаимодействия для Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
        }

        public int rows, columns;
        public bool yesButton;

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (textBox1.Text.Trim() != "" && textBox2.Text.Trim() != "")
            {
                try
                {
                    rows = Convert.ToInt32(textBox2.Text);
                    columns = Convert.ToInt32(textBox1.Text);
                }
                catch
                {
                    MessageBox.Show("Введите количество строк или столбцов цифрами");
                }
                yesButton = true;
                this.Close();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            yesButton = false;
            this.Close();
        }
    }
}
