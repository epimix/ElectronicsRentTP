using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Enum
{
    public enum PaymentType
    {
        moneyTransfer,   // Банківський переказ
        creditCard,     // Кредитна картка
        cash            // Готівка
    }
}
