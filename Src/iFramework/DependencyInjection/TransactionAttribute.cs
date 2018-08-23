﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using IFramework.Infrastructure;

namespace IFramework.DependencyInjection
{
    public class TransactionAttribute : InterceptorAttribute
    {
        public TransactionAttribute(TransactionScopeOption scope = TransactionScopeOption.Required,
                                    IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            Scope = scope;
            IsolationLevel = isolationLevel;
        }

        public TransactionScopeOption Scope { get; set; }
        public IsolationLevel IsolationLevel { get; set; }

        public override Task<object> ProcessAsync(Func<Task<object>> funcAsync,
                                                  IObjectProvider objectProvider,
                                                  Type targetType,
                                                  object invocationTarget,
                                                  MethodInfo method,
                                                  MethodInfo methodInvocationTarget)
        {
            return TransactionExtension.DoInTransactionAsync(funcAsync,
                                                             IsolationLevel,
                                                             Scope);
        }

        public override object Process(Func<object> func,
                                       IObjectProvider objectProvider,
                                       Type targetType,
                                       object invocationTarget,
                                       MethodInfo method,
                                       MethodInfo methodInvocationTarget)
        {
            return TransactionExtension.DoInTransaction(func,
                                                        IsolationLevel,
                                                        Scope);
        }
    }
}