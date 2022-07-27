using UnityEngine;

public class RobotJoint : MonoBehaviour
{
    public Vector3 RotationAxis;
    public Vector3 TranslationAxis;

    public Vector3 StartRotationOffset;
    public Vector3 StartPositionOffset;
    public bool hasConstraint;
    public float MinAngle;
    public float MaxAngle;

    public float MinTranslation;
    public float MaxTranslation;
    public float TranslationDelta;
    public float translationSpeed;
  

    void Awake()
    {
        StartPositionOffset = transform.localPosition;
        StartRotationOffset = transform.localEulerAngles;
    }

}