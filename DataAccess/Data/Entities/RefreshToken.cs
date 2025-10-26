using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = default!;
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string CreatedByIp { get; set; } = default!;
        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
        public string UserId { get; set; }
        public User? User { get; set; }
    }
}
