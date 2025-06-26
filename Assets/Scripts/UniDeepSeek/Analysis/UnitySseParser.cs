using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

namespace Xiyu.UniDeepSeek
{
    public static class UnitySseParser
    {
        public static async UniTask ParseStreamAsync(Stream stream, ChannelWriter<string> writer, CancellationToken cancellation = default)
        {
            using var reader = new StreamReader(stream);
            var dataBuilder = new StringBuilder();
            var inDataBlock = false;
            while (!cancellation.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (line == null) break;

                // 空行表示事件结束
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (dataBuilder.Length > 0)
                    {
                        // await UniTask.SwitchToMainThread(); // 确保在主线程回调
                        writer.TryWrite(dataBuilder.ToString());
                        dataBuilder.Clear();
                    }

                    continue;
                }

                // 处理数据行
                if (line.StartsWith("data:"))
                {
                    inDataBlock = true;
                    var data = line.Length > 5 ? line.Substring(5).Trim() : string.Empty;
                    dataBuilder.Append(data);
                }
                // 其他事件类型可以在此扩展
                else if (inDataBlock)
                {
                    // 多行数据延续
                    dataBuilder.Append("\n").Append(line);
                }
            }

            // 处理最后的数据块
            if (dataBuilder.Length > 0)
            {
                // await UniTask.SwitchToMainThread();
                writer.TryWrite(dataBuilder.ToString());
            }

            writer.TryComplete();
        }

        public static UniTaskCancelableAsyncEnumerable<string> ParseStreamAsync(Stream stream, CancellationToken cancellation = default)
        {
            return UniTaskAsyncEnumerable.Create<string>(Create).WithCancellation(cancellation);

            async UniTask Create(IAsyncWriter<string> writer, CancellationToken cts)
            {
                using var reader = new StreamReader(stream);
                var dataBuilder = new StringBuilder();
                var inDataBlock = false;
                while (!cts.IsCancellationRequested)
                {
                    var line = await reader.ReadLineAsync();
                    if (line == null) break;

                    // 空行表示事件结束
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        if (dataBuilder.Length > 0)
                        {
                            // await UniTask.SwitchToMainThread(); // 确保在主线程回调
                            await writer.YieldAsync(dataBuilder.ToString());
                            dataBuilder.Clear();
                        }

                        continue;
                    }

                    // 处理数据行
                    if (line.StartsWith("data:"))
                    {
                        inDataBlock = true;
                        var data = line.Length > 5 ? line.Substring(5).Trim() : string.Empty;
                        dataBuilder.Append(data);
                    }
                    // 其他事件类型可以在此扩展
                    else if (inDataBlock)
                    {
                        // 多行数据延续
                        dataBuilder.Append("\n").Append(line);
                    }
                }

                // 处理最后的数据块
                if (dataBuilder.Length > 0)
                {
                    // await UniTask.SwitchToMainThread();
                    await writer.YieldAsync(dataBuilder.ToString());
                }
            }
        }
    }
}