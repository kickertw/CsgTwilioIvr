using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IVRPhoneTree.Web.Models;
using IVRPhoneTree.Web.Test;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;

namespace IVRPhoneTree.Web.Controllers
{
    public class VisitsController : TwilioController
    {
        public ActionResult ProcessVisit(string digits, string subjectId, int currentStep = -1)
        {
            int lastStep = -1;
            var response = new TwilioResponse();

            // if currentStep is -1, we are trying to validate the user and resume where they left off or initiate the visit
            if (currentStep == -1)
            {
                lastStep = MockVisitProcessesor.FindLastStep(subjectId);

                if (lastStep > 1) { 
                    response.Say("We have detected a previous unfinished session and will pick up where we left off.", new { voice="alice", language="en-US" })
                        .Pause(1);
                }
            }
            else
            {
                lastStep = currentStep;

                //TODO: Validate and Record Answer (Will really be handled by visit processor in business api)
                var processResponse = MockVisitProcessesor.ProcessAnswer(digits, lastStep);
                if (processResponse != "OK")
                {
                    response.Say(processResponse);
                    response.Pause(2);
                }
                else
                {
                    lastStep++;
                }
            }

            //Return the next question to the user
            if (lastStep == -1)
            {
                response.Say("You have entered an invalid Subject Number");
                response.Redirect(Url.Action("GetSubjectNumber"));
            }
            else
            {
                var currentQuestion = MockVisitProcessesor.GetFlowQuestion(lastStep);

                if (currentQuestion != null)
                {
                    response.BeginGather(
                        new
                        {
                            action =
                                Url.Action("ProcessVisit",
                                    new {subjectId = subjectId, currentStep = currentQuestion.SortOrder})
                        })
                        .Say(currentQuestion.QuestionText);

                    if (currentQuestion.PossibleAnswers != null)
                    {
                        foreach (var answer in currentQuestion.PossibleAnswers)
                        {
                            response.Say($"For {answer.AnswerText}, press {answer.SortOrder}");
                        }
                    }

                    response.EndGather();
                }
                else
                {
                    //No more questions
                    response.Redirect(Url.Action("Hangup"));
                }
            }

            return TwiML(response);
        }

        public ActionResult ConfirmSubjectNumber(string digits)
        {
            var response = new TwilioResponse();
            var message = "You have entered ";

            response.BeginGather(new { action = Url.Action("ProcessSubjectNumber", new { subjectId = digits }), timeout = 10, numDigits = 1 });
            response.Say(message);

            foreach (var digit in digits)
            {
                response.Say(digit.ToString());
            }

            response.Say("If this is correct, press 1.  To re enter, press 2.");
            response.EndGather();

            return TwiML(response);
        }

        public ActionResult ProcessSubjectNumber(string digits, string subjectId)
        {
            var response = new TwilioResponse();

            if (digits == "1")
            {
                //TODO: 
                //      1) Save subject number with callerSID
                //      2) Initiate the Visit
                response.Redirect(Url.Action("ProcessVisit", new { subjectId = subjectId, digits = digits }));
            }
            else
            {
                response.Redirect(Url.Action("GetSubjectNumber"));
            }

            return TwiML(response);
        }

        //https://www.twilio.com/docs/api/twiml/gather
        public ActionResult GetSubjectNumber()
        {
            var response = new TwilioResponse();
            var message = "Please enter your subject number followed by the pound key.";

            response.BeginGather(new { action = Url.Action("ConfirmSubjectNumber", "Visits"), timeout = 10 })
                .Say(message)
                .EndGather();

            return TwiML(response);
        }

        public ActionResult Hangup()
        {
            var response = new TwilioResponse();
            response.Say("Your questionaire has successfully been recorded for your visit.  Goodbye!")
                    .Pause(1)
                    .Hangup();

            return TwiML(response);
        }
    }
}