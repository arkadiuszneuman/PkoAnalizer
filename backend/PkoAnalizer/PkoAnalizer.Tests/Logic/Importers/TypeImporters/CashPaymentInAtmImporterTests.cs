﻿using AutofacContrib.NSubstitute;
using FluentAssertions;
using PkoAnalizer.Core.ExtensionMethods;
using PkoAnalizer.Logic.Import.Importers.TypeImporters;
using PkoAnalizer.Logic.Import.Importers.TypeImporters.Extensions;
using PkoAnalizer.Logic.Import.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PkoAnalizer.Tests.Logic.Importers.TypeImporters
{
    public class CashPaymentInAtmImporterTests
    {
        [Fact]
        public void Should_import_cash_payment_in_atc_transactions()
        {
            //arrange
            var sut = new AutoSubstitute().Resolve<CashPaymentInAtmImporter>();

            //act
            var result = sut.Import(new[] {
                "2018-03-23","2018-03-24","Wpłata gotówki we wpłatomacie","+11.21","PLN","+32.22",
                "Tytuł: PKO BP 123123123123",
                "Lokalizacja: Kraj: COUNTRY Miasto: CITY Adres: STREET",
                "Data i czas operacji: 2018-03-23 17:07:35","Oryginalna kwota operacji: 11,21 PLN",
                "Numer karty: 1234*****323","",""
            });

            //assert
            result.Should().BeEquivalentTo(new PkoTransaction
            {
                OperationDate = new DateTime(2018, 3, 23),
                TransactionDate = new DateTime(2018, 3, 24),
                TransactionType = "Wpłata gotówki we wpłatomacie",
                Amount = 11.21M,
                Currency = "PLN",
                Title = "PKO BP 123123123123",
                Extensions = new LocationExtension { 
                        Location = "Kraj: COUNTRY Miasto: CITY Adres: STREET"
                }.ToJson()
            });
        }

        [Fact]
        public void Should_not_import_different_type()
        {
            //arrange
            var sut = new AutoSubstitute().Resolve<CashPaymentInAtmImporter>();

            //act
            var result = sut.Import(new[] { "2018-03-23","2018-03-24","Other type","+11.21","PLN","+32.22",
                "Tytuł: PKO BP 123123123123",
                "Lokalizacja: Kraj: COUNTRY Miasto: CITY Adres: STREET",
                "Data i czas operacji: 2018-03-23 17:07:35","Oryginalna kwota operacji: 11,21 PLN",
                "Numer karty: 1234*****323","",""
            });

            //assert
            result.Should().BeNull();
        }
    }
}