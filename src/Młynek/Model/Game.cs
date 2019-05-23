using System;

namespace Młynek.Model
{
    class Game
    {
        private int _whitesInHand = 9;
        private int _blacksInHand = 9;

        public event EventHandler<FieldState> NextPlayersTurnEvent;
        public event EventHandler<FieldState> CaptureEvent;
        public event EventHandler<FieldState> GameEndedEvent;
        public event EventHandler<Move> UpdateStatsEvent;

        public GameBoard GameBoard { get; private set; }
        public GameHistory GameHistory { get; private set; }
        public FieldState NextPlayer { get; private set; } = FieldState.White;
        public FieldState Winner { get; private set; } = FieldState.Empty;
        public bool GameEnded { get; private set; } = false;

        public Game()
        {
            GameBoard = new GameBoard();
            GameHistory = new GameHistory();
        }

        public Game(GameBoard gameboard, GameHistory gameHistory)
        {
            GameBoard = gameboard;
            GameHistory = gameHistory;
        }

        public int GetRound()
        {
            if (_whitesInHand > 0 || _blacksInHand > 0) return 1;
            else if (GameBoard.GetFieldsCount(FieldState.White) < 4 || GameBoard.GetFieldsCount(FieldState.Black) < 4) return 3;
            else return 2;
        }

        public int GetPawnsInHand(FieldState player)
        {
            int count = 0;
            switch (player)
            {
                case FieldState.White:
                    count = _whitesInHand;
                    break;
                case FieldState.Black:
                    count = _blacksInHand;
                    break;
                default:
                    break;
            }
            return count;
        }

        public void Start()
        {
            if (!GameHistory.PeekLastMove().IsValid()) NextPlayersTurnEvent?.Invoke(this, NextPlayer);
        }

        public void MakeMove(Move move)
        {
            if (move.From == -1 && move.To > -1) PlacePawn(move);
            else MovePawn(move);
            UpdateStatsEvent?.Invoke(this, move);
        }

        private void MovePawn(Move move)
        {
            if (move.Player != NextPlayer) throw new InvalidOperationException();
            else if (move.From < 0 || move.To < 0)
            {
                GameEnded = true;
                Winner = (FieldState)(-(int)NextPlayer);
                GameEndedEvent?.Invoke(this, (FieldState)(-(int)NextPlayer));
            }
            else
            {
                GameBoard.RemoveFrom(move.From);
                GameBoard.PlaceAt(move.To, move.Player);
                GameHistory.SaveMove(move);

                if (GameBoard.GetField(move.To).IsInMill()) CaptureEvent?.Invoke(this, move.Player);
                else
                {

                    bool isDraw = GameHistory.SaveState(GameBoard.ToString());
                    if (isDraw)
                    {
                        GameEnded = true;
                        GameEndedEvent?.Invoke(this, FieldState.Empty);
                    }
                    else
                    {
                        NextPlayer = (FieldState)(-(int)NextPlayer);
                        NextPlayersTurnEvent?.Invoke(this, NextPlayer);
                    }
                }
            }
        }

        private void PlacePawn(Move move)
        {
            if (move.Player != NextPlayer || move.To < 0 || !HasPawnsInHand(move.Player)) throw new InvalidOperationException();
            else
            {
                GameBoard.PlaceAt(move.To, move.Player);
                GameHistory.SaveMove(move);

                if (move.Player == FieldState.White) _whitesInHand--;
                else if (move.Player == FieldState.Black) _blacksInHand--;

                if (GameBoard.GetField(move.To).IsInMill()) CaptureEvent?.Invoke(this, move.Player);
                else
                {
                    NextPlayer = (FieldState)(-(int)NextPlayer);
                    GameHistory.SaveState(GameBoard.ToString());
                    NextPlayersTurnEvent?.Invoke(this, NextPlayer);
                }
            }
        }

        public void Capture(Move move)
        {
            if (move.Player != NextPlayer || move.From < 0) throw new InvalidOperationException();
            else
            {
                GameBoard.RemoveFrom(move.From);
                GameHistory.SaveMove(move);

                bool isDraw = GameHistory.SaveState(GameBoard.ToString());

                if (isDraw)
                {
                    GameEnded = true;
                    GameEndedEvent?.Invoke(this, FieldState.Empty);
                }
                else if (_whitesInHand == 0 && _blacksInHand == 0 && GameBoard.GetFieldsCount((FieldState)(-(int)NextPlayer)) < 3)
                {
                    GameEnded = true;
                    Winner = NextPlayer;
                    GameEndedEvent?.Invoke(this, NextPlayer);
                }
                else
                {
                    NextPlayer = (FieldState)(-(int)NextPlayer);
                    NextPlayersTurnEvent?.Invoke(this, NextPlayer);
                }
            }
            UpdateStatsEvent?.Invoke(this, move);
        }

        public void Undo()
        {
            if (GameEnded)
            {
                GameEnded = false;
                Winner = FieldState.Empty;
            }

            //LAST MOVE WAS GIVE-UP MOVE
            if (!GameHistory.PeekLastMove().IsValid()) GameHistory.PopLastMove(); 

            GameHistory.RemoveState(GameBoard.ToString());

            Move lastMove = GameHistory.PopLastMove();

            //LAST MOVE WAS CAPTURE, UNDO ONE MORE MOVE
            if (lastMove.To < 0)
            {
                GameBoard.PlaceAt(lastMove.From, (FieldState)(-(int)lastMove.Player));

                Move oneMore = GameHistory.PopLastMove();

                if (oneMore.Player != lastMove.Player) throw new InvalidOperationException();

                if (oneMore.To >= 0) GameBoard.RemoveFrom(oneMore.To);
                if (oneMore.From >= 0) GameBoard.PlaceAt(oneMore.From, oneMore.Player);

                // LAST MOVE WAS PLACEMENT, ADD PAWN TO HAND
                if (oneMore.From < 0)
                {
                    if (oneMore.Player == FieldState.White) _whitesInHand++;
                    else if (oneMore.Player == FieldState.Black) _blacksInHand++;
                }
            }
            // LAST MOVE WAS REGULAR MOVE
            else
            {
                if (lastMove.To >= 0) GameBoard.RemoveFrom(lastMove.To);
                if (lastMove.From >= 0) GameBoard.PlaceAt(lastMove.From, lastMove.Player);

                // IF LAST MOVE WAS PLACEMENT, ADD PAWN TO HAND
                if (lastMove.From < 0)
                {
                    if (lastMove.Player == FieldState.White) _whitesInHand++;
                    else if (lastMove.Player == FieldState.Black) _blacksInHand++;
                }
            }

            NextPlayer = lastMove.Player;
            NextPlayersTurnEvent?.Invoke(this, NextPlayer);
        }


        public Game Duplicate()
        {
            Game duplicate = new Game(GameBoard.Duplicate(), GameHistory.Duplicate());
            duplicate._whitesInHand = _whitesInHand;
            duplicate._blacksInHand = _blacksInHand;
            duplicate.NextPlayer = NextPlayer;
            duplicate.Winner = Winner;
            duplicate.GameEnded = GameEnded;
            return duplicate;
        }

        public void Restart()
        {
            _whitesInHand = 9;
            _blacksInHand = 9;
            GameEnded = false;
            NextPlayer = FieldState.White;
            Winner = FieldState.Empty;
            GameBoard.Clear();
            GameHistory.Clear();
        }

        private bool HasPawnsInHand(FieldState player)
        {
            bool hasPawns = false;
            if (player == FieldState.White) hasPawns = _whitesInHand > 0;
            else if (player == FieldState.Black) hasPawns = _blacksInHand > 0;
            return hasPawns;
        }
    }
}
