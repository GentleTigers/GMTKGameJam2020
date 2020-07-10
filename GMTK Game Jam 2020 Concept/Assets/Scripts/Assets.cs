﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assets : MonoBehaviour {

    public static Assets instance;

    public Sprite healthySprite;
    public Sprite infectiousSprite;
    public Sprite infectedSprite;
    public Sprite doctorSprite;
    public Sprite imuneSprite;


    private void Awake() {
        instance = this;
    }

}
