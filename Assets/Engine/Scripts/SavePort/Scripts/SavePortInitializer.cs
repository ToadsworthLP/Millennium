using SavePort.Saving;
using UnityEngine;

namespace SavePort {

    //Load this script in the first scene of your game.
    //After that, you will be able to use the SavePort save system.
    //While this isn't always neccessary, this makes the system way more reliable
    public class SavePortInitializer : MonoBehaviour {

        [SerializeField]
        private SaveConfiguration saveConfiguration;

        private void Start() {
            saveConfiguration.Initialize();
            Destroy(gameObject);
        }

    }

}
