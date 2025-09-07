using Firebase.Firestore;

[FirestoreData]
public class LocalLvlDB
{
    [FirestoreProperty]
    public int LvlNumber { get; set; }
    [FirestoreProperty]
    public int EnemyCountIncrease { get; set; }
    [FirestoreProperty]
    public int LvlLenghtIncrease { get; set; }
}