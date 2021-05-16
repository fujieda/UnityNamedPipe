using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class IPC
    {
        private static NamedPipeServerStream _pipe;
        private static bool _running;
        private static Func<string, UniTask> _handler;
        public static string PipeName { private get; set; } = "UnityNamedPipe";

        public static void Run(Func<string, UniTask> handler)
        {
            _handler = handler;
            if (!_running)
                Loop().Forget();
        }

        private static async UniTaskVoid Loop()
        {
            while (true)
            {
                var message = await Receive();
                if (message == "")
                    break;
                await _handler(message);
                await UniTask.WaitForEndOfFrame();
            }
            _running = false;
        }

        public static async UniTask Send(string json)
        {
            var bytes = Encoding.UTF8.GetBytes(json);
            if (_pipe is null || !_pipe.CanWrite)
                return;
            try
            {
                await UniTask.Run(() => _pipe.Write(bytes, 0, bytes.Length));
            }
            finally
            {
                _pipe.Dispose();
            }
        }

        private static async UniTask<string> Receive()
        {
            try
            {
                _pipe = new NamedPipeServerStream(PipeName);
                return await UniTask.Run(() =>
                {
                    var buffer = new byte[1024];
                    _pipe.WaitForConnection();
                    var len = _pipe.Read(buffer, 0, buffer.Length);
                    return Encoding.UTF8.GetString(buffer, 0, len);
                });
            }
            catch (Exception e)
            {
                _pipe?.Dispose();
                Debug.Log(e.Message);
                return "";
            }
        }

        public static void Close()
        {
            try
            {
                var pipe = new NamedPipeClientStream(PipeName);
                pipe.Connect(500);
                pipe.Flush();
                pipe.Close();
            }
            catch (FileNotFoundException)
            {
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            finally
            {
                _pipe?.Close();
            }
        }
    }
}