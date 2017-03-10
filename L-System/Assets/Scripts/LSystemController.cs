using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
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


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

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
		create the line drawer 
		add stochasticity
		adjust lengths a -> aa has equal lengths
		Fix problems with colours
	*/
		struct stoch_rules {
		public float probability;
		public string result;

	}

	struct turt_pos {
		public Vector3 vec;
		public float ang;
	};

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

	private MeshFilter mf;
	private MeshRenderer mr;
	private Mesh mesh;

	private List<Vector3> mesh_points;
	private List<Vector3> mesh_normals;
	private List<int> mesh_triangles;
	private List<Color> mesh_color;

	private string[] rules_default = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p",
		"q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", 
		"L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
	private string[] symbols_default = { "+", "-","!", "@", "#", "$", "%", "^", "&", "*", "(", ")"};
	private string[] rules_in = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p",
		"q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", 
		"L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
	private float[] rules_ratios;
	private string[] rules_out;
	private Color[] colours;
	private float[] width;
	private float[] length;
	private float[][] weight; // need an uneven array
	private float[] angle;

	/// Name: Start()
	/// Initializes all of the needed gui options
	void Start() {

		mf = GetComponent<MeshFilter> ();
		if (mf == null) {
			Debug.Log ("No meshfilter");
		} 
		mr = GetComponent<MeshRenderer> ();
		if (mf == null) {
			Debug.Log ("No meshrenderer");
		}

		mesh_points = new List<Vector3> ();
		mesh_normals = new List<Vector3>();
		mesh_triangles = new List<int> ();
		mesh_color = new List<Color> ();
			
		dropdown_rules_in.options.Clear ();
		dropdown_variables_in.options.Clear ();
		dropdown_symbols_in.options.Clear ();
		dropdown_recursions.options.Clear ();
		rules_ratios = new float[rules_default.Length];
		for (int i=0; i<rules_default.Length;++i) {
			dropdown_rules_in.options.Add (new Dropdown.OptionData(){text=rules_default[i]});
			rules_ratios [i] = 1;
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
			
		rules_out = new string[rules_default.Length];
		colours = new Color[rules_default.Length];
		length = new float[rules_default.Length];
		width = new float[rules_default.Length];
		for (int i=0; i<rules_default.Length;++i) {
			rules_out [i] = rules_default [i];
			colours[i] = new Color(0.0f,0.0f,0.0f,1.0f);
			length [i] = 3.0f;
			width [i] = 0.1f;
		}
		angle = new float[20];
		for (int i=0;i<20;i++) {
			angle [i] = 0.0f;
		}
	}

	public void MakeRatios(int position) {
		Vector3 finish = new Vector3(0,0,0);
		float angle = 0;
		int branch = 0;


		for (int i=0;i< rules_out[position].Length; ++i) {
			if (string.Equals(rules_out[position][i], "[")) {
				branch++;
			}
			if (string.Equals(rules_out[position][i], "]")) {
				branch--;
			}

			if (branch < 1) {
				for (int j = 0; j < rules_in.Length; ++j) {
					//string temp = input[i].ToString();
					//if (string.Equals(temp, rules_in [j])) {
					//	output += rules_out [j];
					//} else if (j == rules_in.Length -1 ) {
					//	output+= input[i];
					//}


					//if (true/*line*/) {

					//} else if (false/*angle*/) {

					//} else if (false/*push to pop*/) {


					//}
				}

				for (int j = 0; j < symbols_default.Length; ++j) {
					//string temp = input[i].ToString();
					//if (string.Equals(temp, rules_in [j])) {
					//	output += rules_out [j];
					//} else if (j == rules_in.Length -1 ) {
					//	output+= input[i];
					//}


					//if (true/*line*/) {

					//} else if (false/*angle*/) {

					//} else if (false/*push to pop*/) {


					//}
				}
			}
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
		for (int i=0;i<rules_in.Length;++i) {
			MakeRatios (i);
		}
		string tree = LSystemGo ();
		Destroy (mesh);
		mesh = new Mesh ();

		float turtle_direction = 0;
		Vector3 turtle_place = new Vector3 (0,0,0);
		//stack that has position and angle
		Stack<turt_pos> turtle_stack = new Stack<turt_pos>();

		// code for mesh construction from grammar here
		for (int i = 0; i < tree.Length; ++i) {
			string temp_tree = tree[i].ToString();
			if (string.Equals(temp_tree, "[" )) {
				turt_pos temp = new turt_pos();
				temp.ang = turtle_direction;
				temp.vec = turtle_place;
				turtle_stack.Push(temp);
			} else if (string.Equals(temp_tree, "]" )) { 
				turt_pos temp = turtle_stack.Pop();
				turtle_direction = temp.ang;
				turtle_place = temp.vec;
			} else {
				for (int j=0;j<rules_in.Length;++j) {
					if (string.Equals(temp_tree, rules_in[j] )) {
						DrawSquare (turtle_place, turtle_direction, j);
						break;
					}
				}
				for (int j=0;j<symbols_default.Length;++j) {
					if (string.Equals(temp_tree, symbols_default[j] )) {
						turtle_direction += angle[j];
						break;
					}
				}
			}
		}

		Vector3[] mvert = new Vector3[mesh_points.Count];
		mesh_points.CopyTo (mvert);
		mesh.vertices = mvert;
		Vector3[] mnorms = new Vector3[mesh_normals.Count];
		mesh_normals.CopyTo(mnorms);
		mesh.vertices = mnorms;
		int[] mint = new int[mesh_triangles.Count];
		mesh_triangles.CopyTo(mint);
		mesh.triangles = mint;
		Color[] mcol = new Color[mesh_color.Count];
		mesh_color.CopyTo(mcol);
		mesh.colors = mcol;

		mesh = new Mesh ();
		mesh.vertices = mvert;
		mesh.normals = mnorms;
		mesh.triangles = mint;
		mesh.colors = mcol;

		mf.mesh = mesh;
	}



	public void DrawSquare(Vector3 position, float direction, int variable){
		Debug.Log ("DrawSquare called");
		float w = width[variable]/2;
		Vector3 P1 = new Vector3( -w, length[variable], 0); //need to adjust once ratios is implemented
		Vector3 P2 = new Vector3( w, length[variable], 0);
		Vector3 P3 = new Vector3( -w, 0, 0);
		Vector3 P4 = new Vector3( w, 0, 0);

		// use rotation matrix to adjust points
		// x = px cos a - py sin a + tx
		// y = px sin a + py cos a + ty
		float cos_angle = (float)Math.Cos(direction);
		float sin_angle = (float)Math.Sin(direction);
		P1 = new Vector3(P1.x * cos_angle - P1.y * sin_angle + position.x, 
							P1.x * sin_angle + P1.y * cos_angle + position.y, 0);
		P2 = new Vector3(P2.x * cos_angle - P2.y * sin_angle + position.x, 
							P2.x * sin_angle + P2.y * cos_angle + position.y, 0);
		P3 = new Vector3(P3.x * cos_angle - P3.y * sin_angle + position.x, 
							P3.x * sin_angle + P3.y * cos_angle + position.y, 0);
		P4 = new Vector3(P4.x * cos_angle - P4.y * sin_angle + position.x, 
							P4.x * sin_angle + P4.y * cos_angle + position.y, 0);
		//add vertices to mesh
		mesh_points.Add(P1);
		mesh_points.Add(P2);
		mesh_points.Add(P3);
		mesh_points.Add(P4);

		int temp_tr = 0;
		if (mesh_triangles.Count>0) {
			temp_tr = mesh_triangles[mesh_triangles.Count-1]+1;
		}

		//add triangles
		mesh_triangles.Add(temp_tr);
		mesh_triangles.Add(temp_tr+1);
		mesh_triangles.Add(temp_tr+2);
		mesh_triangles.Add(temp_tr+2);
		mesh_triangles.Add(temp_tr+1);
		mesh_triangles.Add(temp_tr+3);

		//add normals
		mesh_normals.Add(new Vector3(0,0,-1));
		mesh_normals.Add(new Vector3(0,0,-1));
		mesh_normals.Add(new Vector3(0,0,-1));
		mesh_normals.Add(new Vector3(0,0,-1));

		//add colors
		//mesh_color.Add(new Color(colours[variable].r,colours[variable].g,colours[variable].b,1));
		//mesh_color.Add(new Color(colours[variable].r,colours[variable].g,colours[variable].b,1));
		//mesh_color.Add(new Color(colours[variable].r,colours[variable].g,colours[variable].b,1));
		//mesh_color.Add(new Color(colours[variable].r,colours[variable].g,colours[variable].b,1));

		mesh_color.Add(new Color(0,0,0,1));
		mesh_color.Add(new Color(0,0,0,1));
		mesh_color.Add(new Color(0,0,0,1));
		mesh_color.Add(new Color(0,0,0,1));
		}

	// gui helper functions

	public void MakeString(){
		LSystemGo ();
	}

	public void MakeImage(){
		string final = LSystemGo ();
		BuildTree (final);
	}

	public void OnRelationStringChanged(){
		rules_out[dropdown_rules_in.value] = inputfield_rules_out.text;
	}

	public void OnRelationChanged(){
		inputfield_rules_out.text = rules_out[dropdown_rules_in.value];
	}

	public void OnSymbolChanged(){
		inputfield_symbol_angle.text = angle[dropdown_symbols_in.value].ToString();
	}

	public void OnAngleChanged(){
		angle[dropdown_symbols_in.value] = float.Parse(inputfield_symbol_angle.text);
	}

	public void OnVariableChanged(){
		inputfield_variables_width.text = width[dropdown_symbols_in.value].ToString();
		inputfield_variables_length.text = length[dropdown_symbols_in.value].ToString();
	}

	public void OnWidthChanged(){
		width[dropdown_variables_in.value] = float.Parse(inputfield_variables_width.text);
	}

	public void OnLenthChanged(){
		length[dropdown_variables_in.value] = float.Parse(inputfield_variables_length.text);
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
