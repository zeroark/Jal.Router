﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jal.Router.Model
{

    public class OutboundMessageContext<TContent> : OutboundMessageContext
    {
        public TContent Content { get; set; }
    }


    public class OutboundMessageContext
    {
        public string Id { get; set; }
        public string ToConnectionString { get; set; }
        public string ToPath { get; set; }
        public string ReplyToConnectionString { get; set; }
        public string ReplyToPath { get; set; }
        public string From { get; set; }
    }
}