using NHibernate.Validator.Constraints;
using Shaml.Core.DomainModel;
using Shaml.Core.PersistenceSupport;
using Shaml.Membership.Core;
using System;
using Salient.StackApps.Routes;

namespace CodeChirp.Core
{
    
    public class Badge : Entity
    {
        public Badge() { }

        [NotNull]
        public virtual string name { get; set; }

        [NotNull]
        public virtual int count { get; set; }

        [NotNull]
        public virtual BadgesRank rank { get; set; }

        [NotNull]
        public virtual string description { get; set; }

        [NotNull]
        public virtual Site sitename { get; set; }

        [NotNull]
        public virtual int siteid { get; set; }
    }
}
