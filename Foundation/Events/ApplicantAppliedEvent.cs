using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Events
{
    public class ApplicantAppliedEvent : IntegrationEvent
    {
        public int JobId { get; }
        public int ApplicantId { get; }
        public string Title { get; }

        public ApplicantAppliedEvent(int jobId, int applicantId, string title)
        {
            JobId = jobId;
            ApplicantId = applicantId;
            Title = title;
        }
    }
}
