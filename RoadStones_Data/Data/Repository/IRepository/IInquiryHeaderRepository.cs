using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using RoadStones_Models;

namespace RoadStones_Data.Data.Repository.IRepository
{
    public interface IInquiryHeaderRepository : IRepository<InquiryHeader>
    {
        void Update(InquiryHeader obj);

        
    }
}