using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using SimpleRandomGenerator;
[System.Serializable]

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]


public class LSystemController : MonoBehaviour {
	/* This Program allows the user to produce corals from an L-System
	 *
	 * 							
	*/
	private bool debug_progress = false;
	private bool debug_colours_branches = false;
	private bool debug_colours_base = false;
	private bool debug_BuildTree= false;
	private bool debug_StringReplace = false;
	private bool debug_PickOutputString = false;
	private bool debug_AdjustAngles = false;
	private bool debug_LSystemGo = false;
	private bool debug_RotatePoint = false;
	private bool debug_DrawSquare = false;
	private bool debug_MakeImage = false;

	private bool add_phototaxis = true;
	private bool invariant_flow_on = true;
	private bool flow_on = false;
	private bool add_wobble = true;

	private float too_far_down = 7.0f;
	private float max_mag = 10.0f;
	private float flow_constant = 0.030f;
	private float flow_constant_lerp = 0.020f;
	private float wobble_strength = 0.07f;
	private float light_constant = 0.03f;
	private float two_pi = 6.28318531f;
	int MAX_TRIANGLES = 65000;
	private int max_iterations = 30;

	struct stoch_rules {
		public float probability;
		public string result;
	}

	struct turt_pos {
		public Vector3 vec;
		public float angle_a;
		public float angle_b;
		public float age;
	};

	private Vector3 flow;
	private Vector3 test_flow = new Vector3(0.0f,0.0f,1.0f);
	private Vector3 light_direction = new Vector3 (0.0f, -1.0f, 0.0f);
	//public Text display;
	//private Text starter_string;
	private int current_coral;
	public Dropdown dropdown_coral_type;
	public Dropdown dropdown_recursions;

	private MeshFilter mf;
	private MeshRenderer mr;
	private Mesh mesh;
	private Perlin rng;
	private Perlin wobble_rng;


	private List<Vector3> mesh_points;
	private List<Vector3> mesh_normals;
	private List<Color> mesh_color;
	private List<int> mesh_triangles;

	private int seed_value = 42;
	private string[] corals_available = { "Staghorn"};
	private string[] coral_input_string = {"!a[@d]a"};

	private int base_rules_indexes = 2;// keep the base from being affected by flow
	private int base_symbols_indexes = 0;// keep the base from being affected by flow
	private string[] rules_in = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p",
		"q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
	// let a b and c be the main rules for base and d, e, f the rules for initial branch
	private string[,] rules_out = {{"ab", "ba"},/* a */ {"c[@d]b", "bc"},/* b */ {"c", "c"},/* c */ {"zzzzzzzzf", "zzzzzzzzzf"},/* d */
		{"zzzzze", "zzzzf"}, /* e */ {"zzzzg", "zzzzzzzg"},/* f */ {"zzzzz[+e][-e]e", "zzz[#e][$e]e"}, /* g */
		{"h", "h"}, /* h */ {"i", "i"}, /* i */
		{"j", "j"}, /* j */ {"k", "k"}, /* k */ {"l", "l"},/* l */ {"m", "m"},/* m */ {"n", "n"},/* n */ {"o", "o"},/* o */ 
		{"p", "p"},/* p */ {"q", "q"},/* q */ {"r", "r"},/* r */ {"s", "s"},/* s */ {"t", "t"}, /* t */ {"u", "u"},/* u */ 
		{"v", "v"},/* v */ {"w", "w"},/* w */ {"x", "x"},/* x */ {"y", "y"},/* y */ {"z", "z"}/* z */}; 
	private float[,] stochastic_values = {{0.5f, 1.0f}, {0.01f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f},
		{0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f},
		{0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f}, 
		{0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f}, {0.5f, 1.0f}};
	private float[] width_tip={0.05f, 0.05f, 0.05f, 0.05f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 
		0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f};
	private float[] width_base ={0.15f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 
		0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f};
	private float[] length = {0.03f, 0.03f, 0.03f, 1.3f, 1.3f, 1.3f, 1.3f, 1.3f, 1.3f, 1.3f, 1.1f, 1.1f, 1.1f, 
		0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f};
	private string[] symbols_default_start = { "@", "+", "-", "~", "#", "$", "%", "^", "&", "*", "(", ")", "!"};
	private float[] angle_a ={/* @ */ 0.0f,/* + */ -0.7854f,/* - */ 0.7854f,/* ~ */ 0.1f,/* # */ 0.1f,/* $ */ 0.2f,/* % */ 0.3f,
		/* ^ */ 0.4f,/* & */ 1.0f,/* * */ -1.0f,/* ( */ 0.0f,/* ) */ 0.0f, /* ! */ 0.0f};
	private float[] angle_b ={/* @ */1.571f,/* + */ 0.52f,/* - */ -1.571f,/* ~ */ 1.571f,/* # */ -1.571f,/* $ */ 1.571f,/* % */ 0.3f,
		/* ^ */ 0.4f,/* & */ 1.0f,/* * */ -1.0f,/* ( */ 0.0f,/* ) */ 0.0f,/* ! */ -1.571f};	
	private Color[] colours;
	//private Quaternion start_quaternion;

	/// Name: Start()
	/// Initializes all of the needed gui options
	void Start() {
		rng = new Perlin (seed_value);
		wobble_rng = new Perlin (seed_value);
		flow = new Vector3 (0,0,0);
		mf = GetComponent<MeshFilter> ();
		if (mf == null) {
			Debug.Log ("No meshfilter");
		} 
		mr = GetComponent<MeshRenderer> ();
		if (mf == null) {
			Debug.Log ("No meshrenderer");
		}
		//start_quaternion = new Quaternion (0,1,0,1);
		mesh_points = new List<Vector3> ();
		mesh_normals = new List<Vector3>();
		mesh_triangles = new List<int> ();
		mesh_color = new List<Color> ();
			
		dropdown_coral_type.options.Clear ();
		for (int i=0; i<corals_available.Length;++i) {
			dropdown_coral_type.options.Add (new Dropdown.OptionData(){text=corals_available[i]});
		}
		dropdown_recursions.options.Clear ();
		for (int i=0; i<max_iterations;++i) {
			dropdown_recursions.options.Add (new Dropdown.OptionData(){text=i.ToString()});
		}
		colours = new Color[rules_in.Length];
		colours[0] = new Color (0.88f, 0.88f, 0.34f, 1.0f); // a
		colours[1] = new Color (0.88f, 0.88f, 0.34f, 1.0f); // b  
		colours[2] = new Color (0.88f, 0.88f, 0.34f, 1.0f); // c 
		colours[3] = new Color (0.88f, 0.88f, 0.34f, 1.0f); // d  
		colours[4] = new Color (0.88f, 0.88f, 0.34f, 1.0f); // e  
		colours[5] = new Color (0.88f, 0.88f, 0.34f, 1.0f);; // f  
		colours[6] = new Color (0.88f, 0.88f, 0.34f, 1.0f);; // g 
		colours[7] = new Color (0.88f, 0.88f, 0.34f, 1.0f);; // h 
		colours[8] = new Color (0.88f, 0.88f, 0.34f, 1.0f);; // i 
		colours[9] = new Color (0.88f, 0.88f, 0.34f, 1.0f);; // j  
		colours[10] = new Color (0.88f, 0.88f, 0.34f, 1.0f);; // k  
		colours[11] = new Color (0.88f, 0.88f, 0.34f, 1.0f);; // l  
		colours[12] = new Color (0.88f, 0.88f, 0.34f, 1.0f);; // m  
		colours[13] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // n  
		colours[14] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // o  
		colours[15] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // p  
		colours[16] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // q  
		colours[17] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // r 
		colours[18] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // s  
		colours[19] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // t 
		colours[20] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // u 
		colours[21] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // v 
		colours[22] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // w  
		colours[23] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // x  
		colours[24] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // y  
		colours[25] = new Color (0.88f, 0.88f, 0.34f, 1.0f);; // z 

		if (debug_colours_branches){
			colours[0] = new Color (1.0f, 0.0f, 0.0f, 1.0f); // a
			colours[1] = new Color (1.0f, 0.0f, 0.0f, 1.0f); // b  
			colours[2] = new Color (1.0f, 0.0f, 0.0f, 1.0f); // c 
			colours[3] = new Color (0.0f, 0.2f, 0.2f, 1.0f); // d  
			colours[4] = new Color (0.0f, 0.4f, 0.4f, 1.0f); // e  
			colours[5] = new Color (0.0f, 0.6f, 0.6f, 1.0f); // f  
			colours[6] = new Color (0.0f, 0.8f, 0.8f, 1.0f); // g 
			colours[7] = new Color (0.0f, 1.0f, 1.0f, 1.0f); // h 
			colours[8] = new Color (0.0f, 0.2f, 0.0f, 1.0f); // i 
			colours[9] = new Color (0.0f, 0.4f, 0.0f, 1.0f); // j  
			colours[10] = new Color (0.0f, 0.6f, 0.0f, 1.0f); // k  
			colours[11] = new Color (0.0f, 0.8f, 0.0f, 1.0f); // l  
			colours[12] = new Color (0.0f, 1.0f, 0.0f, 1.0f); // m  
			colours[13] = new Color (0.0f, 0.0f, 0.2f, 1.0f); // n  
			colours[14] = new Color (0.0f, 0.0f, 0.4f, 1.0f); // o  
			colours[15] = new Color (0.0f, 0.0f, 0.6f, 1.0f); // p  
			colours[16] = new Color (0.0f, 0.0f, 0.8f, 1.0f); // q  
			colours[17] = new Color (0.0f, 0.0f, 1.0f, 1.0f); // r 
			colours[18] = new Color (0.2f, 0.0f, 0.2f, 1.0f); // s  
			colours[19] = new Color (0.4f, 0.0f, 0.4f, 1.0f); // t 
			colours[20] = new Color (0.6f, 0.0f, 0.6f, 1.0f); // u 
			colours[21] = new Color (0.8f, 0.0f, 0.8f, 1.0f); // v 
			colours[22] = new Color (1.0f, 0.0f, 1.0f, 1.0f); // w  
			colours[23] = new Color (0.2f, 0.2f, 0.0f, 1.0f); // x  
			colours[24] = new Color (0.4f, 0.4f, 0.0f, 1.0f); // y  
			colours[25] = new Color (0.6f, 0.6f, 0.0f, 1.0f); // z 
		}

		if (debug_colours_base){
			colours[0] = new Color (1.0f, 0.0f, 0.0f, 1.0f); // a
			colours[1] = new Color (0.0f, 1.0f, 0.0f, 1.0f); // b  
			colours[2] = new Color (0.0f, 0.0f, 1.0f, 1.0f); // c 
			colours[3] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // d  
			colours[4] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // e  
			colours[5] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // f  
			colours[6] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // g 
			colours[7] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // h 
			colours[8] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // i 
			colours[9] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // j  
			colours[10] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // k  
			colours[11] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // l  
			colours[12] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // m  
			colours[13] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // n  
			colours[14] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // o  
			colours[15] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // p  
			colours[16] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // q  
			colours[17] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // r 
			colours[18] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // s  
			colours[19] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // t 
			colours[20] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // u 
			colours[21] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // v 
			colours[22] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // w  
			colours[23] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // x  
			colours[24] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // y  
			colours[25] = new Color (1.0f, 1.0f, 1.0f, 1.0f); // z 
		}
	}

	string String_Replace(string input){
		string output = "";
		for (int i = 0; i < input.Length; ++i) {
			for (int j = 0; j < rules_in.Length; ++j) {
				string temp = input[i].ToString();
				if (string.Equals(temp, rules_in [j])) {
					output += PickOutputString(j);
					break;
				} else if (j == (rules_in.Length-1)) {
					output+= temp;
				}
			}

		}
		return output;
	}

	/* Function: PickOutputString(position)
	 * 
	 * checks the string position and then returns the rule which corresponds to the value chosen
	 * 
	 * 
	*/
	public string PickOutputString(int rule_position){
		float rule_value = rng.NextFloat();
		for (int i = 0; i < stochastic_values.GetLength(1); ++i) {
			if (stochastic_values[rule_position,i] >= rule_value) {
			    return rules_out[rule_position,i];
			}
		}
		return rules_in[rule_position];
	}

	// checks the angle relative to the flow and phototaxis
	// relevant variables are light_constant, light_direction, flow, test_flow, and flow_constant
	private void AdjustAngles(ref float angle_a, ref float angle_b){

		Vector3 temp_vector = RotatePoint (new Vector3 (0, 1, 0),angle_a, angle_b);
		//float dp = Mathf.Sqrt(Vector3.Dot(test_flow, temp_vector))/(Mathf.Sqrt(Vector3.Dot(test_flow,test_flow))*Mathf.Sqrt(Vector3.Dot(temp_vector, temp_vector)));
		if (debug_AdjustAngles) {
			//Debug.Log ("cp value is " + cp);
			//Debug.Log ("dp value is " + dp);
			Debug.Log ("angle a value before is " + angle_a);
			Debug.Log ("angle b value before is " + angle_b);
		}

		// if dp = 0 we are happy, else we want to make dp = 0 because that means the branch is perpendicular to the flow
		if (flow_on) {
			Vector3 cp1 =  new Vector3(0,0,0);
			Vector3 cp2 =  new Vector3(0,0,0);
			Vector3 temp = new Vector3(0,0,0);
			cp1 = Vector3.Cross( test_flow, temp_vector);
			cp2 = Vector3.Cross(test_flow, cp1);
			temp = Vector3.RotateTowards (temp_vector, cp2, flow_constant_lerp, max_mag);
			float theta = Mathf.PI/2;
			if (temp.x!=0) {
				theta = Mathf.Atan (temp.y / temp.x);
			}
			float phi = Mathf.Acos (temp.z);
			angle_b = (flow_constant*phi) + ((1.0f - flow_constant)*angle_b);
			angle_a = (flow_constant*theta) + ((1.0f - flow_constant)*angle_a);
			if (debug_AdjustAngles) {
				Debug.Log ("theta and phi are " + theta +" "+ phi);
				Debug.Log ("cp1 is " + cp1.ToString());
				Debug.Log ("cp2 is " + cp2.ToString());
				Debug.Log ("temp is " + temp.ToString());

			}
		}
		// we need to look at the continueum propperly, cp produces sin theta and dp produces the dot product
		if (add_phototaxis){
			if (angle_b > Mathf.PI) {
				angle_b += light_constant;
			} else {
				angle_b -= light_constant;
			}
		}
		if (invariant_flow_on) {
			if (angle_a > Mathf.PI) {
				if (angle_a > ((3*Mathf.PI) / 2)) {
					angle_a += flow_constant;
				} else {
					angle_a -= flow_constant;
				}
			} else {
				if (angle_a < (Mathf.PI / 2)) {
					angle_a -= flow_constant;
				} else {
					angle_a += flow_constant;
				}
			}
		}
		if (add_wobble) {
			angle_a += (wobble_rng.NextFloat () - 0.5f)*wobble_strength;
			angle_b += (wobble_rng.NextFloat () - 0.5f)*wobble_strength;
		}
		if (debug_AdjustAngles) {
			Debug.Log ("angle a value after is " + angle_a);
			Debug.Log ("angle b value after is " + angle_b);
		}
		angle_a = (angle_a + two_pi) % two_pi;
		angle_b = (angle_b + two_pi) % two_pi;
	}

	public string LSystemGo() {
		string midstep = coral_input_string[dropdown_coral_type.value];
		//string midstep = "tree";
		int iterate = dropdown_recursions.value;
		for (int i = 0; i < iterate; ++i) {
			if (debug_progress) {
				Debug.Log("string building progress " + i + " out of " + iterate);
			}
			midstep = String_Replace (midstep);
		}
		return midstep;
	}

	public void BuildTree(string input){
		string tree = LSystemGo (); 
		if (debug_BuildTree) {
			Debug.Log (tree);
		}
		Destroy (mesh);
		mesh_color.Clear ();
		mesh_normals.Clear ();
		mesh_triangles.Clear ();
		mesh_points.Clear ();
		mesh = new Mesh ();

		if (mesh_points.Count!=mesh_normals.Count) {
			Debug.Log("points and normals not equal");

		}
		if (debug_BuildTree) {
			Debug.Log ("points, normals, triangles, colours: " + mesh_points.Count + " " + mesh_normals.Count + " "
			+ mesh_triangles.Count + " " + mesh_color.Count);
		}
		float turtle_direction_a = 0;
		float turtle_direction_b = 0;
		float turtle_age = 0;
		Vector3 turtle_place = new Vector3 (0,0,0);
		//stack that has position and angle
		Stack<turt_pos> turtle_stack = new Stack<turt_pos>();

		// code for mesh construction from grammar here
		for (int i = 0; i < tree.Length; ++i) {
			if (debug_progress) {
				Debug.Log("string building progress " + i + " out of " + tree.Length);
			}
			string temp_tree = tree[i].ToString();
			if (string.Equals (temp_tree, "[")) {
				if (debug_BuildTree) {
					Debug.Log ("Turtle angles before push" + turtle_direction_a.ToString () + " " + turtle_direction_a.ToString ());
				}
				turt_pos temp = new turt_pos ();
				temp.angle_a = turtle_direction_a;
				temp.angle_b = turtle_direction_b;
				temp.age = turtle_age;
				temp.vec = turtle_place;
				turtle_stack.Push (temp);
				if (debug_BuildTree) {
					Debug.Log ("Turtle after push" + turtle_direction_a.ToString () + " " + turtle_direction_a.ToString ());
				}
			} else if (string.Equals (temp_tree, "]")) { 
				if (debug_BuildTree) {
					Debug.Log ("Turtle before pop" + turtle_direction_a.ToString () + " " + turtle_direction_a.ToString ());
				}
				turt_pos temp = turtle_stack.Pop();
				turtle_direction_a = temp.angle_a;
				turtle_direction_b = temp.angle_b;
				turtle_age = temp.age;
				turtle_place = temp.vec;
				if (debug_BuildTree) {
					Debug.Log ("Turtle after pop"+turtle_direction_a.ToString() + " " +turtle_direction_a.ToString());
				}
			} else {
				for (int j=0;j<rules_in.Length;++j) {
					if (string.Equals(temp_tree, rules_in[j] )) {
						if (debug_BuildTree) {
							Debug.Log ("Turtle position before " + turtle_place.ToString ());
						}
						if (j > base_rules_indexes) {
							AdjustAngles (ref turtle_direction_a, ref turtle_direction_b);
						} else {
							if (turtle_direction_a < (Mathf.PI/2)) {
								turtle_direction_a += 2*rng.NextFloat ();
								turtle_direction_a = (turtle_direction_a + two_pi) % two_pi;
							} else {
								turtle_direction_a -= 2*rng.NextFloat ();
								turtle_direction_a = (turtle_direction_a + two_pi) % two_pi;
							}
						}
						DrawSquare (turtle_place, turtle_direction_a, turtle_direction_b, j, i, turtle_age);
						turtle_place = RotatePoint (new Vector3 (0, length [j], 0), turtle_direction_a, turtle_direction_b) + turtle_place; 
						if (debug_BuildTree) {
							Debug.Log ("Turtle position after " + turtle_place.ToString ());
						}
						break;
					}
				}
				for (int j=0;j<symbols_default_start.Length;++j) {
					if (string.Equals(temp_tree, symbols_default_start[j] )) {
						if (debug_BuildTree) {
							Debug.Log ("Turtle rotate before "+turtle_direction_a + " " + turtle_direction_b);
						}

						turtle_direction_a += angle_a[j];
						turtle_direction_b += angle_b[j];
						turtle_direction_a = (turtle_direction_a + two_pi) % two_pi;
						turtle_direction_b = (turtle_direction_b + two_pi) % two_pi;
						if (j > base_symbols_indexes) {
							if (turtle_direction_a < (Mathf.PI/2)) {
								turtle_direction_a += 2*rng.NextFloat ();
								turtle_direction_a = (turtle_direction_a + two_pi) % two_pi;
							} else {
								turtle_direction_a -= 2*rng.NextFloat ();
								turtle_direction_a = (turtle_direction_a + two_pi) % two_pi;
							}
						}
						if (debug_BuildTree) {
							Debug.Log ("Turtle rotate after "+turtle_direction_a + " " + turtle_direction_b);
						}
						break;
					}
				}
			}
		}

		if (mesh_points.Count!=mesh_normals.Count) {
			Debug.Log("points and normals not equal");
			return;
		}
		if (debug_BuildTree) {
			Debug.Log ("points, normals, triangles, colours: " + mesh_points.Count + " " + mesh_normals.Count + " "
			+ mesh_triangles.Count + " " + mesh_color.Count);
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


	/* Method: RotatePoint
	 * takes a point and rotates it around the x and z axis given angles for both
	 * 
	 */
	private Vector3 RotatePoint(Vector3 point, float direction_a, float direction_b) {
		float cos_angle_a = (float)Math.Cos(direction_a);
		float sin_angle_a = (float)Math.Sin(direction_a);
		float cos_angle_b = (float)Math.Cos(direction_b);
		float sin_angle_b = (float)Math.Sin(direction_b);
		return new Vector3(
			(point.x * cos_angle_a * cos_angle_b) - (point.y * cos_angle_a * sin_angle_b) + (point.z * sin_angle_a), 
			(point.x * sin_angle_b) + (point.y * cos_angle_b),
			(point.y * sin_angle_a * sin_angle_b)- (point.x * sin_angle_a * cos_angle_b) + (point.z * cos_angle_a)
		);
		//Quaternion e = Quaternion.RotateTowards(start_quaternion, new Quaternion( (sin_angle_a * cos_angle_b), (sin_angle_a * sin_angle_b), 
		//	cos_angle_a, 1), 10000000.0f );
		//return e.eulerAngles;
	}

	/* Method: DrawSquare
	 * purpose: This uses two angles, a and b, a base width, a peak width, and a length in order to position a 
	 * construct in 3 dimensional space
     *
	*/
	public void DrawSquare(Vector3 position, float direction_a, float direction_b, int variable, int stringPlace, float age){

		if (mesh_points.Count > (MAX_TRIANGLES+8)) {
			return;
		}


		float wt = width_tip[variable]/2;
		float wb = width_base[variable]/2;
		Vector3 P1 = new Vector3( 0, 0, wb); //need to adjust once ratios is implemented
		Vector3 P2 = new Vector3( 0, 0, -wb);
		Vector3 P3 = new Vector3( 0, length[variable], wt);
		Vector3 P4 = new Vector3( 0, length[variable], -wt);
		Vector3 P5 = new Vector3( wb, 0, 0); //need to adjust once ratios is implemented
		Vector3 P6 = new Vector3( -wb, 0, 0);
		Vector3 P7 = new Vector3( wt, length[variable], 0);
		Vector3 P8 = new Vector3 (-wt, length [variable], 0);

		// use rotation matrix to adjust points
		P1 = RotatePoint (P1, direction_a, direction_b) + position;
		P2 = RotatePoint (P2, direction_a, direction_b) + position;
		P3 = RotatePoint (P3, direction_a, direction_b) + position;
		P4 = RotatePoint (P4, direction_a, direction_b) + position;
		P5 = RotatePoint (P5, direction_a, direction_b) + position;
		P6 = RotatePoint (P6, direction_a, direction_b) + position;
		P7 = RotatePoint (P7, direction_a, direction_b) + position;
		P8 = RotatePoint (P8, direction_a, direction_b) + position;
		mesh_points.Add(P1);
		mesh_points.Add(P2);
		mesh_points.Add(P3);
		mesh_points.Add(P4);
		mesh_points.Add(P5);
		mesh_points.Add(P6);
		mesh_points.Add(P7);
		mesh_points.Add(P8);

		int temp_tr = 0;
		if (mesh_triangles.Count>0) {
			temp_tr = mesh_triangles[mesh_triangles.Count-1]+1;
		}

		//add triangles
		mesh_triangles.Add(temp_tr);
		mesh_triangles.Add(temp_tr+1);
		mesh_triangles.Add(temp_tr+2);
		mesh_triangles.Add(temp_tr+1);
		mesh_triangles.Add(temp_tr+2);
		mesh_triangles.Add(temp_tr+3);

		mesh_triangles.Add(temp_tr);
		mesh_triangles.Add(temp_tr+2);
		mesh_triangles.Add(temp_tr+1);
		mesh_triangles.Add(temp_tr+1);
		mesh_triangles.Add(temp_tr+3);
		mesh_triangles.Add(temp_tr+2);

		mesh_triangles.Add(temp_tr+4);
		mesh_triangles.Add(temp_tr+6);
		mesh_triangles.Add(temp_tr+5);
		mesh_triangles.Add(temp_tr+5);
		mesh_triangles.Add(temp_tr+7);
		mesh_triangles.Add(temp_tr+6);

		mesh_triangles.Add(temp_tr+4);
		mesh_triangles.Add(temp_tr+5);
		mesh_triangles.Add(temp_tr+6);
		mesh_triangles.Add(temp_tr+5);
		mesh_triangles.Add(temp_tr+6);
		mesh_triangles.Add(temp_tr+7);

		//add normals
		mesh_normals.Add(new Vector3(0, 0, -1));
		mesh_normals.Add(new Vector3(0, 0, -1));
		mesh_normals.Add(new Vector3(0, 0, -1));
		mesh_normals.Add(new Vector3(0, 0, -1));

		mesh_normals.Add(new Vector3(0, 0, -1));
		mesh_normals.Add(new Vector3(0, 0, -1));
		mesh_normals.Add(new Vector3(0, 0, -1));
		mesh_normals.Add(new Vector3(0, 0, -1));

		//add colors 
		mesh_color.Add(new Color( colours[variable].r, colours[variable].g, colours[variable].b, 1 ));
		mesh_color.Add(new Color( colours[variable].r, colours[variable].g, colours[variable].b, 1 ));
		mesh_color.Add(new Color( colours[variable].r, colours[variable].g, colours[variable].b, 1 ));
		mesh_color.Add(new Color( colours[variable].r, colours[variable].g, colours[variable].b, 1 ));

		mesh_color.Add(new Color( colours[variable].r, colours[variable].g, colours[variable].b, 1 ));
		mesh_color.Add(new Color( colours[variable].r, colours[variable].g, colours[variable].b, 1 ));
		mesh_color.Add(new Color( colours[variable].r, colours[variable].g, colours[variable].b, 1 ));
		mesh_color.Add(new Color( colours[variable].r, colours[variable].g, colours[variable].b, 1 ));
	}


	//
	// gui helper functions
	//

	public void MakeString(){
		LSystemGo ();
	}

	public void MakeImage(){
		rng.Seed(seed_value);
		float f = rng.NextFloat();
		if (debug_MakeImage) {
			Debug.Log ("start float is" + f);
		}
		string final = LSystemGo ();
		BuildTree (final);
	}

	public void OnCoralChanged(){
		//current_coral = dropdown_coral_type.value;
	}

	public void OnRecursionsChanged(){
		//iterations = dropdown_recursions.value;
	}
		
	public void OnFlowXChanged(float c) {
		flow.x = c;
	}

	public void OnFlowYChanged(float c) {
		flow.y = c;
	}	

	public void OnFlowZChanged(float c) {
		flow.z = c;
	}
}
