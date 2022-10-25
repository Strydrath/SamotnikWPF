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

namespace Samotnik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int[,] boardValues = new int[7, 7];
        public MainWindow()
        {
            InitializeComponent();
            initValues();
            initBoard();

        }
        public void initValues()
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    boardValues[i, j] = -1;
                }
                for (int j = 2; j < 5; j++)
                {
                    boardValues[i, j] = 1;
                }
                for (int j = 5; j < 7; j++)
                {
                    boardValues[i, j] = -1;
                }
            }
            for (int i = 2; i < 5; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    boardValues[i, j] = 1;
                }
            }
            for (int i = 5; i < 7; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    boardValues[i, j] = -1;
                }
                for (int j = 2; j < 5; j++)
                {
                    boardValues[i, j] = 1;
                }
                for (int j = 5; j < 7; j++)
                {
                    boardValues[i, j] = -1;
                }
            }

            boardValues[3, 3] = 0;
        }
        private void initBoard()
        {
            foreach (Control button in Board.Children)
            {
                if (button.GetType() == typeof(Button))
                {
                    int rowIndex = System.Windows.Controls.Grid.GetRow(button);
                    int columnIndex = System.Windows.Controls.Grid.GetColumn(button);
                    if (boardValues[rowIndex, columnIndex] == 0)
                    {
                        ((Button)button).Background = Brushes.Black;
                    }
                    else if (boardValues[rowIndex, columnIndex] == 1)
                    {
                        ((Button)button).Background = Brushes.Orange;
                    }
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int rowIndex = System.Windows.Controls.Grid.GetRow(button);
            int columnIndex = System.Windows.Controls.Grid.GetColumn(button);
            if (boardValues[rowIndex, columnIndex] == 0)
            {
                return;
            }
            highilightMoves(rowIndex + 2, columnIndex);
            highilightMoves(rowIndex - 2, columnIndex);
            highilightMoves(rowIndex, columnIndex);
            highilightMoves(rowIndex, columnIndex - 2);
        }
        private void highilightMoves(int row, int column)
        {
            if (row < 0 || column < 0)
            {
                return;
            }
            if (row >= 7 || column >= 7)
            {
                return;
            }
            if (boardValues[row, column] == 0)
            {
                Button button1 = (Button)Board.FindName("b" + row + column);
                button1.Background = Brushes.Blue;
                boardValues[row, column] = 2;
            }
        }
    }
}
