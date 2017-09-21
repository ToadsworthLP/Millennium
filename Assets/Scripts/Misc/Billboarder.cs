using UnityEngine;

public class Billboarder : MonoBehaviour {
    [SerializeField]
    float angleBetween;
    [SerializeField]
    float angleBetween2;
    public float zScale;
    public float frontBack;
    public float camSide;
    float camAngle;
	float thisAngle;
    public float spawnSide;
	float parentAngle;

    public bool localSpace;

    public bool turn;
    float turnSpd;
    public float dir;
	void Start () {
	
	}
    
    void FixedUpdate() {
        dir = Mathf.Repeat(dir, 360);
        camAngle = Camera.main.transform.eulerAngles.y;
        parentAngle = this.transform.parent.eulerAngles.y;
        
        angleBetween = Mathf.Abs(Mathf.DeltaAngle(camAngle, parentAngle));

        angleBetween2 = PointsToAngle(Camera.main.transform.position, this.transform.position);

        #region needlessly complicated
        //thisAngle = this.transform.localEulerAngles.y;
        //zScale = Mathf.Clamp((Mathf.Sin((thisAngle + angleBetween2) * Mathf.Deg2Rad) * 1000000), -1, 1);//Mathf.Round(Mathf.Cos(thisAngle*Mathf.Deg2Rad + 45)*1.0001f);

        /*
         * zScale = Mathf.Clamp((Mathf.Sin((angleBetween2 + this.transform.eulerAngles.y) * Mathf.Deg2Rad) * 1000000), -1, 1);
        
        //Camera.main.transform.LookAt (this.transform);
        frontBack = Mathf.Clamp((Mathf.Abs(Mathf.Cos(0.5f * (thisAngle + angleBetween2) * Mathf.Deg2Rad)) - 0.5f) * 2000000, -1, 1);
        */
        #endregion
        if (angleBetween<90)
            camSide = 1;
        else camSide = -1;
        float zSA = Mathf.Repeat(angleBetween2 + this.transform.eulerAngles.y, 360) - 180;
        if (zSA > 0 && zSA < 180)
        {
            zScale = -1;
        }
        if (zSA < 0 && zSA > -180)
        {
            zScale = 1;
        }
        
        if (turn)
        {
            //transform.eulerAngles = Vector3.Slerp(transform.localEulerAngles, new Vector3(transform.localRotation.x, dir, transform.localEulerAngles.z), turnSpd);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(transform.localRotation.x, dir, transform.localRotation.z)), Time.fixedDeltaTime * 9);//0.15f
        }
        this.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, spawnSide * zScale);
        #region junk
        //print(zSA);
        /*if (angleBetween < 90)
			this.transform.eulerAngles = new Vector3 (0, Mathf.MoveTowardsAngle(this.transform.eulerAngles.y, camAngle + 180, 15), 0);
		else
			this.transform.eulerAngles = new Vector3 (0, Mathf.MoveTowardsAngle(this.transform.eulerAngles.y, camAngle, 15), 0);
            */


        /*switch (localSpace)
        {
            case true:

                angleBetween2 = Mathf.DeltaAngle(camAngle, parentAngle);

                thisAngle = this.transform.localEulerAngles.y;
                zScale = Mathf.Clamp((Mathf.Cos((thisAngle + angleBetween2) * Mathf.Deg2Rad) * 1000000), -1, 1);//Mathf.Round(Mathf.Cos(thisAngle*Mathf.Deg2Rad + 45)*1.0001f);
                this.transform.localScale = new Vector3(1, 1, spawnSide * zScale);
                //Camera.main.transform.LookAt (this.transform);
                frontBack = Mathf.Clamp((Mathf.Abs(Mathf.Sin(0.5f * (thisAngle + angleBetween2) * Mathf.Deg2Rad)) - 0.5f) * 2000000, -1, 1);
                break;

            case false:

                angleBetween2 = Mathf.DeltaAngle(camAngle, this.transform.eulerAngles.y);

                thisAngle = this.transform.eulerAngles.y;
                zScale = Mathf.Clamp((Mathf.Cos((angleBetween2-180) * Mathf.Deg2Rad) * 1000000), -1, 1);
                this.transform.localScale = new Vector3(1, 1, spawnSide * zScale);
                frontBack = Mathf.Clamp((Mathf.Abs(Mathf.Sin((0.5f * (angleBetween2+90)) * Mathf.Deg2Rad)) - 0.5f) * 2000000, -1, 1);
                break;
        }*/

        //this.transform.position = Mathf.Clamp()
        /*camAngle = Camera.main.transform.rotation.y;
        //thisAngle = this.transform.rotation.y;
        parentAngle = this.transform.parent.transform.rotation;

        angleBetween = Mathf.Abs(parentAngle.y - camAngle);// Quaternion.Angle (parentAngle, new Quaternion (0, camAngle, 0, 0));

        //Mathf.LerpAngle (thisAngle, camAngle, 0.25f);
        if (angleBetween <= 0.5f) {
            this.transform.rotation = new Quaternion (0, camAngle + 0.5f, 0, 0); print ("A");
        }
        else {
            this.transform.rotation = new Quaternion (0, camAngle, 0, 0); print ("B");
        }

        //Camera.main.transform.LookAt (this.transform.parent.transform.position);
        print (angleBetween);
        //Vector2.Angle (Vector2 (0, camAngle), Vector2 (0, thisAngle));
        //angleBetween = Quaternion.Angle(new Quaternion(0,0,0,0), new Quaternion(0,0,0,0));//Vector2.Angle(new Vector2(0,camAngle),new Vector2(0,thisAngle));
        //side = Mathf.CeilToInt (Mathf.Clamp01 (angleBetween));
        //this.transform.eulerAngles = Quaternion.Slerp(new Vector3(0,0,thisAngle,0), )*/
        #endregion
    }

    public static float PointsToAngle(Vector3 v3a, Vector3 v3b) {
        return Mathf.Atan2(v3b.z - v3a.z, v3b.x - v3a.x) * 180 / Mathf.PI;
    }
}
