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
         mapping.Map(x => x.body).CustomType("StringClob").CustomSqlType("text");
         mapping.Map(x => x.sitename).Index("post_site_name_index");
         mapping.Map(x => x.siteid).Index("post_site_id_index");
         mapping.HasManyToMany<Tag>(x => x.tags);
       }
   }
}
