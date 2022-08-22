using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CrossSceneTransitioner : MonoBehaviour {
    public static CrossSceneTransitioner instance;

    public AnimationCurve fadeOut = new AnimationCurve();
    public AnimationCurve fadeIn = new AnimationCurve();
    [Space]
    public Image panel;
    [Space]
    public float autoDuration = 0.3f;
    public bool autoAnimateOnLoad = true;

    private void OnEnable() {
        instance = this;
        panel.color = new Color(0, 0, 0, 0);
        if (autoAnimateOnLoad) onSceneLoad(autoDuration);
    }



    public void onSceneLoad(float animationDuration) {
        StartCoroutine(fadeInRoutine(animationDuration));
    }
    public void onSceneLoad() {
        StartCoroutine(fadeInRoutine(autoDuration));
    }

    public void loadScene(float animationDuration, string newScene, bool usePhoton = false, bool additiveLoad = false) {
        StartCoroutine(fadeOutRoutine(animationDuration, newScene, usePhoton, additiveLoad));
    }
    public void loadScene(string newScene, bool usePhoton = false, bool additiveLoad = false) {
        StartCoroutine(fadeOutRoutine(autoDuration, newScene, usePhoton, additiveLoad));
    }


    IEnumerator fadeInRoutine(float duration) {
        float timer = 0;
        while (timer < duration) {
            timer += Time.deltaTime;

            panel.color = new Color(0, 0, 0, fadeIn.Evaluate(timer / duration));

            yield return null;
        }

        panel.color = new Color(0, 0, 0, 0);
    }

    IEnumerator fadeOutRoutine(float duration, string scene, bool usePhoton, bool additive) {
        float timer = 0;
        while (timer < duration) {
            timer += Time.deltaTime;

            panel.color = new Color(0, 0, 0, fadeOut.Evaluate(timer / duration));

            yield return null;
        }
        if (!usePhoton) {
            if (additive) SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            else SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        } else {
            PhotonNetwork.LoadLevel(scene);
        }
    }

}