using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
[System.Serializable]

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class LSystemController : MonoBehaviour {
	/* This Program allows the user to produce corals from an L-System
	 *
	 * 							
	*/
	struct stoch_rules {
		public float probability;
		public string result;
	}

	struct turt_pos {
		public Vector3 vec;
		public float angle_a;
		public float angle_b;
	};

	private Vector3 flow;
	public Text display;
	public Text starter;
	public int iterations;
	private int current_coral;
	public Dropdown dropdown_coral_type;
	public Dropdown dropdown_recursions;

	private MeshFilter mf;
	private MeshRenderer mr;
	private Mesh mesh;

	private List<Vector3> mesh_points;
	private List<Vector3> mesh_normals;
	private List<Color> mesh_color;
	private List<int> mesh_triangles;

	private string[] corals_available = { "Staghorn"};
	private string[] coral_input_string = {"!b"};

	private string[] rules_in_start = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p",
		"q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
	private string[] rules_out_start = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p",
		"q", "r", "s", "t", "u", "v", "w", "x", "y", "z"}; // associated with the rules in
	private float[] width_tip={1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 
		1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f};
	private float[] width_base ={1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 
		1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f};
	private string[] symbols_default_start = { "+", "-","!", "@", "#", "$", "%", "^", "&", "*", "(", ")"};// associated with angles a&b
	private float[] angle_a ={1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 
		1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f};
	private float[] angle_b ={1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 
		1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f};
	private string[] rules_in;	
	private string[] rules_out;	
	private Color[] colours;

	private float[] length;
	private float[][] weight; // need an uneven array?



	/// Name: Start()
	/// Initializes all of the needed gui options
	void Start() {

		flow = new Vector3 (0,0,0);
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
			
		dropdown_coral_type.options.Clear ();
		for (int i=0; i<corals_available.Length;++i) {
			dropdown_coral_type.options.Add (new Dropdown.OptionData(){text=corals_available[i]});
		}

		length = new float[rules_in_start.Length];
		width_base = new float[rules_in_start.Length];
		width_tip = new float[rules_in_start.Length];
		rules_out = new string[rules_in_start.Length];
		colours = new Color[rules_in_start.Length];
		for (int i=0; i<rules_in_start.Length;++i) {
			rules_out [i] = rules_out_start [i];
			colours[i] = new Color(1.0f,1.0f,1.0f,1.0f);
			length [i] = 3.0f;
			width_tip [i] = 0.1f;
			width_base [i] = 0.1f;
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

		float turtle_direction_a = 0;
		float turtle_direction_b = 0;
		Vector3 turtle_place = new Vector3 (0,0,0);
		//stack that has position and angle
		Stack<turt_pos> turtle_stack = new Stack<turt_pos>();

		// code for mesh construction from grammar here
		for (int i = 0; i < tree.Length; ++i) {
			string temp_tree = tree[i].ToString();
			if (string.Equals(temp_tree, "[" )) {
				turt_pos temp = new turt_pos();
				temp.angle_a = turtle_direction_a;
				temp.angle_b = turtle_direction_b;
				temp.vec = turtle_place;
				turtle_stack.Push(temp);
			} else if (string.Equals(temp_tree, "]" )) { 
				turt_pos temp = turtle_stack.Pop();
				turtle_direction_a = temp.angle_a;
				turtle_direction_b = temp.angle_b;
				turtle_place = temp.vec;
				Debug.Log ("Popped: " + turtle_place.ToString());
				Debug.Log (turtle_direction_a);
				Debug.Log (turtle_direction_b);
			} else {
				for (int j=0;j<rules_in.Length;++j) {
					if (string.Equals(temp_tree, rules_in[j] )) {
						DrawSquare (turtle_place, turtle_direction_a, turtle_direction_b, j, i);
						turtle_place = new Vector3( turtle_place.x + (length[j]*(float)Math.Cos(turtle_direction_a)), 
							turtle_place.y +(length[j]*(float)Math.Sin(turtle_direction_a)), turtle_place.z);
						break;
					}
				}
				for (int j=0;j<symbols_default_start.Length;++j) {
					if (string.Equals(temp_tree, symbols_default_start[j] )) {
						turtle_direction_a += angle_a[j];
						turtle_direction_b += angle_b[j];
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



	public void DrawSquare(Vector3 position, float direction_a, float direction_b, int variable, int stringPlace){
		float wt = width_tip[variable]/2;
		float wb = width_base[variable]/2;
		Vector3 P1 = new Vector3( length[variable], wt, 0); //need to adjust once ratios is implemented
		Vector3 P2 = new Vector3( length[variable], -wt, 0);
		Vector3 P3 = new Vector3( 0, wb, 0);
		Vector3 P4 = new Vector3( 0, -wb, 0);

		// use rotation matrix to adjust points
		// x = px cos a - py sin a + tx
		// y = px sin a + py cos a + ty
		float cos_angle = (float)Math.Cos(direction_a) ;
		float sin_angle = (float)Math.Sin(direction_a);
		P1 = new Vector3((P1.x * cos_angle - P1.y * sin_angle + position.x), 
			(P1.x * sin_angle + P1.y * cos_angle + position.y), 0);
		P2 = new Vector3((P2.x * cos_angle - P2.y * sin_angle + position.x), 
			(P2.x * sin_angle + P2.y * cos_angle + position.y), 0);
		P3 = new Vector3((P3.x * cos_angle - P3.y * sin_angle + position.x), 
			(P3.x * sin_angle + P3.y * cos_angle + position.y), 0);
		P4 = new Vector3((P4.x * cos_angle - P4.y * sin_angle + position.x), 
			(P4.x * sin_angle + P4.y * cos_angle + position.y), 0);		
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

	public void OnCoralChanged(){
		current_coral = dropdown_coral_type.value;
	}

	public void OnRecursionsChanged(){
		iterations = dropdown_recursions.value;
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
