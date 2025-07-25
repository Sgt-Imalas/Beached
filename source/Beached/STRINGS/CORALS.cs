﻿using Beached.Content.Defs.Entities.Corals;

namespace Beached
{
	public partial class STRINGS
	{
		public class CORALS
		{
			public class BEACHED_CORAL_WASHUSPONGE
			{
				public static LocString NAME = Link("Washu Sponge", WashuSpongeConfig.ID);
				public static LocString DESCRIPTION = "Woshu Sponge can sustain itself by consuming germs. This happens to be very useful for those wishing to keep their liquid reservoirs clean.\n" +
					"\n" +
					"Removes germs from liquids, and produces edible frags.";
			}
			public class BEACHED_LEAFLETCORAL
			{
				public static LocString NAME = Link("Leaflet Coral", LeafletCoralConfig.ID);
				public static LocString DESCRIPTION = "A coral with green capitulum resembling cabbage leaves. This coral is lined with a polymetallic outer epidermis, rich in Manganese and Iron, allowing natural slow electrolysis of Water. The plant consumes Hydrogen and releases excess Oxygen in the process.\n" +
					"\n" +
					"Inefficiently converts Water into Oxygen.";
				public static LocString DOMESTICATEDDESC = "...";
			}


			public class BEACHED_VISCOUS_CORAL
			{
				public static LocString NAME = Link("Viscous Coral", "Beached_ViscousCoral");
				public static LocString DESCRIPTION = "Viscous Corals really like to consume oil, very unlike most other sea life.\n" +
					"\n" +
					"{DescriptionEnd}";

				public static LocString DESCRIPTION_END_NO_BITUMEN = "Converts Crude Oil into Petroleum.";
				public static LocString DESCRIPTION_END_YES_BITUMEN = "Converts Crude Oil into Petroleum and Bitumen.";
			}

			public class FRAGS
			{
				public class BEACHED_LEAFLETCORAL
				{
					public static LocString NAME = Link("Leaflet Coral Frag", LeafletCoralConfig.ID);
					public static LocString DESCRIPTION = ($"The {Link("Frag", "CORALS")} of a {CORALS.BEACHED_LEAFLETCORAL.NAME}.");
				}
			}
		}
	}
}
