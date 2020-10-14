using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taki_Client
{
    class Rule
    {
        private string condition;
        private ValidTypes[] priorityList;

        public string Condition { get { return this.condition; } }
        public ValidTypes[] PriorityList { get { return this.priorityList; } }
        
        public Rule(string condition, ValidTypes[] priorityList)
        {
            this.condition = condition;
            this.priorityList = priorityList;
        }

        public bool Match(string condition)
        {
            return condition.Contains(this.condition);
        }

    }
}
