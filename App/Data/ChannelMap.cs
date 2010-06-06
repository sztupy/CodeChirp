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
   public class ChannelMap : IAutoMappingOverride<Channel>
   {
       public void Override(AutoMapping<Channel> mapping)
       {
           mapping.Map(x => x.name).Index("channel_name_index");
           mapping.HasManyToMany<Soul>(x => x.users).Cache.NonStrictReadWrite();
       }
   }
}
