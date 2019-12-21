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
using ChineseChess.Model;
using ChineseChess.View;
using ChineseChess.Control;

namespace ChineseChess_G1
{
    enum GameStatus
    {
        TO_CHOOSE,
        TO_MOVE
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Custom properties definition for chessBoardRowProperty and chessBoardColumnProperty
        private static readonly DependencyProperty chessBoardRowProperty = DependencyProperty.Register(
            "chessBoardRow",
            typeof(int),
            typeof(Image),
            new PropertyMetadata(default(int))
        );
        private static readonly DependencyProperty chessBoardColProperty = DependencyProperty.Register(
            "chessBoardCol",
            typeof(int),
            typeof(Image),
            new PropertyMetadata(default(int))
        );

        // Initial piece status
        private GameStatus gameStatus = GameStatus.TO_CHOOSE;

        // Method to change the piece status
        private void changeGameStatus(GameStatus gameStatus)
        {
            this.gameStatus = gameStatus;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        // Initial positions of all the pieces and initialization of 9*10 grid
        private void IniGame()
        {
            // Start the game
            GameRules.iniGame();

            // Implement of 9 Columns and 10 Rows
            for (int row = 0; row < Board.pieces.GetLength(0); row++)
            {
                chessPanel.RowDefinitions.Add(new RowDefinition());
            }
            for (int col = 0; col < Board.pieces.GetLength(1); col++)
            {
                chessPanel.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Implement of 90 images
            for (int row = 0; row < Board.pieces.GetLength(0); row++)
            {
                for (int col = 0; col < Board.pieces.GetLength(1); col++)
                {
                    Image image = new Image
                    {
                        // Set default properties (name, margin) of each image
                        Name = "Image" + row.ToString() + col.ToString(),
                        Margin = new Thickness(5, 5, 5, 5)
                    };
                    // Set customized propertied
                    image.SetValue(chessBoardRowProperty, row);
                    image.SetValue(chessBoardColProperty, col);
                    // Click event
                    image.MouseUp += new MouseButtonEventHandler(this.Image_MouseUp);

                    // Attach to the chessPanel
                    Grid.SetRow(image, row);
                    Grid.SetColumn(image, col);
                    chessPanel.Children.Add(image);
                }
            }
        }

        // Draw the pieces on the chess panel
        private void redrawPieces()
        {
            int i = 0;
            foreach (Pieces piece in Board.pieces)
            {
                // Obtain every single piece from the chess panel
                Image pieceImg = (Image)chessPanel.Children[i];
                // If there is a piece stored on the chess board, draw it on the panel
                if (piece != null) { pieceImg.Source = new BitmapImage(new Uri(piece.url, UriKind.RelativeOrAbsolute)); pieceImg.Opacity = 1; }
                // if not, draw null image on the panel
                else pieceImg.Source = null;
                i++;
            }
        }

        // Draw the valid moves of the chosen piece
        private void drawValidMove()
        {
            // calculate the valid move of the chosen piece
            List<int> validMoves = Board.pieces[Board.getLastOriLocation()[0], Board.getLastOriLocation()[1]].validMoveList;
            foreach (int validMove in validMoves)
            {
                // the valid move location (x,y) is corresponding to the index 9*x+y of the chess panel's grid
                Image validMoveImg = (Image)chessPanel.Children[9 * (validMove / 10) + validMove % 10];
                // If there is a piece in the valid move position, the piece will become transparent
                if (Board.pieces[validMove / 10, validMove % 10] != null) validMoveImg.Opacity = 0.4;
                // If the valid move position has no piece we put a ValidMoveBox
                else validMoveImg.Source = new BitmapImage(new Uri("/Images/ValidMoveBox.png", UriKind.RelativeOrAbsolute));
            }
        }

        // Show the current round is of which team
        private void redrawCurrenColour()
        {
            if (Board.currentColour % 2 == 0) txtblkCurrentColor.Text = "Move of BLACK";
            else txtblkCurrentColor.Text = "Move of RED";
        }

        // Start button click event handle
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // Initialization of all the components of the game
            IniGame();
            // draw the current colour
            redrawCurrenColour();
            // apprear the current colour textblock, regret button, restart button
            // as well as disappear the start button
            txtblkCurrentColor.SetValue(VisibilityProperty, Visibility.Visible);
            btnStart.SetValue(VisibilityProperty, Visibility.Hidden);
            btnRestart.SetValue(VisibilityProperty, Visibility.Visible);
            btnRegret.SetValue(VisibilityProperty, Visibility.Visible);
            // redraw all the pieces in the board on the panel
            redrawPieces();
        }

        private void btnRegret_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Display the chances left for regret
            GameRules.regret();
            redrawCurrenColour();
            redrawPieces();
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            // Re-initialization of the game data only
            GameRules.iniGame();
            // Re-draw the current colour
            redrawCurrenColour();
            // Re-draw all the pieces in the board on the panel
            redrawPieces();
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int imgRow = (int)(((Image)sender).GetValue(chessBoardRowProperty));
            int imgCol = (int)(((Image)sender).GetValue(chessBoardColProperty));

            txtblkMessage.Text = "Click location: " + imgRow.ToString() + "," + imgCol.ToString();

            try
            {
                switch (gameStatus)
                {
                    case GameStatus.TO_CHOOSE:
                        PiecesHandler.chooseOri(imgRow, imgCol);
                        drawValidMove();
                        changeGameStatus(GameStatus.TO_MOVE);
                        break;
                    case GameStatus.TO_MOVE:
                        // TODO: If the player clicks another piece in his own team, consider he try to change another piece to move
                        if (Board.pieces[imgRow, imgCol] != null && Board.pieces[imgRow, imgCol].colour == Board.currentColour % 2)
                        {
                            redrawPieces();
                            PiecesHandler.chooseOri(imgRow, imgCol);
                            drawValidMove();
                            break;
                        }
                        else
                        {
                            PiecesHandler.chooseDest(imgRow, imgCol);
                            changeGameStatus(GameStatus.TO_CHOOSE);
                            redrawCurrenColour();
                            redrawPieces();
                            break;
                        }
                        
                }
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message, "Invalid choice");
                changeGameStatus(GameStatus.TO_CHOOSE);
                redrawPieces();
            }
        }
    }
}
