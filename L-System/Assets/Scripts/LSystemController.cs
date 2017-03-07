using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Microsoft.CSharp;
[System.Serializable]

/*
Instructions
Assignment 3: L-Systems

10% of course total for both graduates and undergraduates.

Part I (50%)

Implement the Pythagoras tree L-System as described here:
https://en.wikipedia.org/wiki/L-system#Example_2:_Pythagoras_tree

variables : a, b
constants: [, ]
axiom  : a
rules  : (b → bb), (a → b[a]a)

0: draw a line segment ending in a leaf
1: draw a line segment
[: push position and angle, turn left 45 degrees
]: pop position and angle, turn right 45 degrees

Part II (30%)

Implement a visualization of the generated tree
 
Part III (20%)

Turn this L-system into a stochastic one (add some variations to the length and direction of the branches, etc.)

*/

struct stoch_rules {
	public float probability;
	public string result;

}

public class LSystemController : MonoBehaviour {
	/* This Program allows the user to define a set of rules and produce a basic L-System from those rules
	 * 
	 * 
	 * 
	 * Character definitions: 	[] - branch
	 * 							letters - lines with colour c and length l and width w, if l = 0 the line is not drawn
	 * 							symbols - angles/turn
	 * 							
	*/
	/*
	To do: 
		finish the string operations 
		create the line drawer 
		add stochasticity
	*/

	//public List<string> rule_in = {""};
	//public List<string> rule_out = {""};

	//stack that has position and angle
	public Text display;
	public Text starter;
	public int iterations;
	public Dropdown dropdown_rules_in;
	public Dropdown dropdown_variables_in;
	public Dropdown dropdown_symbols_in;
	public Dropdown dropdown_recursions;
	public InputField inputfield_rules_out;
	public InputField inputfield_variables_length;
	public InputField inputfield_variables_width;
	public InputField inputfield_symbol_angle;

	private Mesh mesh;

	private string[] rules_default = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p",
		"q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", 
		"L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
	private string[] symbols_default = {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "!", "@", "#", "+",
		"$", "%", "^", "&", "*", "(", ")"};
	private string[] rules_in = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p",
		"q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", 
		"L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
	private string[] rules_out;
	//private int RelationIndex;

	//private int VariableIndex;
	private Color[] colours;
	private float[] width;
	private float[] length;
	private float[][] weight; // need an uneven array

	//private int SymbolIndex;
	private float[] angle;

	/// Name: Start()
	/// Initializes all of the needed gui options
	void Start() {

		dropdown_rules_in.options.Clear ();
		dropdown_variables_in.options.Clear ();
		dropdown_symbols_in.options.Clear ();

		for (int i=0; i<rules_default.Length;++i) {
			dropdown_rules_in.options.Add (new Dropdown.OptionData(){text=rules_default[i]});
		}
		for (int i=0; i<rules_default.Length;++i) {
			dropdown_variables_in.options.Add (new Dropdown.OptionData(){text=rules_default[i]});
		}
		for (int i=0; i<symbols_default.Length;++i) {
			dropdown_symbols_in.options.Add (new Dropdown.OptionData(){text=symbols_default[i]});
		}
		for (int i=0; i<11;++i) {
			dropdown_recursions.options.Add (new Dropdown.OptionData(){text=i.ToString()});
		}

		//SymbolIndex = 0;
		//RelationIndex = 0;
		//VariableIndex = 0;
		rules_out = new string[rules_default.Length];
		colours = new Color[rules_default.Length];
		length = new float[rules_default.Length];
		width = new float[rules_default.Length];

		for (int i=0; i<rules_default.Length;++i) {
			rules_out [i] = rules_default [i];
			colours[i] = new Color(0.0f,0.0f,0.0f,1.0f);
			length [i] = 1.0f;
			width [i] = 0.1f;
		}

		angle = new float[20];
		for (int i=0;i<20;i++) {
			angle [i] = 0.0f;
		}

	}


	string String_Replace(string input){
		string output = "";
		for (int i = 0; i < input.Length; ++i) {
			for (int j = 0; j < rules_in.Length; ++j) {
				string temp = input[i].ToString();
				if (string.Equals(temp, rules_in [j])) {
					output += rules_out [j];
				} else if (j == rules_in.Length -1 ) {
					output+= input[i];
				}
			}
		}
		return output;
	}

	string LSystemGo() {
		string midstep = starter.text;
		for (int i = 0; i < dropdown_recursions.value; ++i) {
			midstep = String_Replace (midstep);
		}
		display.text = midstep;
		return midstep;
	}
		
	public void BuildTree(string input){
		Destroy (mesh);
		mesh = new Mesh ();
		// code for mesh construction from grammar here
	}

	// gui helper functions


	public void MakeString(){
		LSystemGo ();
	}

	public void MakeImage(){
		string final = LSystemGo ();
		BuildTree (final);
	}


	public void OnRelationChanged(){
		//rules_out[dropdown_rules_in.value];
	}

	public void OnRelationStringChanged(string rel){
		rules_out[dropdown_rules_in.value] = float.Parse(rel);
	}


	public void OnSymbolChanged(){
		//angle[dropdown_symbols_in.value] = sym;
	}

	public void OnAngleChanged(string sym){
		angle[dropdown_symbols_in.value] = float.Parse(sym);
	}


	public void OnVariableChanged(){
		//[dropdown_symbols_in.value];
		//[dropdown_symbols_in.value];
	}

	public void OnWidthChanged(string wid){
		width[dropdown_variables_in.value] = float.Parse(wid);
	}

	public void OnLenthChanged(string len){
		length[dropdown_variables_in.value] = float.Parse(len);
	}
		
	public void OnRedChanged(float c) {
		colours [dropdown_variables_in.value].r = c;
	}

	public void OnBlueChanged(float c) {
		colours [dropdown_variables_in.value].b = c;		
	}

	public void OnGreenChanged(float c) {
		colours [dropdown_variables_in.value].g = c;
	}
}
