using System.Collections;
using System.Collections.Generic;
using Amazon;
using Amazon.S3;
using UnityEngine;

public class TestMono : MonoBehaviour
{

    private void Awake()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        new AmazonS3Client("AKIAWKQUMIXG5NXU6KWQ", "wKn9En64R85rlCz28wOenQilPA7eC6HdPf8Nq+C7", RegionEndpoint.USWest2);
        Debug.LogWarning("TESTING");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
