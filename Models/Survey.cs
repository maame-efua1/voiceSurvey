namespace TALKPOLL.Models

{
    public class Survey
    {
        public string surveyId { get; set; }
        
        public string title { get; set; }
        
        public string description { get; set; }

        public string creatorName { get; set; }

        public string dateCreated { get; set; }

        public string expiryDate { get; set; }

        public string status { get; set; }

        public string language { get; set; }
        public string responses { get; set; }
        
        public List<SurveyQuestions> Questions { get; set; }
        
        
    }
    
}
