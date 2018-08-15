using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DiscordRelay.Config;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DiscordRelay
{
    public class Relay
    {
		private	List<DiscordGuild> monitoredGuilds = new List<DiscordGuild>();
		private List<DiscordChannel> monitoredChannels = new List<DiscordChannel>();
		public readonly string configPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "ovrConfig.json";
		private DiscordClient client;
		private ConfigurationFile config;

		public delegate void GuildMessageHandler(string guild_name, string channel_name, string user_name, string message);
		public event GuildMessageHandler OnGuildMessage;
		public delegate void DirectMessageHandler(string user_name, string message);
		public event DirectMessageHandler OnDirectMessage;
		public delegate void DisconnectHandler();
		public event DisconnectHandler OnDisconnect;
		public delegate void ChannelAmountHandler(int newAmount);
		public event ChannelAmountHandler OnChannelAmountChanged;

		private Thread RunningThread = null;
		private bool DiscordShouldRun = true;

		public bool Running
		{
			get
			{
				return RunningThread != null && RunningThread.IsAlive;
			}
		}

		public void StartDiscord(string botToken, string ownerName, string ownerDiscriminator, string commandPrefix)
		{
			if(RunningThread != null)
			{
				return;
			}
			RunningThread = new Thread(() =>
			{
				DiscordShouldRun = true;
				Initialize(botToken, ownerName, ownerDiscriminator, commandPrefix).ConfigureAwait(false).GetAwaiter().GetResult();
			});
			RunningThread.Start();
		}

		public void StopDiscord()
		{
			DiscordShouldRun = false;
		}

		private async Task Initialize(string botToken, string ownerName, string ownerDiscriminator, string commandPrefix)
		{
			config = ConfigurationFile.ReadFromFile(configPath);
			if(config == null)
			{ 
				config = ConfigurationFile.Make(botToken, ownerName, ownerDiscriminator, commandPrefix);
				config.SaveToFile(configPath);
			}

			client = new DiscordClient(new DiscordConfiguration()
			{
				Token = config.BotToken,
				TokenType = TokenType.Bot
			});
			client.MessageCreated += async e =>
			{
				await NotifyMessage(e);
			};
			await client.ConnectAsync();
			foreach(SavedChannel c in config.SavedChannels)
			{
				monitoredChannels.Add(await c.Get(client));
			}
			OnChannelAmountChanged(MonitoredChannelsNumber);
			while (DiscordShouldRun)
			{
				await Task.Delay(3000);
			}
			await client.DisconnectAsync();
			RunningThread = null;
			OnDisconnect();
		}

		public void StopDiscordAndWait()
		{
			StopDiscord();
			if (Thread.CurrentThread == RunningThread)
				return;
			if (RunningThread != null && RunningThread.IsAlive)
				RunningThread.Join();
		}

		async Task NotifyMessage(MessageCreateEventArgs m)
		{
			if(m.Channel.Guild == null)
			{
				OnDirectMessage(m.Author.Username + "#" + m.Author.Discriminator, m.Message.Content);
			}
			if (monitoredChannels.Contains(m.Channel))
			{
				OnGuildMessage(m.Guild.Name, m.Channel.Name, m.Author.Username + "#" + m.Author.Discriminator, m.Message.Content);
			}
			if(config.OwnerID == 0 && m.Author.Username.Equals(config.OwnerUsername) && m.Author.Discriminator.Equals(config.OwnerDiscriminator))
			{
				config.OwnerID = m.Author.Id;
				config.SaveToFile(configPath);
			}
			if(m.Author.Id == config.OwnerID)
			{
				if (m.Message.Content.StartsWith(config.CommandPrefix))
				{
					if (m.Channel.Guild != null)
					{
						string command = m.Message.Content.Substring(config.CommandPrefix.Length);
						switch (command)
						{
							case "startMonitor":
								if (monitoredChannels.Contains(m.Channel))
								{
									await m.Message.RespondAsync("Channel is already monitored, dumbdumb...");
									break;
								}
								monitoredChannels.Add(m.Channel);
								config.SavedChannels.Add(new SavedChannel()
								{
									guild_id = m.Channel.GuildId,
									id = m.Channel.Id
								});
								config.SaveToFile(configPath);
								OnChannelAmountChanged(MonitoredChannelsNumber);
								await m.Message.RespondAsync("@here Notice: this channel is now monitored by this bot.");
								break;
							case "stopMonitor":
								if (!monitoredChannels.Contains(m.Channel))
								{
									await m.Message.RespondAsync("Channel is no currently monitored, genius...");
									break;
								}
								monitoredChannels.Remove(m.Channel);
								config.SavedChannels.Remove(new SavedChannel()
								{
									guild_id = m.Channel.GuildId,
									id = m.Channel.Id
								});
								config.SaveToFile(configPath);
								OnChannelAmountChanged(MonitoredChannelsNumber);
								await m.Message.RespondAsync("Notice: this channel is not monitored by this bot anymore.");
								break;
							default:
								await m.Message.RespondAsync("Invalid command.");
								break;
						}
					}
					else
					{
						await m.Message.RespondAsync("Commands are not available for DMs");
					}
				}
			}
		}

		public int MonitoredChannelsNumber
		{
			get
			{
				return monitoredChannels.Count;
			}
		}
    }
}
