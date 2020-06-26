using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace E3J.Utilities
{
    /// <summary>
    /// AsynchronousQueryExecutor class
    /// </summary>
    public static class AsynchronousQueryExecutor
    {
        /// <summary>
        /// Calls the specified query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="errorCallback">The error callback.</param>
        public static void Call<T>(IEnumerable<T> query, Action<IEnumerable<T>> callback, Action<Exception> errorCallback)
        {
            Func<IEnumerable<T>, IEnumerable<T>> func = InnerEnumerate<T>;
            IEnumerable<T> result;
            var ar = func.BeginInvoke(
                                query,
                                delegate (IAsyncResult arr)
                                {
                                    try
                                    {
                                        result = ((Func<IEnumerable<T>, IEnumerable<T>>)((AsyncResult)arr).AsyncDelegate).EndInvoke(arr);
                                    }
                                    catch (Exception ex)
                                    {
                                        errorCallback?.Invoke(ex);
                                        return;
                                    }
                                    //errors from inside here are the callbacks problem
                                    //I think it would be confusing to report them
                                    callback(result);
                                },
                                null);
        }
        /// <summary>
        /// Inners the enumerate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        private static IEnumerable<T> InnerEnumerate<T>(IEnumerable<T> query)
        {
            return query;
        }
    }
}
