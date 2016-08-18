﻿using Discord;
using Discord.Commands;
using NadekoBot.Attributes;
using NadekoBot.Extensions;
using System;
using System.Threading.Tasks;
using NadekoBot.Services;

namespace NadekoBot.Modules.Translator
{
    [Module("~", AppendSpace = false)]
    public class Translator : DiscordModule
    {
        public Translator(ILocalization loc, CommandService cmds, IBotConfiguration config, IDiscordClient client) : base(loc, cmds, config, client)
        {
        }

        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task Translate(IMessage imsg, string langs, [Remainder] string text = null)
        {
            var channel = imsg.Channel as ITextChannel;

            try
            {
                var langarr = langs.ToLowerInvariant().Split('>');
                if (langarr.Length != 2)
                    return;
                string from = langarr[0];
                string to = langarr[1];
                text = text?.Trim();
                if (string.IsNullOrWhiteSpace(text))
                    return;

                await imsg.Channel.TriggerTypingAsync().ConfigureAwait(false);
                string translation = await GoogleTranslator.Instance.Translate(text, from, to).ConfigureAwait(false);
                await imsg.Channel.SendMessageAsync(translation).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await imsg.Channel.SendMessageAsync("Bad input format, or something went wrong...").ConfigureAwait(false);
            }
        }

        [LocalizedCommand, LocalizedDescription, LocalizedSummary]
        [RequireContext(ContextType.Guild)]
        public async Task Translangs(IMessage imsg)
        {
            var channel = imsg.Channel as ITextChannel;

            await imsg.Channel.SendTableAsync(GoogleTranslator.Instance.Languages, str => str, columns: 4);
        }

    }
}