using Discord;
using Discord.Commands;
using JangleeBot.Helpers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace JangleeBot.Modules
{
    public class CommandLine : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<CommandLine> _logger;

        public CommandLine(ILogger<CommandLine> logger)
            => _logger = logger;

        [Command("Help")]
        public async Task Help()
        {
            await ReplyAsync("Blank");
        }
        [Command("Hello")]
        public async Task Hello()
        {
            await ReplyAsync("Xin chào " + Context.Message.Author.Mention +
                "\nTôi là Janglee chưa tròn 1 tuổi, tôi được Võ Tấn Tài tạo ra để phục vụ các bạn.");
        }
        [Command("Time")]
        public async Task GetTime()
        {
            var datetime = DateTime.Now;
            await ReplyAsync("Bây giờ là " + datetime.Hour + " giờ " + datetime.Minute + " phút.");
        }
        [Command("Random")]
        public async Task Random(int min, int max)
        {
            Random random = new Random();
            var result = random.Next(min, max + 1);
            await ReplyAsync("Chào " + Context.Message.Author.Mention +
                "\nYour random number is " + result);
        }
        [Command("Girl")]
        public async Task RandomGirl()
        {
            var rand = new Random();
            var files = Directory.GetFiles("C:\\Users\\Administrator\\Desktop\\Janglee Bot\\Janglee Bot\\Images\\", "*.jpg");
            var img = files[rand.Next(files.Length)];
            await Context.Channel.SendFileAsync(img, "Của bạn đây " + Context.Message.Author.Username);
        }
        [Command("Taixiu")]
        public async Task Taixiu([Remainder] string value)
        {
            if (value.ToLower().Equals("tai") || value.ToLower().Equals("xiu"))
            {
                Random random = new Random();
                var fdice = random.Next(1, 7);
                var sdice = random.Next(1, 7);
                var result = fdice + sdice;
                var sresult = "Xỉu";
                var final = "Thua";
                var color = Color.Red;
                if (result % 2 == 0)
                {
                    sresult = "Tài";
                    if (value.ToLower().Equals("tai"))
                    {
                        final = "Chiến thắng";
                        color = Color.Green;
                    }
                }
                else
                {
                    if (value.ToLower().Equals("xiu"))
                    {
                        final = "Chiến thắng";
                        color = Color.Green;
                    }
                }
                var builder = new EmbedBuilder()
                    .WithThumbnailUrl(Context.Message.Author.GetAvatarUrl())
                    .WithDescription("Chào mừng bạn đến vận mệnh vẫy gọi")
                    .WithColor(color)
                    .AddField("Xúc sắc một", fdice + " điểm", true)
                    .AddField("Xúc sắc hai", sdice + " điểm", true)
                    .AddField("Tổng cộng", result + " điểm")
                    .AddField("Kết quả", sresult, true)
                    .AddField(Context.Message.Author.Username, final)
                    .WithCurrentTimestamp();
                var emb = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, emb);
                return;
            }
            await ReplyAsync("Chào " + Context.Message.Author.Mention +
                "\nJust type 'tai' or 'xiu'");
        }
        [Command("Date")]
        public async Task GetDate()
        {
            var datetime = DateTime.Now;
            var dow = DateTime.Now.DayOfWeek;
            var sdow = dow.ToString();
            if (sdow.ToString().Equals("Monday"))
            {
                sdow = "Thứ 2";
            }
            if (sdow.ToString().Equals("Tuesday"))
            {
                sdow = "Thứ 3";
            }
            if (sdow.ToString().Equals("Wednesday"))
            {
                sdow = "Thứ 4";
            }
            if (sdow.ToString().Equals("Thursday"))
            {
                sdow = "Thứ 5";
            }
            if (sdow.ToString().Equals("Friday"))
            {
                sdow = "Thứ 6";
            }
            if (sdow.ToString().Equals("Saturday"))
            {
                sdow = "Thứ 7";
            }
            if (sdow.ToString().Equals("Sunday"))
            {
                sdow = "Chủ Nhật";
            }
            await ReplyAsync("Chào " + Context.Message.Author.Mention +
                "\nHôm nay là " + sdow + " ngày " + datetime.Day + " tháng " + datetime.Month + " năm " + datetime.Year + ".");
        }
        [Command("Covid")]
        public async Task CovidInformation()
        {
            var url = "https://api.apify.com/v2/key-value-stores/EaCBL1JNntjR3EakU/records/LATEST?disableRedirect=true";
            ClientHelper.InitializeClient();
            var http = ClientHelper.Client;
            var data = await http.GetStringAsync(url);
            var json = JObject.Parse(data);
            var datetime = DateTime.Now;
            var builder = new EmbedBuilder()
                .WithColor(new Color(50, 205, 50))
                .WithThumbnailUrl("https://i.ibb.co/4gzv4Fb/covid.png")
                .WithDescription("Tình hình dịch bệnh ngày " + datetime.ToString("dd/MM/yyyy"))
                .AddField("Số ca nhiễm", json["infected"], true)
                .AddField("Đang điều trị", json["treated"], true)
                .AddField("Đã hồi phục", json["recovered"])
                .AddField("Tử vong", json["deceased"], true)
                .WithCurrentTimestamp();
            ;
            var emb = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, emb);

        }
    }
}