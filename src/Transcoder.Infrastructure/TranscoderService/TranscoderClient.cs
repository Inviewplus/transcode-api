using System;
using System.Diagnostics;

namespace Transcoder.Infrastructure.TranscoderService
{
    public class TranscoderClient : ITranscoder
    {
        public void ConvertHLS(string input, string output, string quality)
        {
            string ffmpegArgs = $"-i {input} -c:v h264 -c:a aac -b:v 2M -b:a 128k -hls_time 2  -hls_list_size 15 {output}/{quality}p/index.m3u8";

            // Inicia o processo do FFmpeg para converter para HLS
            Process(ffmpegArgs);
        }

        public void ConvertQuality(string input, string output)
        {
            // Comando FFmpeg para conversão para 1080p
            string command1080p = $"-i \"{input}\" -vf scale=1920:1080 \"{output}/output_1080_p.mp4\" -y";

            // Comando FFmpeg para conversão para 720p
            string command720p = $"-i \"{input}\" -vf scale=1280:720 \"{output}/output_720_p.mp4\" -y";

            // Comando FFmpeg para conversão para 480p
            string command480p = $"-i \"{input}\" -vf scale=854:480 \"{output}/output_480_p.mp4\" -y";


            Process(command1080p);
            Process(command720p);
            Process(command480p);
        }


        #region PROCESS TRANSCODER
        private static void Process(string ffmpegArgs)
        {
            try
            {
                Console.WriteLine(ffmpegArgs);
                // Configurar o processo para executar o comando FFmpeg
                Process ffmpegProcess = new Process();
                ffmpegProcess.StartInfo.FileName = "ffmpeg"; // Se o ffmpeg não estiver no PATH, forneça o caminho completo
                ffmpegProcess.StartInfo.Arguments = ffmpegArgs;
                ffmpegProcess.StartInfo.UseShellExecute = false;
                ffmpegProcess.StartInfo.RedirectStandardOutput = true;
                ffmpegProcess.StartInfo.RedirectStandardError = true;

                // Iniciar o processo FFmpeg
                ffmpegProcess.Start();
                ffmpegProcess.WaitForExit();

                Console.WriteLine("Conversão concluída com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao converter o vídeo: {ex.Message}");
            }
        }
        #endregion
    }
}
