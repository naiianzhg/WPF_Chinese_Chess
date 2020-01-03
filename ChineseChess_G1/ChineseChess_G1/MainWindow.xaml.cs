using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
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
        TO_MOVE,
        GAME_OVER
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

        // String to store manual
        public string manual;

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
                // If the original location of last move of the other team is in the valid move, fullfill with the origin location box
                // When it is the first round, there was no move before to indicate
                // after, the ancien move which need to be indicated will be the SECOND LAST location in the last ori location lost, the first last location is the cliking location
                if (Board.lastOriLocationList.Count > 1 && validMove == Board.lastOriLocationList[Board.lastOriLocationList.Count - 2])
                {
                    validMoveImg.Source = new BitmapImage(new Uri("/Images/OriLocationBox.png", UriKind.RelativeOrAbsolute));
                    continue;
                }
                // If there is a piece in the valid move position, the piece will become transparent
                if (Board.pieces[validMove / 10, validMove % 10] != null) validMoveImg.Opacity = 0.4;
                // If the valid move position has no piece we put a ValidMoveBox
                else validMoveImg.Source = new BitmapImage(new Uri("/Images/ValidMoveBox.png", UriKind.RelativeOrAbsolute));
            }
        }

        // Indicate the current team colour and round
        private void redrawCurrenColour()
        {
            txtblkRound.Text = $"Round {Board.currentColour}";
            txtblkRound.SetValue(VisibilityProperty, Visibility.Visible);
            if (Board.currentColour % 2 == 0)
            {
                txtblkCrrClr.Text = "Black's Move";
                txtblkCrrClr.Background = Brushes.Black;
                txtblkCrrClr.SetValue(VisibilityProperty, Visibility.Visible);
            }
            else
            {
                txtblkCrrClr.Text = "Red's Move";
                txtblkCrrClr.Background = Brushes.Red;
                txtblkCrrClr.SetValue(VisibilityProperty, Visibility.Visible);
            }
        }

        // Start button click event handle
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // Initialization of all the components of the game
            IniGame();
            // draw the current colour
            redrawCurrenColour();
            // Re-draw the chances left on the regret button, if currently it is black's turn, the regret chance will be red's
            btnRegret.Content =
                Board.currentColour % 2 == 0 ?
                $"Red's Regret({Board.regretAmount[Board.currentColour % 2]})" :
                $"Black's Regret({Board.regretAmount[Board.currentColour % 2]})";
            // apprear the current colour textblock, regret button, restart button
            // as well as disappear the start button
            btnStart.SetValue(VisibilityProperty, Visibility.Collapsed);
            btnRestart.SetValue(VisibilityProperty, Visibility.Visible);
            btnRegret.SetValue(VisibilityProperty, Visibility.Visible);
            // redraw all the pieces in the board on the panel
            redrawPieces();
        }

        // Restart button click event handle
        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Restart Game", MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                changeGameStatus(GameStatus.TO_CHOOSE);
                chessPanel.Cursor = Cursors.Arrow;
                // Re-initialization of the game data only
                GameRules.iniGame();
                // Re-draw the current colour
                redrawCurrenColour();
                // Re-draw the chances left on the regret button, if currently it is black's turn, the regret chance will be red's
                btnRegret.Content =
                    Board.currentColour % 2 == 0 ?
                    $"Red's Regret({Board.regretAmount[Board.currentColour % 2]})" :
                    $"Black's Regret({Board.regretAmount[Board.currentColour % 2]})";
                // Re-draw all the pieces in the board on the panel
                redrawPieces();
            }
        }

        // Regret button click event handle
        private void btnRegret_Click(object sender, RoutedEventArgs e)
        {
            if (gameStatus == GameStatus.TO_MOVE) btnRegret.Cursor = Cursors.No;
            else
            {
                btnRegret.Cursor = Cursors.Arrow;
                if (MessageBox.Show("Are you sure?", "Regret Chess", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        GameRules.regret();
                        // If the player regrets from the checkmate situation, change the cursor back to defaut
                        chessPanel.Cursor = Cursors.Arrow;
                        // And set game status to TO_CHOOSE
                        changeGameStatus(GameStatus.TO_CHOOSE);

                        redrawCurrenColour();
                        // Re-draw the chances left on the regret button, if currently it is black's turn, the regret chance will be red's
                        btnRegret.Content =
                            Board.currentColour % 2 == 0 ?
                            $"Red's Regret({Board.regretAmount[Board.currentColour % 2]})" :
                            $"Black's Regret({Board.regretAmount[Board.currentColour % 2]})";
                        redrawPieces();
                        // Redraw the origin location indication
                        if (Board.lastOriLocationList.Count > 0)
                        {
                            Image oriLocationImg = (Image)chessPanel.Children[9 * Board.getLastOriLocation()[0] + Board.getLastOriLocation()[1]];
                            oriLocationImg.Source = new BitmapImage(new Uri("/Images/OriLocationBox.png", UriKind.RelativeOrAbsolute));
                        }
                    }
                    catch (Exception excp)
                    {
                        MessageBox.Show(excp.Message, "Invalid operation", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // Pieces/Valid move images click event handle
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
                        // If the player clicks another piece in his own team, consider he try to change another piece to move
                        if (Board.pieces[imgRow, imgCol] != null && Board.pieces[imgRow, imgCol].colour == Board.currentColour % 2)
                        {
                            // Changing chosen piece does not count as the valid last ori location
                            Board.removeLastOriLocation();
                            redrawPieces();
                            // Indicate the origin location with a blue box EXCEPT there is no move yet
                            if (Board.lastOriLocationList.Count > 0)
                            {
                                Image oriLocationImg = (Image)chessPanel.Children[9 * Board.getLastOriLocation()[0] + Board.getLastOriLocation()[1]];
                                oriLocationImg.Source = new BitmapImage(new Uri("/Images/OriLocationBox.png", UriKind.RelativeOrAbsolute));
                            }
                            PiecesHandler.chooseOri(imgRow, imgCol);
                            drawValidMove();
                            break;
                        }
                        else
                        {
                            PiecesHandler.chooseDest(imgRow, imgCol);
                            redrawCurrenColour();
                            // Re-draw the chances left on the regret button, if currently it is black's turn, the regret chance will be red's
                            btnRegret.Content =
                                Board.currentColour % 2 == 0 ?
                                $"Red's Regret({Board.regretAmount[Board.currentColour % 2]})" :
                                $"Black's Regret({Board.regretAmount[Board.currentColour % 2]})";
                            redrawPieces();
                            // Indicate the origin location with a blue box
                            Image oriLocationImg = (Image)chessPanel.Children[9 * Board.getLastOriLocation()[0] + Board.getLastOriLocation()[1]];
                            oriLocationImg.Source = new BitmapImage(new Uri("/Images/OriLocationBox.png", UriKind.RelativeOrAbsolute));

                            // If this move cause a CHECKMATE
                            if (GameRules.isCheckmate())
                            {
                                if (Board.currentColour % 2 == 0) MessageBox.Show("Red wins", "Congratulations", MessageBoxButton.OK, MessageBoxImage.Information);
                                else if (Board.currentColour % 2 == 1) MessageBox.Show("Black wins", "Congratulations", MessageBoxButton.OK, MessageBoxImage.Information);
                                changeGameStatus(GameStatus.GAME_OVER);
                                break;
                            }

                            // If this move cause a CHECK
                            bool[] check = GameRules.isChecked();
                            if (check[0]) MessageBox.Show("Black is checked", "Danger", MessageBoxButton.OK, MessageBoxImage.Warning);
                            else if (check[1]) MessageBox.Show("Red is checked", "Danger", MessageBoxButton.OK, MessageBoxImage.Warning);
                            changeGameStatus(GameStatus.TO_CHOOSE);

                            break;
                        }
                    case GameStatus.GAME_OVER:
                        chessPanel.Cursor = Cursors.No;
                        break;
                }
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message, "Invalid move", MessageBoxButton.OK, MessageBoxImage.Error);
                changeGameStatus(GameStatus.TO_CHOOSE);
                redrawPieces();
                // Indicate the origin location with a blue box
                if (Board.lastOriLocationList.Count > 0)
                {
                    Image oriLocationImg = (Image)chessPanel.Children[9 * Board.getLastOriLocation()[0] + Board.getLastOriLocation()[1]];
                    oriLocationImg.Source = new BitmapImage(new Uri("/Images/OriLocationBox.png", UriKind.RelativeOrAbsolute));
                }
            }
        }

        // TODO
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This operation will clear the current game, are you sure?", "Restart Game", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (manual == null) MessageBox.Show("You did not import any chess manual", "Invalid operation", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    // Re-initialization of the game data only
                    GameRules.iniGame();
                    // Re-draw the current colour
                    redrawCurrenColour();
                    // Re-draw the chances left on the regret button, if currently it is black's turn, the regret chance will be red's
                    btnRegret.Content =
                        Board.currentColour % 2 == 0 ?
                        $"Red's Regret({Board.regretAmount[Board.currentColour % 2]})" :
                        $"Black's Regret({Board.regretAmount[Board.currentColour % 2]})";
                    // Re-draw all the pieces in the board on the panel
                    redrawPieces();
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = @"c:\Users\";
            if (openFileDialog.ShowDialog() == true)
            {
                txtblkUrl.SetValue(VisibilityProperty, Visibility.Visible);
                txtblkUrl.Text = openFileDialog.FileName;
                manual = File.ReadAllText(openFileDialog.FileName);
                // Read chess manual
                PiecesHandler.readManual(manual);
            }
        }
    }
}
