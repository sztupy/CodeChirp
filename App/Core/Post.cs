using NHibernate.Validator.Constraints;
using Shaml.Core.DomainModel;
using Shaml.Core.PersistenceSupport;
using Shaml.Membership.Core;
using System;
using Salient.StackApps.Routes;
using System.Collections.Generic;

namespace CodeChirp.Core
{
    
    public class Post : Entity
    {
        public Post() {
            tags = new List<Tag>();
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
        public virtual int score { get; set; }

        [NotNull]
        public virtual Soul user { get; set; }

        [NotNull]
        public virtual DateTime lastactivity { get; set; }

        [NotNull]
        public virtual List<Tag> tags { get; protected set; }

        [NotNull]
        public virtual Site site { get; set; }

        [NotNull]
        public virtual int siteid { get; set; }
    }
}
