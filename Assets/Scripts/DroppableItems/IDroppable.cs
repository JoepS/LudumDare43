using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDroppable : MonoBehaviour {

    [SerializeField] protected float _amount;
    [SerializeField] protected float _despawnTime;
    [SerializeField] protected ItemType _type;

    Rigidbody2D _rigidbody;

    [SerializeField] AudioSource _orbPickupSource;
    [SerializeField] AudioSource _goldPickupSource;

    void Awake()
    {
        _rigidbody = this.GetComponent<Rigidbody2D>();
    }

    public void SetData(float amount, float despawnTime, ItemType type)
    {
        _amount = amount;
        _despawnTime = despawnTime;
        _type = type;
    }

    public void Drop() {
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f));
        _rigidbody.AddForce(dir * 5, ForceMode2D.Impulse);
    }
    public void Despawn()
    {
        Destroy(this.gameObject);
    }
    bool _pickedUp = false;
    public void Pickup(GameObject go)
    {
        if (!_pickedUp)
        {
            _pickedUp = true;
            StartCoroutine(PickupAnimation(go));
        }
    }

    IEnumerator PickupAnimation(GameObject go)
    {
        if (this._type == ItemType.Gold)
            _goldPickupSource.Play();
        else
            _orbPickupSource.Play();
        while (Vector3.Distance(this.transform.localPosition, go.transform.position) > 0.15f)
        {
            this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, go.transform.position, Time.deltaTime * 20);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        EntityStats playerStats = go.GetComponent<PlayerMovement>().GetStats();
        playerStats.AddDroppable(_amount, _type);
        Despawn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            Pickup(collision.gameObject);
        }
    }

}

public enum ItemType
{
    Health,
    Magic,
    Gold,
    Experience
        
}
