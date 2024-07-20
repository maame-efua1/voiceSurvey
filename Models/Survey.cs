namespace TALKPOLL.Models

{
    public class Survey
    {
        public string surveyId { get; set; }
        
        public string title { get; set; }
        
        public string description { get; set; }

        public string cratorId { get; set; }

        public DateTime dateCreated { get; set; }

        public DateTime expiryDate { get; set; }

        public string status { get; set; }
        
        public List<SurveyQuestions> Questions { get; set; }
        
        
    }
    
}
