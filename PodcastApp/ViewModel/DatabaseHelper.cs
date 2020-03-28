using PodcastApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastApp.ViewModel
{
    public static class DatabaseHelper
    {
        public static void InsertPodcast(Podcast podcast)
        {
            PodcastAppEntities1 dbContext = new PodcastAppEntities1();

            dbContext.Podcasts.Add(podcast);
            Console.WriteLine("{0} inserted into DB", podcast.Title);
            dbContext.SaveChanges();
            dbContext.Dispose();
        }

        public static List<Podcast> GetPodcasts()
        {
            PodcastAppEntities1 dbContext = new PodcastAppEntities1(); //made from entities

            return dbContext.Podcasts.SqlQuery("SELECT * FROM PodcastApp.dbo.Podcast").ToList<Podcast>();
        }

        public static void UpdatePodcast(Podcast podcast)
        {
            PodcastAppEntities1 dbContext = new PodcastAppEntities1();

            var oldPodcast = dbContext.Podcasts.FirstOrDefault<Podcast>(p => p.Id == podcast.Id);

            if (oldPodcast != null)
            {
                oldPodcast.RssLink = podcast.RssLink;
                oldPodcast.ThumbnailFileLocation = podcast.ThumbnailFileLocation;
                oldPodcast.ThumbnailFileUrl = podcast.ThumbnailFileUrl;
                oldPodcast.Title = podcast.Title;
                oldPodcast.Description = podcast.Description;

                dbContext.SaveChanges();
            }
        }

        public static void DeletePodcast(Podcast podcast) 
        {
            if (podcast != null)
            {
                PodcastAppEntities1 dbContext = new PodcastAppEntities1();

                // Need to attach entity to context since it was retrieved by a different context
                dbContext.Podcasts.Attach(podcast);

                dbContext.Podcasts.Remove(podcast);
                dbContext.SaveChanges();
            }
        }
    }
}
