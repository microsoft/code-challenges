using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.Visitors.Model
{
    /// <summary>
    /// Question entity
    /// </summary>
    public class Question
    {
        /// <summary>
        /// UniqueId
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// Question Text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Answer
        /// </summary>
        public string Answer { get; set; }

    }
}
