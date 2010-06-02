using NHibernate.Validator.Constraints;
using Shaml.Core.DomainModel;
using Shaml.Core.PersistenceSupport;
using Shaml.Membership.Core;
using System;

namespace CodeChirp.Core
{
    
    public class Soul : Entity
    {
        public Soul() { }

        [NotNull]
        public virtual string name { get; set; }

        [NotNull]
        public virtual string gravatar { get; set; }

        [NotNull]
        public virtual int point { get; set; }

        [NotNull]
        public virtual Site site { get; set; }

        [NotNull]
        public virtual int siteid { get; set; }
    }
}
