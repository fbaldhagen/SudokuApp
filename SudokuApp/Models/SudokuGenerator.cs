using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SudokuApp.Models
{
    public class SudokuGenerator
    {
        private static readonly Random random = new Random();
        public event EventHandler<int[,]> OnSolveStep;

        public int[,] Generate()
        {
            int[,] board = new int[9, 9];

            FillValues(board);

            // Remove some numbers to create the puzzle
            RemoveNumbers(board);

            return board;
        }

        private void FillValues(int[,] board)
        {
            FillDiagonal(board);

            FillRemaining(0, 3, board);
        }

        private void FillDiagonal(int[,] board)
        {
            for (int i = 0; i < 9; i += 3)
            {
                FillBox(board, i, i);
            }
        }

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

        private bool FillRemaining(int i, int j, int[,] board)
        {
            if (j >= 9 && i < 9 - 1)
            {
                j = 0;
                i++;
            }

            if (i >= 9 && j >= 9)
            {
                return true;
            }

            if (i < 3)
            {
                if (j < 3)
                {
                    j = 3;
                }
            }
            else if (i < 6)
            {
                if (j == (i / 3) * 3)
                {
                    j += 3;
                }
            }
            else
            {
                if (j == 6)
                {
                    j = 0;
                    i++;

                    if (i >= 9)
                    {
                        return true;
                    }
                }
            }

            for (int num = 1; num <= 9; num++)
            {
                if (IsSafe(i, j, num, board))
                {
                    board[i, j ] = num;
                    if (FillRemaining(i, j + 1, board))
                    {
                        return true;
                    }
                    board[i, j] = 0;
                }
            }
            return false;
        }

        public static bool IsSafe(int i, int j, int num, int[,] board)
        {
            return (ValidRow(i, num, board) &&
        ValidCol(j, num, board) &&
        ValidBox(i - i % 3,
        j - j % 3,
        num, board));
        }
        
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

        public bool Solve(int[,] board)
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
                if (IsSafe(row, col, num, board))
                {
                    board[row, col] = num;
                    if (Solve(board))
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


        private void RemoveNumbers(int[,] board)
        {
            int minRemovedCells = 45; // Minimum number of cells to remove
            int maxRemovedCells = 65; // Maximum number of cells to remove
            int count = 0;

            while (count < maxRemovedCells)
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
                    if (count >= minRemovedCells) // Stop removing cells if the minimum number of removed cells is reached and no more unique solutions can be found
                    {
                        break;
                    }
                }
            }
        }



        private bool HasUniqueSolution(int[,] board)
        {
            int solutions = 0;
            CountSolutions(board, ref solutions);
            return solutions == 1;
        }

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

        public async Task<bool> ShowSolve(int[,] board, int visualizationDelay)
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
                if (IsSafe(row, col, num, board))
                {
                    board[row, col] = num;
                    OnSolveStep?.Invoke(this, board);
                    await Task.Delay(visualizationDelay); // adjust the delay time (in milliseconds) to control the speed of the visualization
                    if (await ShowSolve(board, visualizationDelay))
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

    }
}
