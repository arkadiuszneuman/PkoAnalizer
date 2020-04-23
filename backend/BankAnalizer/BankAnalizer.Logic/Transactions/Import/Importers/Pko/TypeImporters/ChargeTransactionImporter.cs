﻿using BankAnalizer.Logic.Transactions.Import.Models;

namespace BankAnalizer.Logic.Transactions.Import.Importers.Pko.TypeImporters
{
    public class ChargeTransactionImporter : IPkoTypeImporter
    {
        public ImportedBankTransaction Import(string[] splittedLine)
        {
            var type = splittedLine.Index(2);
            if (type == "Obciążenie" || type == "Opłata")
            {
                return new ImportedBankTransaction
                {
                    OperationDate = splittedLine.Index(0).ConvertToDate(),
                    TransactionDate = splittedLine.Index(1).ConvertToDate(),
                    TransactionType = splittedLine.Index(2),
                    Amount = splittedLine.Index(3).ConvertToDecimal(),
                    Currency = splittedLine.Index(4),
                    Title = splittedLine.Index(6)
                };
            }

            return null;
        }
    }
}