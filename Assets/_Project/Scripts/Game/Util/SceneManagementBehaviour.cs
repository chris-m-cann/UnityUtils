using UnityEngine;

namespace Util
{
    public class SceneManagementBehaviour : MonoBehaviour
    {
        public void LoadNextScene() => SceneManagerEx.LoadNextScene();
        public void LoadScene(string sceneName) => SceneManagerEx.LoadScene(sceneName);
        public void ReloadScene() => SceneManagerEx.ReloadScene();
    }
}