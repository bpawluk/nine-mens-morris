using Młynek.Model;
using Młynek.Utils;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Młynek.Players
{
    class MinMaxPlayer : IPlayer
    {
        private FieldState _color;
        private IEvaluateGameState _evaluator;
        private AIHelper _aiHelper;
        private Game _game;
        private Move _pendingCapture;
        private long _nodesCount;
        private int _depth;


        public bool IsHuman => false;
        public string TypeName => "MinMax";

        public MinMaxPlayer(FieldState color, int depth, GameStateEvaluator evaluator)
        {
            _color = color;
            _depth = depth;
            _evaluator = evaluator;
            _aiHelper = new AIHelper();
        }

        public async Task<Move> AIMove(Game game)
        {
            _nodesCount = 0;
            Stopwatch timer = Stopwatch.StartNew();
            Move nextMove = await Task.Run(() =>
            {
                _game = game.Duplicate();
                return MinMax();
            });
            if (nextMove.Capture >= 0) _pendingCapture = new Move(_color, nextMove.Capture);
            timer.Stop();
            nextMove.Time = timer.ElapsedMilliseconds;
            nextMove.NodesVisited = _nodesCount;
            return nextMove;
        }

        public Move AICapture(Game game)
        {
            if (!_pendingCapture.IsValid()) throw new InvalidOperationException();

            Move capture = _pendingCapture;
            _pendingCapture = new Move();
            return capture;
        }

        public Move MinMax()
        {
            int depth = _game.GetRound() == 2 ? _depth : 3;
            Move nextMove = MinMaxRecursion(depth).Item2;
            _game = null;
            return nextMove;
        }

        public (float, Move) MinMaxRecursion(int depth)
        {
            _nodesCount++;

            // GAME ENDED OR MAX DEPTH REACHED
            if (depth <= 0 || _game.GameEnded)
            {
                return (_evaluator.EvaluateGameState(_game, _color), new Move(FieldState.Empty));
            }

            Move[] possibleMoves = _aiHelper.GetAvaiableMoves(_game);

            // GIVE-UP PSEUDO MOVE
            if (possibleMoves.Length == 0)
            {
                Move giveUp = new Move(_game.NextPlayer);
                _game.MakeMove(giveUp);
                return ((MinMaxRecursion(depth - 1).Item1, giveUp));
            }

            // MINI-MAX EVALUATION
            var max = (Item1: float.MinValue, new Move(FieldState.Empty));
            var min = (Item1: float.MaxValue, new Move(FieldState.Empty));

            for (int i = 0; i < possibleMoves.Length; i++)
            {
                Move move = possibleMoves[i];

                _game.MakeMove(move);
                if (move.CreatesMill) _game.Capture(new Move(move.Player, move.Capture));

                var eval = ((MinMaxRecursion(depth - 1).Item1, move));

                if (eval.Item1 >= max.Item1) max = eval;
                if (eval.Item1 <= min.Item1) min = eval;

                _game.Undo();
            }

            if (_game.NextPlayer == _color) return max;
            else return min;
        }

        public void Move(Game game)
        {
            throw new NotImplementedException();
        }

        public void Capture(Game game)
        {
            throw new NotImplementedException();
        }
    }
}