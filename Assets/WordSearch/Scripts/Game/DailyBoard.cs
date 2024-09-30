using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BBG.WordSearch.Board;

namespace BBG.WordSearch
{
    public class DailyBoard
    {
        public List<string> words;           // Words available for selection
        //public List<string> foundWords;      // Words that have been found
        public List<char> letterHintsUsed;   // Letter hints
        public string quoteText;             // Full quote
        public List<string> missingWords = new List<string>();    // Words missing from the quote

        public int rows;                     // Number of rows in the grid
        public int cols;

        public void FromJson(JSONNode json)
        {
            // Create a new DailyBoard object to store the data
            DailyBoard dailyBoard = new DailyBoard();

            // Parse rows and cols
            rows = json["rows"].AsInt;
            cols = json["cols"].AsInt;


            // Initialize lists and sets
            words = new List<string>();
            missingWords = new List<string>();

            for (int i = 0; i < json["toFindWords"].AsArray.Count; i++)
            {
                string missingWord = json["toFindWords"].AsArray[i].Value.Trim(' ', '"');
                words.Add(missingWord);
            }
            //foundWords = new HashSet<string>();


            // Parse toFindWords (these are the missing words in the quote)
            missingWords = new List<string>(); // Initialize missingWords in DailyBoard
            for (int i = 0; i < json["toFindWords"].AsArray.Count; i++)
            {
                string missingWord = json["toFindWords"].AsArray[i].Value.Trim(' ', '"');
                missingWords.Add(missingWord);
            }

            // Parse quoteText (assuming it's included in JSON)
            if (json.AsObject.HasKey("quoteText"))
            {
                quoteText = json["quoteText"].Value;
            }

        }
    }
}
