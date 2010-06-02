using NHibernate.Validator.Constraints;
using Shaml.Core.DomainModel;
using Shaml.Core.PersistenceSupport;
using Shaml.Membership.Core;
using System;

namespace CodeChirp.Core
{
    
    public class Tag : Entity
    {
        public Tag() { }

        [NotNull]
        public virtual string name { get; set; }

        [NotNull]
        public virtual Site site { get; set; }

    }
}
