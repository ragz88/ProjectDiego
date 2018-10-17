using UnityEngine;
using System.Collections;

namespace TPCEngine
{
	public struct ScanSettings
	{
		public ScanFlags scanFlags;
		public int? joystick;
		public float timeout;
		public string cancelScanButton;
		public object userData;
	}
}