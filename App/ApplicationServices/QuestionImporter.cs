using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Salient.StackApps;
using Salient.StackApps.Routes;
using Shaml.Core.PersistenceSupport.NHibernate;
using CodeChirp.Core;
using Shaml.Data.NHibernate;
using System.Text.RegularExpressions;

/*
 * CodeChirp is a twitter-like question/comment/answer aggregator for the SO-trilogy where you can follow posts by users, badges or tags. You can also create  your own channel, which other people might follow.

CodeChirp is a web application that periodically harvests information from all registered API sites, and stores them in it's own database. It also hosts an API 

CodeChirp */

namespace CodeChirp.ApplicationServices
{
    public class QuestionImporter
    {
        INHibernateQueryRepository<Soul> userRepository;
        INHibernateQueryRepository<Tag> tagRepository;
        INHibernateQueryRepository<Post> postRepository;
        UserImporter userimport;
        Site currentSite;

        public QuestionImporter()
        {
            userRepository = new NHibernateQueryRepository<Soul>();
            tagRepository = new NHibernateQueryRepository<Tag>();
            postRepository = new NHibernateQueryRepository<Post>();
            userimport = new UserImporter();
        }

        public QuestionImporter(INHibernateQueryRepository<Soul> rep, INHibernateQueryRepository<Tag> tag, INHibernateQueryRepository<Post> post, UserImporter import)
        {
            userRepository = rep;
            userimport = import;
            tagRepository = tag;
            postRepository = post;
        }

        public void ImportQuestion(Questions q)
        {
            Post post = postRepository.FindOne(new { type = PostType.question, sitename = currentSite, siteid = q.question_id });
            if (post == null)
            {
                post = new Post();
            }
            post.body = "<p>"+q.title+"</p>"+q.body;
            post.summary = q.title;
            post.community = q.community_owned;
            post.lastactivity = q.last_activity_date.FromUnixTime();
            post.lastedit = q.last_edit_date > q.creation_date ? q.last_edit_date.FromUnixTime() : q.creation_date.FromUnixTime();
            post.score = q.score;
            post.siteid = q.question_id;
            post.sitename = currentSite;
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
            post.type = PostType.question;
            post.user = userimport.Import(userRepository, currentSite, q.owner);
            post.parent = null;
            postRepository.SaveOrUpdate(post);
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

        public void Import()
        {
            foreach (Site site in Enum.GetValues(typeof(Site)))
            {
                currentSite = site;
                Api.DefaultTarget = currentSite.ToName();
                Api.ApiKey = "";

                QuestionsRouteMap target = new QuestionsRouteMap();
                target.Parameters.pagesize = 100;
                target.Parameters.page = 0;
                target.Parameters.body = true;
                target.Parameters.comments = true;
                target.Parameters.min = -1;
                target.JsonText = true;

                QuestionsResult result = target.GetResult();
                userRepository.DbContext.BeginTransaction();
                foreach (Questions q in result.questions)
                {
                    ImportQuestion(q);
                }
                userRepository.DbContext.CommitTransaction();

                System.Console.WriteLine("{0} api calls left: {1}", Enum.GetName(typeof(Site), site), Api.RemainingRequests);
            }
        }
    }
}
