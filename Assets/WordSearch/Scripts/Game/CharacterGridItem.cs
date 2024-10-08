using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace BBG.WordSearch
{
	public class CharacterGridItem : MonoBehaviour
	{
		#region Inspector Variables

		public TextMeshProUGUI	characterText;
		public bool isVisible = false;

		#endregion

		#region Properties

		public int	Row				{ get; set; }
		public int	Col				{ get; set; }
		public bool	IsHighlighted	{ get; set; }

		#endregion
	}
}
