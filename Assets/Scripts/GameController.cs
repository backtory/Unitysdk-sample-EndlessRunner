using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Assets.BacktorySDK.game;
using Assets.BacktorySDK.attributes;
using Assets.BacktorySDK.auth;

public class TopRunnersLeaderboard : BacktoryLeaderBoard
{
	[LeaderboardId]
	public const string Id = "57f119a0e4b0d83aa5be3404";
}

public class GameOverEvent : BacktoryGameEvent
{
	[EventName]
	public const string EventName = "GameOver";

	[FieldName ("score")]
	public int ScoreValue { get; set; }

	public GameOverEvent (int ScoreValue)
	{
		this.ScoreValue = ScoreValue;
	}
}

public class GameController : MonoBehaviour
{

	public GameObject player;
	public GameObject hazard;
	public Vector3 spawnValues;
	public float spawnWait;
	public Text scoreText;
	public Text rankText;

	private int count;
	private TopRunnersLeaderboard topRunnersLeaderboard;

	void Start ()
	{
		count = 0;
		topRunnersLeaderboard = new TopRunnersLeaderboard ();
		StartCoroutine (SpawnWaves ());

		topRunnersLeaderboard.GetPlayerRankInBackground (rankResponse => {
			if (rankResponse.Successful) {
				rankText.text = "Rank: " + rankResponse.Body.Rank +
				"; Score: " + rankResponse.Body.Scores [0];
			} else {
				rankText.text = "No rank yet!";
			}
		});

		topRunnersLeaderboard.GetTopPlayersInBackground (100, leaderboardResponse => {
			if (leaderboardResponse.Successful) {
//				UserProfile topPlayer = leaderboardResponse.Body.UsersProfile [0];
//				string username = topPlayer.UserBriefProfile.UserName;
//				List<Integer> scores = topPlayer.Scores; 
//
//				// Logging best player
//				Debug.Log ("best player is: " + username
//				+ " who is scoring: " + scores);
			} else {
				Debug.Log ("Strange! Get top players failed! code: " + leaderboardResponse.Code);
			}
		});
	}

	IEnumerator SpawnWaves ()
	{
		while (true) {
			yield return new WaitForSeconds (spawnWait);

			if (player == null) { // if destroyed
				new GameOverEvent (count).SendInBackground (backtoryResponse => {
					if (backtoryResponse.Successful) {
						Debug.Log ("Saved event successfully");
					} else {
						Debug.Log ("Strange! Game over event failed! code: " + backtoryResponse.Code);
					}
				});
				yield break;
			}
			
			IncrementScore ();
			Vector3 spawnPosition = new Vector3 (
				                        Random.Range (-spawnValues.x, spawnValues.x),
				                        spawnValues.y,
				                        spawnValues.z
			                        );
			Instantiate (hazard, spawnPosition, Quaternion.identity);
		}
	}

	void IncrementScore ()
	{
		count = count + 1;
		scoreText.text = "Score: " + count;
	}

	public void OnGoAgainButtonClicked ()
	{
		BacktoryUser.LogoutInBackground ();
		SceneManager.LoadScene ("Login");
	}
}
