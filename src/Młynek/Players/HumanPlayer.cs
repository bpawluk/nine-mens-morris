using Młynek.Model;
using Młynek.ViewModel;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Młynek.Players
{
    class HumanPlayer : IPlayer
    {
        private Stopwatch _timer;
        private HumanPlayerViewModel _humanPlayerViewModel;
        private Game _game;
        private FieldState _color;
        private Move _nextMove;

        public event EventHandler<Move> MoveChosenEvent;
        public event EventHandler<Move> CaptureChosenEvent;

        public bool IsHuman => true;
        public string TypeName => "Player";

        public HumanPlayer(FieldState color, HumanPlayerViewModel humanPlayerViewModel)
        {
            _color = color;
            _humanPlayerViewModel = humanPlayerViewModel;
        }

        public void Move(Game game)
        {
            _nextMove = new Move(_color);

            // CHECK WHETHER PLAYER CAN MOVE
            if (game.GetPawnsInHand(_color) <= 0 && game.GameBoard.GetAvaiableMoves(_color, _game.GameHistory.PeekLastMove(_color, true), game.GameBoard.GetFieldsCount(_color) < 4).Length == 0) MoveChosenEvent?.Invoke(this, _nextMove);

            // AWAIT MOVE
            else
            {
                _game = game;
                _humanPlayerViewModel.BeginInteraction($"{_color.ToString().ToUpper()}'S TURN", FieldChosen);
                _timer = Stopwatch.StartNew();
            }
        }

        public void Capture(Game game)
        {
            _nextMove = new Move(_color);
            _game = game;
            _humanPlayerViewModel.BeginInteraction($"{_color.ToString().ToUpper()}'S TURN: CHOOSE PAWN TO CAPTURE", CaptureChosen);
            _timer = Stopwatch.StartNew();
        }

        private void FieldChosen(int coords)
        {
            //IF FIRST PHASE OF GAME
            if (_game.GetPawnsInHand(_color) > 0)
            {
                PlacePawn(coords);
            }
            // CHOOSE PAWN TO MOVE
            else if (_nextMove.From < 0)
            {
                ChoosePawn(coords);
            }
            // PAWN ALREADY CHOSEN
            else
            {
                //CHOSEN PAWN CHANGED
                if (_game.GameBoard.GetField(coords).State == _color) _nextMove.From = coords;

                //OCCUPIED DESTINATION CHOSEN
                else if (_game.GameBoard.GetField(coords).State != FieldState.Empty) _humanPlayerViewModel.Message = $"{_color.ToString().ToUpper()}'S TURN: CHOOSE FIELD THAT IS NOT OCCUPIED";

                //CHOOSE DESTINATION
                else
                {
                    ChooseDestination(coords);
                }
            }
        }

        private void PlacePawn(int coords)
        {
            if (_game.GameBoard.GetField(coords).State != FieldState.Empty) _humanPlayerViewModel.Message = $"{_color.ToString().ToUpper()}'S TURN: CHOOSE EMPTY FIELD";
            else MovePawnTo(coords);
        }

        private void ChoosePawn(int coords)
        {
            if (_game.GameBoard.GetField(coords).State != _color) _humanPlayerViewModel.Message = $"{_color.ToString().ToUpper()}'S TURN: CHOOSE YOUR PAWN";
            else
            {
                _nextMove.From = coords;
                _humanPlayerViewModel.Message = $"{_color.ToString().ToUpper()}'S TURN: CHOOSE EMPTY FIELD";
            }
        }

        private void ChooseDestination(int coords)
        {
            //CONSTRAINED MOVEMENT
            if (_game.GameBoard.GetFieldsCount(_color) > 3)
            {
                //ILLEGAL MOVE
                if (!_game.GameBoard.GetField(_nextMove.From).IsNeighbour(_game.GameBoard.GetField(coords))) _humanPlayerViewModel.Message = $"{_color.ToString().ToUpper()}'S TURN: CHOOSE NEIGHBOURING FIELD";

                //LEGAL MOVE
                else
                {
                    MovePawnTo(coords);
                }
            }
            //FREE MOVEMENT
            else
            {
                MovePawnTo(coords);
            }
        }

        private void MovePawnTo(int coords)
        {
            _nextMove.To = coords;
            if (_nextMove.Equals(_game.GameHistory.PeekLastMove(_color, true).Inverse())) _humanPlayerViewModel.Message = $"{_color.ToString().ToUpper()}'S TURN: YOU CANNOT REVERSE YOUR LAST MOVE!";
            else
            {
                _humanPlayerViewModel.EndInteraction();
                _timer.Stop();
                _nextMove.Time = _timer.ElapsedMilliseconds;
                MoveChosenEvent?.Invoke(this, _nextMove);
            }
        }

        private void CaptureChosen(int coords)
        {
            FieldState opponent = (FieldState)(-(int)_color);

            if (_game.GameBoard.GetField(coords).State != opponent) _humanPlayerViewModel.Message = $"{_color.ToString().ToUpper()}'S TURN: CHOOSE OPPONENT'S PAWN TO CAPTURE";
            else
            {
                if (!_game.GameBoard.GetField(coords).IsInMill() || _game.GameBoard.GetFieldsNotInMill(opponent).Length == 0)
                {
                    _humanPlayerViewModel.EndInteraction();
                    _nextMove.From = coords;
                    _timer.Stop();
                    _nextMove.Time = _timer.ElapsedMilliseconds;
                    CaptureChosenEvent?.Invoke(this, _nextMove);
                }
                else _humanPlayerViewModel.Message = $"{_color.ToString().ToUpper()}'S TURN: PAWNS IN A MILL ARE PROTECTED!";
            }
        }

        public Task<Move> AIMove(Game game)
        {
            throw new NotImplementedException();
        }

        public Move AICapture(Game game)
        {
            throw new NotImplementedException();
        }
    }
}
