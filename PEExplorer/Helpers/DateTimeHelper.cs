using System;


namespace PEExplorer.Helpers {
	public static class DateTimeHelper {
		public static DateTime FromSeconds(uint seconds) => new DateTime(1970, 1, 1) + TimeSpan.FromSeconds((double)seconds);
	}
}
