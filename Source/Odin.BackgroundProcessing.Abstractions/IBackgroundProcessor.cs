using System.Linq.Expressions;
using Odin.System;

namespace Odin.BackgroundProcessing
{
    /// <summary>
    /// Interface to abstract out-of-process background job management 
    /// </summary>
    public interface IBackgroundProcessor
    {
        /// <summary>
        /// Schedules a once-off job
        /// </summary>
        /// <param name="taskExpression"></param>
        /// <param name="enqueueAt"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Outcome<JobDetails> ScheduleJob<T>(Expression<Func<T, Task>> taskExpression, DateTimeOffset enqueueAt);

        /// <summary>
        /// Schedules a once-off job
        /// </summary>
        /// <param name="taskExpression"></param>
        /// <param name="enqueueIn"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Outcome<JobDetails>  ScheduleJob<T>(Expression<Action<T>> taskExpression, TimeSpan enqueueIn);
        
        /// <summary>
        /// Schedules a once-off job
        /// </summary>
        /// <param name="taskExpression"></param>
        /// <param name="enqueueAt"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Outcome<JobDetails> ScheduleJob<T>(Expression<Action<T>> taskExpression, DateTimeOffset enqueueAt);

        /// <summary>
        /// Schedules a once-off job
        /// </summary>
        /// <param name="taskExpression"></param>
        /// <param name="enqueueIn"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Outcome<JobDetails>  ScheduleJob<T>(Expression<Func<T, Task>> taskExpression, TimeSpan enqueueIn);

        /// <summary>
        /// Ensures a recurring job is registered.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="recurringJobName"></param>
        /// <param name="cronExpression"></param>
        /// <param name="timeZoneInfo"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Outcome AddOrUpdateRecurringJob<T>(Expression<Action<T>> methodCall, string recurringJobName, string cronExpression, TimeZoneInfo timeZoneInfo, string queueName = "default");
        
        /// <summary>
        /// Ensures a recurring job is deleted, if it exists...
        /// </summary>
        /// <param name="recurringJobName"></param>
        /// <returns></returns>
        Outcome RemoveRecurringJob(string recurringJobName);

        /// <summary>
        /// Waits for all active jobs to complete
        /// </summary>
        /// <param name="timespan">Time after which server gives up on waiting for processing jobs to complete</param>
        /// <returns></returns>
        Task WaitForJobsToComplete(TimeSpan timespan, CancellationToken cancellationToken);

        /// <summary>
        /// Returns true if there are any active (running) jobs
        /// </summary>
        /// <returns></returns>
        bool HasActiveJobs();

        /// <summary>
        /// Update Job Completion 
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task UpdateJobCompletion(string jobId);

        /// <summary>
        /// Add Job To Track
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="tcs"></param>
        /// <returns></returns>
        Task AddJobToTrack(string jobId, TaskCompletionSource tcs);

        /// <summary>
        /// Stop procesing any new jobs
        /// </summary>
        /// <returns></returns>
        Task StopProcessingNewJobs();

        /// <summary>
        /// Stop any scheduled jobs
        /// </summary>
        /// <returns></returns>
        Task StopScheduledJobs();

    }
}