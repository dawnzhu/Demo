using System;
using System.Transactions;

namespace DotNet.Core.Demo.Utilities.ComponentModel
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class TransactionAttribute : Attribute
    {
        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// 事务范围
        /// </summary>
        public TransactionScopeOption ScopeOption { get; set; }

        /// <summary>
        /// 事务隔离级别
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; }

        public TransactionAttribute()
        {
            Timeout = 60;
            ScopeOption = TransactionScopeOption.Required;
            IsolationLevel = IsolationLevel.ReadCommitted;
        }
    }
}
