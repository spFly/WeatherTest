using System;
using System.ComponentModel.DataAnnotations;

namespace RequestLogAPI.Data
{
    public class RequestLog
    {
        [Key]
        public Guid ID { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Text { get; set; }
    }
}
