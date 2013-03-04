using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using SRPTalk.Models;

namespace SRPTalk.Controllers
{
    public class BaseballController : Controller
    {
        readonly CorpusGenerator _corpusGenerator;

        public BaseballController()
        {
            _corpusGenerator = 
                new CorpusGenerator(
                    new ParagraphGenerator(
                        new SentenceGenerator(
                            new IpsumDictionary())));
        }

        public ViewResult Index()
        {
            var initialModel = new SubmitFormViewModel { NumParagraphs = 5 };
            return View(initialModel);
        }

        [HttpPost]
        public ViewResult Index(SubmitFormViewModel model)
        {
            model.GeneratedParagraphs =
                _corpusGenerator.GenerateParagraphs(
                    model.NumParagraphs,
                    model.StartWithBaseballIpsum);
            
            return View(model);
        }
    }

    public class CorpusGenerator
    {
        readonly ParagraphGenerator _paragraphGenerator;

        public CorpusGenerator(ParagraphGenerator paragraphGenerator)
        {
            _paragraphGenerator = paragraphGenerator;
        }

        public List<string> GenerateParagraphs(int numParagraphs, bool startWithBaseballIpsum)
        {
            var paragraphs = new List<string>();
            for (int i = 0; i < numParagraphs; i++)
            {
                var paragraph = _paragraphGenerator.GetRandomParagraph();
                paragraphs.Add(paragraph.Trim());
            }

            // if the user wants to start with "baseball ipsum dolor sit amet"
            if (startWithBaseballIpsum)
            {
                // get the first paragraph
                var firstSentence = paragraphs[0];
                // split it into individual words
                var firstSentenceWords = firstSentence.Split(' ');
                // replace the first five words
                firstSentenceWords[0] = "Baseball";
                firstSentenceWords[1] = "ipsum";
                firstSentenceWords[2] = "dolor";
                firstSentenceWords[3] = "sit";
                firstSentenceWords[4] = "amet";
                // replace the first paragraph with the modified one
                paragraphs[0] = string.Join(" ", firstSentenceWords);
            }
            return paragraphs;
        }
    }

    public class ParagraphGenerator
    {
        readonly SentenceGenerator _sentenceGenerator;

        public ParagraphGenerator(SentenceGenerator sentenceGenerator)
        {
            _sentenceGenerator = sentenceGenerator;
        }

        public string GetRandomParagraph()
        {
            var paragraph = string.Empty;
            var random = new Random();
            var numSentences = random.Next(6, 7);   // 6 or 7 sentences
            var commaSentence = random.Next(0, numSentences); // one comma in the paragraph randomly
            for (int j = 0; j < numSentences; j++)
                paragraph += _sentenceGenerator.GetRandomSentence(commaSentence);
            return paragraph;
        }
    }

    public class SentenceGenerator
    {
        readonly IpsumDictionary _words;
        readonly Random _random;

        public SentenceGenerator(IpsumDictionary words)
        {
            _words = words;
            _random = new Random();
        }

        public string GetRandomSentence(int commaSentence)
        {
            var sentence = string.Empty;

            var numWords = _random.Next(7, 10); // from 7 to 10 words per sentence
            for (int k = 0; k < numWords; k++)
            {
                var word = _words.GetRandomWord();
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
            sentence = sentence.Trim();
            sentence += ". ";
            return sentence;
        }
    }

    public class IpsumDictionary
    {
        readonly List<string> _words;
        readonly Random _rand;
        readonly int _numWords;

        public IpsumDictionary()
        {
            _rand = new Random();
            _words = LoadFromTxtFile();
            _numWords = _words.Count;
        }

        List<string> LoadFromTxtFile()
        {
            var words = new List<string>();
            var assembly = GetType().Assembly;
            using (var stream = assembly.GetManifestResourceStream("SRP2013.Refactored.Models.words.txt"))
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

        public string GetRandomWord()
        {
            return _words[_rand.Next(0, _numWords - 1)];
        }
    }
}
