namespace TALKPOLL.Models
{
    public class SurveyQuestions
    {
        
        public string questionId { get; set; }
    
        public int surveyId { get; set; }
        
        public string text { get; set; }

        public string type { get; set; }

        public int position { get; set; }
        
        public bool IsRequired { get; set; }

        public List<SurveyOptions> Options { get; set; }
    }
}