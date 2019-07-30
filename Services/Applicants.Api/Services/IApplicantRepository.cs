using Applicants.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Applicants.Api.Services
{
    interface IApplicantRepository
    {
        Task<IEnumerable<Applicant>> GetAll();
        Task<int> AddApplicantSubmission(ApplicantSubmission applicantSubmission);
    }
}
