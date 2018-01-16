﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Save
{
    public List<int> livingTargetPosition = new List<int>();
    public List<int> livingMonsterTypes = new List<int>();
    public int shootNum = 0;
    public int score = 0;
}