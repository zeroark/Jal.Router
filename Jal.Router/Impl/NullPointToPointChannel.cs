﻿using System;
using Jal.Router.Interface;
using Jal.Router.Model;

namespace Jal.Router.Impl
{
    public class NullPointToPointChannel : IPointToPointChannel
    {
        public void Send(MessageContext context)
        {
            
        }

        public void Listen(Route route, Action<object>[] routeactions, string channelpath)
        {

        }
    }
}