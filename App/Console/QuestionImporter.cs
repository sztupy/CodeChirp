using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Salient.StackApps;
using Salient.StackApps.Routes;
using CodeChirp.Core;
using System.Text.RegularExpressions;
using System.IO;
using Npgsql;

namespace CodeChirp.ApplicationServices
{
    public class QuestionImporter
    {
        UserImporter userimport;
        Site currentSite;
        TextWriter textwriter;
        NpgsqlConnection conn;

        public QuestionImporter(NpgsqlConnection conn)
        {
            this.conn = conn;
            userimport = new UserImporter();
            textwriter = System.Console.Out;
        }

        public QuestionImporter(NpgsqlConnection conn, UserImporter import, TextWriter tw)
        {
            this.conn = conn;
            userimport = import;
            textwriter = tw;
        }

        public void ImportQuestion(Questions q)
        {
            Post post = new Post();
            post.body = "<p>"+q.title+"</p>"+q.body;
            post.summary = q.title;
            post.community = q.community_owned;
            post.lastactivity = q.last_activity_date.FromUnixTime();
            post.lastedit = q.last_edit_date > q.creation_date ? q.last_edit_date.FromUnixTime() : q.creation_date.FromUnixTime();
            post.score = q.score;
            post.siteid = q.question_id;
            post.sitename = currentSite;
            post.type = PostType.question;
            post.user = userimport.Import(conn, currentSite, q.owner);
            post.parent = null;

            post.tags.Clear();
            foreach (string s in q.tags)
            {
                Tag t = tagRepository.FindOne(new { name = s, site = currentSite });
                if (t == null)
                {
                    t = new Tag();
                    t.name = s;
                    t.site = currentSite;
                    tagRepository.Save(t);
                }
                post.tags.Add(t);
            }
            foreach (Comments c in q.comments)
            {
                ImportComment(post, c);
            }
            foreach (Answers a in q.answers)
            {
                ImportAnswer(post, a);
            }
        }

        public void ImportComment(Post parent, Comments c)
        {
            Post post = postRepository.FindOne(new { type = PostType.or, sitename = currentSite, siteid = c.comment_id });
            if (post == null)
            {
                post = new Post();
            }
            post.body = "<p>"+c.body.Replace("\n","<br />")+"</p>";
            if (c.body.Contains('\n'))
            {
                post.summary = c.body.Remove(c.body.IndexOf('\n'));
            }
            else
            {
                post.summary = c.body;
            }
            post.community = parent.community;
            post.lastactivity = c.creation_date.FromUnixTime();
            post.lastedit = parent.lastedit.ToUnixTime() > c.creation_date ? parent.lastedit : c.creation_date.FromUnixTime();
            post.parent = parent;
            post.score = c.score;
            post.siteid = c.comment_id;
            post.sitename = currentSite;
            post.tags.Clear();
            foreach (Tag t in parent.tags)
            {
                post.tags.Add(t);
            }
            post.type = PostType.or;
            post.user = userimport.Import(userRepository, currentSite, c.owner);
            postRepository.SaveOrUpdate(post);
        }

        public void ImportAnswer(Post parent, Answers a)
        {
            Post post = postRepository.FindOne(new { type = PostType.answer, sitename = currentSite, siteid = a.answer_id });
            if (post == null)
            {
                post = new Post();
            }
            post.body = a.body;
            post.community = parent.community;
            post.lastactivity = a.last_activity_date.FromUnixTime();
            post.lastedit = a.last_edit_date > a.creation_date ? a.last_edit_date.FromUnixTime() : a.creation_date.FromUnixTime();
            post.parent = parent;
            post.score = a.score;
            post.siteid = a.answer_id;
            post.sitename = currentSite;
            string strip = Regex.Replace(a.body, "<.*?>", string.Empty);
            strip = strip.Replace("\r", "").Replace("\n", "");
            string[] elementWords = strip.Split(new char[] { ' ' });
            int i = 0;
            StringBuilder sb = new StringBuilder();
            while (i < elementWords.Length)
            {
                if (i != 0)
                {
                    sb.Append(" ");
                }
                if (sb.Length + elementWords[i].Length < 250)
                {
                    sb.Append(elementWords[i]);
                }
                else
                {
                    sb.Append("...");
                    break;
                }
                i++;
            }        
            post.summary = sb.ToString();
            post.tags.Clear();
            foreach (Tag t in parent.tags)
            {
                post.tags.Add(t);
            }
            post.type = PostType.answer;
            post.user = userimport.Import(userRepository, currentSite, a.owner);
            postRepository.SaveOrUpdate(post);
            foreach (Comments c in a.comments)
            {
                ImportComment(post, c);
            }
        }

        public void Import(int amount)
        {
            foreach (Site site in Enum.GetValues(typeof(Site)))
            {
                currentSite = site;
                Api.DefaultTarget = currentSite.ToName();
                Api.ApiKey = "J9pn2Pf2MEWl5OVgaCjmSw";

                QuestionsRouteMap target = new QuestionsRouteMap();
                target.Parameters.pagesize = 100;
                target.Parameters.page = 0;
                target.Parameters.body = true;
                target.Parameters.comments = true;
                target.Parameters.min = -1;
                target.JsonText = true;

                if (amount == 1)
                {
                    QuestionsResult result = target.GetResult();
                    foreach (Questions q in result.questions)
                    {
                        textwriter.WriteLine("Importing Question {0}", q.title);
                        ImportQuestion(q);
                    }
                }
                else
                {
                    QuestionsResult result = target.GetResult();
                    while ((result.page <= amount) && (result.questions.Length > 0))
                    {
                        textwriter.WriteLine("Loading page {0}", result.page);
                        foreach (Questions q in result.questions)
                        {
                            textwriter.WriteLine("Importing Question {0}", q.title);
                            ImportQuestion(q);
                        }
                        target.Parameters.page++;
                        result = target.GetResult();
                    }
                }
                textwriter.WriteLine("{0} api calls left: {1}", Enum.GetName(typeof(Site), site), Api.RemainingRequests);
            }
        }
    }
}
