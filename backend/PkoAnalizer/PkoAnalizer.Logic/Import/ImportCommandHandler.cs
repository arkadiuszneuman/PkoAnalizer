﻿using PkoAnalizer.Core.Commands.Import;
using PkoAnalizer.Core.Cqrs.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using PkoAnalizer.Logic.Import.Importers;
using PkoAnalizer.Logic.Import.Db;
using System.Threading.Tasks;
using PkoAnalizer.Core.Cqrs.Event;
using PkoAnalizer.Logic.Import.Events;
using System.IO;

namespace PkoAnalizer.Logic.Import
{
    public class ImportCommandHandler : ICommandHandler<ImportCommand>
    {
        private readonly ILogger<ImportCommandHandler> logger;
        private readonly IEnumerable<IImporter> importers;
        private readonly BankTransactionAccess bankTransactionAccess;
        private readonly IEventsBus eventsBus;

        public ImportCommandHandler(ILogger<ImportCommandHandler> logger,
            IEnumerable<IImporter> importers,
            BankTransactionAccess bankTransactionAccess,
            IEventsBus eventsBus)
        {
            this.logger = logger;
            this.importers = importers;
            this.bankTransactionAccess = bankTransactionAccess;
            this.eventsBus = eventsBus;
        }

        public async Task Handle(ImportCommand command)
        {
            logger.LogInformation("Importing transactions from file");

            var lastOrder = await bankTransactionAccess.GetLastTransactionOrder();
            var transactions = importers.SelectMany(i => i.ImportTransactions(command.FileText, lastOrder).ToList()).ToList();

            var transactionSavedEventTasks = new List<Task>();

            foreach (var transaction in transactions)
            {
                transaction.Order = ++lastOrder;

                var databaseTransaction = await bankTransactionAccess.AddToDatabase(transaction);

                if (databaseTransaction != null)
                    transactionSavedEventTasks.Add(eventsBus.Publish(new TransactionSavedEvent(transaction, databaseTransaction)));
                else
                    --lastOrder;
            }

            Task.WaitAll(transactionSavedEventTasks.ToArray());

            await eventsBus.Publish(new CommandCompletedEvent(command.ConnectionId, command.Id));

            logger.LogInformation("Transactions from file imported");
        }
    }
}
