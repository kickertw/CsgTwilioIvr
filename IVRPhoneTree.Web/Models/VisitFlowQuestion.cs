using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IVRPhoneTree.Web.Models
{
    public class VisitFlowQuestion : BaseQuestion
    {
        public Guid OriginalQuestionId { get; set; }
        public int SortOrder { get; set; }
    }
}