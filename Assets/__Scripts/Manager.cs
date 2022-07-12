using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum gameStatus
{
    init, next, play, gameover, win
}

public class Manager : Loader<Manager>
{
    [Header("UI")]
    public Text moneyLabel;
    public Text waveCurrent;
    public Text escapedTotalLabel;
    public Text buttonPlayLabel;
    public Button buttonPlay;

    [Header("Set in Inspector")]

    public int wavesTotal = 10;
    public int escapedMaximum = 10;
    public GameObject spawnPoint;
    public Enemy[] enemiesPrefabs;
    public int enemies;
    public int enemiesPerSpawn;
    public float spawnDelay = 0.5f;
    public int hardestPossibleEnemy;
    public float spawnAreaRadius;

    public List<string> initialStrings;
    public List<int> initialValues;
    
    public Dictionary<string, int> initialValuesDict;


    [Header("Set Dynamically")]
    
    public List<Enemy> enemyList = new List<Enemy>();
    public int _waveNumber = 0;
    public int money = 10;
    public int escapedTotal = 0;
    public int escaped = 0;
    public int killed = 0;
    public gameStatus currentState = gameStatus.init;
    public AudioSource audioSource;

    public int waveNumber
    {
        get { return _waveNumber; }
        set { Debug.Log(waveNumber); _waveNumber = value; }
    }

    IEnumerator Spawn()
    {
        if(enemiesPerSpawn > 0 && enemyList.Count < enemies)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                if(enemyList.Count < enemies)
                {
                    float max = (float)hardestPossibleEnemy + 0.1f;
                    Enemy randomPrefab = enemiesPrefabs[Mathf.RoundToInt(Random.Range(0, max))];
                    Enemy newEnemy = Instantiate(randomPrefab) as Enemy;
                    Vector3 offset = Vector3.one;
                    offset.x *= Random.Range(-spawnAreaRadius, spawnAreaRadius);
                    offset.z = offset.y = 0;
                    newEnemy.transform.position = offset + spawnPoint.transform.position;
                }
            }
            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(Spawn());
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        initialValuesDict = new Dictionary<string, int>();
        audioSource = GetComponent<AudioSource>();
        CompileDict();
        InitVariables();
        moneyLabel.text = money.ToString();
        buttonPlay.gameObject.SetActive(false);
        ShowButton();
    }
    private void Update()
    {
        EscapeHandler();
    }
    public void RegisterEnemy(Enemy enemy)
    {
        enemyList.Add(enemy);
    }
    public void UnregisterEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }
    public void RemoveAllEnemies()
    {
        foreach (Enemy enemy in enemyList)
            Destroy(enemy.gameObject);
        enemyList.Clear();
    }

    /// <summary>
    /// Âûחמג ןנט סלונעט / בודסעגו מקונוהםמדמ גנאדא
    /// </summary>
    public void IsWaveOver()
    {
        escapedTotalLabel.text = "Escaped " + escaped + " / " + escapedMaximum;
        if((escaped + killed) == enemies)
        {
            if (waveNumber <= enemiesPrefabs.Length - 1)
                hardestPossibleEnemy = waveNumber;
            SetCurrentGameState();
            ShowButton();
        }
    }
    public void SetCurrentGameState() 
    {
        if (escapedTotal >= escapedMaximum)
        {
            currentState = gameStatus.gameover;
        }
        else if (waveNumber >= wavesTotal)
        {
            currentState = gameStatus.win;
        }
        else
        {
            currentState = gameStatus.next;
        }
    }
    public void CompileDict()
    {
        for(int i = 0; i < initialStrings.Count; i++)
        {
            initialValuesDict.Add(initialStrings[i], initialValues[i]);
        }
    }
    public void InitVariables()
    {
        foreach(KeyValuePair<string, int> keyValue in initialValuesDict)
        {
            switch (keyValue.Key)
            {
                case ("enemies"):
                    enemies = keyValue.Value;
                    break;
                case ("escapedTotal"):
                    escapedTotal = keyValue.Value;
                    break;
                case ("money"):
                    money = keyValue.Value;
                    break;
                case ("hardestPossibleEnemy"):
                    hardestPossibleEnemy = keyValue.Value;
                    break;
            }
        }
    }
    public void PlayButtonPressed()
    {
        RemoveAllEnemies();
        killed = 0;
        escaped = 0;
        switch (currentState)
        {
            case gameStatus.next:
                waveNumber += 1;
                enemies += waveNumber;
                break;
            case gameStatus.gameover:
                InitVariables();
                moneyLabel.text = money.ToString();
                escapedTotalLabel.text = "Escaped " + escapedTotal.ToString() + " / " + 10;
                TowerManager.instance.ClearTowerList();
                TowerManager.instance.ClearTowerPlacements();
                audioSource.PlayOneShot(SoundManager.instance.newGame);
                break;
        }
        waveCurrent.text = "Wave " + (waveNumber + 1);
        StartCoroutine(Spawn());
        buttonPlay.gameObject.SetActive(false);
        currentState = gameStatus.play;
    }
    public void AddMoney(int amount) { money += amount; moneyLabel.text = money.ToString(); }
    public void SubtractMoney(int amount) { money -= amount; moneyLabel.text = money.ToString(); }
    public void ShowButton()
    {
        switch(currentState)
        {
            case gameStatus.gameover:
                buttonPlayLabel.text = "Play again!";
                audioSource.PlayOneShot(SoundManager.instance.gameOver);
                break;
            case gameStatus.next:
                buttonPlayLabel.text = "Next wave";
                break;
            case gameStatus.play:
                break;
            case gameStatus.win:
                buttonPlayLabel.text = "You won!";
                break;
            case gameStatus.init:
                buttonPlayLabel.text = "Start Game";
                break;
        }
        buttonPlay.gameObject.SetActive(true);
    }
    public void EscapeHandler()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TowerManager.instance.DisableDrag();
            TowerManager.instance.towerBtnPressed = null;
        }
    }
}
