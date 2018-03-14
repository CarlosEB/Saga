using System;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace Saga.Util
{
    public static class RetryHelper
    {
        /// <summary>
        /// Method used to Retry N times when a specific LastException occurs
        /// </summary>
        /// <typeparam name="TException">LastException to be checked</typeparam>
        /// <param name="operation">Operation to be executed</param>
        /// <param name="times">Number of times to retry</param>
        /// <param name="retryDelay">Delay between each retry</param>
        /// <param name="logger">Log</param>
        /// <param name="optionalExceptionCheck">Optional check in the exception result that can be used as a plus condition to retry</param>
        /// <param name="optionalFireIfException">Optional callback when a retry occurs</param>
        /// <returns></returns>
        public static async Task RetryAsync<TException>(Func<Task> operation, int times, TimeSpan retryDelay, ILogger logger, Func<Exception, bool> optionalExceptionCheck = null, Action<Exception> optionalFireIfException = null) where TException : Exception
        {
            if (times <= 0)
                throw new ArgumentOutOfRangeException(nameof(times));

            int attempts = 0;
            do
            {
                try
                {
                    attempts++;
                    await operation();
                    break;
                }

                catch (TException ex)
                {
                    await CatchHandle(times, retryDelay, logger, optionalExceptionCheck, optionalFireIfException, ex, attempts);
                }

            } while (true);
        }

        public static async Task<dynamic> RetryAsync<TException>(Func<Task<dynamic>> operation, int times, TimeSpan retryDelay, ILogger logger, Func<Exception, bool> optionalExceptionCheck = null, Action<Exception> optionalFireIfException = null) where TException : Exception
        {
            if (times <= 0)
                throw new ArgumentOutOfRangeException(nameof(times));

            int attempts = 0;
            do
            {
                try
                {
                    attempts++;
                    return await operation();
                }

                catch (TException ex)
                {
                    await CatchHandle(times, retryDelay, logger, optionalExceptionCheck, optionalFireIfException, ex, attempts);
                }

            } while (true);
        }

        private static async Task CatchHandle<TException>(int times, TimeSpan retryDelay, ILogger logger, Func<Exception, bool> optionalExceptionCheck, Action<Exception> optionalFireIfException, TException ex, int attempts) where TException : Exception
        {
            bool check = optionalExceptionCheck?.Invoke(ex) ?? true;

            if (attempts == times || check == false)
            {
                //logger.WriteError($"LastException occured trying to execute. {ex}");
                throw ex;
            }

            //logger.WriteWarn($"LastException on attempt {attempts} of {times}. Will times after sleeping for {retryDelay}.");
            optionalFireIfException?.Invoke(ex);
            await Task.Delay(retryDelay);
        }
    }
}
