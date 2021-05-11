using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreTable : MonoBehaviour
{
    //Public variables of HighScoreTable
    [SerializeField] string highscoreFile = "scores.txt";
    [SerializeField] Font scoreFont;

    //List to store highscores
    List<HighScoreEntry> allScores = new List<HighScoreEntry>();

    struct HighScoreEntry
    {
        public int score;
        public string name;
    }

    public void OnEnable()
    {
        LoadHighScoreTable();
        SortHighScoreEntries();
        CreateHighScoreText();
    }

    //Reads Highscores from the scores.txt file
    public void LoadHighScoreTable()
    {
        using (TextReader file = File.OpenText(highscoreFile))
        {
            allScores.Clear();
            string text = null;
            while((text = file.ReadLine()) != null) 
            {
                Debug.Log(text);
                string[] splits = text.Split(' ');
                HighScoreEntry entry;
                entry.name = splits[0];
                entry.score = int.Parse(splits[1]);
                allScores.Add(entry);
            }
        }
    }

    //Creates the Highscore table in the interface
    void CreateHighScoreText()
    {
        GameObject[] highscores = GameObject.FindGameObjectsWithTag("Highscore Text");
        if(highscores != null)
        {
            foreach(GameObject i in highscores)
            {
                Destroy(i);
            }
        }

        for (int i = 0; i < allScores.Count && i < 10; ++i)
        {
            GameObject o = new GameObject();
            o.tag = "Highscore Text";
            o.transform.parent = transform;

            Text t = o.AddComponent<Text>();
            t.text = allScores[i].name + "\t\t" + allScores[i].score;
            t.font = scoreFont;
            t.fontSize = 50;
            t.color = Color.black;

            o.transform.localPosition = new Vector3(0, -(i) * 6, 0);

            o.transform.localRotation = Quaternion.identity;
            o.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            o.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 100);
        }
    }

    
    public void SortHighScoreEntries()
    {
        allScores.Sort((x, y) => y.score.CompareTo(x.score));
    }

    public int GetHighestScore()
    {
        return allScores[0].score;
    }


    

}
