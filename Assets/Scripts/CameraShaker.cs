using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour {

	public void ShakeCamera(float duration, float magnitude, Vector2 range)
    {
        StartCoroutine(Shake(duration, magnitude, range));
    }

    IEnumerator Shake(float duration, float magnitude, Vector2 range)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;
        while(elapsed < duration)
        {
            float x = Random.Range(-range.x, range.x) * magnitude;
            float y = Random.Range(-range.y, range.y) * magnitude;

            transform.localPosition = transform.localPosition + new Vector3(x, y, 0);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            elapsed += Time.unscaledDeltaTime;
        }
        transform.localPosition = originalPos;
    }
}
