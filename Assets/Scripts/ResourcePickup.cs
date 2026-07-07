using System.Collections;
using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    [SerializeField] private ResourceType type;
    [SerializeField] private int amount = 1;
    //[SerializeField] private GameObject visuals;

    private Collider pickupCollider;
    private readonly WaitForSeconds destroyDelay = new(1f);

    private void Awake()
    {
        pickupCollider = GetComponent<Collider>();
    }

    //TODO The machine can spawn pickups with different amounts dynamically.
    public void Initialise(ResourceType type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerInventory inventory))
            return;

        pickupCollider.enabled = false;

        //if (visuals != null)
        //    visuals.SetActive(false);

        inventory.AddResource(type, amount);

        StartCoroutine(DestroyObjectWithDelay());
    }

    private IEnumerator DestroyObjectWithDelay()
    {
        yield return destroyDelay;
        // TODO object pool
        Destroy(gameObject);
    }
}