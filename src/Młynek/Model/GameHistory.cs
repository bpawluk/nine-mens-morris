using System;
using System.Collections.Generic;

namespace Młynek.Model
{
    class GameHistory
    {
        private Dictionary<string, int> _history;
        private Stack<Move> _moves;
        private Stack<Move> _whiteMoves;
        private Stack<Move> _blackMoves;

        public GameHistory()
        {
            _history = new Dictionary<string, int>();
            _moves = new Stack<Move>();
            _whiteMoves = new Stack<Move>();
            _blackMoves = new Stack<Move>();
        }

        public Move PeekLastMove(FieldState player = FieldState.Empty, bool ignoreCaptures = false)
        {
            Move move = new Move();
            switch (player)
            {
                case FieldState.White:
                    if (_whiteMoves.Count > 0)
                    {
                        if (ignoreCaptures && _whiteMoves.Peek().To < 0)
                        {
                            Move temp = _whiteMoves.Pop();
                            move = _whiteMoves.Peek();
                            _whiteMoves.Push(temp);
                        }
                        else move = _whiteMoves.Peek();
                    }
                    break;
                case FieldState.Black:
                    if (_blackMoves.Count > 0)
                    {
                        if (ignoreCaptures && _blackMoves.Peek().To < 0)
                        {
                            Move temp = _blackMoves.Pop();
                            move = _blackMoves.Peek();
                            _blackMoves.Push(temp);
                        }
                        else move = _blackMoves.Peek();
                    }
                    break;
                case FieldState.Empty:
                    if (_moves.Count > 0)
                    {
                        if (ignoreCaptures && _moves.Peek().To < 0)
                        {
                            Move temp = _moves.Pop();
                            move = _moves.Peek();
                            _moves.Push(temp);
                        }
                        else move = _moves.Peek();
                    }
                    break;
                default:
                    break;
            }

            return move;
        }

        public void SaveMove(Move move)
        {
            _moves.Push(move);
            if (move.Player == FieldState.White) _whiteMoves.Push(move);
            else if (move.Player == FieldState.Black) _blackMoves.Push(move);
            else throw new InvalidOperationException();
        }

        public Move PopLastMove()
        {
            Move move = new Move();
            if (_moves.Count > 0)
            {
                move = _moves.Pop();
                if (move.Player == FieldState.White) _whiteMoves.Pop();
                else if (move.Player == FieldState.Black) _blackMoves.Pop();
            }
            return move;
        }

        public bool SaveState(string state)
        {
            if (_history.ContainsKey(state)) _history[state]++;
            else _history.Add(state, 1);
            return _history[state] >= 3;
        }

        public void RemoveState(string state)
        {
            if (_history.ContainsKey(state))
            {
                _history[state]--;
                if (_history[state] < 0) throw new InvalidOperationException();
            }
        }

        public GameHistory Duplicate()
        {
            GameHistory duplicate = new GameHistory();
            duplicate._history = new Dictionary<string, int>(_history);
            duplicate._moves = new Stack<Move>(new Stack<Move>(_moves));
            duplicate._whiteMoves = new Stack<Move>(new Stack<Move>(_whiteMoves));
            duplicate._blackMoves = new Stack<Move>(new Stack<Move>(_blackMoves));
            return duplicate;
        }

        public void Clear()
        {
            _history.Clear();
            _moves.Clear();
            _whiteMoves.Clear();
            _blackMoves.Clear();
        }
    }
}
