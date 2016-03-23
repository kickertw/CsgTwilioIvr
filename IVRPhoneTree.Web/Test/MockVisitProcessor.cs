using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IVRPhoneTree.Web.Models;

namespace IVRPhoneTree.Web.Test
{
    public static class MockVisitProcessesor
    {
        public static VisitFlowConfig GetFlowData()
        {
            VisitFlowConfig config = new VisitFlowConfig();

            config.Questions = new List<VisitFlowQuestion>();

            config.Questions.Add(new VisitFlowQuestion()
            {
                QuestionText = "What is your date of birth? Use 2 digits for your month, 2 digits for your day, and 4 digits for the year",
                DataType = "DateTime",
                FieldSize = 8,
                SortOrder = 1
            });

            config.Questions.Add(new VisitFlowQuestion()
            {
                QuestionText = "What is your gender?",
                DataType = "String",
                FieldSize = 1,
                PossibleAnswers = new List<Answer>()
                {
                    { new Answer() { AnswerText = "Male", SortOrder = 1}  },
                    { new Answer() { AnswerText = "Female", SortOrder = 2}  }
                },
                SortOrder = 2
            });

            config.Questions.Add(new VisitFlowQuestion()
            {
                QuestionText = "On a scale of 1 to 10, 10 being the worst, how much pain are you experiencing?",
                FieldSize = 2,
                DataType = "Integer",
                Operator = "Less Than",
                CompareValue = "11",
                SortOrder = 3
            });

            return config;
        }

        public static int FindLastStep(string subjectId)
        {
            if (subjectId.StartsWith("1"))
            {
                return 1;
            }
            else if (subjectId.StartsWith("2"))
            {
                return 2;
            }
            else
            {
                return -1;
            }
        }

        public static string ProcessAnswer(string digits, int lastStep)
        {
            var retVal = "OK";
            var currentQuestion = GetFlowData().Questions.Find(i => i.SortOrder == lastStep);

            if (currentQuestion.PossibleAnswers != null && currentQuestion.PossibleAnswers.Any())
            {
                var selectedAnswer = currentQuestion.PossibleAnswers.FirstOrDefault(i => i.SortOrder.ToString() == digits);

                if (selectedAnswer == null)
                {
                    retVal = $"I'm sorry, {digits} is not a valid option.";
                }
            }
            else
            {
                if (currentQuestion.DataType.ToLower() == "integer")
                {
                    int intAnswer = -1;
                    int.TryParse(digits, out intAnswer);
                    bool isValidInt = intAnswer > -1;

                    if (isValidInt)
                    {
                        switch (currentQuestion.Operator)
                        {
                            case "Greater Than":
                                if (intAnswer <= int.Parse(currentQuestion.CompareValue))
                                {
                                    retVal = "I'm sorry.  Your entry is not within the acceptable range.";
                                }
                                break;
                            case "Less Than":
                                if (intAnswer >= int.Parse(currentQuestion.CompareValue))
                                {
                                    retVal = "I'm sorry.  Your entry is not within the acceptable range.";
                                }
                                break;
                        }
                    }
                }
                else if (currentQuestion.DataType.ToLower() == "datetime")
                {
                    DateTime dt = DateTime.MinValue;
                    string newDt = digits.Substring(0, 2) + "/" + digits.Substring(2, 2) + "/" + digits.Substring(4);
                    if (!DateTime.TryParse(newDt, out dt))
                    {
                        var errDate = digits.ToCharArray();
                        string errDigits = errDate.Aggregate("", (current, digit) => current + (digit + ", "));

                        retVal = $"You have entered an invalid date of {errDigits}. ";
                    }
                }
            }

            return retVal;
        }

        public static VisitFlowQuestion GetFlowQuestion(int order)
        {
            return GetFlowData().Questions.FirstOrDefault(i => i.SortOrder == order);
        }
    }
}