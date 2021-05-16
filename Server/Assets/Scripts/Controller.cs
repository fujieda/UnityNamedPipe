using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Controller : MonoBehaviour
    {
        private GameObject _target;

        private void Awake()
        {
            if (FindObjectsOfType<Controller>().Length > 1)
                Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _target = GameObject.Find("/Target");
            IPC.Run(Handler);
        }

        private async UniTask Handler(string message)
        {
            var tokens = message.Split(' ');
            if (tokens.Length != 2)
            {
                await IPC.Send("Invalid message: " + message);
                return;
            }
            switch (tokens[0])
            {
                case "Scene":
                    SceneManager.LoadScene(tokens[1]);
                    await IPC.Send("Succeeded");
                    _target = GameObject.Find("/Target");
                    return;
                case "Color":
                    if (_target == null)
                    {
                        await IPC.Send("Target not exists");
                        return;
                    }
                    if (ColorUtility.TryParseHtmlString(tokens[1], out var color))
                        _target.GetComponent<Renderer>().material.color = color;
                    await IPC.Send("Succeeded");
                    return;
            }
            await IPC.Send("Invalid command: " + tokens[0]);
        }

        private void OnDisable()
        {
            IPC.Close();
        }
    }
}