﻿using AutoMapper;
using PkoAnalizer.Logic.Rules;
using PkoAnalizer.Logic.Rules.Db;
using PkoAnalizer.Logic.Rules.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PkoAnalizer.Logic.Read.Rule
{
    public class RuleReader
    {
        private readonly RuleAccess ruleAccess;
        private readonly RuleParser ruleParser;
        private readonly IMapper mapper;

        public RuleReader(RuleAccess ruleAccess,
            RuleParser ruleParser,
            IMapper mapper)
        {
            this.ruleAccess = ruleAccess;
            this.ruleParser = ruleParser;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<RuleParsedViewModel>> ReadRules(Guid userId)
        {
            var rules = await ruleAccess.GetRules(userId);
            return mapper.Map<IEnumerable<RuleParsedViewModel>>(ruleParser.Parse(rules));
        }
    }
}
