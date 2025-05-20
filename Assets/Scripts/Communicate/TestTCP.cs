using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PinballBenki.Communicate
{
    public sealed class TestTCP : IDisposable
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        public event Action<string> OnMessageReceived;
        public bool IsConnect { get; private set; }


        public TestTCP()
        {
            try
            {
                _client = new TcpClient("127.0.0.1", 4645);
                _stream = _client.GetStream();
                IsConnect = true;
            }
            catch (SocketException ex)
            {
                Debug.LogError($"接続に失敗しました: {ex.Message}");
                IsConnect = false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"予期しないエラーで接続に失敗しました: {ex.Message}");
                IsConnect = false;
            }
        }

        public async UniTask SendAsync(string message, CancellationToken ct)
        {
            if (!_stream.CanWrite)
            {
                return;
            }
            byte[] data = Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(data, 0, data.Length, ct);
        }

        public async UniTask StartReceiveLoop(CancellationToken ct)
        {
            byte[] buffer = new byte[1024];

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    if (!_stream.CanRead) break;

                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, ct);
                    if (bytesRead == 0)
                    {
                        break; //接続終了
                    }

                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (response == "Server is shutting down")
                    {
                        Debug.LogWarning("サーバーから終了通知を受け取りました。通信を切断します。");
                        Dispose(); //閉じる
                        return;
                    }

                    OnMessageReceived?.Invoke(response);
                }
            }
            catch (OperationCanceledException)
            {
                //むし
            }
            catch (IOException ex)
                when (ex.InnerException is SocketException socketEx
                && socketEx.SocketErrorCode == SocketError.OperationAborted)
            {
                Debug.LogWarning("接続が中断されました（OperationAborted）");
            }
            catch (IOException ex)
            {
                Debug.LogError($"I/Oエラー: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"予期しないエラー: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _stream?.Close();
            _client?.Close();
        }
    }
}
