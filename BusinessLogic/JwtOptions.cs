using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class JwtOptions
    {
        public string Key { get; set; }
        public int LifetimeInMinutes { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }

        public int RefreshTokenExpirationDays { get; set; }

        public string Issuer { get; set; }
    }
}
