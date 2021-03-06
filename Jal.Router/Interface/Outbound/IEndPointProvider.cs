﻿using System;
using Jal.Router.Model;

namespace Jal.Router.Interface.Outbound
{
    public interface IEndPointProvider
    {
        EndPoint Provide(string name, Type contenttype);

        EndPointSetting Provide<TContent>(EndPoint endPoint, TContent content);
    }
}