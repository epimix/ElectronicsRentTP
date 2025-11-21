using BusinessLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly IUserServices _userService;
        public BalanceService(IUserServices userService)
        {
            _userService = userService;
        }
        public async Task<decimal> GetUserBalanceAsync(string userId)
        {
            if (userId == null) 
                throw new InvalidOperationException("User not found");
            var user = await _userService.GetById(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            return user.Balance;
        }

        public async Task Payment(string userId, decimal amount)
        {
            if (userId == null)
                throw new InvalidOperationException("User not found");
            var user = await _userService.GetById(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            user.Balance -= amount;
            await _userService.Update(user);
        }


        public async Task OwnerPay(string userId, decimal amount)
        {
            if (userId == null)
                throw new InvalidOperationException("User not found");
            var user = await _userService.GetById(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            user.Balance += amount;
            await _userService.Update(user);
        }

        public async Task ReplenishmentBalanceAsync(string userId, decimal amount)
        {
            if (userId == null)
                throw new InvalidOperationException("User not found");
            var user = await _userService.GetById(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            user.Balance += amount;
            await _userService.Update(user);
        }
    }
}
