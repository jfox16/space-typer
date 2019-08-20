using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 1.0f;
    [SerializeField] List<GameObject> backgroundObjects = new List<GameObject>();
    [SerializeField] Vector2 backgroundDimensions = new Vector2(25f, 20f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject _bgObj in backgroundObjects) {
            _bgObj.transform.position += Vector3.down * scrollSpeed * Time.deltaTime;
            if (_bgObj.transform.position.y < -backgroundDimensions.y) {
                float _resetY = (backgroundObjects.Count-1) * backgroundDimensions.y;
                float zPos = _bgObj.transform.position.z;
                _bgObj.transform.position = (Vector3.up * _resetY) + new Vector3(0, 0, zPos);
            }
        }
    }
}
