using System.Collections.Generic;

namespace Laba1.SaveController.Model
{
    public class SaveModel
    {
        public int CountVertex { get;}
        public Dictionary<string, string> Dictionary { get; }
        
        public SaveModel(int countVertex, Dictionary<string,string> dictionary)
        {
            CountVertex = countVertex;
            Dictionary = dictionary;
        }
    }
}