using Control.Endeavour.FrontEnd.Services.Interfaces.AIService;
using OpenAI_API;
using OpenAI_API.Completions;
using System.Buffers.Text;

namespace Control.Endeavour.FrontEnd.Services.Services.AIService
{
    public class AnswerGeneratorService : IAnswerGeneratorService
    {
        public async Task<string> GenerateAnswer(string prompt)
        {
            string apiKey = "sk-9m9XLSjOYt17fdd8iTeST3BlbkFJl7A291Gj6LVGiMCT7YOZ";
            string answer = string.Empty;

            var openAi = new OpenAIAPI(apiKey);

            CompletionRequest completion = new CompletionRequest();
            completion.Prompt = prompt;
            completion.MaxTokens = 4000;

            var response = await openAi.Completions.CreateCompletionAsync(completion);

            if (response != null)
            {
                foreach (var item in response.Completions)
                {
                    answer = item.Text;
                }
            }

            return answer;
        }

        public async Task<string> GenerateAnswerSpeechToText(string base64, string fileName)
        {
            string apiKey = "sk-9m9XLSjOYt17fdd8iTeST3BlbkFJl7A291Gj6LVGiMCT7YOZ";

            var openAi = new OpenAIAPI(apiKey);

            if (base64.StartsWith("data:"))
            {
                base64 = base64.Substring(base64.IndexOf(",") + 1);
            }
            byte[] bytes = Convert.FromBase64String(base64);

            using Stream memoryStream = new MemoryStream(bytes);

            string resultText = await openAi.Transcriptions.GetTextAsync(memoryStream, fileName);

            return resultText;
        }

        public async Task<string> GenerateAnswerSpeechToAnswer(string base64, string fileName)
        {
            string apiKey = "sk-9m9XLSjOYt17fdd8iTeST3BlbkFJl7A291Gj6LVGiMCT7YOZ";

            var openAi = new OpenAIAPI(apiKey);

            if (base64.StartsWith("data:"))
            {
                base64 = base64.Substring(base64.IndexOf(",") + 1);
            }
            byte[] bytes = Convert.FromBase64String(base64);

            using Stream memoryStream = new MemoryStream(bytes);

            string resultText = await openAi.Transcriptions.GetTextAsync(memoryStream, fileName);

            string answer = await GenerateAnswer(resultText);

            return answer;
        }
    }
}