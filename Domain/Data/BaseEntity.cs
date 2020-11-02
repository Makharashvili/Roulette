using System;

namespace Domain.Data
{
    public class BaseEntity<T>
    {
        public T Id { get;set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}
