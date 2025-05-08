using UnityEngine;
using System.Collections;

namespace LLMUnitySamples
{

    public class IntroCameraSequence : MonoBehaviour
    {
        public Transform startPosition;
        public Transform endPosition;
        public float duration = 12f;

        public AudioSource introMusicSource;
        public AudioClip introMusic;
        public float fadeDuration = 2f;

        public FunctionCalling functionCalling;

        private float timeElapsed = 0f;

        void Start()
        {
            StartCoroutine(SwirlToGameScreen());
        }

        IEnumerator SwirlToGameScreen()
        {
            transform.position = startPosition.position;
            transform.rotation = startPosition.rotation;

            if (introMusicSource != null && introMusic != null)
            {
                introMusicSource.clip = introMusic;
                introMusicSource.volume = 1f;
                introMusicSource.Play();
            }

            float fadeStartTime = duration - fadeDuration;

            while (timeElapsed < duration)
            {
                float t = timeElapsed / duration;

                transform.position = Vector3.Slerp(startPosition.position, endPosition.position, t);
                transform.rotation = Quaternion.Slerp(startPosition.rotation, endPosition.rotation, t);

                if (introMusicSource != null && timeElapsed >= fadeStartTime)
                {
                    float fadeT = (timeElapsed - fadeStartTime) / fadeDuration;
                    introMusicSource.volume = Mathf.Lerp(1f, 0f, fadeT);
                }

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = endPosition.position;
            transform.rotation = endPosition.rotation;

            if (introMusicSource != null)
            {
                introMusicSource.Stop();
                introMusicSource.volume = 1f;
            }

            if (functionCalling != null)
                functionCalling.StartGameShow();
        }
    }
}