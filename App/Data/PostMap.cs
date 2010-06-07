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
           mapping.Cache.ReadOnly();
           mapping.Map(x => x.body).CustomType("StringClob").CustomSqlType("text");
           mapping.Map(x => x.summary).CustomType("StringClob").CustomSqlType("text");
           mapping.Map(x => x.sitename).Index("post_site_name_index");
           mapping.Map(x => x.siteid).Index("post_site_id_index");
           mapping.Map(x => x.lastedit).Index("post_lastedit_index");
           mapping.Map(x => x.lastactivity).Index("post_lastactivity_index");
           mapping.Map(x => x.type).Index("post_type_index");
           mapping.HasManyToMany<Tag>(x => x.tags).Cache.ReadOnly();

           mapping.Map(x => x.type).UniqueKey("post_site_unique");
           mapping.Map(x => x.siteid).UniqueKey("post_site_unique");
           mapping.Map(x => x.sitename).UniqueKey("post_site_unique");
       }
   }
}
