using Młynek.Model;
using Młynek.Utils;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Młynek.Players
{
    class AlfaBetaPlayer : IPlayer
    {
        private FieldState _color;
        private IEvaluateGameState _evaluator;
        private AIHelper _aiHelper;
        private Game _game;
        private Move _pendingCapture;
        private long _nodesCount;
        private int _depth;


        public bool IsHuman => false;
        public string TypeName => "AlfaBeta";

        public AlfaBetaPlayer(FieldState color, int depth, GameStateEvaluator evaluator)
        {
            _color = color;
            _evaluator = evaluator;
            _depth = depth;
            _aiHelper = new AIHelper();
        }

        public async Task<Move> AIMove(Game game)
        {
            _nodesCount = 0;
            Stopwatch timer = Stopwatch.StartNew();
            Move nextMove = await Task.Run(() =>
            {
                _game = game.Duplicate();
                return AlfaBeta();
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

        public Move AlfaBeta()
        {
            int depth = _game.GetRound() == 2 ? _depth : 3;
            Move nextMove = AlfaBetaRecursion(depth, float.MinValue, float.MaxValue).Item2;
            _game = null;
            return nextMove;
        }

        public (float, Move) AlfaBetaRecursion(int depth, float alfa, float beta)
        {
            _nodesCount++;

            // GAME ENDED
            if (depth == 0 || _game.GameEnded)
            {
                return (_evaluator.EvaluateGameState(_game, _color), new Move(FieldState.Empty));
            }

            Move[] possibleMoves = _aiHelper.GetAvaiableMoves(_game);

            //GIVE-UP PSEUDO MOVE
            if (possibleMoves.Length == 0)
            {
                Move giveUp = new Move(_game.NextPlayer);
                _game.MakeMove(giveUp);
                return ((AlfaBetaRecursion(depth - 1, alfa, beta).Item1, giveUp));
            }

            // MAXIMIZING PLAYER
            if (_game.NextPlayer == _color)
            {
                var max = (Item1: float.MinValue, new Move(FieldState.Empty));

                for (int i = 0; i < possibleMoves.Length; i++)
                {
                    Move move = possibleMoves[i];

                    _game.MakeMove(move);
                    if (move.CreatesMill) _game.Capture(new Move(move.Player, move.Capture));

                    if (max.Item1 >= alfa) alfa = max.Item1;

                    var eval = ((AlfaBetaRecursion(depth - 1, alfa, beta).Item1, move));
                    _game.Undo();

                    if (eval.Item1 >= beta) return eval;

                    if (eval.Item1 >= max.Item1) max = eval;
                }
                return max;
            }
            // MINIMIZING PLAYER
            else
            {
                var min = (Item1: float.MaxValue, new Move(FieldState.Empty));

                for (int i = 0; i < possibleMoves.Length; i++)
                {
                    Move move = possibleMoves[i];

                    _game.MakeMove(move);
                    if (move.CreatesMill) _game.Capture(new Move(move.Player, move.Capture));

                    if (min.Item1 <= beta) beta = min.Item1;

                    var eval = ((AlfaBetaRecursion(depth - 1, alfa, beta).Item1, move));
                    _game.Undo();

                    if (eval.Item1 <= alfa) return eval;

                    if (eval.Item1 <= min.Item1) min = eval;
                }
                return min;
            }
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