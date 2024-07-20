using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using TALKPOLL.Models;

public class SurveyController : Controller
{
    
    /*public IActionResult Index()
        {
            var surveyData = GetSurveyData();
            return View(surveyData);
        }

        private List<SurveyQuestion> GetSurveyData()
        {
            var surveyQuestions = new List<SurveyQuestion>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string queryQuestions = "SELECT Id, Question FROM SurveyQuestions";
                using (var commandQuestions = new SqlCommand(queryQuestions, connection))
                {
                    using (var readerQuestions = commandQuestions.ExecuteReader())
                    {
                        while (readerQuestions.Read())
                        {
                            var surveyQuestion = new SurveyQuestion
                            {
                                Id = readerQuestions.GetInt32(0),
                                Question = readerQuestions.GetString(1),
                                Options = new List<SurveyOption>()
                            };

                            surveyQuestions.Add(surveyQuestion);
                        }
                    }
                }

                string queryOptions = "SELECT Id, OptionText, SurveyQuestionId FROM SurveyOptions";
                using (var commandOptions = new SqlCommand(queryOptions, connection))
                {
                    using (var readerOptions = commandOptions.ExecuteReader())
                    {
                        while (readerOptions.Read())
                        {
                            var surveyOption = new SurveyOption
                            {
                                Id = readerOptions.GetInt32(0),
                                OptionText = readerOptions.GetString(1),
                                SurveyQuestionId = readerOptions.GetInt32(2)
                            };

                            var question = surveyQuestions.Find(q => q.Id == surveyOption.SurveyQuestionId);
                            if (question != null)
                            {
                                question.Options.Add(surveyOption);
                            }
                        }
                    }
                }
            }

            return surveyQuestions;
        }*/
        public IActionResult Index()
    {
        return View();
    }
}
