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
   public class PostMap : IAutoMappingOverride<Post>
   {
       public void Override(AutoMapping<Post> mapping)
       {
         // mapping.Map(x => x.type).CustomType("StringClob").CustomSqlType("text");
         // mapping.Map(x => x.summary).CustomType("StringClob").CustomSqlType("text");
         // mapping.Map(x => x.body).CustomType("StringClob").CustomSqlType("text");
         // mapping.Map(x => x.community).CustomType("StringClob").CustomSqlType("text");
         // mapping.Map(x => x.score).CustomType("StringClob").CustomSqlType("text");
         // mapping.Map(x => x.user).CustomType("StringClob").CustomSqlType("text");
         // mapping.Map(x => x.lastactivity).CustomType("StringClob").CustomSqlType("text");
         // mapping.Map(x => x.tags).CustomType("StringClob").CustomSqlType("text");
       }
   }
}
