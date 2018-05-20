using Sitecore.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreMaster.Foundation.Bucketing.Rules
{
    public class MatchedRule<T>
        where T : RuleContext
    {
        public Rule<T> Rule { get; set; }
        public T Context { get; set; }
    }
}
