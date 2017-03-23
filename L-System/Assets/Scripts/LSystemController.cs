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

45 = 0.7854
90 = 1.571
{
use b = bb
	a = b[+a]-a
	! = 1.571
+ = 0.7854
- = -0.7854
! = 1.571
axiom = !a
}
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
		add stochasticity
		adjust lengths a -> aa has equal lengths
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
	private float[] final_ratios;
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
		final_ratios = new float[1];
		final_ratios [0] = 1.0f;
		rules_out = new string[rules_default.Length];
		colours = new Color[rules_default.Length];
		length = new float[rules_default.Length];
		width = new float[rules_default.Length];
		for (int i=0; i<rules_default.Length;++i) {
			rules_out [i] = rules_default [i];
			colours[i] = new Color(1.0f,1.0f,1.0f,1.0f);
			length [i] = 3.0f;
			width [i] = 0.1f;
		}
		angle = new float[20];
		for (int i=0;i<20;i++) {
			angle [i] = 0.0f;
		}
	}

	public void MakeRatios (int position)
	{
		float finish = 0.0f;
		float ang = 0.0f;
		string rule = rules_out [position];
		for (int i = 0; i < rule.Length; ++i) {
			for (int j = 0; j < rules_out.Length; ++j) {
				if (string.Equals(rules_out[j], rule[i])){
					finish += (float)Math.Cos(ang)*length[j];
				}
			}
			for (int j = 0; j < symbols_default.Length; ++j) {
				if (string.Equals(symbols_default[j], rule[i] )) {
					ang+=angle[j];
				}
			}
		}
		if (finish > 0) {
			rules_ratios [position] = length [position] / finish;
		} else {
			rules_ratios[position] = 1.0f;
		}
	}

	string String_Replace(string input){
		string output = "";
		for (int i = 0; i < input.Length; ++i) {
			for (int j = 0; j < rules_in.Length; ++j) {
				string temp = input[i].ToString();
				if (string.Equals(temp, rules_in [j])) {
					output += rules_out [j];
					break;
				} else if (j == (rules_in.Length-1)) {
					output+= temp;
				}
			}

		}
		//float[] temp_array = new float[output.Length]; 
		//int temp_place = 0;

		//
		//for (int i = 0; i < input.Length; ++i) {
		//	string temp = input[i].ToString();
		//	for (int j = 0; j < rules_in.Length; ++j) {
		//		if (string.Equals(temp, rules_in [j])) {
		//			int size = rules_out [j].Length;
		//			//float temp_ratio = rules_ratios[j] * final_ratios[i];
		//			float temp_ratio = rules_ratios[j];
		//			for (int k = 0; k < size; ++k) {
		//				temp_array[temp_place] = temp_ratio;
		//				temp_place++;
		//			}
		//		} else {
		//			temp_array[temp_place] = 1.0f;
		//		}
		//	}
		//}
		//
		//final_ratios = temp_array;
		return output;
	}

	public string LSystemGo() {
		string midstep = starter.text;
		for (int i = 0; i < dropdown_recursions.value; ++i) {
			midstep = String_Replace (midstep);
		}
		display.text = midstep;
		return midstep;
	}

	public void BuildTree(string input){
		for (int i=0;i<rules_in.Length;++i) {
			//MakeRatios (i);
		}
		string tree = LSystemGo (); // add ratios to this method
		Destroy (mesh);
		mesh_color.Clear ();
		mesh_normals.Clear ();
		mesh_triangles.Clear ();
		mesh_points.Clear ();
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
				Debug.Log ("Popped: " + turtle_place.ToString());
				Debug.Log (turtle_direction);
			} else {
				for (int j=0;j<rules_in.Length;++j) {
					if (string.Equals(temp_tree, rules_in[j] )) {
						DrawSquare (turtle_place, turtle_direction, j, i);
						turtle_place = new Vector3( turtle_place.x + (length[j]*(float)Math.Cos(turtle_direction)), 
							turtle_place.y +(length[j]*(float)Math.Sin(turtle_direction)), turtle_place.z);
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



	public void DrawSquare(Vector3 position, float direction, int variable, int stringPlace){
		float w = width[variable]/2;
		Vector3 P1 = new Vector3( length[variable], w, 0); //need to adjust once ratios is implemented
		Vector3 P2 = new Vector3( length[variable], -w, 0);
		Vector3 P3 = new Vector3( 0, w, 0);
		Vector3 P4 = new Vector3( 0, -w, 0);

		// use rotation matrix to adjust points
		// x = px cos a - py sin a + tx
		// y = px sin a + py cos a + ty
		float cos_angle = (float)Math.Cos(direction) ;
		float sin_angle = (float)Math.Sin(direction);
		P1 = new Vector3((P1.x * cos_angle - P1.y * sin_angle + position.x), 
			(P1.x * sin_angle + P1.y * cos_angle + position.y), 0);
		P2 = new Vector3((P2.x * cos_angle - P2.y * sin_angle + position.x), 
			(P2.x * sin_angle + P2.y * cos_angle + position.y), 0);
		P3 = new Vector3((P3.x * cos_angle - P3.y * sin_angle + position.x), 
			(P3.x * sin_angle + P3.y * cos_angle + position.y), 0);
		P4 = new Vector3((P4.x * cos_angle - P4.y * sin_angle + position.x), 
			(P4.x * sin_angle + P4.y * cos_angle + position.y), 0);		
		//P1 = new Vector3((P1.x * cos_angle - P1.y * sin_angle + position.x)*final_ratios[stringPlace], 
		//		(P1.x * sin_angle + P1.y * cos_angle + position.y)*final_ratios[stringPlace], 0);
		//P2 = new Vector3((P2.x * cos_angle - P2.y * sin_angle + position.x)*final_ratios[stringPlace], 
	//		(P2.x * sin_angle + P2.y * cos_angle + position.y)*final_ratios[stringPlace], 0);
	//	P3 = new Vector3((P3.x * cos_angle - P3.y * sin_angle + position.x)*final_ratios[stringPlace], 
	//		(P3.x * sin_angle + P3.y * cos_angle + position.y)*final_ratios[stringPlace], 0);
	//	P4 = new Vector3((P4.x * cos_angle - P4.y * sin_angle + position.x)*final_ratios[stringPlace], 
	//		(P4.x * sin_angle + P4.y * cos_angle + position.y)*final_ratios[stringPlace], 0);
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
		mesh_color.Add(new Color(colours[variable].r,colours[variable].g,colours[variable].b,1));
		mesh_color.Add(new Color(colours[variable].r,colours[variable].g,colours[variable].b,1));
		mesh_color.Add(new Color(colours[variable].r,colours[variable].g,colours[variable].b,1));
		mesh_color.Add(new Color(colours[variable].r,colours[variable].g,colours[variable].b,1));
		}


	//
	// gui helper functions
	//

	public void MakeString(){
		LSystemGo ();
	}

	public void MakeImage(){
		string final = LSystemGo ();
		BuildTree (final);
	}

	public void OnRelationChanged(){
		inputfield_rules_out.text = rules_out[dropdown_rules_in.value];
	}

	public void OnSymbolChanged(){
		inputfield_symbol_angle.text = angle[dropdown_symbols_in.value].ToString();
	}

	public void OnVariableChanged(){
		inputfield_variables_width.text = width[dropdown_variables_in.value].ToString();
		inputfield_variables_length.text = length[dropdown_variables_in.value].ToString();
	}

	public void OnRelationStringChanged(){
		rules_out[dropdown_rules_in.value] = inputfield_rules_out.text;
	}

	//
	public void OnAngleChanged(){
		float temp = 0;
		bool isSucessful = float.TryParse(inputfield_symbol_angle.text, out temp);
		if (isSucessful) {
			angle[dropdown_symbols_in.value] = temp;
		}
	}

	public void OnWidthChanged(){
		float temp = 0;
		bool isSucessful = float.TryParse(inputfield_variables_width.text, out temp);

		if (isSucessful) {
			width[dropdown_variables_in.value] = temp;
		}
	}

	public void OnLenthChanged(){
		float temp = 0;
		bool isSucessful = float.TryParse(inputfield_variables_length.text, out temp);

		if (isSucessful) {
			length[dropdown_variables_in.value] = temp;
		}
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
