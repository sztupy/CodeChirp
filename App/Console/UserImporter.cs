using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Salient.StackApps.Routes;
using CodeChirp.Core;
using Npgsql;

namespace CodeChirp.ApplicationServices
{
    public class UserImporter
    {
        Dictionary<KeyValuePair<long, Site>, Soul> cache;

        public UserImporter()
        {
            cache = new Dictionary<KeyValuePair<long, Site>, Soul>();
        }

        public Soul Import(NpgsqlConnection conn, Site site, Users user)
        {
            User u = new User();
            u.display_name = user.display_name;
            u.email_hash = user.email_hash;
            u.reputation = user.reputation;
            u.user_id = user.user_id;
            u.user_type = user.user_type;
            return Import(conn, site, u);
        }

        private void SetParameters(NpgsqlCommand command, Soul soul)
        {
            command.Parameters.Add(new NpgsqlParameter("point", NpgsqlTypes.NpgsqlDbType.Integer));
            command.Parameters.Add(new NpgsqlParameter("name", NpgsqlTypes.NpgsqlDbType.Varchar));
            command.Parameters.Add(new NpgsqlParameter("gravatar", NpgsqlTypes.NpgsqlDbType.Varchar));
            command.Parameters.Add(new NpgsqlParameter("siteid", NpgsqlTypes.NpgsqlDbType.Integer));
            command.Parameters.Add(new NpgsqlParameter("sitename", NpgsqlTypes.NpgsqlDbType.Integer));
            command.Parameters[0].Value = soul.point;
            command.Parameters[1].Value = soul.name;
            command.Parameters[2].Value = soul.gravatar;
            command.Parameters[3].Value = soul.siteid;
            command.Parameters[4].Value = soul.sitename;
        }

        public Soul Import(NpgsqlConnection conn, Site site, User user)
        {
            if (user != null)
            {
                Soul soul = null;
                if (cache.TryGetValue(new KeyValuePair<long, Site>(user.user_id, site), out soul))
                {
                    return soul;
                }
                soul = new Soul();
                soul.siteid = user.user_id;
                soul.point = user.reputation;
                soul.name = user.display_name;
                soul.sitename = site;
                soul.gravatar = user.email_hash;
                int retrycount = 0;
                using (conn.BeginTransaction())
                {
                    NpgsqlCommand updater = new NpgsqlCommand("UPDATE Souls SET point = :point, name = :name, gravatar = :gravatar WHERE siteid = :siteid AND sitename = :sitename RETURNING id;", conn);
                    SetParameters(updater, soul);
                    while (true)
                    {
                        object returnedid = updater.ExecuteScalar();
                        if (returnedid == null)
                        {
                            try
                            {
                                NpgsqlCommand inserter = new NpgsqlCommand("INSERT INTO Souls (id,point,name,gravatar,siteid,sitename) VALUES (nextval('Soul_sequence'),:point,:name,:gravatar,:siteid,:sitename);", conn);
                                SetParameters(inserter, soul);
                                inserter.ExecuteNonQuery();
                                NpgsqlCommand selector = new NpgsqlCommand("SELECT FROM currval('Soul_sequence');", conn);
                                returnedid = updater.ExecuteScalar();
                                if (returnedid != null)
                                {
                                    soul.Id = (int)returnedid;
                                }
                                break;
                            }
                            catch (NpgsqlException e)
                            {
                                if ((e.Message.Contains("unique")) && (retrycount<10))
                                {
                                    retrycount++;
                                    // duplicate value usually means race condition, we'll try again, but only a maximum of 10 times.
                                }
                                else
                                {
                                    // some other error, throw to lower level.
                                    throw;
                                }
                            }
                        }
                        else
                        {
                            soul.Id = (int)returnedid;
                            break;
                        }
                    }
                    cache[new KeyValuePair<long, Site>(soul.siteid, soul.sitename)] = soul;
                    return soul;
                }
            }
            else return null;
        }
    }
}
