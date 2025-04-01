using System.Collections.Concurrent;
using System.Linq.Expressions;
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

        //Dictionary of active jobs
        private static readonly ConcurrentDictionary<string, TaskCompletionSource> ActiveJobs = new();
        public static bool IsNotAllowingNewJobs = false;
        
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="recurringJobManager"></param>
        /// <param name="logger"></param>
        public HangfireBackgroundProcessor(IRecurringJobManagerV2 recurringJobManager, IBackgroundJobClient jobClient,
            ILoggerAdapter<HangfireBackgroundProcessor> logger)
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
        public Outcome<JobDetails> ScheduleJob<T>([NotNull, InstantHandle] Expression<Func<T, Task>> methodCall,
            DateTimeOffset enqueueAt)
        {
            try
            {
                if (IsNotAllowingNewJobs)
                {
                    _logger.LogInformation("No new jobs allowed!");
                    return Outcome.Fail<JobDetails>($"No new jobs allowed!");
                }
                
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
        public Outcome<JobDetails> ScheduleJob<T>([NotNull, InstantHandle] Expression<Action<T>> methodCall,
            DateTimeOffset enqueueAt)
        {
            
            try
            {
                
                if (IsNotAllowingNewJobs)
                {
                    _logger.LogInformation("No new jobs allowed!");
                    return Outcome.Fail<JobDetails>($"No new jobs allowed!");
                }
                
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
                if (IsNotAllowingNewJobs)
                {
                    _logger.LogInformation("No new jobs allowed!");
                    return Outcome.Fail("No new jobs allowed");
                }

                _recurringJobManager.AddOrUpdate<T>(recurringJobId, queueName, methodCall, cronExpression,
                    new RecurringJobOptions() { TimeZone = timeZoneInfo });
                
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
        public Outcome<JobDetails> ScheduleJob<T>([NotNull, InstantHandle] Expression<Func<T, Task>> methodCall,
            TimeSpan enqueueIn)
        {
            return IsNotAllowingNewJobs ? Outcome.Fail<JobDetails>($"No new jobs allowed!") : 
                ScheduleJob(methodCall, DateTimeOffset.Now.Add(enqueueIn));
        }

        /// <summary>
        /// Schedules a once-off job in Hangfire
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="enqueueIn"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Outcome<JobDetails> ScheduleJob<T>([NotNull, InstantHandle] Expression<Action<T>> methodCall,
            TimeSpan enqueueIn)
        {
            return IsNotAllowingNewJobs ? Outcome.Fail<JobDetails>($"No new jobs allowed!") :
                ScheduleJob(methodCall, DateTimeOffset.Now.Add(enqueueIn));
        }

        public async Task WaitForJobsToComplete(TimeSpan timespan, CancellationToken cancellationToken = default)
        {
            try
            {
                var processingJobs = JobStorage.Current.GetMonitoringApi().ProcessingJobs(0, int.MaxValue);
                //Jobs that are processing in Hangfire
                _logger.LogInformation($"Jobs still processing: {processingJobs.Count}");
                //Active Jobs (registered tasks that we are tracking)
                _logger.LogInformation($"Jobs still active: {ActiveJobs.Count}");

                IsNotAllowingNewJobs = false;
                _logger.LogInformation($"No new jobs will be allowed ...");
                
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"Timeout reached while waiting for jobs. Remaining jobs: {JobStorage.Current.GetMonitoringApi().ProcessingCount()}");
            }
            catch (Exception e)
            {
                _logger.LogError("Shutdown with exception: ", e);
            }
        }

        public Task StopScheduledJobs()
        {
            var scheduledJobs = JobStorage.Current.GetMonitoringApi().ScheduledJobs(0, int.MaxValue);
            if (scheduledJobs.Any())
            {
                foreach (var job in scheduledJobs)
                {
                    _logger.LogInformation(
                        $"Scheduled job: [{job.Key}] has been marked for deletion");
                    var hasJobBeenDeleted = BackgroundJob.Delete(job.Key);
                    ActiveJobs.TryRemove(job.Key, out _);
                    _logger.LogInformation(
                        hasJobBeenDeleted 
                            ? $"Scheduled job: [{job.Key}] has been deleted"
                            : $"Scheduled job: [{job.Key}] failed to be deleted!");
                }
            }
            return Task.CompletedTask;
        }
        
        public Task StopProcessingNewJobs()
        {
            IsNotAllowingNewJobs = true;
            _logger.LogInformation($"No new jobs will be allowed ...");
            return Task.CompletedTask;
        }

        public Task UpdateJobCompletion(string jobId)
        {
            if (!ActiveJobs.TryGetValue(jobId, out var tcs)) return Task.CompletedTask;
            
            tcs.SetResult(); // Ensure the task is completed first
            if (ActiveJobs.TryRemove(jobId, out _)) // Now remove the entry
            {
                _logger.LogInformation($"Job {jobId} completed!");
            }

            return Task.CompletedTask;
        }
        
        public Task AddJobToTrack(string jobId, TaskCompletionSource tcs)
        {
            
            ActiveJobs.TryAdd(jobId, tcs);
            
            tcs.Task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.SetException(t.Exception!);
                else if (t.IsCanceled)
                    tcs.SetCanceled();
                else
                    tcs.SetResult();
            }, TaskContinuationOptions.ExecuteSynchronously);
            
            _logger.LogInformation($"Added job to track: {jobId}");
            return Task.CompletedTask;
        }

        public bool HasActiveJobs()
        {
            _logger.LogInformation($"Jobs {ActiveJobs.Count} processing ...");
            return ActiveJobs.Count > 0;
        }
    }
}