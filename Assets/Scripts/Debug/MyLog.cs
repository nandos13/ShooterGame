using UnityEngine;
using System.Collections;

public class MyLog : MBAction {

	public string output = "";

	public override void Execute()
	{
		Debug.Log(output);
	}
}
