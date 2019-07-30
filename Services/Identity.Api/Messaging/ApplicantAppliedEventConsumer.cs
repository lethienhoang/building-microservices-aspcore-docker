using Events;
using Identity.Api.Services;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Api.Messaging
{
    public class ApplicantAppliedEventConsumer : IConsumer<ApplicantAppliedEvent>
    {
        private readonly IIdentityRepository _identityRepository;

        public ApplicantAppliedEventConsumer()
        {

        }
 
        public ApplicantAppliedEventConsumer(IIdentityRepository identityRepository)
        {
            _identityRepository = identityRepository;
        }
        public async Task Consume(ConsumeContext<ApplicantAppliedEvent> context)
        {
            await _identityRepository.UpdateUserApplicationCountAsync(context.Message.ApplicantId.ToString());
        }
    }
}
