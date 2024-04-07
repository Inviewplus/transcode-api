using System.Diagnostics;

namespace Transcoder.Infrastructure.TranscoderService
{
    public class TranscoderClient : ITranscoder
    {
        public void ConvertHLS(string input, string output, string quality)
        {
            string ffmpegArgs = $"-i {input} -c:v h264 -c:a aac -b:v 2M -b:a 128k -hls_time 10  -hls_list_size 5 {output}\\{quality}p\\index.m3u8";

            // Inicia o processo do FFmpeg para converter para HLS
            Process(ffmpegArgs);
        }

        public void ConvertQuality(string input, string output)
        {
            // Comando FFmpeg para conversão para 1080p
            string command1080p = $"-i \"{input}\" -vf scale=1920:1080 \"{output}\\output_1080_p.mp4\" -y";

            // Comando FFmpeg para conversão para 720p
            string command720p = $"-i \"{input}\" -vf scale=1280:720 \"{output}\\output_720_p.mp4\" -y";

            // Comando FFmpeg para conversão para 480p
            string command480p = $"-i \"{input}\" -vf scale=854:480 \"{output}\\output_480_p.mp4\" -y";


            Process(command1080p);
            Process(command720p);
            Process(command480p);
        }


        #region PROCESS TRANSCODER
        private static void Process(string ffmpegArgs)
        {
            // Inicia o processo do FFmpeg para converter para HLS
            Process process = new Process();
            process.StartInfo.FileName = "ffmpeg";
            process.StartInfo.Arguments = ffmpegArgs;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            // Manipula a saída do processo
            process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            Console.WriteLine("Conversão para HLS concluída.");
        }
        #endregion
    }
}
