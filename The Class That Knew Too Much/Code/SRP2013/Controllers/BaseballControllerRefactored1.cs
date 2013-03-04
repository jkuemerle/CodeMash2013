using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using SRPTalk.Models;

namespace SRPTalk.Controllers
{
    public class BaseballController : Controller
    {
        public ViewResult Index()
        {
            var initialModel = new SubmitFormViewModel { NumParagraphs = 5 };
            return View(initialModel);
        }

        [HttpPost]
        public ViewResult Index(SubmitFormViewModel model)
        {
            Random random = new Random();
            List<string> words = GetWords();
            model.GeneratedParagraphs = GetCorpus(model.NumParagraphs, random, words);
            if (model.StartWithBaseballIpsum)
                ModifyFirstParagraph(model);
            return View(model);
        }
  
        private static void ModifyFirstParagraph(SubmitFormViewModel model)
        {
            // get the first paragraph
            var firstSentence = model.GeneratedParagraphs[0];
            // split it into individual words
            var firstSentenceWords = firstSentence.Split(' ');
            // replace the first five words
            firstSentenceWords[0] = "Baseball";
            firstSentenceWords[1] = "ipsum";
            firstSentenceWords[2] = "dolor";
            firstSentenceWords[3] = "sit";
            firstSentenceWords[4] = "amet";
            // replace the first paragraph with the modified one
            model.GeneratedParagraphs[0] = string.Join(" ", firstSentenceWords);
        }
  
        private static List<string> GetCorpus(int numParagraphs, Random random, List<string> words)
        {
            var paragraphs = new List<string>();
            for (int i = 0; i < numParagraphs; i++)
            {
                var numSentences = random.Next(6, 7);   // 6 or 7 sentences
                var commaSentence = random.Next(0, numSentences); // one comma in the paragraph randomly
                var paragraph = GetParagraph(numSentences, random, words, commaSentence);
                paragraphs.Add(paragraph.Trim());
            }
            return paragraphs;
        }
  
        private static string GetParagraph(int numSentences, Random random, List<string> words, int commaSentence)
        {
            var paragraph = string.Empty;
            for (int j = 0; j < numSentences; j++)
            {
                var numWords = random.Next(7, 10); // from 7 to 10 words per sentence
                var sentence = GetSentence(numWords, words, random, commaSentence);
                sentence = sentence.Trim();
                sentence += ". ";
                paragraph += sentence;
            }
            return paragraph;
        }
  
        private static string GetSentence(int numWords, List<string> words, Random random, int commaSentence)
        {
            var sentence = string.Empty;
            for (int k = 0; k < numWords; k++)
            {
                var word = words[random.Next(0, words.Count - 1)];
                if (k == 0)
                {
                    // uppercase the first letter of the first word
                    var wordArray = word.ToCharArray();
                    wordArray[0] = char.ToUpper(wordArray[0]);
                    sentence += new string(wordArray);
                }
                else
                    sentence += word;
                // add a comma after the 5th word once per paragraph
                if (k == commaSentence && k == 4)
                    sentence += ",";
                sentence += " ";
            }
            return sentence;
        }
  
        private static List<string> GetWords()
        {
            // all the words in the dictionary
            var words = new List<string>();
            // read in all the words to a List
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("SRP2013.Models.words.txt"))
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                        words.Add(line.Trim());
                }
            }
            return words;
        }
    }
}
