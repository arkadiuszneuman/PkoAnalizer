﻿using BankAnalizer.Logic.Transactions.Import.Importers.Pko.TypeImporters;
using BankAnalizer.Logic.Transactions.Import.Models;
using FluentAssertions;
using System;
using Xunit;

namespace BankAnalizer.Tests.Logic.Importers.Pko.TypeImporters
{
    public class LoanPaymentImporterTests : BaseUnitTest<LoanPaymentImporter>
    {
        [Fact]
        public void Should_import_valid_loan_payment_transactions()
        {
            //act
            var result = Sut.Import(new[] { "2019-02-15","2019-02-16","Spłata kredytu",
                "-123.48","PLN","+2342.21",
                "Tytuł: KAPITAŁ: 123,48 ODSETKI: 0,00 ODSETKI SKAPIT.: 0,00 ODSETKI KARNE: 0,00 321232123","","","","","",""
            });

            //assert
            result.Should().BeEquivalentTo(new ImportedBankTransaction
            {
                OperationDate = new DateTime(2019, 2, 15),
                TransactionDate = new DateTime(2019, 2, 16),
                TransactionType = "Spłata kredytu",
                Amount = -123.48M,
                Currency = "PLN",
                Title = "KAPITAŁ: 123,48 ODSETKI: 0,00 ODSETKI SKAPIT.: 0,00 ODSETKI KARNE: 0,00 321232123",
            });
        }

        [Fact]
        public void Should_not_import_different_type()
        {
            //act
            var result = Sut.Import(new[] {  "2019-02-15","2019-02-16","Other type",
                "-123.48","PLN","+2342.21",
                "Tytuł: KAPITAŁ: 123,48 ODSETKI: 0,00 ODSETKI SKAPIT.: 0,00 ODSETKI KARNE: 0,00 321232123","","","","","","" });

            //assert
            result.Should().BeNull();
        }
    }
}