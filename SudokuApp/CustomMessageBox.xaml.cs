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
    /// <summary>
    /// Represents a custom message box that can be centered to its owner window.
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        /// <summary>
        /// Gets the result of the custom message box, either MessageBoxResult.Yes or MessageBoxResult.No.
        /// </summary>
        public MessageBoxResult Result { get; private set; }

        /// <summary>
        /// Initializes a new instance of the CustomMessageBox class with the specified owner, message, and title.
        /// </summary>
        /// <param name="owner">The owner window of the custom message box.</param>
        /// <param name="message">The message to be displayed in the custom message box.</param>
        /// <param name="title">The title of the custom message box.</param>
        public CustomMessageBox(Window owner, string message, string title)
        {
            InitializeComponent();
            Owner = owner;
            Title = title;
            MessageText.Text = message;
            Result = MessageBoxResult.None;
        }

        /// <summary>
        /// Handles the click event of the Yes button, sets the result to MessageBoxResult.Yes, and closes the custom message box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }

        /// <summary>
        /// Handles the click event of the No button, sets the result to MessageBoxResult.No, and closes the custom message box.
        ///</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }
    }
}
