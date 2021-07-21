using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;

namespace JangleeBot.Modules
{
    public class MusicModule : ModuleBase
    {
        private readonly LavaNode _lavaNode;
        public MusicModule(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }

        [Command("Ping")]
        public async Task Ping()
        {
            await ReplyAsync("Pong!!!");
        }

        [Command("Join", RunMode = RunMode.Async)]
        public async Task JoinAsync()
        {
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm already connected to a voice channel!");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync("Xin chào " + Context.Message.Author.Mention);
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Command("?", RunMode = RunMode.Async)]
        public async Task Answer([Remainder] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                await ReplyAsync("Please insert your question!");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel!");
                return;
            }

            var searchResponse = await _lavaNode.SearchYouTubeAsync(query);
            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                await ReplyAsync($"I don't know! Sorry!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            var track = searchResponse.Tracks[0];

            await player.PlayAsync(track);

            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Message.Author.GetAvatarUrl())
                .WithThumbnailUrl(Context.Message.Author.GetAvatarUrl())
                .WithColor(Color.Gold)
                .WithTitle("Janglee Assistant Media")
                .WithDescription("Erudite");
            var emb = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, emb);
        }

        [Command("Play", RunMode = RunMode.Async)]
        public async Task PlayAsync([Remainder] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                await ReplyAsync("Please provide search terms.");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            var searchResponse = await _lavaNode.SearchYouTubeAsync(query);
            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                await ReplyAsync($"I wasn't able to find anything for `{query}`.");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            var track = searchResponse.Tracks[0];
            var status = "Đang phát";
            var description = "Playing";
            var color = Color.Green;

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                player.Queue.Enqueue(track);
                status = "Đã thêm vào danh sách phát";
                color = Color.Orange;
                description = "Added to queue";
            }
            else
            {
                await player.PlayAsync(track);
            }
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Message.Author.GetAvatarUrl())
                .WithColor(color)
                .WithTitle("Janglee Assistant Media")
                .WithDescription(description)
                .AddField(status, track.Title);
            var emb = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, emb);
        }

        [Command("Skip", RunMode = RunMode.Async)]
        public async Task Skip()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel!");
                return;
            }
            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You need to be in the same chanel as me!");
                return;
            }
            if (player.Queue.Count == 0)
            {
                await ReplyAsync("There are no more song in the queue!");
                return;
            }
            await player.SkipAsync();
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Message.Author.GetAvatarUrl())
                .WithColor(Color.Red)
                .WithTitle("Janglee Assistant Media")
                .WithDescription("Skipped")
                .AddField("Đang phát", player.Track.Title);
            var emb = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, emb);
        }

        [Command("Stop", RunMode = RunMode.Async)]
        public async Task Stop()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel!");
                return;
            }
            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You need to be in the same chanel as me!");
                return;
            }
            await player.StopAsync();
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Message.Author.GetAvatarUrl())
                .WithColor(Color.Red)
                .WithTitle("Janglee Assistant Media")
                .WithDescription("Stopped");
            var emb = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, emb);
        }

        [Command("Pause", RunMode = RunMode.Async)]
        public async Task PauseSkip()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel!");
                return;
            }
            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You need to be in the same chanel as me!");
                return;
            }
            if (player.PlayerState == PlayerState.Paused || player.PlayerState == PlayerState.Stopped)
            {
                await ReplyAsync("There music is already paused!");
                return;
            }
            await player.PauseAsync();
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Message.Author.GetAvatarUrl())
                .WithColor(Color.Purple)
                .WithTitle("Janglee Assistant Media")
                .WithDescription("Paused")
                .AddField("Đã tạm dừng", player.Track.Title);
            var emb = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, emb);
        }

        [Command("Resume", RunMode = RunMode.Async)]
        public async Task Resume()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel!");
                return;
            }
            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You need to be in the same chanel as me!");
                return;
            }
            if (player.PlayerState == PlayerState.Playing)
            {
                await ReplyAsync("There music is already playing!");
                return;
            }
            await player.ResumeAsync();
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Message.Author.GetAvatarUrl())
                .WithColor(Color.Blue)
                .WithTitle("Janglee Assistant Media")
                .WithDescription("Resumed")
                .AddField("Tiếp tục phát", player.Track.Title);
            var emb = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, emb);
        }

        [Command("Queue", RunMode = RunMode.Async)]
        public async Task Queue()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }
            var player = _lavaNode.GetPlayer(Context.Guild);
            var queue = player.Queue.ToList();

            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Message.Author.GetAvatarUrl())
                .WithTitle("Janglee Assistant Media")
                .WithDescription("Queue")
                .WithColor(Color.Blue);
            if (player.Track == null)
            {
                builder.WithDescription("The queue is empty");
            }
            else
            {
                builder.AddField("Đang phát", player.Track.Title);
                foreach (var item in queue)
                {
                    builder.AddField("Đang chờ ", item.Title);
                }
            }
            var emb = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, emb);
        }

        [Command("Loop", RunMode = RunMode.Async)]
        public async Task Loop()
        {
            var voiceState = Context.User as IVoiceState;
            var player = _lavaNode.GetPlayer(Context.Guild);
            var queue = player.Queue;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }
            if (queue.Count == 0)
            {
                await ReplyAsync("You must add songs before looping the queue!");
                return;
            }
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Message.Author.GetAvatarUrl())
                .WithColor(Color.Gold)
                .WithTitle("Janglee Assistant Media")
                .WithDescription("Hiện tại chỉ có thể lặp lại bài hát bằng tay vì tôi chưa code được chức năng này");
            var emb = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, emb);
        }

        [Command("Disconnect", RunMode = RunMode.Async)]
        public async Task Disconnect()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }
            await ReplyAsync("Tạm biệt " + Context.Message.Author.Mention);
            await _lavaNode.LeaveAsync(voiceState.VoiceChannel);
        }
    }
}
