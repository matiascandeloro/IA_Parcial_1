using UnityEngine;

public class Bait : Agent
{
    [SerializeField] private float _lifeTime = 5f;
    private float _currentLifeTime;
    [SerializeField] private float _eatDuration = 2f;
    [SerializeField] private bool _isBeingEat = false;

    // Update is called once per frame
    void Update()
    {
        if (_isBeingEat)
        {
            _eatDuration -= Time.deltaTime;
            if (_eatDuration <= 0)
            {
                DestroyObject();
            }
        }
        else
        {
            _lifeTime -= Time.deltaTime;
            if (_lifeTime <= 0)
            {
                DestroyObject();
            }
        }
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
