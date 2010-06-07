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
                Soul soul = new Soul();
                soul.siteid = user.user_id;
                soul.point = user.reputation;
                soul.name = user.display_name;
                soul.sitename = site;
                soul.gravatar = user.email_hash;
                NpgsqlCommand updater = new NpgsqlCommand("UPDATE Souls SET point = :point, name = :name, gravatar = :gravatar WHERE siteid = :siteid AND sitename = :sitename;",conn);
                SetParameters(updater, soul);
                while (true)
                {
                    int rowsAffected = updater.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        try
                        {
                            NpgsqlCommand inserter = new NpgsqlCommand("INSERT INTO Souls (point,name,gravatar,siteid,sitename) VALUES (:point,:name,:gravatar,:siteid,:sitename);",conn);
                            SetParameters(inserter, soul);
                            inserter.ExecuteNonQuery();
                            break;
                        }
                        catch (NpgsqlException)
                        {
                            // probably race condition, we'll try again.
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                return soul;
            }
            else return null;
        }
    }
}
