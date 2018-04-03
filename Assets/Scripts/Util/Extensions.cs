using System;

namespace Assets.Scripts.Util
{
	public static class Extensions
	{
		private static void Nop<T>(this T o) { }

		public static bool Exists(this object o)
		{
			if (o == null)
				return false;
			bool exists = false;
			var obj = o as UnityEngine.Object;
			try
			{
				obj.hideFlags.Nop();
				exists = true;
			}
			catch (Exception)
			{ }
			return exists;
		}
	}
}