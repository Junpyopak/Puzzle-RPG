using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gotittle : MonoBehaviour
{
    public void GoTitle()
    {
        Scenemgr.Instance.ChangeScene(eSCENE.TITLE);
    }
}
