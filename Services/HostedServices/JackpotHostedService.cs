using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.ServiceInterfaces;
using Services.SignalrHubs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.HostedServices
{
    public class JackpotHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHubContext<ServiceHub> _context;
        private Timer _timer { get; set; }

        public JackpotHostedService(IServiceScopeFactory serviceScopeFactory, IHubContext<ServiceHub> context)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _context = context;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(JackpotTimerCallback, null, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(500));
            return Task.CompletedTask;
        }

        private void JackpotTimerCallback(object state)
        {
            _ = SendJackpotInfo(state);
        }

        private async Task SendJackpotInfo(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var jackpotService = scope.ServiceProvider.GetRequiredService<IJackpotService>();
                var jackpot =  jackpotService.GetJackpot();
                if (jackpot.Success)
                {
                    await _context.Clients.All.SendAsync("RecieveJackpotAmount", jackpot.Amount);
                }
                else
                {
                    await _context.Clients.All.SendAsync("RecieveError", jackpot.ErrorCode.ToString(),jackpot.DeveloperMessage);
                }
            };
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
