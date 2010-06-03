using NHibernate.Validator.Constraints;
using Shaml.Core.DomainModel;
using Shaml.Core.PersistenceSupport;
using Shaml.Membership.Core;
using System;
using Iesi.Collections;
using Iesi.Collections.Generic;

namespace CodeChirp.Core
{
    
    public class Soul : Entity
    {
        public Soul() {
            badges = new HashedSet<Badge>();
        }

        [NotNull]
        public virtual string name { get; set; }

        [NotNull]
        public virtual string gravatar { get; set; }

        [NotNull]
        public virtual long point { get; set; }

        [NotNull]
        public virtual Site sitename { get; set; }

        [NotNull]
        public virtual long siteid { get; set; }

        public virtual ISet<Badge> badges { get; protected set; }
    }
}
