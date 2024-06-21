using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Services.Interfaces.AIService
{
    public interface IAnswerGeneratorService
    {
        Task<string> GenerateAnswer(string prompt);

        Task<string> GenerateAnswerSpeechToText(string base64, string fileName);

        Task<string> GenerateAnswerSpeechToAnswer(string base64, string fileName);

    }
}