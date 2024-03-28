using System;
using TrelloNet;
using UnityEngine;

public class TrelloIntegration : MonoBehaviour
{
	private const string ApplicationKey = "5d6a4a7280e7a39e25739aa469a943e8";
	private const string Token = "ATTAcef2eafb5ba09ee4abbd112b32d150cbef7df1922c25d871642e765ed18e27f70E6F46A9";
	private const string BoardId = "65c1208c2dc8c7f9aef4bfda";
	private ITrello trello;

	public void Start()
	{
		// Initialize Trello with your application key
		try
		{
			trello = new Trello(ApplicationKey);
			Debug.Log("Trello initialized successfully.");
		}
		catch (System.Exception ex)
		{
			Debug.LogError("Failed to initialize Trello: " + ex.Message);
			return;
		}

		// Authorize with your token
		try
		{
			trello.Authorize(Token);
			Debug.Log("Trello authorized successfully.");
		}
		catch (System.Exception ex)
		{
			Debug.LogError("Failed to authorize Trello: " + ex.Message);
			return;
		}

		// Retrieve the board with the specified ID
		Board board = null;
		try
		{
			board = trello.Boards.WithId(BoardId);
			Debug.Log("Board retrieved successfully: " + (board != null ? board.Name : "null"));
		}
		catch (System.Exception ex)
		{
			Debug.LogError("Failed to retrieve board: " + ex.Message);
			return;
		}
		if (board == null)
			return;

		Console.WriteLine("Board Name...");
		Console.WriteLine(board.Name);

		Console.WriteLine("Board Lists...");
		foreach (var list in trello.Lists.ForBoard(board))
			Console.WriteLine(list.Name);
	}
}