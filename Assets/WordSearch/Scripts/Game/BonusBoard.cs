using FunGames.Tools.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BBG.WordSearch
{
    public class BonusBoard
    {
        public int rows;
        public int cols;
        public List<string> wordsNotToAdd;
        public List<string> words;
        public List<string> foundedWords;
        public void FromJson(JSONNode json, JSONNode _json)
        {
            int rows = _json["rows"].AsInt;
            int cols = _json["cols"].AsInt;
            wordsNotToAdd = new List<string>();
            words = new List<string>();
            foundedWords = new List<string>();
            // Jo bhi zyada hai usay limit set karein
            int limit = rows > cols ? rows : cols;
            for (int i = 0; i < _json["words"].AsArray.Count; i++)
            {
                string word = _json["words"].AsArray[i].Value;
                // Check agar word length 2 se zyada ho aur limit se chhoti ho

                wordsNotToAdd.Add(word);

            }
            for (int i = 0; i < json["words"].AsArray.Count; i++)
            {
                string word = json["words"].AsArray[i].Value;
                // Check agar word length 2 se zyada ho aur limit se chhoti ho
                if (word.Length > 2 && word.Length <= limit)
                {
                    if (!wordsNotToAdd.Contains(word.ToUpper()))
                    {
                        words.Add(word);
                    }
                }
            }
            FoundedListUpDate();
        }

        public void FoundedListUpDate()
        {
            PlayerPrefs.DeleteKey("FoundedWord");
            PlayerPrefs.Save();
            string contents = (PlayerPrefs.GetString("FoundedWord", JsonUtility.ToJson(foundedWords)));
            JSONNode json = JSON.Parse(contents);
            //Debug.Log(contents);
            //Debug.Log(json);
            for (int i = 0; i < json["words"].AsArray.Count; i++)
            {
                string word = json["words"].AsArray[i].Value;
                // Check agar word length 2 se zyada ho aur limit se chhoti ho

                foundedWords.Add(word);
            }

            //foundedWords = JsonUtility.FromJson<List<string>>(contents);
        }
    }
}
