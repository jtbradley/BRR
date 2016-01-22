using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Blue_Ribbon.Controllers
{
    public class ParsedFeed
    {
        public string PublishDate { get; set; }
        public List<ParsedReview> FeedReviews { get; set; }

        public ParsedFeed(string userID)
        {
            String URLString = String.Format("https://www.amazon.com/rss/people/{0}/reviews/ref=cm_rss_member_rev_manlink", userID);
            XmlTextReader reader = new XmlTextReader(URLString);
            this.FeedReviews = new List<ParsedReview> { };
            reader.ReadToFollowing("link");
            this.PublishDate = reader.ReadElementContentAsString();
            while (reader.ReadToFollowing("item"))
            {
                ParsedReview review = new ParsedReview();
                reader.ReadToFollowing("title");
                review.Title = reader.ReadElementContentAsString().Trim();
                reader.ReadToFollowing("guid");
                string tempguid = reader.ReadElementContentAsString();
                review.ASIN = tempguid.Substring(tempguid.Length - 10);
                reader.ReadToFollowing("link");
                review.Link = reader.ReadElementContentAsString();
                reader.ReadToFollowing("pubDate");
                review.ReviewDate = DateTime.Parse(reader.ReadElementContentAsString());
                reader.ReadToFollowing("description");
                review.RawText = reader.ReadElementContentAsString();
                this.FeedReviews.Add(review);

            }
        }
    }
}