﻿using BankAnalizer.Logic.Transactions.Import.Models;

namespace BankAnalizer.Logic.Transactions.Import.Importers.Ing.TypeImporters
{
    public interface IIngTypeImporter
    {
        public ImportedBankTransaction Import(string[] splittedLine);
    }
}
