﻿using System;

namespace PkoAnalizer.Core
{
    public class DatabaseException : BankAnalizerException
    {
        public DatabaseException(string message) : base(message)
        {
        }

        public DatabaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
