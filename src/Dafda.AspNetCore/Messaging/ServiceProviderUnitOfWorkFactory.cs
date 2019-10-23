using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Dafda.Messaging
{
    public class ServiceProviderUnitOfWorkFactory : IHandlerUnitOfWorkFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderUnitOfWorkFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IHandlerUnitOfWork CreateForHandlerType(Type handlerType)
        {
            return new ServiceScopedUnitOfWork(_serviceProvider, handlerType, ExecuteInScope);
        }

        private class ServiceScopedUnitOfWork : IHandlerUnitOfWork
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly Type _handlerType;
            private readonly Func<IServiceScope, Func<Task>, Task> _executeInScope;

            public ServiceScopedUnitOfWork(IServiceProvider serviceProvider, Type handlerType, Func<IServiceScope, Func<Task>, Task> executeInScope)
            {
                _serviceProvider = serviceProvider;
                _handlerType = handlerType;
                _executeInScope = executeInScope;
            }

            public async Task Run(Func<object, Task> handlingAction)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var handlerInstance = scope.ServiceProvider.GetRequiredService(_handlerType);

                    await _executeInScope(scope, () => handlingAction(handlerInstance));
                }
            }
        }

        protected virtual Task ExecuteInScope(IServiceScope scope, Func<Task> execute)
        {
            return execute();
        }
    }
}