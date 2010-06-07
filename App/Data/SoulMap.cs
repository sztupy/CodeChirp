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
           mapping.Cache.ReadOnly();
           mapping.Map(x => x.name).Index("soul_name_index");
           mapping.Map(x => x.sitename).Index("soul_site_name_index");
           mapping.Map(x => x.siteid).Index("soul_site_id_index");
           mapping.HasManyToMany<Badge>(x => x.badges).Cache.ReadOnly();

           mapping.Map(x => x.siteid).UniqueKey("soul_site_unique");
           mapping.Map(x => x.sitename).UniqueKey("soul_site_unique");
       }
   }
}
