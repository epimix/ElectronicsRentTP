using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IBalanceService
    {
        Task<decimal> GetUserBalanceAsync(string userId);
        Task ReplenishmentBalanceAsync(string userId, decimal amount);
        Task Payment(string userId, decimal amount);
    }
}
