public class CreateSurvey
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Language { get; set; }
    public List<QuestionViewModel> Questions { get; set; }
}

public class QuestionViewModel
{
    public string Text { get; set; }
    public string Type { get; set; }
    public bool IsRequired { get; set; }
    public List<OptionViewModel> Options { get; set; }
}

public class OptionViewModel
{
    public string Text { get; set; }
}
