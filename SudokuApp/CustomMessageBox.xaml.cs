using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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

namespace SudokuApp
{
    public partial class CustomMessageBox : Window
    {
        public MessageBoxResult Result { get; private set; }

        public CustomMessageBox(Window owner, string message, string title)
        {
            InitializeComponent();
            Owner = owner;
            Title = title;
            MessageText.Text = message;
            Result = MessageBoxResult.None;
        }


        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }


        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }
    }
}
