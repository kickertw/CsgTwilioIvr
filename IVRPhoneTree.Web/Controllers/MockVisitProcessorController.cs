using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using IVRPhoneTree.Web.Models;
using Newtonsoft.Json.Linq;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;

namespace IVRPhoneTree.Web.Controllers
{
    public class MockVisitProcessorController : TwilioController
    {
        public async Task<ActionResult> StartVisit(string digits)
        {
            var selectedOption = digits;

            return digits == "1" ?
                   await ProcessVisitQuestion() :
                   Hangup();
        }

        public async Task<TwiMLResult> ProcessVisitQuestion(string digits = "")
        {
            dynamic nextQuestion = await GetQuestion(digits);
            string whatToSay = nextQuestion.QuestionText;

            if (nextQuestion.PossibleAnswers != null)
            {
                for (int ii = 0; ii < nextQuestion.PossibleAnswers.length; ii++)
                {
                    var answer = nextQuestion.PossibleAnswers[ii];
                    whatToSay = $"For {answer.Name}, press {ii+1}. ";
                }
            }

            var response = new TwilioResponse();

            if (nextQuestion.AudioFileLocation != null && nextQuestion.AudioFileLocation.length > 0)
            {
                response.BeginGather(new {action = Url.Action("ProcessVisitQuestion"), numDigits = nextQuestion.AnswerLength})
                        .Play(nextQuestion.AudioFileLocation);

                if (nextQuestion.PossibleAnswers != null)
                {
                    for (int ii = 0; ii < nextQuestion.PossibleAnswers.length; ii++)
                    {
                        //response.Pause(1);
                        response.Play(nextQuestion.PossibleAnswers[ii].AudioFileLocation);
                    }
                }

                response.EndGather();
            }
            else
            {
                response.BeginGather(new { action = Url.Action("ProcessVisitQuestion"), numDigits = nextQuestion.AnswerLength })
                    .Say(whatToSay, new { voice = "alice", language = "en-US" })
                    .EndGather();
            }

            return TwiML(response);
        }

        public TwiMLResult Hangup()
        {
            var response = new TwilioResponse();
            response.Say("Goodbye!");
            response.Hangup();

            return new TwiMLResult(response);
        }

        protected virtual async Task<JObject> GetQuestion(string digits)
        {
            var jsonRetVal = @"{
                QuestionText: 'What is your subject ID?',
                AnswerType: 'int',
                AnswerLength: '5'
            }";

            JObject retVal = JObject.Parse(jsonRetVal);

            return retVal;

            //string jsonResp = "";
            //dynamic questionObj = new JObject();
            //using (var client = new HttpClient())
            //{
            //    var uri = new System.Uri("");
            //    var ajaxResp = await client.GetAsync(uri);
            //    jsonResp = await ajaxResp.Content.ReadAsStringAsync();
            //}
            //if (jsonResp.Length > 0)
            //{
            //    questionObj = JObject.Parse(jsonResp);
            //}

            //return questionObj;
        }
    }
}
