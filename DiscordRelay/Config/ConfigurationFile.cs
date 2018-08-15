using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace DiscordRelay.Config
{
	[DataContract]
	public class ConfigurationFile
	{
		public static ConfigurationFile Make(string botToken, string ownerName, string ownerDiscriminator, string commandPrefix)
		{
			return new ConfigurationFile()
			{
				BotToken = botToken,
				OwnerUsername = ownerName,
				OwnerDiscriminator = ownerDiscriminator,
				CommandPrefix = commandPrefix,
				SavedChannels = new List<SavedChannel>() 
			};
		}

		public static ConfigurationFile ReadFromFile(string path)
		{
			try
			{
				using (FileStream fStream = new FileStream(path, FileMode.Open, FileAccess.Read))
				{
					return (ConfigurationFile)new DataContractJsonSerializer(typeof(ConfigurationFile)).ReadObject(fStream);
				}
			}catch(Exception ex)
			{
				Console.WriteLine("Exception: " + ex.Message + " StackTrace: ");
				Console.WriteLine(ex.StackTrace);
				return null;
			}
		}

		public void SaveToFile(string path)
		{
			try
			{
				using(FileStream fstream = new FileStream(path, FileMode.Create, FileAccess.Write))
				{
					new DataContractJsonSerializer(typeof(ConfigurationFile)).WriteObject(fstream, this);
				}
			}catch(Exception ex)
			{
				Console.WriteLine("Exception: " + ex.Message + " StackTrace: ");
				Console.WriteLine(ex.StackTrace);
			}
		}

		[DataMember(IsRequired = true)]
		public List<SavedChannel> SavedChannels;

		[DataMember(IsRequired = false)]
		public ulong OwnerID;

		[DataMember(IsRequired = true)]
		public string OwnerUsername;

		[DataMember(IsRequired = true)]
		public string OwnerDiscriminator;

		[DataMember(IsRequired = true)]
		public string BotToken;

		[DataMember(IsRequired = true)]
		public string CommandPrefix;

	}

	[DataContract]
	public class SavedChannel
	{
		[DataMember(IsRequired = true)]
		public ulong id;
		[DataMember(IsRequired = true)]
		public ulong guild_id;

		public async Task<DiscordChannel> Get(DiscordClient discord)
		{
			var guild = await discord.GetGuildAsync(guild_id);
			var channel = guild.GetChannel(id);
			return channel;
		}

		public override bool Equals(object other)
		{
			if (!(other is SavedChannel))
				return false;
			SavedChannel o = (SavedChannel)other;
			return id == o.id && guild_id == o.guild_id;
		}

		public override int GetHashCode()
		{
			return (int) (id / guild_id);
		}
	}
}
