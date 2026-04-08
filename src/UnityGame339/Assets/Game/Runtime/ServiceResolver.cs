using System;
using Game339.Shared;
using Game339.Shared.DependencyInjection;
using Game339.Shared.DependencyInjection.Implementation;
using Game339.Shared.Diagnostics;
using Game339.Shared.Models;
using Game339.Shared.Services;
using Game339.Shared.Services.Implementation;

namespace Game.Runtime
{
    public static class ServiceResolver
    {
        public static T Resolve<T>() => Container.Value.Resolve<T>();

        private static readonly Lazy<IMiniContainer> Container = new Lazy<IMiniContainer>(() =>
        {
            var container = new MiniContainer();

            UnityGameLogger logger = new UnityGameLogger();
            container.RegisterSingletonInstance<IGameLog>(logger);

            ScoreService scoreService = new ScoreService();
            scoreService.DayScore.Value = 0;
            scoreService.TotalScore.Value = 0;
            container.RegisterSingletonInstance(scoreService);


            var stringService = new StringService(logger);
            container.RegisterSingletonInstance<IStringService>(stringService);

            return container;
        });
    }
}
