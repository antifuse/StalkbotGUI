using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;

namespace StalkbotGUI.Stalkbot.Utilities
{
    public class AlertCommand : BaseCommandModule
    {
        private volatile int _state = 0;
        /// <summary>
        /// Plays an alert if the command will be executed 
        /// </summary>
        /// <param name="ctx">Context this command has been executed in</param>
        /// <returns>The built task</returns>
        public override Task BeforeExecutionAsync(CommandContext ctx)
        {
            // if the command is disabled or there's no file, abort
            if (File.Exists($"{ctx.Command.Name.ToLower()}.wav"))
            {
                // play the audio
                using (var player = new SoundPlayer($"{ctx.Command.Name.ToLower()}.wav"))
                {
                    player.Play();
                }

            }

            new ToastContentBuilder()
                .AddAppLogoOverride(new Uri(ctx.User.AvatarUrl), ToastGenericAppLogoCrop.Circle)
                .AddText($"{ctx.Member.Nickname ?? ctx.Member.Username} triggered {ctx.Command.Name} command.")
                .AddText("Accept?")
                .AddButton(new ToastButton()
                .SetContent("Accept")
                .AddArgument("action", "accept")
                .SetBackgroundActivation()
                )
                .AddButton(new ToastButton()
                .SetContent("Deny")
                .AddArgument("action", "deny")
                .SetBackgroundActivation())
                .SetToastScenario(ToastScenario.Reminder)
                .AddArgument("message", ctx.Message.Id.ToString())
                .Show();


            _state = 0;
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);
                if (args.Get("message") != ctx.Message.Id.ToString()) return;
                if (!args.Contains("action") || args.Get("action") == "deny")
                {
                    ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("❌"));
                    
                    _state = 1;
                    
                }                   
              
                else if (args.Get("action") == "accept")
                {
                    _state = 2;
                }
                    
            };

            while (_state == 0) { }
            if (_state == 2) return base.BeforeExecutionAsync(ctx);

            return Task.Delay(Timeout.Infinite);
        }

    }
}
