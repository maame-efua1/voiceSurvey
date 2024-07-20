namespace TALKPOLL.Models
{
public class SurveyOptions
   {
        public int optionId { get; set; }

        public int questionId { get; set; }

        public string text { get; set; }

        public int position { get; set; }
    }
}