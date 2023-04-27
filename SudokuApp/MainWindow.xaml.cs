using SudokuApp.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SudokuApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            NewGameButton.IsEnabled = false;
            SolveButton.IsEnabled = false;
            ClearButton.IsEnabled = false;

            SudokuGenerator generator = new SudokuGenerator();

            int[,] newPuzzle = await Task.Run(() => generator.Generate());

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    TextBox textBox = GetTextBoxAt(row, col);
                    int value = newPuzzle[row, col];
                    textBox.Text = value == 0 ? string.Empty : value.ToString();
                }
            }

            NewGameButton.IsEnabled = true;
            SolveButton.IsEnabled = true;
            ClearButton.IsEnabled = true;
        }



        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            int[,] board = new int[9, 9];

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    TextBox textBox = GetTextBoxAt(row, col); 
                    int.TryParse(textBox.Text, out int value);
                    board[row, col] = value;
                }
            }

            SudokuGenerator solver = new SudokuGenerator();
            solver.Solve(board);

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    TextBox textBox = GetTextBoxAt(row, col);
                    textBox.Text = board[row, col].ToString();
                }
            }
        }


        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    TextBox textBox = GetTextBoxAt(row, col); 
                    textBox.Text = string.Empty;
                }
            }
        }
        private TextBox GetTextBoxAt(int row, int col)
        {
            int boxRow = row / 3;
            int boxCol = col / 3;
            int inBoxRow = row % 3;
            int inBoxCol = col % 3;

            int cellIndex = boxRow * 27 + inBoxRow * 3 + boxCol * 9 + inBoxCol;
            string textBoxName = $"Cell{cellIndex:D2}";
            TextBox textBox = (TextBox)this.FindName(textBoxName);
            return textBox;
        }


    }
}
