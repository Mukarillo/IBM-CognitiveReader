using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI.Extensions
{
	public static class InputFieldExtension
	{
		public static string GetSelection(this InputField aField)
		{
			if (aField == null)
				return "";
			int start = aField.selectionAnchorPosition;
			int end = aField.selectionFocusPosition;
			int len = end - start;
			if (len == 0)
				return "";
			if (len < 0)
			{
				start = end;
				len = -len;
			}
			return aField.text.Substring(start, len);
		}
	}
}