using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;
using Zenject;

public class DataManageService
{
    private LocalLvlDB _localLvlDB = new ();
    private FirebaseFirestore _db;
    private FirebaseUser _user;

    #region Game Settings

    private readonly ProgressSettings _progressSettings;

    #endregion

    public LocalLvlDB LocalLvlDB => _localLvlDB;

    [Inject]
    public DataManageService(ProgressSettings progressSettings)
    {
        _progressSettings = progressSettings;

        SetDefaultData();
    }

    // Load in Firebase Manager
    public async Task InitialLoad(FirebaseUser user)
    {
        try
        {
            _user = user;

            SetUpDatabase();
            await LoadData();
        }
        catch (Exception e)
        {
            Debug.LogError($"InitialLoad Error: {e.Message}");
        }
    }

    private void SetUpDatabase()
    {
        _db = FirebaseFirestore.DefaultInstance;
        if (_db == null)
        {
            Debug.LogError($"InitialLoad Error: FirebaseFirestore.DefaultInstance is null");
        }
    }

    private void SetDefaultData()
    {
        _localLvlDB.LvlLenghtIncrease = _progressSettings.MapLenghtIncrease;
        _localLvlDB.LvlNumber = 1;
        _localLvlDB.EnemyCountIncrease = _progressSettings.EnemiesIncrease;
    }

    private async Task LoadData()
    {
        try
        {
            if (_user == null) return;
            
            DocumentReference docRef = _db.Collection("users").Document(_user.Email);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                _localLvlDB = snapshot.ConvertTo<LocalLvlDB>();
            }
            else
            {
                await SaveData();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Load data Error: {e.Message}");
        }
    }
    
    public async Task SaveData(int? lvl = null, int? enemyIncrease = null, int? lvlLengthIncrease = null)
    {
        try
        {
            DocumentReference docRef = _db.Collection("users").Document(_user.Email);
            
            if (lvl.HasValue) _localLvlDB.LvlNumber = lvl.Value;
            if (lvlLengthIncrease.HasValue) _localLvlDB.LvlLenghtIncrease = lvlLengthIncrease.Value;
            if (enemyIncrease.HasValue) _localLvlDB.EnemyCountIncrease = enemyIncrease.Value;

            await docRef.SetAsync(_localLvlDB);
        }
        catch (Exception e)
        {
            Debug.LogError($"Save data Error: {e.Message}");
        }
    }
}