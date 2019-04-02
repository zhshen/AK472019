using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCS.Standard.Library.Core.Threading
{
    public static class SyncExtensions
    {
        public static void DoReadAction(this ReaderWriterLockSlim rwLock, Action action)
        {
            if (rwLock != null && action != null)
            {
                rwLock.EnterReadLock();

                try
                {
                    action();
                }
                finally
                {
                    rwLock.ExitReadLock();
                }
            }
        }

        public static R DoReadFunc<R>(this ReaderWriterLockSlim rwLock, Func<R> func)
        {
            R result = default(R);

            if (rwLock != null && func != null)
            {
                rwLock.EnterReadLock();

                try
                {
                    result = func();
                }
                finally
                {
                    rwLock.ExitReadLock();
                }
            }

            return result;
        }

        public static void DoWriteAction(this ReaderWriterLockSlim rwLock, Action action)
        {
            if (rwLock != null && action != null)
            {
                rwLock.EnterWriteLock();

                try
                {
                    action();
                }
                finally
                {
                    rwLock.ExitWriteLock();
                }
            }
        }

        public static R DoWriteFunc<R>(this ReaderWriterLockSlim rwLock, Func<R> func)
        {
            R result = default(R);

            if (rwLock != null && func != null)
            {
                rwLock.EnterWriteLock();

                try
                {
                    result = func();
                }
                finally
                {
                    rwLock.ExitWriteLock();
                }
            }

            return result;
        }

        public static void DoAction(this SemaphoreSlim semaphore, Action action)
        {
            if (semaphore != null && action != null)
            {
                semaphore.Wait();

                try
                {
                    action();
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public static async Task DoActionAsync(this SemaphoreSlim semaphore, Func<Task> action)
        {
            if (semaphore != null && action != null)
            {
                await semaphore.WaitAsync();

                try
                {
                    await action();
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public static R DoFunc<R>(this SemaphoreSlim semaphore, Func<R> func)
        {
            R result = default(R);

            if (semaphore != null && func != null)
            {
                semaphore.Wait();

                try
                {
                    result = func();
                }
                finally
                {
                    semaphore.Release();
                }
            }

            return result;
        }

        public static async Task<R> DoFuncAsync<R>(this SemaphoreSlim semaphore, Func<Task<R>> func)
        {
            R result = default(R);

            if (semaphore != null && func != null)
            {
                await semaphore.WaitAsync();

                try
                {
                    result = await func();
                }
                finally
                {
                    semaphore.Release();
                }
            }

            return result;
        }
    }
}
