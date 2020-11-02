using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Domain.Data
{
    public class User : IdentityUser<int>
    {
        /// <summary>
        /// Balance is in Cents
        /// </summary>
        public long Balance { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public ICollection<Round> Rounds { get; set; }
    }

    public class UserRole : IdentityRole<int>
    {
    }
}
