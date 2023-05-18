using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Question;
using WebFilm.Core.Enitites.User;
using WebFilm.Core.Interfaces.Repository;
using WebFilm.Core.Interfaces.Services;
using Newtonsoft.Json;
using WebFilm.Core.Enitites.Film;
using WebFilm.Core.Enitites.User.Search;
using WebFilm.Core.Enitites.Answer;

namespace WebFilm.Infrastructure.Repository
{
    public class QuestionRepository : BaseRepository<int, Question>, IQuestionRepository
    {
        IUserContext _userContext;
        public QuestionRepository(IConfiguration configuration, IUserContext userContext) : base(configuration)
        {
            _userContext = userContext;
        }

        public object GetQuestionsAndAnswers(int FilmID)
        {
            using (SqlConnection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Question q where q.FilmID = @FilmID;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@FilmID", FilmID);
                var result = SqlConnection.Query<Question>(sql,parameters);
                var listQues = result.ToList();
                var total = listQues.Count;
                List<QuesAndAns> listQuesAndAns = new List<QuesAndAns>();

                var sql2 = "SELECT * FROM Answer;";
                var result2 = SqlConnection.Query<Answer>(sql2);
                List<Answer> allAnswers = result2.ToList();

                foreach (Question ques in listQues)
                {
                    QuesAndAns quesAndAns = new QuesAndAns();
                    quesAndAns.QuestionID = ques.QuestionID;
                    quesAndAns.question = ques.question;
                    List<Answer> listAns = allAnswers.Where(a => a.QuestionID == ques.QuestionID).ToList();
                    List<AnswerLite> listAnswerLite = new List<AnswerLite>();
                    foreach (Answer ans in listAns)
                    {
                        AnswerLite answerLite = new AnswerLite();
                        answerLite.answer = ans.answer;
                        answerLite.Image = ans.Image;
                        if (ans.RightAnswer == 1)
                        {
                            answerLite.RightAnswer = true;
                        }
                        else
                        {
                            answerLite.RightAnswer = false;
                        }
                        listAnswerLite.Add(answerLite);
                    }
                    listAnswerLite = listAnswerLite.OrderBy(i => Guid.NewGuid()).ToList();
                    quesAndAns.answers = listAnswerLite;
                    listQuesAndAns.Add(quesAndAns);
                }
                
                    return new
                {
                    Data = listQuesAndAns,
                    Total = total,
                };
            }
        }


    }
}
