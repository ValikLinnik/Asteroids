using Injection;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class DataStorageManager : InjectorBase<DataStorageManager> 
{
    #region PUBLIC PROPERTIES

    public string PlayerName
    {
        set
        {
            _name = string.IsNullOrEmpty(value) ? "Name" : value;
        }
    }

    public bool IsNameEntered
    {
        get
        {
            return !string.IsNullOrEmpty(_name);
        }
    }

    public GameConfig GameConfig
    {
        get
        {
            if(!_gameConfig) _gameConfig = GetConfigData();
            return _gameConfig;
        }
    }

    #endregion

    #region PRIVATE FIELDS

    private static DataStorageManager _instance;

    private string _path = "/SQLite/db/GameInfo.db";
    private string _name;
    private string _fullPath;
    private GameConfig _gameConfig;
    private string _stage = "SANDBOX";

    #endregion

    #region UNITY EVENTS

    protected override void Awake()
    {
        if (_instance == null) _instance = this;
        else
        {
            _instance.InjectManager();
            Destroy(gameObject);
            return;
        }

        base.Awake();
        DontDestroyOnLoad(this);
        _fullPath = string.Format("URI=file:{0}{1}", Application.dataPath, _path);
    }

    #endregion

    #region PRIVATE METHODS

    private void InjectManager()
    {
        base.Awake();
    }

    #endregion

    #region PUBLIC METHODS

    private GameConfig GetConfigData()
    {
        var command = @"SELECT * FROM GameConfig WHERE Stage = @StageValue";

        var result = new GameConfig();

        using (var dbConnection = new SqliteConnection(_fullPath))
        {
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbConnection.Open();
                dbCommand.CommandText = command;
                dbCommand.Parameters.Add("@StageValue", DbType.String).Value = _stage;

                using (var dbReader = dbCommand.ExecuteReader())
                {
                    while (dbReader.Read())
                    {
                        if (dbReader.GetValue(0) != null)
                        {
                            result.MenuSceneIndex = dbReader.GetInt32(0);
                            result.GameSceneIndex = dbReader.GetInt32(1);
                            result.DefaultScore = dbReader.GetString(2);
                            result.LifeStartValue = dbReader.GetInt32(3);
                            result.AsteroidTag = dbReader.GetString(4);
                            result.LaserTag = dbReader.GetString(5);
                            result.PlayerTag = dbReader.GetString(6);
                        }
                    }
                }
            }
        }

        return result;
    }

    public int GetBestScore()
    {
        var command = @"SELECT * FROM PlayersRecords WHERE Name = @Name";

        int result = 0;

        using (var dbConnection = new SqliteConnection(_fullPath))
        {
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbConnection.Open();
                dbCommand.CommandText = command;
                dbCommand.Parameters.Add("@Name", DbType.String).Value = _name;

                using (var dbReader = dbCommand.ExecuteReader())
                {
                    while (dbReader.Read())
                    {
                        if (dbReader.GetValue(0) != null)
                        {
                            result = dbReader.GetInt32(1);
                        }
                    }
                }
            }
        }

        return result;
    }

    public void SetBestScore(int score)
    {
        var command = @"UPDATE PlayersRecords SET BestScore = @ScoreValue WHERE Name = @dNameValue";

        using (var dbConnection = new SqliteConnection(_fullPath))
        {
            using (var dbCommand = dbConnection.CreateCommand())
            {
                dbConnection.Open();
                dbCommand.CommandText = command;
                dbCommand.Parameters.Add("@ScoreValue", DbType.Int32).Value = score;
                dbCommand.Parameters.Add("@dNameValue", DbType.String).Value = _name;

                int entries = dbCommand.ExecuteNonQuery();

                if (entries == 0)
                {
                    command = @"INSERT INTO PlayersRecords (Name, BestScore) VALUES (@dNameValue, @ScoreValue)";
                    dbCommand.CommandText = command;
                    dbCommand.ExecuteNonQuery();
                }
            }
        }
    }

    #endregion
}

public class GameConfig
{
    #region PUBLIC PROPERTIES

    public int MenuSceneIndex;
    public int GameSceneIndex;
    public string DefaultScore;
    public int LifeStartValue;
    public string AsteroidTag;
    public string LaserTag;
    public string PlayerTag;

    #endregion

    #region OVERRODE OPERATORS

    public static implicit operator bool(GameConfig obj)
    {
        return obj != null;   
    }

    #endregion
}