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
        private CancellationTokenSource _cancellationTokenSource;


        /// <summary>
        /// Initializes the MainWindow instance, setting up the UI and timer.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
        }

        /// <summary>
        /// Initializes the timer, sets the interval, and adds an event handler.
        /// </summary>
        private void InitializeTimer()
        {
            _timer = new System.Timers.Timer(1000); // Interval in milliseconds
            _timer.Elapsed += Timer_Elapsed;
            ResetAndUpdateTimer();
        }

        /// <summary>
        /// Updates the timer display each time the interval elapses.
        /// </summary>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _elapsedSeconds++;
            Dispatcher.Invoke(() =>
            {
                TimerTextBlock.Text = TimeSpan.FromSeconds(_elapsedSeconds).ToString(@"hh\:mm\:ss");
            });
        }

        /// <summary>
        /// Starts the timer and resets the elapsed time.
        /// </summary>
        private void StartTimer()
        {
            _elapsedSeconds = 0;
            _timer.Start();
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        private void StopTimer()
        {
            _timer.Stop();
        }

        /// <summary>
        /// Resets the timer and updates the timer display.
        /// </summary>
        private void ResetAndUpdateTimer()
        {
            _timer.Stop();
            _elapsedSeconds = 0;
            Dispatcher.Invoke(() =>
            {
                TimerTextBlock.Text = TimeSpan.FromSeconds(_elapsedSeconds).ToString(@"hh\:mm\:ss");
            });
        }

        /// <summary>
        /// Generates a new Sudoku puzzle when the "New Game" button is clicked.
        /// Asks for user confirmation before generating a new puzzle.
        /// </summary>
        private async void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            
            CustomMessageBox messageBox = new CustomMessageBox(this, "Do you want to generate a new puzzle?\nThis action cannot be undone.", "Generate New Puzzle"); // Using custom message boxes so the location is the center of the main window.
            messageBox.ShowDialog();
            MessageBoxResult result = messageBox.Result;
            
            if (result == MessageBoxResult.Yes)
            {
                DisableButtons();

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
                EnableButtons();
            }
        }

        /// <summary>
        /// Solves the current Sudoku puzzle when the "Solve" button is clicked.
        /// Asks for user confirmation before solving the puzzle.
        /// </summary>
        private async void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox(this, "Are you sure you want to solve the puzzle?\nThis action cannot be undone.", "Solve Puzzle"); // Using custom message boxes so the location is the center of the main window.
            messageBox.ShowDialog();
            MessageBoxResult result = messageBox.Result;

            if (result == MessageBoxResult.Yes)
            {
                DisableButtons();

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
                bool isSolved = await Task.Run(() => solver.Solve(board, 5000));

                if (isSolved)
                {
                    for (int row = 0; row < 9; row++)
                    {
                        for (int col = 0; col < 9; col++)
                        {
                            TextBox textBox = GetTextBoxAt(row, col);
                            textBox.Text = board[row, col].ToString();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("The puzzle couldn't be solved. Make sure your entries are correct.", "Couldn't solve", MessageBoxButton.OK, MessageBoxImage.Warning);
                }


                EnableButtons();
            }
        }

        /// <summary>
        /// Clears the entire Sudoku board when the "Clear" button is clicked.
        /// Asks for user confirmation before clearing the board.
        /// </summary>
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox(this, "Are you sure you want to clear the entire board?\nYou will lose any progress made and you will not be able to finish the puzzle.", "Clear board?");
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

        /// <summary>
        /// Retrieves a TextBox instance at a given row and column index.
        /// </summary>
        /// <param name="row">Row index of the TextBox to retrieve.</param>
        /// <param name="col">Column index of the TextBox to retrieve.</param>
        /// <returns>The TextBox instance at the specified row and column.</returns>
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

        /// <summary>
        /// Validates the current Sudoku solution entered by the user when the "Check" button is clicked.
        /// Shows a message box indicating whether the solution is correct or incorrect/incomplete.
        /// </summary>
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

        /// <summary>
        /// Resets all non-fixed cells on the Sudoku board to their initial empty state when the "Reset" button is clicked.
        /// </summary>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetGrid();
        }

        private void ResetGrid()
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

        /// <summary>
        /// Validates the current Sudoku solution entered by the user.
        /// </summary>
        /// <returns>True if the solution is valid, otherwise false.</returns>
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

        /// <summary>
        /// Retrieves the current state of the Sudoku puzzle from the UI.
        /// </summary>
        /// <returns>A 9x9 integer array representing the current Sudoku puzzle state.</returns>
        private int[,] GetCurrentBoard()
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
            return board;
        }

        /// <summary>
        /// Updates the UI based on the provided Sudoku puzzle state.
        /// </summary>
        /// <param name="board">A 9x9 integer array representing the Sudoku puzzle state to be displayed on the UI.</param>
        private void UpdateUIFromBoard(int[,] board)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    TextBox textBox = GetTextBoxAt(row, col);
                    int value = board[row, col];
                    textBox.Text = value == 0 ? string.Empty : value.ToString();
                }
            }
        }

        /// <summary>
        /// Handles the OnSolveStep event of the SudokuGenerator. Updates the UI with the provided board state.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="board">A 9x9 integer array representing the current state of the Sudoku puzzle during the solving process.</param>
        private async void Generator_OnSolveStep(object sender, int[,] board)
        {
            await Dispatcher.InvokeAsync(() => UpdateUIFromBoard(board));
        }


        /// <summary>
        /// Handles the VisualizeSolveButton click event. Disables UI elements, visualizes the solving process,
        /// updates the UI based on the result, and handles cancellation if the user decides to stop the visualization.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void VisualizeSolveButton_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();
            CancelButton.IsEnabled = true;
            _cancellationTokenSource = new CancellationTokenSource();

            int[,] board = GetCurrentBoard();
            int visualizationDelay = (int)VisualizationSpeedSlider.Value;

            if (!SudokuGenerator.IsValidPuzzle(board))
            {
                MessageBox.Show("The puzzle is invalid and cannot be solved.", "Invalid Puzzle", MessageBoxButton.OK, MessageBoxImage.Error);
                CancelButton.IsEnabled = false;
                EnableButtons();
                return;
            }

            SudokuGenerator generator = new SudokuGenerator();
            generator.OnSolveStep += Generator_OnSolveStep;

            bool isSolved = false;
            try
            {
                isSolved = await generator.ShowSolve(board, visualizationDelay, _cancellationTokenSource.Token);
            }
            catch (Exception ex)  when (ex is TaskCanceledException ||  ex is OperationCanceledException)
            {
                ResetGrid();
            }

            if (isSolved)
            {
                UpdateUIFromBoard(board);
            }
            else
            {
                // message if the puzzle couldn't be solved
            }
            CancelButton.IsEnabled = false;
            EnableButtons();
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            CancelButton.IsEnabled = false;
        }

        /// <summary>
        /// Disables the UI elements (buttons and slider) by setting their IsEnabled properties to false.
        /// </summary>
        private void DisableButtons()
        {
            NewGameButton.IsEnabled = false;
            CheckButton.IsEnabled = false;
            ResetButton.IsEnabled = false;
            ClearButton.IsEnabled = false;

            SolveButton.IsEnabled = false;
            VisualizeSolveButton.IsEnabled = false;
            VisualizationSpeedSlider.IsEnabled = false;

        }

        /// <summary>
        /// Enables the UI elements (buttons and slider) by setting their IsEnabled properties to true.
        /// </summary>
        private void EnableButtons()
        {
            NewGameButton.IsEnabled = true;
            CheckButton.IsEnabled = true;
            ResetButton.IsEnabled = true;
            ClearButton.IsEnabled = true;

            SolveButton.IsEnabled = true;
            VisualizeSolveButton.IsEnabled = true;
            VisualizationSpeedSlider.IsEnabled = true;
        }

        /// <summary>
        /// Event handler for when the selected item in the DifficultyComboBox is changed.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>
        /// This method calls the SetDifficulty method in the SudokuGenerator class,
        /// using the selected index of the DifficultyComboBox as the difficulty level.
        /// </remarks>
        private void OnDifficultyChanged(object sender, EventArgs e)
        {
            SudokuGenerator.SetDifficulty(DifficultyComboBox.SelectedIndex);
        }
    }
}
