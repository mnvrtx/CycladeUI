using System.Collections;
using UnityEngine;

namespace CycladeUI.Utils
{
    public class SetActiveDelayed : MonoBehaviour
    {
        public void Begin(float delay, bool isActive)
        {
            if (isActive == gameObject.activeSelf)
                return;

            StopAllCoroutines();
            if (Mathf.Approximately(delay, 0))
                gameObject.SetActive(isActive);
            else
                StartCoroutine(SetActiveWithDelay(delay, isActive));
        }

        private IEnumerator SetActiveWithDelay(float delay, bool isActive)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(isActive);
        }
    }
}