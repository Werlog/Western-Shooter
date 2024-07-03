using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHandler : MonoBehaviour
{
    [SerializeField] private Transform weaponHolder;

    public void EquipHoldable(HoldableObject holdable)
    {
        DestroyHeldItems();
        Instantiate(holdable.firstPersonPrefab, weaponHolder);
    }

    private void DestroyHeldItems()
    {
        foreach (Transform child in weaponHolder)
        {
            Destroy(child.gameObject);
        }
    }
}
