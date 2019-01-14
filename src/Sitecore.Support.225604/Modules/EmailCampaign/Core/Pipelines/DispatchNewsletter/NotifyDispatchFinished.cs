using System;
using System.Reflection;
using Sitecore.Diagnostics;
using Sitecore.ExM.Framework.Diagnostics;
using Sitecore.Modules.EmailCampaign.Core;
using Sitecore.Modules.EmailCampaign.Core.Dispatch;
using Sitecore.Modules.EmailCampaign.Core.Pipelines.DispatchNewsletter;
using Sitecore.Modules.EmailCampaign.Factories;

namespace Sitecore.Support.Modules.EmailCampaign.Core.Pipelines.DispatchNewsletter
{
  public class NotifyDispatchFinished
  {
    private readonly CoreFactory coreFactory;
    private readonly ILogger logger;

    public NotifyDispatchFinished() : this(typeof(BusinessLogicFactory).GetProperty("CoreFactory", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(EcmFactory.GetDefaultFactory().Bl) as CoreFactory, EcmFactory.GetDefaultFactory().Io.Logger)
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
      if ((!args.IsTestSend && args.AllowNotifications) && args.Message.EnableNotifications)
      {
        try
        {
          string str;
      
          if (args.SendingAborted || !args.RequireFinalMovement)
          {
            str = EcmTexts.Localize("Aborted", new object[0]);
          }
          else if (args.DispatchInterruptRequest == DispatchInterruptSignal.Abort)
          {
            str = EcmTexts.Localize("Aborted", new object[0]);
          }
          else if (args.DispatchInterruptRequest == DispatchInterruptSignal.Pause)
          {
            str = EcmTexts.Localize("Paused", new object[0]);
          }
          else
          {
            str = EcmTexts.Localize("Completed", new object[0]);
          }

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
