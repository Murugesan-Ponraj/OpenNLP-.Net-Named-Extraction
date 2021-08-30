using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNLPDotNet
{
    public class EntityExtractor
    {
        /// <summary>
        /// Extractor for the entity types available in openNLP.
        /// Copyright 2013, Don Krapohl www.augmentedintel.com
        /// This source is free for unlimited distribution and use
        /// TODO:
        ///     try/catch/exception handling
        ///     filestream closure
        ///     model training if desired
        ///     Regex or dictionary entity extraction
        ///     clean up the setting of the Name Finder model path
        /// </summary>
        /// Call syntax:  myList = ExtractEntities(myInText, EntityType.Person);

        private string sentenceModelPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "en-sent.bin");   //path to the model for sentence detection
        private string nameFinderModelPath;                              //NameFinder model path for English names
        private string tokenModelPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "en-token.bin");     //model path for English tokens
        public enum EntityType
        {
            Date = 0,
            Location,
            Money,
            Organization,
            Person,
            Time
        }

        public List<string> ExtractEntities(string inputData)
        {
            /*required steps to detect names are:
             * downloaded sentence, token, and name models from http://opennlp.sourceforge.net/models-1.5/
             * 1. Parse the input into sentences
             * 2. Parse the sentences into tokens
             * 3. Find the entity in the tokens
 
            */
            var personPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "en-ner-person.bin");
            nameFinderModelPath = personPath;
            //------------------Preparation -- Set Name Finder model path based upon entity type-----------------
            //switch (targetType)
            //{
            //    case EntityType.Date:
            //        nameFinderModelPath = "c:\\models\\en-ner-date.bin";
            //        break;
            //    case EntityType.Location:
            //        nameFinderModelPath = "c:\\models\\en-ner-location.bin";
            //        break;
            //    case EntityType.Money:
            //        nameFinderModelPath = "c:\\models\\en-ner-money.bin";
            //        break;
            //    case EntityType.Organization:
            //        nameFinderModelPath = "c:\\models\\en-ner-organization.bin";
            //        break;
            //   case EntityType.Person:
            //        nameFinderModelPath = "c:\\models\\en-ner-person.bin";
            //        break;
            //    case EntityType.Time:
            //        nameFinderModelPath = "c:\\models\\en-ner-time.bin";
            //        break;
            //    default:
            //        break;
            //}

            //----------------- Preparation -- load models into objects-----------------
            //initialize the sentence detector
            opennlp.tools.sentdetect.SentenceDetectorME sentenceParser = prepareSentenceDetector();

            //initialize person names model
            opennlp.tools.namefind.NameFinderME nameFinder = prepareNameFinder();

            //initialize the tokenizer--used to break our sentences into words (tokens)
            opennlp.tools.tokenize.TokenizerME tokenizer = prepareTokenizer();

            //------------------  Make sentences, then tokens, then get names--------------------------------

            String[] sentences = sentenceParser.sentDetect(inputData); //detect the sentences and load into sentence array of strings
            List<string> results = new List<string>();

            foreach (string sentence in sentences)
            {
                //now tokenize the input.
                //"Don Krapohl enjoys warm sunny weather" would tokenize as
                //"Don", "Krapohl", "enjoys", "warm", "sunny", "weather"
                string[] tokens = tokenizer.tokenize(sentence);

                //do the find
                opennlp.tools.util.Span[] foundNames = nameFinder.find(tokens);

                //important:  clear adaptive data in the feature generators or the detection rate will decrease over time.
                nameFinder.clearAdaptiveData();

                results.AddRange(opennlp.tools.util.Span.spansToStrings(foundNames, tokens).AsEnumerable());
            }

            return results;
        }

        #region private methods
        private opennlp.tools.tokenize.TokenizerME prepareTokenizer()
        {
            java.io.FileInputStream tokenInputStream = new java.io.FileInputStream(tokenModelPath);     //load the token model into a stream
            opennlp.tools.tokenize.TokenizerModel tokenModel = new opennlp.tools.tokenize.TokenizerModel(tokenInputStream); //load the token model
            return new opennlp.tools.tokenize.TokenizerME(tokenModel);  //create the tokenizer
        }
        private opennlp.tools.sentdetect.SentenceDetectorME prepareSentenceDetector()
        {
            java.io.FileInputStream sentModelStream = new java.io.FileInputStream(sentenceModelPath);       //load the sentence model into a stream
            opennlp.tools.sentdetect.SentenceModel sentModel = new opennlp.tools.sentdetect.SentenceModel(sentModelStream);// load the model
            return new opennlp.tools.sentdetect.SentenceDetectorME(sentModel); //create sentence detector
        }
        private opennlp.tools.namefind.NameFinderME prepareNameFinder()
        {
            java.io.FileInputStream modelInputStream = new java.io.FileInputStream(nameFinderModelPath); //load the name model into a stream
            opennlp.tools.namefind.TokenNameFinderModel model = new opennlp.tools.namefind.TokenNameFinderModel(modelInputStream); //load the model
            return new opennlp.tools.namefind.NameFinderME(model);                   //create the namefinder
        }
        #endregion
    }
}
