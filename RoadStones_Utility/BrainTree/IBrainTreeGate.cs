using Braintree;

namespace RoadStones_Utility.BrainTree
{
    public interface IBrainTreeGate
    {
      IBraintreeGateway  CreateGateway();

      IBraintreeGateway GetGateway();
    }
}