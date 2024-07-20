namespace TALKPOLL.Models
{
    public class SurveyQuestions
    {
        
        public string questionId { get; set; }
    
        public string SurveyId { get; set; }
        
        public int text { get; set; }

        public int type { get; set; }

        public int position { get; set; }
        
        public bool IsRequired { get; set; }

        public List<SurveyOptions> Options { get; set; }
    }
}