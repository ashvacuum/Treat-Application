using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;

namespace Firebase
{
    public class FirestoreService : IFirestoreService
    {
        private FirebaseFirestore _db;
        private ListenerRegistration _listenerRegistration;
        private const string DB_NAME = "Leaderboards";
        
        public async Task<bool> Initialize()
        {
            try
            {
                var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _db = FirebaseFirestore.DefaultInstance;
                    Debug.Log("Firebase initialized successfully!");
                    return true;
                }
                Debug.LogError($"Could not resolve dependencies: {dependencyStatus}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Firebase initialization failed: {e.Message}");
                return false;
            }
        }

        public async Task SavePlayerScore(string playerId, string playerName, int score)
        {
            try
            {
                var docRef = _db.Collection(DB_NAME).Document(playerId);
                var playerData = new Dictionary<string, object>
                {
                    { "name", playerName },
                    { "score", score },
                    { "lastUpdated", Timestamp.GetCurrentTimestamp() }
                };

                await docRef.SetAsync(playerData);
                Debug.Log("Player data saved successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving player data: {e.Message}");
                throw;
            }
        }

        public async Task<Dictionary<string, object>> GetPlayerData(string playerId)
        {
            try
            {
                var docRef = _db.Collection(DB_NAME).Document(playerId);
                var snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    var playerData = snapshot.ToDictionary();
                    Debug.Log($"Retrieved player data for ID: {playerId}");
                    return playerData;
                }
                Debug.Log($"No player data found for ID: {playerId}");
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading player data: {e.Message}");
                throw;
            }
        }

        public async Task UpdatePlayerScore(string playerId, int newScore)
        {
            try
            {
                DocumentReference docRef = _db.Collection(DB_NAME).Document(playerId);
                await docRef.UpdateAsync("score", newScore);
                Debug.Log("Player score updated successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error updating player score: {e.Message}");
                throw;
            }
        }


        public async Task<List<GameSessionData>> QueryHighScores(int minimumScore)
        {
            var GameSessionDataList = new List<GameSessionData>();
            try
            {
                Query highScoresQuery = _db.Collection(DB_NAME)
                    .WhereGreaterThan("score", minimumScore)
                    .OrderByDescending("score")
                    .Limit(10);

                QuerySnapshot querySnapshot = await highScoresQuery.GetSnapshotAsync();
                
                foreach (DocumentSnapshot document in querySnapshot.Documents)
                {
                    Dictionary<string, object> playerData = document.ToDictionary();
                    Debug.Log($"Player: {playerData["name"]}, Score: {playerData["score"]}");
                    
                    GameSessionDataList.Add(new GameSessionData()
                    {
                        playerId = (string)playerData["name"],
                        score = int.Parse((string)playerData["score"])
                    });
                }

                return GameSessionDataList;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error querying high scores: {e.Message}");
                throw;
            }
        }
    }

    public interface IFirestoreService
    {
        Task<bool> Initialize();
        Task SavePlayerScore(string playerId, string playerName, int score);
        Task<Dictionary<string, object>> GetPlayerData(string playerId);
        Task UpdatePlayerScore(string playerId, int newScore);
    }
}