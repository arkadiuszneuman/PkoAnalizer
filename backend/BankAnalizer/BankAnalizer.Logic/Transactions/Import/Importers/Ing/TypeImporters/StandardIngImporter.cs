﻿using BankAnalizer.Core.ExtensionMethods;
using BankAnalizer.Logic.Transactions.Import.Importers.Pko.TypeImporters;
using BankAnalizer.Logic.Transactions.Import.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Dynamic;

namespace BankAnalizer.Logic.Transactions.Import.Importers.Ing.TypeImporters
{
    public class StandardIngImporter : IIngTypeImporter
    {
        private readonly ILogger<StandardIngImporter> logger;

        public StandardIngImporter(ILogger<StandardIngImporter> logger)
        {
            this.logger = logger;
        }

        public ImportedBankTransaction Import(string[] splittedLine)
        {
            if (!IsValid(splittedLine))
                return null;
            
            return new ImportedBankTransaction
            {
                BankName = "ING",
                TransactionDate = splittedLine.Index(0).ConvertToDate(),
                OperationDate = splittedLine.Index(1).ConvertToDate(),
                TransactionType = ConvertTransactionType(splittedLine.Index(6)).Value.ToString(),
                Amount = splittedLine.Index(8).ConvertToDecimal(),
                Currency = splittedLine.Index(9),
                Title = splittedLine.Index(3),
                Extensions = GetExtensions(splittedLine).ToJson()
            };
        }

        private bool IsValid(string[] splittedLine)
        {
            if (splittedLine.Length < 7)
                return false;

            if (string.IsNullOrEmpty(splittedLine.Index(6)))
                return false;

            if (string.IsNullOrEmpty(splittedLine.Index(1)))
                return false;

            var transactionType = ConvertTransactionType(splittedLine.Index(6));
            if (transactionType == null)
            {
                logger.LogWarning("Unhandled type {type}", splittedLine.Index(6));
                return false;
            }

            return true;
        }

        private static object GetExtensions(string[] splittedLine)
        {
            dynamic extensions = new ExpandoObject();
            extensions.ContractorName = splittedLine.Index(2);

            var billNumber = splittedLine.Index(4).Trim('\'').Trim();
            if (!string.IsNullOrEmpty(billNumber))
                extensions.BillNumber = billNumber;

            var bankName = splittedLine.Index(5).Trim();
            if (!string.IsNullOrEmpty(bankName))
                extensions.BankName = bankName;

            return extensions;
        }

        private static TransactionTypeEnum? ConvertTransactionType(string transactionType) =>
            transactionType.ToUpper() switch
        {
            _ when transactionType.StartsWith("TR.KART") => TransactionTypeEnum.CardTransaction,
            _ when transactionType.StartsWith("TR.BLIK") => TransactionTypeEnum.Blik,
            _ when transactionType.StartsWith("PRZELEW") => TransactionTypeEnum.Transfer,
            _ when transactionType.StartsWith("ST.ZLEC") => TransactionTypeEnum.StandingOrder,
            _ => null
        };
    }
}
