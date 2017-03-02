using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Microsoft.CSharp;

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

	string String_Replace(string input){
		string output = "";
		Debug.Log (input.Length);

		for (int i = 0; i < input.Length; ++i) {
			for (int j = 0; j < rules_in.Length; ++j) {
				string temp = input[i].ToString();
				if (string.Equals(temp, rules_in [j])) {
					output += rules_out [j];
				} else if (j == rules_in.Length -1 ) {
					output+= input[i];
				}
				//Debug.Log (output);
			}
		}
		return output;
	}

	string LSystemGo() {
		string midstep = starter.text;
		for (int i = 0; i < iterations; ++i) {
			midstep = String_Replace (midstep);
		}
		display.text = midstep;
		return midstep;
	}

	public void MakeString(){
		LSystemGo ();
	}

	public void MakeImage(){
	
	
	}

}
