using UnityEngine;

namespace OodlesParty
{
    public class SingleTransformPrediction : ClientPrediction<SingleTransformInput, SingleTransformState>
    {
        protected override SingleTransformInput GetInput(float deltaTime, uint currentTick)
        {
            return new SingleTransformInput(deltaTime, currentTick);
        }
    }
}