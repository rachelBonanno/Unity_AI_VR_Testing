using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockUnit : MonoBehaviour {
    [SerializeField] private float FOVAngle;
    [SerializeField] private smoothDamp;
    private List<FlockUnit> cohesionNeighbours = new List<FlockUnit>();
    private Flock assignedFlock;
    private Vector3 currentVelocity;
    private float speed;
    public Transform MyTransform { get; set; }

    public void Awake() {
        myTransform = myTransform;
    }

    public void AssignFlock(Flock flock) {
        assignedFlock = flock;
    }

    public void InitializeSpeed(float speed) {
        this.speed = speed;
    }

    public void MoveUnit() {
        FindNeighbours();
        var cohesionVector = CalculateCohesionVector();
        var moveVector = Vector3.SmoothDamp(myTransform.forward, cohesionVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized * speed;
        myTransform.forward = moveVector;
        myTransform.position += moveVector * Time.deltaTime;
    }

    private FindNeighbours() {
        cohesionNeighbours.Clear();
        var allUnits = assignedFlock.allUnits;
        for (int i = 0; i < allUnits.Length; i++) {
            var currentUnit = allUnits[i];
            if (currentUnit != this) {
                float currentNeighbourDistanceSqr = Vector3.SqrMagnitude(currentUnit.transform.position - transform.position);
                if (currentNeighbourDistanceSqr <= assignedFlock.cohesionDistance * assignedFlock.cohesionDistance) {
                    cohesionNeighbours.Add(currentUnit);
                }
            }
        }
    }

    private Vector3 CalculateCohesionVector() {
        var cohesionVector = Vector3.zero;
        if (cohesionNeighbours.Count == 0) {
            return cohesionVector;
        }
        int neighboursInFOV = 0;
        for (int i = 0; i < cohesionNeighbours.Count; i++) {
            if (IsInFOV(cohesionNeighbours[i].myTransform.position)) {
                neighboursInFOV++;
                cohesionVector += cohesionNeighbours[i].myTransform.position;
            }
        }

        if (neighboursInFOV == 0) {
            return cohesionVector;
        }

        cohesionVector /= neighboursInFOV;
        cohesionVector -= myTransform.position;
        cohesionVector = Vector3.Normalize(cohesionVector);
        return cohesionVector;
    }

    private bool IsInFOV(Vector3 position) {
        return Vector3.Angle(myTransform.forward, position - myTransform.position) <= FOVAngle;
    }
}
