using Castle.DynamicProxy.Contributors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gestor.Services
{
    public class CronJobs : IHostedService, IDisposable
    {
        private readonly IServiceProvider services;
        private Timer timerNotificacoes;


        public CronJobs(IServiceProvider _services) {
            services = _services;
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            timerNotificacoes = new Timer(NotificacoesWorker, null, TimeSpan.Zero,
             TimeSpan.FromMinutes(10));

            return Task.CompletedTask;
        }

        private void NotificacoesWorker(object state) {
            using (var scope = services.CreateScope()) {
                // Executar a rotina aqui
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            timerNotificacoes?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose() {
            timerNotificacoes?.Dispose();
        }
    }
}
