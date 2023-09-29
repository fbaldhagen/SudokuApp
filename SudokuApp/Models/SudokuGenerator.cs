using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SudokuApp.Models
{
    public class SudokuGenerator
    {
        private static readonly Random random = new Random();
        public event EventHandler<int[,]> OnSolveStep;
        private static int MinRemovedCells = 36;
        private static int MaxRemovedCells = 45;

        /// <summary>
        /// Sets the difficulty of the Sudoku game.
        /// </summary>
        /// <param name="difficulty">The difficulty level. The levels are represented by the integers 0 to 3, 
        /// where 0 is the easiest level and 3 is the hardest.
        /// The difficulty level determines the number of cells removed from the initial, completed Sudoku grid.
        /// </param>
        /// <remarks>
        /// The difficulty levels adjust the minimum and maximum number of cells removed as follows:
        /// - Easy: 36 to 45 cells
        /// - Medium: 46 to 50 cells
        /// - Hard: 51 to 55 cells
        /// - Evil: 56 to 70 cells
        /// </remarks>
        public static void SetDifficulty(int difficulty) 
        {
            switch (difficulty)
            {
                case 0:
                    MinRemovedCells = 36;
                    MaxRemovedCells = 45;
                    break;
                case 1:
                    MinRemovedCells = 46;
                    MaxRemovedCells = 50;
                    break;
                case 2:
                    MinRemovedCells = 50;
                    MaxRemovedCells = 55;
                    break;
                case 3:
                    MinRemovedCells = 55;
                    MaxRemovedCells = 70;
                    break;
            }
        }


        /// <summary>
        /// Generates a new Sudoku puzzle by filling a grid and removing some numbers.
        /// </summary>
        public int[,] Generate()
        {
            int[,] board = new int[9, 9];

            FillValues(board);

            // Remove some numbers to create the puzzle
            RemoveNumbers(board);

            return board;
        }

        /// <summary>
        /// Fills the Sudoku grid with values.
        /// </summary>
        private void FillValues(int[,] board)
        {
            FillDiagonal(board);

            FillRemaining(0, 3, board);
        }

        /// <summary>
        /// Fills the diagonal of the Sudoku grid.
        /// </summary>
        private void FillDiagonal(int[,] board)
        {
            for (int i = 0; i < 9; i += 3)
            {
                FillBox(board, i, i);
            }
        }

        /// <summary>
        /// Fills a 3x3 box in the Sudoku grid with numbers 1 to 9.
        /// </summary>
        private void FillBox(int[,] board, int row, int col)
        {
            int[] cellNumbers = Enumerable.Range(1, 9).ToArray();

            Shuffle(cellNumbers);

            int count = 0;

            for (int currRow = row; currRow < row + 3; currRow++)
            {
                for (int currCol = col; currCol < col + 3; currCol++)
                {
                    board[currRow, currCol] = cellNumbers[count];
                    count++;
                }
            }
        }

        /// <summary>
        /// Recursively fills remaining cells of the Sudoku grid.
        /// </summary>
        private bool FillRemaining(int i, int j, int[,] board)
        {
            // Move to the next row if we're at the end of the current one
            if (j >= 9 && i < 9 - 1)
            {
                j = 0;
                i++;
            }

            // If we've filled the entire board, we're done
            if (i >= 9 && j >= 9)
            {
                return true;
            }

            // Skip the top-left 3x3 block if we're in the first 3 rows
            if (i < 3 && j < 3)
            {
                j = 3;
            }
            // For the next 3 rows, skip the middle 3x3 block if necessary
            else if (i < 6 && j == (i / 3) * 3)
            {
                j += 3;
            }
            // For the last 3 rows, wrap to the next row if we're in the last 3 columns
            else if (j == 6)
            {
                j = 0;
                i++;

                // If we've filled the entire board, we're done
                if (i >= 9)
                {
                    return true;
                }
            }

            // Try numbers 1-9 in the current cell
            for (int num = 1; num <= 9; num++)
            {
                if (IsSafe(i, j, num, board))
                {
                    board[i, j] = num;

                    if (FillRemaining(i, j + 1, board))
                    {
                        return true;
                    }

                    // Backtrack
                    board[i, j] = 0;
                }
            }

            // If no number can be placed in the current cell, backtrack further
            return false;
        }


        /// <summary>
        /// Checks if placing a number in a specific cell is safe.
        /// </summary>
        public static bool IsSafe(int i, int j, int num, int[,] board)
        {
            return (ValidRow(i, num, board) &&
        ValidCol(j, num, board) &&
        ValidBox(i - i % 3,
        j - j % 3,
        num, board));
        }

        /// <summary>
        /// Checks if a number is valid for a row in the Sudoku grid.
        /// </summary>
        private static bool ValidRow(int row, int num, int[,] board)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board[row, col] == num)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if a number is valid for a column in the Sudoku grid.
        /// </summary>
        private static bool ValidCol(int col, int num, int[,] board)
        {
            for (int row = 0; row < 9; row++)
            {
                if (board[row, col] == num)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if a number is valid for a 3x3 box in the Sudoku grid.
        /// </summary>
        private static bool ValidBox(int rowStart, int colStart, int num, int[,] board)
        {
            for (int row = rowStart; row < rowStart + 3; row++)
            {
                for (int col = colStart; col < colStart + 3; col++)
                {
                    if (board[row, col] == num)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Shuffles the order of the numbers in an array.
        /// </summary>
        private void Shuffle(int[] cellNumbers) 
        {
            int n = cellNumbers.Length;
            while (n > 1)
            {
                int k = random.Next(n--);
                int temp = cellNumbers[n];
                cellNumbers[n] = cellNumbers[k];
                cellNumbers[k] = temp;
            }
        }

        /// <summary>
        /// Solves the Sudoku puzzle using a backtracking algorithm with a time-out.
        /// </summary>
        /// <param name="board">The Sudoku board represented as a 9x9 integer array.</param>
        /// <param name="timeOut">The maximum allowed time (in milliseconds) for solving the puzzle.</param>
        /// <returns>True if the puzzle is solved within the time limit, false otherwise.</returns>
        public bool Solve(int[,] board, int timeOut)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(timeOut);
            return SolveHelper(board, cts.Token);
        }

        /// <summary>
        /// A helper method that solves the Sudoku puzzle using a backtracking algorithm.
        /// </summary>
        /// <param name="board">The Sudoku board represented as a 9x9 integer array.</param>
        /// <param name="cancellationToken">A cancellation token to handle time-outs and cancellations.</param>
        /// <returns>True if the puzzle is solved, false otherwise.</returns>
        private bool SolveHelper(int[,] board, CancellationToken cancellationToken)
        {
            int row = -1;
            int col = -1;
            bool isEmpty = true;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board[i, j] == 0)
                    {
                        row = i;
                        col = j;
                        isEmpty = false;
                        break;
                    }
                }
                if (!isEmpty)
                {
                    break;
                }
            }

            if (isEmpty)
            {
                return true;
            }

            for (int num = 1; num <= 9; num++)
            {
                if (cancellationToken.IsCancellationRequested) return false;

                if (IsSafe(row, col, num, board))
                {
                    board[row, col] = num;
                    if (SolveHelper(board, cancellationToken))
                    {
                        return true;
                    }
                    else
                    {
                        board[row, col] = 0;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Removes numbers from the Sudoku grid to create a puzzle.
        /// </summary>
        private void RemoveNumbers(int[,] board)
        {
            int count = 0;

            while (count < MaxRemovedCells)
            {
                int row = random.Next(9);
                int col = random.Next(9);

                while (board[row, col] == 0)
                {
                    row = random.Next(9);
                    col = random.Next(9);
                }

                int backup = board[row, col];
                int[,] boardCopy = (int[,])board.Clone();
                boardCopy[row, col] = 0;

                if (HasUniqueSolution(boardCopy))
                {
                    board[row, col] = 0;
                    count++;
                }
                else
                {
                    board[row, col] = backup;
                    if (count >= MinRemovedCells) // Stop removing cells if the minimum number of removed cells is reached and no more unique solutions can be found
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the Sudoku grid has a unique solution.
        /// </summary>
        private bool HasUniqueSolution(int[,] board)
        {
            int solutions = 0;
            CountSolutions(board, ref solutions);
            return solutions == 1;
        }

        /// <summary>
        /// Counts the number of solutions for the given Sudoku grid.
        /// </summary>
        private void CountSolutions(int[,] board, ref int count)
        {
            int row = -1;
            int col = -1;
            bool isEmpty = true;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board[i, j] == 0)
                    {
                        row = i;
                        col = j;
                        isEmpty = false;
                        break;
                    }
                }
                if (!isEmpty)
                {
                    break;
                }
            }

            if (isEmpty)
            {
                count++;
                return;
            }

            for (int num = 1; num <= 9; num++)
            {
                if (IsSafe(row, col, num, board))
                {
                    board[row, col] = num;
                    CountSolutions(board, ref count);
                    board[row, col] = 0;
                }
            }
        }

        /// <summary>
        /// Visualizes the process of solving a Sudoku puzzle with a delay between steps.
        /// </summary>
        public async Task<bool> ShowSolve(int[,] board, int visualizationDelay, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            (int row, int col) = FindEmptyCell(board);

            if (row == -1 && col == -1)
            {
                return true;
            }

            for (int num = 1; num <= 9; num++)
            {
                if (IsSafe(row, col, num, board))
                {
                    board[row, col] = num;
                    OnSolveStep?.Invoke(this, board);
                    await Task.Delay(visualizationDelay, cancellationToken);

                    if (await ShowSolve(board, visualizationDelay, cancellationToken))
                    {
                        return true;
                    }
                    else
                    {
                        board[row, col] = 0;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Finds the first empty cell in the Sudoku grid.
        /// </summary>
        private (int row, int col) FindEmptyCell(int[,] board)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board[i, j] == 0)
                    {
                        return (i, j);
                    }
                }
            }
            return (-1, -1);
        }

        /// <summary>
        /// Determines whether the given Sudoku puzzle is valid by checking for duplicates
        /// in each row, column, and 3x3 block.
        /// </summary>
        /// <param name="board">A 9x9 integer array representing the Sudoku puzzle.</param>
        /// <returns>True if the puzzle is valid, false otherwise.</returns>
        public static bool IsValidPuzzle(int[,] board)
        {
            // Check for duplicates in rows, columns, and blocks
            for (int i = 0; i < 9; i++)
            {
                HashSet<int> rowSet = new HashSet<int>();
                HashSet<int> colSet = new HashSet<int>();
                HashSet<int> blockSet = new HashSet<int>();

                for (int j = 0; j < 9; j++)
                {
                    int rowValue = board[i, j];
                    int colValue = board[j, i];
                    int blockValue = board[i / 3 * 3 + j / 3, i % 3 * 3 + j % 3];

                    if (rowValue != 0 && !rowSet.Add(rowValue)) return false;
                    if (colValue != 0 && !colSet.Add(colValue)) return false;
                    if (blockValue != 0 && !blockSet.Add(blockValue)) return false;
                }
            }

            return true;
        }
    }
}
