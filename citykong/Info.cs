using UnityEngine;
using System.Collections;

public class Info {
	public static GameObject[] characters = new GameObject[4];
	public static Vector2[] bricks;
	public static int[] brickIndex;
	public static int[] fireLoc;
	public static int[] waterLoc;
	public static int[] windLoc;

	public static string myName;
	public static string sessionid;
	public static int myTurn;

	public static string invitationId;
	public static int numOfPlayers;

	public static ArrayList playerIdList = new ArrayList();

	public static bool soundOn = true;

	public static string[] avatarSet;
	public static string[] items;
	public static long score;
	public static long spent;
}
