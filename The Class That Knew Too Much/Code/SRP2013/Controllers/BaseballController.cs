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
            // all the words in the dictionary
            var words = new List<string>();
            var random = new Random();
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
            // populate the viewmodel with an empty list of sentences
            model.GeneratedParagraphs = new List<string>();
            for (int i = 0; i < model.NumParagraphs;i++)
            {
                var paragraph = string.Empty;
                var numSentences = random.Next(6, 7);   // 6 or 7 sentences
                var commaSentence = random.Next(0, numSentences); // one comma in the paragraph randomly
                for(int j=0;j<numSentences;j++)
                {
                    var numWords = random.Next(7, 10); // from 7 to 10 words per sentence
                    for(int k=0;k<numWords;k++)
                    {
                        var word = words[random.Next(0, words.Count - 1)];
                        if (k == 0)
                        {
                            // uppercase the first letter of the first word
                            var wordArray = word.ToCharArray();
                            wordArray[0] = char.ToUpper(wordArray[0]);
                            paragraph += new string(wordArray);
                        }
                        else
                            paragraph += word;
                        // add a comma after the 5th word once per paragraph
                        if(k == commaSentence && k == 4)
                            paragraph += ",";
                        paragraph += " ";
                    }
                    paragraph = paragraph.Trim();
                    paragraph += ". ";
                }
                model.GeneratedParagraphs.Add(paragraph.Trim());
            }
            // if the user wants to start with "baseball ipsum dolor sit amet"
            if (model.StartWithBaseballIpsum)
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
            return View(model);
        }
    }
}
