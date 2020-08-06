using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HTMS.Modules
{
    [Serializable()]
    public class RazerInfo {
        public float Money { get; set; }
        public float Tool { get; set; }

        public RazerInfo()
        {}

        public RazerInfo(float m, float t)
        {
            Money = m;
            Tool = t;
        }
    }

    public class Razer : ModuleBase<SocketCommandContext>
    {
        Dictionary<string, RazerInfo> info;

        [Command("razer")]
        public async Task razer()
        {
            await Context.Channel.SendMessageAsync(getInfo(Context.User.Id).Money.ToString());
        }

        [Command("work")]
        public async Task work()
        {
            var user = getInfo(Context.User.Id);
            user.Money += user.Tool;
            await Context.Channel.SendMessageAsync("You now have " + getInfo(Context.User.Id).Money.ToString() + " razers");
        }
        [Command("shop")]
        public async Task shop()
        {
            await Context.Channel.SendMessageAsync("Bronze shovel 10000 razers \n Silver shovel 20000 razers \n Gold shovel  50000 razers \n For example, do !buy gold");
        }
        [Command("buy")]
        public async Task shop(string thing)
        {
            var user = getInfo(Context.User.Id); //spaghetti
            if (thing.ToLower() == "bronze" && user.Money >= 10000)
            {
                user.Money -= 10000;
                await (Context.Channel.SendMessageAsync("success"));
            }
            else if (thing.ToLower() == "silver" && info[Context.User.Id.ToString()].Money >= 20000)
            {
                user.Money -= 20000;
                await (Context.Channel.SendMessageAsync("success"));
            }
            else if (thing.ToLower() == "gold" && info[Context.User.Id.ToString()].Money >= 50000)
            {
                user.Money -= 50000;
                await (Context.Channel.SendMessageAsync("success"));
            }
            else
            {
                await (Context.Channel.SendMessageAsync("error, you don't have enough money to buy that"));
            }
        }
        [Command("give")]
        public async Task give(SocketGuildUser user, float amount)
        {
            var commandUser = Context.User as SocketGuildUser;
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Admin");
            if (commandUser.Roles.Contains(role))
            {
                getInfo(user.Id).Money += amount;
            }
            else
            {
                await Context.Channel.SendMessageAsync("You need an admin role to do this");
            }

            await Context.Channel.SendMessageAsync("You now have " + getInfo(user.Id).Money.ToString() + " razers");
        }
        [Command("payout")]
        public async Task payout(SocketGuildUser user, float amount)
        {
            if (amount <= 5 || amount > 200000000)
            {
                await Context.Channel.SendMessageAsync("Error: invalid amount");
                return;
            }


            var commandUser = Context.User as SocketGuildUser;
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Admin");
            if (commandUser.Roles.Contains(role))
            {
                getInfo(user.Id).Money -= amount;
                await Context.Channel.SendMessageAsync("You now have " + getInfo(user.Id).Money.ToString() + " razers");
            }
            else
            {
                await Context.Channel.SendMessageAsync("You need an admin role to do this");
            }
        }

        public RazerInfo getInfo(ulong id)
        {
            if (info == null)
            {
                try
                {
                    info = JsonSerializer.Deserialize<Dictionary<string, RazerInfo>>(File.ReadAllText("userinfo"));
                }
                catch
                {
                    info = new Dictionary<string, RazerInfo>();
                }
            }
            else
            {
                var jsonString = JsonSerializer.Serialize(info);
                File.WriteAllText("userinfo", jsonString);
            }

            if (!info.ContainsKey(id.ToString()))
            {
                info.Add(id.ToString(), new RazerInfo(0, 5));
            }

            return info[id.ToString()];
        }
    }
}
