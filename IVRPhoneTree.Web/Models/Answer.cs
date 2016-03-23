using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IVRPhoneTree.Web.Models
{
    public class Answer
    {
        public string AnswerText { get; set; }

        public string AnswerValue { get; set; }

        public string AudioFileLocation { get; set; }

        public int SortOrder { get; set; }
    }
}