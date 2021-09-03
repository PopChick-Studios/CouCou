using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanPlayerFish : MonoBehaviour
{
    public bool playerCanFish = false;
    public bool playerHasRod = false;

    public InventoryList playerInventory;

    private GameObject boundary;
    public Vector3 collisionPoint;
    public Vector3 collisionNormal;
    private Fishing fishing;

    private void Awake()
    {
        fishing = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Fishing>();
    }

    private void Start()
    {
        foreach (InventoryList.ItemInventory item in playerInventory.itemInventory)
        {
            if (item.itemAttribute == ItemsDatabase.ItemAttribute.Fishing)
            {
                playerHasRod = true;
                break;
            }
        }
    }

    private void Update()
    {
        if (boundary == null)
        {
            return;
        }
        if (Vector3.Distance(collisionPoint, gameObject.transform.position) < 6 && playerHasRod)
        {
            playerCanFish = true;
        }
        else
        {
            boundary = null;
            playerCanFish = false;
            fishing.DestroyUI();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Boundary") && boundary == null && playerHasRod && !fishing.isFishing)
        {
            Debug.Log("Colliding");
            boundary = hit.gameObject;
            collisionPoint = hit.point;
            collisionNormal = hit.normal;
            fishing.InstantiateUI();
        }
    }
}
