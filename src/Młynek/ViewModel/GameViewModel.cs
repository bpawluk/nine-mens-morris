using Młynek.Model;
using Młynek.Players;
using Młynek.Utils;
using System.ComponentModel;
using System.Windows.Input;

namespace Młynek.ViewModel
{
    class GameViewModel : INotifyPropertyChanged
    {
        private ICommand _startGameCmd;
        private IPlayer _whitePlayer;
        private IPlayer _blackPlayer;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand StartGameCmd
        {
            get
            {
                if (_startGameCmd == null) _startGameCmd = new Command(StartGame);
                return _startGameCmd;
            }
        }
        public Game Game { get; private set; }
        public HumanPlayerViewModel HumanPlayerViewModel { get; private set; }
        public PlayerConfigViewModel WhiteConfig { get; private set; }
        public PlayerConfigViewModel BlackConfig { get; private set; }
        public StatisticsViewModel WhiteStats { get; private set; }
        public StatisticsViewModel BlackStats { get; private set; }

        public string WhiteName { get => _whitePlayer?.TypeName.ToUpper() ?? "N/A"; }
        public string BlackName { get => _blackPlayer?.TypeName.ToUpper() ?? "N/A"; }


        public GameViewModel()
        {
            Game = new Game();
            HumanPlayerViewModel = new HumanPlayerViewModel();
            WhiteConfig = new PlayerConfigViewModel();
            BlackConfig = new PlayerConfigViewModel();
            WhiteStats = new StatisticsViewModel();
            BlackStats = new StatisticsViewModel();
        }

        private void StartGame(object parameter)
        {
            InstantiateGame();
            Game.Start();
        }

        private void OnUpdateStats(object sender, Move move)
        {
            if (move.Player == FieldState.Black)
            {
                if (move.To < 0) BlackStats.AddCapture(move);
                else BlackStats.AddMove(move);
                NotifyPropertyChanged("BlackStats");
            }
            else if (move.Player == FieldState.White)
            {
                if (move.To < 0) WhiteStats.AddCapture(move);
                else WhiteStats.AddMove(move);
                NotifyPropertyChanged("WhiteStats");
            }
        }

        private void OnCaptureChosen(object sender, Move move) => Game.Capture(move);

        private void OnMoveChosen(object sender, Move move) => Game.MakeMove(move);
        
        private void OnCapture(object sender, FieldState player)
        {
            Game game = sender as Game;
            if (game.NextPlayer == FieldState.Black)
            {
                if (_blackPlayer.IsHuman) _blackPlayer.Capture(game);
                else
                {
                    Move move = _blackPlayer.AICapture(game);
                    game.Capture(move);
                }
            }
            else
            {
                if (_whitePlayer.IsHuman) _whitePlayer.Capture(game);
                else
                {
                    Move move = _whitePlayer.AICapture(game);
                    game.Capture(move);
                }
            }
        }

        private void OnNextPlayersTurn(object sender, FieldState player) => AwaitMove(sender as Game);

        private async void AwaitMove(Game game)
        {
            if (game.NextPlayer == FieldState.Black)
            {
                if (_blackPlayer.IsHuman) _blackPlayer.Move(game);
                else
                {
                    Move move = await _blackPlayer.AIMove(game);
                    game.MakeMove(move);
                }
            }
            else
            {
                if (_whitePlayer.IsHuman) _whitePlayer.Move(game);
                else
                {
                    Move move = await _whitePlayer.AIMove(game);
                    game.MakeMove(move);
                }
            }
        }

        private void OnGameEnded(object sender, FieldState winner)
        {
            HumanPlayerViewModel.InputEnabled = false;
            if(winner == FieldState.Empty) HumanPlayerViewModel.Message = $"IT'S A DRAW!";
            else HumanPlayerViewModel.Message = $"{winner.ToString().ToUpper()} WINS!";
        }

        private void InstantiateGame()
        {
            Game.CaptureEvent -= OnCapture;
            Game.NextPlayersTurnEvent -= OnNextPlayersTurn;
            Game.GameEndedEvent -= OnGameEnded;
            Game.UpdateStatsEvent -= OnUpdateStats;

            Game = new Game();
            Game.CaptureEvent += OnCapture;
            Game.NextPlayersTurnEvent += OnNextPlayersTurn;
            Game.GameEndedEvent += OnGameEnded;
            Game.UpdateStatsEvent += OnUpdateStats;

            NotifyPropertyChanged("Game");

            if(WhiteConfig.IsHuman)
            {
                _whitePlayer = new HumanPlayer(FieldState.White, HumanPlayerViewModel);
                (_whitePlayer as HumanPlayer).MoveChosenEvent += OnMoveChosen;
                (_whitePlayer as HumanPlayer).CaptureChosenEvent += OnCaptureChosen;
            }
            else
            {
                GameStateEvaluator whiteEvaluator = new GameStateEvaluator(WhiteConfig.PositionsWeight, WhiteConfig.RowsWeight, WhiteConfig.ColumnsWeight, WhiteConfig.MovesWeight, WhiteConfig.PawnsWeight);
                if (WhiteConfig.AlfaBetaEnabled) _whitePlayer = new AlfaBetaPlayer(FieldState.White, WhiteConfig.TreeDepth, whiteEvaluator);
                else _whitePlayer = new MinMaxPlayer(FieldState.White, WhiteConfig.TreeDepth, whiteEvaluator);
            }

            if (BlackConfig.IsHuman)
            {
                _blackPlayer = new HumanPlayer(FieldState.Black, HumanPlayerViewModel);
                (_blackPlayer as HumanPlayer).MoveChosenEvent += OnMoveChosen;
                (_blackPlayer as HumanPlayer).CaptureChosenEvent += OnCaptureChosen;
            }
            else
            {
                GameStateEvaluator blackEvaluator = new GameStateEvaluator(BlackConfig.PositionsWeight, BlackConfig.RowsWeight, BlackConfig.ColumnsWeight, BlackConfig.MovesWeight, BlackConfig.PawnsWeight);
                if (BlackConfig.AlfaBetaEnabled) _blackPlayer = new AlfaBetaPlayer(FieldState.Black, BlackConfig.TreeDepth, blackEvaluator);
                else _blackPlayer = new MinMaxPlayer(FieldState.Black, BlackConfig.TreeDepth, blackEvaluator);
            }

            WhiteStats = new StatisticsViewModel();
            BlackStats = new StatisticsViewModel();

            HumanPlayerViewModel.Message = "";

            NotifyPropertyChanged("WhiteStats");
            NotifyPropertyChanged("BlackStats");

            NotifyPropertyChanged("WhiteName");
            NotifyPropertyChanged("BlackName");
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}