using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using OVRCards;

namespace OVRCService
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
	[ServiceContract]
	public interface IOVRCardService
	{
		[OperationContract]
		bool Connect();

		[OperationContract]
		void PostCard(OVRCard card);

		[OperationContract]
		void Disconnect();
	}

	// Use a data contract as illustrated in the sample below to add composite types to service operations.
	// You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "OVRCService.ContractType".
	[DataContract]
	public class OVRCard
	{
		string caption = "";
		string text = "";
		float r = 0f;
		float g = 0f;
		float b = 0f;
		float captionR = 1f;
		float captionG = 1f;
		float captionB = 1f;
		float textR = 1f;
		float textG = 1f;
		float textB = 1f;
		int duration = 5000;

		[DataMember]
		public string Caption
		{
			get => caption;
			set => caption = value;
		}

		[DataMember]
		public string Text
		{
			get => text;
			set => text = value;
		}

		[DataMember]
		public float R
		{
			get => r;
			set => r = value;
		}

		[DataMember]
		public float G
		{
			get => g;
			set => g = value;
		}

		[DataMember]
		public float B
		{
			get => b;
			set => b = value;
		}

		[DataMember]
		public float CaptionR
		{
			get => captionR;
			set => captionR = value;
		}

		[DataMember]
		public float CaptionG
		{
			get => captionG;
			set => captionG = value;
		}

		[DataMember]
		public float CaptionB
		{
			get => captionB;
			set => captionB = value;
		}

		[DataMember]
		public float TextR
		{
			get => textR;
			set => textR = value;
		}

		[DataMember]
		public float TextG
		{
			get => textG;
			set => textG = value;
		}

		[DataMember]
		public float TextB
		{
			get => textB;
			set => textB = value;
		}

		[DataMember]
		public int DurationMS
		{
			get => duration;
			set => duration = value;
		}
	}
}
