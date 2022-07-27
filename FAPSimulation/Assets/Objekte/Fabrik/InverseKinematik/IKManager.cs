using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKManager : MonoBehaviour
{
    public RobotJoint[] Joints;
    public float SamplingDistance;
    public float LearningRate;
    public float DistanceThreshold;
    public GameObject Target;
    public GameObject[] Targets;

    public GameObject Gripper;

    public float[] Angles;
    public int targetCount=0;
    public float targetChangeIntervall = 3;
    
    public enum ControlModes
    {
        ContinousPath, HandFollow
    }
    public ControlModes controlMode;

    void Start()
    {
        // InvokeRepeating("cycleTargetArray",5, targetChangeIntervall);
    }
    public void SetControlMode (int mode)
    {
        if (mode == 0) controlMode = ControlModes.ContinousPath;
        if (mode == 1) controlMode = ControlModes.HandFollow;
        for (int i = 0; i < Targets.Length; i++)
        {
            if (Vector3.SqrMagnitude(Targets[i].transform.position- Gripper.transform.position)> Vector3.SqrMagnitude(Targets[targetCount].transform.position- Gripper.transform.position))
            {
                targetCount = i;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        


        //should i get the Angles from Joints[] here to save in float[] Angles?
        for (int i = 0; i < Joints.Length; i++)
        {

            if (Joints[i].RotationAxis.x != 0)
            {
                Angles[i] = Joints[i].transform.localEulerAngles.x + Joints[i].StartRotationOffset.x;
            }
            if (Joints[i].RotationAxis.y != 0)
            {
                Angles[i] = Joints[i].transform.localEulerAngles.y + Joints[i].StartRotationOffset.y;
            }
            if (Joints[i].RotationAxis.z != 0)
            {
                Angles[i] = Joints[i].transform.localEulerAngles.z + Joints[i].StartRotationOffset.z;
            }
            


        }
        // Debug.Log(winkel);
        if (controlMode == ControlModes.HandFollow)
        {
            // do translation towards target (Linearachse)
            TranslateTowardsTarget(Target.transform.position, Angles);

            // now do inverse kinematics (Robotergelenke)
            InverseKinematics(Target.transform.position, Angles);
            SetTransforms();
        }
        else if (controlMode == ControlModes.ContinousPath)
        {
            InverseKinematics(Targets[targetCount].transform.position, Angles);
            SetTransforms();
        }
        

        //need to return Android ActivityIndicatorStyle transform the position of the Joint Gameobjects with that

    }

    public void SetTransforms()
    {
        for (int i = 0; i < Joints.Length; i++)
        {
            if (Joints[i].RotationAxis.x != 0)
            {
                Joints[i].transform.localEulerAngles = new Vector3(Angles[i], 0, 0);
            }
            if (Joints[i].RotationAxis.y != 0)
            {
                Joints[i].transform.localEulerAngles = new Vector3(0, Angles[i], 0);
            }
             if (Joints[i].RotationAxis.z != 0)
            {
                Joints[i].transform.localEulerAngles = new Vector3(0, 0, Angles[i]);
            }
        }
    }
    public void TranslateTowardsTarget(Vector3 target, float[] angles)
    {
        if (DistanceFromTarget(target, angles) < DistanceThreshold)
        {
            return;
        }
        for (int i = 0; i < Joints.Length; i++)
        {
            if (Joints[i].TranslationAxis.x != 0)
            {
                float currDistance = Vector3.SqrMagnitude(target - Joints[i].transform.GetChild(0).position);
                float checkDistance1 = Vector3.SqrMagnitude(target - Joints[i].transform.GetChild(0).position + Joints[i].transform.GetChild(0).right * Time.deltaTime);
                float checkDistance2 = Vector3.SqrMagnitude(target - Joints[i].transform.GetChild(0).position - Joints[i].transform.GetChild(0).right * Time.deltaTime);
                
                if (currDistance > checkDistance1 && Joints[i].TranslationDelta > Joints[i].MinTranslation)
                {
                    Joints[i].transform.GetChild(0).Translate(-Vector3.right * Time.deltaTime * Joints[i].translationSpeed, Space.Self);
                    Joints[i].TranslationDelta -= Time.deltaTime * Joints[i].translationSpeed;
                }
                else if (currDistance > checkDistance2 && Joints[i].TranslationDelta < Joints[i].MaxTranslation)
                {
                    Joints[i].transform.GetChild(0).Translate(Vector3.right * Time.deltaTime * Joints[i].translationSpeed, Space.Self);
                    Joints[i].TranslationDelta += Time.deltaTime * 0.5f * Joints[i].translationSpeed;
                }
            }
            else if (Joints[i].TranslationAxis.y != 0)
            {
                float currDistance = Vector3.SqrMagnitude(target - Joints[i].transform.GetChild(0).position);
                float checkDistance1 = Vector3.SqrMagnitude(target - Joints[i].transform.GetChild(0).position + Joints[i].transform.GetChild(0).up * Time.deltaTime);
                float checkDistance2 = Vector3.SqrMagnitude(target - Joints[i].transform.GetChild(0).position - Joints[i].transform.GetChild(0).up * Time.deltaTime);

                if (currDistance > checkDistance1 && Joints[i].TranslationDelta > Joints[i].MinTranslation)
                {
                    Joints[i].transform.GetChild(0).Translate(-Vector3.up * Time.deltaTime * Joints[i].translationSpeed, Space.Self);
                    Joints[i].TranslationDelta -= Time.deltaTime * Joints[i].translationSpeed;
                }
                else if (currDistance > checkDistance2 && Joints[i].TranslationDelta < Joints[i].MaxTranslation)
                {
                    Joints[i].transform.GetChild(0).Translate(Vector3.up * Time.deltaTime * Joints[i].translationSpeed, Space.Self);
                    Joints[i].TranslationDelta += Time.deltaTime * 0.5f * Joints[i].translationSpeed;
                }
            }
            else if (Joints[i].TranslationAxis.z != 0)
            {
                float currDistance = Vector3.SqrMagnitude(target - Joints[i].transform.GetChild(0).position);
                float checkDistance1 = Vector3.SqrMagnitude(target - Joints[i].transform.GetChild(0).position + Joints[i].transform.GetChild(0).forward * Time.deltaTime);
                float checkDistance2 = Vector3.SqrMagnitude(target - Joints[i].transform.GetChild(0).position - Joints[i].transform.GetChild(0).forward * Time.deltaTime);

                if (currDistance > checkDistance1 && Joints[i].TranslationDelta > Joints[i].MinTranslation)
                {
                    Joints[i].transform.GetChild(0).Translate(-Vector3.forward * Time.deltaTime * Joints[i].translationSpeed, Space.Self);
                    Joints[i].TranslationDelta -= Time.deltaTime * Joints[i].translationSpeed;
                }
                else if (currDistance > checkDistance2 && Joints[i].TranslationDelta < Joints[i].MaxTranslation)
                {
                    Joints[i].transform.GetChild(0).Translate(Vector3.forward * Time.deltaTime * Joints[i].translationSpeed, Space.Self);
                    Joints[i].TranslationDelta += Time.deltaTime * 0.5f * Joints[i].translationSpeed;
                }
            }

        }
    }


    public Vector3 ForwardKinematics(float[] angles)
    {
        /*
        Vector3 prevPoint = Joints[0].transform.position;
        Quaternion rotation = Quaternion.identity;
        for (int i = 1; i < Joints.Length; i++)
        {
            // Rotates around a new axis
            rotation *= Quaternion.AngleAxis(angles[i - 1], -Joints[i - 1].Axis);
            Vector3 nextPoint = prevPoint + rotation * Joints[i].StartOffset;
            Debug.DrawLine(prevPoint, nextPoint, Color.red, Time.deltaTime);
            prevPoint = nextPoint;
        }
        // Debug.DrawLine(Vector3.zero, prevPoint);
        return prevPoint;
        */
        // Ich stell einfach die Winkel einzeln ein und schau am Ende auf die Distanz:
        SetTransforms();
        return Joints[Joints.Length - 1].transform.position;
    }
    public void InverseKinematics(Vector3 target, float[] angles)
    {
        if (DistanceFromTarget(target, angles) < DistanceThreshold)
        {
            return;
        }

        for (int i = Joints.Length - 1; i >= 0; i--)
        {
            // Gradient descent
            // Update : Solution -= LearningRate * Gradient
            float gradient = PartialGradient(target, angles, i);
            
            angles[i] -= LearningRate * gradient;
            if (angles[i] > 180) angles[i] -= 360;
            // Clamp
            if (Joints[i].hasConstraint)
            {
                Debug.Log("Angle: " + angles[i]);
                
                if (angles[i] < Joints[i].MinAngle) { angles[i] = Joints[i].MinAngle; Debug.Log("Zu gering: " + angles[i] + " min: " + Joints[i].MinAngle); }
                else if (angles[i] > Joints[i].MaxAngle) { angles[i] = Joints[i].MaxAngle; Debug.Log("Zu hoch: " + angles[i] +" max: " + Joints[i].MaxAngle); }

            }

            // Early termination
            if (DistanceFromTarget(target, angles) < DistanceThreshold)
            {
                if (targetCount!=1|| targetCount != 3) Invoke("cycleTargetArray", 3.0f);
                else
                {
                    cycleTargetArray();
                }
                return;
            }
               
        }
    }


    public float PartialGradient(Vector3 target, float[] angles, int i)
    {
        // Saves the angle,
        // it will be restored later
        float angle = angles[i];

        // Gradient : [F(x+SamplingDistance) - F(x)] / h
        float f_x = DistanceFromTarget(target, angles);

        angles[i] += SamplingDistance;
        float f_x_plus_d = DistanceFromTarget(target, angles);

        float gradient = (f_x_plus_d - f_x) / SamplingDistance;

        // Restores
        angles[i] = angle;

        return gradient;
    }

    public float DistanceFromTarget(Vector3 target, float[] angles)
    {
        Vector3 point = ForwardKinematics(angles);

        
        // Debug.DrawLine(point, target, Color.blue);
        Debug.DrawLine(Gripper.transform.position, target, Color.blue);
        // Debug.Log(Vector3.Distance(Gripper.transform.position, target));
        // return Vector3.Distance(Gripper.transform.position, target);
        return Vector3.Distance(point, target);
    }

    public void cycleTargetArray()
    {
        if (targetCount<Targets.Length-1)
        {
            targetCount++;
        }
        else
        {
            targetCount = 0;
        }
    } 
}
