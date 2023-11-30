namespace CopyCatAiApi.Interfaces
{
    public class EmbeddingServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public EmbeddingServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEmbeddingService Create()
        {
            return _serviceProvider.GetRequiredService<IEmbeddingService>();
        }
    }
}