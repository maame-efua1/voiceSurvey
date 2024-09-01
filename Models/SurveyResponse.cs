namespace TALKPOLL.Models
{
public class SurveyResponse
{
    public int QuestionId { get; set; }
    public string? Answer_Text { get; set; }
    public string? Answer_Option { get; set; }
    public string? Answer_Options { get; set; }
}
}