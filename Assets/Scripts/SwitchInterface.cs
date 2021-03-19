using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface iSwitch
{
    event Action switchEnabled;
    event Action switchDisabled;
}