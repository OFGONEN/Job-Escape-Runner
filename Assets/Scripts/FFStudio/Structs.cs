/* Created by and for usage of FF Studios (2021). */

using System;
using UnityEngine;

namespace FFStudio
{
	public struct TransformInfo
	{
		public Vector3 position;
		public Vector3 rotation;
		public Vector3 scale;

		public TransformInfo( Transform transform )
		{
			position = transform.localPosition;
			rotation = transform.localEulerAngles;
			scale = transform.localScale;
		}

		public TransformInfo( Rigidbody rigidbody )
		{
			position = rigidbody.transform.localPosition;
			rotation = rigidbody.transform.localEulerAngles;
			scale = rigidbody.transform.localScale;
		}
	}

	[Serializable]
	public struct UIEntityInfo
	{
		public Sprite entityFlag;
		public string entityName;
	}
}
