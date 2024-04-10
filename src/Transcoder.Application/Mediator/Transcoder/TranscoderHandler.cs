using MediatR;
using Oci.ObjectstorageService.Responses;
using System.Text;
using Transcoder.Infrastructure.OciClient;
using Transcoder.Infrastructure.TranscoderService;

namespace Transcoder.Application.Mediator.Transcoder
{
    public class TranscoderHandler : IRequestHandler<TranscoderRequest, TranscoderResponse>
    {
        private readonly IOracleClient _oracleClient;
        private readonly ITranscoder _transcoder;

        public TranscoderHandler(IOracleClient oracleClient, ITranscoder transcoder)
        {
            _oracleClient = oracleClient;
            _transcoder = transcoder;
        }

        public async Task<TranscoderResponse> Handle(TranscoderRequest request, CancellationToken cancellationToken)
        {
            var response = await _oracleClient.GetFile(request.FileName);

            if (response is null)
                return null;

            string inputPath, outputPath;
        
            CreatePath(out inputPath, out outputPath);
            GetFileToLocal(request, response, inputPath);
            ConvertHLS(inputPath, outputPath);
            await UploadToBucket(request, outputPath);

            Directory.Delete(inputPath, true);
            Directory.Delete(outputPath, true);

            var result = await _oracleClient.GetUrl(request.IdProduct);

            return new TranscoderResponse { OK = true, Url = result };
        }

        private async Task UploadToBucket(TranscoderRequest request, string outputPath)
        {
            string[] files = Directory.GetFiles(outputPath);
            string[] pastas = Directory.GetDirectories(outputPath);

            try
            {
                foreach (string filePath in files)
                {
                    // Lê o conteúdo do arquivo
                    byte[] fileContent = File.ReadAllBytes(filePath);
                    // Obtém o nome do arquivo do caminho completo
                    string fileName = Path.GetFileName(filePath);

                    await _oracleClient.PutFile(request.IdProduct, fileName, new MemoryStream(fileContent));
                }


                foreach (string path in pastas)
                {
                    if (Directory.Exists(path))
                    {
                        string[] partes = path.Split(Path.DirectorySeparatorChar);
                        string ultimaPasta = partes[partes.Length - 1] + "/";

                        await _oracleClient.PutFile(request.IdProduct, ultimaPasta, null);

                        string[] arquivos = Directory.GetFiles(path);
                        if (arquivos.Length > 0)
                        {
                            foreach (string arquivo in arquivos)
                            {
                                byte[] fileContent = File.ReadAllBytes(arquivo);
                                string fileName = Path.GetFileName(arquivo);

                                await _oracleClient.PutFile(request.IdProduct, ultimaPasta + fileName, new MemoryStream(fileContent));

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
        private void GetFileToLocal(TranscoderRequest request, GetObjectResponse? response, string inputPath)
        {
            using (var responseStream = response?.InputStream)
            {
                using (var fileStream = File.Create(Path.Combine(inputPath, request.FileName)))
                {
                    responseStream?.CopyTo(fileStream);
                }
                Console.WriteLine(Path.Combine(inputPath, request.FileName));
                _transcoder.ConvertQuality(Path.Combine(inputPath, request.FileName), inputPath);
                File.Delete(Path.Combine(inputPath, request.FileName));
            }
        }

        private void ConvertHLS(string inputPath, string outputPath)
        {
            Console.WriteLine($"Funcao de conversão");
            Console.WriteLine($"====================================");
            Console.WriteLine($"=====================================");
            Console.WriteLine(inputPath);
            string[] files = Directory.GetFiles(inputPath);
            Console.WriteLine(files.Length);
            var masterPlaylistContent = new StringBuilder();
            masterPlaylistContent.AppendLine("#EXTM3U");
            masterPlaylistContent.AppendLine("#EXT-X-VERSION:3");

            foreach (var file in files)
            {
                var objectName = Path.GetFileName(file).Split("_");
                _transcoder.ConvertHLS(file, outputPath, objectName[1]);
                masterPlaylistContent.AppendLine($"#EXT-X-STREAM-INF:BANDWIDTH=2000000,RESOLUTION={Resolution(objectName[1])}");
                masterPlaylistContent.AppendLine($"{objectName[1]}p/index.m3u8");
            }

            File.WriteAllText($"{outputPath}/master.m3u8", masterPlaylistContent.ToString());
        }

        static void CreatePath(out string inputPath, out string outputPath)
        {
            var novaPasta = Guid.NewGuid().ToString();

            Directory.CreateDirectory($"/transcoder/input/{novaPasta}");
            inputPath = Path.Join($"/transcoder/input/{novaPasta}");

            Directory.CreateDirectory($"/transcoder/output/{novaPasta}");
            outputPath = Path.Join($"/transcoder/output/{novaPasta}");

            Directory.CreateDirectory(outputPath + "/1080p");
            Directory.CreateDirectory(outputPath + "/720p");
            Directory.CreateDirectory(outputPath + "/480p");

            Console.WriteLine("Pastas Criadas com sucesso");   
        }

        static string Resolution(string resolution) => resolution switch
        {
            "1080" => "1920x1080",
            "720" => "1280x720",
            "480" => "854x480",
            _ => ""
        };
    }
}
