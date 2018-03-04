/*
 * This is the main class that manipulates the cube , it uses the 2DCube class as a blue print to
 * properly make decisions on which face and pieces to be rotated ,
 * 
 * The logic here is to use tags to map all pieces , this is then
 * used in the 2D Cube class to simulate rotations,the 2D cube is also used to return
 * the result of the rotated face which contains the pieces tags , the tags are then used to change parents
 * of the pieces to their respective face which is the center piece of the selected face,
 * this center piece is then rotated which results in all child
 * pieces to be rotated along the parent which gives our simulation much wow.
 * 
 * This class contains the following features:
 * - Cube face rotation calculation and whole cube rotation.
 * - Cube scrambling algorithm.
 * - Trail toggle.
 * - Keyboard inputs.
 * - Buttons input.
 * - Reset Cube.
 * - Timer.
 * - Piston Effect.
 * - AI cube solver. (Via Cube2D)
 * - Rotation speed selection.
 * 
 * More features can be added in the future such as :
 * - World Records.
 * - Speedsolver ladder.
 * - Mouse drag input via raycasting.
 * - Cube solving tutorials for beginners.
 * - Practice mode to solve certain cube states.
 * 
 * If you have any questions you can reach me out at Fahad@NoCakeNoCode.com
 * 
 * -Fahad
 * 
 * ..........Last Edited on 12th Dec 2015
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/*
using doge.shibe.wow;
░░░░░░░░░▄░░░░░░░░░░░░░░▄░░░░
░░░░░░░░▌▒█░░░░░░░░░░░▄▀▒▌░░░
░░░░░░░░▌▒▒█░░░░░░░░▄▀▒▒▒▐░░░
░░░░░░░▐▄▀▒▒▀▀▀▀▄▄▄▀▒▒▒▒▒▐░░░
░░░░░▄▄▀▒░▒▒▒▒▒▒▒▒▒█▒▒▄█▒▐░░░
░░░▄▀▒▒▒░░░▒▒▒░░░▒▒▒▀██▀▒▌░░░
░░▐▒▒▒▄▄▒▒▒▒░░░▒▒▒▒▒▒▒▀▄▒▒▌░░
░░▌░░▌█▀▒▒▒▒▒▄▀█▄▒▒▒▒▒▒▒█▒▐░░
░▐░░░▒▒▒▒▒▒▒▒▌██▀▒▒░░░▒▒▒▀▄▌░
░▌░▒▄██▄▒▒▒▒▒▒▒▒▒░░░░░░▒▒▒▒▌░
▀▒▀▐▄█▄█▌▄░▀▒▒░░░░░░░░░░▒▒▒▐░
▐▒▒▐▀▐▀▒░▄▄▒▄▒▒▒▒▒▒░▒░▒░▒▒▒▒▌
▐▒▒▒▀▀▄▄▒▒▒▄▒▒▒▒▒▒▒▒░▒░▒░▒▒▐░
░▌▒▒▒▒▒▒▀▀▀▒▒▒▒▒▒░▒░▒░▒░▒▒▒▌░
░▐▒▒▒▒▒▒▒▒▒▒▒▒▒▒░▒░▒░▒▒▄▒▒▐░░
░░▀▄▒▒▒▒▒▒▒▒▒▒▒░▒░▒░▒▄▒▒▒▒▌░░
░░░░▀▄▒▒▒▒▒▒▒▒▒▒▄▄▄▀▒▒▒▒▄▀░░░
░░░░░░▀▄▄▄▄▄▄▀▀▀▒▒▒▒▒▄▄▀░░░░░
░░░░░░░░░▒▒▒▒▒▒▒▒▒▒▀▀░░░░░░░░
wer am I !?
wow
much code violence
such progrombing
hw dis hapen?!!
*/

public class CubeController : MonoBehaviour {

	// cube pieces
	private Transform[] centerP ;
	private Transform[] cornerP ;
	private Transform[] edgeP;
	private Transform cube_3x3x3;
	
	// rotation variables
	private float sensitivityX = 15.0f;
	private float sensitivityY = 15.0f;
    [HideInInspector]
	public int[] rotationSpeeds;
   
	private int speed = 10;
	private int count = 10;
	private Transform cameraTm;
	private bool rotate = false;
	private bool rotateInit = false;

	// scrambling
	private List<int> scrambleList;
	private List<bool> scrambleReverseList;
	private bool scramble = false;
	
	// move selection indicator
	private int selection = -1;
	
	// rotation speed drop down initial selection , the speed list contains index to the rotation speed list
    public enum speedList {
        _1 = 0,_2 = 1,_3 = 2,_5 = 3,_6 = 4,_9 = 5,_10 = 6,_15 = 7,_18 = 8,_30 = 9,_45 = 10,_90 = 11
    }

    public speedList speedOption;

    [HideInInspector]
    public int selectedItemIndex;
	
	// reverse control
	private GameObject reverseObj;
	public bool reverse = false;
	
	/* 2D Cube Logic used to map cubies to parents , 
	 * another solution is to go with cube colliders and triggers (semi-unreliable + time consuming to debug)*/
	private Cube2D cube2d;
	private string[,] face;
	
	// trail toggle
	private GameObject[] trailObjs;
	private TrailRenderer[] trailRends;
	//private bool trailChecked = true;

	// timer
	[SerializeField]
	public Text timeText;
	private float time = 0.0f;
	private bool timerStarted = false;
	
	// Audio Controlls for BGM/SFX
	private AudioController audioCon;
	
	//  Variables to Simulate Piston Effect
    public Toggle piston;
	private int state = -1;
	private bool stateOddOrEven = false; // false -> Even | true -> Odd
	private int countState = 0;
	public float pistonLength = 2.7f;
	public bool pistonEnabled = false;
	private List<Transform> pistonEdges = new List<Transform>(); // for future development
	private string[] pistonEdgesAxis = new string[4]; // for future development

	// lightning beam effect toggle
	public bool lightningBeamEnabled = true;

	// AI Solve
	private bool solveMode = false;
	private List<int> solveMoveList;
	private List<bool> solveMoveReverseList;
    public Text solved;

	void Start () {  

		// initialize 2D RB Logic
		cube2d = new Cube2D ();
		
		// initialize pieces
		initPieces ();

		// initialize trails
		initTrail ();

		// initialize game objects
		initETC ();
		
		// initialize rotation speed list and combo box
		initSpeedList ();

	}

	private void initTrail(){
		trailObjs = GameObject.FindGameObjectsWithTag("Trail");
        
        trailRends = new TrailRenderer[trailObjs.Length];
        for (int i = 0; i < trailObjs.Length; i++)
            trailRends[i] = trailObjs[i].GetComponentInChildren<TrailRenderer>();
        if (GameObject.Find("trail_toggle") != null) {
            for (int i = 0; i < trailRends.Length; i++)
                trailRends[i].enabled = GameObject.Find("trail_toggle").GetComponent<Toggle>().isOn;
        }


        /*
            * In my game scene I use a game object called Passport in order to keep changes after
            * reseting the cube by reloading the scene.
            */
        if (GameObject.Find("Passport") != null && GameObject.Find("Passport").GetComponent<Reset>().trailEnabled) {
            GameObject.Find("trail_toggle").GetComponent<Toggle>().isOn = true;
            OnTrailTogglePushed();
        }
        
	}
	
	private void initETC(){
		
		cube_3x3x3 = GameObject.Find ("Cube 3x3x3").transform;
		reverseObj = GameObject.Find("reverse");

		scrambleList = new List<int>();
		scrambleReverseList = new List<bool>();

		solveMoveList = new List<int> ();
		solveMoveReverseList = new List<bool> ();

		cameraTm = Camera.main.transform;

		if (GameObject.Find ("Passport") != null)
			audioCon = GameObject.Find ("Passport").GetComponent<AudioController> ();
		else
			audioCon = null;

        selectedItemIndex = (int)speedOption;
     
	}

	private void initPieces(){

		centerP = new Transform[6];
		cornerP = new Transform[8];
		edgeP = new Transform[12];
//		Transform core = GameObject.FindGameObjectWithTag ("Core").transform;
		for (int i = 0; i < 6; i++) {
			centerP[i] = GameObject.FindGameObjectWithTag ("Center Piece " + (i + 1)).transform;
			 //centerPSpacing(centerP[i] , pistonLength);
		}

		for (int i = 0; i < 8; i++) {
			cornerP[i] = GameObject.FindGameObjectWithTag ("Corner Piece " + (i + 1)).transform;
			// cornerPSpacing(cornerP[i] , pistonLength);
		}

		for (int i = 0; i < 12; i++) {
			edgeP[i] = GameObject.FindGameObjectWithTag ("Edge Piece " + (i + 1)).transform;
			// edgePSpacing(edgeP[i] , pistonLength);
		}
	}

	//  Center piece spacing
	private void centerPSpacing(Transform centerPie , float space){
		float x = centerPie.localPosition.x;
		float y = centerPie.localPosition.y;
		float z = centerPie.localPosition.z;

		if(Mathf.Abs(x) > Mathf.Max(Mathf.Abs(y),Mathf.Abs(z))){
			x += x > 0 ? space : -space;
		}else if(Mathf.Abs(y) > Mathf.Abs(z)){
			y += y > 0 ? space : -space;
		}else{
			z += z > 0 ? space : -space;
		}
		centerPie.GetComponent<LightningGenerator> ().Spawn = true;
		centerPie.localPosition = new Vector3(x,y,z);
	}

	//  Corner piece spacing
	private void cornerPSpacing(Transform cornerPie , float space){
		float x = cornerPie.localPosition.x;
		float y = cornerPie.localPosition.y;
		float z = cornerPie.localPosition.z;
				
		//x += x > 0 ? space : -space;
		
		y += y > 0 ? space : -space;
		
		z += z > 0 ? space : -space;

		cornerPie.GetComponent<LightningGenerator> ().Spawn = true;
		cornerPie.localPosition = new Vector3(x,y,z);
	}



	// Edge piece spacing
	private float incr = 1.1f;
	private void edgePSpacing(Transform edgePie , float space){

		/*float x = edgePie.localPosition.x;
		float y = edgePie.localPosition.y;
		float z = edgePie.localPosition.z;*/

		edgePie.position = (edgePie.position - centerP [selection].position).normalized * (incr) + centerP [selection].position;
		incr += space > 0 ? 0.01f : -0.01f;
		//x += space;
		edgePie.GetComponent<LightningGenerator> ().Spawn = true;
		//edgePie.localPosition = new Vector3(x,y,z);
	}
	
	private void initSpeedList(){
		// The factors of 90. Answer : 1,2,3,5,6,9,10,15,18,30,45,90
		/* using facotrs of 90 to properly rotate the cube, other values non factor of 90 are suspect of 
		 * floating point precision limitation which can cause marginal precision errors. -> wow
		 */ 
		rotationSpeeds = new int[]{1,2,3,5,6,9,10,15,18,30,45,90};		
	}

	void Update () {

		if (scramble && !rotate)
			if(scrambleList.Count > 0){
				selection = scrambleList[0];
				scrambleList.RemoveAt(0);
				reverse = scrambleReverseList[0];
				scrambleReverseList.RemoveAt(0);
				rotate = true;
			} else {
				scramble = false;
				rotate = false;
				reverse = scrambleReverseList[0];
				scrambleReverseList.RemoveAt(0);
			}

		if (solveMode && !rotate) {
			if(solveMoveList.Count > 0){
				selection = solveMoveList[0];
				solveMoveList.RemoveAt(0);
				reverse = solveMoveReverseList[0];
				solveMoveReverseList.RemoveAt(0);
				rotate = true;
			}else{
				solveMode = false;
				rotate = false;
				reverse = solveMoveReverseList[0];
				solveMoveReverseList.RemoveAt(0);
			}
		}

		controls ();
		rotation ();
	}

	// Keyboard inputs
	private void controls(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (timerStarted) {
				timerStarted = false;
			} else {
				time = 0.0f;
				timerStarted = true;
			}
		} else if (Input.GetKeyDown (KeyCode.R) && !rotate) {
			selection = 0;
			rotate = true;
		} else if (Input.GetKeyDown (KeyCode.L) && !rotate) {
			selection = 2;
			rotate = true;
		} else if (Input.GetKeyDown (KeyCode.U) && !rotate) {
			selection = 5;
			rotate = true;
		} else if (Input.GetKeyDown (KeyCode.D) && !rotate) {
			selection = 4;
			rotate = true;
		} else if (Input.GetKeyDown (KeyCode.F) && !rotate) {
			selection = 3;
			rotate = true;
		} else if (Input.GetKeyDown (KeyCode.B) && !rotate) {
			selection = 1;
			rotate = true;
		} else if (Input.GetKeyDown (KeyCode.LeftControl) && !rotate) {
			if (!reverse) {
				if (reverseObj != null)
					reverseObj.GetComponentInChildren<Text> ().text = "Reverse On";
				reverse = true;
			} else if (reverse) {
				if (reverseObj != null)
					reverseObj.GetComponentInChildren<Text> ().text = "Reverse Off";
				reverse = false;
			}
		} else if (Input.GetKeyDown (KeyCode.S) && !rotate) { //solve cross test
            SolveCube();
		}
	}



	private void rotation(){
		
		// Mouse Drag Cube Rotation
		if (Input.GetMouseButton (1)) { // mouse right click hold
			float rotationX = Input.GetAxis ("Mouse X") * sensitivityX;
			float rotationY = Input.GetAxis ("Mouse Y") * sensitivityY;
			transform.Rotate (cameraTm.up, -Mathf.Deg2Rad * rotationX * Time.deltaTime * 5000,Space.World);
			transform.Rotate (cameraTm.right, Mathf.Deg2Rad * rotationY * Time.deltaTime * 5000,Space.World);
		}
		
		// rotate cube face
		if (rotate) {
			if(!rotateInit){
                if (piston != null)
				    GameObject.Find("Piston").GetComponent<Toggle>().interactable = false;

				// get selected face
				face = cube2d.getFace(selection);
				
				//int res = 0;
				pistonEdges.Clear();
				for(int i = 0;i < 3;i++){
					for(int j = 0;j < 3;j++){
						// preparing edges for piston movement logic (for future development)
						/*if(GameObject.FindWithTag(face[i,j]).name.StartsWith("Edge")){
							//print(GameObject.FindWithTag(face[i,j]).name + " " + GameObject.FindWithTag(face[i,j]).transform.localPosition);

							float x = GameObject.FindWithTag(face[i,j]).transform.localPosition.x;
							float y = GameObject.FindWithTag(face[i,j]).transform.localPosition.y;
							float z = GameObject.FindWithTag(face[i,j]).transform.localPosition.z;

							// Top Face Check
							if( (int) y == 1 && (int)z == -1){ //move -z axis
								pistonEdges.Add(GameObject.FindWithTag(face[i,j]).transform);
								pistonEdgesAxis[res] = "-y";
								res++;
							}else if((int)x == -1 && (int)y == 1){ //move -x axis
								pistonEdges.Add(GameObject.FindWithTag(face[i,j]).transform);
								pistonEdgesAxis[res] = "-z";	
								res++;
							}else if((int)y == 1 && (int)z == 1){ //move z axis
								pistonEdges.Add(GameObject.FindWithTag(face[i,j]).transform);
								pistonEdgesAxis[res] = "y";		
								res++;
							}else if((int)x == 1 && (int)y == 1){ //move x axis
								pistonEdges.Add(GameObject.FindWithTag(face[i,j]).transform);
								pistonEdgesAxis[res] = "z";	
								res++;
							}

						}*/

						// Assign lightning effect end points
						if(GameObject.FindWithTag(face[i,j]).name.StartsWith("Corner")){
							GameObject.FindWithTag(face[i,j]).GetComponent<LightningGenerator>().EndPoint = centerP[selection].gameObject;
						}else if(GameObject.FindWithTag(face[i,j]).name.StartsWith("Edge")){
							GameObject.FindWithTag(face[i,j]).GetComponent<LightningGenerator>().EndPoint = centerP[selection].gameObject;
						}else if(GameObject.FindWithTag(face[i,j]).name.StartsWith("Center")){
							GameObject.FindWithTag(face[i,j]).GetComponent<LightningGenerator>().EndPoint = GameObject.FindWithTag("Core").gameObject;
						}

						// assign face cubies to parent
						GameObject.FindWithTag(face[i,j]).transform.parent = centerP[selection];
					}
				}

				// rotate 2d cube

				cube2d.Rotate(selection);
				if(reverse){ // a little cheat so I don't have to code too much = slight performance impact :]
					cube2d.Rotate(selection);
					cube2d.Rotate(selection);
				}

				
				count = speed = rotationSpeeds [selectedItemIndex]; // set rotation speed
				rotateInit = true;

				// play cube turning sound if sfx is enabled
				if(audioCon != null && audioCon.enableSFX)
					audioCon.sfxsource.PlayOneShot(audioCon.rubikturnsfx);

				// calculate if increments total results in odd or even number
				state = 90 / speed;
				stateOddOrEven = state % 2 == 0 ? false : true;
			}
			
			// rotate face
			centerP [selection].Rotate(new Vector3(reverse ? -speed : speed, 0 , 0));
			countState++;

			// piston effect
			if(pistonEnabled){
				if(stateOddOrEven) { // if odd
					if(countState > ((state / 2) + 1)){
						centerPSpacing(centerP[selection] , (-1f / state) * pistonLength);
						foreach(Transform child in centerP[selection]){
							if(child.name.StartsWith("Corner")) cornerPSpacing(child , (-1f / state) * pistonLength);
							else if(child.name.StartsWith("Edge")) edgePSpacing(child , (-1f / state) * pistonLength);
						}
					} else if(countState < ((state / 2) + 1)){ 
						centerPSpacing(centerP[selection] , (1f / state) * pistonLength);
						foreach(Transform child in centerP[selection]){
							if(child.name.StartsWith("Corner")) cornerPSpacing(child , (1f / state) * pistonLength);
							else if(child.name.StartsWith("Edge")) edgePSpacing(child , (1f / state) * pistonLength );
						}
					}			
				} else if(!stateOddOrEven ) { // if even
					if(countState > (state / 2)){
						centerPSpacing(centerP[selection] , (-1f / state) * pistonLength);
						foreach(Transform child in centerP[selection]){
							if(child.name.StartsWith("Corner")) cornerPSpacing(child , (-1f / state) * pistonLength);
							else if(child.name.StartsWith("Edge")) edgePSpacing(child , (-1f / state) * pistonLength);
						}
					} else if(countState <= (state / 2)){
						centerPSpacing(centerP[selection] , (1f / state) * pistonLength);
						foreach(Transform child in centerP[selection]){
							if(child.name.StartsWith("Corner")) cornerPSpacing(child , (1f / state) * pistonLength);
							else if(child.name.StartsWith("Edge")) edgePSpacing(child , (1f / state) * pistonLength);
						}
					}
				}
			}

			// check if rotation should finish to reset state
			if(count >= 90){
				resetParents();
				rotateInit = false;
				rotate = false;
				count = 0;
				countState = 0;
                if (piston != null)
			    	piston.interactable = true;

				if(cube2d.isSolved()){
                    if(solved != null)
                        solved.enabled = true;
					timerStarted = false;
				}else{
                    if (solved != null)
                        solved.enabled = false;
				}
				//debugTiles();

			}

			// update count flag to keep up with rotation bounds
			count += speed;
		}

		// update time on screen
		if (timerStarted) {
			time += Time.deltaTime;
			timeText.text = "Timer : " + time.ToString ("F2") + "s";
		}
	}

	// print tiles on console for development purposes..
	private void debugTiles(){
		/*Debug section to test tiles values*/
		string msg = "\t\t\t\t";
		string [,] orange = cube2d.GetOrangeFaceTiles();
		string [,] green = cube2d.GetGreenFaceTiles();
		string [,] white = cube2d.GetWhiteFaceTiles();				
		string [,] blue = cube2d.GetBlueFaceTiles();
		string [,] red = cube2d.GetRedFaceTiles();
		string [,] yellow = cube2d.GetYellowFaceTiles();
		
		for(int i = 0;i < 3;i++){
			for(int j = 0;j < 3;j++){
				msg += orange[i,j] + " ";
			}
			msg += "\n\t\t\t\t";
		}
		
		msg += "\n";
		for(int i = 0;i < 3;i++){
			for(int j = 0;j < 9;j++){
				if(j < 3)
					msg += green[i,j] + " ";
				else if(j > 2 && j < 6)
					msg += white[i,j - 3] + " ";
				else
					msg += blue[i,j - 6] + " ";
				if(j == 2 || j == 5) msg += "\t\t";
			}
			msg += "\n";
		}
		
		msg += "\n\t\t\t\t";
		for(int i = 0;i < 3;i++){
			for(int j = 0;j < 3;j++){
				msg += red[i,j] + " ";
			}
			msg += "\n\t\t\t\t";
		}
		
		msg += "\n\t\t\t\t";
		for(int i = 0;i < 3;i++){
			for(int j = 0;j < 3;j++){
				msg += yellow[i,j] + " ";
			}
			msg += "\n\t\t\t\t";
		}
		print (msg);
	}
	
	// set face pieces parents to main cube to avoid any unwanted rotation bugs/glitches
	private void resetParents(){
		for(int i = 0; i < 3; i++)
			for(int j = 0; j < 3; j++)
				GameObject.FindWithTag(face[i,j]).transform.parent = cube_3x3x3;
	}
	
	public void onButtonClick(string button){
		int result;
		if (!rotate && int.TryParse (button, out result)) {
			// assign selected face to be rotated
			selection = result;
			rotate = true;
		} else {
			// check string conditions and apply changes accordingly
			if(button.Equals("reset")){

				// save trail status in passport reset script
				if(GameObject.Find("Passport") != null)
				GameObject.Find("Passport").GetComponent<Reset>().trailEnabled = 
					GameObject.Find("trail_toggle").GetComponent<Toggle>().isOn;

				DontDestroyOnLoad(GameObject.Find("Passport"));
				//DontDestroyOnLoad(GameObject.Find("Canvas"));


				Application.LoadLevel(0);

			}else if(!rotate && button.Equals("reverse")){
				if(!reverse){
					reverseObj.GetComponentInChildren<Text>().text = "Reverse On";
					reverse = true;
				}else if(reverse){
					reverseObj.GetComponentInChildren<Text>().text = "Reverse Off";
					reverse = false;
				}
			}else if(!rotate && button.Equals("scramble")){
				// trigger scramble mode
				scramble = true;
				// give a number of random faces to be rotated , populate scramble reverse list
				for(int i = 0;i < 30;i++){
					scrambleList.Add(Random.Range(0 , 6));	
					scrambleReverseList.Add(Random.Range(0 , 2) == 1 ? true : false);
				}
				scrambleReverseList.Add(reverse); // store current reverse status for later reset
			}
		}
	}

    public void SolveCube() {
        
        //print("Pressed Solve S , starting to solve cube internally....");
        solveMode = true;
        cube2d.solveCube();

        //print ("Unsolved Edge Pieces found : " + cube2d.unsolvedEdges);

        //print("Finished solving cross internally....applying to this 3D Cube..");

        while (cube2d.moveList.Count > 0) {
            string temp = cube2d.moveList[0];
            cube2d.moveList.RemoveAt(0);
            //print(temp); //print the move
            solveMoveReverseList.Add(temp.StartsWith("-") ? true : false);

            int f = 0;

            if (temp.Contains("F")) {
                f = 3;
            } else if (temp.Contains("B")) {
                f = 1;
            } else if (temp.Contains("D")) {
                f = 4;
            } else if (temp.Contains("R")) {
                f = 0;
            } else if (temp.Contains("L")) {
                f = 2;
            } else if (temp.Contains("U")) {
                f = 5;
            }
            solveMoveList.Add(f);
        }
        solveMoveReverseList.Add(reverse); // store current reverse status for later reset
        //print("Finished!");
    }


	// Toggle trail renderer
	public void OnTrailTogglePushed(){

		// change state
		bool trailChecked = GameObject.Find("Passport").GetComponent<Reset>().trailEnabled;
		GameObject.Find ("Passport").GetComponent<Reset> ().trailEnabled = trailChecked ? false : true;
		trailChecked = GameObject.Find ("Passport").GetComponent<Reset> ().trailEnabled;

		if (trailChecked) 
			for(int i = 0;i < trailRends.Length;i++) 
				trailRends[i].enabled = true;
		else if(!trailChecked)
			for(int i = 0;i < trailRends.Length;i++) 
				trailRends[i].enabled = false;
		
	}

	// Toggle Lightning Piston
	public void OnPistonTogglePushed(){
		bool pistonChecked = GameObject.Find ("Passport").GetComponent<Reset> ().pistonEnabled;
		GameObject.Find ("Passport").GetComponent<Reset> ().pistonEnabled = pistonChecked ? false : true;
		pistonChecked = GameObject.Find("Passport").GetComponent<Reset>().pistonEnabled;

		if (!rotate) {
			if (pistonChecked) {
				pistonEnabled = true;
			} else {
				pistonEnabled = false;
			}
		}
	}
}