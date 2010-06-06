using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Shaml.Data.NHibernate.FluentNHibernate;
using Shaml.Membership.Core;
using CodeChirp.Core;
using FluentNHibernate.Mapping;

namespace CodeChirp.Data.Mapping
{
   public class TagMap : IAutoMappingOverride<Tag>
   {
       public void Override(AutoMapping<Tag> mapping)
       {
           mapping.Cache.ReadOnly();
           mapping.Map(x => x.name).Index("tag_name_index");
           mapping.Map(x => x.site).Index("tag_site_name_index");
       }
   }
}
