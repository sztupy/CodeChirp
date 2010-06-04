using NHibernate.Validator.Constraints;
using Shaml.Core.DomainModel;
using Shaml.Core.PersistenceSupport;
using Shaml.Membership.Core;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CodeChirp.Core
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Tag : Entity
    {
        public Tag() { }

        [NotNull]
        public virtual string name { get; set; }

        [NotNull]
        public virtual Site site { get; set; }
    }
}
