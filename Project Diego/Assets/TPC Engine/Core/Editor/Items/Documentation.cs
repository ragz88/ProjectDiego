/* ================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================ */

using UnityEngine;

namespace TPCEngine.Editor
{
	public static class Documentation
	{
		public static string LocalURL = System.Uri.EscapeUriString("Assets/TPC Engine/Documentation/Third Person Character Engine Manual.pdf");
		public static string EthernetURL = "https://docs.google.com/document/d/13mKD_2SScX3Ru5eHXXPGxxe-f9cwWe7ertDx0HboWVE/edit?usp=sharing";

		public static void Open()
		{
			string URL = (Application.internetReachability != NetworkReachability.NotReachable) ? EthernetURL : LocalURL;
			Application.OpenURL(URL);
		}
	}
}