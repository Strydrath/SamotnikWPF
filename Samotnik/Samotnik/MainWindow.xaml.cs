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
        private Field[,] boardValues = new Field[7, 7];
        private Field chosen;
        private Field inDanger;
        private List<Field> highlighted;
        private int score;
        public MainWindow()
        {
            highlighted = new List<Field>();
            InitializeComponent();
            initValues();
            initBoard();
            score = 0;
            PointCounter.Text = "Punkty = " + score;

        }
        public void initValues()
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    boardValues[i, j] = new Field(i,j,Field.FieldState.outside);
                }
                for (int j = 2; j < 5; j++)
                {
                    boardValues[i, j] = new Field(i, j, Field.FieldState.full);
                }
                for (int j = 5; j < 7; j++)
                {
                    boardValues[i, j] = new Field(i, j, Field.FieldState.outside);
                }
            }
            for (int i = 2; i < 5; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    boardValues[i, j] = new Field(i, j, Field.FieldState.full);
                }
            }
            for (int i = 5; i < 7; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    boardValues[i, j] = new Field(i, j, Field.FieldState.outside);
                }
                for (int j = 2; j < 5; j++)
                {
                    boardValues[i, j] = new Field(i, j, Field.FieldState.full);
                }
                for (int j = 5; j < 7; j++)
                {
                    boardValues[i, j] = new Field(i, j, Field.FieldState.outside);
                }
            }

            boardValues[3, 3] = new Field(3, 3, Field.FieldState.empty);
        }
        private void initBoard()
        {
            foreach (Control button in Board.Children)
            {
                if (button.GetType() == typeof(Button))
                {
                    int rowIndex = System.Windows.Controls.Grid.GetRow(button);
                    int columnIndex = System.Windows.Controls.Grid.GetColumn(button);
                    if (boardValues[rowIndex, columnIndex].State == Field.FieldState.empty)
                    {
                        ((Button)button).Background = Brushes.Black;
                    }
                    else if (boardValues[rowIndex, columnIndex].State == Field.FieldState.full)
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
            Field clicked = boardValues[rowIndex, columnIndex];
            if (clicked.State == Field.FieldState.highlighted)
            {

                if (checkMove(clicked))
                {
                    inDanger.State = Field.FieldState.empty;
                    Button button4 = (Button)Board.FindName("b" + inDanger.Row + inDanger.Column);
                    button4.Background = Brushes.Black;
                }
                chosen.State = Field.FieldState.empty;
                Button button2 = (Button)Board.FindName("b" + chosen.Row + chosen.Column);
                button2.Background = Brushes.Black;
                foreach(Field f in highlighted)
                {
                    Button button3 = (Button)Board.FindName("b" + f.Row + f.Column);
                    button3.Background = Brushes.Black;
                }
                highlighted.Clear();
                clicked.State = Field.FieldState.full;
                Button button1 = (Button)Board.FindName("b" + clicked.Row + clicked.Column);
                button1.Background = Brushes.Orange;
                score += 1;
                PointCounter.Text = "Punkty = "+score;
            }
            else if (clicked.State == Field.FieldState.empty)
            {
                return;
            }
            else if (clicked.State == Field.FieldState.full)
            {
                boardValues[rowIndex, columnIndex].State = Field.FieldState.chosen;
                chosen = clicked;
                highilightMoves(rowIndex + 2, columnIndex);
                highilightMoves(rowIndex - 2, columnIndex);
                highilightMoves(rowIndex, columnIndex + 2);
                highilightMoves(rowIndex, columnIndex - 2);
            }
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
            if (boardValues[row, column].State == Field.FieldState.empty )
            {
                if (checkMove(boardValues[row, column]))
                {
                    Button button1 = (Button)Board.FindName("b" + row + column);
                    button1.Background = Brushes.Blue;
                    boardValues[row, column].State = Field.FieldState.highlighted;
                    highlighted.Add(boardValues[row, column]);
                }
            }
        }
        private bool checkMove(Field f)
        {
            int row, column;
            if (f.Column > chosen.Column)
            {
                column = f.Column - 1;
                row = f.Row;
            }
            else if(f.Column < chosen.Column)
            {
                column = f.Column + 1;
                row = f.Row;
            }
            else if(f.Row < chosen.Row)
            {
                column = f.Column;
                row = f.Row + 1;
            }
            else if (f.Row > chosen.Row)
            {
                column = f.Column;
                row = f.Row - 1;
            }
            else
            {
                return false;
            }
            if(boardValues[row,column].State == Field.FieldState.full)
            {
                inDanger = boardValues[row, column];
                return true;
            }
            return false;
        }
    }
}
