using System;

namespace ECS {

	class Utils {

		public static int CeilPower2(int x) {
			if (x < 2) {
				return 1;
			}
			return (int) Math.Pow(2, (int) Math.Log(x-1, 2) + 1);
		}

	}

}
