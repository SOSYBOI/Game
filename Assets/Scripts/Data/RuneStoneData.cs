using Firebase.Firestore;

[FirestoreData]
public class RuneStoneData
{
    [FirestoreProperty] public string content { get; set; }  // 符文石上的留言
    [FirestoreProperty] public string chunk_id { get; set; } // 所属区块 ID（用于快速查询）
    [FirestoreProperty] public float x { get; set; }         // Unity 世界坐标 X
    [FirestoreProperty] public float y { get; set; }         // Unity 世界坐标 Y
    [FirestoreProperty] public float z { get; set; }         // Unity 世界坐标 Z
    [FirestoreProperty] public int likes { get; set; }       // 点赞数
    [FirestoreProperty] public Timestamp timestamp { get; set; } 
}