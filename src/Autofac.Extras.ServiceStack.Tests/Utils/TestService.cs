using System.Threading.Tasks;
using Autofac.Extras.ServiceStack.Tests.Utils;
using Funq;
using ServiceStack;

namespace Autofac.Extras.ServiceStack.Tests
{
    [Route("/value")]
    public class GetValue : IReturn<GetValueResponse>
    {
        public int Value { get; set; }
    }

    public class GetValueResponse
    {
        public int Value { get; set; }
    }


    public class TestService : Service
    {
        private readonly ILifetimeScope _scope;

        public TestService(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public async Task<GetValueResponse> Get(GetValue _)
        {
            await DoSomethingAsync();

            var holder = _scope.Resolve<ValueHolder>();
            return new GetValueResponse { Value = holder.Value };
        }

        private async Task DoSomethingAsync()
        {
            await Task.Delay(10);
        }
    }


    public class AppHost : AppSelfHostBase
    {
        public AppHost() : base("Test service", typeof(TestService).Assembly) { }

        public override void Configure(Container container)
        {
            GlobalRequestFilters.Add((request, response, dto) =>
            {
                var holder = container.Resolve<ValueHolder>();
                holder.Value = (dto as GetValue).Value;
            });
        }
    }
}
