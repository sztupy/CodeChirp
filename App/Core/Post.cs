using NHibernate.Validator.Constraints;
using Shaml.Core.DomainModel;
using Shaml.Core.PersistenceSupport;
using Shaml.Membership.Core;
using System;
using Salient.StackApps.Routes;
using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace CodeChirp.Core
{
    
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
    }
}
