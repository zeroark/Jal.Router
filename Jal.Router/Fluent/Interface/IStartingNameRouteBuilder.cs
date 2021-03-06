﻿namespace Jal.Router.Fluent.Interface
{
    public interface IStartingNameRouteBuilder<THandler, out TData>
    {
        IHandlerBuilder<TContent, THandler, TData> ForMessage<TContent>();
    }
}