using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

using System.Text;

namespace CopyCatAiApi.Services
{
    public class FileService
    {
        private readonly OpenAIService _openAIService;

        public FileService(OpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public List<string> ConvertPdfToText(Stream pdfStream)
        {
            using var PdfReader = new PdfReader(pdfStream);
            using var PdfDocument = new PdfDocument(PdfReader);
            var text = new StringBuilder();

            for (int page = 1; page <= PdfDocument.GetNumberOfPages(); page++)
            {
                text.Append(PdfTextExtractor.GetTextFromPage(PdfDocument.GetPage(page)));
            }

            // split text into blocks of 1000 characters, with an overlap of 200 between the blocks
            var textBlocks = new List<string>();
            var textLength = text.Length;
            var blockLength = 1000;
            var overlap = 200;
            var blockStart = 0;

            while (blockStart < textLength)
            {
                var blockEnd = Math.Min(blockStart + blockLength, textLength);
                var block = text.ToString(blockStart, blockEnd - blockStart);
                textBlocks.Add(block);
                blockStart += blockLength - overlap;
            }
            return textBlocks;
        }

        public async Task<List<string>> ConvertTextBlocksToEmbeddings(List<string> textBlocks)
        {
            var embeddings = new List<string>();

            foreach (var block in textBlocks)
            {
                try
                {
                    var embedding = await _openAIService.GetEmbedding(block);
                    embeddings.Add(embedding);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return embeddings;
        }
    }
}