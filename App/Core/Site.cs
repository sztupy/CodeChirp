using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeChirp.Core
{
    public enum Site
    {
        StackOverflow,
        ServerFault,
        MetaStackOverflow,
        SuperUser
    }

    static public class SiteHelpers
    {
        static public string ToUrl(this Site s) {
            return "http://" + s.ToName() + ".com";
        }

        static public string ToName(this Site s)
        {
            switch (s)
            {
                case Site.MetaStackOverflow: return "meta.stackoverflow";
                case Site.ServerFault: return "serverfault";
                case Site.StackOverflow: return "stackoverflow";
                case Site.SuperUser: return "superuser";
                default: return "";
            }
        }
    }
}
