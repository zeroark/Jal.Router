﻿using Jal.Router.Impl;
using Jal.Router.Model;
using Jal.Router.Tests.Model;

namespace Jal.Router.Tests.Impl
{
    public class OtherRequestSender : AbstractRequestSender<Request, Response>
    {
        public override Response Send(Request request, EndPoint endPoint)
        {
            return new Response(){Status = "BAD"};
        }
    }
}