using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blue_Ribbon.Controllers
{
    public class ParsedReview
    {
        public string Title { get; set; }
        public string ASIN { get; set; }
        public string Link { get; set; }
        public DateTime ReviewDate { get; set; }
        public string RawText { get; set; }
        public int Rating
        {
            get
            {
                int stars = 0;
                var match = Regex.Match(Title, "[1-5][ ]star", RegexOptions.Compiled);
                String nums = new String(match.ToString().Where(Char.IsDigit).ToArray());
                stars = int.Parse(nums);
                return stars;
            }
        }
        public int ReviewLength
        {
            get
            {
                //Extract actual review text from RawText field which indcludes other text.
                string reviewWords = Regex.Match(RawText, "(?<=<div class=\\\"reviewText\\\">).*?(?=</div>)", RegexOptions.Compiled).ToString();
                //Remove HTML formatting from actual review text.
                reviewWords = Regex.Replace(reviewWords, @"<[^>]+>|&nbsp;", "").Trim();
                //Count words in review.
                int c = 0;
                for (int i = 1; i < reviewWords.Length; i++)
                {
                    if (char.IsWhiteSpace(reviewWords[i - 1]) == true)
                    {
                        if (char.IsLetterOrDigit(reviewWords[i]) == true ||
                            char.IsPunctuation(reviewWords[i]))
                        {
                            c++;
                        }
                    }
                }
                if (reviewWords.Length > 2)
                {
                    c++;
                }
                return c;

            }
        }
        public bool VerfiedPurchase
        {
            get
            {
                return RawText.Contains(">Verified Purchase<");
            }
        }

        public bool HasDisclaimer
        {
            //not foolproof! But a start.
            get
            {
                bool disclaimerwordsfound = false;
                if (RawText.Contains("discount")) { disclaimerwordsfound = true; }
                if (RawText.Contains("discounted")) { disclaimerwordsfound = true; }
                if (RawText.Contains("free")) { disclaimerwordsfound = true; }
                if (RawText.Contains("unbiased")) { disclaimerwordsfound = true; }
                if (RawText.Contains("honest")) { disclaimerwordsfound = true; }
                if (RawText.Contains("exchange")) { disclaimerwordsfound = true; }

                return disclaimerwordsfound;

            }
        }

        public bool HasVideo
        {
            get
            {
                return RawText.Contains(">See video on Amazon.com</a>");
            }
        }

        public bool HasPhoto
        {
            get
            {
                return RawText.Contains("<img class=\"review-image-thumbnail");
            }
        }
    }
}
