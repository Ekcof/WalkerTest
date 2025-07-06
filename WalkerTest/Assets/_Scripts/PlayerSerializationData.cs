using System;
using System.Collections.Generic;

namespace Serialization
{
	[Serializable]
	public class PlayerSerializationData
	{
		public int Experience;
		public float Health;
		public List<SerializedItem> Items;
	}

	[Serializable]
	public class SerializedItem
	{
		public string Id;
		public int Amount;
	}
}