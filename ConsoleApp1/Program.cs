﻿namespace ConsoleApp1
{
    using System;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Logging;
    using Serilog;
    using LogLevel = Microsoft.Extensions.Logging.LogLevel;
    public class Program
    {
        private static void Main(string[] args)
        {

            RunProgramRunExample().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to close the application");
            Console.ReadKey();
        }

        private static async Task RunProgramRunExample()
        {
            try
            {
                // Grab the Scheduler instance from the Factory
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();

                // and start it off
                await scheduler.Start();

                // define the job and tie it to our HelloJob class
                IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                // Trigger the job to run now, and then repeat every 10 seconds
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(10)
                        .RepeatForever())
                    .Build();

                // Tell quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(job, trigger);

                // some sleep to show what's happening
                await Task.Delay(TimeSpan.FromDays(15));

                // and last shut down the scheduler when you are ready to close your program
                await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }

     
    }

    public class HelloJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await context.Scheduler.PauseAll();
            Log.Logger.Error("başla ScrapingBusiness");
            var business = new ScrapingBusiness();
            business.Start();
            Log.Logger.Error("bitti ScrapingBusiness");
            await context.Scheduler.ResumeAll();
        }
    }

    //internal class Program
    //{
    //    private static IScheduler _scheduler;
    //    public static IServiceProvider ServiceProvider { get; set; }
    //    private static IConfigurationRoot Configuration { get; set; }
    //    private static void Main(string[] args)
    //    {
    //        try
    //        {

    //            Log.Logger = new LoggerConfiguration().WriteTo.File("log.txt", rollingInterval: RollingInterval.Day).CreateLogger();
    //            Log.Logger.Error("başla");
    //            InitializeJobs().GetAwaiter().GetResult();
    //            Log.Logger.Error("Bitir");

    //        }
    //        catch (Exception e)
    //        {
    //            Log.Logger.Error(e, e.Message);
    //        }
    //        Console.WriteLine("Zamanlanmış görevler başladı!");
    //        Console.ReadLine();
    //    }

    //    private static async Task InitializeJobs()
    //    {
    //        _scheduler = await new StdSchedulerFactory().GetScheduler();
    //        await _scheduler.Start();

    //        var userEmailsJob = JobBuilder.Create<SendUserEmailsJob>().WithIdentity("SendUserEmailsJob").Build();
    //        var userEmailsTrigger = TriggerBuilder.Create().WithIdentity("SendUserEmailsCron").StartNow().WithCronSchedule("0 */30 * ? * *").Build();
    //        var result = await _scheduler.ScheduleJob(userEmailsJob, userEmailsTrigger);
    //    }
    //    public class SendUserEmailsJob : IJob
    //    {
    //        public Task Execute(IJobExecutionContext context)
    //        {
    //            _scheduler.PauseAll();
    //            Log.Logger.Error("başla ScrapingBusiness");
    //            var business = new ScrapingBusiness();
    //            business.Start();
    //            Log.Logger.Error("bitti ScrapingBusiness");
    //            _scheduler.ResumeAll();
    //            return Task.CompletedTask;
    //        }
    //    }
    //}
}
