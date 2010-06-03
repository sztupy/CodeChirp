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
   public class BadgeMap : IAutoMappingOverride<Badge>
   {
       public void Override(AutoMapping<Badge> mapping)
       {
            mapping.Map(x => x.description).CustomType("StringClob").CustomSqlType("text");
            mapping.Map(x => x.sitename).Index("badge_site_name_index");
            mapping.Map(x => x.siteid).Index("badge_site_id_index");        
       }
   }
}
