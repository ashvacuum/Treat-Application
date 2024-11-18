using UnityEngine;
using UnityEngine.SceneManagement;

namespace Firebase
{
    
    public class ServiceInitializer : MonoBehaviour
    {
        private async void Awake()
        {
            var firestoreService = new FirestoreService();
            await firestoreService.Initialize();
            ServiceLocator.Instance.RegisterService<IFirestoreService>(firestoreService);
            DontDestroyOnLoad(this);
            SceneManager.LoadSceneAsync(sceneBuildIndex: 1, LoadSceneMode.Single);
        }

        private void OnDestroy()
        {
            ServiceLocator.Instance.Reset();
        }
    }
}
