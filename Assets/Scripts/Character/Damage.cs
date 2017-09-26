using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DAMAGE_ATTRIBUTE {
    INSTANT_DEATH,
    CAPTURE
}

public class Damage : MonoBehaviour {
    public float attackPoint;
    public DAMAGE_ATTRIBUTE[] attribute;
}
