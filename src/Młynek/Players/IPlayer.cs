using Młynek.Model;
using System.Threading.Tasks;

namespace Młynek.Players
{
    interface IPlayer
    {
        bool IsHuman { get; }
        string TypeName { get; }
        void Move(Game game);
        Task<Move> AIMove(Game game);
        void Capture(Game game);
        Move AICapture(Game game);
    }
}