using System;
using System.Linq;
using System.Reflection;
using System.Transactions;
using Castle.DynamicProxy;
using DotNet.Core.Demo.Utilities.ComponentModel;
using DotNet.Standard.Common.Utilities;

namespace DotNet.Core.Demo.Models
{
    /// <summary>
    /// 服务方法拦截器
    /// </summary>
    public class ServiceInterceptor : IInterceptor
    {
        // 是否开发模式
        private readonly bool _isDev = false;
        public void Intercept(IInvocation invocation)
        {
            if (!_isDev)
            {
                var methodInfo = invocation.MethodInvocationTarget;
                if (methodInfo == null)
                {
                    methodInfo = invocation.Method;
                }
                var transaction = methodInfo.GetCustomAttributes<TransactionAttribute>(true).FirstOrDefault();
                if (transaction != null)
                {
                    var transactionOptions = new TransactionOptions
                    {
                        IsolationLevel = transaction.IsolationLevel,
                        Timeout = new TimeSpan(0, 0, transaction.Timeout)
                    };
                    //设置事务隔离级别
                    //设置事务超时时间为60秒
                    using (var scope = new TransactionScope(transaction.ScopeOption, transactionOptions))
                    {
                        try
                        {
                            //实现事务性工作
                            invocation.Proceed();
                            scope.Complete();
                        }
                        catch (Exception er)
                        {
                            LogUtil.WriteLog(er);
                            // 记录异常
                            throw;
                        }
                    }
                }
                else
                {
                    // 没有事务时直接执行方法
                    invocation.Proceed();
                }
            }
            else
            {
                // 开发模式直接跳过拦截
                invocation.Proceed();
            }

            //结果集返回前，自动完成一些操作
            if (invocation.ReturnValue is ResultInfo ret)
            {
                ret.Auto();
            }
        }
    }
}
