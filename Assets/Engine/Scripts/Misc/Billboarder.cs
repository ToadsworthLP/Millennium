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
    public float spawnSide;
	float parentAngle;

    public bool localSpace;

    public bool turn;
    public float dir;
    
    void Update() {
        dir = Mathf.Repeat(dir, 360);
        camAngle = Camera.main.transform.eulerAngles.y;
        parentAngle = this.transform.parent.eulerAngles.y;
        
        angleBetween = Mathf.Abs(Mathf.DeltaAngle(camAngle, parentAngle));

        angleBetween2 = PointsToAngle(Camera.main.transform.position, transform.position);

        if (angleBetween<90){
            camSide = 1;
        }else{
            camSide = -1;
        }

        float zSA = Mathf.Repeat(angleBetween2 + transform.eulerAngles.y, 360) - 180;

        if (zSA > 0 && zSA < 180){
            zScale = -1;
        }

        if (zSA < 0 && zSA > -180){
            zScale = 1;
        }
        
        if (turn){
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(transform.localRotation.x, dir, transform.localRotation.z)), Time.fixedDeltaTime * 9);//0.15f
        }
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, spawnSide * zScale);
    }

    public static float PointsToAngle(Vector3 v3a, Vector3 v3b) {
        return Mathf.Atan2(v3b.z - v3a.z, v3b.x - v3a.x) * 180 / Mathf.PI;
    }
}
