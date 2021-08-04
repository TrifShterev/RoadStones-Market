using System.Collections.Generic;

namespace RoadStones_Models.ViewModels
{
    public class InquiryVM
    {
        public InquiryHeader InquiryHeader { get; set; }

        public IEnumerable<InquiryDetails> InquiryDetails { get; set; }
    }
}