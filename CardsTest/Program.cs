using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OVRCards;
using OVRCards.Cards;

namespace CardsTest
{
	class Program
	{
		static NotificationManager manager;

		static void Main(string[] args)
		{
			AsyncMain().ConfigureAwait(false).GetAwaiter().GetResult();
		}

		static async Task AsyncMain()
		{
			manager = NotificationManager.Create();
			manager.StartRender();
			manager.CardQueue.Enqueue(new Card("Test", "Test test test test test Test Test Test Test Test Test Test Test Test Test Test Test Test ", 1, 0, 0, 2500, 0, 1, 0, 1, 1, 1));
			manager.CardQueue.Enqueue(new Card("Prova", "Prova", 0, 0, 1, 2500, 1, 0, 0, 1, 1, 1));
			while (Console.ReadLine() != "quit")
			{
				await Task.Delay(100);
			}
			manager.StopRender();
		}
	}
}
