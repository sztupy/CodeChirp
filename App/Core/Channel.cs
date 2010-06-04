using NHibernate.Validator.Constraints;
using Shaml.Core.DomainModel;
using Shaml.Core.PersistenceSupport;
using Shaml.Membership.Core;
using System;
using Iesi.Collections.Generic;

namespace CodeChirp.Core
{
    
    public class Channel : Entity
    {
        public Channel() { users = new HashedSet<Soul>(); }

        [NotNull(Message="The channel needs a name")]
        [NotEmpty(Message="The channel needs a name")]
        public virtual string name { get; set; }

        public virtual User owner { get; set; }

        public virtual ISet<Soul> users { get; protected set; }

        public virtual void AddUser(Soul user)
        {
            users.Add(user);
        }

    }
}
