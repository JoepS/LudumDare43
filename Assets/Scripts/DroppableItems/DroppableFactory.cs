using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppableFactory : MonoBehaviour {

    public static DroppableFactory instance;

    [SerializeField] Sprite _healthOrb;
    [SerializeField] Sprite _magicOrb;
    [SerializeField] Sprite _experienceOrb;
    [SerializeField] Sprite _goldOrb;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else{
            Destroy(this.gameObject);
        }
    }

    [SerializeField] GameObject _droppablePrefab;

    public IDroppable CreateDroppable(float amount, float respawnTime, ItemType type, Vector3 location)
    {
        GameObject go = GameObject.Instantiate(_droppablePrefab, location, Quaternion.identity);
        switch (type)
        {
            case ItemType.Health:
                go.GetComponentInChildren<SpriteRenderer>().sprite = _healthOrb;
                break;
            case ItemType.Magic:
                go.GetComponentInChildren<SpriteRenderer>().sprite = _magicOrb;
                break;
            case ItemType.Experience:
                go.GetComponentInChildren<SpriteRenderer>().sprite = _experienceOrb;
                break;
            case ItemType.Gold:
                go.GetComponentInChildren<SpriteRenderer>().sprite = _goldOrb;
                break;
        }
        IDroppable droppable = go.GetComponent<IDroppable>();
        droppable.SetData(amount, respawnTime, type);
        return droppable;
    }
}
