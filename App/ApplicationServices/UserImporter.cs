using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Salient.StackApps.Routes;
using Shaml.Data.NHibernate;
using CodeChirp.Core;
using NHibernate;
using Shaml.Core.PersistenceSupport.NHibernate;

namespace CodeChirp.ApplicationServices
{
    public class UserImporter
    {
        public Soul Import(INHibernateQueryRepository<Soul> soulRepository, Site site, Users user)
        {
            User u = new User();
            u.display_name = user.display_name;
            u.email_hash = user.email_hash;
            u.reputation = user.reputation;
            u.user_id = user.user_id;
            u.user_type = user.user_type;
            return Import(soulRepository, site, u);
        }

        public Soul Import(INHibernateQueryRepository<Soul> soulRepository, Site site, User user)
        {
            if (user != null)
            {
                Soul soul = soulRepository.FindOne(new { siteid = user.user_id, sitename = site });
                if (soul == null)
                {
                    soul = new Soul();
                }
                soul.siteid = user.user_id;
                soul.point = user.reputation;
                soul.name = user.display_name;
                soul.sitename = site;
                soul.gravatar = user.email_hash;
                soulRepository.SaveOrUpdate(soul);
                return soul;
            }
            else return null;
        }
    }
}
