using PodcastApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastApp.ViewModel
{
    public class DatabaseHelper
    {
        public static void InsertPodcast(Podcast podcast)
        {
            PodcastAppEntities1 dbContext = new PodcastAppEntities1();

            dbContext.Podcasts.Add(podcast);
            Console.WriteLine("{0} inserted into DB", podcast.Title);
            dbContext.SaveChanges();
        }

        public static List<Podcast> GetPodcasts()
        {
            PodcastAppEntities1 dbContext = new PodcastAppEntities1(); //made from entities

            return dbContext.Podcasts.SqlQuery("SELECT * FROM PodcastApp.dbo.Podcast").ToList<Podcast>();
        }

        public static void UpdatePodcast(Podcast podcast)
        {
            //TODO: Implement retrieve podcast functionality
        }

        public static void DeletePodcast(Podcast podcast) 
        { 
            //TODO: Implement delete podcast functionality
        }
    }
}
