using System;
using System.Linq;

namespace OpenNLPDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                EntityExtractor extractor = new EntityExtractor(); 

                var path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "speechText.txt");
                string speechContent = System.IO.File.ReadAllText(path);
                var extractedUniquenames = extractor.ExtractEntities(speechContent).Distinct().ToList();
                foreach (var names in extractedUniquenames)
                {
                    if (names.Length != 1) // to remove single character names
                    {
                        speechContent = speechContent.Replace(names, "<b>" + names + "</b>");
                    }

                } 
                 
                System.IO.File.WriteAllText(@"ExtractedNames.htm", speechContent); //file created in bin folder 

 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
