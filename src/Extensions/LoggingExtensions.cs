﻿using System;
using System.Threading.Tasks;
using AlbaVulpes.API.Models.Config;
using AlbaVulpes.API.Services;
using AlbaVulpes.API.Services.AWS;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Seq;
using NLog.Targets.Wrappers;

namespace AlbaVulpes.API.Extensions
{
    public static class LoggingExtensions
    {
        private static void UpdateLoggingConfiguration(Action<LoggingConfiguration> updateAction)
        {
            var currentConfig = NLog.LogManager.Configuration ?? new LoggingConfiguration();

            updateAction(currentConfig);

            NLog.LogManager.Configuration = currentConfig;
        }

        public static void UseConsoleLogging(this IApplicationBuilder builder)
        {
            var consoleTarget = new ConsoleTarget();

            UpdateLoggingConfiguration(config =>
            {
                config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, consoleTarget);
            });

            NLog.LogManager.GetCurrentClassLogger().Info("Console logging initialized.");
        }

        public static void UseFileLogging(this IApplicationBuilder builder)
        {
            var fileTarget = new FileTarget
            {
                FileName = $"logs/{DateTime.UtcNow.ToString("yyyy-MM-dd")}.log"
            };

            UpdateLoggingConfiguration(config =>
            {
                config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, fileTarget);
            });

            NLog.LogManager.GetCurrentClassLogger().Info("File logging initialized.");
        }

        public static void UseSeqLogging(this IApplicationBuilder builder)
        {
            var appServices = builder.ApplicationServices;

            var appSettings = appServices.GetService<IOptions<AppSettings>>().Value;
            var secretsManagerService = appServices.CreateScope().ServiceProvider.GetService<ISecretsManagerService>();

            var appSecrets = Task.Run(async () => await secretsManagerService.Get()).Result;

            var seqTarget = new SeqTarget
            {
                ServerUrl = appSettings.Seq.ServerUrl,
                ApiKey = appSecrets.Seq_ApiKey
            };

            // Ensures that writes to Seq do not block the application
            var bufferingWrapper = new BufferingTargetWrapper
            {
                Name = "logseq",
                BufferSize = 1000,
                FlushTimeout = 2000,
                WrappedTarget = seqTarget
            };

            UpdateLoggingConfiguration(config =>
            {
                config.AddRule(NLog.LogLevel.Warn, NLog.LogLevel.Fatal, bufferingWrapper);
            });

            NLog.LogManager.GetCurrentClassLogger().Info("Seq logging initialized.");
        }
    }
}