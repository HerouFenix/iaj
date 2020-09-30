using Assets.Scripts.IAJ.Unity.Movement.KinematicMovement;
using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;
using UnityEngine.UI;

public class KinematicManager : MonoBehaviour
{
    public const float X_WORLD_SIZE = 55;
    public const float Z_WORLD_SIZE = 32.5f;
    private const float MAX_SPEED = 10.0f;
    private const float TIME_TO_TARGET = 2.0f;
    private const float RADIUS = 1.0f;
    private const float MAX_ROTATION = 8*MathConstants.MATH_PI;

	private KinematicCharacter BlueCharacter { get; set; }
	private KinematicCharacter GreenCharacter { get; set; }

    private Text BlueMovementText { get; set; }

    private Text GreenMovementText { get; set; }

	// Use this for initialization
	void Start () 
	{
		var textObj = GameObject.Find ("InstructionsText");
		if (textObj != null) 
		{
			textObj.GetComponent<Text>().text = 
				"Instructions\n\n" +
				"Blue Character\n" +
				"Q - Stationary\n" +
				"W - Seek\n" + 
				"E - Flee\n" + 
				"R - Arrive\n" + 
				"T - Wander\n\n" +  
				"Green Character\n" + 
				"A - Stationary\n" +
				"S - Seek\n" +
				"D - Flee\n" + 
				"F - Arrive\n" +
				"G - Wander\n"; 
		}

		var BlueObj = GameObject.Find ("Blue");
		if(BlueObj != null) this.BlueCharacter = new KinematicCharacter(BlueObj);
		var greenObj = GameObject.Find ("Green");
		if (greenObj != null) this.GreenCharacter = new KinematicCharacter(greenObj);

	    this.BlueMovementText = GameObject.Find("BlueMovement").GetComponent<Text>();
	    this.GreenMovementText = GameObject.Find("GreenMovement").GetComponent<Text>();
	}

	void Update()
	{
        // Blue Movement ///////////////////////////
        //// Stationary  ///////////////////////////
        if (Input.GetKeyDown (KeyCode.Q)) 
		{
			this.BlueCharacter.Movement = null;
		}
        ////    Seek    ///////////////////////////
        else if (Input.GetKeyDown (KeyCode.W)) 
		{
            this.BlueCharacter.Movement = new KinematicSeek
			{
                Target = this.GreenCharacter.StaticData,
			    MaxSpeed = MAX_SPEED
			};
		}
        ////    Flee    ///////////////////////////
        else if (Input.GetKeyDown (KeyCode.E)) 
		{
            this.BlueCharacter.Movement = new KinematicFlee
			{
                Target = this.GreenCharacter.StaticData,
				MaxSpeed = MAX_SPEED
			};
		}
        ////   Arrive   ///////////////////////////
        else if (Input.GetKeyDown (KeyCode.R)) 
		{
            this.BlueCharacter.Movement = new KinematicArrive
			{
                Target = this.GreenCharacter.StaticData,
                MaxSpeed = MAX_SPEED,
                TimeToTarget = TIME_TO_TARGET,
                Radius = RADIUS
			};
		}
        ////   Wander   ///////////////////////////
        else if (Input.GetKeyDown (KeyCode.T)) 
		{
            this.BlueCharacter.Movement = new KinematicWander
			{
				MaxRotation = MAX_ROTATION,
				MaxSpeed = MAX_SPEED
			};
		}

        // Green Movement ///////////////////////////
        //// Stationary  ///////////////////////////
        if (Input.GetKeyDown(KeyCode.A))
        {
            this.GreenCharacter.Movement = null;
        }
        ////    Seek    ///////////////////////////
        else if (Input.GetKeyDown(KeyCode.S))
        {
            this.GreenCharacter.Movement = new KinematicSeek
            {
                Target = this.BlueCharacter.StaticData,
                MaxSpeed = MAX_SPEED
            };
        }
        ////    Flee    ///////////////////////////
        else if (Input.GetKeyDown(KeyCode.D))
        {
            this.GreenCharacter.Movement = new KinematicFlee
            {
                Target = this.BlueCharacter.StaticData,
                MaxSpeed = MAX_SPEED
            };
        }
        ////   Arrive   ///////////////////////////
        else if (Input.GetKeyDown(KeyCode.F))
        {
            this.GreenCharacter.Movement = new KinematicArrive
            {
                Target = this.BlueCharacter.StaticData,
                MaxSpeed = MAX_SPEED,
                TimeToTarget = TIME_TO_TARGET,
                Radius = RADIUS
            };
        }
        ////   Wander   ///////////////////////////
        else if (Input.GetKeyDown(KeyCode.G))
        {
            this.GreenCharacter.Movement = new KinematicWander
            {
                MaxRotation = MAX_ROTATION,
                MaxSpeed = MAX_SPEED
            };
        }

        this.UpdateMovingGameObject(this.BlueCharacter);
        this.UpdateMovingGameObject(this.GreenCharacter);

        this.UpdateMovementText();
	}

    private void UpdateMovingGameObject(KinematicCharacter movingCharacter)
    {
        if (movingCharacter.Movement != null)
        {
            movingCharacter.Update();
            movingCharacter.StaticData.ApplyWorldLimit(X_WORLD_SIZE,Z_WORLD_SIZE);
            movingCharacter.GameObject.transform.position = movingCharacter.StaticData.position;
        }
    }

    private void UpdateMovementText()
    {
        if (this.GreenCharacter.Movement == null)
        {
            this.GreenMovementText.text = "Green Movement: Stationary";
        }
        else
        {
            this.GreenMovementText.text = "Green Movement: " + this.GreenCharacter.Movement.Name;
        }

        if (this.BlueCharacter.Movement == null)
        {
            this.BlueMovementText.text = "Blue Movement: Stationary";
        }
        else
        {
            this.BlueMovementText.text = "Blue Movement: " + this.BlueCharacter.Movement.Name;
        }
    }
}
