using DataAccess.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IAdvertsService
    {
        Task<IList<Equipment>> GetCartItems(string userId);
    }
}
