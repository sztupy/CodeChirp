using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Shaml.Data.NHibernate.FluentNHibernate;
using Shaml.Membership.Core;
using CodeChirp.Core;

namespace CodeChirp.Data.Mapping
{
   public class SoulMap : IAutoMappingOverride<Soul>
   {
       public void Override(AutoMapping<Soul> mapping)
       {
         // mapping.Map(x => x.name).CustomType("StringClob").CustomSqlType("text");
         // mapping.Map(x => x.gravatar).CustomType("StringClob").CustomSqlType("text");
         // mapping.Map(x => x.point).CustomType("StringClob").CustomSqlType("text");
       }
   }
}
