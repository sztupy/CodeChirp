﻿/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Salient.StackApps;
using Salient.StackApps.Routes;
using Shaml.Core.PersistenceSupport.NHibernate;
using CodeChirp.Core;
using Shaml.Data.NHibernate;
using System.Text.RegularExpressions;

namespace CodeChirp.ApplicationServices
{
    public class BadgeImporter
    {
        const int MAXNUMBEROFUSERSALLOWED = 10;

        INHibernateQueryRepository<Soul> userRepository;
        INHibernateQueryRepository<Tag> tagRepository;
        INHibernateQueryRepository<Badge> badgeRepository;
        UserImporter userimport;
        Site currentSite;

        public BadgeImporter()
        {
            userRepository = new NHibernateQueryRepository<Soul>();
            tagRepository = new NHibernateQueryRepository<Tag>();
            badgeRepository = new NHibernateQueryRepository<Badge>();
            userimport = new UserImporter();
        }

        public BadgeImporter(INHibernateQueryRepository<Soul> rep, INHibernateQueryRepository<Tag> tag, INHibernateQueryRepository<Badge> badge, UserImporter import)
        {
            userRepository = rep;
            userimport = import;
            tagRepository = tag;
            badgeRepository = badge;
        }

        List<Badge> GetBadges()
        {
            BadgesRouteMap target = new BadgesRouteMap();
            target.JsonText = true;
            BadgesResult results = target.GetResult();
            List<Badge> badges = new List<Badge>();
            foreach (Badges b in results.badges)
            {
                if (b.award_count <= MAXNUMBEROFUSERSALLOWED )
                {
                    Badge badge = badgeRepository.FindOne(new { sitename = currentSite, siteid = b.badge_id });
                    if (badge == null)
                    {
                        badge = new Badge();
                    }
                    badge.count = b.award_count;
                    badge.description = b.description;
                    badge.name = b.name;
                    badge.rank = b.rank;
                    badge.siteid = b.badge_id;
                    badge.sitename = currentSite;
                    badgeRepository.SaveOrUpdate(badge);
                    badges.Add(badge);
                }
            }
            badgeRepository.DbContext.CommitChanges(true);
            return badges;
        }

        public void Import()
        {
            foreach (Site site in Enum.GetValues(typeof(Site)))
            {
                currentSite = site;
                Api.DefaultTarget = currentSite.ToName();
                Api.ApiKey = "J9pn2Pf2MEWl5OVgaCjmSw";

                badgeRepository.DbContext.BeginTransaction();
                List<Badge> badges = GetBadges();
                badgeRepository.DbContext.CommitTransaction();
                System.Console.WriteLine("Got {0} badges", badges.Count);
                foreach (Badge badge in badges)
                {
                    BadgesByIdRouteMap target = new BadgesByIdRouteMap();
                    target.JsonText = true;
                    target.Parameters.id = new string[] { badge.siteid.ToString() };
                    target.Parameters.page = 1;
                    target.Parameters.pagesize = 100;

                    BadgesByIdResult result = target.GetResult();
                    while (((result.page-1) * result.pagesize < result.total) && (result.users.Length > 0))
                    {
                        badgeRepository.DbContext.BeginTransaction();
                        System.Console.WriteLine("Enumerating badge {0} ({1}) page {2}", badge.name, badge.count, result.page);
                        foreach (Users u in result.users) {
                            Soul soul = userimport.Import(userRepository, currentSite, u);
                            if (soul != null)
                            {
                                badge.AddUser(soul);
                            }
                        }
                        target.Parameters.page++;
                        result = target.GetResult();
                        badgeRepository.Update(badge);
                        badgeRepository.DbContext.CommitChanges(true);
                        badgeRepository.DbContext.CommitTransaction();
                    }
                }
                

                System.Console.WriteLine("{0} api calls left: {1}", Enum.GetName(typeof(Site), site), Api.RemainingRequests);
            }
        }
    }
}
*/