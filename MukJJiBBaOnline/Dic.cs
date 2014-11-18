using UnityEngine;
using System.Collections;
using LitJson;

public class Dic {
	public static string[] tips = new string[]{
		"수비시 상대의 손이 흔들리면 버튼을 누르세요.",
		"묵찌빠는 운이 아닙니다. 과학입니다.",
		"남자는 주먹!",
		"때로는 반복되는 공격이 효과적입니다.",
		"공격권을 가진 쪽은 주먹이 불타오릅니다.",
		"렐리가 길어질수록 많은 피해를 입힙니다.",
		"공격은 3초 안에",
		"엇박자로 상대를 교란하세요.",
		"상대의 패턴을 읽으세요.",
		"타임오버는 패배의 지름길",
		"AI 이기기가 더 힘들어요ㅠㅠ"
	};
	public static JsonData json;

	public static string myName;
	public static int myLevel;
	public static int win;
	public static int lose;
	public static int win_com;
	public static int lose_com;
	public static int win_frd;
	public static int lose_frd;

	public static string oppName;
	public static int oppLevel;

	
	public static bool isFirst;
}
