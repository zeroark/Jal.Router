using System;
using System.Diagnostics;
using System.Linq;
using Jal.Router.Interface;
using Jal.Router.Interface.Inbound;
using Jal.Router.Interface.Management;
using Jal.Router.Interface.Outbound;
using Jal.Router.Model;
using Jal.Router.Model.Outbount;

namespace Jal.Router.Impl.Outbound
{
    public class Bus : IBus
    {
        private readonly IEndPointProvider _provider;

        private readonly IComponentFactory _factory;

        private readonly IConfiguration _configuration;

        public Bus(IEndPointProvider provider, IComponentFactory factory, IConfiguration configuration)
        {
            _provider = provider;
            _factory = factory;
            _configuration = configuration;
        }

        private void Send<TContent>(OutboundMessageContext<TContent> message, Options options)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var channel = _factory.Create<IPointToPointChannel>(_configuration.PointToPointChannelType);

            var loggers = _configuration.BusLoggerTypes.Select(x => _factory.Create<IBusLogger>(x)).ToArray();

            var interceptor = _factory.Create<IBusInterceptor>(_configuration.BusInterceptorType);

            Array.ForEach(loggers, x => x.OnSendEntry(message, options));

            interceptor.OnSendEntry(message, options);

            try
            {
                if (!string.IsNullOrWhiteSpace(message.ToConnectionString) && !string.IsNullOrWhiteSpace(message.ToPath))
                {
                    channel.Send(message);

                    Array.ForEach(loggers, x => x.OnSendSuccess(message, options));

                    interceptor.OnSendSuccess(message, options);
                }
            }
            catch (Exception ex)
            {
                Array.ForEach(loggers, x => x.OnSendError(message, options, ex));

                interceptor.OnSendError(message, options, ex);

                throw;
            }
            finally
            {
                stopwatch.Stop();

                Array.ForEach(loggers, x => x.OnSendExit(message, options, stopwatch.ElapsedMilliseconds));

                interceptor.OnSendExit(message, options);
            }
        }

        public void Send<TContent>(TContent content, EndPointSetting endpoint, Origin origin, Options options)
        {
            var message = new OutboundMessageContext<TContent>
            {
                Id = options.Id,
                Content = content,
                ToConnectionString = endpoint.ToConnectionString,
                ToPath = endpoint.ToPath,
                Origin = origin,
                Headers = options.Headers,
                Version = options.Version,
                ScheduledEnqueueDateTimeUtc = options.ScheduledEnqueueDateTimeUtc,
                RetryCount = options.RetryCount,
                Saga = options.Saga,
                ContentType = typeof(TContent)
            };

            Send(message, options);
        }

        public void Send<TContent>(TContent content, Options options)
        {
            var endpoints = _provider.Provide<TContent>(options.EndPointName);

            foreach (var endpoint in endpoints)
            {
                var setting = _provider.Provide(endpoint, content);

                var origin = endpoint.Origin;

                Send(content, setting, origin, options);
            }
        }

        public void Send<TContent>(TContent content, Origin origin, Options options)
        {
            var endpoints = _provider.Provide<TContent>(options.EndPointName);

            foreach (var endpoint in endpoints)
            {
                var setting = _provider.Provide(endpoint, content);

                if (string.IsNullOrWhiteSpace(origin.Name))
                {
                    origin.Name = endpoint.Origin.Name;
                }

                if (string.IsNullOrWhiteSpace(origin.Key))
                {
                    origin.Key = endpoint.Origin.Key;
                }

                Send(content, setting, origin, options);
            }
        } 
        public void Publish<TContent>(TContent content, Options options)
        {
            var endpoints = _provider.Provide<TContent>(options.EndPointName);

            foreach (var endpoint in endpoints)
            {
                var setting = _provider.Provide(endpoint, content);

                var origin = endpoint.Origin;

                Publish(content, setting, origin, options);
            }
        }

        public void Publish<TContent>(TContent content, Origin origin, Options options)
        {
            var endpoints = _provider.Provide<TContent>(options.EndPointName);

            foreach (var endpoint in endpoints)
            {
                var setting = _provider.Provide(endpoint, content);

                if (string.IsNullOrWhiteSpace(origin.Name))
                {
                    origin.Name = endpoint.Origin.Name;
                }

                if (string.IsNullOrWhiteSpace(origin.Key))
                {
                    origin.Key = endpoint.Origin.Key;
                }

                Publish(content, setting, origin, options);
            }
        }

        public void Publish<TContent>(TContent content, EndPointSetting endpoint, Origin origin, Options options)
        {
            var message = new OutboundMessageContext<TContent>
            {
                Id = options.Id,
                Content = content,
                ToConnectionString = endpoint.ToConnectionString,
                ToPath = endpoint.ToPath,
                Origin = origin,
                Headers = options.Headers,
                Version = options.Version,
                ScheduledEnqueueDateTimeUtc = options.ScheduledEnqueueDateTimeUtc,
                RetryCount = options.RetryCount,
                Saga = options.Saga,
                ContentType = typeof(TContent)
            };

            Publish(message, options);
        }

        private void Publish<TContent>(OutboundMessageContext<TContent> message, Options options)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var channel = _factory.Create<IPublishSubscribeChannel>(_configuration.PublishSubscribeChannelType);

            var loggers = _configuration.BusLoggerTypes.Select(x => _factory.Create<IBusLogger>(x)).ToArray();

            var interceptor = _factory.Create<IBusInterceptor>(_configuration.BusInterceptorType);

            Array.ForEach(loggers, x => x.OnPublishEntry(message, options));

            interceptor.OnPublishEntry(message, options);

            try
            {
                if (!string.IsNullOrWhiteSpace(message.ToConnectionString) && !string.IsNullOrWhiteSpace(message.ToPath))
                {
                    channel.Send(message);

                    Array.ForEach(loggers, x => x.OnPublishSuccess(message, options));

                    interceptor.OnPublishSuccess(message, options);
                }
            }
            catch (Exception ex)
            {
                Array.ForEach(loggers, x => x.OnPublishError(message, options, ex));

                interceptor.OnPublishError(message, options, ex);

                throw;
            }
            finally
            {
                stopwatch.Stop();

                Array.ForEach(loggers, x => x.OnPublishExit(message, options, stopwatch.ElapsedMilliseconds));

                interceptor.OnPublishExit(message, options);
            }
        }

        public void FireAndForget<TContent>(TContent content, EndPointSetting endpoint, Origin origin, Options options)
        {
            var message = new OutboundMessageContext<TContent>
            {
                Id = options.Id,
                Content = content,
                Origin = origin,
                ToConnectionString = endpoint.ToConnectionString,
                ToPath = endpoint.ToPath,
                Headers = options.Headers,
                Version = options.Version,
                ScheduledEnqueueDateTimeUtc = options.ScheduledEnqueueDateTimeUtc,
                RetryCount = options.RetryCount,
                Saga = options.Saga,
                ContentType = typeof(TContent)
            };

            message.Origin.Key = string.Empty;

            Send(message, options);
        }

        public void FireAndForget<TContent>(TContent content, Options options)
        {
            var endpoints = _provider.Provide<TContent>(options.EndPointName);

            foreach (var endpoint in endpoints)
            {
                var setting = _provider.Provide(endpoint, content);

                FireAndForget(content, setting, new Origin() {Key = endpoint.Origin.Key, Name = endpoint.Origin.Name }, options);
            }
        }

        public void FireAndForget<TContent>(TContent content, Origin origin, Options options)
        {
            var endpoints = _provider.Provide<TContent>(options.EndPointName);

            foreach (var endpoint in endpoints)
            {
                var setting = _provider.Provide(endpoint, content);

                if (string.IsNullOrWhiteSpace(origin.Name))
                {
                    origin.Name = endpoint.Origin.Name;
                }

                if (string.IsNullOrWhiteSpace(origin.Key))
                {
                    origin.Key = endpoint.Origin.Key;
                }

                FireAndForget(content, setting, origin, options);
            }
        }
    }
}