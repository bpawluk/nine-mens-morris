using Młynek.Model;
using Młynek.ViewModel;

namespace Młynek.Utils
{
    interface IEvaluateGameState
    {
        float EvaluateGameState(Game game, FieldState player);
    }
}
