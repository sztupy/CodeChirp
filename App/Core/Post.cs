using NHibernate.Validator.Constraints;
using Shaml.Core.DomainModel;
using Shaml.Core.PersistenceSupport;
using Shaml.Membership.Core;
using System;
using Salient.StackApps.Routes;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using Newtonsoft.Json;
using System.Globalization;

namespace CodeChirp.Core
{
    public static class DateConverter
    {
        public static string NaturalString(this DateTime dt)
        {
            TimeSpan ts = DateTime.UtcNow - dt;
            if (ts.TotalSeconds < 120)
            {
                return "just now";
            }
            if (ts.TotalMinutes < 90)
            {
                return Math.Round(ts.TotalMinutes) + " minutes ago";
            }
            if (ts.TotalHours < 24)
            {
                return Math.Round(ts.TotalHours) + " hours ago";
            }
            if (dt.Year == DateTime.UtcNow.Year)
            {
                return dt.ToString("MMM dd \\a\\t HH:mm", CultureInfo.CreateSpecificCulture("en-US"));
            }
            else
            {
                return dt.ToString("MMM dd \\'yy \\a\\t HH:mm", CultureInfo.CreateSpecificCulture("en-US"));
            }
        }
    }

    [JsonObject(MemberSerialization.OptOut)]
    public class Post : Entity
    {  
        public Post() {
            tags = new HashedSet<Tag>();
        }

        [NotNull]
        public virtual PostType type { get; set; }

        [NotNull]
        public virtual string summary { get; set; }

        [NotNull]
        public virtual string body { get; set; }

        [NotNull]
        public virtual bool community { get; set; }

        [NotNull]
        public virtual long score { get; set; }

        public virtual Soul user { get; set; }

        [NotNull]
        public virtual DateTime lastedit { get; set; }

        [NotNull]
        public virtual DateTime lastactivity { get; set; }

        [NotNull]
        public virtual ISet<Tag> tags { get; protected set; }

        public virtual Post parent { get; set; }

        [NotNull]
        public virtual Site sitename { get; set; }

        [NotNull]
        public virtual long siteid { get; set; }

        public virtual string ToUrl()
        {
            string s = sitename.ToUrl();
            switch (type)
            {
                case PostType.answer: return s + "/questions/" + parent.siteid + "/name/" + siteid + "#" + siteid;
                case PostType.or: return parent.ToUrl();
                case PostType.question: return s + "/questions/" + siteid;
                default: return s;
            }
        }

        public virtual string Activity()
        {
            switch (type)
            {
                case PostType.answer: return "answered";
                case PostType.or: return "commented";
                case PostType.question: return "asked";
                default: return "";
            }
        }
    }
}
