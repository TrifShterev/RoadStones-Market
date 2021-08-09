using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using RoadStones_Models;

namespace RoadStones_Data.Data.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);

        
    }
}