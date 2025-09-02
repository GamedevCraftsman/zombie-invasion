using Firebase.Auth;
using Firebase.Firestore;
using Zenject;

public class DataManageService
{
    private LocalLvlDB _localLvlDB;
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
    public void InitialLoad(FirebaseUser user)
    {
        _user = user;

        SetUpDatabase();
        LoadData();
    }

    private void SetUpDatabase()
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
    }

    private void SetDefaultData()
    {
        _localLvlDB.LvlLenghtIncrease = _progressSettings.MapLenghtIncrease;
        _localLvlDB.LvlNumber = 1;
        _localLvlDB.EnemyCountIncrease = _progressSettings.EnemiesIncrease;
    }

    private void LoadData()
    {
    }

    private void SaveData()
    {
    }
}