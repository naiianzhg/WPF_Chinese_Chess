﻿using System;
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
using ChineseChess.Control;
using System.Windows.Threading;
using System.Threading;

namespace ChineseChess_G1
{
    enum GameStatus
    {
        TO_CHOOSE,
        TO_MOVE,
        GAME_OVER,
        MANUAL_MODE
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

        // MediaPlayer to play sound effect
        private MediaPlayer mediaPlayer = new MediaPlayer();

        // for the timeCounter
        private DispatcherTimer timeCounter = new DispatcherTimer();
        int remainTime;

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
                if (piece != null) { pieceImg.Source = new BitmapImage(new Uri(piece.imageUrl, UriKind.RelativeOrAbsolute)); pieceImg.Opacity = 1; }
                // if not, draw null image on the panel
                else pieceImg.Source = null;
                i++;
            }
        }

        // Draw the valid moves of the chosen piece
        private void drawValidMove()
        {
            // calculate the valid move of the chosen piece
            List<int> validMoves = Board.pieces[Board.getLastOriLocation()[0], Board.getLastOriLocation()[1]].calculateValidMoveList(Board.getLastOriLocation());
            foreach (int validMove in validMoves)
            {
                // the valid move location (x,y) is corresponding to the index 9*x+y of the chess panel's grid
                Image validMoveImg = (Image)chessPanel.Children[9 * (validMove / 10) + validMove % 10];
                // If the original location of last move of the other team is in the valid move, fullfill with the origin location box
                // When it is the first round, there was no move before to indicate
                // after, the ancien move which need to be indicated will be the SECOND LAST location in the last ori location lost, the first last location is the cliking location
                if (Board.lastOriLocationList.Count > 1 && validMove == Board.lastOriLocationList[Board.lastOriLocationList.Count - 2])
                {
                    validMoveImg.Source = new BitmapImage(new Uri("Resources/OriLocationBox.png", UriKind.RelativeOrAbsolute));
                    continue;
                }
                // If there is a piece in the valid move position, the piece will become transparent
                if (Board.pieces[validMove / 10, validMove % 10] != null) validMoveImg.Opacity = 0.4;
                // If the valid move position has no piece we put a ValidMoveBox
                else validMoveImg.Source = new BitmapImage(new Uri("Resources/ValidMoveBox.png", UriKind.RelativeOrAbsolute));
            }
        }

        // Indicate the current team colour and round
        private void redrawCurrenColour()
        {
            txtblkRound.Text = $"Round {Board.currentColour}";
            txtblkRound.SetValue(VisibilityProperty, Visibility.Visible);
            txtblkCrrClr.SetValue(VisibilityProperty, Visibility.Visible);
            if (Board.currentColour % 2 == 0)
            {
                TimeCounter();
                txtblkCrrClr.Text = "Black's Move";
                txtblkCrrClr.Background = Brushes.Black;
            }
            else
            {
                TimeCounter();
                txtblkCrrClr.Text = "Red's Move";
                txtblkCrrClr.Background = Brushes.Red;
            }
        }

        // Start button click event handle
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // Initialization of all the components of the game
            IniGame();
            // draw the current colour
            redrawCurrenColour();
            // appear the withdraw button, regret button, restart button as well as disappear the start button
            btnStart.SetValue(VisibilityProperty, Visibility.Collapsed);
            btnRestart.SetValue(VisibilityProperty, Visibility.Visible);
            btnRegret.SetValue(VisibilityProperty, Visibility.Visible);

            // Play manual button content
            btnPlay.IsEnabled = false;
            btnPlay.Content = "Load Manual";

            // Re-draw the chances left on the regret button
            btnRegret.Content =
                Board.currentColour % 2 == 0 ?
                $"Regret of Black({Board.regretAmount[Board.currentColour % 2]})" :
                $"Regret of Red({Board.regretAmount[Board.currentColour % 2]})";

            // appear the manual demo mode options
            ManualDemo.SetValue(VisibilityProperty, Visibility.Visible);
            // appear the game message
            Message.SetValue(VisibilityProperty, Visibility.Visible);
            // redraw all the pieces in the board on the panel
            redrawPieces();
        }

        // Restart button click event handle
        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            timeCounter.Stop();
            if (MessageBox.Show("Are you sure?", "Restart Game", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                changeGameStatus(GameStatus.TO_CHOOSE);

                URL.SetValue(VisibilityProperty, Visibility.Collapsed);
                Message.SetValue(VisibilityProperty, Visibility.Visible);
                // pause the win sound
                mediaPlayer.Pause();

                // Enable buttons
                chessPanel.Cursor = Cursors.Arrow;
                btnRegret.Cursor = Cursors.Arrow;

                GameRules.iniGame();

                // Re-set the manual demo mode
                manual = null;
                txtblkUrl.SetValue(VisibilityProperty, Visibility.Collapsed);
                // Re-draw the current colour
                redrawCurrenColour();
                // Re-draw the chances left on the regret button
                btnRegret.Content =
                    Board.currentColour % 2 == 0 ?
                    $"Regret of Black({Board.regretAmount[Board.currentColour % 2]})" :
                    $"Regret of Red({Board.regretAmount[Board.currentColour % 2]})";
                // Re-draw all the pieces in the board on the panel
                redrawPieces();
            }
            else { if (gameStatus != GameStatus.MANUAL_MODE) timeCounter.Start(); }
        }

        // Withdraw button click event handle
        private void btnWithdraw_Click(object sender, RoutedEventArgs e)
        {
            if (gameStatus == GameStatus.TO_CHOOSE || gameStatus == GameStatus.GAME_OVER)
            {
                if (MessageBox.Show("Are you sure?", "Withdraw Move", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        PiecesHandler.withdraw();
                        // Set game status to TO_CHOOSE
                        changeGameStatus(GameStatus.TO_CHOOSE);
                        // If the player regrets from the checkmate situation, change the cursor back to defaut
                        chessPanel.Cursor = Cursors.Arrow;
                        redrawCurrenColour();
                        redrawPieces();
                        // Redraw the origin location indication
                        if (Board.lastOriLocationList.Count > 0)
                        {
                            Image oriLocationImg = (Image)chessPanel.Children[9 * Board.getLastOriLocation()[0] + Board.getLastOriLocation()[1]];
                            oriLocationImg.Source = new BitmapImage(new Uri("Resources/OriLocationBox.png", UriKind.RelativeOrAbsolute));
                        }
                    }
                    catch (Exception excp)
                    {
                        MessageBox.Show(excp.Message, "Invalid operation", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnRegret_Click(object sender, RoutedEventArgs e)
        {
            if (gameStatus != GameStatus.MANUAL_MODE)
            {
                timeCounter.Stop();
                if (MessageBox.Show("Are you sure?", "Regret Move", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (gameStatus == GameStatus.TO_MOVE) Board.removeLastOriLocation();
                        PiecesHandler.regret();
                        // Set game status to TO_CHOOSE
                        changeGameStatus(GameStatus.TO_CHOOSE);
                        // If the player regrets from the checkmate situation, change the cursor back to defaut
                        chessPanel.Cursor = Cursors.Arrow;
                        redrawCurrenColour();
                        // Re-draw the chances left on the regret button
                        btnRegret.Content =
                            Board.currentColour % 2 == 0 ?
                            $"Regret of Black({Board.regretAmount[Board.currentColour % 2]})" :
                            $"Regret of Red({Board.regretAmount[Board.currentColour % 2]})";
                        redrawPieces();
                        // Redraw the origin location indication
                        if (Board.lastOriLocationList.Count > 0)
                        {
                            Image oriLocationImg = (Image)chessPanel.Children[9 * Board.getLastOriLocation()[0] + Board.getLastOriLocation()[1]];
                            oriLocationImg.Source = new BitmapImage(new Uri("Resources/OriLocationBox.png", UriKind.RelativeOrAbsolute));
                        }
                    }
                    catch (Exception excp)
                    {
                        MessageBox.Show(excp.Message, "Invalid operation", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else timeCounter.Start();
            }
        }

        // Pieces/Valid move images click event handle
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int imgRow = (int)(((Image)sender).GetValue(chessBoardRowProperty));
            int imgCol = (int)(((Image)sender).GetValue(chessBoardColProperty));

            txtblkMessage.Text = "Click Location: " + imgRow.ToString() + "," + imgCol.ToString();

            try
            {
                switch (gameStatus)
                {
                    case GameStatus.TO_CHOOSE:
                        PiecesHandler.chooseOri(imgRow, imgCol);
                        // Choose sound effect
                        mediaPlayer.Open(new Uri(Board.pieces[imgRow, imgCol].chooseSoundUrl, UriKind.RelativeOrAbsolute));
                        mediaPlayer.Play();
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
                                oriLocationImg.Source = new BitmapImage(new Uri("Resources/OriLocationBox.png", UriKind.RelativeOrAbsolute));
                            }
                            PiecesHandler.chooseOri(imgRow, imgCol);
                            // Choose sound effect
                            mediaPlayer.Open(new Uri(Board.pieces[imgRow, imgCol].chooseSoundUrl, UriKind.RelativeOrAbsolute));
                            mediaPlayer.Play();
                            drawValidMove();
                            break;
                        }
                        else
                        {
                            PiecesHandler.chooseDest(imgRow, imgCol);
                            // Move sound effect
                            mediaPlayer.Open(new Uri(Board.pieces[imgRow, imgCol].moveSoundUrl, UriKind.RelativeOrAbsolute));
                            mediaPlayer.Play();
                            redrawCurrenColour();
                            // Re-draw the chances left on the regret button
                            btnRegret.Content =
                                Board.currentColour % 2 == 0 ?
                                $"Regret of Black({Board.regretAmount[Board.currentColour % 2]})" :
                                $"Regret of Red({Board.regretAmount[Board.currentColour % 2]})";
                            redrawPieces();
                            // Indicate the origin location with a blue box
                            Image oriLocationImg = (Image)chessPanel.Children[9 * Board.getLastOriLocation()[0] + Board.getLastOriLocation()[1]];
                            oriLocationImg.Source = new BitmapImage(new Uri("Resources/OriLocationBox.png", UriKind.RelativeOrAbsolute));

                            // If this move cause a CHECKMATE
                            if (GameRules.isCheckmate())
                            {
                                if (Board.currentColour % 2 == 0) MessageBox.Show("Red wins", "Congratulations", MessageBoxButton.OK, MessageBoxImage.Information);
                                else if (Board.currentColour % 2 == 1) MessageBox.Show("Black wins", "Congratulations", MessageBoxButton.OK, MessageBoxImage.Information);
                                changeGameStatus(GameStatus.GAME_OVER);
                                // Win sound effect
                                mediaPlayer.Open(new Uri("Resources/win.mp3", UriKind.RelativeOrAbsolute));
                                mediaPlayer.Play();
                                timeCounter.Stop();
                                btnRegret.Cursor = Cursors.Arrow;
                                chessPanel.Cursor = Cursors.No;
                                break;
                            }

                            // If this move cause a CHECK
                            if (GameRules.isChecked())
                            {
                                if (Board.currentColour % 2 == 0) MessageBox.Show("Black is checked", "Danger", MessageBoxButton.OK, MessageBoxImage.Warning);
                               else if (Board.currentColour % 2 == 1) MessageBox.Show("Red is checked", "Danger", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            changeGameStatus(GameStatus.TO_CHOOSE);
                            btnRegret.Cursor = Cursors.Arrow;
                            break;
                        }
                }
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message, "Invalid move", MessageBoxButton.OK, MessageBoxImage.Error);
                changeGameStatus(GameStatus.TO_CHOOSE);
                btnRegret.Cursor = Cursors.Arrow;
                redrawPieces();
                // Indicate the origin location with a blue box
                if (Board.lastOriLocationList.Count > 0)
                {
                    Image oriLocationImg = (Image)chessPanel.Children[9 * Board.getLastOriLocation()[0] + Board.getLastOriLocation()[1]];
                    oriLocationImg.Source = new BitmapImage(new Uri("Resources/OriLocationBox.png", UriKind.RelativeOrAbsolute));
                }
            }
        }

        // Suspension
        async Task putPlayTaskDelay()
        {
            await Task.Delay(800);
        }
        private async void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            timeCounter.Stop();

            // disable play button
            btnPlay.IsEnabled = false;
            btnPlay.Content = "Playing..";
            // disable open button
            btnOpen.IsEnabled = false;
            btnOpen.Content = "Waiting..";

            int[] manualOriLocation = new int[2], manualDestLocation = new int[2];
            // Automatical move
            for (int i = 0; i < Board.manualOriLocationList.Count; i++)
            {
                manualOriLocation[0] = Board.manualOriLocationList[i] / 10; manualOriLocation[1] = Board.manualOriLocationList[i] % 10;
                manualDestLocation[0] = Board.manualDestLocationList[i] / 10; manualDestLocation[1] = Board.manualDestLocationList[i] % 10;
                // Save this chosen original location as last original location
                Board.addLastOriLocation(manualOriLocation);

                // Choose simulation
                // Choose sound effect
                mediaPlayer.Open(new Uri(Board.pieces[manualOriLocation[0], manualOriLocation[1]].chooseSoundUrl, UriKind.RelativeOrAbsolute));
                mediaPlayer.Play();
                drawValidMove();
                await putPlayTaskDelay();

                if (gameStatus == GameStatus.TO_CHOOSE) break;

                // Move sound effect
                mediaPlayer.Open(new Uri(Board.pieces[manualOriLocation[0], manualOriLocation[1]].moveSoundUrl, UriKind.RelativeOrAbsolute));
                mediaPlayer.Play();
                PiecesHandler.moveTo(manualOriLocation, manualDestLocation);
                redrawPieces();
                // Indicate the origin location with a blue box
                if (Board.lastOriLocationList.Count > 0)
                {
                    Image oriLocationImg = (Image)chessPanel.Children[9 * Board.getLastOriLocation()[0] + Board.getLastOriLocation()[1]];
                    oriLocationImg.Source = new BitmapImage(new Uri("Resources/OriLocationBox.png", UriKind.RelativeOrAbsolute));
                }
                await putPlayTaskDelay();
            }

            btnPlay.Content = "Reload Maunal";
            //Enable open button
            btnOpen.IsEnabled = true;
            btnOpen.Content = "Open Manual";
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            timeCounter.Stop();
            if (MessageBox.Show("This operation will clear the current game, are you sure?", "Manual demo mode", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Manual text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.InitialDirectory = @"c:\Users\";
                if (openFileDialog.ShowDialog() == true)
                {
                    // RE-INITIALIZATION CHESS BOARD
                    // Re-initialization of the game data only
                    GameRules.iniGame();
                    // Re-draw all the pieces in the board on the panel
                    redrawPieces();

                    URL.SetValue(VisibilityProperty, Visibility.Visible);
                    Message.SetValue(VisibilityProperty, Visibility.Collapsed);
                    timeCounter.Stop();
                    changeGameStatus(GameStatus.MANUAL_MODE);

                    // Enable play button
                    btnPlay.IsEnabled = true;
                    btnPlay.Cursor = Cursors.Arrow;
                    btnPlay.Content = "Play Manual";

                    // Disable panel
                    chessPanel.Cursor = Cursors.No;
                    // Disable function buttons
                    btnRegret.Cursor = Cursors.No;

                    txtblkUrl.SetValue(VisibilityProperty, Visibility.Visible);
                    txtblkUrl.Text = openFileDialog.FileName;
                    manual = File.ReadAllText(openFileDialog.FileName);
                    try
                    {
                        // Read the moves from chess manual, stored in Board
                        Board.readManual(manual);
                    }
                    catch (Exception)
                    {
                        changeGameStatus(GameStatus.TO_CHOOSE);
                        // Enable panel
                        chessPanel.Cursor = Cursors.Arrow;
                        // Enable function buttons
                        btnRegret.Cursor = Cursors.Arrow;

                        // Play manual button content
                        btnPlay.IsEnabled = false;
                        btnPlay.Content = "Load Manual";

                        MessageBox.Show("Please open the right manual file", "Incorrect manual", MessageBoxButton.OK, MessageBoxImage.Error);
                        manual = null;
                        URL.SetValue(VisibilityProperty, Visibility.Collapsed);
                        Message.SetValue(VisibilityProperty, Visibility.Visible);
                        timeCounter.Start();
                    }
                }
                else
                {
                    if (gameStatus != GameStatus.MANUAL_MODE) timeCounter.Start();
                }
            }
            else { if (gameStatus != GameStatus.MANUAL_MODE) timeCounter.Start(); }
        }

        private void TimeCounter()
        {
            remainTime = 30;
            // Remove the last event or the event will be repeatly handled
            timeCounter.Tick -= timer_Tick;
            //for initial the counter value
            txtblkTimer.FontSize = 20;
            TimerR.Padding = new Thickness(0, 1, 0, 0);
            TimerS.Padding = new Thickness(0, 1, 0, 0);
            txtblkTimer.Foreground = Brushes.Black;
            txtblkTimer.Text = $"{remainTime}";
            //set the span of the counter as 1 second
            timeCounter.Interval = TimeSpan.FromSeconds(1);
            //every span (per second) will execut the timer_Tick event
            timeCounter.Tick += timer_Tick;
            if (gameStatus != GameStatus.MANUAL_MODE) timeCounter.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            remainTime--;
            txtblkTimer.Text = $"{remainTime}";

            //3 seconds before time out 
            if (remainTime < 3)
            {
                if (remainTime == 2)
                {
                    mediaPlayer.Open(new Uri("Resources/Timeout.mp3", UriKind.Relative));
                    mediaPlayer.Play();
                }
                TimerR.Padding= new Thickness(0,11,0,0);
                TimerS.Padding= new Thickness(0,11,0,0);
                txtblkTimer.FontSize = 30;
                txtblkTimer.Foreground = Brushes.Red;
                //Time out
                if (remainTime == 0)
                {
                    try
                    {
                        PiecesHandler.randomMove();
                        mediaPlayer.Open(new Uri(Board.pieces[Board.getLastDestLocation()[0], Board.getLastDestLocation()[1]].moveSoundUrl, UriKind.RelativeOrAbsolute));
                        mediaPlayer.Play();
                        redrawPieces();
                        Image oriLocationImg = (Image)chessPanel.Children[9 * Board.getLastOriLocation()[0] + Board.getLastOriLocation()[1]];
                        oriLocationImg.Source = new BitmapImage(new Uri("Resources/OriLocationBox.png", UriKind.RelativeOrAbsolute));
                        redrawCurrenColour();
                        changeGameStatus(GameStatus.TO_CHOOSE);
                        btnRegret.Cursor = Cursors.Arrow;
                    }
                    catch (Exception)
                    {
                        PiecesHandler.moveTo(Board.getLastOriLocation(), Board.getLastDestLocation());
                        mediaPlayer.Open(new Uri(Board.pieces[Board.getLastDestLocation()[0], Board.getLastDestLocation()[1]].moveSoundUrl, UriKind.RelativeOrAbsolute));
                        mediaPlayer.Play();
                        redrawPieces();
                        Image oriLocationImg = (Image)chessPanel.Children[9 * Board.getLastOriLocation()[0] + Board.getLastOriLocation()[1]];
                        oriLocationImg.Source = new BitmapImage(new Uri("Resources/OriLocationBox.png", UriKind.RelativeOrAbsolute));
                        redrawCurrenColour();
                        MessageBox.Show("You lost because of random move!", "GAME OVER", MessageBoxButton.OK, MessageBoxImage.Information);
                        changeGameStatus(GameStatus.GAME_OVER);

                    }                  

                    MessageBox.Show("Your time ran out, the system play a random move for you", "Time out remainder", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }
        }
    }
}
