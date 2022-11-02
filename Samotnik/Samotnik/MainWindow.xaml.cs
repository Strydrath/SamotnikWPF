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
    public partial class MainWindow : Window
    {
        private static int boardSize = 7;
        public bool GameOver;
        public bool GameStarted;
        private Field[,] boardValues = new Field[boardSize, boardSize];
        private Field chosen;
        private Field inDanger;
        private List<Field> highlighted;
        private int score;
        private List<(Field field,int round)> moves;
        private Style fullStyle;
        private Style emptyStyle;
        private Style chosenStyle;
        private Style highlightStyle;
        public MainWindow()
        {
            GameOver = false;
            GameStarted = false;
            highlighted = new List<Field>();
            moves = new List<(Field, int)>();
            InitializeComponent();
            setupStyles();
            initValues();
            initBoard();
            score = 0;
            PointCounter.Text = "Punkty = " + score;

        }

        private void setupStyles()
        {
            fullStyle = this.FindResource("FullButton") as Style;
            emptyStyle = this.FindResource("EmptyButton") as Style;
            chosenStyle = this.FindResource("ChosenButton") as Style;
            highlightStyle = this.FindResource("HighlightedButton") as Style;

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
                for (int j = 5; j < boardSize; j++)
                {
                    boardValues[i, j] = new Field(i, j, Field.FieldState.outside);
                }
            }
            for (int i = 2; i < 5; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    boardValues[i, j] = new Field(i, j, Field.FieldState.full);
                }
            }
            for (int i = 5; i < boardSize; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    boardValues[i, j] = new Field(i, j, Field.FieldState.outside);
                }
                for (int j = 2; j < 5; j++)
                {
                    boardValues[i, j] = new Field(i, j, Field.FieldState.full);
                }
                for (int j = 5; j < boardSize; j++)
                {
                    boardValues[i, j] = new Field(i, j, Field.FieldState.outside);
                }
            }

            boardValues[(boardSize-1)/2, (boardSize - 1) / 2] = new Field(3, 3, Field.FieldState.empty);
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
                        ((Button)button).Style = emptyStyle;
                    }
                    else if (boardValues[rowIndex, columnIndex].State == Field.FieldState.full)
                    {
                        ((Button)button).Style = fullStyle;
                    }
                }
            }
        }

        private void showStates()
        {
            foreach (Control button in Board.Children)
            {
                int rowIndex = System.Windows.Controls.Grid.GetRow(button);
                int columnIndex = System.Windows.Controls.Grid.GetColumn(button);
                Field b = boardValues[rowIndex, columnIndex];
                switch (b.State)
                {
                    case Field.FieldState.full:
                        ((Button)button).Style = fullStyle;
                        break;
                    case Field.FieldState.empty:
                        ((Button)button).Style = emptyStyle;
                        break;
                    case Field.FieldState.highlighted:
                        ((Button)button).Style = highlightStyle;
                        break;
                }
                if (b.Equals(chosen))
                {
                    ((Button)button).Style = chosenStyle;
                }
            }
        }

        private void ResetGame(object sender, RoutedEventArgs e)
        {
            score = 0;
            initValues();
            initBoard();
            PointCounter.Text = "Punkty = " + score;
            showStates();
            GameStarted = true;
            gameOver.IsOpen = false;
        }
        private void Undo(object sender, RoutedEventArgs e)
        {
            if (score > 0) { 
                score--;
                foreach((var move, var number) in moves)
                {
                    if(number == score)
                    {
                        boardValues[move.Row, move.Column].State = move.State;
                    }   
                }
                moves.RemoveAll(item => item.round == score);
                PointCounter.Text = "Punkty = " + score;
                showStates();
            }
        }

        private void Start_Game(object sender, RoutedEventArgs e)
        {
            GameStarted = true;
            Button button = (Button)sender;
            button.Visibility = Visibility.Collapsed;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!GameStarted)
            {
                return;
            }
            Button button = (Button)sender;
            int rowIndex = System.Windows.Controls.Grid.GetRow(button);
            int columnIndex = System.Windows.Controls.Grid.GetColumn(button);
            Field clicked = boardValues[rowIndex, columnIndex];
            if (clicked.State == Field.FieldState.highlighted)
            {

                if (checkMove(clicked,chosen))
                {
                    inDanger.State = Field.FieldState.empty;
                    chosen.State = Field.FieldState.empty;
                    clearHighlights();
                    highlighted.Clear();
                    clicked.State = Field.FieldState.full;
                    moves.Add((new Field(inDanger.Row,inDanger.Column,Field.FieldState.full), score));
                    moves.Add((new Field(chosen.Row, chosen.Column, Field.FieldState.full), score));
                    moves.Add((new Field(clicked.Row, clicked.Column, Field.FieldState.empty), score));
                    score += 1;
                    PointCounter.Text = "Punkty = " + score;
                    if (checkWin())
                    {
                        PointCounter.Text = "Punkty = "+score.ToString();
                        GameOver = true;
                        GameStarted = false;
                        gameOver.IsOpen = true;
                        gameOverText.Text = "WYGRALES,GRATULACJE!";
                    }
                    else if (checkGameEnd())
                    {
                        PointCounter.Text = "Punkty = " + score.ToString();
                        GameOver = true;
                        GameStarted = false;
                        gameOver.IsOpen = true;
                        gameOverText.Text = "KONIEC GRY Z WYNIKIEM : " + score;
                    }
                    chosen = null;
                    showStates();
                }
                
            }
            else if (clicked.State == Field.FieldState.empty)
            {
                return;
            }
            else if (clicked.State == Field.FieldState.full)
            {
                clearHighlights();
                chosen = clicked;
                showStates();
                highilightMoves(rowIndex + 2, columnIndex);
                highilightMoves(rowIndex - 2, columnIndex);
                highilightMoves(rowIndex, columnIndex + 2);
                highilightMoves(rowIndex, columnIndex - 2);
            }
            showStates();
        }
        private void highilightMoves(int row, int column)
        {
            if (row < 0 || column < 0)
            {
                return;
            }
            if (row >= boardSize  || column >= boardSize)
            {
                return;
            }
            if (boardValues[row, column].State == Field.FieldState.empty || boardValues[row, column].State == Field.FieldState.highlighted)
            {
                if (checkMove(boardValues[row, column],chosen))
                {
                    Button button1 = (Button)Board.FindName("b" + row + column);
                    button1.Style = chosenStyle;
                    boardValues[row, column].State = Field.FieldState.highlighted;
                    highlighted.Add(boardValues[row, column]);
                }
            }
        }

        private void clearHighlights()
        {
            foreach (Field f in highlighted)
            {
                f.State = Field.FieldState.empty;
            }
            highlighted.Clear();
        }
        private bool isAMove(Field a, int row, int column)
        {
            if(row>=0 && row<7 && column>=0 && column < 7)
            {
                if (boardValues[row, column].State == Field.FieldState.empty)
                {
                    if(checkMove(boardValues[row, column], a))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool checkWin()
        {
            int count = 0;
            foreach (Field f in boardValues)
            {
                if (f.State == Field.FieldState.full||f.State == Field.FieldState.highlighted)
                {
                    count++;
                }
            }

            if (count > 1)
            {
                return false;
            }

            return true;
        }
        private bool checkGameEnd()
        {
            bool end = true;
            foreach(Field f in boardValues)
            {
                if (f.State == Field.FieldState.full)
                {
                    if(f.Row ==4 &&f.Column == 0){
                        int x = 0;
                    }
                    if (isAMove(f, f.Row + 2, f.Column))
                    {
                        end = false;
                    }
                    if (isAMove(f, f.Row - 2, f.Column))
                    {
                        end = false;
                    }
                    if (isAMove(f, f.Row, f.Column + 2))
                    {
                        end = false;
                    }
                    if (isAMove(f, f.Row, f.Column - 2))
                    {
                        end = false;
                    }
                }
            }
            return end;
        }
        private bool checkMove(Field a, Field b)
        {
            int row, column;
            if (b.Column > a.Column)
            {
                column = b.Column - 1;
                row = a.Row;
            }
            else if(b.Column < a.Column)
            {
                column = b.Column + 1;
                row = b.Row;
            }
            else if(b.Row < a.Row)
            {
                column = b.Column;
                row = b.Row + 1;
            }
            else if (b.Row > a.Row)
            {
                column = b.Column;
                row = b.Row - 1;
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
