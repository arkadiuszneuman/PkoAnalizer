﻿using BankAnalizer.Db;
using BankAnalizer.Db.Models;
using BankAnalizer.Logic.Rules.ViewModels;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAnalizer.Logic.Rules.Db
{
    public class RuleAccess
    {
        private readonly IConnectionFactory connectionFactory;
        private readonly IContextFactory contextFactory;

        public RuleAccess(IConnectionFactory connectionFactory,
            IContextFactory contextFactory)
        {
            this.connectionFactory = connectionFactory;
            this.contextFactory = contextFactory;
        }

        public async Task<IEnumerable<RuleViewModel>> GetRules(Guid userId)
        {
            using var connection = connectionFactory.CreateConnection();
            return await connection.QueryAsync<RuleViewModel>("SELECT * FROM Rules WHERE UserId = @userId", new { userId });
        }

        public async Task<IEnumerable<BankTransaction>> GetBankTransactions(Guid userId)
        {
            using var context = contextFactory.GetContext();
            return await context.BankTransactions
                .Where(t => t.User.Id == userId)
                .Include(c => c.BankTransactionType)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
