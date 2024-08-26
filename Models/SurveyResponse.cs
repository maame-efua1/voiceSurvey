namespace TALKPOLL.Models
{
    public class SurveyResponse
{
    public int SurveyId { get; set; }
    public int QuestionId { get; set; }
    public string Text { get; set; }
    public int? SelectedOptionId { get; set; }
}
}