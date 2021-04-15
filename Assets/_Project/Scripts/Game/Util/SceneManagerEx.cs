using UnityEngine.SceneManagement;

namespace Util
{
    public static class SceneManagerEx 
    {
        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        
        public static void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        public static void LoadNextScene()
        {
            var current = SceneManager.GetActiveScene().buildIndex;
            var next = current + 1;

            if (next >= SceneManager.sceneCount)
            {
                next = 0;
            }

            SceneManager.LoadScene(next);
        }
    }
}