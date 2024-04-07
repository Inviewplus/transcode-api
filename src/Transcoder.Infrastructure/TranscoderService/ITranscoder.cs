namespace Transcoder.Infrastructure.TranscoderService
{
    public interface ITranscoder
    {
        void ConvertHLS(string input, string output, string quality);
        void ConvertQuality(string input, string output);
    }
}
