using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessRuleEngine.Entities
{
    public partial class RuleSetRule
    {
        public string Id { get; set; }
        public int RuleSetId { get; set; }
        public string PropertyName { get; set; }
        public string Operation { get; set; }
        public string Value { get; set; }
        public string RuleName { get; set; }
    }
}
