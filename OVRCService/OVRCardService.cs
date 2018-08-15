using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using OVRCards;
using OVRCards.Cards;

namespace OVRCService
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class OVRCardService : IOVRCardService
	{
		protected static NotificationManager manager = null;
		protected static int numberConnections = 0;

		public bool Connect()
		{
			numberConnections++;
			if (manager == null)
			{
				manager = NotificationManager.Create();
				manager.StartRender();
			}
			numberConnections++;
			return true;
		}

		public void Disconnect()
		{
			numberConnections--;
			if(numberConnections <= 0)
			{
				manager.StopRenderWait();
				manager = null;
			}
		}

		public void PostCard(OVRCard card)
		{
			Card c = new Card(card.Caption, card.Text, card.R, card.G, card.B, card.DurationMS, card.CaptionR, card.CaptionG, card.CaptionB, card.TextR, card.TextG, card.TextB);
			manager.CardQueue.Enqueue(c);
		}
	}
}
