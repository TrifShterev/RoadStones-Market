using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using RoadStones_Models;

namespace RoadStones_Data.Data.Repository.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);

        
    }
}