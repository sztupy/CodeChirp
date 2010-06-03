using NHibernate.Validator.Constraints;
using Shaml.Core.DomainModel;
using Shaml.Core.PersistenceSupport;
using Shaml.Membership.Core;
using System;
using Salient.StackApps.Routes;
using Iesi.Collections;
using Iesi.Collections.Generic;

namespace CodeChirp.Core
{
    
    public class Badge : Entity
    {
        public Badge() {
            users = new HashedSet<Soul>();
        }

        [NotNull]
        public virtual string name { get; set; }

        [NotNull]
        public virtual long count { get; set; }

        [NotNull]
        public virtual BadgesRank rank { get; set; }

        [NotNull]
        public virtual string description { get; set; }

        [NotNull]
        public virtual Site sitename { get; set; }

        [NotNull]
        public virtual long siteid { get; set; }

        public virtual ISet<Soul> users { get; protected set; }

        public virtual void AddUser(Soul user)
        {
            users.Add(user);
            user.badges.Add(this);
        }
    }
}
