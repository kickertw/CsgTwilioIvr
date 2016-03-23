using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IVRPhoneTree.Web.Models
{
    public class BaseQuestion
    {
        public string QuestionText { get; set; }

        public string Name { get; set; }

        public string AudioFileLocation { get; set; }

        public string HelpText { get; set; }

        public string DisplayType { get; set; }

        public string DataType { get; set; }

        public int FieldSize { get; set; }

        public string DefaultValue { get; set; }

        public IEnumerable<Answer> PossibleAnswers { get; set; }

        public bool IsRequired { get; set; }

        public string Operator { get; set; }

        public string CompareValue { get; set; }

        public string ErrorText { get; set; }

        public bool IsActive { get; set; }

        public bool DisplayOnConfirmation { get; set; }
    }
}