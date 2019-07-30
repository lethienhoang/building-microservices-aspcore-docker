using Jobs.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobs.Api.Services
{
    interface IJobRepository
    {
        Task<IEnumerable<Job>> GetAll();
        Task<Job> Get(int jobId);
        Task<int> AddApplicant(JobApplicant jobApplicant);
    }
}
