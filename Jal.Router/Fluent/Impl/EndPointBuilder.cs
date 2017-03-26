﻿using System;
using Jal.Router.Fluent.Interface;
using Jal.Router.Interface;
using Jal.Router.Model;

namespace Jal.Router.Fluent.Impl
{
    public class EndPointBuilder<TExtractor> : IToEndPointBuilder, IReplyToEndPointBuilder, IFromEndPointBuilder, INameEndPointBuilder where TExtractor : IEndPointValueSettingFinder
    {
        private readonly EndPoint _endpoint;

        public EndPointBuilder(EndPoint endpoint)
        {
            _endpoint = endpoint;

            _endpoint.ExtractorType = typeof (TExtractor);
        }

        public IFromEndPointBuilder ForMessage<TMessage>()
        {
            _endpoint.MessageType = typeof (TMessage);

            return this;
        }

        public IToEndPointBuilder From(Func<IEndPointValueSettingFinder, string> fromextractor)
        {
            if (fromextractor == null)
            {
                throw new ArgumentNullException(nameof(fromextractor));
            }

            _endpoint.FromExtractor = fromextractor;

            return this;
        }

        public void ReplyTo(Func<IEndPointValueSettingFinder, string> connectionstringextractor, Func<IEndPointValueSettingFinder, string> pathextractor)
        {
            if (connectionstringextractor == null)
            {
                throw new ArgumentNullException(nameof(connectionstringextractor));
            }

            if (pathextractor == null)
            {
                throw new ArgumentNullException(nameof(pathextractor));
            }

            _endpoint.ReplyToConnectionStringExtractor = connectionstringextractor;

            _endpoint.ReplyToPathExtractor = pathextractor;
        }

        public IReplyToEndPointBuilder To(Func<IEndPointValueSettingFinder, string> connectionstringextractor, Func<IEndPointValueSettingFinder, string> pathextractor)
        {
            if (connectionstringextractor == null)
            {
                throw new ArgumentNullException(nameof(connectionstringextractor));
            }

            if (pathextractor == null)
            {
                throw new ArgumentNullException(nameof(pathextractor));
            }

            _endpoint.ToConnectionStringExtractor = connectionstringextractor;

            _endpoint.ToPathExtractor = pathextractor;

            return this;
        }
    }
}