using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSystemController : MonoBehaviour {

	/* Character definitions: 	[] - branch
	 * 							letters - lines with colour c
	 * 							symbols - angles
	*/


	//public List<string> rule_in = {""};
	//public List<string> rule_out = {""};
	public Text display;
	public Text starter;
	public int iterations;

	private string[] rules_in = {"a", "b"};
	private string[] rules_out = {"b", "aa"};



	// Use this for initialization
	void Start () {
		starter.text = "a";
		LSystemGo (iterations);
	}

	void String_Replace(string input, string output){
		output = "";
		for (int i = 0; i < input.Length; ++i) {
			for (int j = 0; j < rules_in.Length; ++j) {
				//if (input [i] == rules_in [j]) {
					output += rules_out [j];
				//}
			}
		}
		return;
	}

	void LSystemGo(int iterations) {
		string produced = "";
		string midstep = starter.text;
		for (int i = 0; i < iterations; ++i) {
			String_Replace (midstep, produced);
			midstep = produced;
		}
		display.text = produced;
	}

}
