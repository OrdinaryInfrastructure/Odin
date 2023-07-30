using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Annotations;
using Odin.DesignContracts;
using Odin.Logging;
using Odin.System;

namespace Odin.BackgroundProcessing
{
    /// <summary>
    /// Hangfire based IJobManager
    /// </summary>
    public sealed class HangfireBackgroundProcessor : IBackgroundProcessor
    {
        private readonly ILoggerAdapter<HangfireBackgroundProcessor> _logger;
        private readonly IRecurringJobManagerV2 _recurringJobManager;
        private readonly IBackgroundJobClient _jobClient;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="recurringJobManager"></param>
        /// <param name="logger"></param>
        public HangfireBackgroundProcessor(IRecurringJobManagerV2 recurringJobManager, IBackgroundJobClient jobClient, ILoggerAdapter<HangfireBackgroundProcessor> logger)
        {
            PreCondition.RequiresNotNull(recurringJobManager);
            PreCondition.RequiresNotNull(jobClient);
            PreCondition.RequiresNotNull(logger);
            _recurringJobManager = recurringJobManager;
            _jobClient = jobClient;
            _logger = logger;
            
        }

        /// <summary>
        /// Schedules a once-off job in Hangfire
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="enqueueAt"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Outcome<JobDetails> ScheduleJob<T>([NotNull, InstantHandle] Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt)
        {
            try
            {
                string jobId = _jobClient.Schedule<T>(methodCall, enqueueAt);
                return Outcome.Succeed(new JobDetails(jobId, enqueueAt));
            }
            catch (Exception err)
            {
                string message = $"Exception scheduling {methodCall.Name} for {enqueueAt}. {err.Message}";
                _logger.LogError($"{nameof(ScheduleJob)}: {message}", err);
                return Outcome.Fail<JobDetails>(message);
            }
        }
        
        /// <summary>
        /// Schedules a once-off job in Hangfire
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="enqueueAt"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Outcome<JobDetails> ScheduleJob<T>([NotNull, InstantHandle] Expression<Action<T>> methodCall, DateTimeOffset enqueueAt)
        {
            try
            {
                string jobId = _jobClient.Schedule<T>(methodCall, enqueueAt);
                return Outcome.Succeed(new JobDetails(jobId, enqueueAt));
            }
            catch (Exception err)
            {
                string message = $"Exception scheduling {methodCall.Name} for {enqueueAt}. {err.Message}";
                _logger.LogError($"{nameof(ScheduleJob)}: {message}", err);
                return Outcome.Fail<JobDetails>(message);
            }
        }

        /// <summary>
        /// Ensures a recurring job is registered in Hangfire.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="recurringJobId"></param>
        /// <param name="cronExpression"></param>
        /// <param name="timeZoneInfo"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Outcome AddOrUpdateRecurringJob<T>(
            [NotNull, InstantHandle] Expression<Action<T>> methodCall,
            string recurringJobId, string cronExpression, TimeZoneInfo timeZoneInfo, string queueName = "default")
        {
            try
            {
                _recurringJobManager.AddOrUpdate<T>(recurringJobId, queueName, methodCall, cronExpression, new RecurringJobOptions(){ TimeZone = timeZoneInfo});
                return Outcome.Succeed();
            }
            catch (Exception err)
            {
                string message = $"Error scheduling recurring job {recurringJobId}. {err.Message}";
                _logger.LogError($"{nameof(AddOrUpdateRecurringJob)}: {message}", err);
                return Outcome.Fail(message);
            }
        }

        /// <summary>
        /// Removes a job if it exists
        /// </summary>
        /// <param name="jobName"></param>
        public Outcome RemoveRecurringJob(string jobName)
        {
            try
            {
                _recurringJobManager.RemoveIfExists(jobName);
                return Outcome.Succeed();
            }
            catch (Exception err)
            {
                string message = $"Error removing recurring job {jobName}. {err.Message}";
                _logger.LogError($"{nameof(RemoveRecurringJob)}: {message}", err);
                return Outcome.Fail(message);
            }
        }


        /// <summary>
        /// Schedules a once-off job in Hangfire
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="enqueueIn"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Outcome<JobDetails> ScheduleJob<T>([NotNull, InstantHandle] Expression<Func<T, Task>> methodCall, TimeSpan enqueueIn)
        {
            return ScheduleJob(methodCall, DateTimeOffset.Now.Add(enqueueIn));
        }
        
        /// <summary>
        /// Schedules a once-off job in Hangfire
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="enqueueIn"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Outcome<JobDetails> ScheduleJob<T>([NotNull, InstantHandle] Expression<Action<T>> methodCall, TimeSpan enqueueIn)
        {
            return ScheduleJob(methodCall, DateTimeOffset.Now.Add(enqueueIn));
        }
    }
}