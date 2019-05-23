using Młynek.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Młynek.Players
{
    class AIHelper
    {
        public Move[] GetAvaiableMoves(Game game)
        {
            List<Move> avaiableMoves = new List<Move>();
            Move[] moves = game.GetPawnsInHand(game.NextPlayer) > 0 ? game.GameBoard.GetAvaiablePlacements(game.NextPlayer) : game.GameBoard.GetAvaiableMoves(game.NextPlayer, game.GameHistory.PeekLastMove(game.NextPlayer, true), game.GameBoard.GetFieldsCount(game.NextPlayer) < 4);
            for(int i = 0; i < moves.Length; i++)
            {
                Move nextMove = moves[i];
                if (nextMove.CreatesMill)
                {
                    Field[] possibleCaptures = game.GameBoard.GetAvaiableCaptures((FieldState)(-(int)nextMove.Player));
                    for(int j = 0; j < possibleCaptures.Length; j++)
                    {
                        nextMove.Capture = possibleCaptures[j].Position;
                        avaiableMoves.Add(nextMove);
                    }
                }
                else avaiableMoves.Add(nextMove);
            }
            return avaiableMoves.ToArray();
        }
    }
}
