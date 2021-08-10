using System;
using Braintree;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace RoadStones_Utility.BrainTree
{
    public class BrainTreeGate : IBrainTreeGate
    {


        public BrainTreeSettings _optionBrainTreeSettings { get; set; }

        private IBraintreeGateway _braintreeGateway { get; set; }

        public BrainTreeGate(IOptions<BrainTreeSettings> options)
        {
            _optionBrainTreeSettings = options.Value;

        }

       

        public IBraintreeGateway CreateGateway()
        {
            return new BraintreeGateway(_optionBrainTreeSettings.Environment, _optionBrainTreeSettings.MerchantId,
                _optionBrainTreeSettings.PublicKey, _optionBrainTreeSettings.PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            

            if (_braintreeGateway==null)
            {
                _braintreeGateway = CreateGateway();
            }

            return _braintreeGateway;
        }
    }
}