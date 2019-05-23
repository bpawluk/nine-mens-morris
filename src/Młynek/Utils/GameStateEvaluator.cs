using Młynek.Model;

namespace Młynek.Utils
{
    class GameStateEvaluator : IEvaluateGameState
    {
        private float _positionEvaluationWeight = 10;
        private float _rowOwnershipWeight = 100;
        private float _columnOwnershipWeight = 100;
        private float _possibleMovesWeight = 10;
        private float _pawnsCountWeight = 10;

        public GameStateEvaluator(float position, float row, float column, float moves, float pawns)
        {
            _positionEvaluationWeight = position;
            _rowOwnershipWeight = row;
            _columnOwnershipWeight = column;
            _possibleMovesWeight = moves;
            _pawnsCountWeight = pawns;
        }

        public float EvaluateGameState(Game game, FieldState player)
        {
            var eval = EndGameEvaluation(game, player);
            if (eval.Item1) return eval.Item2;

            float fieldConnectedEval = 0;
            foreach (Field field in game.GameBoard.Fields)
            {
                fieldConnectedEval = fieldConnectedEval + _positionEvaluationWeight * PositionEvaluation(field, player)
                                                        + _rowOwnershipWeight * RowsOwnership(field, player)
                                                        + _columnOwnershipWeight * ColumnOwnership(field, player);
            }

            return fieldConnectedEval / 24
                + _possibleMovesWeight * PossibleMoves(game, player)
                + _pawnsCountWeight * PawnsCount(game, player);
        }

        private (bool, float) EndGameEvaluation(Game game, FieldState player)
        {
            if (game.GameEnded)
            {
                float eval = game.Winner == player ? float.MaxValue : (game.Winner == FieldState.Empty ? 0f : float.MinValue);
                return (true, eval);
            }

            if (game.GetPawnsInHand(player) == 0 && game.GameBoard.GetAvaiableMoves(player, game.GameHistory.PeekLastMove(player, true), game.GameBoard.GetFieldsCount(player) < 4).Length == 0) return (true, float.MinValue);

            if (game.GetPawnsInHand((FieldState)(-(int)player)) == 0 && game.GameBoard.GetAvaiableMoves((FieldState)(-(int)player), game.GameHistory.PeekLastMove((FieldState)(-(int)player), true), game.GameBoard.GetFieldsCount((FieldState)(-(int)player)) < 4).Length == 0) return (true, float.MaxValue);

            return (false, 0f);
        }

        private float PositionEvaluation(Field field, FieldState player)
        {
            float[] weights = new float[] { 2, 3, 2, 2, 4, 2, 2, 3, 2, 2, 4, 2, 2, 4, 2, 2, 3, 2, 2, 4, 2, 2, 3, 2 };
            int value = 0;
            if (field.State == player) value = 1;
            else if (field.State == (FieldState)(-(int)player)) value = -1;
            return value * weights[field.Position];
        }

        private int RowsOwnership(Field field, FieldState player)
        {
            int count = 0;
            if (field.State == player)
            {
                if (field.IsInMill()) count = 5;
                else
                {
                    foreach (Field row in field.Row)
                    {
                        if (row.State == player) count++;
                        else if (row.State == (FieldState)(-(int)player)) count = count - 3;
                    }
                }

            }
            return count;
        }

        private float ColumnOwnership(Field field, FieldState player)
        {
            int count = 0;
            if (field.State == player)
            {
                if (field.IsInMill()) count = 5;
                else
                {
                    foreach (Field column in field.Column)
                    {
                        if (column.State == player) count++;
                        else if (column.State == (FieldState)(-(int)player)) count = count - 3;
                    }
                }
            }
            return count;
        }

        private int PossibleMoves(Game game, FieldState player)
        {
            return game.GameBoard.GetAvaiableMoves(player, game.GameHistory.PeekLastMove(player, true), game.GameBoard.GetFieldsCount(player) < 4).Length - game.GameBoard.GetAvaiableMoves((FieldState)(-(int)player), game.GameHistory.PeekLastMove((FieldState)(-(int)player), true), game.GameBoard.GetFieldsCount((FieldState)(-(int)player)) < 4).Length;
        }

        private int PawnsCount(Game game, FieldState player)
        {
            return game.GameBoard.GetFieldsCount(player) - game.GameBoard.GetFieldsCount((FieldState)(-(int)player));
        }
    }
}