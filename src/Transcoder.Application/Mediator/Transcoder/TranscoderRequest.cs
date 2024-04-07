using MediatR;

namespace Transcoder.Application.Mediator.Transcoder
{
    public class TranscoderRequest : IRequest<TranscoderResponse>
    {
        public TranscoderRequest(string idProduct, string fileName)
        {
            IdProduct = idProduct;
            FileName = fileName;
        }

        public string IdProduct { get; set; }
        public string FileName { get; private set; }
    }
}
