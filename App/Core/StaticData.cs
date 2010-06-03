using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shaml.Core.DomainModel;
using NHibernate.Validator.Constraints;

namespace CodeChirp.Core
{
    public class StaticData : Entity
    {
        public StaticData() { }

        [NotNull]
        public virtual DateTime lastmodifieddate { get; set; }
    }

   /* public static class TimestampHelpers
    {
        public static DateTime FromTimestamp(this long timestamp)
        {
            DateTime dt = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dt.AddSeconds(timestamp);
        }

        public static long ToTimestamp(this DateTime datetime)
        {
            TimeSpan span = (datetime - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            return (long)span.TotalSeconds;
        }
    }*/
}
