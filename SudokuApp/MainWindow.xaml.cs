using SudokuApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Timers;

namespace SudokuApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private System.Timers.Timer _timer;
        private int _elapsedSeconds;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _timer = new System.Timers.Timer(1000); // Interval in milliseconds
            _timer.Elapsed += Timer_Elapsed;
            ResetAndUpdateTimer();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _elapsedSeconds++;
            Dispatcher.Invoke(() =>
            {
                TimerTextBlock.Text = TimeSpan.FromSeconds(_elapsedSeconds).ToString(@"hh\:mm\:ss");
            });
        }

        private void StartTimer()
        {
            _elapsedSeconds = 0;
            _timer.Start();
        }

        private void StopTimer()
        {
            _timer.Stop();
        }

        private void ResetAndUpdateTimer()
        {
            _timer.Stop();
            _elapsedSeconds = 0;
            Dispatcher.Invoke(() =>
            {
                TimerTextBlock.Text = TimeSpan.FromSeconds(_elapsedSeconds).ToString(@"hh\:mm\:ss");
            });
        }


        private async void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            
            CustomMessageBox messageBox = new CustomMessageBox(this, "Do you want to generate a new puzzle?\nThis action cannot be undone.", "Generate New Puzzle"); // Using custom message boxes so the location is the center of the main window.
            messageBox.ShowDialog();
            MessageBoxResult result = messageBox.Result;
            
            if (result == MessageBoxResult.Yes)
            {
                NewGameButton.IsEnabled = false;
                SolveButton.IsEnabled = false;
                ClearButton.IsEnabled = false;

                SudokuGenerator generator = new SudokuGenerator();

                int[,] newPuzzle = await Task.Run(() => generator.Generate());
                StartTimer();

                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        TextBox textBox = GetTextBoxAt(row, col);
                        int value = newPuzzle[row, col];
                        textBox.Text = value == 0 ? string.Empty : value.ToString();
                        textBox.IsReadOnly = value != 0;
                        textBox.Background = value == 0 ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.LightGray);
                    }
                }

                NewGameButton.IsEnabled = true;
                SolveButton.IsEnabled = true;
                ClearButton.IsEnabled = true; 
            }
        }



        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox(this, "Are you sure you want to solve the puzzle?\nThis action cannot be undone.", "Solve Puzzle"); // Using custom message boxes so the location is the center of the main window.
            messageBox.ShowDialog();
            MessageBoxResult result = messageBox.Result;

            if (result == MessageBoxResult.Yes)
            {
                NewGameButton.IsEnabled = false;
                SolveButton.IsEnabled = false;
                ClearButton.IsEnabled = false;

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

                NewGameButton.IsEnabled = true;
                SolveButton.IsEnabled = true;
                ClearButton.IsEnabled = true;
            }
        }


        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox(this, "Are you sure you want to clear the board?\nYou will lose any progress made.", "Clear board?");
            messageBox.ShowDialog();
            MessageBoxResult result = messageBox.Result;
            if (result == MessageBoxResult.Yes)
            {
                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        TextBox textBox = GetTextBoxAt(row, col);
                        textBox.Text = string.Empty;
                        textBox.IsReadOnly = false;
                        textBox.Background = new SolidColorBrush(Colors.White);
                    }
                }

                ResetAndUpdateTimer(); 
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

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateSolution())
            {
                StopTimer();
                MessageBox.Show("The solution is correct!", "Correct Solution", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("The solution is incorrect/incomplete.", "Incorrect Solution", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    TextBox textBox = GetTextBoxAt(row, col);
                    if (!textBox.IsReadOnly)
                    {
                        textBox.Text = string.Empty;
                    }
                }
            }
        }


        private bool ValidateSolution()
        {
            int[,] board = new int[9, 9];

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    TextBox textBox = GetTextBoxAt(row, col);
                    int value;

                    if (int.TryParse(textBox.Text, out value) && value >= 1 && value <= 9)
                    {
                        board[row, col] = value;
                    }
                    else
                    {
                        // Invalid value entered
                        return false;
                    }
                }
            }

            // Check if the Sudoku solution is valid
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    int value = board[row, col];

                    // Check row and column
                    for (int i = 0; i < 9; i++)
                    {
                        if ((i != col && board[row, i] == value) || (i != row && board[i, col] == value))
                        {
                            return false;
                        }
                    }

                    // Check 3x3 box
                    int boxStartRow = row - row % 3;
                    int boxStartCol = col - col % 3;

                    for (int r = boxStartRow; r < boxStartRow + 3; r++)
                    {
                        for (int c = boxStartCol; c < boxStartCol + 3; c++)
                        {
                            if (r != row && c != col && board[r, c] == value)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}
