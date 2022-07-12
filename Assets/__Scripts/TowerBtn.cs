using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBtn : MonoBehaviour
{
    [SerializeField]
    TowerControl _towerObject;
    [SerializeField]
    Sprite _towerSprite;

    public int towerPrice;

    public TowerControl towerObject
    {
        get
        {
            return _towerObject;
        }
    }

    public Sprite towerSprite
    {
        get
        {
            return _towerSprite;
        }
    }
}
