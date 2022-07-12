using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class TowerManager : Loader<TowerManager>
{
    public TowerBtn towerBtnPressed;
    SpriteRenderer spriteRend;
    public List<TowerControl> towers;
    public List<Collider2D> towerFilledPlacements;
    public Collider2D towerPlacement;
    
    // Start is called before the first frame update
    void Start()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        towers = new List<TowerControl>();
        towerFilledPlacements = new List<Collider2D>();
        spriteRend.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePostion, Vector2.zero);

            if ((hit.collider != null) && (hit.collider.tag == "TowerPlacement"))
            {
                towerPlacement = hit.collider;
                towerPlacement.tag = "TowerPlacementFilled";
                AddPlacementToList(towerPlacement);
                PlaceTower(hit);
            }
        }
        if(spriteRend.enabled)
        {
            FollowMouse();
        }
    }
    void AddPlacementToList(Collider2D placement)
    {
        towerFilledPlacements.Add(placement);
    }
    void AddTowerToList(TowerControl tower)
    {
        towers.Add(tower);
    }
    public void ClearTowerPlacements()
    {
        foreach(Collider2D colld in towerFilledPlacements)
        {
            colld.tag = "TowerPlacement";
        }
        towerFilledPlacements.Clear();
    }
    public void ClearTowerList()
    {
        foreach (TowerControl tower in towers)
        {
            Destroy(tower.gameObject);
        }
        towers.Clear();
    }
    void PlaceTower(RaycastHit2D hit)
    {
        /*if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Clicked on the UI");
        }*/
        //если клик мыши не на элементе GUI + режим построения башен -> создать объект в точке
        if (!EventSystem.current.IsPointerOverGameObject() && towerBtnPressed != null)
        {
            TowerControl tower = Instantiate(towerBtnPressed.towerObject);
            tower.transform.position = hit.transform.position;
            BuyTower(towerBtnPressed.towerPrice);
            Manager.instance.audioSource.PlayOneShot(SoundManager.instance.towerBuilt);
            AddTowerToList(tower);
            DisableDrag();
        }
    }
    public void BuyTower(int towerPrice)
    {
        Manager.instance.SubtractMoney(towerPrice);
    }
    public void SelectedTower(TowerBtn towerSelected)
    {
        if (towerSelected.towerPrice <= Manager.instance.money)
        {
            towerBtnPressed = towerSelected;
            EnableDrag();
        }
    }
    public void FollowMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }
    public void EnableDrag() 
    {
        spriteRend.enabled = true;
        spriteRend.sprite = towerBtnPressed.towerSprite;
    }
    public void DisableDrag()
    {
        spriteRend.enabled = false;
    }
}
