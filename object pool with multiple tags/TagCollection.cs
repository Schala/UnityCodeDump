using System.Collections.Generic;
using UnityEngine;

public class TagCollection : MonoBehaviour
{
    public List<string> tags = null;

	/*private void Awake()
	{
		if (CompareTag("Untagged"))
		{
			tags = new List<string>(0);
			return;
		}

		if (tags == null) tags = new List<string>(1) { tag };
	}*/
}
