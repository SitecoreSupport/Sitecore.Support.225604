namespace Sitecore.Modules.EmailCampaign.Core.Pipelines.DispatchNewsletter
{
    using Sitecore.Diagnostics;
    using Sitecore.EmailCampaign.Model;
    using Sitecore.EmailCampaign.Model.Dispatch;
    using Sitecore.ExM.Framework.Diagnostics;
    using Sitecore.Modules.EmailCampaign.Core;
    using System;

    public class NotifyDispatchFinished
    {
        private readonly CoreFactory coreFactory;
        private readonly ILogger logger;

        public NotifyDispatchFinished() : this(EcmFactory.GetDefaultFactory().Bl.CoreFactory, EcmFactory.GetDefaultFactory().Io.Logger)
        {
        }

        public NotifyDispatchFinished(CoreFactory coreFactory, ILogger logger)
        {
            Assert.ArgumentNotNull(coreFactory, "coreFactory");
            Assert.ArgumentNotNull(logger, "logger");
            this.coreFactory = coreFactory;
            this.logger = logger;
        }

        public void Process(DispatchNewsletterArgs args)
        {
            if ((!args.IsTestSend && args.AllowNotifications) && (args.Message.EnableNotifications && (args.DispatchInterruptRequest != DispatchInterruptSignal.Pause)))
            {
                try
                {
                    string str = args.SendingAborted ? EcmTexts.Localize("Aborted", new object[0]) : EcmTexts.Localize("Completed", new object[0]);
                    this.coreFactory.GetNotification(args.Message).SendDispatchFinished(str.ToLower());
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception);
                }
            }
        }
    }
}
