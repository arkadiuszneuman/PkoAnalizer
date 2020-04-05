﻿using BankAnalizer.Core.Api;
using PkoAnalizer.Core.Cqrs.Command;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankAnalizer.Infrastructure.Commands
{
    public class RequestUserConnectionCommand : Command
    {
        public string RequestedUsername { get; set; }
    }
}
