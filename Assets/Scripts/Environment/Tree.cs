namespace UnityEngine.Internal
{
	public class Tree : MonoBehaviour
	{
		[SerializeField] private GameObject[] apples = null;

		public void DropApples()
		{
			foreach (var apple in apples) apple.SetActive(true);
		}
	}
}